namespace Jikji
{
    [System.Flags]
    public enum Style
    {
        Width = 0b01,
        Weight = 0b10,

        Full = 0b00,
        Half = 0b01,
        Regular = 0b00,
        Bold = 0b10,

        FullRegular = Full | Regular, // 0b00
        HlafRegular = Half | Regular, // 0b01
        FullBold = Full | Bold,       // 0b10
        HalfBold = Half | Bold,       // 0b11
    }
}
