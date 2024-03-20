﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022, 2023, 2024  Dirk Stolle

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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Manages updates for Thunderbird.
    /// </summary>
    public class Thunderbird : AbstractSoftware
    {
        /// <summary>
        /// NLog.Logger for Thunderbird class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Thunderbird).FullName);

        
        /// <summary>
        /// publisher of the signed binaries
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// certificate expiration date
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 20, 0, 0, 0, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Thunderbird software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Thunderbird(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode) || !d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 32 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/115.9.0/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "b3d537ab9a8053c0dd070dc50adddda62c6cecc2fbc631ab0931804fdd14c22829183861f7967402f6e2975fd417481b00172926b33de623573f25d02e7d00ed" },
                { "ar", "d2f02edc3d73152094523010a17ea18d6e7f32417d0d8335e90eaed6e36c2922d59a63adbb24b175f6d8004fa96f72dc5832587d93d15e6b3cb500edec03cf77" },
                { "ast", "d114fa593907cdee5b8c7febd0a25d0b88ce7881ee120619230fc71d36789245397f24d36891bed4eb31575f28f80561953d6a9058f94c8817bdaeedc1c74ff9" },
                { "be", "f8a7768fabb531ff66be7479903235141c8d18286b918abf534e2434347b50aedbbb41c333f7962b88bf6d2e264c79bf1b0fa801df74313f4994111df7c9144e" },
                { "bg", "5bd9860985be4e49eb3a4aa0e9bbac552c0bd8dece95a25f886c350a14afee90b2161aeb8d98de5ef943bfd2837e6c07e6bf261258458badcbb2736fbbbdbd8f" },
                { "br", "24aed6a80f2cb0510386bbdb768a5c4093cae4d54fffecef88a7017b857514afb2ec735b7146b319ff02a78b1d884519b72fa47920431fac29e97c6ac5fed7a5" },
                { "ca", "c2293ef4afed3afc57b70672d2f1ec5724b92ea3ce0bf39bc89b062aeca03bce467d94a3578556ead630d8d261eb48bf41e3a2ae13ddad219ada39e861ae6bad" },
                { "cak", "9cf1eba3c983cbb848ab137b8cbcff3f288e477baf688e1493a9bcfa8e236cc2b8244bdd94210277b6704747544c1695f1f498c8ba678a4b7abf511bae94701c" },
                { "cs", "a60f9f0cb7446d0ecaf2a9c25636d31ecf48af4f7f69f9714c683291635359c5e97665e369410584426d87a381b1ec959c9fc1b952c1881397fe69bdcebbfe4d" },
                { "cy", "24a1a6919edb233384d62abda7c314c445fc1644989f02c43d0987a227cc025e074e30286f7ca462b4780bf0df74d0e536242a5ecfa69ca018cb159a83634eec" },
                { "da", "e7adb4a618922c3b79e3834918cdc9a0e5d6886439e5b9986145aa047e98bfeb03d49ae08ec1f5399fae90bd8cc092ad4117d92bada4db8253385cac5a010eff" },
                { "de", "021b6905a0518ba5a658df13c72f442a6d2e7c55e9a31f97128127a30c5f33d32d51847f29f727b00fe44afc3b9737915bbc91d3e07451b41281280b7aecb911" },
                { "dsb", "da6d1f1bac6b966f107515b9083200437fe4d5cfcdc00184887b14590e4ec0a1e99d2b21993e1516c52447259e3ed30f07a27fae85eb20ecd33cc980391aaefc" },
                { "el", "4820bf7ebd05876de9304eb8b19f8259bb2522684af92a1a1efb29401434713e5665ba1745f6637a58987b52c8ae0e53c4295d32d92aa2ea5d6fc981f1d390fc" },
                { "en-CA", "34a4ef7c2c8b34918f1f482f5484f966d2ad2e2065d6c84c5146c4011f0c3ee4024c90e134d9f2b546227bcb623db63cf87fb3d2b3f88c06f8a79d5002c9c8f7" },
                { "en-GB", "219e3fdabcea5a86c6efbd2be852074bc5f262e643c90ab55a76bf464a3ee36076c0cab866fbdee00130f5160f7fc21e7e97caa4d2f71d699c9ba65e7d25690f" },
                { "en-US", "5a8b894dcd86cb6f842a0060f2141125686619be3b50c26922517a250974288d3e02342a7dee612ee7c8092d92f05249fad65edab02c51a1dbdae068a3fcf41d" },
                { "es-AR", "a0f4baf73b354631496eaa69ac1b36f76a8ab6113c4e9d82be3fa19eccb49d97c4647683e52c0f6424505fdb839bea0eb9b1bcdc23ee97f5703fc49d42141f83" },
                { "es-ES", "0c67fbefe04dfa4c73fc7e2c3f13613945204c37e0260df8a08cd74a454b13d1494ab01de0566f78b9d9c998cfb0df88b3c57c2c901ce9314f9c852cc7ccfc6f" },
                { "es-MX", "202e4d69fbf0322c37345df021296d76253537ddd4c15c28f7b8dcda7c401597f8f239906e89fd23d21f6edbe552891be57134771435b66ac79d8ce0f3f73d86" },
                { "et", "8d45d3db6ba08f0dea1643ba59fdc2011f613e140b12ea1d779cf15b7b684fc86af15f80e828765179e28aff8278663100a9409b8a371b754a409298149b774d" },
                { "eu", "42de87275b56acabb1cd58317cd58458b539a039fd1211c951a1c2292f4dfbacdf05cf75b6fb9fa107d23cc608026c7665c3f2634014d5442e6f62a008dff669" },
                { "fi", "c6098ae1bd936c0d14786153f1a7396f556b58385878ac0b155820e6f945c3bc5d528b7684caffef48d0d4c8c4c592b0b931c81be2bf2c4370b826bdbc6c8171" },
                { "fr", "9b27dec2efc15e3cb551366a74c6f57218e1cd709127f4c270a56593ba6dbaa4bc1ae7e96a06f0eec7b9d99558fa930a5b6010abcf1ea0c308d090e50d8c217d" },
                { "fy-NL", "f49995a1e27fdc22fa6f6a173d079af772c5a9cd808254a210ca684a631faf679bf3601bfbdcdca3b5d709ecfe842b3e0603acb98fc5852dc1ab3aed31bcfb39" },
                { "ga-IE", "7ea4b3828287739cbc8686fcc924a9de14936c844fbb2da394a6941fc7634f3cfdbd682331e08b0b33b7b78ec61f8c114c6b4069905e7c77bf5e1bcb2856e6a6" },
                { "gd", "8076d7baa46626347ab819aecbb07687468781ef8fe673aa0511d8f4ffba5c4ab0fd2b9b2f17b4d702a27bd605d91e56691885ae32c2af0677ba77ff7cda7fef" },
                { "gl", "e4bcab743601c483830b6ae1c9bfed99565fd727fa43450a625f8507ac9ff7263b200d69225356b568da407bdf48fc2ef44919ac26206af716dd64f7f4e07ada" },
                { "he", "a1ac0b6c529433122351789705cea579866050e8f1ef3842f633b764da80fceb0435780274f676a8b167c7bf72dd2d472b86488a7c5ea0f61b39f7049de58117" },
                { "hr", "e9993fb4142a0f3be672bced0ca2fcc5bda5ff8b33cb7db04a538036caa9db70c999ad6190884afa8e944e6778e85ec1b43df2d34c806e84f7b73a8409c75ca3" },
                { "hsb", "43c54617d4390762ff7ed46a957bcf02726ce9f49ea49c40ab914b55197e687ea75a80a8e833c80d1094b4750774b29a9b21067e4ee7121799051d4b5fe8cddf" },
                { "hu", "3638aae65b95cc887633f8f31252f37a6df7c227c8afe9035c20bcba1e870d271c8ec59361ac35907b107b76e91b0f5df6240cefbffe1669bad7aaf87d85136f" },
                { "hy-AM", "9c6cb7979b4a01240da287d3b7e8aad4443cbe4141c1e47c6b275f3fb24ecee79ef76f2e5e2913629b21dabd08516b707c1f466d0b995caeaea4b04f98edbe46" },
                { "id", "e07d660a4314a93ea4702a35715466fc45bdfcaa8bb98b80eacd2b2df6ba101a24deae23bbfa8d1da13cebe29ce2d0843bf93ea988928a419bc43f1318ce2461" },
                { "is", "f9866d85eb9a5c2eeb3e553cb251e4656a4dc4b596d9ffdf6e5343a067dd209cafed77bd2327588d075678e003ec728130c537507d9043b5ca042e62366188d7" },
                { "it", "4f960d26cb8166120e673a485711bf0f5664df860a4a409692025b20b62ab5cfebef27a12d74079a8c64d36f688085fa3691a842482f44b9fde390a1b355e161" },
                { "ja", "428391b9fd7cf8b7a0a2ab44f7b786180b57645f13afb392b6062338115360c0e12c44d3c1983943a466e0fc0fb1cee7bd6ef16eb6e7febe8e6ac8a14def3e99" },
                { "ka", "6cf5f8c28be068a7ba61ce36cfa3b78707436737e061e860319c56d39e599cf8a06a0dd779a5940a1abd3209763cc1da035918bf830200bfb5bdd6b66156a937" },
                { "kab", "eaee58355667a1b06fdf47c2a0a88bfbe0ae6f85f87e6bcf6a317314a9a6d32a5464b847dd8d69f2630fccd6eeb6736449969212aa866c0a00b78b42b2d60b6b" },
                { "kk", "82d7cb60cb18a98524da0a8b48fad9e8ace65b21122ebc7be46f3af6b96385c380b3b6b57171f0379323f42e3fcc39b75afca71b478a16417872b175721a8841" },
                { "ko", "4837fa7427f8ba36f9d9bb800cffbb73316a27218c9ab289a8afbb9d1bc593043f17137dd77a6a15c4e52d28ae6ed2b74d23400e61473e2f60255853aa30e457" },
                { "lt", "b267f81395905f56abffb750873cc3e56982b04277385f302b5f58e4768801776d4e6f35bf6b5398ec9693fbb97904aca7aca3db431e614b73edccbd3a166bfc" },
                { "lv", "452d4bd3d9d6a261938bedfbdd21e270bf3b327e26b5f1798eb1297f71833102d7ee1fa3c3bf4c8791118fba3f9e990819c1fdfbd96b810a458035ff45705c35" },
                { "ms", "fa06264c93f7275ee5d5b37c67efbf879e436e4a9fa70f4da14b4b1ee0c35d91ffd62452f1215e783bc5b75addcf2e0d245a9e65c13794b260a5d72515097ab4" },
                { "nb-NO", "7aaea109925d3784087e85d6345d3212783dad3ea0d57727103df73900d7a72be43aebfa3ca4e8554c16f25dbbf852d09c993b7e2ef18fe8d05a34557b19d2dd" },
                { "nl", "d9b3647cb761a80fab34866d3f387d0c2620d1c37407bf35f25ef44746dc2958ed08afb3d3fc0b6ddfe91435ceecdb6e589966d1d847f74ce73dab94f99ae4bf" },
                { "nn-NO", "e419d961a2447955bec7b7f3898cedbecf50fe604b859694f1792df5c87bfa3af52095853a6a08b83eb722433b846bf39ca4f773645fcd1d5ab3315d502cb2c5" },
                { "pa-IN", "5cfbf52ea18a12175a5262abdedf05371c596a4b76f3b3376d4c4325d68d864bbd9504d008d513f1f9c499659e8f66678dedf52eae8296644c8fb2e15cd22b2b" },
                { "pl", "9bee5fbf6b6a478697ab525b7ce7e1c4abcbe402c72ed1cc2b5234f4e2f114e3eb5c370dc0d8c39fa5a550755db523b78b20fa8c5af20f3c294f5a2ace6e70d1" },
                { "pt-BR", "4361162d4f5833e0169ba1dd0e267f31c181814ef5a750a5af22df3015678638ac0dfa9d78481a3f3622e6a989cdc6fe6dfa628df9b73b913096d9ff829c7a68" },
                { "pt-PT", "4c34ff57dfddfae1934f8b97422862ac78571698260222dda446a04a6587e14cef136052e7e9193cd5e705392a078cf4da7c4bdc0caa92ca2e26717ba54b6304" },
                { "rm", "dac1b71df16a562a57fc4c5ff1b21e4c8ff6d2ac248a3d5c20130433c9e4a748bac1c098d810dd59d68ff30a195d614113b6cfdb765617d94871249cd677ff24" },
                { "ro", "1c142210e9257a59ca282f9d1296c7cf6991ba2594c886cc2087b37a3678120c6dd594ca4c25fe6676026db3bd789409f56dfd51c465ab96caa91a01ab00ae5a" },
                { "ru", "e2491e102b7b871fadd7a668c375041d2a63b3df0ebe7401d07d364f5626bbad32793872909356b116a1fadd4c3e57b8e51ab23c2f77c38cef0814df60592626" },
                { "sk", "e737794b03f90cdfce5e76b5d6ea14059675bcc996a2b030ccacbd4c071830d9a3a777545d74b6fffef67fc299d2391258bdf207d22acdd2d460fe657ab5d448" },
                { "sl", "fb6c68c330cf60af0cdc7f276b030230425e297ca5232cef21957a38a7bc7329175f094c873a382b3eb9d54e42ee1ee9ac6803162412d8a8c8bf4a6e69ad8253" },
                { "sq", "3937850c332af645f473bc8ef43ef6748e02eb8b4be67b294d34dd029f2153c0b388286b80231123293ee1b7f1e04cf00dffc759be86cc72cd78b506adf99595" },
                { "sr", "2799f1d653c7ecece2a2cf6ec30129d1136f1ab600cdb7dce176ec15639fe8c44445c0c49c91a0f3c4c627c6bb414541d9277f713f1cc5560c7a2af38c796e99" },
                { "sv-SE", "54ec2b187cf72840f753834d26ce75419188b86ae72e6fa4597fef97b05b918f47b39789144029d5d6254b1ad816e062201dacd9f5fa9874a16d9ce8922fb7fc" },
                { "th", "bd95bf68d88ad29bf80318fccdd02a7f1bd7e39ed48b8f1f3410e378227b20fe07726c99a845f38275ec233f664a9b11e59f1b7707b97f892a76417c0cb83e53" },
                { "tr", "a67a97e066bf3b6df6b4d8d2f86e25b0c5dcd08283575c3b322eef77e5bc37c3e53bcf81c97697c5ed682fce10bb4bfd325fa62db38f4fe1d723f0616830450b" },
                { "uk", "4c69b94ba5e67c085996a038864ed7df8fc2f6e923d2da97007c3b6853a76edf2fbf3f771277121f1fc0e968f82562ff72cace67cc8da77cef5a45f006d9dd8b" },
                { "uz", "70b106fa61ae322cbff5ac9cbfa2514a1625d3f125f2baeaae0127b10661ae92888aee0e65b55a48a50829b2c33131627b22fee0c900dfb20587c2499be6db9a" },
                { "vi", "c95de8387373a9569c2e777c9e5857025bdb4385ebbb758e44d32e68c4eae3e9c575eb10967f68cf9fc2427b63bcf1159e959030eccfac64d64503693ad45641" },
                { "zh-CN", "f95696e707e9e6f762a0b892dd337dc74ffa918c6206623cb9c209a09003b5ee3d0c40462cc3f584aa2eb4c6e5c158a2d9d356073fc0d8205a2afd4e136efcdb" },
                { "zh-TW", "ebe12ef13ad4b99247ff88ee88800805df8037008022f72a0fbf6660f1db15c4d96a42dfd02ea3313ae88910cd783bfc8fe9ca508f5315cfe944a543340f168c" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/115.9.0/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "6450398db72fbb08f0f96ab636ba9198450d0ba62ae87c24e648bfc303e0519dd0413c516c12fbdffff5febdff636d5231cbb42cd44e62005a12183619aa0988" },
                { "ar", "2731e92928a4aefbfe0210bf5fa685eddad1934d1d576249cda197a2ff9bf91a0a7669a078f941e41fd1a96a8094072a700c2a5906a605e12878a760a4793278" },
                { "ast", "5bcf24ff46ea3fefe0d9be17fd257b7963df32281208c0133f26669aae59a0ffe818c7338e3b3673a64e52f3b32e960b166e317584a956ad03197f873c7e1db8" },
                { "be", "6cbbd485d4f087c5acc788169b1a24c219188107e7dbfd1b9d13fb9e4f3a508f574001dd6ec8b5f04540633ab00df56eb57f3361c031e0d6bf56321c2afa80fc" },
                { "bg", "53a30a39099818620c7d205e62f0fe213a433f56fb350d11f3692fc247c6c116000602aa14cd26eb7cf6532895d30b357ebff3924dfb47ad86bacd12a931e60c" },
                { "br", "f0d28c51884f46eb95d81960eafff60881746b4515fbe7cccdd41cd128fdb559b2df8f0476a3cf6cbd44d95e46c2b13cb12e63202356c1ed04faf21e61211d74" },
                { "ca", "eafdbadae3813115bb87f58161077fbf28f8e80fdb40c0b84445e9a812a12e4e67dc2628c42815cb59a8d5ccd3c1103e29d8f620d2c8393862e13ed5b357db75" },
                { "cak", "c23b4781522e6049a0f31fc0040648f685709716c562350ca63cbd0d440b1ec65aa115aa31a828e49ad5bc36a3820f113a27b3ff8a7c7d154614277e76c67099" },
                { "cs", "72f4d64504a0c6bac0aeeb60e73e0df08bdedfed0a5c553894ed671a24f86dfb0a7585d7f75fe160f451cf6f9490818eb754b783ee02ab42592c4dc3aa39ea99" },
                { "cy", "3be43a410cb3d3ef1fd424c6cdd12710e6de0ee33bab6d5179a9b5bb3c2985baf4edcf5fbb11d0936a8dba20a1190a2075b4f3149e596eab69a506281f7f2667" },
                { "da", "0cba8f6490c5abd6cac58faa0c584bd0a24d68413a5229014f4da846a3215ef614ba9115afbf7206f1b00589018b4c4ca3e557e9131a0b78f0358feb638decf3" },
                { "de", "21fce1b0f8eff059e03b83440efa297417e73ac523a94dc9731d17addd77bf0b73867720fb700ea6eab6107c43216f7d863c8250f8c2f16a10cc597609575cee" },
                { "dsb", "c538b4815f7488f63f3880472621299cb3766d179c34ddd9603462aa3de448ec0e82db404a95dbdc783f4b51e59c436bcb9a905dfd1fbd8c96cb34a11fb046c7" },
                { "el", "2c3999038cd809b2328469230a269acd97b9713720f48d7f419f04a1bff679c335e188a69a92bafd697948e065d255ec40e11f48cdb48b9279c61aa9ea7bca93" },
                { "en-CA", "0d19085527ed44a3f96db0d1c7453d537da71541a06fa2474cbe2bddccf7206ffafc28152b43b3e661d2d63f0405be41e9201b311cb0f345cbb26d432b67e037" },
                { "en-GB", "7037aa9db833a0b619fb586a77dcc343cb906eba169152150a42f27011a6bad0c8e07890edbe5470b985bc5ba71c2ec9ff0becfe11b6b023e2788aac704d4451" },
                { "en-US", "06d7418768afb490915261f2ea322a1f18e3d51c15679d90e473bdee73b062f39d7a18bdb5eccd1536d8c1422930437ef9796faa8673c582b1d794a8429ce80c" },
                { "es-AR", "37fc1ce7311b49744a21ed4ce25cf0554e4f47b02cffb37b8c3877457457528b9df721922598764fbf93fab4a7e3c273bd89768d1a01ec57ce54331417006727" },
                { "es-ES", "86cab9312fd83dfef30d4b7e304622c33ae0514502e91984ed88a689878d815b2777826d40e678f8159f919762f13fc15901d3b08093647db8909b83f06df8b2" },
                { "es-MX", "01423155d9d59b3d59f2c0d1993b34d995c307547845e9aed8ee1896791efdc6f08bddb905a32e00add106b9af2269776e6b5347134b005a1649c9f20c7a0b02" },
                { "et", "7f5394423073004cc4380a8609d02f15fcc88b4d26dec29df083e85f4386c9a20e6e9fe3a7d9d62bc8087fb5e784ddbd96d7312de60af070739bf07f6cea6d7f" },
                { "eu", "968c9d3097992b2d599c39359e3c79192a07a5bd18e9f8c537838d1146543dec211e426b6eb35a30a50c17359ea3b0f5c4949368d1c61ed20c0f311b7b3fca66" },
                { "fi", "32aab78efb711708eb2d6ad1dff179aadd59461d4e85046238711e552f000fc7131699679372f2a17924c0e67a0a70c52ffadf7a8c092b138bc554fb43650477" },
                { "fr", "6ff88da1aa6de20ca58515ec6e143f412a41ab3287d32ce828497f87a29fa1e2a235ca449654aafdbe255e5052e0721bedf748180343877e52a7084bdb20cec6" },
                { "fy-NL", "81b2509d0b088f77affad72016a3f443a4da43804f02d54926b59b37598a4a064b6e31ced90dc2ddab394ce233423a0c17a72de21f18915ed45f0323c33172bb" },
                { "ga-IE", "45a2dfecc639262a69081fc91d92f6a49ab28fa1f3f19a4706e6632ff908ec23a162eba9d3c1fac2137bb81975d8c78905f6b59eee3be262dda16386fcfb2f74" },
                { "gd", "b17fcd5871db26b34bc9507a15468869a167cd0179eaae82e7e4ffa52a01ab8f4eeed9817f2c433fc8efb3acbc26aabf1eaea88dd322c5124172f56ca75803ca" },
                { "gl", "4229da792ee9ec135f5891f2b6b36a8f62b86fa7d3f48acbed3386f75c6cef9d42013a3d7d47e607f06511c35ef0839dbd7fe853986bab807229fd73ced98318" },
                { "he", "e01a1f450062988109105a8b2d6acbbff5ad0fa7964da7fa5acd74718888a647e7b08a2670327a1439cbe93ab8d3d677ae1c86f78754a8522e7d9788c6310c1e" },
                { "hr", "53e88f6e8d5677515c6e8ef4bc138bdc16b52451fc98fd39efff3118520457891493cc1bf66f4e165494d3ef97d0c6e48d72274d3341a45c5982b31f7e9fec31" },
                { "hsb", "ff7a715e8e2d41fad1b79034c6a220700dfc74fea0e248228ba4b7bc7e71a0a5b524c1c5501d41182674c9769d60166936803367dc968c8d63a8681cab73a0be" },
                { "hu", "b477dee94e882c8bd171169320db9df4c7695cabaf04ac3c7bf913ab046254f020194608d2db28e8bc79ed5e142f0c96591b7ff01af55807819b837fc142b875" },
                { "hy-AM", "4107eb1346f140f81879c551787b7c3b4871078985f824011b6577d3ad9131fc4fed17cbf05173f1be91199ec17a1d8536299606709f735ff1ef2d60d43b6a33" },
                { "id", "5d5f168f44508d8e292a23d11ee6efbbfdc19c64227052eb54d976d816db30127c04e364da6e87d7725af2e5f6b11b34627366dc56ea406f6ebb6fa48e5366dd" },
                { "is", "1b68a91aed6ea80c1dab0eb0ed2939c4804e12149ad5d95bccd574baf42c703b07f480e609d966735d849389163e356eb1b83054cdd9654481d7a86e1b444c58" },
                { "it", "c5746c08ce9a64f1a38e435ce71cde9ba55380b64148ca4ca6125ce814d2536acb0f72224e8c78a4020d880e0dd725746313574a4f7d7feeb79be1c4d90af298" },
                { "ja", "d01f6069ad61deb384e5dda712a32d975d428dd571049f32f4adcd0ccb32c723bbab8dfcb07f0fe5c05e80309db16e15262d6726c3bc93947e76ea0e3a31859b" },
                { "ka", "7537eba4013f7682b1c59cabbfbb8fdf49d2512a61bd59273e4f72bf0b16ee2c95d64ac1da33a25109040925ac8ea73db2df42884d41bb237e0a12393a9843a2" },
                { "kab", "027db7a8bebb63d0989eec784f84ac9b097fbd89fb662bacd4ed43447ad23955a8da735d08ddbd4142020fd697205d021fb4cc0c79b6a928078b4e4e2b8b59d6" },
                { "kk", "38c807aa1946523b1180b0592dc7d5da97f95ac0c339975aa0bb7883451496a05f73c4c734f746ed3fd15a4ad6e775f438c4ed5a657ba1dd000d8b799bf1c494" },
                { "ko", "8aa2531f2fa92b40bddf9f976b8b788348e45fd26c91ebc6828612e89dfcdf463549fdc382a3ec277eec75af009d795cf5548a866dec559e96221bdc5737cd23" },
                { "lt", "f0f1d296de7d8dc5e788085fce4c925b3cfc593827ab970a49c356392c5a4f605ab431fbd380b4638ce06f5a0760d9743c5d31c92c1e192def6b8e8ad1cede83" },
                { "lv", "fc67f35795c0d77f8292018de16e75cae710843b40ca8a4e03e7323e22a66c933f8a973c67468f70de58a0eef463207388c279bb5c453e442bfcf6afb7c7cce5" },
                { "ms", "bc6b1be4801ad9899e54e781fb8c00e5ffe5894d6d296739901b0efd8729a4e97ce69ee57658e361d3e9c99a4496faf4d5cf92637b24c009a78f373bd6e61c17" },
                { "nb-NO", "d064279cac03da13c0f99a4d32f48b08a8d30debff57a611a0ccc097e088df2d7c425ce899f003a40fc2a692bfff2336950259ec8579dbae54788ee80a563a24" },
                { "nl", "a7e56553e52fb39824636be19dc928ef5363eaaca980e381fd0d1eb6d39a090e5d9b46744880f4e5089006a12d0481a6b2d6e40113bcab811f2bfa7d260b853f" },
                { "nn-NO", "a3bb0bdc8cd2e124076a3e86f4f3836b581a8638d1067aade203b9b1afc3d3516e48f3813b28a1f978f96395deed0f7bdc8ab6f513f351e2ed1099e6f4fa6c17" },
                { "pa-IN", "3dc513522f18135550cafcc9740d74062af39e627c9299cc16e6cf1e60a26b13201204899e0f11fef5034ea5bd55f7153185d7fbea389bd36919a1d146d1d958" },
                { "pl", "044d1fc55803a660b1f85bc9d6369384f7748a74b5ec351a7f5f719f6e8766040b4b497951cffe51f31e9749ab46e3ac45c213c5745600ed2cd333da339533bd" },
                { "pt-BR", "7d6af3371b3c7d3787201e9b9a1ecc62881385a43f5bb55cf898bfe3e4e3f2f18ac5aa07ef66e1a7546fcc5d817250c1fc91e4ffcf2a567ea86242e51ba63b61" },
                { "pt-PT", "d102cb67de3dcd497a2b74ac79a19ad813941ea9bb46298fd8c3d294bafb7e562e81cfbb08b706cb2b5abbb1e1801a8dfcee6e25d3628fe79c1457743923acb1" },
                { "rm", "31f25d8ce07af393b7ce76fe9d390f40e859924c4b83a864fadee95e303c229d8b476e8658298c3ec858f4ac68023ed6bee9bd41d59350b52677558cfabe5c99" },
                { "ro", "b5fc3392d7d7466486570512149fb9145b19cf24ac9d0176e4cca75a79f6c4a33be374965ae18aa7b28a3ab03b079f48dcd7dbcb93263ba01e66df99c01c177a" },
                { "ru", "ec65e66ecb4ee9a799d9142422ed2e7441238f52cd9e7c6e8b7617ccdba9fcb69abf10af027a17e81d3a6b93f8cc2db530d8c92f9585ca6fdfab313daf801903" },
                { "sk", "104d85a5438ba4b38e4c363aab4c4bd08bf2ba656a77efbef61cb5f18478bff97d83c0d29ff85a3e2648594234e0ebccd85956e7ee17841418ae5c2fbdaa02eb" },
                { "sl", "065ffae509f5eb6936072bcbf356e2a4de9549f55ed43ea24e960f7873f56722ba79170ea2ac1a8bb4dd96ab0fd84d6c2727af4d98198772a5eeb7eb5be75abd" },
                { "sq", "414c8dc66050b43b6129774b4e5534a5d2c4c21a568197a009a2da278c9229aba46da6f2416126a9c598b4e56d81a6607ee96c42adc299debabbc76c9dc9a6d6" },
                { "sr", "fdd5009f15092905a326f83105bd68035b90764bedd81db4cd9918f1d76a10d9058aa7b62b7d1737342622ad0ad5f54711ef1873832bec46278b307d274d651b" },
                { "sv-SE", "b90664e8c8be17e9691e412726e4a063f532eb603bab2ae82d872b66f175cb27046e0d3cabd1022061a868541e5262e8a10a834fbc24d96e6028f56aca02cedb" },
                { "th", "dff0497e3e60e38906c5332289242e53d58474908999e684350c8c079d9ee6b6d7dae4720031282813dd97203fbcb93c215962cb14bbf02c151542c65ab6b7cf" },
                { "tr", "4b10fdc50bc56c316d14a20d126d81ba0bde72f640f3b249333074e542a308085cb6e9763ddbeb53fd09641aa173987415e19ed1a48f2694c285fcb934568197" },
                { "uk", "92931689272d05bdcb47bff5500e981f1ec88987c7f2206ecea68f79c15cfcf4e30dcece9afcfd13dcc757d3d362d4c34b1f3cbf8e54a2c74e43e566992c7b15" },
                { "uz", "f4cd3f76ed427d21d22459bb63852aedb86fcb5d5392d8fb829d61018b67347d8cb8fa2559e9343ae988f49d5280f461474b3146b3a4c5b28c2ee7842b152da1" },
                { "vi", "f4ec8013025f38a6fe2258419dda4775aadc2eee826e5c9a070abcbf21c89c791ee293c707f016788f8e2e197f5cd0093f9d0925ced7189a3705baeaa4074575" },
                { "zh-CN", "c96b0beb4b247c240a87e1792440c1d1d6a40aca1bdcaef3dbd021fdc611bf8058832e4b80b0abb005e125a7315dc7f0cd75fd335bb5badc3f7cbf1b780de3f9" },
                { "zh-TW", "e7ebfeec7b349a16b95e1922a6e862e31fb9c44ee9a45cb2c3cab74a105efb6258900a63a2df1ff73c49a002a59cf43b346c9ec1a02e490561e88304fb7cea3b" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            var d = knownChecksums32Bit();
            return d.Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            var signature = new Signature(publisherX509, certificateExpiration);
            const string version = "115.9.0";
            return new AvailableSoftware("Mozilla Thunderbird (" + languageCode + ")",
                version,
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win32/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win64/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma"));
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "thunderbird-" + languageCode.ToLower(), "thunderbird" };
        }


        /// <summary>
        /// Tries to find the newest version number of Thunderbird.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=thunderbird-latest&os=win&lang=" + languageCode;
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };
            var client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
            try
            {
                var task = client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
                task.Wait();
                var response = task.Result;
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers.Location?.ToString();
                response = null;
                task = null;
                var reVersion = new Regex("[0-9]+\\.[0-9]+(\\.[0-9]+)?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;
                
                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Thunderbird version: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// Tries to get the checksum of the newer version.
        /// </summary>
        /// <returns>Returns a string containing the checksum, if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/thunderbird/releases/78.7.1/SHA512SUMS
             * Common lines look like
             * "69d11924...7eff  win32/en-GB/Thunderbird Setup 45.7.1.exe"
             * for the 32 bit installer, and like
             * "1428e70c...fb3c  win64/en-GB/Thunderbird Setup 78.7.1.exe"
             * for the 64 bit installer.
             */

            string url = "https://ftp.mozilla.org/pub/thunderbird/releases/" + newerVersion + "/SHA512SUMS";
            string sha512SumsContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                sha512SumsContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Exception occurred while checking for newer version of Thunderbird: " + ex.Message);
                return null;
            }
            // look for line with the correct language code and version
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // Checksums are the first 128 characters of each match.
            return new string[2] {
                matchChecksum32Bit.Value[..128],
                matchChecksum64Bit.Value[..128]
            };
        }


        /// <summary>
        /// Indicates whether or not the method searchForNewer() is implemented.
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
            logger.Info("Searching for newer version of Thunderbird (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            var currentInfo = knownInfo();
            var newTriple = new versions.Triple(newerVersion);
            var currentTriple = new versions.Triple(currentInfo.newestVersion);
            if (newerVersion == currentInfo.newestVersion || newTriple < currentTriple)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if (null == newerChecksums || newerChecksums.Length != 2
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
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
        /// the application cannot be updated while it is running.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a list of process names that block the upgrade.</returns>
        public override List<string> blockerProcesses(DetectedSoftware detected)
        {
            return new List<string>(1)
            {
                "thunderbird"
            };
        }


        /// <summary>
        /// Determines whether or not a separate process must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns true, if a separate process returned by
        /// preUpdateProcess() needs to run in preparation of the update.
        /// Returns false, if not. Calling preUpdateProcess() may throw an
        /// exception in the later case.</returns>
        public override bool needsPreUpdateProcess(DetectedSoftware detected)
        {
            return true;
        }


        /// <summary>
        /// Returns a process that must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a Process ready to start that should be run before
        /// the update. May return null or may throw, if needsPreUpdateProcess()
        /// returned false.</returns>
        public override List<Process> preUpdateProcess(DetectedSoftware detected)
        {
            if (string.IsNullOrWhiteSpace(detected.installPath))
                return null;
            var processes = new List<Process>();
            // Uninstall previous version to avoid having two Thunderbird entries in control panel.
            var proc = new Process();
            proc.StartInfo.FileName = Path.Combine(detected.installPath, "uninstall", "helper.exe");
            proc.StartInfo.Arguments = "/SILENT";
            processes.Add(proc);
            return processes;
        }


        /// <summary>
        /// language code for the Thunderbird version
        /// </summary>
        private readonly string languageCode;


        /// <summary>
        /// checksum for the 32 bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private readonly string checksum64Bit;
    } // class
} // namespace
