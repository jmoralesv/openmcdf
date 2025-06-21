﻿namespace OpenMcdf.Ole;

internal abstract class TypedPropertyValue<T> : ITypedPropertyValue
{
    private readonly VTPropertyType _VTType;
    protected object? propertyValue;

    public PropertyType PropertyType => PropertyType.TypedPropertyValue;

    public VTPropertyType VTType => _VTType;

    public TypedPropertyValue(VTPropertyType vtType, bool isVariant = false)
    {
        _VTType = vtType;
        PropertyDimensions = CheckPropertyDimensions(vtType);
        IsVariant = isVariant;
    }

    public PropertyDimensions PropertyDimensions { get; } = PropertyDimensions.IsScalar;

    public bool IsVariant { get; }

    protected virtual bool NeedsPadding { get; set; } = true;

    private static PropertyDimensions CheckPropertyDimensions(VTPropertyType vtType)
    {
        if ((((ushort)vtType) & 0x1000) != 0)
            return PropertyDimensions.IsVector;
        if ((((ushort)vtType) & 0x2000) != 0)
            return PropertyDimensions.IsArray;
        return PropertyDimensions.IsScalar;
    }

    public virtual object? Value
    {
        get => propertyValue;
        set => propertyValue = value;
    }

    public abstract T? ReadScalarValue(BinaryReader br);

    public void Read(BinaryReader br)
    {
        long currentPos = br.BaseStream.Position;

        switch (PropertyDimensions)
        {
            case PropertyDimensions.IsScalar:
                {
                    propertyValue = ReadScalarValue(br);
                    int size = (int)(br.BaseStream.Position - currentPos);

                    int m = size % 4;

                    if (m > 0 && NeedsPadding)
                        br.ReadBytes(4 - m); // padding
                }

                break;

            case PropertyDimensions.IsVector:
                {
                    uint nItems = br.ReadUInt32();

                    List<T> res = new();

                    for (int i = 0; i < nItems; i++)
                    {
                        T s = ReadScalarValue(br)!;

                        res.Add(s);

                        // The padding in a vector can be per-item
                        int itemSize = (int)(br.BaseStream.Position - currentPos);

                        int pad = itemSize % 4;
                        if (pad > 0 && NeedsPadding)
                            br.ReadBytes(4 - pad); // padding
                    }

                    propertyValue = res;
                    int size = (int)(br.BaseStream.Position - currentPos);

                    int m = size % 4;
                    if (m > 0 && NeedsPadding)
                        br.ReadBytes(4 - m); // padding
                }

                break;
            default:
                break;
        }
    }

    public abstract void WriteScalarValue(BinaryWriter bw, T pValue);

    public void Write(BinaryWriter bw)
    {
        long currentPos = bw.BaseStream.Position;
        int size;
        int m;
        switch (PropertyDimensions)
        {
            case PropertyDimensions.IsScalar:

                bw.Write((ushort)_VTType);
                bw.Write((ushort)0);

                WriteScalarValue(bw, (T)propertyValue!);
                size = (int)(bw.BaseStream.Position - currentPos);
                m = size % 4;

                if (m > 0 && NeedsPadding)
                {
                    for (int i = 0; i < 4 - m; i++) // padding
                        bw.Write((byte)0);
                }

                break;

            case PropertyDimensions.IsVector:

                bw.Write((ushort)_VTType);
                bw.Write((ushort)0);
                bw.Write((uint)((List<T>)propertyValue!).Count);

                for (int i = 0; i < ((List<T>)propertyValue).Count; i++)
                {
                    WriteScalarValue(bw, ((List<T>)propertyValue)[i]);

                    size = (int)(bw.BaseStream.Position - currentPos);
                    m = size % 4;

                    if (m > 0 && NeedsPadding)
                    {
                        for (int q = 0; q < 4 - m; q++) // padding
                            bw.Write((byte)0);
                    }
                }

                size = (int)(bw.BaseStream.Position - currentPos);
                m = size % 4;

                if (m > 0 && NeedsPadding)
                {
                    for (int i = 0; i < 4 - m; i++) // padding
                        bw.Write((byte)0);
                }

                break;
        }
    }
}
