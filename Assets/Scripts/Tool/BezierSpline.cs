using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[SerializeField]
public class Paths : List<Vec3List> { }

public class BezierSpline : CEBehaviour 
{
    [SerializeField]
    private GameObject m_Parent = null;
    [SerializeField]
    private Paths m_Pathes = new Paths();
    [SerializeField]
    private List<Vector3> points = new List<Vector3>();

    [SerializeField]
    private BezierControlPointMode[] modes = new BezierControlPointMode[] 
    {
			BezierControlPointMode.Free,
			BezierControlPointMode.Free,
    };

	[SerializeField]
	private bool loop;
    public GameObject Parent
    {
        get { return m_Parent; }
        set { m_Parent = value; }
    }

    public Paths Pathes
    {
        get { return m_Pathes; }
        set { m_Pathes = value; }
    }

	public bool Loop {
		get {
			return loop;
		}
		set {
			loop = value;
			if (value == true) {
				modes[modes.Length - 1] = modes[0];
				SetControlPoint(0, points[0]);
			}
		}
	}

	public int ControlPointCount {
		get {
			return points == null ? 0 : points.Count;
		}
	}

	public Vector3 GetControlPoint (int index) {
		return points[index];
	}

	public void SetControlPoint (int index, Vector3 point) {
		if (index % 3 == 0) {
			Vector3 delta = point - points[index];
			if (loop) {
				if (index == 0) {
					points[1] += delta;
					points[points.Count - 2] += delta;
					points[points.Count- 1] = point;
				}
				else if (index == points.Count - 1) {
					points[0] = point;
					points[1] += delta;
					points[index - 1] += delta;
				}
				else {
					points[index - 1] += delta;
					points[index + 1] += delta;
				}
			}
			else {
				if (index > 0) {
					points[index - 1] += delta;
				}
				if (index + 1 < points.Count) {
					points[index + 1] += delta;
				}
			}
		}
		points[index] = point;
		EnforceMode(index);
	}

	public void clearPoints()
	{
		points.Clear();
	}

	public BezierControlPointMode GetControlPointMode(int index)
	{
		return modes[(index + 1) / 3];
	}

	public void SetControlPointMode (int index, BezierControlPointMode mode) {
		int modeIndex = (index + 1) / 3;
		modes[modeIndex] = mode;
		if (loop) {
			if (modeIndex == 0) {
				modes[modes.Length - 1] = mode;
			}
			else if (modeIndex == modes.Length - 1) {
				modes[0] = mode;
			}
		}
		EnforceMode(index);
	}

