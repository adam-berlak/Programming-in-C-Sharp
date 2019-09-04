using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Frontend1;

public class VendingMachine {

    Dictionary<int, int> coinKindToHopperIndex;
    Dictionary<string, int> popKindToHopperIndex;
    List<Coin>[] coinHopper;
    List<Pop>[] popHopper;
    List<int> popCosts;

    List<Coin> coinsMade;
    List<Coin> coinsInLimbo;

    List<Deliverable> deliveryChute;

    public VendingMachine(List<int> coinKinds, int popKindCount) {
        var hashCoinKinds = new HashSet<int>(coinKinds);
        if (hashCoinKinds.Count != coinKinds.Count) {
            throw new Exception("Non-unique coin kinds");
        }
        if (hashCoinKinds.Where(ck => ck <= 0).Count() > 0) {
            throw new Exception("Zero or negative coin kinds are not allowed");
        }

        this.coinKindToHopperIndex = new Dictionary<int, int>();
        this.coinHopper = new List<Coin>[coinKinds.Count];
        for (int i = 0; i < coinKinds.Count; i++) {
            this.coinHopper[i] = new List<Coin>();
            this.coinKindToHopperIndex[coinKinds[i]] = i;
        }
        this.popKindToHopperIndex = new Dictionary<string, int>();
        this.popHopper = new List<Pop>[popKindCount];
        for (int i = 0; i < popKindCount; i++) {
            this.popHopper[i] = new List<Pop>();
        }
        this.deliveryChute = new List<Deliverable>();
        this.coinsMade = new List<Coin>();
        this.popCosts = new List<int>();
        this.coinsInLimbo = new List<Coin>();
    }

    public void ConfigureVendingMachine(List<string> popNames, List<int> popCosts) {
        this.popKindToHopperIndex.Clear();
        for (int i = 0; i < popNames.Count; i++) {
            this.popKindToHopperIndex[popNames[i]] = i;
        }
        this.popCosts.Clear();
        this.popCosts.AddRange(popCosts);
    }

    public void LoadCoins(int coinKindIndex, List<Coin> coins) {
        this.coinHopper[coinKindIndex].AddRange(coins);
    }

    public void LoadPops(int popKindIndex, List<Pop> pops) {
        this.popHopper[popKindIndex].AddRange(pops);
    }

    public List<IList> UnloadVendingMachine() {
        var unusedCoins = new List<Coin>();
        foreach (var hopper in this.coinHopper) {
            unusedCoins.AddRange(hopper);
            hopper.Clear();
        }
        var unsoldPops = new List<Pop>();
        foreach (var hopper in this.popHopper) {
            unsoldPops.AddRange(hopper);
            hopper.Clear();
        }

        var returnList = new List<IList>();
        returnList.Add(unusedCoins);
        returnList.Add(new List<Coin>(this.coinsMade));
        returnList.Add(unsoldPops);

        this.coinsMade.Clear();

        return returnList;
    }

    public List<Deliverable> ExtractFromDeliveryChute() {
        var deliveryChuteContents = this.deliveryChute;
        this.deliveryChute = new List<Deliverable>();

        return deliveryChuteContents;
    }

    public void InsertCoin(Coin coin) {
        if (this.coinKindToHopperIndex.Keys.Contains(coin.Value)) {
            this.coinsInLimbo.Add(coin);
        }
        else {
            this.deliveryChute.Add(coin);
        }
    }

    public void PressButton(int popIndex) {
        var popCost = this.popCosts[popIndex];

        if (popIndex > this.popHopper.Length) {
            throw new Exception("Cannot press a button that doesn't exist!");
        }
        if (this.paidMoney() < popCost) {
            return;
        }
        if (this.popHopper[popIndex].Count == 0) {
            return;
        }
        var pop = this.popHopper[popIndex][0];
        this.deliveryChute.Add(pop);
        this.popHopper[popIndex].Remove(pop);
        this.deliveryChute.AddRange(this.makeChange(popCost));
        this.coinsMade.AddRange(this.coinsInLimbo);
        this.coinsInLimbo.Clear();
    }

    private int paidMoney() {
        return this.coinsInLimbo.Sum(c => c.Value);
    }

    private List<Coin> makeChange(int cost) {
        var money = this.paidMoney();
        var coinsInChute = new List<Coin>();
        var changeNeeded = money - cost;

        while (changeNeeded > 0) {
            var hoppersWithMoney = this.coinKindToHopperIndex.Where(h => h.Key <= changeNeeded && this.coinHopper[h.Value].Count > 0).OrderByDescending(h => h.Key);

            if (hoppersWithMoney.Count() == 0) {
                //Console.WriteLine("Woohoo! Customer is screwed out of " + changeNeeded + " pennies!");
                return coinsInChute;
            }

            var biggestHopper = hoppersWithMoney.First();

            changeNeeded = changeNeeded - biggestHopper.Key;
            var coin = this.coinHopper[biggestHopper.Value][0];
            this.coinHopper[biggestHopper.Value].Remove(coin);
            coinsInChute.Add(coin);
        }

        return coinsInChute;
    }
}