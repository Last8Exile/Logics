using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Demo : MonoBehaviour
{

    [SerializeField]
    private Text mWaitIcon = null;
    [SerializeField]
    private InputField mNumber = null;

    void Update()
    {
        mWaitIcon.color = new Color(1, 1, 1, Mathf.Sin(Time.time) * 0.5f + 0.5f);
    }

    public void StartDemo()
    {
        StopAllCoroutines();
        mNumber.Select();
        StartCoroutine(DemoCorutine(int.Parse(mNumber.text)));
    }

    private IEnumerator DemoCorutine(int number)
    {
        int counter = 0;

        if (number < ++counter)
            yield return StartCoroutine(Part1());

        if (number < ++counter)
            yield return StartCoroutine(Part2());
    }

    #region Parts

    //Show NOT
    private IEnumerator Part1()
    {
        var designer = SchemeDesigner.Instance;
        designer.LoadScheme("NOT");
        yield return new WaitForSeconds(2);

        yield return StartCoroutine(SetCameraSize(500));

        var waitLock = StartWait();
        while (!waitLock.Complete)
        {
            designer.CurrentScheme.SetIO("Input", Extensions.FromInt(1, 1), 0, 1, 0, 1);
            yield return new WaitForSeconds(3);
            designer.CurrentScheme.SetIO("Input", Extensions.FromInt(0, 1), 0, 1, 0, 1);
            yield return new WaitForSeconds(3);
        }
    }

    //Create MyAND
    private IEnumerator Part2()
    {
        StartCoroutine(SetCameraSize(750));

        //New Scheme
        var designer = SchemeDesigner.Instance;
        var newSchemeDialog = designer.CreateSchemeAdv();
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(FillInputField(newSchemeDialog.GetComponentInChildren<InputField>(), "MyAND", 0.5f));
        yield return StartCoroutine(WaitForInput());
        newSchemeDialog.Create();

        //Input
        yield return StartCoroutine(WaitForInput());
        var addIOGroupDialog = designer.AddIOGroupAdv();
        yield return new WaitForSeconds(0.5f);
        var dialogInputFields = addIOGroupDialog.GetComponentsInChildren<InputField>();
        yield return StartCoroutine(FillInputField(dialogInputFields[0], "Input", 0.5f));
        yield return StartCoroutine(FillInputField(dialogInputFields[1], "2", 0.5f));
        yield return StartCoroutine(WaitForInput());
        addIOGroupDialog.Create();

        //MoveInput
        yield return new WaitForSeconds(0.5f);
        var ioDesign = designer.CurrentScheme.IOGroupsInfo[0].Design as IOSelfIOGroupDesign;
        yield return StartCoroutine(DragIOSelfIOGroupDesign(ioDesign, new Vector2(-600, 70)));

        //Output
        yield return StartCoroutine(WaitForInput());
        addIOGroupDialog = designer.AddIOGroupAdv();
        yield return new WaitForSeconds(0.5f);
        dialogInputFields = addIOGroupDialog.GetComponentsInChildren<InputField>();
        yield return StartCoroutine(FillInputField(dialogInputFields[0], "Output", 0.5f));
        yield return StartCoroutine(FillInputField(dialogInputFields[1], "1", 0.5f));
        yield return new WaitForSeconds(0.5f);
        var dropDown = addIOGroupDialog.GetComponentInChildren<Dropdown>();
        dropDown.Show();
        yield return new WaitForSeconds(0.5f);
        dropDown.value = 1;
        dropDown.Hide();
        yield return StartCoroutine(WaitForInput());
        addIOGroupDialog.Create();

        //MoveOutput
        yield return new WaitForSeconds(0.5f);
        ioDesign = designer.CurrentScheme.IOGroupsInfo[1].Design as IOSelfIOGroupDesign;
        yield return StartCoroutine(DragIOSelfIOGroupDesign(ioDesign, new Vector2(600, 70)));
    }

    #endregion Parts

    private WaitLock StartWait(KeyCode keyCode = KeyCode.Space)
    {
        var waitLock = new WaitLock();
        StartCoroutine(WaitForInput(waitLock, keyCode));
        return waitLock;
    }

    private IEnumerator WaitForInput(WaitLock waitLock, KeyCode keyCode = KeyCode.Space)
    {
        ShowWaitIcon();
        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => Input.GetKeyUp(keyCode));
        HideWaitIcon();
        waitLock.Complete = true;
    }

    private IEnumerator WaitForInput(KeyCode keyCode = KeyCode.Space)
    {
        ShowWaitIcon();
        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => Input.GetKeyUp(keyCode));
        HideWaitIcon();
    }

    private void ShowWaitIcon()
    {
        mWaitIcon.gameObject.SetActive(true);
    }

    private void HideWaitIcon()
    {
        mWaitIcon.gameObject.SetActive(false);
    }

    private class WaitLock
    {
        public bool Complete;
    }

    private IEnumerator FillInputField(InputField field, string text, float time)
    {
        field.text = "";
        if (string.IsNullOrEmpty(text))
        {
            yield break;
        }
        var interval = time / text.Length;
        foreach (char t in text)
        {
            yield return new WaitForSeconds(interval);
            field.text += t;
        }
    }

    private IEnumerator SetCameraSize(float size, float speed = 0.1f)
    {
        var camera = Camera.main;
        while (Math.Abs(camera.orthographicSize - size) > 0.1f)
        {
            camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, size, speed);
            yield return null;
        }
        camera.orthographicSize = size;
    }

    private IEnumerator DragIOSelfIOGroupDesign(IOSelfIOGroupDesign design, Vector2 delta, float speed = 0.1f)
    {
        var eventData = new PointerEventData(EventSystem.current);
        eventData.button = PointerEventData.InputButton.Left;
        design.OnBeginDrag(eventData);
        while ((eventData.position - delta).sqrMagnitude > 0.1f)
        {
            eventData.position = Vector2.Lerp(eventData.position, delta, speed);
            design.OnDrag(eventData);
            yield return null;
        }
        eventData.position = delta;
        design.OnDrag(eventData);
    }
}
