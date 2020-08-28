using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class WeaponProperties
{
    public float drawTime { private set; get; }
    public float reloadTime { private set; get; }
    public float rateOfFire { private set; get; }
    public float damageAvg { private set; get; }
    public float damageStdDev { private set; get; }
    public float ammoUsage { private set; get; }
    public float magazineSize { private set; get; }

    public WeaponProperties(float drawTime, float reloadTime, float rateOfFire, float damageAvg, float damageStdDev, float ammoUsage, float magazineSize)
    {
        this.drawTime = drawTime;
        this.reloadTime = reloadTime;
        this.rateOfFire = rateOfFire;
        this.damageAvg = damageAvg;
        this.damageStdDev = damageStdDev;
        this.ammoUsage = ammoUsage;
        this.magazineSize = magazineSize;
    }

}
