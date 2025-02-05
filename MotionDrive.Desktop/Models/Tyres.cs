namespace MotionDrive.Desktop.Models;

public class Tyres
{

    // Generate all four tyres => pressure, brake temp, wear just the fields (FOr every tyre there should be something )

    public double TyrePressureFL
    {
        get; set;
    }

    public double TyrePressureFR
    {
        get; set;
    }

    public double TyrePressureRL
    {
        get; set;
    }

    public double TyrePressureRR
    {
        get; set;
    }

    public double BrakeTempFL
    {
        get; set;
    }

    public double BrakeTempFR
    {
        get; set;
    }

    public double BrakeTempRL
    {
        get; set;
    }

    public double BrakeTempRR
    {
        get; set;
    }

    public double TyreWearFL
    {
        get; set;
    }

    public double TyreWearFR
    {
        get; set;
    }

    public double TyreWearRL
    {
        get; set;
    }

    public double TyreWearRR
    {
        get; set;
    }
    public Tyres(double tyrePressureFL, double tyrePressureFR, double tyrePressureRL, double tyrePressureRR, double brakeTempFL, double brakeTempFR, double brakeTempRL, double brakeTempRR, double tyreWearFL, double tyreWearFR, double tyreWearRL, double tyreWearRR)
    {
        TyrePressureFL = tyrePressureFL;
        TyrePressureFR = tyrePressureFR;
        TyrePressureRL = tyrePressureRL;
        TyrePressureRR = tyrePressureRR;
        BrakeTempFL = brakeTempFL;
        BrakeTempFR = brakeTempFR;
        BrakeTempRL = brakeTempRL;
        BrakeTempRR = brakeTempRR;
        TyreWearFL = tyreWearFL;
        TyreWearFR = tyreWearFR;
        TyreWearRL = tyreWearRL;
        TyreWearRR = tyreWearRR;
    }
}
