﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020  Dirk Stolle

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
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using updater.data;
using updater.versions;

namespace updater.software
{
    /// <summary>
    /// Firefox Developer Edition (i.e. aurora channel)
    /// </summary>
    public class FirefoxAurora : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for FirefoxAurora class
        /// </summary>
        private static NLog.Logger logger = NLog.LogManager.GetLogger(typeof(FirefoxAurora).FullName);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "74.0b9";

        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox Developer Edition software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public FirefoxAurora(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException("langCode", "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var validCodes = validLanguageCodes();
            if (!validCodes.Contains<string>(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
            }
            //Do not set checksum explicitly, because aurora releases change too often.
            // Instead we try to get them on demand, when needed.
            checksum32Bit = knownChecksums32Bit()[langCode];
            checksum64Bit = knownChecksums64Bit()[langCode];
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/74.0b9/SHA512SUMS
            var result = new Dictionary<string, string>();

            result.Add("ach", "f768634710eb2974c24d4f72912d64a78008b13b5feff6dea92651edf8b8143b4050a3fce288cfd16094bfe51aaca9bba1d09cae5907605f8d8b6e2c937803bc");
            result.Add("af", "0aa173f11768379f79b642088add77dc6eb9fb947963a825cec504fed8fa7245bb63f08887e2c5083ced3fc423301552dbdc8c6456ecf0ee098b46f4d6da3d4e");
            result.Add("an", "8e5c531e766d63b577281ca4a00f5664f75abc254f1c197bf05b792cc1dc0b8b60064bcf3cde74670fe366f5fff6b3b3183be7beb85a07885f349f96b2364070");
            result.Add("ar", "5918136473efee6a4742b43e7fd29f9d064ca45ce82d2263e9f2bc1507b4d2a4f5f3944ecfea57e755d185808503c0a1402483fd55858ef5304f86508662bf28");
            result.Add("ast", "2ff9a75263b74b5ceab940b45baa68ece9617c209a95c2dfc8d7b6ab8f030105b2a403dc87e0fbacf281f66dc9a4c5c1f7c3eacc242991b91efa0ba64da1c67a");
            result.Add("az", "7fec9ea9ca2ca6f98bebe28aa96c356dd0e1fe61531407fba7550c85c95ad891fb040a6e463f35a1cc0c40ff3b6c889502594ba66cc3550ec157b03551e91a36");
            result.Add("be", "6d71decb222f75a558389ee6434facec9249c2e4955cb3ee712b724ad450bf7f29550561cfaf798e11a7434f2f9ae6c43bde009b3edfd51a310cf89a55b3e474");
            result.Add("bg", "d83d3412ef9538d292e6e4a8afd7701a364e30a8f75be28f24f93188f52d7b66c1628a2e1ef70fac3d2f1092580264dff59895dc73fa277fa19c0507b4402305");
            result.Add("bn", "67385c5607604f4ae2decc0601a13fa99d79072707558c0b7eb5ce42ed86d3cb6ff8e1f87d8f002cc6528de5ee746fdab1c32b1e0c62f66c3641485be43a6066");
            result.Add("br", "7613d151736f7e73c60ca8381c281a385c9bf07ae8940138ccc6a9b3156ff58e61a3bc4d56f25b067de5a69369a2621609d2039dfed1b5b604008ebc1e13ec5c");
            result.Add("bs", "aa0ac4931db1e557e0cf9d819743557b7c6d068161991c869e1b4eaf22b9ca582a43a6565e1cbdfe9da6241a5db0fe395dbce7e09d3715ce845ccfd5aac663de");
            result.Add("ca", "cdb0d239c96e83ac99e645a82bbc652f01c685c0ea8d67618ee5140481b236534e85a4eb91b78bdf9694a2afed297d6c16fffb31d8404a4b00b0f7d0e16e4c2e");
            result.Add("cak", "55bb1d8fc6d9d3f2363a70e364eafdf0e004c52c802377a91303af984bb83d24728e77072afe7b01b502c30a08d66a3767a269602bcead12643ace377079d856");
            result.Add("cs", "bf488da062dd7941daf8d4295a596b407cead967101bdf8fe6afdad7f10c5c21e02b2764c5ddfc28f286bc4149e3a2e8df0b0eedf36d90778b9caa4981354e87");
            result.Add("cy", "ddc7359cc70a11cf54491cb561634d05a826c56d0224f9f587f2665026245eec312c54a34dab43db01dd721425dbc3072ed690bee49a77536e46630901e9a367");
            result.Add("da", "a353ab770aacb9839004d303f79fbb36269f551efdf24b1f4e7f59da079c5e013ac2ad05e1a6adbbcc999711dcb8c523b6e4d0c4429eee40844ec8dac0d07294");
            result.Add("de", "f837ab71120a89a6b9a2e72c08287768f3c6c1462f3dd35e6a26cfd6f8ef97bf6378acb7c0e570496d3d6647a5a6104c213124c210065d0deb25af70ef48e42d");
            result.Add("dsb", "6d436f5c1d7dab1d408dc5bc411da44216e581c4040a9fbb83daf5eb14abec9ab0026b7f63281e9bbf17895cafc6ce66ad8927bb076b49705fa6de4d2f5855f4");
            result.Add("el", "00e2eb0f1595459d2b68250461d8f79f082b90082e68521e52c9b5ae44eabf7806549d2de9be972ee7951443d9c67b33538a3b4efde25fa719ccd3bc95fe9327");
            result.Add("en-CA", "d948b2ea1879d0d1fe7a70f26e6d5515ead462a7cd33cfed3141486ba1f2b7563db5fe824e7ce4346ac8a93482bbe9e77446dbc0cbfde6a350332723430f9188");
            result.Add("en-GB", "d3956ddd0b7ef60a47ec557524efd29efce7fd809bf9e46676063dedf4bb663973b58bce822602ce9d7d2654aa613683c38e11d10c41b53a59db59ddf509d845");
            result.Add("en-US", "52b01708ed7b3abc5668ea8c08547fe133feb05cc5a1250d6ae7b779e6c7e958ef15adae4c51a25e75e3d5717562fb63cb11f43f66b6d3e7b4485a2d6809f89b");
            result.Add("eo", "2aacbfb55635a8bb6b61ccb78bd191c1c6f95932568ea765db231900976b41beee5403fe99b55192e684c6f702a7b408a721938e5617d730dee7dcdd61e72c30");
            result.Add("es-AR", "55a5dfa0abe5bf6d8f784bebee29b166b61e13ccc987ccadc0a1420cd40e65a41d376dc29637bc98ed5faec089f85b3efcb4586330c300f1368a5662bb68626b");
            result.Add("es-CL", "2f88c0bd7406247e0024983dd380acfd9e2f5eb03b2d0f588c6f32316951143b577d441bda12b5e0b1f35f73c70d2722ad2103a296389af48c63757209088852");
            result.Add("es-ES", "1d07b6dc9c6d7c5c0a073181efac8825ca5c69d5a294a3acd03be845e61ad4d85fe1a4ddfcba5ee319d85d4556e2f7a6955881c457381f10bf5594c7983c32b2");
            result.Add("es-MX", "5f3c7ce370293cd1e4de6c09a27a5a02de393226014b0c69024ab49a8f5e3a054127122c9116b577ac98a1c7168d9f68f79f4ae3c520d77ec6c6c377696cf17a");
            result.Add("et", "64f137615167408beaa0535cce405c9e27250180132c3c1b85062610a8e1c8c3e51e8434941de782bf48e36ec0f3c1d51c5303adaa2f7916ebebb37dc9ccdeb6");
            result.Add("eu", "606ac55745eebb8a4e6e675f0f2ad39973a25c7736556e648b2125a5c73c3a2517d78977f6f058f477e176d9e7b762165a2f820c8c144743835f176b9a4767db");
            result.Add("fa", "20e9cfd71c1053d036d381261bbb680388e382e9cc2d0fe2023e633aafc446cf92737ff8f56d97ea7f4edfcc132936388793915b55538ea6d53954715860846b");
            result.Add("ff", "d656ecfd92745b31bcb58f694e764987b9a6c825c29097d4a10c147941d7c9cb542d23eff0f36b2ba949f5c21789e2f16739a701470ec9a1ba2ac2252b9b951a");
            result.Add("fi", "462ffb73cbba3c3b17d7fd8e6aa5a17f8c2488c8e2593f81f468e2c7189652fb8c36cd8257ce533bf58424bc9b225567a08dd258f17bc5862e72fb1815ac85ae");
            result.Add("fr", "092b061169bf750d23476f552a695ab5edbb5b0313afd4762017fe33ef640e4e449030186c28b8144be6a4b266043324bb3668b060c659225b38cf46838f1b6f");
            result.Add("fy-NL", "16eeb8ec5a1645ae4d1c1b8914840603e7771df7d74c6a697568ef7dba3fdd86481486c970dc6efe99a9a73edf6e2f6a6dd2687f424fe8b127caa249ab669928");
            result.Add("ga-IE", "e588db2f8da335eebd719b735564a5f23cde2437ea2f57f4b971996508b3c04ba1e92ff034c3cc83b02710e784a93b5a94dfdf0b162a01fd49ca195e6533d158");
            result.Add("gd", "ac3f33eea3034538f937c94f8bec98f494746a231cfa693c10c026229ceb2f218d9a220ce456a6d9fc3a5b43d09e347a1508de24a4884b2ce20ce0d9c62daaf3");
            result.Add("gl", "9131e6c6eaf9db1fbbe32983b2d8350939bef46cb54c1e22b376725b1e273f571f24e9cdcd6b7ae9be4a7f020db3ac981a34e0823580190ff421deb7f5fcca91");
            result.Add("gn", "fba9be97dad6521f2577bb88658fb1688df117bbcc017512e2304e454caecb019575e596ab03dea891852c9544f2ea7603459a198b697e916f9ed758e5b051e1");
            result.Add("gu-IN", "88a54ae6fcab288ec4ca78e91c5369bd0e4c3160539c2ea4b5cace504d5d5825335d420b5b290298ad72071e2a595314e2372d78bcc4ce614539d1910aac6c28");
            result.Add("he", "7a1c62731e860c571f4e83d592c7c5a8a22474636664e216b77a20307c109e5c9d06a563ef850e554e71cd91c96b0fe24464d008faf228d52634e6f6f633eec8");
            result.Add("hi-IN", "0285e3aadbfc5ac7843c14e1706a8fb75d227e89658e03ae66e49587e68c2f9de59b986ea5d893ce9d21d58f7cabc3440cecf8d763602f5246043973cc418c3e");
            result.Add("hr", "0f4caa5311984c5dcec14ba234e15cc295a2cb4ecac7823a1c8d86ccc5837ec5920321fd9829fbaba54ded9a43df3cd8ef4cb2c6d0dc8d64ea8ed3bce99bd479");
            result.Add("hsb", "8ca21aead6bb9ac35cd1c67a35bc3d546fadc88a0418355915580724cf05c953e2e4646d1b9a1f70e7c96a83a1d97183f15310a8b1e36fa4cdf2aff2ec6a50d0");
            result.Add("hu", "10685dce20c1e87026b1321208ff531fe87bb805b5eb295d91393f4d7a44e9757069e5d4269fe16dc8d8723d7b969c73bc1b069a046d27a64986cde346e95308");
            result.Add("hy-AM", "c6d566220a13aa726c789feda4c79171a683dc8a57ef161854ce19556bc6cde87f3df7c74b3b12e4be74b278b039db74d51564f2392294415e667f2c62002f59");
            result.Add("ia", "760029ffc6e27e30d4f5d3e386599bb407668cae5eb66305b9e9153cf662e5b42d12f6fec8cbb12c98e3440a61f43a693cd80a3353d21583fbcfefbff15e2132");
            result.Add("id", "1259f9a288fe2962f5e359e040373c548fcc8406c542015c710a2003c53fd795709c94590722f0021930c83457b4e788bba3dc1a4f50a52187fb8996c6df02be");
            result.Add("is", "1054a38c64e14700b04bee9cf81fc91875fe236743c78bc036c09b9b602c144ebc577276cc42d3ccb5cbdbc73b1788b1f088112791e602438ff2ee9728a67054");
            result.Add("it", "3c02af8e14078d608964a1dba90e0c8188fbbd338c60214817d41ac9ece9b0a4469149fa3b88d7c139e943f7004b3781b704c78d5d9f0b2ee3cdf645e6e49cce");
            result.Add("ja", "ce3fc5c0586fbeb9f0838d3d8092cfb7b329966af8ebf85ee3c3f4e53ca4f10ccf5dae5e300ae0974142acb04ff07543b4cd89451e963d9d32bc94976ce07b4f");
            result.Add("ka", "834d4ccf782ff5d63f1bacdfca6a261ca74a4728584f2f6243647494a73cda40bbde3a53d633cf6956d57fc7cb1d50bb417675c026785c8383813e88fbfe6b61");
            result.Add("kab", "50e1c7fe51bdbda3884c36d031b85646bb90f96fa257d5dbf9bf3a85d805ce3cbc849bd1cd75375715cabafa7c77dd2da17efd5bb171540959f55593c2a30710");
            result.Add("kk", "fd936e5a3d1b11c3edb79814ed03b913ccedb77d1d9d2aa225c58679ae53c484bfe6a0bbc436831d355993601d485f829388b6ca09949368b30db983201a60a1");
            result.Add("km", "644f75023d81e75be6acd17b2a89db45d2a8e685585f5961f7aff12e8bf8cb6a523671ec4d1f85702d44ea1598366dd54b4314bc4c9b9a0ce658f64f25f18d85");
            result.Add("kn", "e0198d41016b02ff997aae97f93b1c2a7829a48d91a7ef60be89dd43be365e781283a20e6be40d10b4649bb13375671b12a898a8a3846e2676e53d8809c4f32d");
            result.Add("ko", "d4396baa97214a8889440862c222b041614efe06ae6af54292280904d2700b1d62c54cb4b492dc96e0b17972b59f29c2c2202862c2f6a49501ef89a5d2ab9251");
            result.Add("lij", "3381938e4561974308d1b26bfb6ae7331ef85551e285012c1b6d14a98173c16ce073f1a9d60eb544d566c00f9f553c6fe36ce3696da6ccd442d8caffd74c930f");
            result.Add("lt", "29bed05b5a09dd82c3eed90c03269b5169ba33b6eef62cfb07657f0f919d17fa47477962b3e520a525aae5393ab3c242b8f31b7b9490e4008777a3af4069fdff");
            result.Add("lv", "7bcbcbe56fecb177b89f32dba784597b4df0e4d1b9ddc99fff3c5a3ab6834732c0044421820d827efd9fb996fa15ced400c2d23d03d1760df3fdb7c3c8615f61");
            result.Add("mk", "4efb67949f15ddc935044c47df1ed1f9a4344c5e1db39333620cb296d1644227658075bf1d553d7e02091a89e3c0d5c0036b23e2eb6d2d2f39fcefae91e88b58");
            result.Add("mr", "905a613441ca3219492b16d32237892573a10f6e62d6dd22e44d950ffc53ebd8391a576e4039270e4820ca622c21c649c93342ea70774098459a7945a30e3b13");
            result.Add("ms", "37f672e959e9a803840c3044ab4d37fbd46b3c1f096711c7b9809a8034d39f4ba3311568849ed51e0f908f8560294e160cdef0129d7d5f7b1a35105b967995dc");
            result.Add("my", "3191afac692788d4d7c619259c002ea3ec51a2f401cfe114094049df8408a99155d1213089a8becf8d74ba3861eb66e79992ad4d03167f7e34d7759d1ec9198e");
            result.Add("nb-NO", "0f4c4640c0553a9661d3ad3288916a23f0bb54d07a1cadac5b6252c0b9fb27f9aaa108e372a38ebb9344209cae3311b355f105da55e0bedbc1b6b8a644a087fd");
            result.Add("ne-NP", "1dde4ba90b4647fc24b437cbbf5c4228429f015fc6a2827357aed444e90a1d45e9233be7ebdbd7b7035c9b97d94f1102f5e2d1e6e2d5e6d1ae98835a570ba349");
            result.Add("nl", "70572f07e41ee65b41f91ed066acc5f7972be74b2bc077cd9960af7cd3fdfdd17dafd0dc8d287183c08228e603b4d749aa490a819a154783a786b7f9fe31324c");
            result.Add("nn-NO", "2daea1a0dfe3d2f8e24bcb5a8bd506fa4df7d6ffbb866b0d946554568c86d6b1b532b51675a00a7d90cf55e7fd821c86c272b88ad6cfb2ef40d73c613761813a");
            result.Add("oc", "c8482804e754eb569230206a6550cc152a99b5f3c40377f6d97b69544ae82d1de76ec58119739327086ff4742ba307bb942bfcae2507df674c0d6da7b2ac1f7a");
            result.Add("pa-IN", "b5bf2a619abed783cfae0f7fb6bf87793b20c8a8bb3269a383f5ea9703d18f8de73e605d91689c1864603e8336aac31c4aa187b306a2f2ed0dc7b2a8db153486");
            result.Add("pl", "f83e52af4a9ef2fbdcd344c8afe87196bc80e593bad3c56413511a68ad7859a67ec51852850fe8060ac2298dbafee6f3e8486aef2e94a309b0ced827e5f6173a");
            result.Add("pt-BR", "433886a3b7072d8058200d5c54d4c156cb2be651d601a96c26cffa68d158f5ef4ed1acdd814f3a40f87f9bcf9ae1ad8e30de664f689e0e0049bfb71f025c48e7");
            result.Add("pt-PT", "5dd92ad172b224df00d2733867c9605a0ac8b2818a3bda775b3557704a4ed79121a8a40d6a6355d96926bc2942f1b19a4918bac469c4da8aeaecf0369f8ab811");
            result.Add("rm", "09cd15828b55f7802da5e704387a710c8faeb61ef77a0d8e93c1af70d44d357af1305225211c788eca6e176a9e008ed4c054f7ca3edc17ee2a082ef8ca0508d8");
            result.Add("ro", "468ca2bff8e9dcb495aeff86f58b50775d6037cf3ca8d6d9d0200b6cfd6f1410dd9c07eee173c03fbff51b03e2f44a83ceb1fea573d0ebc0c30acca4b63eda4d");
            result.Add("ru", "cb7265d1ff55c4999c460460cc4309cb59ec51ac20cc4e3c42193a2632d4118443842536d48ea27c7ff0ccbfb48c6d7fd62905099dcd146334b0cb41cfe492b8");
            result.Add("si", "9df5d0202aa949e5d3198f39f4023c73886be5af628dea9a57d23043c4eaa58cf2c607ff038735c0bcc8915483660d152385faad6f822119ccac04707f5a1ab6");
            result.Add("sk", "a1e2c89415722c629526d5e8af28a96f3b0e997c8ee699f7e0c3d125636ea7a279372a4d5c66d98db9ebe61d656fe05dcb687abd9d7a749a5cca0537e5bb338d");
            result.Add("sl", "6a4b8eaef686ea292b213ae53ec5e671839546108c83eb0118b339b708bed78bed29a3d00427163ae6383da156d49753f0178bd063e3d2c86a8fa8df3902a8ce");
            result.Add("son", "4fb0b63e06790aa031e0d616276f5c88840fc3a8438eed93df8cf1bbe27675fa268b87ee307a204bbc02f1aa458d6da410d96098ea8158ea85d86a97ae2b3160");
            result.Add("sq", "497080aa733dac5313a2a259dfd6e472a6a633832fc7e637071096f13d7e960fc69ac1a9cb614f969a949c3a9bad9a3cf1db14851ae4a85a217899e99cb5d2cb");
            result.Add("sr", "8939fdb1ce1fa2495d74415058477819f799435e566a23504627edf3b7d32d3aeaf6ee73afa78804cd9ab14b3fba413a413aefc7054125083b3afe8411ac17bb");
            result.Add("sv-SE", "28fb4699f16c105338b3ed7330e92017d57e40f5846fa4a6bb11ad1c29535dbd213a3178d023483d9703af966da9f710cee143c3df0d7e41e33ac100cac01b54");
            result.Add("ta", "793d8639255508e594af9b1b801fe9fd4354b807538230b3c58c7c667fcf136979b10b25692b8b9147b72c966f5faf66075d63b59dff4c5fb58f207fdcfbefaa");
            result.Add("te", "4c4a621e6ef1327ad7a0bd43c6a059fc6c7b4e8f3f7ed6fc5b5f51ffbb236ae0b3218f68587177ac4497df5d43f6c331d9ab8e443fec543f532605b00ec59589");
            result.Add("th", "55b64e16bdcd4a516b84688ca0ac3400bf03adc034d5e5cf3a4c288bc5bf29d7951b203a10ac913bcc3fb5a5010115ac9c37aa822656b46a0288d95c0a941bd2");
            result.Add("tl", "cff4a57ee995286990bff8c7c1230c548cf2b9e76a8d7791c9d9a92d74857e58c11edd60b43a616edb3990802cef54d4742fe260edffeb360f302bfa56e65d3b");
            result.Add("tr", "b93ba8781b6cef1686b753eb7bd41e815037d323720ca07dcbefbc17a9892e1e663a1ad774dbcea1ee7e8116adea87bd73e91a272b236eeae54f225cf89dae37");
            result.Add("trs", "a45abd5b0ae8205b9dd2e70a22299c358899ee3d897fd0fa7418f650969fb59adf264dd1c2204e240ca35e89d2053f2ec477467489675a0ac8f4a016bb96cd96");
            result.Add("uk", "9f881405ac05c3676973e47d42acc89fd6038e69fe3fd0c6e932a0b5ecc857f6e4925d8ba22fa5ee916c4a3bf758d7884930e8fa209910d6532c96774a199cdf");
            result.Add("ur", "b4752175959e85927b782e85213178f950e382f75da86ab564f8dffd276c63f27ffac047dfb26cc378a75400200e27392f9d62102d0235dba1ed5d772c2a5c7b");
            result.Add("uz", "98d37db7c9f842f5daf15f9ee0df239d4fac01553342e4c78571cd9238ab49b6268c73ed95c9f5ff70625223cbdacf655897d2e57788dadeef0fac5f1e9bfd49");
            result.Add("vi", "75373f2b0aa17af4ddbef746ef4650fc105974f4840e9dca60f349bd01d188368dfbcb327a49976e6cb032da0a59fedc933a40dc860e38a5de29d0261ef123cc");
            result.Add("xh", "3539e43a65b2ab5ad680814fe6dc55650d41775c53631fe81f577ef89c4d27c7fedcf90254af354a41da54a064c1063ec12075329c5aa6f08150278855e0f6c5");
            result.Add("zh-CN", "a2a24988e612a59ccd3cbd018d890f72325d6128e81937dffc3a4646ddb6999a4b4bc6a565d3cbd19a53518f7252a2c08ba2f4e894fbbbb98a7d7fd7bb2de50f");
            result.Add("zh-TW", "866cdab71be9c21443e99cff23f9485d60f29d9e50bec73f449c6d779b510eff650bed1f14321ba1450b93ddd3c72aa4fc36d9d994dcaef3de93677a459b2420");

            return result;
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/74.0b9/SHA512SUMS
            var result = new Dictionary<string, string>();
            result.Add("ach", "3df6ad7370a971c98f885a7ab0fd94466c39b55739c739cd222ae784733d85976c24a3e49de55ead73fddb3cd01c5106a7695792b13583466d8fb6e241d11c41");
            result.Add("af", "212af272ef48278c41b00219742f2bda80227b2e8d377c3d01446e301d3f64f9b05b4f8016e4d537f259d71188c4cb6620fea0e01d7c59909bc27e657b25a8f0");
            result.Add("an", "bdaa666041a1f261c601ee584152b6058f9606ce5d9e85e11014395c933e98477584cc2f54bc5c667aa9a9756d5420b5833efddf06df92da33dadc8d32bc45e0");
            result.Add("ar", "84338339e42affbc6e54e758c025e24730667e6afe40cf6f17d577d5ac22bd9325bef3d9f358b76c25cf93c9e51da845e9f2576440d9c268bf9d32785bc9f88c");
            result.Add("ast", "cac8fe8f0c7b594053464fa9121544208ece0b0580acfdf6ca677ea8936adfd1256a3d12546ea5837eff0e55d2dfb579aabef9ec46d5ba260f559d587370ffbe");
            result.Add("az", "045ccf5132dd214d71773ef110c082b66cc361a3989fecc91db1d476b7def361dcc9974209cabf5ec3f590e89d4e1225d8a112474e82ca0e6eb23d7ee2bbddaf");
            result.Add("be", "c017b9a07d7eafe880088a66b091b9f8d171fd8e9d6da6d9bac2c3b245b822627c9289e710a5a310edd5f0da7ee24659dbd05ff36a621f95643828e727fcd6da");
            result.Add("bg", "923566f6034fd6e87a50cb753e09aaed820a8c52599b52c49f3b95c478881c8776701d26886ea6911e3b85615624e08de82722321842f83964b6b4cda96339f2");
            result.Add("bn", "f921a966938e583bf42b87709c33bd3b167436ed52edb1e5033b835b9aaf40eebfb6a18ed57dbddd4cd1f09966da9e7ba99c8db36987aeb2bcc1ecb9cacd45d5");
            result.Add("br", "e12134102d12f7732638ee65b11d930589bf835d5a2267028063fac4e2c3c0438f7a972ad086bc34443d5e654acc06a79337dc1e21bf4164609a30f9d1c78c65");
            result.Add("bs", "24ab855d17fdd814c07af6cd8216c6a18e8fe04965e50412e4057d0d130b76d35f8593536ebe763f9c67829112824db247200725dd6809aa01e2b7a9bee2b81f");
            result.Add("ca", "6004ef19eb500e8d31dc09e3547a3ba9a1eb2469677c048b0ccc2c6ee026b588aad8647dac23484fd87a0e636cc5a695c42bf4c175fe7275eb7db66270b7bf4a");
            result.Add("cak", "7ea047fb3736ddc8eace7ab911c25216d09ea0b1febee6a81b704a3b995c87dd9a27da149ee185fb7ce4c028dc15bf5c08bd74cfcbfbaa54ff353f02847e0f16");
            result.Add("cs", "2ad75eba28b18536845fc3bf3a5734a5f7a2f3477f0b9756e45422480cca0bf55bf279d70a420d13a12ae21fd266f0d88eba8dd683bcee914f3bf8cb96f0a421");
            result.Add("cy", "758a906dc959fdd1c0593c617a28c5f99eb6416aa958b042cf1f0e6550e73dfcb692954852bd22130055869079e14f0ea3574e6da37fd7db9bb95b1c700b12c3");
            result.Add("da", "eef301a76847454086712860b69db7e2b1da6b586d5c004cf0e205095357c8c415d473cc416ec6b8bfa0770cbb62e2d55762f7fcb2fc03b618d367e5336a3c2c");
            result.Add("de", "61a95ef84bd87da6e9919ea97ee33b4d93c7a0e39c19c99b2482f37a44f69913e84387ed510668465193c5135e6ddacc6fa357be136056a0683c9e39b31c51ab");
            result.Add("dsb", "c133279030813f00e9d64b44379707aaeffa536d7a04b690f61372cc970a741d71488448fe387d73621fe87734e7ae2ff6d59cc3c1a5e0ff30459e3e6be8e1ab");
            result.Add("el", "98d9e7a37672c156967e0d387d945893035a92f5bc03e63b962e3d500b9e626f15daf319b316ea2cc53be0b9878160730593657fe9384e3cdafd1e61136d33e0");
            result.Add("en-CA", "a89530bde21f4d5d5e7e696839d36b9930ce7eb0c7407ca828d548184a86e9c1e2260fa8861aa73ec577118713ae46d61d6772c417fe5989b579d4be8196472e");
            result.Add("en-GB", "e9f4c8ad1afc7857baef4a533993c98d3ec0403d4bd896cf59ae2d7bdd1472b354dafa0f0a89e12f1fda1a70aaca060e935301d8c0b017ce6dbad7479f36a0fd");
            result.Add("en-US", "7707469fa7d09d60981af2b8e0fd00d65f6a09e1f82cf96ece1736a12a25e83190a0eb98126bfa6faf5a5de4cf117f903da357dfbc29afa08d2c0147b9ee770f");
            result.Add("eo", "5058fc21bee81523607ef6bb84bef0affbeaae61d68881d5b16d113214d14cc8b8808359e148503b52404dd30f1577ff4f5028b9a9d05f6c5e51c66c39b15070");
            result.Add("es-AR", "04286da10806e8c1439358188913c1ef27091f5e73f0dfe216cfb5517a3f80b3fe44d17ac0c786a57186d246d25e42f33ca35106f2d13664e29b8ccb51233f51");
            result.Add("es-CL", "897ca3d2d5f35260e53cae1b126f635c4819b8ee0c48e65fd79e282d3b83433dfa73cfa31185f79acaf9bda4fa0eba4451166817cef5d4440033c7b5c81eff5f");
            result.Add("es-ES", "f8ed614fdef0f9c23f7c149194615dace8177ed081193c0fbcfc7a43941a769955f7f78b6e5a2b051507878739878510baf3a0dca8abf75aec6e974339a45714");
            result.Add("es-MX", "b7e7e938a6a8909467a5bc6aa7c1eb4a1f5bf235a2c02924610a2a451fe5b53da4d41870a035393842da7c1254efcb76d5b8674a1cbf22c36fe12851dcdf6209");
            result.Add("et", "382995da529a85bd7692fc0136eb6a90596eff732e506f7c9192b9539bfd495f0c546bb9fbf4e56cfe0ef9897df34aa6e1dadfa8284cc918755f58e5d98f9e2f");
            result.Add("eu", "b9bb6496b1f7c8195734613893ad110c516ffa1c8989d98fe83d9468bdcc9f79079f18f043b32454bb5d3c1ab4bf96e49c2f70f71abb4f412e3777cbe74d1112");
            result.Add("fa", "e2a4779e4c38bad2e9ff5a10200745c9ac0b886b6539e7f6cbee92e4bfcd2aad323cd7e67324e26ba578a3cf5c2248acd9e715bb4be710bac55912892ec57239");
            result.Add("ff", "e21082157dbd133e063214d52d9fa317fbbe1c85623def6b4595667f6b6b2b9bf80bd7402d0a1a0f9310049fbd4e6be264365bda5b3aa6a29bfd160aad34d17f");
            result.Add("fi", "00a9ed9855c7459121422f40b229b6bf7247da2d7e01314270ddfe225450c5617227ba00b25c5c9d10021f211a032669c92abd4fb97c296a1ba331221456e9ef");
            result.Add("fr", "a625eca015f3982392166fad848903ae8caeae45290ab530f4f74f670185d8637e583dc23d926a37faed4647a6e77a9f7885ade3299e60b99c9d5963f3abe71f");
            result.Add("fy-NL", "5f7e57534bc5da4e8494ce385c86893d4b416b0459862ffc271263e0e47a18fc6c5a16a7f1ffcfa6d75a2f71d094fd37dbc996219110a67551560346d942f1ea");
            result.Add("ga-IE", "93e86bf5bc202419f5e181e82c62f959b83a3beb7d93364b4db634059f7d4279a735ad0676d1f24c208ce98fbaeefec97a16241a37a2d121d3e69ab083c78072");
            result.Add("gd", "3ceeaf8b60be5d7b905460c80e4b7aa425fe0b91ff4904792e3715c090dfa018cccbf4a962c997e8bf845c945db7b69a589fb35d81c92355abd30e0765be5441");
            result.Add("gl", "e7e15828c5d8b2a4befef3d8136cb6d2c21bc89dc889e4af5d74af7f18ae2559d856ee1cd84d2fa3c291bdbdb8ed114821e6af803fab1db3f6ce52e8e54b5cc4");
            result.Add("gn", "f4e3d284fa9c5f178c5e3c45a2d8793f6d76e3e7311a76b11a8ab98c3fdb12b46e63e955d8db11a2d9da2d246e3a8216df7f6e8d6ad963ef54b7b0b9b51b13be");
            result.Add("gu-IN", "26034d1b446b92e8fa3c589c6071d7bf2ab9cf47128f73dbc1a7fb4a83aede79c95a5a0022d9aac07327678124d8d66603168adcb55fccf46046c4f118bc76d9");
            result.Add("he", "1b937c017d56517f010fbc31dca675630e66698aaaf2734aa58c6864c8f047e213dceb23f407042393e59bc657b9fefc74d6261633992655e0b2488bf1a02278");
            result.Add("hi-IN", "08ba649061248624933316be5edbc874c747dd457313728e27be5f858ea825b4c8e680140f3e358f7d1c07fff97df1506736c588e5bbf3f07cf4bd3382fa961e");
            result.Add("hr", "89b8c938176c1a0bf4d4c6e83ea9457312c51d782e06a0dae96cea2d05f6137c96ad9007439d221d0b402a81e999bb999ba51c9069774d7728001857aed2ea56");
            result.Add("hsb", "c2f43e30f94f0aea63bf12e0fbe046def41772193f78844dadb9c4ccbd457a66a1c5743086353d32f04b969bbccb84249700754df1b3393bc37b2832fcc20d7d");
            result.Add("hu", "f124ceed55fe7424955b739c40eea446101cc1e00a738927c0e715a3920139d3085b08fbffdd1cea02636662cbc7a572030a5d5cc29f619fa4c5815316f5534d");
            result.Add("hy-AM", "ad999bb08cb5619292efe3e3335f9002531d33e96b8e07bccaa4fd321d7a76d18ab541151e3ffb5f0b20a329f4eb2e27f8fac44026fcc77aa877ff94e4a3774c");
            result.Add("ia", "3aebdb405603b8c2b00a88d19d2a863aef21874662006608d07f6acbd758fbfdd8ee41e56c74182507c15890c1615e026b8e0d0da24c7179fdc5c11513cb5132");
            result.Add("id", "c8b462b645bb1a43479e27c8a88c0c452214836e46b1a4001b7aedb493a2353c99cbd5accfff9a51223c210687f07d12288b537d7dcdbe660bcc7456f27ab021");
            result.Add("is", "653f20359f0cf192761b0fcb2027e0b5e84179a25d5095d933ab62051622eb733d8e41651e9cc9b3c04767badf66eead296ded2e2d50d36398859199df4b7faa");
            result.Add("it", "a5a43596b9b7e02591bd3f804ea5de33353041f0aa5aeb4c0475b8ce6210a78276c9d0569ca920bd375adf9b55a508895cae564ba4fa956a8693656ccf9b3bcd");
            result.Add("ja", "070f72edb02c36438458b846c99e799d8a8c3df82530999adc1c3cae3baff853957b94706544c10babd44414fb97045a371a0342694adf3464f575db3a4e4230");
            result.Add("ka", "1a3bf2df5b626f93fe133467508c86ce014b1334bb7b73291a92e4a947633095596f0648aa1472b85f6eb8ab12595cb2903c045d06435c6a7b193a3f2756d7bf");
            result.Add("kab", "2259c8e15f931ddf9a3e51f5b10424da3a934d51178239bbc92f914988e7df0080af690684e7e564e1636edd16f67dcfb2414709b777841a28b252da17b0756c");
            result.Add("kk", "f950ca9f75177e5c6db5a4a7c4a3414cf249ca82b8b17ab816088eb80b73bffe6a6dc4171ff7da44ea03392d0458c5683f4f5584cb2877dfd81efba5daf6fc4c");
            result.Add("km", "577240ad23ec4791c2ead3b5e5c816cb71308b5424dd4fa997fac3d0a7a2cb9b8268e0fafbfa4b7dc0b9e2b57a77ecd6d22afd7717b30a0adc138c0643b040d9");
            result.Add("kn", "dcbcb2e439a24b59655d72a1afbaa8e49fd75e87a0c83278abb1249b15326810e41f5b2bd4388558ed22c881ebee02bc960bad08bb6c911944097c052f8dc364");
            result.Add("ko", "015b627ea83c59c5effb2ae1e7254b21dbf223465f225fc3e8107da9a9112215e43081b7a4352293e0ea6921688d634a32171e3a5f1ffb99fcca8880286d05e6");
            result.Add("lij", "e6a14e7c113ad03abce04e0ac7dbf0527a4e6ae73b32838987e0dab3e22df45f13adf87c363a5800debaeae61a8c055c4024b2cf387afedc672827bb07e30f34");
            result.Add("lt", "5c4741bb205250ca6b2427f65a52f82c83fa98185376466422bf053e46b8a9e6a6ff277d29092f72c0f5e262ef068eab2e3f709e3174cb702cca8add26be96d3");
            result.Add("lv", "3beecd9beae00ac5234f918d55b1c756d151b04c9d0be136cd56bbd68d131cdd5d89726b675082e959a5bd8ed4ff45147712052a33be648b73f3c9d383e29f70");
            result.Add("mk", "f86f5e23b4f0cd62b2101936715136bc6af2788b2063275741fe0aec2ae9451e23d372e8f4e7cf417ebb8fd8f58f77f76ebd9d568dd56ad6525905de1e5d68b1");
            result.Add("mr", "2518b4ae65adddbedeead8aa657fba55ce247949b0f1c69facbcc07341afaaf8248354cf040b013626b4ed603fc11988fecb232d32a54daef643bc713100c7e6");
            result.Add("ms", "4456c8658b8fb5d83eae846c336bb2e000afeb8ef92228047fea99a8771bdc70422e94ee658618f7616623fd191f07f5ad1334c641abf58ad3bd36e0962a6140");
            result.Add("my", "991e945de1382355555bb5198947ffece1f14031b6b52669ad75b735e97e81d1613ec48e58cecf88536f32b341ae62b5b16d547b551885c6431e68fe634ac177");
            result.Add("nb-NO", "ce4eb321563142a3a4a670bcb5d7653b839b3122acd442b53351478b415d81de41b6ee043e2add5b18403d45ddc31a2d82ceaf1ac6ce6fb4748631deccafd980");
            result.Add("ne-NP", "73e1a8a4afa4a994d526585041fccd9c036171cadca0aefec4a25e1e0db9c41b59c163cdb5760827b4feb51380e03428458ea9fd513c5b06b1e3950c04ba88b8");
            result.Add("nl", "8884d4f89db778592156fb1b54fa95b6c6ae1e26b7a22fcf9d05a596a45acf72801bc5b80c0f5ec9a6eae22df07f7c2e4a1e6ebb2c636702053879572e170872");
            result.Add("nn-NO", "7099aa37c173b7e6260d61752a0b552427975d150f5e161dd1b41978da7c8cfed1d844d6e43a0909a3d8b9c919bf8ff0a69f8f63ebc702ddd0c9bb2d6543ba40");
            result.Add("oc", "bba2a7d0b03eb1414cc79a9099130348f421461849885404900f7f39ab728f1497234eb925a8f3e669d4a48685ff718360021adbe66a44bc0b2eb555c545fa7d");
            result.Add("pa-IN", "b9df18d5ece956d44615b6b9cdbc483a78aca8bec7eb53e1056bbd440b03b5a3fd529ba0f781012efebfd640d3652038fa36ab4782bf612c71a58a7518f4a324");
            result.Add("pl", "4639d62001dfbe5d27cc2ad637a902c571b0d214235d1ef2601bf9e3b411034a7c0efdb0e0a51f9481fed352e668b78f8c25934e4356bbaf09ad9142e1027d92");
            result.Add("pt-BR", "a56c645152f80357fcc08ba12e14e411d025d55fafdc9442e7977d2c0ddaccb6cc41d22cda304da2481af896822ea90fdc2efa82fe787fdda0712fc13dfeda40");
            result.Add("pt-PT", "9874c6d2f6f947756bea7fdc6060ffce2c3f63802ee5dbb932171462ad75b1b5b1cfd9b5eaa58bb5a74da1ce332228541c90b771cae6705bcdc31189da3001bd");
            result.Add("rm", "00ae996033c472eb0ec6808a183ff9e182f741e855e56b9f896f5efc92efb363a85d21d3cc195343690015db97cd39540a6e03c1e53b811640ff68b8cce1144b");
            result.Add("ro", "926b3c49e42c0c758845d3dcb912de872308ff37fcaf7ed9d477a8cc756f2f27310564004d6bf6c8c8e5e1135ea7414e5ec4caaa50ab04a9299ca00a0632148f");
            result.Add("ru", "cf5973398541254b3ec9954406d4ea34336f97d5a3da03598c9feb1847eaafc9ead6370727783d56e5ffaee22d7fb9f44978921b4e394bcae619ea38743f4776");
            result.Add("si", "bb4866b43730a7d4cd45bec67470d6909abc5d0adf384e4154a26e9958d188a4b9abf725a7ada2aa88defbf492c1019e7b181ae23e7b773675bf7174859a11a8");
            result.Add("sk", "613ab8c9a65d69a5361d00609487416cc57900d0839605bfc58f2a4b915d671b4678ebf2d5204cef0d785acac5c52718c725a0be482cfef7f1a5469ef1ec6971");
            result.Add("sl", "0a8885ae7f58eaa07f7b71c3c68beb846c9c2c29fd6e5707dcd82f45c84faaa48ac7a04eb4ecc11bbf2666ee6454f6604227926f4a5c41b96b3a5274e8f690df");
            result.Add("son", "9d86db355cbf8b9f97ed31323c2f57e9cb2200487c53ebff0bd1607f13a19574c8149e0f1198ad197f3680e3da122c1a0d4940c60e2766c550b01f41b2eec70c");
            result.Add("sq", "f8f2e7561200c0f2a0672ac144dcb2fbde3b774c9ac4e06ddba5588aee39ba871e7dad171e4c0e29612e6d51b61688806d264f6d0a56e3ff1a4aaa370b72df94");
            result.Add("sr", "399bdfcc784972bd3121c7a051363c10889f6503adf02b982075ecbc49da4f46ec5e194fd2c0f7c43c7435e9149dab96469c38efb44a0a85959443be45a8daf1");
            result.Add("sv-SE", "815d16761469ec57d95eab6c45365cd5112bcf6dc651d202fd2fed258e4437c9ea9720ef1c48dcb0e43e5013f9121f4b3c67f723ce414af7932bad71cf60e5e6");
            result.Add("ta", "dbd07917be1597d2c7c570e8b14fd4c26491a900f307d76a2441e58004d6d763d58f1c5f6d52b2a94dd24a6c78ce9cec0dd2b006127ad686a003b1c75ff733a2");
            result.Add("te", "540a7b9a543ab90b532134a24b7d7b668af99e9b66899f17217e48ac42a5b7aad0e0cabef9a450cc900aa7a84383131e0203e7306752a154d11d2f8e4d7e5440");
            result.Add("th", "bd116b5b190a4e221d5bc2288319e28de1be9f2807a308dbf5dfd13ac9648eacaab5642b40dd5202be68bed463a89e1e3ba35364f2ce3297839e53d07b3fd831");
            result.Add("tl", "22ab681f5723f86280928c0dc4c73afdbd66b340fb5f9645638cbb2a8570635c2a0b9c8f26d20baa7cdbed5e8492de6f0563c1f86db2c75b50bdfdcfc8a53e6a");
            result.Add("tr", "faf652a7ce722408d3c0edcf7ced2ab7cf36694603de8816a78aa40860d76c589381295756511e59f52dd7fa7f69296ebd59f40fea3759f4b703db6b4a97ce80");
            result.Add("trs", "28b053f630f4233c4f558eda779edfb644a5dc4781af9ca069141de254d8ddfd8d1d92556b258bd6bca3cbec7606445e11693e3a05c8ac6db3b988ce1c0fe576");
            result.Add("uk", "6a5a3019da73fea586350b960eaed40d1061f89bf2e3960db41c9c6b49994590186062c5c27d24f949c7cc8d3f2a4026eb00960273397017bdaeffbdda98e07b");
            result.Add("ur", "373bb635dd6c0869457ca6637e60f435f6940ae8bb0c200eef45d936653c3e2ffac20d78bf25d6c7045149ba184811af03f1ecdc07882bdf489d087aa3815505");
            result.Add("uz", "735e90890d20a00123029c85a765157b4c66a10cfda35e5a40a7ef4d809954f431d329c31281eecc1cce4f9efa47082ca2b5e7793979a95dc30fb60ec858efb7");
            result.Add("vi", "2436552cf51452b06925efdfcda470b1f968dcdf4167ce0479515bbe2657993a0ec01e87ee12bc3359934ae815d5fa75c462271e016fd38950f4f65078867c4c");
            result.Add("xh", "44103934c39a1e65d4a63c9922a184af20fa473dd4c2dbf25700e54c8bc18297b34cc8dcdd840dfa4861f3c8ff61b49d3534dfeca6811f493bd285eba2d7fe4b");
            result.Add("zh-CN", "dcf5372fe9e69d539ff6737177c3d0a3f8e78277a7f34599971ea4992e9204da31e8975f59d00ac58889f46a0fa34a3bc74b01b9a54a84080e1d9987adad8997");
            result.Add("zh-TW", "ab0ba1c17ff96794b67430d669c2d9988afbde3f11a8741b0b0d1585723d87fd854392c422877a6fb684327782bc76a8ba6f1cb4b6aae0c71e4c657a31f9d67b");

            return result;
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            return knownChecksums32Bit().Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            return new AvailableSoftware("Firefox Developer Edition (" + languageCode + ")",
                currentVersion,
                "^Firefox Developer Edition [0-9]{2}\\.[0-9]([a-z][0-9])? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Firefox Developer Edition [0-9]{2}\\.[0-9]([a-z][0-9])? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    null,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win64/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    null,
                    "-ms -ma")
                    );
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "firefox-aurora", "firefox-aurora-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox Developer Edition.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://ftp.mozilla.org/pub/devedition/releases/";

            string htmlContent = null;
            using (var client = new WebClient())
            {
                try
                {
                    htmlContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Error while looking for newer Firefox Developer Edition version: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } // using

            // HTML source contains something like "<a href="/pub/devedition/releases/54.0b11/">54.0b11/</a>"
            // for every version. We just collect them all and look for the newest version.
            List<QuartetAurora> versions = new List<QuartetAurora>();
            Regex regEx = new Regex("<a href=\"/pub/devedition/releases/([0-9]+\\.[0-9]+[a-z][0-9]+)/\">([0-9]+\\.[0-9]+[a-z][0-9]+)/</a>");
            MatchCollection matches = regEx.Matches(htmlContent);
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    versions.Add(new QuartetAurora(match.Groups[1].Value));
                }
            } // foreach
            versions.Sort();
            if (versions.Count > 0)
            {
                return versions[versions.Count - 1].full();
            }
            else
                return null;
        }


