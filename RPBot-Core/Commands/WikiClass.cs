using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wiki = DotNetWikiBot;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.Entities;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Net;
using System.Linq;
using DotNetWikiBot;
using System.IO;

namespace RPBot
{
    class WikiClass
    {
        public static Dictionary<string, Action<string, Wiki.Page>> WikiFields = new Dictionary<string, Action<string, Wiki.Page>>();
        public static Wiki.Site WikiSite;


        string[] images =
                        {
           //     "File:ChihaSenshaTank.png",
"File:ChihaSensha.png",
"File:Check.png",
"File:CheckWeapon.png",
"File:Neptune1.png",
"File:Neptune2.png",
"File:Havoc.jpg",
"File:TyranicaEye.png",
"File:TyranicaCivil.png",
"File:TyranicaAngkor2.gif",
"File:TyranicaAngkor1.jpg",
"File:Tyranica3.png",
"File:Tyranica2.png",
"File:Tyranica1.png",
"File:ShaedRogue.jpg",
"File:Silver_Lion_-_Weapons.png",
"File:Silver_Lion.png",
"File:Kinzoku_Dangan.png",
"File:Nike.jpg",
"File:Wiki-wordmark.png",
"File:BnHA.png",
"File:KooriNoKokoro.jpg",
"File:InfamousMetalico.png",
"File:Praetoria.png",
"File:Dies_Irae_-_Enki.jpg",
"File:Enki.jpg",
"File:Mirage.png",
"File:Spectre_-_Mirage.png",
"File:Pietro_De_Rossi.png",
"File:Community-header-background",
"File:EssenceVengeance.jpeg",
"File:WhiteStarSuit1.jpg",
"File:WhiteStarCivilian1.jpg",
"File:WhiteStarSuit2.jpg",
"File:WhiteStarCivilian2.jpg",
"File:Praetoria-0.png",
"File:Shaed.jpg",
"File:ShaedCivilian.jpg",
"File:Kira.png",
"File:Armor.png",
"File:Tasha.jpg",
"File:Tasha's_gun.jpg",
"File:Fujiko_Akizuki.jpg",
"File:Hyakutake.png",
"File:Asuka.jpg",
"File:WHMap.png",
"File:Stray.png",
"File:MovieCentral.jpg",
"File:MovieCentralExterior.jpg",
"File:TheMall2.jpg",
"File:TheMall1.jpg",
"File:CapesNCowls.jpg",
"File:MammothPark3.jpg",
"File:MammothPark7.jpg",
"File:MammothPark8.jpg",
"File:MammothPark5.jpg",
"File:MammothPark6.jpg",
"File:MammothPark2.jpg",
"File:MammothPark4.jpg",
"File:MammothPark1.jpg",
"File:Dark's_suit2.jpg",
"File:Praetoria_Sword.png",
"File:Praetoria_Hero_Outfit.png",
"File:Hale_Sylon.png",
"File:Ookami1.jpg",
"File:ARTEMIS.REF",
"File:Info.png",
"File:Weaver.jpg",
"File:Vendetta2.jpg",
"File:Vendetta1.jpg",
"File:Ookami.jpg",
"File:Inferno-0.jpg",
"File:Sir_Randomizer.jpg",
"File:Inferno.jpg",
"File:DreadknightQOff.png",
"File:DreadknightQOn.png",
"File:DreadknightCasual.png",
"File:JessMiracleArmour.jpg",
"File:JessMiracleJustice.jpeg",
"File:JessMiracle.jpg",
"File:West_Wind.jpg",
"File:Eros3.png",
"File:Eros2.png",
"File:Eros.png",
"File:Silver_Mantis_Red.jpg",
"File:Don_Mistress.png",
"File:Leon.jpg",
"File:HiroshiHayate.jpg",
"File:AyreFlute.jpg",
"File:Ayre.jpg",
"File:PandoraHuman.jpg",
"File:PandoraDragon.png",
"File:Nile1.jpg",
"File:LaMuertaB.png",
"File:LaMuertaA.jpg",
"File:Fargo.jpg",
"File:FargoSuit.png",
"File:TheArena.jpg",
"File:EssenceHero.png",
"File:EssenceNewBod.png",
"File:VCivil.jpg",
"File:VVillain.jpg",
"File:EssenceSword.jpg",
"File:EssenceShield.png",
"File:EssenceIncinerator.jpg",
"File:EssenceIncinerator.gif",
"File:Alexis.jpg",
"File:Essence.jpg",
"File:Kaitlyn.jpg",
"File:Vulcan.jpg",
"File:VulcanRef.jpg",
"File:NMArmor.jpg",
"File:NMShadowForm.png",
"File:Omori.jpg",
"File:Gus.png",
"File:Tasha.png",
"File:LuciferPfp.png",
"File:TheHeroAcademySchedule.png",
"File:Jamie.jpg",
"File:AnaCivil.png",
"File:TriptychNest3.png",
"File:TriptychNest4.png",
"File:TriptychNest1.png",
"File:TriptychNest2.png",
"File:TriptychLower1.png",
"File:TriptychLower2.png",
"File:Triptych4.png",
"File:Triptych5.png",
"File:Triptych6.png",
"File:Triptych3.png",
"File:TriptychDana.jpg",
"File:Triptych2.png",
"File:Triptych1.png",
"File:NathanielGreeney.png",
"File:Maevery.jpg",
"File:Ray.png",
"File:Iris.jpg",
"File:Blake.png",
"File:Suburbs.png",
"File:VPfp.jpg",
"File:Celsius1.jpg",
"File:Celsius2.jpg",
"File:CelsiusPfp.jpg",
"File:ThresherSpike.jpg",
"File:Hyperion.png",
"File:HiroTatePfp.jpg",
"File:StarForgerPfp.png",
"File:PhotoFriend.png",
"File:Overdose.jpeg",
"File:Kyoko10.png",
"File:Kyoko7.png",
"File:Kyoko5.png",
"File:Kyoko4.png",
"File:Kyoko9.png",
"File:Kyoko1.png",
"File:Kyoko2.png",
"File:Kyoko3.png",
"File:Kyoko6.png",
"File:Kyoko8.png",
"File:KyokoPfp.png",
"File:Blank_Slate.jpg",
"File:Skylark.png",
"File:AugustWhite.jpg",
"File:DerekLjundberg.png",
"File:SolInvictus.png",
"File:Orion3.png",
"File:IceSpirit.jpg",
"File:Wiki-background",
"File:RedQueenGun.jpg",
"File:RedQueenBlade.jpg",
"File:RedPfp.jpg",
"File:LunaPfp.png",
"File:ChinoCar.jpg",
"File:ChinoMask.png",
"File:Chinopfp.jpg",
"File:E2c503204a4db60f44cf753635d6f73b-png.jpg",
"File:Asuka.JPG",
"File:Joker7.png",
"File:JokerDiana.png",
"File:Joker18.png",
"File:JokerBored.png",
"File:JokerCheerful.png",
"File:JokerPlotting.png",
"File:JokerPortal.jpg",
"File:JokerScythe.jpg",
"File:JokerJoker.jpg",
"File:JokerPfp.png",
"File:Cliff.jpg",
"File:NilePfp.jpg",
"File:AsuraEquip5.jpg",
"File:AsuraEquip4.jpg",
"File:AsuraEquip1.jpg",
"File:AsuraEquip2.jpg",
"File:AsuraEquip3.jpg",
"File:AsuraEquip.jpg",
"File:AsuraBack.jpg",
"File:Asura.jpg",
"File:Myles.jpg",
"File:Marcus.jpg",
"File:Reggie.jpg",
"File:Sylvester.jpg",
"File:Ronald.png",
"File:Jeremiah.png",
"File:TheSilkCourtWineMenu.png",
"File:TheSilkCourt4.png",
"File:TheSilkCourt3.png",
"File:TheSilkCourt2.png",
"File:TheSilkCourtMenu5.png",
"File:TheSilkCourtMenu4.png",
"File:TheSilkCourtMenu3.png",
"File:TheSilkCourtMenu2.png",
"File:TheSilkCourtMenu1.png",
"File:TheSilkCourt1.png",
"File:MatryoshkaTroikaMenu.jpg",
"File:BoozeNBikesBeerMenu.png",
"File:BoozeNBikesSidesMenu.png",
"File:BlackMarketLocations.png",
"File:Heroes_v_Villains_Logonew.png",
"File:Favicon.ico",
"File:TheCasino.jpg",
"File:ColbridgeAcademy2.jpg",
"File:ColbridgeAcademy1.png",
"File:CapesNCowlsMenu1.png",
"File:CapesNCowlsMenu2.png",
"File:PaladiumDrinkMenu.jpg",
"File:PaladiumMenu.png",
"File:Orion4.png",
"File:Orion5.png",
"File:Orion2.png",
"File:Orion1.png",
"File:BaronSleepy.jpg",
"File:BaronBrush.png",
"File:BaronScarf.png",
"File:RedStar1.jpg",
"File:RedStar2.jpg",
"File:RedStar3.jpg",
"File:RosaliceMoon.png",
"File:Barondoting-0.jpg",
"File:Baron.jpg",
"File:Barondoting.jpg",
"File:DotingBaron.png",
"File:Wolfpack.jpg",
"File:SoO_Crank.png",
"File:SoO_Brank.png",
"File:SoO_Arank.jpg",
"File:SoO_Arank2.jpg",
"File:SoO_Arank3.png",
"File:Cultist.png",
"File:Apophis.png",
"File:Symbol_-_Copy.jpg",
"File:IMG_20171030_204159.jpg",
"File:RedStar4.jpg",
"File:RedStar5.jpg",
"File:RedStar6.jpg",
"File:RedStar7.jpg",
"File:RedStar8.jpg",
"File:RedStar9.jpg",
"File:Enjin.png",
"File:RedStarPfp.jpg",
"File:MatryoshkaTroika.jpg",
"File:BoozeNBikes.jpg",
"File:Samsara.jpg",
"File:Screenshot_8.png",
"File:Whip.jpg",
"File:Shield.png",
"File:Beam2.jpg",
"File:Beam1.jpg",
"File:Orbe.jpg",
"File:Cssdsd.jpg",
"File:1mage.jpg",
"File:Im4ge.jpg",
"File:Staff.png",
"File:Tormenta.jpg",
"File:SpiceCook.jpg",
"File:Bikerh.jpg",
"File:Bik2.jpg",
"File:Bikeleader.jpg",
"File:Bike.png",
"File:WolfRh.png",
"File:Wolfleader.png",
"File:Wi_Eye.png",
"File:SpiceOwner.png",
"File:PI.jpg",
"File:Lyon.png",
"File:Uzi.gif",
"File:Tin_grenade.png",
"File:Molotov.png",
"File:Makarov.png",
"File:Baseballbat.png",
"File:Ak74u.png",
"File:BlyatMobile.jpg",
"File:Blyatman.png",
"File:CT_Inkform.png",
"File:Cartoonist.png",
"File:Khopesh.jpg",
"File:Pethawk.jpg",
"File:HcivilianWH.jpg",
"File:Horus-EndTS.jpg",
"File:Horus-BeginTS.jpg",
"File:Horus.jpg",
"File:20171031_152404.jpg",
"File:Arsen.png",
"File:212559_razorshader_the-dark-phantom.png",
"File:Tetsuya-Kuroko-kuroko-tetsuya-34859976-417-500.png",
"File:Unknown-4.png",
"File:Screenshot_3.png",
"File:Bd488628a288d1bf5f4a2233cb2b7d3a.jpg",
"File:Screenshot_5.png",
"File:Screenshot_7.png",
"File:Screenshot_6.png",
"File:Screenshot_1.png",
"File:Original.jpg",
"File:O.png",
"File:Project.jpg",
"File:REDESIGNsmh.jpg",
"File:Environment_concept_harbour_by_anarki3000-d2zcwda.jpg",
"File:DeadwaterAsylumGuards.png",
"File:DeadwaterAsylumWarden.png",
"File:DeadwaterAsylumInmate37.png",
"File:DeadwaterAsylum.png",
"File:C20110907_sd2020_01_cs1w1_290x.png",
"File:Aff46c8e0fe42f92ee43bee8310bb33a--art-and-illustration-illustrations.png",
"File:Lark's_Goons.png",
"File:Rin.png",
"File:Musa.png",
"File:Lark7.png",
"File:Lark6.png",
"File:Lark5.png",
"File:Lark4.png",
"File:Haemokinetic_Constructs.gif",
"File:Lark's_Blood_Transformation.png",
"File:Lark3.png",
"File:Lark2.png",
"File:Lark1.png",
"File:55ca26176dec694a26aacf59_5727d1d8fbfb470337d1562c_320.jpg",
"File:Fd081306794d3be9c9e1576b5de55328473c3f47_hq.jpg",
"File:928cce006cac671bcae99316ab737924--anime-oc-anime-manga.jpg",
"File:Images.jpg",
"File:624feca26b138a11bf2bac57bc0d6aa4--anime-oc-anime-guys.jpg",
"File:81d4d99ddf6eeb52e5487282dad3496b.jpg",
"File:474293_K9RRN_1095_008_100_0000_Light-Soft-GG-Supreme-belt-bag.jpg",
"File:Nile2.jpg",
"File:Amari.jpg",
"File:Kaminari.jpg",
"File:Kami.png",
"File:Arrogant.png",
"File:280109.jpg",
"File:Untitled-0.jpg",
"File:219474651-1.png",
"File:Image.jpg",
"File:Midnight-tiger-2.png",
"File:Dea7a1bcb31a591458f22704a6415ba0--midnight-tiger-alternative-comics.png",
"File:67f913b9919730113270deeb2652f62d--superheroes-tigers.png",
"File:08c77dd9bdd22b43c9abe12a1fb0ccaf--book-themes-digital-comics.png",
"File:Khvjdihvbs.jpg",
"File:Untitled-0.png",
"File:Tsu_amari.png",
"File:Superb_-2_Review_Feature_Cover.jpg",
"File:25fbcacf313fc8eba494e4cb84fde6d0--art-drawings-anime-guys.jpg",
"File:13017_2.jpg",
"File:C366e299e407022da5e898b0a7fff9d4.png",
"File:8748199dbbe4c80d8fc6802cc7951438--superhero-ideas-book-characters.png",
"File:6b5ce74422b5e2e57c6f2bfd95e6da34.png",
"File:Bc1e8f9c8300f42e363b3d316c246ab6.png",
"File:9ed2048a79d504b84c12f1e4fe3d007a.png",
"File:9fe00e3c059940018391de292f2f8816.png",
"File:He_s_the_sound_i_m_the_fury_by_genekelly.png",
"File:07acd02aa6527c55ebfeadde6ad4e1fc.png",
"File:7ddf4ad970679685c87a9479ec87a47f.png",
"File:Unnown.png",
"File:Unknown.png",
"File:Imge.png",
"File:Image.png",
"File:Imag.png",
"File:6520005012c2d3775f10a9f2839cf3ad.jpg",
"File:0884809dd334cb6f7dfeddf75116139d.jpg",
"File:Psira_1.png",
"File:1309230405815.png",
"File:Tiberium_concept2020Tiberium_scientist.png",
"File:31a4bce36dfffb68354fb4239aa460db.png",
"File:Alle-page-bsf.png",
"File:4ce551388346ac805d25d7cb59e6fd1c.png",
"File:3e0f6e5ce34c39ada9d0ef8dfe9a3c2d--npc-rpg-villager-bara.png",
"File:942b1b6fbc3e6e9e594aac064795cf38--character-concept-art-character-design.png",
"File:Bfe1bf48169890296569be3586b24f24.png",
"File:Bd349973f749845c30d716f47651257d--alternative-comics-superhero-ideas.png",
"File:Interview-blue_girl_smoke.png",
"File:Interview-zandora.png",
"File:Promo325469429.png",
"File:Untitled.png",
"File:Brauk.jpg",
"File:20171010_182042.jpg",
"File:Form1.jpg",
"File:Gabriel-Dardec.jpg",
"File:Misty_Villain_Nicole_Cardiff.png",
"File:Misty_Civilian_Nicole_Cardiff.jpg",
"File:Nensha.png",
"File:Gaku.jpg",
"File:Gaku.png",
"File:Silver_Falcon.png",
"File:Alistair.png",
"File:Crescent.png",
"File:Elizabeth_Misawa.jpg",
"File:X-Girl-Candys-World-Doll-Divine-portrait2.jpg",
"File:Sting_eucliffe_by_esteban_93-d9p58qe.png",
"File:Viaan_Manifesting_his_secondary_shield_inside_the_primary_shield.png",
"File:Subcon_full_power_Psion.png",
"File:Viaan_demonstrating_his_quirk.png",
"File:40px-Cquote2.svg.png",
"File:40px-Cquote1.svg.png",
"File:Dreamer.jpg",
"File:WHA_uniform.png",
"File:Mystic(present).jpg",
"File:Aki.jpg",
"File:Police_commissioner.png",
"File:Psion_before_timeskip.png",
"File:Viaan_(After_timeskip).png",
"File:CommanderLovve.png",
"File:WayHavenPDCleaners.png",
"File:WayHavenPDCaptains.png",
"File:WayHavenPDHovercraft.png",
"File:WayHavenPDLieutenants.png",
"File:WayHavenPDCar.png",
"File:WayHavenPDSergeants.png",
"File:WayHavenPD.png",
"File:Zephyr.png",
"File:Isei_by_yourmansjibbz-dbjvpv6.png",
"File:Finnick_by_yourmansjibbz-dbjvp9f.png",
"File:Von_by_yourmansjibbz-dbjtzqm.png",
"File:Andriel_by_yourmansjibbz-dbjtwv9.png",
"File:Mayor_Theodore_Hill.png",
"File:Wesley_Gibbons.png",
"File:Amazon.png",
"File:The_Alchemist.png",
"File:Hayato.jpg",
"File:U34ntitled.png",
"File:Avalon060s.png",
"File:AvalonTheMaker.png",
"File:AvalonInterior.jpg",
"File:Avalon_Interior.png",
"File:AvalonResearchBuilding.png",
"File:GovernmentHQ.jpg",
"File:GovernmentHQCrows.png",
"File:PaladiumUnderground.png",
"File:TheCasinoEspinozaFamily.png",
"File:TheCasinoHitmen.png",
"File:Paladium.jpg",
"File:ColbridgeAcademy3.png",
"File:PaladiumKazuki.png",
"File:Mizune.png",
"File:PaladiumInterior.png",
"File:Droids.png",
"File:Sawbones.png",
"File:Pristina.png",
"File:WayHaven_Hospital.png",
"File:DeadEye.png",
"File:Shrill.png",
"File:TheHeroAcademy2.png",
"File:TheHeroAcademy1.png",
"File:HeroHQTheGuardians.png",
"File:Deadwater_Asylum.png",
"File:Mammoth_Armored.png",
"File:Mammoth.png",
"File:Psion.png",
"File:Eolosix_Civilian.jpg",
"File:Ff4_by_anklesnsocks-d50qrhx.jpg",
"File:League_by_corankizerstone-d70kwc5.jpg",
"File:373ddfe2bc9b68f9191dd09b75e37f77.png",
"File:SteamShackles.jpg",
"File:SteamSpider.jpg",
"File:SteamSniper.jpg",
"File:Steampunk_Capsules.jpg",
"File:Steam_Gauntlets.jpg",
"File:Steam_demon.png",
"File:SuperSteams.jpg",
"File:SteamSuit.jpg",
"File:Steam_Cycle.jpg",
"File:Eolosix.png",
"File:Link.png",
"File:Dante_Volere.png",
"File:Osiris_Robed.png",
"File:Steamshock.jpg",
"File:Nightmare.gif",
"File:Nightmare_taking_his_mask_off.gif",
"File:Nightmare.jpg",
"File:X.png",
"File:Mokusatsu.png",
"File:Drystan.png",
"File:Not_available.jpg",
"File:WayHaven.png",
"File:Tabitha.jpg",
"File:Gilzean.jpg",
"File:Misaki.jpg",
"File:Evra.jpg",
"File:Raina.jpg",
"File:Cal.jpg",
"File:Fbe39ad8f78eb6dbdc7b8c484607f2c3.jpg",
"File:Anne.png",
"File:Hana.jpg",
"File:Mai.jpg",
"File:Kai.jpg",
"File:EJwVzMsNwyAMANBdGAAT85HJNoggghQCws6p6u5tru_wPupZl9rVKTJ5Bzga57EOzTJWqkXXMepV0mys8-iQRFI-e7mFwW6OXDToYyCy6NH-yRvajMXgTMAQKELrbzPvqr4_vy8hzw.jpeg",
"File:705204.jpg"
            };

