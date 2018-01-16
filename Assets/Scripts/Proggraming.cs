using System;
using System.Collections;

public static class Proggraming {

	public static BitArray SetDataAddr(int address)
	{
		return Proggraming.BuildConst(address);
	}

	public static BitArray WriteData(int data)
	{
		return Proggraming.BuildCommand(OperationCode.ADD, Operand1.Constant, Operand2.AR, Destination.Data, false, data);
	}

	public static BitArray BuildCommand(OperationCode opCode, Operand1 op1, Operand2 op2, Destination dest, bool jmpIfZero, int constant)
	{
		var array = new BitArray(16);

		if (constant < 0)
		{
			array[4] = true;
			constant = -constant - 1;
			for (int i = 0; i < 4; i++)
			{
				array[i] = constant % 2 == 0;
				constant /= 2;
			}
		}
		else
		{
			for (int i = 0; i < 4; i++)
			{
				array[i] = constant % 2 == 1;
				constant /= 2;
			}
		}

		array[5] = jmpIfZero;

		var operation = (int)opCode;
		for (int i = 6; i < 10; i++)
		{
			array[i] = operation % 2 == 1;
			operation /= 2;
		}

		var operand2 = (int)op2;
		for (int i = 10; i < 12; i++)
		{
			array[i] = operand2 % 2 == 1;
			operand2 /= 2;
		}

		array[12] = op1 == Operand1.Constant;

		var destination = (int)dest;
		for (int i = 13; i < 15; i++)
		{
			array[i] = destination % 2 == 1;
			destination /= 2;
		}

		return array;
	}

	public static BitArray BuildConst(int number)
	{
		var array = new BitArray(16);
		array[15] = true;
		for (int i = 0; i < 15; i++)
		{
			array[i] = number % 2 == 1;
			number /= 2;
		}
		return array;
	}
}

[Flags]
public enum OperationCode
{
	ADD = 0,
	NegateOut = 1,
	NAND = 2,
	NegateIn2 = 4,
	NegateIn1 = 8
}

public enum Operand1
{
	AR = 0,
	Constant = 1
}

public enum Operand2
{
	Constant = 0,
	AR = 1,
	MR = 2,
	Data = 3
}

public enum Destination
{
	None = 0,
	AR = 1,
	MR = 2,
	Data = 3
}