	private void EnforceMode (int index) {
		int modeIndex = (index + 1) / 3;
		BezierControlPointMode mode = modes[modeIndex];
		if (mode == BezierControlPointMode.Free || !loop && (modeIndex == 0 || modeIndex == modes.Length - 1)) {
			return;
		}

		int middleIndex = modeIndex * 3;
		int fixedIndex, enforcedIndex;
		if (index <= middleIndex) {
			fixedIndex = middleIndex - 1;
			if (fixedIndex < 0) {
				fixedIndex = points.Count - 2;
			}
			enforcedIndex = middleIndex + 1;
			if (enforcedIndex >= points.Count) {
				enforcedIndex = 1;
			}
		}
		else {
			fixedIndex = middleIndex + 1;
			if (fixedIndex >= points.Count) {
				fixedIndex = 1;
			}
			enforcedIndex = middleIndex - 1;
			if (enforcedIndex < 0) {
				enforcedIndex = points.Count - 2;
			}
		}

		Vector3 middle = points[middleIndex];
		Vector3 enforcedTangent = middle - points[fixedIndex];
		if (mode == BezierControlPointMode.Aligned) {
			enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, points[enforcedIndex]);
		}
		points[enforcedIndex] = middle + enforcedTangent;
	}

	public int CurveCount {
		get {
			return (points.Count - 1) / 3;
		}
	}

	public Vector3 GetPoint (float t) {
		int i;
		if (t >= 1f) {
			t = 1f;
			i = points.Count - 4;
		}
		else {
			t = Mathf.Clamp01(t) * CurveCount;
			i = (int)t;
			t -= i;
			i *= 3;
		}
		return transform.TransformPoint(Bezier.GetPoint(points[i], points[i + 1], points[i + 2], points[i + 3], t));
	}

    public Vector3 GetPoint(float t, int index)
    {
        int i;
        if (t >= 1f)
        {
            t = 0;
            i = index + 1;
            i *= 3;
        }
        else
        {
            t = Mathf.Clamp01(t) + index;
            i = (int)t;
            t -= i;
            i *= 3;
        }

		int index1 = Math.Max(0, Math.Min(i, points.Count-1));
		int index2 = Math.Max(0, Math.Min(i+1, points.Count-1));
		int index3 = Math.Max(0, Math.Min(i+2, points.Count-1));
		int index4 = Math.Max(0, Math.Min(i+3, points.Count-1));
		return (Bezier.GetPoint(points[index1], points[index2], points[index3], points[index4], t));

    }
	public Vector3 GetVelocity (float t) {
		int i;
		if (t >= 1f) {
			t = 1f;
			i = points.Count - 4;
		}
		else {
			t = Mathf.Clamp01(t) * CurveCount;
			i = (int)t;
			t -= i;
			i *= 3;
		}
		return transform.TransformPoint(Bezier.GetFirstDerivative(points[i], points[i + 1], points[i + 2], points[i + 3], t)) - transform.position;
	}

    public Vector3 GetVelocity(float t, int index)
    {
        int i;
        if (t >= 1f)
        {
            t = 0;
            i = index + 1;
            i *= 3;
        }
        else
        {
            t = Mathf.Clamp01(t) + index;
            i = (int)t;
            t -= i;
            i *= 3;
        }

		int index1 = Math.Max(0, Math.Min(i, points.Count - 1));
		int index2 = Math.Max(0, Math.Min(i + 1, points.Count - 1));
		int index3 = Math.Max(0, Math.Min(i + 2, points.Count - 1));
		int index4 = Math.Max(0, Math.Min(i + 3, points.Count - 1));
		return transform.TransformPoint(Bezier.GetFirstDerivative(points[index1], points[index2], points[index3], points[index4], t)) - transform.position;
    }

	
	public Vector3 GetDirection (float t) {
		return GetVelocity(t).normalized;
	}

    public Vector3 GetDirection(float t, int index)
    {
        return GetVelocity(t, index).normalized;
    }

    public void AddCurve(Vector3 first, Vector3 second)
    {
        if (points.Count == 0)
            points.Add(first);
        
        //Array.Resize(ref points, points.Count + 3);
        first.x += 5f;
        first.y += 5f;
        //points[points.Count - 3] = point;
        points.Add(first);
       second.x += 5f;
        second.y -= 5f;
        //points[points.Count - 2] = point;
        points.Add(second);
        second.x -= 5f;
        second.y += 5f;
        points.Add(second);
        //points[points.Count - 1] = point;

        Array.Resize(ref modes, modes.Length + 1);
        modes[modes.Length - 1] = modes[modes.Length - 2];
        EnforceMode(points.Count - 4);

        if (loop)
        {
            points[points.Count - 1] = points[0];
            modes[modes.Length - 1] = modes[0];
            EnforceMode(0);
        }
    }
	public void AddCurve () 
    {
        AddCurve(points[points.Count - 1], points[points.Count - 1]);
	}

    public ScreenLevel m_Screen = null;
    private float progress = 0.0f;
    [HideInInspector]
    public float duration = 5f;
    [HideInInspector]
    public GameObject testPlayer = null;

    public ScreenLevel Screen
    {
        get { return m_Screen; }
        set 
        { 
            m_Screen = value; 
        }
    }
