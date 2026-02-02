[System.Serializable]
public class RadarPayload
{
    public string timestamp;
    public string radar_id;
    public float frequency_ghz;
    public TargetData target;
    public EnvironmentData environment;
    public SignalData signal;
}

[System.Serializable]
public class TargetData
{
    public float range_m;
    public float velocity_mps;
    public float rcs_sqm;
}

[System.Serializable]
public class EnvironmentData
{
    public float rain_rate_mmph;
    public float attenuation_db;
}

[System.Serializable]
public class SignalData
{
    public float doppler_hz;
    public float received_power_dbm;
    public float snr_db;
}