        public static void InitWiki()
        {
            // WikiSite = new Wiki.Site("http://roleplay-heroes-and-villains.wikia.com", "jcryer", "Gracie3038");
             WikiSite = new Wiki.Site("http://heroes-and-villains-rp.wikia.com", "jcryer", "Gracie3038");

            Action<string, Wiki.Page> title = (string message, Wiki.Page page) => page.title = message;
            WikiFields.Add("Enter Title", title);

            Action<string, Wiki.Page> personality = (string message, Wiki.Page page) => page.text += Environment.NewLine + "== Personality ==" + Environment.NewLine + message;
            WikiFields.Add("Enter Personality text", personality);

            Action<string, Wiki.Page> backstory = (string message, Wiki.Page page) => page.text += Environment.NewLine + "== Backstory ==" + Environment.NewLine + message;
            WikiFields.Add("Enter Backstory text", backstory);

            Action<string, Wiki.Page> resources = (string message, Wiki.Page page) => page.text += Environment.NewLine + "== Resources ==" + Environment.NewLine + message;
            WikiFields.Add("Enter Resources text", resources);

            Action<string, Wiki.Page> equipment = (string message, Wiki.Page page) => page.text += Environment.NewLine + "=== Equipment/Weaponry ===" + Environment.NewLine + message;
            WikiFields.Add("Enter Equipment text", equipment);

            Action<string, Wiki.Page> specialisations = (string message, Wiki.Page page) => page.text += Environment.NewLine + "=== Specializations ===" + Environment.NewLine + message;
            WikiFields.Add("Enter Specialisations text", specialisations);

            Action<string, Wiki.Page> quirk = (string message, Wiki.Page page) => page.text += Environment.NewLine + "== Quirk ==" + Environment.NewLine + message;
            WikiFields.Add("Enter Quirk text", quirk);

            Action<string, Wiki.Page> versatility = (string message, Wiki.Page page) => page.text += Environment.NewLine + "=== Versatility ===" + Environment.NewLine + message;
            WikiFields.Add("Enter Quirk Versatility text", versatility);

            Action<string, Wiki.Page> example = (string message, Wiki.Page page) => page.text += Environment.NewLine + "=== Example ===" + Environment.NewLine + message;
            WikiFields.Add("Enter Quirk Example text", example);
        }

