using System;
using System.Collections;
using System.Linq;
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
            yield return StartCoroutine(Part0());
        if (number < ++counter)
            yield return StartCoroutine(Part1());
        if (number < ++counter)
            yield return StartCoroutine(Part2());
        if (number < ++counter)
            yield return StartCoroutine(Part3());
        if (number < ++counter)
            yield return StartCoroutine(Part4());
        if (number < ++counter)
            yield return StartCoroutine(Part5());
    }

    #region Parts

    //Show NOT
    private IEnumerator Part0()
    {
        var designer = SchemeDesigner.Instance;
        designer.LoadScheme("NOT");
        yield return new WaitForSeconds(2);

        StartCoroutine(SetCameraPos(Vector2.zero));
        yield return StartCoroutine(SetCameraSize(600));

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
    private IEnumerator Part1()
    {
        StartCoroutine(SetCameraSize(750));
        StartCoroutine(SetCameraPos(Vector2.zero));

        //New Scheme
        var designer = SchemeDesigner.Instance;
        var newSchemeDialog = designer.CreateSchemeAdv();
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(FillInputField(newSchemeDialog.GetComponentInChildren<InputField>(), "MyAND", 0.5f));
        yield return new WaitForSeconds(1.5f);
        newSchemeDialog.Create();

        #region IOGroups

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
        yield return StartCoroutine(DragIOSelfIOGroupDesign(ioDesign, new Vector2(-450, 75)));

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
        yield return StartCoroutine(DragIOSelfIOGroupDesign(ioDesign, new Vector2(450, 75)));

        #endregion IOGroups

        yield return StartCoroutine(WaitForInput());

        #region InnerSchemes

        //NAND
        var innerDialog = designer.AddInnerSchemeAdv("NAND");
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(FillInputField(innerDialog.GetComponentInChildren<InputField>(), "Nand", 0.5f));
        yield return new WaitForSeconds(0.5f);
        innerDialog.Create();
        yield return new WaitForSeconds(0.5f);
        var innerDesign = designer.CurrentScheme.InnerSchemes[0].Design as BaseInnerSchemeDesign;
        yield return StartCoroutine(DragBaseInnerSchameDesign(innerDesign, new Vector2(-200, 30)));

        //NOT
        innerDialog = designer.AddInnerSchemeAdv("NOT");
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(FillInputField(innerDialog.GetComponentInChildren<InputField>(), "Not", 0.5f));
        yield return new WaitForSeconds(0.5f);
        innerDialog.Create();
        yield return new WaitForSeconds(0.5f);
        innerDesign = designer.CurrentScheme.InnerSchemes[1].Design as BaseInnerSchemeDesign;
        yield return StartCoroutine(DragBaseInnerSchameDesign(innerDesign, new Vector2(200, 30)));

        #endregion InnerSchemes

        yield return StartCoroutine(WaitForInput());

        #region Links

        //Input -> Nand
        designer.AddLinkAsSource(designer.CurrentScheme.IOGroupsInfo[0]);
        yield return StartCoroutine(WaitForInput());
        designer.AddLinkAsTarget(designer.CurrentScheme.InnerSchemes[0], false);
        var linkDialog = designer.AddLinkAdv();
        yield return StartCoroutine(WaitForInput());
        linkDialog.Create();

        //Nand -> Not
        yield return StartCoroutine(WaitForInput());
        yield return new WaitForSeconds(1);
        designer.AddLinkAsSource(designer.CurrentScheme.InnerSchemes[0]);
        yield return new WaitForSeconds(0.5f);
        designer.AddLinkAsTarget(designer.CurrentScheme.InnerSchemes[1], false);
        linkDialog = designer.AddLinkAdv();
        yield return new WaitForSeconds(0.5f);
        linkDialog.Create();
        yield return new WaitForSeconds(0.5f);
        designer.AddLinkAsSource(designer.CurrentScheme.InnerSchemes[1]);
        yield return new WaitForSeconds(0.5f);
        designer.AddLinkAsTarget(designer.CurrentScheme.IOGroupsInfo[1], false);
        linkDialog = designer.AddLinkAdv();
        yield return new WaitForSeconds(0.5f);
        linkDialog.Create();


        #endregion Links

        yield return StartCoroutine(WaitForInput());

        #region Test

        var waitLock = StartWait();
        while (!waitLock.Complete)
        {
            designer.CurrentScheme.SetIO("Input", Extensions.FromInt(1, 2), 0, 2, 0, 2);
            yield return new WaitForSeconds(2);
            designer.CurrentScheme.SetIO("Input", Extensions.FromInt(2, 2), 0, 2, 0, 2);
            yield return new WaitForSeconds(2);
            designer.CurrentScheme.SetIO("Input", Extensions.FromInt(3, 2), 0, 2, 0, 2);
            yield return new WaitForSeconds(2);
            designer.CurrentScheme.SetIO("Input", Extensions.FromInt(0, 2), 0, 2, 0, 2);
            yield return new WaitForSeconds(2);
        }

        #endregion Test

    }

    //Gates
    private IEnumerator Part2()
    {
        var designer = SchemeDesigner.Instance;
        designer.LoadScheme("OR");
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(SetCameraSize(1000));
        StartCoroutine(SetCameraPos(new Vector2(-350, 100)));
        yield return new WaitForSeconds(5f);
        designer.CurrentScheme.SetIO("Input", Extensions.FromInt(1, 2), 0, 2, 0, 2);
        yield return new WaitForSeconds(5f);
        designer.CurrentScheme.SetIO("Input", Extensions.FromInt(2, 2), 0, 2, 0, 2);
        yield return new WaitForSeconds(5f);
        designer.CurrentScheme.SetIO("Input", Extensions.FromInt(3, 2), 0, 2, 0, 2);
        yield return StartCoroutine(WaitForInput());

        designer.LoadScheme("XOR");
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(SetCameraSize(1000));
        StartCoroutine(SetCameraPos(new Vector2(-100, 350)));
        yield return new WaitForSeconds(5f);
        designer.CurrentScheme.SetIO("Input", Extensions.FromInt(1, 2), 0, 2, 0, 2);
        yield return new WaitForSeconds(5f);
        designer.CurrentScheme.SetIO("Input", Extensions.FromInt(2, 2), 0, 2, 0, 2);
        yield return new WaitForSeconds(5f);
        designer.CurrentScheme.SetIO("Input", Extensions.FromInt(3, 2), 0, 2, 0, 2);
        yield return StartCoroutine(WaitForInput());

        designer.LoadScheme("FULLADDER");
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(SetCameraSize(1050));
        StartCoroutine(SetCameraPos(new Vector2(-175, 200)));
        yield return new WaitForSeconds(5f);
        designer.CurrentScheme.SetIO("In", Extensions.FromInt(1, 2), 0, 2, 0, 2);
        yield return new WaitForSeconds(5f);
        designer.CurrentScheme.SetIO("In", Extensions.FromInt(3, 2), 0, 2, 0, 2);
        yield return new WaitForSeconds(5f);
        designer.CurrentScheme.SetIO("CarryIn", Extensions.FromInt(1, 1), 0, 1, 0, 1);
        yield return StartCoroutine(WaitForInput());
    }

    //Memory
    private IEnumerator Part3()
    {
        var designer = SchemeDesigner.Instance;

        designer.LoadScheme("REGISTER");
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(SetCameraSize(933));
        StartCoroutine(SetCameraPos(new Vector2(-150, 350)));
        yield return StartCoroutine(WaitForInput());

        var dialog = designer.RemoveInnerSchemeAdv(designer.CurrentScheme.InnerSchemes[1]);
        yield return new WaitForSeconds(0.5f);
        dialog.DialogResult = DialogResult.Ok;
        yield return new WaitForSeconds(0.5f);
        designer.AddLinkAsSource(designer.CurrentScheme.IOGroupsInfo[0]);
        designer.AddLinkAsTarget(designer.CurrentScheme.InnerSchemes[0], false);
        var linkDialog = designer.AddLinkAdv();
        yield return new WaitForSeconds(0.5f);
        linkDialog.Create();
        yield return StartCoroutine(WaitForInput());
        CycleManager.Instance.Start();
        yield return new WaitForSeconds(5f);
        designer.CurrentScheme.SetIO("In", Extensions.FromInt(1, 1), 0, 1, 0, 1);
        yield return new WaitForSeconds(2f);
        designer.CurrentScheme.SetIO("In", Extensions.FromInt(0, 1), 0, 1, 0, 1);
        yield return new WaitForSeconds(1.2f);
        designer.CurrentScheme.SetIO("In", Extensions.FromInt(1, 1), 0, 1, 0, 1);
        yield return new WaitForSeconds(0.1f);
        designer.CurrentScheme.SetIO("In", Extensions.FromInt(0, 1), 0, 1, 0, 1);
        yield return new WaitForSeconds(0.1f);
        designer.CurrentScheme.SetIO("In", Extensions.FromInt(1, 1), 0, 1, 0, 1);
        yield return new WaitForSeconds(0.1f);
        designer.CurrentScheme.SetIO("In", Extensions.FromInt(0, 1), 0, 1, 0, 1);
        yield return new WaitForSeconds(0.7f);
        designer.CurrentScheme.SetIO("In", Extensions.FromInt(1, 1), 0, 1, 0, 1);
        yield return StartCoroutine(WaitForInput());

        CycleManager.Instance.Stop();
        designer.LoadScheme("REGISTER");
        yield return StartCoroutine(WaitForInput());
    }

    //AdvancedSchemes
    private IEnumerator Part4()
    {
        var designer = SchemeDesigner.Instance;

        //Counter
        designer.LoadScheme("COUNTER16B");
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(SetCameraSize(1741));
        StartCoroutine(SetCameraPos(new Vector2(-230, 900)));


        yield return StartCoroutine(WaitForInput());
        CycleManager.Instance.Start();
        yield return new WaitForSeconds(8f);
        designer.CurrentScheme.SetIO("Reset", Extensions.FromInt(1, 1), 0, 1, 0, 1);
        yield return new WaitForSeconds(4f);
        designer.CurrentScheme.SetIO("Reset", Extensions.FromInt(0, 1), 0, 1, 0, 1);
        yield return StartCoroutine(WaitForInput());
        CycleManager.Instance.Stop();
        designer.CurrentScheme.SetIO("Load", Extensions.FromInt(1, 1), 0, 1, 0, 1);
        designer.CurrentScheme.SetIO("In", Extensions.FromInt(64, 16), 0, 16, 0, 16);
        yield return StartCoroutine(WaitForInput());
        CycleManager.Instance.RaiseTick();
        designer.CurrentScheme.SetIO("Load", Extensions.FromInt(0, 1), 0, 1, 0, 1);
        yield return StartCoroutine(WaitForInput());
        CycleManager.Instance.Start();
        yield return new WaitForSeconds(4f);
        yield return StartCoroutine(WaitForInput());
        CycleManager.Instance.Stop();
        
        //Cpu Inside
        designer.LoadScheme("CPU");
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(SetCameraPos(new Vector2(-367, 1421)));
        StartCoroutine(SetCameraSize(2639));
        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(WaitForInput());

        //LoadValues
        designer.CurrentScheme.SetIO("instr", Extensions.FromInt(31771, 16), 0, 16, 0, 16);
        designer.CurrentScheme.SetIO("data", Extensions.FromInt(6, 16), 0, 16, 0, 16);
        yield return StartCoroutine(WaitForInput());

        //Instr
        StartCoroutine(SetCameraPos(new Vector2(-2902, 2263)));
        StartCoroutine(SetCameraSize(1071));
        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(WaitForInput());

        //Data and Reset
        StartCoroutine(SetCameraPos(new Vector2(-1649, 225)));
        StartCoroutine(SetCameraSize(1319));
        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(WaitForInput());

        //Outs
        StartCoroutine(SetCameraSize(1414));
        StartCoroutine(SetCameraPos(new Vector2(2522, 2590)));
        yield return new WaitForSeconds(2f);
        var outputs = designer.CurrentScheme.IOGroupsInfo.Where(x => x.IOGroup.IO == IO.Output).Select(x => x.Design as IOSelfIOGroupDesign).ToList();
        StartCoroutine(DragIOSelfIOGroupDesign(outputs.First(x => x.gameObject.name.Contains("write")), new Vector2(-4.2085f, 196.5029f)));
        StartCoroutine(DragIOSelfIOGroupDesign(outputs.First(x => x.gameObject.name.Contains("dataAddr")), new Vector2(64.41211f, 1062.611f)));
        StartCoroutine(DragIOSelfIOGroupDesign(outputs.First(x => x.gameObject.name.Contains("instrAddr")), new Vector2(62.72046f, 1465.897f)));
        StartCoroutine(DragIOSelfIOGroupDesign(outputs.First(x => x.gameObject.name.Contains("result")), new Vector2(130f, 268.6553f)));
        yield return new WaitForSeconds(2f);
        //StartCoroutine(ResizeIOSelfIOGroupDesign(outputs.First(x => x.gameObject.name.Contains("write")), new Vector2(0, 500)));
        StartCoroutine(ResizeIOSelfIOGroupDesign(outputs.First(x => x.gameObject.name.Contains("dataAddr")), new Vector2(0, 500)));
        StartCoroutine(ResizeIOSelfIOGroupDesign(outputs.First(x => x.gameObject.name.Contains("instrAddr")), new Vector2(0, 500)));
        StartCoroutine(ResizeIOSelfIOGroupDesign(outputs.First(x => x.gameObject.name.Contains("result")), new Vector2(0, 500)));
        yield return new WaitForSeconds(2f);
        StartCoroutine(SetCameraPos(new Vector2(2624.015f, 3330.604f)));
        StartCoroutine(SetCameraSize(430));
        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(WaitForInput());
    }

    //Fib
    private IEnumerator Part5()
    {
        var designer = SchemeDesigner.Instance;

        yield return StartCoroutine(WaitForInput());
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

    private IEnumerator SetCameraPos(Vector2 pos, float speed = 0.1f)
    {
        var camera = Camera.main.transform;
        var targetPos = pos.ToVector3(-10);
        while ((camera.position - targetPos).sqrMagnitude > 0.1f)
        {
            camera.position = Vector3.Lerp(camera.position, targetPos, speed);
            yield return null;
        }
        camera.position = targetPos;
    }

    private IEnumerator DragIOSelfIOGroupDesign(IOSelfIOGroupDesign design, Vector2 delta, float speed = 0.1f)
    {
        delta = Extensions.WorldPosToScreen(delta);
        var eventData = new PointerEventData(EventSystem.current);
        eventData.button = PointerEventData.InputButton.Left;
        eventData.position = Extensions.WorldPosToScreen(Vector2.zero);
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

    private IEnumerator ResizeIOSelfIOGroupDesign(IOSelfIOGroupDesign design, Vector2 delta, float speed = 0.1f)
    {
        delta = Extensions.WorldPosToScreen(delta);
        var eventData = new PointerEventData(EventSystem.current);
        eventData.button = PointerEventData.InputButton.Left;
        eventData.position = Extensions.WorldPosToScreen(Vector2.zero);
        design.CornerOnBeginDrag(eventData);
        while ((eventData.position - delta).sqrMagnitude > 0.1f)
        {
            eventData.position = Vector2.Lerp(eventData.position, delta, speed);
            design.CornerOnDrag(eventData);
            yield return null;
        }
        eventData.position = delta;
        design.CornerOnDrag(eventData);
        design.CornerOnEndDrag();
    }

    private IEnumerator DragBaseInnerSchameDesign(BaseInnerSchemeDesign design, Vector2 delta, float speed = 0.1f)
    {
        delta = Extensions.WorldPosToScreen(delta);
        var eventData = new PointerEventData(EventSystem.current);
        eventData.button = PointerEventData.InputButton.Left;
        eventData.position = Extensions.WorldPosToScreen(Vector2.zero);
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
