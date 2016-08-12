using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class Message
{
    public static string MSG_TIME_OUT = "Time out!";
    public static string MSG_RECEIVE_FAILED = "Receive Failed !";
    public static string MSG_RECEIVING_ERROR = "Receiving Error !";
    public static string MSG_INVALID_USERNAME = "Invalid user name, please input user name.";
    public static string MSG_INVALID_EMAIL = "Invalid email, please input email address";
    public static string MSG_INVALID_GENDER = "Invalid gender, please input gender";
    public static string MSG_INVALID_PWD = "Invalid Password, please input again password.";

	
	public static string MSG_INVALID_USERID_OR_PWD = "Invalid User id or Password, please input again.";
	public static string MSG_SING_UP_EMAIL_ERROR = "Email must be unique and non-empty";
	public static string MSG_EMAIL_SENT = "Please check your email.";
}
public class MessageBox : CEBehaviour 
{
    public Text m_Content = null;

    private string m_Msg = "";
	// Use this for initialization
	protected override void OnEnable () 
    {
        SetMsg();
	}

    public void Ok()
    {
		rectTransform.SetAsFirstSibling ();
        gameObject.SetActive(false);
    }

    public string Msg
    {
        get { return m_Msg; }
        set 
        {
            if (m_Msg == value)
                return;

            m_Msg = value;
            SetMsg();
        }
    }

    private void SetMsg()
    {
        if (m_Content)
            m_Content.text = m_Msg;
    }
}
