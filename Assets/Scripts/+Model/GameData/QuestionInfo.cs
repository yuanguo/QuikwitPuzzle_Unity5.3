using UnityEngine;
using System.Collections;

using System.Collections.Generic;


public class CollectQuestions : List<QuestionInfo> { }


public enum QuestionType
{
	none = 0,
	regular_question = 1,
	bonus_question = 2,
	prize_question = 3,
}

public class Answer : CEObject
{
	private static int answerInstance = 0;

	public string answer = string.Empty;
	public bool isCorrect = false;

	public Answer()
	{
		answerInstance++;
	}

	~Answer()
	{
		answerInstance--;
	}

	public override void testValue()
	{
		base.testValue();
// 		return;
// 
// 		answer = string.Format("answer is {0}", answerInstance);
// 		isCorrect = answerInstance % 4 == 0;
	}
}

public class CollectAnswers : List<Answer>{};

public class QuestionInfo : CEObject
{
	public const int MAX_ANSWERS = 4;

	private static int questionInstance = 0;

	public QuestionType questionType
	{
		get {
			if (type == string.Empty)
				m_type = QuestionType.none;
			else if (type == "regular_question")
				m_type = QuestionType.regular_question;
			else if (type == "bonus_question")
				m_type = QuestionType.bonus_question;
			else if (type == "prize_question")
				m_type = QuestionType.prize_question;

			return m_type;
		}
	}

	public string id = string.Empty;
	public string question = string.Empty;
	public string publishState = string.Empty;
	public string type = string.Empty;
	public string updateDate = string.Empty;
	public string level = string.Empty;
	public string category = string.Empty;

	public bool isPassed = false;

	private QuestionType m_type = QuestionType.regular_question;

	public CollectAnswers answers = new CollectAnswers();

	public QuestionInfo()
	{
		questionInstance++;
	}

	~QuestionInfo()
	{
		questionInstance--;
	}

	public override void InitObject()
	{
		question = string.Empty;

		if (answers == null)
			answers = new CollectAnswers();

		answers.Clear();

		base.InitObject();
	}

	public override void testValue()
	{
		base.testValue();

		return;

// 		if (questionType == QuestionType.regular_question)
// 			question = string.Format("Normal Question{0}, OK?", questionInstance);
// 		else
// 			question = string.Format("Special Question{0}, OK?", questionInstance);
// 
// 		answers.Clear();
// 
// 		for (int nIdx = 0; nIdx < MAX_ANSWERS; nIdx++)
// 		{
// 			Answer tempAnswer = new Answer();
// 			answers.Add(tempAnswer);
// 		}
	}

	public void pass(bool _passingFlag)
	{
		isPassed = _passingFlag;
	}

	public QuestionInfo copy ()
	{
		return (QuestionInfo)this.MemberwiseClone();
	}
}
