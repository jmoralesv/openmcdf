﻿namespace OpenMcdf.Ole;

// A separate factory for DocumentSummaryInformation properties, to handle special cases with unaligned strings.
internal sealed class DocumentSummaryInfoPropertyFactory : PropertyFactory
{
    public static DocumentSummaryInfoPropertyFactory Default { get; } = new();

    protected override ITypedPropertyValue CreateLpstrProperty(VTPropertyType vType, int codePage, uint propertyIdentifier, bool isVariant)
    {
        // PIDDSI_HEADINGPAIR and PIDDSI_DOCPARTS use unaligned (unpadded) strings - the others are padded
        if (propertyIdentifier is 0x0000000C or 0x0000000D)
            return new VT_Unaligned_LPSTR_Property(vType, codePage, isVariant);

        return base.CreateLpstrProperty(vType, codePage, propertyIdentifier, isVariant);
    }
}
