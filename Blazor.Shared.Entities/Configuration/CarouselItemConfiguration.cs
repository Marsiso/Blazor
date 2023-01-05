using Blazor.Shared.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blazor.Shared.Entities.Configuration;

public class CarouselItemConfiguration : IEntityTypeConfiguration<CarouselItemEntity>

{
    public void Configure(EntityTypeBuilder<CarouselItemEntity> builder)
    {
        builder.HasData(new List<CarouselItemEntity>
        {
            new()
            {
                Id = 1,
                Alt = "iPhone 14 Pro 128GB fialová",
                Caption = "Mobilní telefon - 6,1\" OLED 2556 × 1179, 120Hz, procesor Apple A16 Bionic 6jádrový, RAM 6 GB, interní paměť 128 GB, zadní fotoaparát s optickým zoomem 48 Mpx (f/1,78) + 12 Mpx (f/1,78) + 12 Mpx (f/2,2), přední fotoaparát 12 Mpx, optická stabilizace, GPS, Glonass, NFC, LTE, 5G, Lightning port, voděodolný dle IP68, single SIM + eSIM, neblokovaný, rychlé nabíjení 20W, bezdrátové nabíjení 15W, iOS 16"
            },
            new()
            {
                Id = 2,
                Alt = "Garmin Tactix 7 Pro Solar Sapphire Ballistics",
                Caption = "Chytré hodinky - pánské, s ovládáním v češtině, transflektivní MIP displej, GPS, NFC platby skrze Garmin Pay, měření tepu, monitoring spánku, krokoměr, oxymetr, barometr, hledání telefonu, přehrávač hudby v hodinkách, ovládání přehrávače hudby v mobilu, předpověď počasí, režim spánku/nerušit, obchod s aplikacemi, navigace a integrované mapy, vhodné na běh, cyklistiku, jógu, golf, plavání, vodotěsnost 100 m (10 ATM), max. výdrž baterie 888 h, materiál pouzdra titan"
            },
            new()
            {
                Id = 3,
                Alt = "JURA E6 Platin",
                Caption = "Automatický kávovar tlak 15 bar, automatické vypnutí, cappuccino a latte, časovač, displej, funkce horké vody, mléčný systém, mlýnek na kávu, nahřívač šálků, nastavení množství kávy, nastavení množství vody, odkapávací systém, odvápňovací systém, parní tryska, příprava dvou šálků najednou a samočisticí systém, objem nádržky na vodu 1,9 l, velikost zásobníku mlýnku 280 g, příkon 1450 W, šířka 28 cm, výška 34,6 cm, hloubka 44,4 cm, hmotnost 9,8 kg"
            },
            new()
            {
                Id = 4,
                Alt = "MSI GeForce RTX 3080 VENTUS 3X PLUS 10G OC LHR",
                Caption = "Grafická karta - 10 GB GDDR6X (19000 MHz ), NVIDIA GeForce, Ampere (GA102, 1440 MHz), Boost 1740 MHz, PCI Express x16 4.0, 320Bit, DisplayPort 1.4a a HDMI 2.1, LHR (Low Hash Rate)"
            },
            new()
            {
                Id = 5,
                Alt = "AMD Ryzen 5 5600X",
                Caption = "Procesor 6 jádrový, 12 vláken, 3,7GHz (TDP 65W), Boost 4,6 GHz, 32MB L3 cache, bez integrovaného grafického čipu, socket AMD AM4, Vermeer, box chladič, Wraith Stealth"
            }
        });
    }
}