        /// <summary>
        /// Tries to get the checksums of the newer version.
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32 bit an 64 bit (in that order), if successfull.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/devedition/releases/60.0b9/SHA512SUMS
             * Common lines look like
             * "7d2caf5e18....2aa76f2  win64/en-GB/Firefox Setup 60.0b9.exe"
             */

            logger.Debug("Determining newest checksums of Firefox Developer Edition (" + languageCode + ")...");
            string sha512SumsContent = null;
            if (!string.IsNullOrWhiteSpace(checksumsText) && (newerVersion == currentVersion))
            {
                // Use text from earlier request.
                sha512SumsContent = checksumsText;
            }
            else
            {
                // Get file content from Mozilla server.
                string url = "https://ftp.mozilla.org/pub/devedition/releases/" + newerVersion + "/SHA512SUMS";
                using (var client = new WebClient())
                {
                    try
                    {
                        sha512SumsContent = client.DownloadString(url);
                        if (newerVersion == currentVersion)
                        {
                            checksumsText = sha512SumsContent;
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Warn("Exception occurred while checking for newer"
                            + " version of Firefox Developer Edition (" + languageCode + "): " + ex.Message);
                        return null;
                    }
                    client.Dispose();
                } // using
            } // else
            if (newerVersion == currentVersion)
            {
                if (cs64 == null || cs32 == null)
                {
                    fillChecksumDictionaries();
                }
                if (cs64 != null && cs32 != null && cs32.ContainsKey(languageCode) && cs64.ContainsKey(languageCode))
                {
                    return new string[2] { cs32[languageCode], cs64[languageCode] };
                }
            }
            var sums = new List<string>();
            foreach (var bits in new string[] { "32", "64" })
            {
                // look for line with the correct data
                Regex reChecksum = new Regex("[0-9a-f]{128}  win" + bits + "/" + languageCode.Replace("-", "\\-")
                    + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
                Match matchChecksum = reChecksum.Match(sha512SumsContent);
                if (!matchChecksum.Success)
                    return null;
                // checksum is the first 128 characters of the match
                sums.Add(matchChecksum.Value.Substring(0, 128));
            } // foreach
            // return list as array
            return sums.ToArray();
        }


        /// <summary>
        /// Takes the plain text from the checksum file (if already present) and extracts checksums from that file into a dictionary.
        /// </summary>
        private void fillChecksumDictionaries()
        {
            if (!string.IsNullOrWhiteSpace(checksumsText))
            {
                if ((null == cs32) || (cs32.Count == 0))
                {
                    // look for lines with language code and version for 32 bit
                    Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value.Substring(0, 128));
                    } //for
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    //look for line with the correct language code and version for 64 bit
                    Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value.Substring(0, 128));
                    } //for
                }
            }
        }

        /// <summary>
        /// Determines whether or not the method searchForNewer() is implemented.
        /// </summary>
        /// <returns>Returns true, if searchForNewer() is implemented for that
        /// class. Returns false, if not. Calling searchForNewer() may throw an
        /// exception in the later case.</returns>
        public override bool implementsSearchForNewer()
        {
            return true;
        }


        /// <summary>
        /// Looks for newer versions of the software than the currently known version.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the information
        /// that was retrieved from the net.</returns>
        public override AvailableSoftware searchForNewer()
        {
            logger.Debug("Searching for newer version of Firefox Developer Edition (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            // If versions match, we can return the current information.
            var currentInfo = knownInfo();
            if (newerVersion == currentInfo.newestVersion)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if ((null == newerChecksums) || (newerChecksums.Length != 2)
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
                // fallback to known information
                return null;
            // replace all stuff
            string oldVersion = currentInfo.newestVersion;
            currentInfo.newestVersion = newerVersion;
            currentInfo.install32Bit.downloadUrl = currentInfo.install32Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install32Bit.checksum = newerChecksums[0];
            currentInfo.install64Bit.downloadUrl = currentInfo.install64Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install64Bit.checksum = newerChecksums[1];
            return currentInfo;
        }


        /// <summary>
        /// Lists names of processes that might block an update, e.g. because
        /// the application cannot be update while it is running.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a list of process names that block the upgrade.</returns>
        public override List<string> blockerProcesses(DetectedSoftware detected)
        {
            return new List<string>();
        }


        /// <summary>
        /// language code for the Firefox Developer Edition version
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


        /// <summary>
        /// static variable that contains the text from the checksums file
        /// </summary>
        private static string checksumsText = null;

        /// <summary>
        /// dictionary of known checksums for 32 bit versions (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs32 = null;

        /// <summary>
        /// dictionary of known checksums for 64 bit version (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs64 = null;
    } // class
} // namespace
