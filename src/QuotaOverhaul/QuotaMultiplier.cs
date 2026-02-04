using System.Collections;
using System.Collections.Generic;
using LethalNetworkAPI;

namespace QuotaOverhaul
{
    public class QuotaMultiplier
    {
        private static readonly List<QuotaMultiplier> multipliers = [];

        public readonly string name;
        private readonly LNetworkVariable<double> multiplier;
        public readonly double defaultMultiplier;
        public readonly bool persistent;

        public QuotaMultiplier(string name, double defaultMultiplier = 1d, bool persistent = true)
        {
            this.name = name;
            this.defaultMultiplier = defaultMultiplier;
            multiplier = LNetworkVariable<double>.Connect(this.name, onValueChanged: SyncToClients);
            multiplier.Value = defaultMultiplier;
            this.persistent = persistent;

            multipliers.Add(this);
        }

        private void SyncToClients(double oldValue, double newValue)
        {
            multiplier.Value = newValue;
        }

        public double Get()
        {
            return multiplier.Value;
        }

        public void Set(double value)
        {
            if (!GameNetworkManager.Instance.isHostingGame) return;
            multiplier.Value = value;
            QuotaOverhaul.UpdateProfitQuota();
        }

        public void Increase(double increase)
        {
            Set(Get() + increase);
        }

        public void Reset()
        {
            Set(defaultMultiplier);
        }

        public static void SaveAll(string saveFile)
        {
            foreach (QuotaMultiplier multiplier in multipliers)
            {
                multiplier.Save(saveFile);
            }
        }

        public void Save(string saveFile)
        {
            if (!GameNetworkManager.Instance.isHostingGame) return;
            if (!persistent) return;
            ES3.Save(name, multiplier, saveFile);
        }

        public static void LoadAll(string saveFile)
        {
            foreach (QuotaMultiplier multiplier in multipliers)
            {
                multiplier.Load(saveFile);
            }
        }

        public void Load(string saveFile)
        {
            if (!GameNetworkManager.Instance.isHostingGame) return;
            if (!persistent) return;
            if (!ES3.KeyExists(name, saveFile)) return;
            double value = ES3.Load<double>(name, saveFile);
            Set(value);
        }

        public static int GetQuotaWithMultipliers(int? baseQuota = null, List<QuotaMultiplier>? blacklist = null)
        {
            double result = baseQuota is null ? QuotaOverhaul.GetBaseProfitQuota() : (int)baseQuota;
            List<QuotaMultiplier> list = multipliers;
            if (blacklist is not null)
            {
                foreach (QuotaMultiplier multiplier in blacklist)
                {
                    list.Remove(multiplier);
                }
            }

            foreach (QuotaMultiplier multiplier in list)
            {
                double beforeMultiplier = result;
                result *= multiplier.Get();
                Plugin.Log.LogDebug("Applying quota multiplier: " + multiplier.name + "\n" + beforeMultiplier + " * " + multiplier.Get() + " = " + result);
            }
            return (int)result;
        }
    }
}
