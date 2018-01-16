using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

	[Serializable]
	public struct RandomTests
	{
		public int[] RandomNumbers;
		public int ValueSize;
		public int TestCount;
		public int Needed;
		[ShowOnlyAttribute]
		public int NumberOfTries;
		[ShowOnlyAttribute]
		public float AverageOfTries;
		[ShowOnlyAttribute]
		public int Max,Min;
	}

	public RandomTests RTS = new Test.RandomTests();


	void Start () 
	{
		RTS.Max = int.MinValue;
		RTS.Min = int.MaxValue;
	}

	public void LogAllTypes()
	{
		var types = this.GetType().Assembly.GetTypes();
		var str = "";
		foreach (var type in types)
			str += type.Name + '\n';
		str += types.Count().ToString();

		Console.Instance.Log(str);
	}

	public void RandomTest()
	{
		RTS.RandomNumbers = new int[RTS.ValueSize];
		var rand = new System.Random();

		RTS.AverageOfTries = 0;
		for (int i = 0; i < RTS.TestCount; i++) {
			RTS.NumberOfTries = 0;
			for (int j = 0; j < RTS.ValueSize; j++)
				RTS.RandomNumbers[j] = 0;
			while (RTS.RandomNumbers.Any(x => x < RTS.Needed)) {
				RTS.NumberOfTries++;
				RTS.RandomNumbers[rand.Next(RTS.ValueSize)]++;
			}
			RTS.AverageOfTries += RTS.NumberOfTries;
			RTS.Max = Math.Max(RTS.Max, RTS.NumberOfTries);
			RTS.Min = Math.Min(RTS.Min, RTS.NumberOfTries);
		}
		RTS.AverageOfTries /= RTS.TestCount;
	}

	public void StartTest()
	{
		StartCoroutine(StartTestCoroutine());
	}

	IEnumerator StartTestCoroutine()
	{
		yield return new WaitForEndOfFrame ();

		var input = "Input";
		var output = "Output";
		var nand = "Nand";
		var not = "NOT";
		var log = new Action<string>((message) => Console.Instance.Log(message));

		var scheme = new UIScheme(new UISelfSchemeBuildInfo(not), 2, 1, 2);
		SchemeDesigner.Instance.CreateScheme(scheme);

		scheme.AddIOGroup(new UIIOGroupBuildInfo(
			new IOGroupBuildString(input, 1, IO.Input),
			new Vector2(-200, 0)
		));
		scheme.AddIOGroup(new UIIOGroupBuildInfo(
			new IOGroupBuildString(output, 1, IO.Output),
			new Vector2(200, 0)
		));

		scheme.AddScheme(new UIInnerSchemeBuildInfo(nand, NAND.Type, null, Vector2.zero));

		scheme.AddLink (new LinkBuilder (not, nand, input, NAND.Input, 0, 1, 0, 2));
		scheme.AddLink (new LinkBuilder (nand, not, NAND.Output, output, 0, 1, 0, 1));

		log(scheme.IOGroups[input].IOArray.Print());
		log(scheme.IOGroups[output].IOArray.Print());

		scheme.SetIO(input, Extensions.Array(true), 0, 1, 0, 1);

		log(scheme.IOGroups[input].IOArray.Print());
		log(scheme.IOGroups[output].IOArray.Print());

		scheme.SetIO(input, Extensions.Array(false), 0, 1, 0, 1);

		log(scheme.IOGroups[input].IOArray.Print());
		log(scheme.IOGroups[output].IOArray.Print());

		SchemeDesigner.Instance.SaveScheme();

		yield break;
	}

	public void StartAnotherTest()
	{
		StartCoroutine(StartAnotherTestCoroutine());
	}

	IEnumerator StartAnotherTestCoroutine()
	{
		yield return new WaitForEndOfFrame ();

		var schameName = "AND";
		var input1 = "A";
		var input2 = "B";
		var output = "C";

		var nandName = "Nand";

		var notType = "NOT";
		var notName = "Not";

		var notInput = "Input";
		var notOutput = "Output";

		var scheme = new UIScheme(new UISelfSchemeBuildInfo(schameName), 3, 2, 4);
		SchemeDesigner.Instance.CreateScheme(scheme);

		scheme.AddIOGroup(new UIIOGroupBuildInfo(
			new IOGroupBuildString(input1, 1, IO.Input),
			new Vector2(-200, -100)
		));
		scheme.AddIOGroup(new UIIOGroupBuildInfo(
			new IOGroupBuildString(input2, 1, IO.Input),
			new Vector2(-200, 100)
		));

		scheme.AddIOGroup(new UIIOGroupBuildInfo(
			new IOGroupBuildString(output, 1, IO.Output),
			new Vector2(200, 0)
		));

		scheme.AddScheme(new UIInnerSchemeBuildInfo(nandName, NAND.Type, null, Vector2.left * 50));
		scheme.AddScheme(new UIInnerSchemeBuildInfo(notName, notType, null, Vector2.right * 50));

		scheme.AddLink (new LinkBuilder (schameName, nandName, input1, NAND.Input, 0, 1, 0, 1));
		scheme.AddLink (new LinkBuilder (schameName, nandName, input2, NAND.Input, 0, 1, 1, 1));
		scheme.AddLink (new LinkBuilder (nandName, notName, NAND.Output, notInput, 0, 1, 0, 1));
		scheme.AddLink (new LinkBuilder (notName, schameName, notOutput, output, 0, 1, 0, 1));

		yield break;
	}

	public void StartPCTest()
	{
		StartCoroutine(StartPCTestCoroutine());
	}

	IEnumerator StartPCTestCoroutine()
	{
		yield return new WaitForEndOfFrame();

		var schemeName = "PC";
		var ramName = "RAM";
		var romName = "ROM";
		var cpuName = "Corei7";

		SchemeDesigner.Instance.LoadScheme(schemeName);

		yield return new WaitForEndOfFrame();

		var scheme = SchemeDesigner.Instance.CurrentScheme;
		var rom = (RAMX)scheme.Schemes[romName].Scheme;
		var cpu = scheme.Schemes[cpuName].Scheme;

		yield return new WaitForEndOfFrame();

		var instrName = "instr";
		var dataName = "data";
		var resetName = "reset";
		var instrAddrName = "instrAddr";
		var dataAddrName = "dataAddr";
		var resultName = "result";
		var writeName = "write";

		var instr = cpu.IOGroups[instrName];
		var data = cpu.IOGroups[dataName];
		var reset = cpu.IOGroups[resetName];
		var instrAddr = cpu.IOGroups[instrAddrName];
		var dataAddr = cpu.IOGroups[dataAddrName];
		var result = cpu.IOGroups[resultName];
		var write = cpu.IOGroups[writeName];

		var testNumber = 1;

		var log = new Action(() => { 
			Console.Instance.LogShort(
				testNumber.ToString("00") + " | " +
				reset.IOArray.Print() + " | " +
				data.IOArray.ToHex() + " | " +
				instr.IOArray.ToHex() + " | " +
				write.IOArray.Print() + " | " +
				dataAddr.IOArray.ToHex() + " | " +
				instrAddr.IOArray.ToHex() + " | " +
				result.IOArray.ToHex());
			testNumber++;
			CycleManager.Instance.RaiseTick();
		});

		log();//1

		cpu.SetIO(resetName, Extensions.Array(true), 0, 1, 0, 1);
		log();//2

		rom.LoadValue(Extensions.FromHEX("80FF"), 0);
		rom.LoadValue(Extensions.FromHEX("3C01"), 1);
		rom.LoadValue(Extensions.FromHEX("0000"), 2);
		rom.LoadValue(Extensions.FromHEX("4000"), 3);
		rom.LoadValue(Extensions.FromHEX("7800"), 4);
		rom.LoadValue(Extensions.FromHEX("1020"), 5);

		cpu.SetIO(resetName, Extensions.Array(false), 0, 1, 0, 1);
		log();//3

		log();//4
		log();//5
		log();//6
		log();//7
		log();//8

		rom.LoadValue(Extensions.FromHEX("1000"), 1);
		rom.LoadValue(Extensions.FromHEX("1000"), 2);
		log();//9
		log();//10

		yield break;
	}

	public void StartFIBTest()
	{
		StartCoroutine(StartFIBTestCoroutine());
	}

	public int FIBNumber = 6;

    public string SetFibNumber
    {
        set { FIBNumber = int.Parse(value); }
    }

    private Action<CycleManager.TickState> mLog;

	IEnumerator StartFIBTestCoroutine()
	{
		yield return new WaitForEndOfFrame();

		var schemeName = "PC";
		var ramName = "RAM";
		var romName = "ROM";
		var cpuName = "Corei7";

		SchemeDesigner.Instance.LoadScheme(schemeName);

		yield return new WaitForEndOfFrame();

		var scheme = SchemeDesigner.Instance.CurrentScheme;
		var rom = (RAMX)scheme.Schemes[romName].Scheme;
		var ram = (RAMX)scheme.Schemes[ramName].Scheme;
		var cpu = scheme.Schemes[cpuName].Scheme;

		yield return new WaitForEndOfFrame();

		var commandCounter = 0;
		var addCommand = new Action<BitArray>((array) => {
			rom.LoadValue(array, commandCounter); 
			commandCounter++;
		});
			
		ram.LoadValue(Extensions.FromINT(FIBNumber, 16), 0);

		//BEGIN
		//[2] = 1
		addCommand(Proggraming.SetDataAddr(2));
		addCommand(Proggraming.WriteData(1));
		//[1] = 0
		addCommand(Proggraming.SetDataAddr(1));
		addCommand(Proggraming.WriteData(1));

		//JUMP TO INIT
		addCommand(Proggraming.SetDataAddr(commandCounter+14));
		addCommand(Proggraming.BuildCommand(OperationCode.ADD, Operand1.Constant, Operand2.Constant, Destination.None, true, 0));

		//LABEL: RETURN [1]
		var return1 = commandCounter;
		addCommand(Proggraming.SetDataAddr(1));
		addCommand(Proggraming.BuildCommand(OperationCode.ADD, Operand1.Constant, Operand2.Data, Destination.AR, false, 0));
		addCommand(Proggraming.SetDataAddr(0));
		addCommand(Proggraming.BuildCommand(OperationCode.ADD, Operand1.Constant, Operand2.AR, Destination.Data, false, 0));

		addCommand(Proggraming.SetDataAddr(commandCounter+6));
		addCommand(Proggraming.BuildCommand(OperationCode.ADD, Operand1.Constant, Operand2.Constant, Destination.None, true, 0));

		//LABEL: RETURN [2]
		var return2 = commandCounter;
		addCommand(Proggraming.SetDataAddr(2));
		addCommand(Proggraming.BuildCommand(OperationCode.ADD, Operand1.Constant, Operand2.Data, Destination.AR, false, 0));
		addCommand(Proggraming.SetDataAddr(0));
		addCommand(Proggraming.BuildCommand(OperationCode.ADD, Operand1.Constant, Operand2.AR, Destination.Data, false, 0));

		//LABEL: STOP
		var stop = commandCounter;
		//JUMP TO END
		addCommand(Proggraming.SetDataAddr(38));
		addCommand(Proggraming.BuildCommand(OperationCode.ADD, Operand1.Constant, Operand2.Constant, Destination.None, true, 0));

		//LABEL: INIT
		var init = commandCounter;
		//AR = [0] - 1
		addCommand(Proggraming.SetDataAddr(0));
		addCommand(Proggraming.BuildCommand(OperationCode.ADD, Operand1.Constant, Operand2.Data, Destination.AR, false, -1));
		//JUMP IF AR = 0 TO RETURN [1] 
		addCommand(Proggraming.BuildConst(return1));
		addCommand(Proggraming.BuildCommand(OperationCode.ADD, Operand1.Constant, Operand2.AR, Destination.None, true, 0));

		//LABEL: CHECK
		var check = commandCounter;
		//AR = [0] - 2
		addCommand(Proggraming.SetDataAddr(0));
		addCommand(Proggraming.BuildCommand(OperationCode.ADD, Operand1.Constant, Operand2.Data, Destination.AR, false, -2));
		//JUMP IF AR = 0 TO RETURN [2]
		addCommand(Proggraming.SetDataAddr(return2));
		addCommand(Proggraming.BuildCommand(OperationCode.ADD, Operand1.Constant, Operand2.AR, Destination.None, true, 0));

		//AR = [1] + [2]
		addCommand(Proggraming.SetDataAddr(1));
		addCommand(Proggraming.BuildCommand(OperationCode.ADD, Operand1.Constant, Operand2.Data, Destination.AR, false, 0));
		addCommand(Proggraming.SetDataAddr(2));
		addCommand(Proggraming.BuildCommand(OperationCode.ADD, Operand1.AR, Operand2.Data, Destination.AR, false, 0));
		//[2] = AR
		addCommand(Proggraming.BuildCommand(OperationCode.ADD, Operand1.AR, Operand2.Constant, Destination.Data, false, 0));
		//[1] = AR - [1]
		addCommand(Proggraming.SetDataAddr(1));
		addCommand(Proggraming.BuildCommand(OperationCode.NegateIn2, Operand1.AR, Operand2.Data, Destination.AR, false, 0));
		addCommand(Proggraming.BuildCommand(OperationCode.ADD, Operand1.AR, Operand2.Constant, Destination.Data, false, 1));
		//[0] = [0]-1
		addCommand(Proggraming.SetDataAddr(0));
		addCommand(Proggraming.BuildCommand(OperationCode.ADD, Operand1.Constant, Operand2.Data, Destination.Data, false, -1));
		//JUMP TO CHECK 
		addCommand(Proggraming.SetDataAddr(check));
		addCommand(Proggraming.BuildCommand(OperationCode.ADD, Operand1.Constant, Operand2.Constant, Destination.None, true, 0));

		//LABEL: END
		addCommand(Proggraming.SetDataAddr(0));

		var instrName = "instr";
		var dataName = "data";
		var resetName = "reset";
		var instrAddrName = "instrAddr";
		var dataAddrName = "dataAddr";
		var resultName = "result";
		var writeName = "write";

		var instr = cpu.IOGroups[instrName];
		var data = cpu.IOGroups[dataName];
		var reset = cpu.IOGroups[resetName];
		var instrAddr = cpu.IOGroups[instrAddrName];
		var dataAddr = cpu.IOGroups[dataAddrName];
		var result = cpu.IOGroups[resultName];
		var write = cpu.IOGroups[writeName];

	    if (mLog != null)
	        CycleManager.Instance.Tick -= mLog;
		mLog = (state) => { 
		    if (state != CycleManager.TickState.PreTick)
		        return;
		    Console.Instance.LogShort(
		        "DATA " +
		        data.IOArray.ToInt().ToString("00000") + " | INSTRUCTION " +
		        instr.IOArray.Print() + " | WRITE " +
		        write.IOArray.Print() + " | DATAADDR " +
		        dataAddr.IOArray.ToInt().ToString("00000") + " | INSTRADDR " +
		        instrAddr.IOArray.ToInt().ToString("00000") + " | RESULT " +
		        result.IOArray.ToInt().ToString("00000"));
		};
		CycleManager.Instance.Tick += mLog;

		yield break;
	}

}