//     [HideInInspector]
//     private Animation m_animation = null;
    private Transform player = null;
	private Player m_chPlayer = null;
    private ScreenLevel m_ScreenLevel = null;
    protected override void Awake()
    {
        if (!m_ScreenLevel)
        {
            Transform trn = m_Parent.transform.parent.parent;
            m_ScreenLevel = trn.GetComponent<ScreenLevel>();
        }
    }
	protected override void OnEnable()
    {
        CreateCharacter();
    }
    public void CreateCharacter()
    {
        GameObject obj = gameLogic.Character;
        if (obj)
        {
            player = obj.transform;
            player.SetParent(m_Parent.transform);
            player.localPosition = new Vector3(0.0f, 0.0f, -20.0f);
			player.localScale = new Vector3(105f, 105f, 105f);
			Vector2 pos = GetPoint(0, Mathf.Min(gameData.curWorldInfo.curQuestionNum - 1, WorldInfo.MAX_QUESTION_COUNT - 1));
            RectTransform rt = player as RectTransform;
            rt.anchoredPosition = pos;
            Vector3 euler = rt.eulerAngles;
            rt.eulerAngles = new Vector3(euler.x, 180.0f, euler.z);

			m_chPlayer = obj.GetComponent<Player>();
			m_chPlayer.m_delegate = null;

            if (m_ScreenLevel)
				m_ScreenLevel.UpdateFocus(gameData.curWorldInfo.curQuestionNum - 1);
        }
        else
        {
            if (!testPlayer)
                return;

            player = testPlayer.transform;
            player.SetParent(m_Parent.transform);
            player.localPosition = new Vector3(0.0f, 0.0f, -20.0f);
            player.localScale = new Vector3(105f, 105f, 105f);
            Vector2 pos = GetPoint(0, Mathf.Min(gameData.curWorldInfo.curQuestionNum - 1, WorldInfo.MAX_QUESTION_COUNT - 1));
            RectTransform rt = player as RectTransform;
            rt.anchoredPosition = pos;
            Vector3 euler = rt.eulerAngles;
            rt.eulerAngles = new Vector3(euler.x, 180.0f, euler.z);
        }
    }

    public void StartAction()
    {
        if (!player)
            return;

        progress = 0.0f;
//        m_animation = player.gameObject.GetComponentInChildren<Animation>();

        //if (gameLogic.EscapeKey)
        //    gameData.curWorldInfo.prevQuestion();

		if (gameData.curWorldInfo.curQuestionNum - 1 < WorldInfo.MAX_QUESTION_COUNT - 1)
			StartCoroutine(WalkAni());
    }
    private IEnumerator WalkAni()
    {
        RectTransform rt = player as RectTransform;
        Animation ani = player.GetComponent<Animation>();
        gameLogic.PlayWalkingSound(true);
        while (progress < 1f)
        {
            yield return null;

            progress = progress + Time.deltaTime / duration;
            if (m_chPlayer)
			    m_chPlayer.PlayWalkingAnimation();
            else
                ani.Play("walking");

			int indexOfNextPod = gameData.curWorldInfo.curQuestionNum - 1 > 0 ? gameData.curWorldInfo.curQuestionNum - 1 : 0;
            Vector2 pos = GetPoint(progress, Mathf.Min(indexOfNextPod, WorldInfo.MAX_QUESTION_COUNT - 1));
            rt.anchoredPosition = pos;

            Vector3 forward = rt.InverseTransformDirection(player.up);
			Vector3 dir = GetDirection(progress, indexOfNextPod);
            dir.Normalize();
            float angle = Vector3.Angle(dir, forward);
            angle = Vector3.Dot(dir, Vector3.right) > 0.0f ? angle : -angle;
            rt.eulerAngles = new Vector3(0, angle, 0);
        }

        if (m_chPlayer)
			m_chPlayer.PlayIdleAnimation();
        else
            ani.Play("idle");

        gameLogic.PlayWalkingSound(false);
        if (gameData.curWorldInfo.curQuestionInfo().questionType == QuestionType.bonus_question ||
            gameData.curWorldInfo.curQuestionInfo().questionType == QuestionType.prize_question)
            gameLogic.PlayBonusSound();

        Vector3 euler = rt.eulerAngles;
        rt.eulerAngles = new Vector3(euler.x, 180.0f, euler.z);
        progress = 1f;
		gameData.curWorldInfo.curQuestionNum++;
        if (m_Screen)
            m_Screen.UpdateLevels();
    }
}