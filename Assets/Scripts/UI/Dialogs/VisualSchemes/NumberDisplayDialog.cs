using UnityEngine;
using UnityEngine.UI;

public class NumberDisplayDialog : InnerSchemeDialog
{
    [SerializeField]
    private InputField mValueSize = null;

    [SerializeField]
    private Toggle mSigned = null;

    public override void Create()
    {
        byte valueSize;
        if (byte.TryParse(mValueSize.text, out valueSize) && valueSize > 0)
        {
            var parameters = new NumberDisplay.Parameters();
            parameters.Size = valueSize;
            parameters.Signed = valueSize == 1 ? false : mSigned.isOn;
            Params.Parameters = MyJsonSerializer.Serialize(parameters);
            base.Create();
        }
    }
}
