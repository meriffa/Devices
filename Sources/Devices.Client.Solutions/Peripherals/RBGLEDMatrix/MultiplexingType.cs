namespace Devices.Client.Solutions.Peripherals.RBGLEDMatrix;

/// <summary>
/// Multiplexing type
/// </summary>
public enum MultiplexingType : int
{
    Direct = 0,
    Stripe = 1,
    Checker = 2,
    Spiral = 3,
    ZStripe = 4,
    ZnMirrorZStripe = 5,
    Coreman = 6,
    Kaler2Scan = 7,
    ZStripeUneven = 8,
    P10_128x4_Z = 9,
    QiangLiQ8 = 10,
    InversedZStripe = 11,
    P10Outdoor1R1G1_1 = 12,
    P10Outdoor1R1G1_2 = 13,
    P10Outdoor1R1G1_3 = 14,
    P10CoremanMapper = 15,
    P8Outdoor1R1G1 = 16,
    FlippedStripe = 17
}