        [Command("testing")]
        public async Task TestAsync(CommandContext e, string characterName)
        {
            Wiki.Page page = new Wiki.Page(WikiSite);

            //   page.text = @"{{Character|pagetitle = Neptune|image = Neptune.ref2.png|civilian_name = Brook Oceane|relatives = Arnold Oceane (Father, Deceased)|affiliation = Pro Hero|marital_status = Single|age = 25|date_of_birth = ?/?/1994|place_of_birth = Boston, USA|species = Human|gender = Male|height = 6'0" + "\"" + " | weight = 80 kg, 176 lb | hair_color = Blond | eye_color = Aqua Blue}}";

            /*
             * {{Character|
             * pagetitle = Neptune|
             * image = Neptune.ref2.png|
             * civilian_name = Brook Oceane|
             * relatives = Arnold Oceane (Father, Deceased)|
             * affiliation = Pro Hero|
             * marital_status = Single|
             * age = 25|
             * date_of_birth = ?/?/1994|
             * place_of_birth = Boston, USA|
             * species = Human|
             * gender = Male|
             * height = 6'0" |
             * weight = 80 kg, 176 lb |
             * hair_color = Blond |
             * eye_color = Aqua Blue}}
             * */

            string[] test =
            {
                "pagetitle", "image", "civilian_Name", "relatives", "affiliation",
                "marital_status", "age", "date_of_birth", "place_of_birth", "species", "gender",
                "height", "weight", "hair_color", "eye_color"
            };

            string infobox = "{{Character|";

            var interactivity = e.Client.GetInteractivity();

            foreach (var pair in WikiFields)
            {
                await e.RespondAsync(pair.Key);

                var msg = await interactivity.WaitForMessageAsync(x => x.Author == e.Member, TimeSpan.FromSeconds(120));
                if (msg != null)
                {
                    pair.Value(msg.Message.Content, page);
                }
            }

            var embed = new DiscordEmbedBuilder()
            {
                Title = "Infobox",
                Color = DiscordColor.Red
            };
            var message = await e.RespondAsync(embed: embed);
            foreach (var infoBoxField in test)
            {
                var message2 = await e.RespondAsync("Please enter the: " + infoBoxField);

                var msg = await interactivity.WaitForMessageAsync(x => x.Author == e.Member, TimeSpan.FromSeconds(120));
                if (msg != null)
                {

                    if (infoBoxField == "image")
                    {
                        Regex ItemRegex = new Regex(@"\.(png|gif|jpg|jpeg|tiff|webp)");
                        if (ItemRegex.IsMatch(msg.Message.Content))
                        {
                            Wiki.Page p = new Wiki.Page(WikiSite, "File:" + msg.Message.Content.Split('\\').Last());
                            p.UploadImageFromWeb(msg.Message.Content, "N/A", "N/A", "N/A");
                        }
                        infobox += infoBoxField + " = " + msg.Message.Content + "|";
                        embed.AddField(infoBoxField, msg.Message.Content);
                        await message.ModifyAsync(embed: embed);
                        await message2.DeleteAsync();
                        await msg.Message.DeleteAsync();
                    }
                }

                page.text = page.text.Insert(0, infobox.TrimEnd('|') + "}}");

                page.Save();
            }
        }

        [Command("aaa")]
        public async Task aaa(CommandContext e)
        {


            //          PageList p = new PageList(WikiSite);
            //  foreach (string cat in categories) {
            //   p.FillFromAllPages("", 0, true, 1000);
            //limit=500&user=&title=Special%3AListFiles&
            //        var x = WikiSite.GetApiQueryResult("user=", "title=Special:ListFiles", 1000);

            // p.FillFromCustomSpecialPage("ListFiles", 10000);
            //p.FillFromCategoryTree("Category:All_Characters");
            //  }
            //WikiSite.fil

           // string[] images = Directory.GetFiles("Images");
            int count = 0;
            foreach (string imageName in images)
            {
                count++;
                Wiki.Page l = new Wiki.Page(WikiSite, "File:" + imageName);
                l.UploadImageFromWeb("http://51.15.222.156/wiki/" + imageName.Replace("File:", ""), "N/A", "N/A", "N/A");
                Console.WriteLine(count);
                Console.ReadLine();
            }
        }
    }
}

