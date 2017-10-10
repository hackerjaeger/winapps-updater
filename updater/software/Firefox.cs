﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017  Dirk Stolle

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Firefox, release channel
    /// </summary>
    public class Firefox : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for Firefox class
        /// </summary>
        private static NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Firefox).FullName);



        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Firefox(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException("langCode", "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
            }
            if (!d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// gets a dictionary with the known checksums for the installers (key: language, value: checksum)
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/56.0.1/SHA512SUMS
            var result = new Dictionary<string, string>();
            result.Add("ach", "97fcb42a7b2acbb11125ed4c9a2cdddfef6545bc63629779ad856e5e7d0427af679ad7e3f3179a7a8239f6aab53af751061815f79aaf61b47586224904c0a648");
            result.Add("af", "6d1c11b5eb7ab48f487652245610763e5a6cb6919b81e598d7f305684d30445d8577a8193c67a838068e4fde993ef26784a641c24146e5fd0571345a71570f49");
            result.Add("an", "5f455a750103238d6dfd81eb5df1fe86f3315aab4dda3cfbeb266392a7b1a05347df8a2ff1565d997cbf3935033d0e3defa804678ac131778e33e7feb7fd04e0");
            result.Add("ar", "2e5df73d1466ee770593d269ca3619691dfb3c49e4de478702e1069397c3b9d3c342d51383545adc2cdf6db294a25f136f0b76996694192dceaf5990dbe66b9f");
            result.Add("as", "08a4a70d9c8d7fc75845404c4b2dc4d3e8b1e67fa5691e58546b3cd852becd540a167da900c151558425bc8cc4664ee2572d0dff8c97a837f71ed561773b1935");
            result.Add("ast", "78c99d21ef915e4d3a506cf5a91acfc017407a3fe2d227cfc9d8becc20eb3ada34416b8c3af9823a5eb7db66382fd381de30ca1e43303294fc3f202622dd0d51");
            result.Add("az", "74e95e45e1fee8d3caf84d38751407c1e4848d9280b1549a77595c107a0a4e49bdd98160d710afc4905422b0764a146c3c39fec0da7683e45a77a95357a689c7");
            result.Add("be", "5b90289d6066e1513b7c5b9ba8a74099b62d2e49dddc87b91f1a4f0cb7986ce7d3392cf5de915574dfc5005ea1ff9d69cd1e10b9aac3c768e16e615a0d5053d4");
            result.Add("bg", "c3b20b65a74deed91952306e0759ae5ada06b378525ad67ae15f8a005bffa0bc264bbccab07593c73c0643425ee0771f7e942fce9ba70af9e8cad63bf669ce56");
            result.Add("bn-BD", "b62f66146f2d800b0c3c5aabd8160cda6213301d769f6270963edf36f6a3035f4921bc116742af9f0c071fcabbf40fb10c156f0507c9bf3699b467559fae8204");
            result.Add("bn-IN", "3713c6d9ca94dabccc7abf92717ad42ff220e98ca60cc900aca369223e0d3135fef22a57a388246d622f93915b455beb50cf1a156147ced1cd03f02199b549be");
            result.Add("br", "a1ca78ce4d6dc8c520f335928a93a3c599f069426a0018dbf65be1278a8ed948da16fb9ba93b797cda2de9dc00e18fd503a3903c5c33ece6b37a19557025b3e8");
            result.Add("bs", "789cc02036511f11492817ab8085d45aee1ef01ae0e40fc7824467ec4032cf1d3d8b30a567336a0ab45d4f7588c724d78f527c22e0aaa2240fa192478755cbcc");
            result.Add("ca", "e29adc4fa81dec41971921191627dea005217a0843bc5a8092700a50a3317e1bf4b20ae9e23f7de66bfe24c2107735a986973ccfda4bd33328fedadcd29e7b76");
            result.Add("cak", "81294284a032b9eea8114668fb4ac029089e426030fdead855e590ea632f61f05fa6d37727b68ef4df8c06f5101b4b8bfb27f0ff9f2463afb657491b872d7501");
            result.Add("cs", "74a4e0a1eec2bfdba3f5824aeb7727e6730550e319959303e6168e041910e0132a35ed67ad15bc8866d5c7a64cfbc3d285b491a6ed75b2c9a25ff377442f5cad");
            result.Add("cy", "f0dce8c793a81e2f9fa022774c3af61c2b58aa54919e8de396abf11315f61c96e05d38a195b20613ad47c388bf0b2b774d540489cd0f3daa1b0f5b01f07f3065");
            result.Add("da", "66ce8555b6660928125c356d710b00710345726a75c3e6daf22deaf283c8b871018e3bf6f3267a11730e73b76365d170138ee9544bad45402318011bb264f55c");
            result.Add("de", "8b5e48f6ec44316061c731b54ce02977ca62e6fbbdfdd358c469b85ab5183e82cd3a8bfcb6b4391efa5b1c144cad9a2a43bc4b35f028547aebd0ad427eaae10a");
            result.Add("dsb", "f2fe9fcb6ebf89c8176b39242d688b5366bec0489d66d34ba85efc7a043687fbe5d46ff3d7165379d1aae09819d8dbaf7154d4b9661f8f3210769b71a3c4d9ff");
            result.Add("el", "4136e5ba9d230f10450db76174cf3bd0af768117a5c869c8772b69a0eaad094290b4f601b48a0064e5ada4be6d635fd68492add13e58ec06ef9e456700597ae3");
            result.Add("en-GB", "ad81e22489a88c6ca1fae679ed569f6a58faf08a5d2e744f73f683773b3c62d3801c08e76975e1a0821054ae8161378082c0def7ae67c6401de72f914ae2e3a9");
            result.Add("en-US", "f6ebf0d72861f835c9db6d8a5d9c13931bc67d58058897ee1f3e10a800cecdc2f42b4e02cc3a05266d6a9b3cc886af48aa52979199d8b1442ab00d625c483158");
            result.Add("en-ZA", "eee2f00cc8b273d172f7f3365f0626194c4759014a69640b2ff762d5b9fab5b77146cffae00214782980da6f442d6ec6c4a4f4e851dad5c186f9e7e745c35ee2");
            result.Add("eo", "7a2f460a8fd1be750300a6faf81775f04945c7080245dd90b459d0e1b5ccf809129f2d84103f2d25051c6c8c00da1411e2747128638f1da9f88c662f3cde5bef");
            result.Add("es-AR", "125635e87385babfec2aed44426642d966bd61a1804d89964d378b77abd9f1ab5606029444d2d88eb403c8fd96247197dd9b04ea4a0b9f30633559cd65da29bb");
            result.Add("es-CL", "2ccbc82ac24fc623f1aaebca70a199b6f300cbf3cb718d9d58890ddcefab77aedf22c8bffe2220cfd1d2a5b38c119a30a8973f196afca6aa1c392c37d74c5513");
            result.Add("es-ES", "f710756b1e083472b32b93bce2e396d7e41715e5da4dd2abcfb37a4c3e9e243befb2c648763c4eb29d5dfb610497d3c128e323d6d4df38b88a0b278b6e3b3318");
            result.Add("es-MX", "9d8f56571a455840b9bac9ad1b610f7ebbe2cf52379ffc828a35c96bbcb5d4caf5453a77e76a90e292965734fcc55d4425a40daf25bc38f7b10da3c2e6d03e07");
            result.Add("et", "7da33f97f28ccd5e901e5b5a6056e743c963f62a926e24601238045a4c4a638257fd825e64c92d22288dbd075885ddab5ed061144d45644008cec8a2b5070b30");
            result.Add("eu", "bd7d9d5260a66121673b6c6a9f3c738ec24add671eb7b59caf5982d82049294a147958b671c799571f10394057e6439a5bd5f3aee367421a864b9c26756f936d");
            result.Add("fa", "97733e878100d535e5023210968beb2ff8948176a70733657e37051feab145d39615e6adbdafed2ed66293a5c1fa4774ef815a833797813d9c5360a3b8d83d95");
            result.Add("ff", "608dcf23d7aa9e1d3b0d46c24b0aaf5eea28b9056b251266b0771deb6801a179a36d6366b4d11451a494aebfff437f1e7bfcc21adf4c69f3200f19ca5587d942");
            result.Add("fi", "9e7ef1fbddb17be4eda83cf5706054189dd4f3aa4dbe8dd3994ed2a6f7873e8fbbdb4fd105005d127548c11d34630913ce6b2755eae75e8e6a20c89cd21eecf1");
            result.Add("fr", "cba08bf1631a700a94aed3087e303ad916112d64753a1e269c107aebcd3c07130665a45e01c96e0100c8cabe9edaec39144d60478641c3993956b9e0974bb662");
            result.Add("fy-NL", "5869fcddce5baa3216e4e0e580c4e8a3fea9f042516d6cd131809412424ac98d3d430c076e523b1d5c79a775ebe89db8f593416aff93c75a64d09f63e243be8e");
            result.Add("ga-IE", "294045de2096f2c92cd1b480a048a08c757f6de2cac988c9cf2c281cbd44e9e18e67f2b11e7b774ea6208d0ce62730eb57d7ee47ca000013f517c2e37a7a8948");
            result.Add("gd", "811c87435a4dc9c2d6de433700283bc45112d9a2f21c77bc3763a4e135253a2e29ed4d885f53590ebbe6e13dd0f7961559ba202da6fe119d37555c5c9315ddcb");
            result.Add("gl", "585357eb932909d2e4e6304f0b0b7a20efb9eb1857cfcee6ed36ad032ed5a8c1045e4ba3c6ccffafd1c3a829ad3aa5888655c89cafa68016c1806cbea2ab0e56");
            result.Add("gn", "328dbc8b38752b70707ae110768e4c9579c6b59cbd345b29eeb019ebf9a0196707db11ba903aa6673ea357191f135d090e76eaa348f4a37d423d27d879a79c91");
            result.Add("gu-IN", "c3e1fd971a00d74fd26beabca102d1b3640a78d14b6f3de1ffdce685010a9b2b3b00be61b970278f009dae2a9f6732843d5bf5221b7c3fdb84f5ee4ea98a0e8c");
            result.Add("he", "2805194704b761e2cb05b8fdc94befb58b63ecff82cf099b3a9359a3359ef1dcb2f82d2d560414a40e146407493bb51401a8e394333dc5e4e2840dc0ea19230c");
            result.Add("hi-IN", "3b6ad1c5f54a6c4c3486025c9c6e99f875a8083d811afdd4d5d7c282a45c8eff56dac3f5363e13b9a9f6f21e38e88ef68b0ae4e581bda76c1c951d3a3eb6fff6");
            result.Add("hr", "078411158f4781b2d6a0249038e5362cb60ec2ea80d357bdba38ebb63f47ab1cdc48f479a01809d84d4e41065b853c731c709644091e05ad7615315501035af0");
            result.Add("hsb", "074174e85c6cfa04d9b9117ff5117efafbbc01f4ff0b7d838b8f5a05dbd9017a53dceb4327b309ca1a6b4421750f3b6850f6c9ed9c735f49e0739508446d516a");
            result.Add("hu", "1397d3f3d02e10c6d299d00d311fc6adac4140c0704f3f21dda0c722c0b9297a4e8b74edd6b12216f3716e45d60a20d452ace92dbdab7abe39d38bb01cbfc50e");
            result.Add("hy-AM", "ce23351b6517697f4d76c9e4118c466cb1d2204ed43c033d8824378032002a0fefd84c987301fe9a1828cf15d4197354ea231936bf2faa25deb38b566f8eeec0");
            result.Add("id", "4003a9fe50322dba1194ef5f689424cc4172040e25619c233c2573ab2c3ef32610aa6bfff4a48a11d82ecddcb141dbd9d730f839cc50245db9be46f11d278c9f");
            result.Add("is", "0421386ec6268a33d2e3e137d28a97324a49e39c542c99b117e9d9b8f2cdfa50ae86db840db15d1a8fc22e67c239368b5852fabeb7b50e032d606442eca668c5");
            result.Add("it", "60768d67b6a9a050b84f05d153a5409becdf4bfedaca19288d40533a6a6d71fee6bdb77e6f6d3a1de3d35baad7557cefecd4f5288a220a0ec886f7ff7c239dbf");
            result.Add("ja", "eb51cacf4098599e197280fa81d227b8d562a8243daee28ccc66f32cb27fdc95fbd89597243d0c4ad2ad6b8123657916582448632f6124e0e76d8550f7573c68");
            result.Add("ka", "ba06e0bdc5b7e85258d76e9fdffcf3f4b863d5927a4f95821c6624890e64b234f8c047df002dbe639bf71dc2535c11328a434663f1f903605bd527d64f025b2b");
            result.Add("kab", "f502fcf1c8d78e57202be193eca8e086b4f11fe6293b5eb33625d295b76468d0135218e626946c0d0002e571f3494c3e267bd563b6598dbf75a2e35d2c2ef1bb");
            result.Add("kk", "11e7f793936071d21c0523ac51d9c77a116b072cdfc3ed754e6dc4d55e99267f98d102bcc5be2df188216e5dbd9758c463cdd3c51c1ea6571ad611f8b4a2b42c");
            result.Add("km", "d8fc25789d1e8fe2523b8c7ee8487941f2c87de276b8aa0f878d33878c0c2cbec3f477b0ab9b512f1dea189d55fe69c7f63d482f831e696b876166fd36ffdeea");
            result.Add("kn", "f62ceb381d8b9cda3523aae594e1d27b01eeb0ceb86bf7ca30493fab19f11f1523c8ba008cca010026596059c893bdc27b91652f9356b894aa45ea1fc366c60e");
            result.Add("ko", "626e3ebafe1689536ad776c93353a09e80731b1814b7a59006fb715d1a4906fde1b0e08e6e52f9eed748d9ccbb161ab63c24f532f76ce88ddd106aa686d3de01");
            result.Add("lij", "609b91784b853dfec42c9fa3a072152aa423b88012e9153733da40e4f76c15a16742c43f2afd9d71a469eeff786f89a7aaaee7340dbe44673b0a4db43663c772");
            result.Add("lt", "d9a0b4337ad86cbe53574dd818f2f57b188db58a04722a660a2c090eaa360880cab82647a5c69aadfb9bd0b37b4f5ee9dde94cf52afacb6a819c83824b8a6637");
            result.Add("lv", "1b61cf5c8bf5f4553890c938e3713e7c6adb9bcbf70e6f86d44ad0e1ddb2512557c221453a4677f9dfb669f100ab9aae08d81fb676eddc39d12c7bb2edba849d");
            result.Add("mai", "1ec1dec5f3f47541b3e5273d814e05832a0d17ea8db04859ed461b155d67ce7c8919463f95d70ab8225f35fae74da3613ed042d656d978261e440ad9a2f0ddf0");
            result.Add("mk", "f7ad683048affd6233161dddfe7d0d387c86c8f1d0dcf6135a30bb35f5b356fc889a84d91ee9a2bdd31d1b4a38726d8574955ea6a15d0e21efb38ac3e781b067");
            result.Add("ml", "0497f29321a0710b3d2db460b946d5205db30a06ccc2bf568cf217efc0acad9938c1f555317643ad08a1b0b4a6d8fd374f73fc209b6a3375cc8faa4be664095d");
            result.Add("mr", "86cb2b74304bac0a64db12aac6c31880a5ee4d93cc49c345611442d8afc410d3df49ea7bd37835ae205e5bcae241b85273ed9543db3ba1236dccc5334ec1b8b0");
            result.Add("ms", "b9324b631fa0ece6e944a73670d6bdaa921f9d00a869da808e53f9eb34a3d8c419f152315ac8c41c7fd6983917b40cea7f5c478fbcb8e1ae795dbe64b6e43fe3");
            result.Add("my", "2e92e5db887da5b417ea3d041538bb6857eb7b49b6b5e12482a7b846ba4c0fad5f76b6a84af35db1d7f46868ea1cb81b5214df474f38348fe02ccf131bff1611");
            result.Add("nb-NO", "1250e4c4ed900d74474965084c0a03312b2c88be09999f33bfc6eed74208e50de999673fdac520353f57cd1fa63e391cdf9edad04fe7129d05fe46830a9bf4d7");
            result.Add("nl", "8939b2d42d5d86e0171a23dd6757ed6f686aa47aaed418f25306bdde6d3a86c5972e543f40d45332efa5c55892e0a1450afcbff0b29317e346dd042c6eb568a9");
            result.Add("nn-NO", "8c5552eac3e8ff0db8b033e709ae99d3c0ab920a48b07a332d027aa1209da6d92306b6e1c5dedabab1ec066ba356ca7d54528584f8b245354f8a60185e74345d");
            result.Add("or", "969c5f54d15564b83a677b3bac669277c83b67c8be1245053b718202666d3db105a9f3ad9ca37916d290f2e0f5b41494d47b27a6bcfeb7132c3f0c06bcf9280e");
            result.Add("pa-IN", "2e4ec5289abbf6b61de364b1e0e445ae8b76a9f61e307af2188f36b796526fa1704e832aa2a9adc6a682e0d13864840107163b8fea204c189416a0f6d568022e");
            result.Add("pl", "7589f29c7450334aaa63aa90b5be4cd536dc6328c10a0dd0aeeff9d910a23f3c662f707ce645ff3787078f1f907ae81c2a0c1e10a07edebfcad5ea3636c9168b");
            result.Add("pt-BR", "b5db08190aba5dcf3c9332c9fa0b953fb3c5766bde4befdbba13da1aa2ee8311200b6007ff48cf9b7e79db76bd13dc4b9a56c0559e7764339b02151a504cc907");
            result.Add("pt-PT", "0020146398729523a2d9e024619ed87edd071a4d6e3416d43f2ef12fb5c330091a99a6682285e47b1380952fc3e998983efe13d71419d453e15b07b8f93086cf");
            result.Add("rm", "42160618eb1b91de8ac1ccb4ccf368b5135acc1f6ed55e20ba126f4ecc7b74e5e2ae4b1704b3eeac19d0fdcda3283c508c667d06e4bf13daa38efb1f00048974");
            result.Add("ro", "5afa4ed35b2c3f904fe2954220a90f1288dbe11cf7a3082794755ca9fbcd70e9565fdab41c100e57d7932e9476d287e78e834de65690b0aa7bce874c330fcff6");
            result.Add("ru", "b11f975eec629e0570cdd5895fcab2d4bfa84afa22b8fbe213e6076639475f48ee5411061c87361064b62fca2f4601f528d534522c85cef9fcd09401bc317b94");
            result.Add("si", "60c6246a96ba55177234b8c40313ab8002cf61562c5352b0d5b6deecd66ee1275acdc198f620daece1fed1360ba330f811ff3de75daef62db5cc3b32a0c68b0d");
            result.Add("sk", "4f1c8ad59ec0622002bfcbf4740db6c58f7a9f9d8385023e019e61c73bcab5138e24f52a2162a6d860e96dde56259eca7ee2b5d426c33e0740e35bf291c5f039");
            result.Add("sl", "1e3729ed84d5de0e68d9714840b7ecb405dca8d6f4a4fccea012a814fac7aa8aad0561d2f610c7189a4ae8d5a9bdfa071856d2b577eaa1a17456489fcfbc1cc4");
            result.Add("son", "a5b6a4fd057be99a9b5a6132d17f6b6049e77bd5ae6a3d389cf5cecd63fb9da41481603ee146e347cf2481f013e9a2004cba74e66f671d72977e28f3abb19200");
            result.Add("sq", "231a934d16ef861364ac3f9198f9628bce171938748e1d86b6a50eac43341e69e2973916e31bd08bf9b84415e20d928937b07dd8dfc0293987e32a5a37ad85e1");
            result.Add("sr", "d36a381e63e48f75375402008b1f8c9cc13e5428d47b3065689d2648d7e7b596d313e67f17bc637bad5c24efdfccbc119faa3b29329231782d76fdf1b047abee");
            result.Add("sv-SE", "e396954b6fd4e2562329d32dc57d16dcbbc26cb6f49c28afd8d62b36dbd9fa1043e78e56dcbd80ddb252efd0669a40d92144264ffc4c76a9a59531afc7b2792f");
            result.Add("ta", "f9a0d53d5dea672879a0fe6b54be74395d50203157abd387db226e888cc940020fceb41394207d71ae3daa06f1618b0579c233168f7f2a0405af26b84bee44c6");
            result.Add("te", "c6749472b5df7c61bac02c455ef81d9c231bdf82a32921efe3eb5dc2d16eac677abee61d8999ea30a996326834f446cf143d3330af8c30d0fe99d0a7c02b6d16");
            result.Add("th", "12f4a4c774a24ec54a7b50966f9f0245de19f85e39e4b1ce5392184c6a7e1d0a3f0392d2cacc8aa08e853e979ad1c816db6085503d01a4e944af16b4371a5b9d");
            result.Add("tr", "2634c9b3cc8212c7194b2919d73806939fec6c8177ee0a768c1f3a6ba46a7eacfcf7bffb407c37ff5e6f02fa3a287fb837a88c853682e137a003ef4d53032319");
            result.Add("uk", "83502ecad34c4a67599d882af03b89d0d20a65deab979224859df10453dea0e7eb7b8274ff3ae10cee288744c1ff351bf09796363105d1255778f1190192befc");
            result.Add("ur", "575bdf0c613657f4e83aa702920697481a0a2ac7fe319ec1f8413e11178138bcafdf979112bfab7034ccfc3f28e7452ecf99da47108d67950014e914f5c7c7ae");
            result.Add("uz", "614c31c18db87e0d949e6a4017e6e3e3776206f10fe1aaa66a7dad5e2afa45b50e9f7286cfbcea67f0a7566120f8392918be6511d2dc19ae709646361c1fb223");
            result.Add("vi", "57021bd75139bb72db0d06ed68a3cda296d2e1d26522bc074e77ac10d66883d7dff02090fb32b680d72858610aba6de5762f707666289b5695e14eee1e7df2b4");
            result.Add("xh", "adda180b16b25a7c15a6ad912fdd7038dde27c267daada6db304f77a941548a4a9a53709732494b7e67e56191c3dd52edc858a290c50ca4b5dcb7ed99324e782");
            result.Add("zh-CN", "d009e48ec0bd5c2e9431c8fffa9e91066241c7b0aa48cb2180bae82d82cbbd2226a13598576d220890b06bd49711ab638f15e6d3e9d19a181a2bfadae063cc0c");
            result.Add("zh-TW", "8f7290d1ae87f7e30500e95ead40ee6c052483357944360a18b2785cb6905dd8151c724ae8bb8dbd8b37c032fcb21e760e65814186f5476b89ba52f75a404321");

            return result;
        }


        /// <summary>
        /// gets a dictionary with the known checksums for the installers (key: language, value: checksum)
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/56.0.1/SHA512SUMS
            var result = new Dictionary<string, string>();
            result.Add("ach", "32ba8229accbd4618f0f7a0def7090de9053315f25d6d362a8f1ad8d87fc8542023fd7b571df7d9725be077e670281c2231f6ed1ee9ed70a491eb9fedabd450d");
            result.Add("af", "0e6c3ea68ae5233aeded1e3a33e0f05f779825910f6e600a40d31a2968a8349b3b00f6a76cc5ebb7ad4418854711e06268cc7cbf98cf0a628b60d9c2bb8de1f8");
            result.Add("an", "52c78d962429daf25024294265fffb7fbbcb8c4441df9c46d4520e56e5a48deb07edf2bf2b216d74d0ff373d52a6acfca6991ec6478b1c2a38c9f6279594057c");
            result.Add("ar", "8d8e051fc61fb97544873f911944e3a8dfdf5a8216c2bd8777212532514cc7b295c86c3b616ccdeffd891db96ae511dd22d0a184c46304ea84db9ad9604c1499");
            result.Add("as", "b6d6495945dc01653f7210470bea76c34dbd0b35292ff4f9057548101480e5c4d44ffade535f6a72bb420bda45b5c08875296612e13657676288f5dd84848bea");
            result.Add("ast", "e1f8276bb7abdda66d01035bb2381639d7923bb33c01c742d53872a67546ac5941cc01a66ed6ac80d95db762b8986764543c6cdf5ce255f631b6a1cfb473be35");
            result.Add("az", "f562fb14e4666eafe9881853fed2722f8ae8d51e117d5660ce33527ca59a23ccbe4a70af8ffec87f508d5830f3b8dd85e9696064258ad0ff590f540e2b8805b3");
            result.Add("be", "359758f9d62a82c174b726620054d9504aacff14aa5339cd7baf1b8696cd721e8a7e7c008684c6f264c843e77a2568caf323cff7cd666e69c3b40b5034d9aa9a");
            result.Add("bg", "e6255516f508088290743f0dc3d8f52251279c4371bfa766f7fc3b6b427f1e0f7f78f74023e13141ded915951d6f532f986716470c575bd5801df5b647e1c24d");
            result.Add("bn-BD", "b086df0ab8d972b5dea310769f5f4364ab05a61987d7b018908f7e9d794a2cd7fd3eaa2ae55393caae0159a0bee7f14d3b1319ff1e46d6acfa4f69ea1a7330f8");
            result.Add("bn-IN", "86199b7bb71e438998a47b14984d3e337cb4e9ab4101a82b52682ea28372fae551b8883fd4a17562fc8df51ceee6bdb23e3c2f34ebdfbea8b28fe3301f4f6fc0");
            result.Add("br", "96b6bfca42ba279aa64d668957496ca9e8ae821215365dc314abc945d37fca4c40b4bee374643ddacc667b27b90b5ba70836329e68cb8994f804994ff7cda2d2");
            result.Add("bs", "f5b3644e9c70e98d1c477fe6dc10bf8f86c4284816da652ea097f8996a0f02a4bc2b9f0061254d05fcac87d6b722fa94ea7d44e9868d0498ce42ce8f08d3bb54");
            result.Add("ca", "b2c192bbcbb11f6e1b87b4493243c27a4914f1fe7f5449dec3b5b7faf7c7d7f920eccebb5a7547ecdf61a4651f8c840a715db6e91104a4c3eada7fa528fc09a8");
            result.Add("cak", "76bb3c0331d5d0826fe014233b31dfb22eb538267de412a6db908298dced39bae705561dfe8790a183b360bce3573f17c02c0e617c549638c9f9824ec5716818");
            result.Add("cs", "a3f6e3e9e6e649ea000ff73e2c9dce3b5998f88862f3bc2c30483290a0b72b34fe8c2cbc9e24c8338ba1312559bb70407b9b28717149a9355060d9e775bf3801");
            result.Add("cy", "931d40347366f1ac245d06ee658e64ca7dfb770726b0cb9a31e63a8e6719997ff6b72ba8ac15fe8fa7e57a64b91bf4ddd517ed791807372fdf45db3ceac35f5f");
            result.Add("da", "2710eedcc48e419eafc24d192f34bcb9ba3d30c54b91c948f462435ab24acbfdfcca2108a9b2fcfddb70c56c81ab4d34de9c0f6399df271262a92051b99e8484");
            result.Add("de", "c4c03df728e4e35d9db0a03342cc25b711694d916af8434fa3c514da3dc96d4ed8eabdeb312f3395708f2f2706c49ec66f12b8fdf68b46fcb719db8a191c5d79");
            result.Add("dsb", "beb6e11a15f50259054e0a6981494539fab727ef7da7dd8765b51a819f08c1fd58af3e8526fe0a8da0bcd69aab148eaa44968cf9116c3cc7a8e8800abf4a5c3c");
            result.Add("el", "22dadad178c77ddbfae2e498350835e6691dafb9d1be461d879e6b7c62731f7acf88abd03f475f1683fcb705da637aad50b6feba772f55aa914edd8d760b59f1");
            result.Add("en-GB", "6a84196861d353372887bc024a3038274d93485610db923fe10f82ccb9fb5b9483792e337052b69cb21d7eac728d51b81ebddd384e4826c7432feceaeef62928");
            result.Add("en-US", "b2b24370c7472cf175be5925afcf29e5cabd13d8cb24141e44fb67a3f9ef812c4e0a570eb60dcf8ba9a0132426069b0c9f30b7a687e7f6e78acd4a29390b780b");
            result.Add("en-ZA", "fa19fd500508d216b982969a195f7ccc0dd8e7dd4883e8a59d908b96466733f9d457dd0a0aaf6028e2862757b83417446f03abec67b36f040efaef93936f3459");
            result.Add("eo", "7f13977ca61853fd718d910314a7cee9447d75b1fe7af72f0e24ea51dbf4c96c8c535f48469da289538fabfcc02f2583c62ef6c7034217adef8d14cf0fc1505b");
            result.Add("es-AR", "6e5432b1669f7220ad5b3441977e6a68575fbd94c7aa7ecfe8311bf3a5e521ccead61dfc6b8731d76b4a442282997aba37f92f3a5b4679180acd686899321d1b");
            result.Add("es-CL", "8e6dd91e817e5a06716b5647aeb7a64b59f38213a57e4b324a4f0b8e9f533ab54e9c9cc98ec2e06ee7a50abe3c5efdbc8051b56c5b155845c4662fdfce56c5e5");
            result.Add("es-ES", "62c41433ff6202f1924e64251afd8b86c1c3a6ad6f6e1719b3170bd58248a07d7fa019f42994ae375ffeb6b157b7edce3034b0e28788f6e10a1af494a510366c");
            result.Add("es-MX", "a3fe329845370b4f2143764d39c7c697cfec4d1400b95ba1267e5a53725f2faf0c50f75c477b9d51807b708646635c79818ca70dc02004e56d211e86600d867b");
            result.Add("et", "f46af970f25c43029ea2347f75c832006202e8118a651375d29b2be31b8d55142a67c61fd4d871f76a3d14cdcd8c7101082485d0a961d342c1212d85c9fe019d");
            result.Add("eu", "a609800a101aca40119b9ec7adafae54650da85e66fe42ad166985c4007007e0c19af86f43e65b54e0a2cb3f4c513c643de321a53fd2922d2ca927a0d05b746b");
            result.Add("fa", "30cbe82cc7925621370eff7afb5a7495f27f22939a8ed236cf5fa49025a6aa0f0c360d40690e2973905645edfff3a056fe65c3b76ec78a3bf9f885241d1697e2");
            result.Add("ff", "4d6adc93a698abcba998e0c65f519c3391e96767114125b3724920851e3a30e63f7f297616b339494086f14d333c4c016a2c218eece20297013b13b25b8f7a6b");
            result.Add("fi", "43d1b75e08d313a627743ca35b2399f021f803d9d191042d5d81205e365b15fe8ece52aaaa57ac583a42f2b4a816075a0dabd3060f2eba7692f74521e32bd516");
            result.Add("fr", "0d7e53fa498c4c02b4e88b196afd398b1bec7c891f6962c0d45be7d2ab85988cc30b47aeff0bacd7ed58f28ec98a2e27348feafc222b74af471e7dc21cca38a6");
            result.Add("fy-NL", "2bfd6eb1bddc56b894a474abcd936ce1e37abdbe492e50e5a62748a7d5f88173d4082bca1aff8020ce14bdeee04255f5c66a8b1f56329e2d4dc4e373f6553a5e");
            result.Add("ga-IE", "6ab1da3e53a40a4d23928cc3c34f4e05d30d3ece8473e04b033d1258037d8e0fc955876d085559215aad2bc3dab9bee7668da2296c78a128198e14ed59b9a45f");
            result.Add("gd", "c124f948367c7f389fee59f58cce680f7ea00d1d2b524a2a6235edaaff221f04b675c829e0a6a8be8a048af213bf5ebab7c4774b139703a28fd99afc892384f5");
            result.Add("gl", "413ecdc72197732ca12ffe3c41e3b7786578f0519e991b004f01367518bfcd3011d179c3b6f87204710a87ca1c71264f6cf112bcbe024cc839338e657af731c1");
            result.Add("gn", "039847f9212ed28b0c046bbed25d4bc15eae57dcbbb5ebf88ce10da070478c7de25bbc2315beb7f3be6dc1fc174fd26025bde8f5eb602f5db0d68fa64b2c3a59");
            result.Add("gu-IN", "bbd53e434ef58de4102ee3c4202e073b398e66eb38fadf70ca18f78acb4f7115f07c71931ffe430259e81547de3abe0c1a1ea66ccb55ffb467801d7017323758");
            result.Add("he", "4e172ed7a5991598c455bbc337723be1e5cce068d1fc7a0faa82ef42600624b3ee25836a2bf209e078a40dac45c4be8857af055267e9bbbeb97f7707f2ed7d24");
            result.Add("hi-IN", "e491525d80672e2f16838bf5832a7654eff606f2a4e250cc57d78e1ba1b350d572995fc8a6f1f11f934158baa034cdd8b597619715527d1ebe50a72e3ac84c24");
            result.Add("hr", "7ab650c4d86e89061a191cf6c2a07f5c8132d43c51f10add35b398385f63f974b31c0ad585c474d2f7dc96886176b89f8cab1f78fa1d18c9e3f088566d2147ed");
            result.Add("hsb", "1e2c0035ae2df6aa4cd7c8e86ea8303389f4b5eff8abc83dd131168a9077dbfd5c9e5db2671497e8212f440f0747c04fcdfd73eaa2d7278da591d5926db583af");
            result.Add("hu", "78528330dd71493e2b47f5d6299e1f5c88dc22815b4a6b237bc5db4df1c040cdfd2e8789f1391d0892a673c396be00b2a745a2066cab02428c4cd2d10b1ed157");
            result.Add("hy-AM", "e18a146ad2e4d6b8b5daebf825339b724c9a259f44ae37616f2bf3fcca54b86df390891e580d672712cdfa1510a3959d8aac15cb9cda0c5ec147db1dc30fd515");
            result.Add("id", "5e4c98cb7355acb8a77049ae61ffa1cf5e0e0a3a28649a971f7c4de925a2ed0349542dfb593c7e136037ffb4ef8649b24095be7382dd20f88fbb141b557df142");
            result.Add("is", "a6c13580a28ac549015b3fe1b57c68202cba86c1b5f15613d6a9225e0535731071f6e593efa221e67662e65faf0cafa39b08ff12d23c3732a4a916187d63e19f");
            result.Add("it", "666d6bab7af6dfe5fb6d9d2705c2e7418287306ca9778ed078043b807e2423560bbaf655bbe0182c75182e7a9a949beb2bc803459ed3caee4adcda8eff8feb91");
            result.Add("ja", "cded5235a9cccb83b35a6cb6636f365083480da32678259fbead0fbbc28fd5858b319f2db3e37065e223124b42c880401c0454d2624028083b372a9f5e23ff71");
            result.Add("ka", "bc2d6aa122040f15156d77b820b3c4ef2e64799932a02950ef61e80dd2077112f230c9636be0b0db5d21930927cc7e95140401cb5684a1d742e5a6fca552effc");
            result.Add("kab", "a843250fb090c68418bd911a32a94b6fc3b5dd083613464840632176f2304177f1e1fba24090bd23d492422c5a4482f62e1d47001c537add9d4364e6c2f11747");
            result.Add("kk", "216f49bedf31e4095010aaea721695885dd2169ed608b0a55f55ebb2f638de49dae6d9066b9d0041bd17e4a3f7114c3df0ce517fa7b1f65925738e1daac2c379");
            result.Add("km", "27328ba6c72a092c3ffed24612ceabc40776ce18517e99fe8cd7dd3a6f447813f1197318dc1288e07665104c633825d097bf4f18c6c074cea4cb5d777e388eda");
            result.Add("kn", "68538a68881bc7e8d13f6135762c00e8858379e892c859ed99f0a61e757bb3a49c3cb7012fdd46463050a01f2572a716627f0f62ea98eacb2628fdd9a0d28a23");
            result.Add("ko", "5b312892dc74825fbec15714591f2b1b6d00e9574f24f7601d02b76893ea2607ad8c2350ddb20478c91b379b37aa05759880dd36cb8584fd6553265d19341fd9");
            result.Add("lij", "6d197d25c8a4d3c75363e541c58c35cd2465bd656a07e750876573316f12314171bba20472a7630ce11ef208477327df7d04b368d2845da16274c6aadb3bce42");
            result.Add("lt", "7b0b9d67020b1d82d92d4a9bb559a38ca65e0e3159aa0c03f4fbc7c97d5d857e50b78ad353fb57a23bb48facbcaeecec27074730561586336814fbfb936c4c03");
            result.Add("lv", "eed5cfb6ca8f54fcc14b803f7c01dfdac68bd31a1bd324cd3db904ad1a3265821c1d1b0aab60a7c7b6a3d5497c6e01dd2d892eaf4a134d9620bfc0fe04f53f4b");
            result.Add("mai", "f4a457a8e5aeb55ff0f4b399946c3075278a677e1b6826365a3636eb971b081b910c9e9a3509ff55313b3fe5128ca2dede70b25fae89860ea967d32a62877a59");
            result.Add("mk", "57cbddaf3e0c6ccb224c800363105cb6e92f153ffa3ec244ed2634d36994b58fa18a79d3000f9eab541213e7b77c536932d796382c86f1d7058c9f619f5e55af");
            result.Add("ml", "8d27b193866d79917072b1a926d59c2ebcffdaa92a9d4dec538758c2ef2da6a23993cab9ad0a54eb2b019b807d3835f152101512d156b7ac8a47d83fb1b0cee3");
            result.Add("mr", "4c654408da1635ea22d5b1030a024b93be0ea265cc075abf29899abd95d1734858b4ce63915b7d4251d4884d6bbc5236378f6eabef155510ead37807057b7127");
            result.Add("ms", "325fb60971ba15be55182adfc65b6914140885cba88a93ac3349bcbd4641202cd78a85d3bc8a923dc128558381ab2e5e338a75f2ef23722cbc6141d4a9448b78");
            result.Add("my", "1b6653b2170b6f4a629dc3a5405af4ac5ac9285e7422286568b88568e76cbdd9f372460ae6409180d4f9842e642e313780ec1294f8c5f473dd62a73022badbc8");
            result.Add("nb-NO", "7ca02122e200143d994ad04e0126b954df86ed3d6b9e026049129848ccf5c327978a0ab95bf5850d9b565814be6dbba2de7c836c7f5feee0ea0f4220f3ee95cb");
            result.Add("nl", "b0ad3d8b44fe32810944a5274fb14bb4301c8cc58e666f437f92f70ff50d80c81fda751984c982bc68e343c805c6e3c60f90b55122a6f376a1ab341775b99484");
            result.Add("nn-NO", "bec9f28706d99ebb80c55b90b693f24fb3b1062f421b9665519babcd9256817a57b93104565ccef9d7df65bc572c8212481066a489390502d08bf582a4e0c6a0");
            result.Add("or", "b28aa8fe46afcdeb38605523546597b2bd330e1839fbec8aa422f5d5505797c63a7f32bae393d4a8fcc8fb72711dcbb62c937f50a75d87690105c5952064cf04");
            result.Add("pa-IN", "150ac5ec660a99b258bef0cb9e544d5ad2ef0dbd21b5ea306aaaa45cf82d63d02430fce235ff1bff3b58da9913cd6ad1a44b3b72003b78eae5575b017a5a0a0f");
            result.Add("pl", "8193c3cfd42296520784dff5f30067cd89c3990dc233ec1c9e0f4e94f2285de2058e11bf68ec9202dc48ccb4e7cb36bfe0b52bebde25edf19a5d01ac3a7cb4e2");
            result.Add("pt-BR", "2b25de7137519de9dbbd57e14e879166316351859902f837ed68dcb808e72c43bca283692db10fcbd7abf55bb5a06637a5a46ba22989c6b62f1b4c982272295d");
            result.Add("pt-PT", "953a828c0d87d2c86f34d4cb8c078793df6fe358a79fa90e5681512c5feaf86d9d2865f1fe1ff411def5bcb68f308a29288994fafed40c8e69ce63e480534e50");
            result.Add("rm", "394f59e05bbe156465356175182cc831d97e5bd16f997f07a0375daf4cfc2eb4bf6bf17a6f1ab6d5fa2a8e1c24124223ae8c495a1b609d841be35480398000d4");
            result.Add("ro", "ba1a0c35248f062139a2c998455b454405e40221401fdb69e4122dec70c4c27a64d95ef837c088495a12cee9f00ba09692ff7b4b4a997b2bd5d6fec2941b3bb2");
            result.Add("ru", "e9744fa420f5151b5004ad64f214d6a53a50c473a9cd30096c006a89a63eda63939f168ae6ec7bedd3f135d192c6ba07d42580229bd44c66bcf55a6b3a1d1d52");
            result.Add("si", "edb1ad43d72272b1f35d109732c251da6151ceec3d5963434b8f4fa787e55c07d1d109cfb292c766564dd8ff96a52773b32da9847edf37e36531a817e1214675");
            result.Add("sk", "bb6a9a5bf52a55d1e1190f805f43d75470ebf44109654621aea6b4c60662ca67d7a7f5fbddc3dd06d5107de4ca776b220c0c484f738f3bf95de145784710589d");
            result.Add("sl", "eb61b7d52e313fa9fef5cb7d3205c3379aed532a7f7dd07bdc2cc6751e1d4507a678a490331d021f58012be71f9b239f677faa3432ccae3c932bede8a7705d46");
            result.Add("son", "5766c463c3491fc0e357a2882ee81b3fd26e9fa0d27dcb63478cf95d3aa413de31eaa42a3c54c3c8444e11fbbd42931ac62fa2c5a2e7b77cffb54bed88e2657a");
            result.Add("sq", "16c12918f870ae94f8815619dd48d68edb9f2048b6c893e70f6c32931e62ff664b02ccc6b92132446a96570f16164102494108371cbc961e14efbb2bc2f95773");
            result.Add("sr", "8dc6cd4bcb46d69b04a9b777bc113cf41b9b61bb3a8ded936ed3300d5e73f1f7d100e1af3fade5aafabc63a24d1539a641a86418c0fc9446440a7fef85cb0a7e");
            result.Add("sv-SE", "8c11d2a027815debfb465481846d45b47874584de4feea4ec8e2e6359456e6caa96fe4847d7a096cf01b6f8b50f7fc3b58cf9b4c5ffdd7eedef879a9e0b4e85e");
            result.Add("ta", "5c62440a180bd428473621cdd5688f9e23637cada560601fddc633d84cbd27aaa85a97e28e00abee8b6dd9d9d2355224d9eebfd5dcc33da1f100c73d68d93fc6");
            result.Add("te", "bdfda35f304ef76628f0e0ac98837011855330e9302c8de2a17f02d67bdbc1c513b340a8f13b3b132f46965f374e4c0c13fc9d780b419220deab5737630e0706");
            result.Add("th", "cba80a4cd5795701e7c89ca7e06a78fa1ccfe6ffa910385189e65767778fbb2ee7d69b5d7bae2b60b4b8760bd0580ff5d4dc28e7164a1cf435941cb09587000a");
            result.Add("tr", "929db99abcc1a7b532ce3a4f7e29eaa4e29423f9b7d3e2956f88a38d8af07676c9d2972564b7854277f19686c431f3d8b1b106314a1afe8b36c20789eab40139");
            result.Add("uk", "db3e72f94209c386b3bba0c68740e43967b21ec78d2f2ecd5c4c334c5cea953a58b01aa235893c66a743984be3e1ffaa01e5917f6435134febddfd2a3bd7974a");
            result.Add("ur", "546022b0c2de62e0b33b8b67e52861d09ddfd7ac99add25f1df3a0f2969247f7eb35764b29c0ab775e8d59abcb2076b33db50e3e308ca1500b1c72e449664f7c");
            result.Add("uz", "4835c87f41b74d45c0825fab8a21748f9669ab1b42a97ace8ee6599cddc93e080019cd0338d273c7f85ad9ef72d15ee7f7067d301df776a5c652869a7f7256f6");
            result.Add("vi", "032a7212c4b6149273a9ae6eb06394912dc9d5c1bb3fc9652c6a306b2480db0d79dccebf8bef8e2847a155d511e38a4dd21506cbfbf826c1477936784a3ac553");
            result.Add("xh", "24a1cad8ad8a253cf805fff2efdc7182f00ed1d1d91f660a4433e6d733b356f9343c3e28c44a435fb993b74667c8ac7075dae8aadf33b03a9a1962a0a04f5646");
            result.Add("zh-CN", "ea9401ec5f7c9974e86b6c8348fe7db53eb17ecfdd1cacc1e9bed129c1500db58fe6c68e6343331fd535a18c941447eb2f0daf0328d1ab458a5ff4179dd99df4");
            result.Add("zh-TW", "418b910853dee73b7d0539c0e2759262c3f58e43c68b200e152219372bf8309fa1a6d7d8d8c799b81dd06bce80cc11fef49805d8005b0e75a0fc913eaab46933");

            return result;
        }


        /// <summary>
        /// gets an enumerable collection of valid language codes
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            var d = knownChecksums32Bit();
            return d.Keys;
        }


        /// <summary>
        /// gets the currently known information about the software
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            const string knownVersion = "56.0.1";
            return new AvailableSoftware("Mozilla Firefox (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox [0-9]{2}\\.[0-9](\\.[0-9])? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox [0-9]{2}\\.[0-9](\\.[0-9])? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                //32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    null,
                    "-ms -ma",
                    "C:\\Program Files\\Mozilla Firefox",
                    "C:\\Program Files (x86)\\Mozilla Firefox"),
                //64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    null,
                    "-ms -ma",
                    "C:\\Program Files\\Mozilla Firefox",
                    "C:\\Program Files (x86)\\Mozilla Firefox")
                    );
        }


        /// <summary>
        /// list of IDs to identify the software
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "firefox", "firefox-" + languageCode.ToLower() };
        }


        /// <summary>
        /// tries to find the newest version number of Firefox
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=firefox-latest&os=win&lang=" + languageCode;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Head;
            request.AllowAutoRedirect = false;
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers[HttpResponseHeader.Location];
                request = null;
                response = null;
                Regex reVersion = new Regex("[0-9]{2}\\.[0-9](\\.[0-9])?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;

                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox version: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// tries to get the checksums of the newer version
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32 bit an 64 bit (in that order), if successfull.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/firefox/releases/51.0.1/SHA512SUMS
             * Common lines look like
             * "02324d3a...9e53  win64/en-GB/Firefox Setup 51.0.1.exe"
             */

            string url = "https://ftp.mozilla.org/pub/firefox/releases/" + newerVersion + "/SHA512SUMS";
            string sha512SumsContent = null;
            using (var client = new WebClient())
            {
                try
                {
                    sha512SumsContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer version of Firefox: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } //using
            //look for line with the correct language code and version for 32 bit
            Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            //look for line with the correct language code and version for 64 bit
            Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return new string[] { matchChecksum32Bit.Value.Substring(0, 128), matchChecksum64Bit.Value.Substring(0, 128) };
        }


        /// <summary>
        /// whether or not the method searchForNewer() is implemented
        /// </summary>
        /// <returns>Returns true, if searchForNewer() is implemented for that
        /// class. Returns false, if not. Calling searchForNewer() may throw an
        /// exception in the later case.</returns>
        public override bool implementsSearchForNewer()
        {
            return true;
        }


        /// <summary>
        /// looks for newer versions of the software than the currently known version
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the information
        /// that was retrieved from the net.</returns>
        public override AvailableSoftware searchForNewer()
        {
            logger.Debug("Searcing for newer version of Firefox...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            //If versions match, we can return the current information.
            var currentInfo = knownInfo();
            if (newerVersion == currentInfo.newestVersion)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if ((null == newerChecksums) || (newerChecksums.Length != 2)
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
                // failure occurred
                return null;
            //replace all stuff
            string oldVersion = currentInfo.newestVersion;
            currentInfo.newestVersion = newerVersion;
            currentInfo.install32Bit.downloadUrl = currentInfo.install32Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install32Bit.checksum = newerChecksums[0];
            currentInfo.install64Bit.downloadUrl = currentInfo.install64Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install64Bit.checksum = newerChecksums[1];
            return currentInfo;
        }


        /// <summary>
        /// lists names of processes that might block an update, e.g. because
        /// the application cannot be update while it is running
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a list of process names that block the upgrade.</returns>
        public override List<string> blockerProcesses(DetectedSoftware detected)
        {
            return new List<string>();
        }


        /// <summary>
        /// language code for the Firefox ESR version
        /// </summary>
        private string languageCode;


        /// <summary>
        /// checksum for the 32 bit installer
        /// </summary>
        private string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private string checksum64Bit;
    } //class
} //namespace
