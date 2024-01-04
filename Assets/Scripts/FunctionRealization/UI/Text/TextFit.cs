//------------------------------------------------------------
// Unity中的Text内容有空格导致换行，以及让每行首字符不出现标点符号
// Homepage: https://www.bilibili.com/read/cv13324084/
//------------------------------------------------------------

using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Text))]
public class TextFit : Text
{

    /// <summary>
    /// 标记不换行的空格（换行空格Unicode编码为/u0020，不换行的/u00A0）
    /// </summary>
	public static readonly string Non_breaking_space = "\u00A0";

    /// <summary>
    /// 用于匹配标点符号（正则表达式）
    /// </summary>
    private readonly string strPunctuation = @"\p{P}";

    /// <summary>
    /// 用于存储text组件中的内容
    /// </summary>
    private StringBuilder TempText = null;

    /// <summary>
    /// 用于存储text生成器中的内容
    /// </summary>
    private IList<UILineInfo> TextLine;

    /// <summary>
    /// protected Text mytext;//Text组件
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        this.RegisterDirtyVerticesCallback(OnTextChange);
    }
    /// <summary>
    /// 解决换行空格问题
    /// </summary>
    public void OnTextChange()
    {
        if (this.text.Contains(" "))
        {
            this.text = this.text.Replace(" ", Non_breaking_space);
        }
    }

    protected override void OnPopulateMesh(VertexHelper toFill)
    {
        base.OnPopulateMesh(toFill);
        ClearUpPunctuationMode(this).Forget();
    }
    /// <summary>
    /// 解决首字符出现标点问题
    /// </summary>
    /// <param name="_component"></param>
    /// <returns></returns>
    async UniTaskVoid ClearUpPunctuationMode(Text _component)
    {
        //必须等当前帧跑完后，不然如果在运行text显示的时候对显示内容进行修改会报错
        await UniTask.WaitForEndOfFrame(this);
        //清除上一次添加的换行符号
        _component.text = _component.text.Replace("\n", string.Empty);

        TextLine = _component.cachedTextGenerator.lines;
        //需要改变的字符序号
        int ChangeIndex = -1;
        TempText = new StringBuilder(_component.text);
        for (int i = 1; i < TextLine.Count; i++)
        {
            //首位是否有标点
            bool IsPunctuation = Regex.IsMatch(TempText[TextLine[i].startCharIdx].ToString(), strPunctuation);
            //因为将换行空格都改成不换行空格后需要另外判断下如果首字符是不换行空格那么还是需要调整换行字符的下标
            if (TempText[TextLine[i].startCharIdx].ToString() == Non_breaking_space)
            {
                IsPunctuation = true;
            }

            //没有标点就跳过本次循环
            if (!IsPunctuation)
            {
                continue;
            }
            else
            {
                //有标点时保存当前下标
                ChangeIndex = TextLine[i].startCharIdx;
                //下面这个循环是为了判断当已经提前一个字符后当前这个的首字符还是标点时做的继续提前字符的处理
                while (IsPunctuation)
                {
                    ChangeIndex = ChangeIndex - 1;
                    if (ChangeIndex < 0)
                        break;

                    IsPunctuation = Regex.IsMatch(TempText[ChangeIndex].ToString(), strPunctuation);
                    //因为将换行空格都改成不换行空格后需要另外判断下如果首字符是不换行空格那么还是需要调整换行字符的下标
                    if (TempText[ChangeIndex].ToString() == Non_breaking_space)
                    {
                        IsPunctuation = true;
                    }

                }
                if (ChangeIndex < 0)
                    continue;

                if (TempText[ChangeIndex - 1] != '\n')
                    TempText.Insert(ChangeIndex, "\n");
            }

        }
        _component.text = TempText.ToString();

    }

}