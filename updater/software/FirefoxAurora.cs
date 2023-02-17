﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022, 2023  Dirk Stolle

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
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(FirefoxAurora).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "111.0b2";

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
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var validCodes = validLanguageCodes();
            if (!validCodes.Contains(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
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
            // https://ftp.mozilla.org/pub/devedition/releases/111.0b2/SHA512SUMS
            return new Dictionary<string, string>(99)
            {
                { "ach", "b5d8bafcb5f72679fe04af170a96c0e67b0beadd368c3badb68917349bb2cc7d4a7d1700ac70cf6ff249fed997ca7875fd3cb335ca0474fea683c43567489c7c" },
                { "af", "e7d71c211989e7798f126b671ab8eacd7bed1f984a77a134b23f1bece7e89e10042eaac4437843ac2b04602d0440aac726fbe14182855da17606373dfa95b7ea" },
                { "an", "5836afff1fd12dff05f0e787211e6fcc106e653a5e4ea5bf53b440bf24b6e26aabc0153d4800d5966a8d219d6fd4051a8cf78531088df5c3c5c992b00a940e15" },
                { "ar", "c53fbf523181848c907b80894259a41807716a7d0bfc1cdefdf0b87341f56b47333634ad0209d3a2776867865d96490b8918c31bd5aba06a6f0cfd2d45c32348" },
                { "ast", "6a8e10cd2202a2421a0303080270e475fca9486e8ef1727e9096ea4f8878545261021d130a72abc3dda36049dc5d1b6fd822d30dd530cb0475ad58cd2869babf" },
                { "az", "1648e575bb8b9232719771cbd4d156d35734942034cc50b6190d9d4adfcc9cf66d8f819ea6465f40af408245f838ffd4202ba2b73569a036e487e34f179df10c" },
                { "be", "641fa9c1f2465163d75ea41ac8f922a1afd7bbbe34b0223ea39fe8c26061ab553170e682493cbe98ec7b51e06da9d0f38a08631a225e30e6d8abfa0760d76904" },
                { "bg", "4b70ec7c24b103e6a7a3ec2be9a51aaa212afd7ee80f27c047783bbada93ca0d0ad639bb7fd7d513832ca120db0df0918156ba8887bcb92d553c4a2b4cfbec70" },
                { "bn", "e87393fa98635953299412c4e879fec811f38e1f72d30c34209fe90ae2e10759300f0a9d88cefa71a331486ac740b44566d32462efadeca04b517c473fb985d0" },
                { "br", "2b4cd675b96ddc234375d6298b9dde619d66a42d085cf0b32e3562d4c01090fba684a8454cc95346adce45e44b1fa233979b2dc909e218d271d280a5180c17d2" },
                { "bs", "60d118c90d8c7c17980601ffc08801e6313e0ad899292ea3ef47bf51a85cd162b484bf7ea54fbd2c82cef3addcb24ff7168f3f283e687336ced3e3eacd9770a8" },
                { "ca", "6f8f5c38e26ccdec3917d237672231c85ccdb8003e48cd99128a1022c69f015fb1bd7ab0608bf8c1902f4eb0fbc35ae323d96cf7e21c268cba76b2c8852a326f" },
                { "cak", "ca9589e44555d93a6bda169f3c3ef39c750f0e3d50e3cdfb751b28992371798465da0853dcc858d902e07c645e2390b1df029e28da99b1b546b1bd99180a1bdf" },
                { "cs", "050829c4b15bb359d04db4909c96cea8ec0045abb5fdef6c4f36bb856aa9dc1bf7aba5396611785454627232e2f5c34e39c2b0b69fa2a9ca94596e3c856141c2" },
                { "cy", "ad2a6810bbe621bd5430b16dbc51441f4f6d7fc28d800d0393c23df23f1ff5a7ce5154fb851a9f361a78f36113f7116ccc3ae322e91c67ef356eeb158915b84f" },
                { "da", "061e0f9f271008ab9924cf145d9e90451e970d82c372ca3a1e4720ebec909d3ef33cdeface122c25ab03bcba805424e15b40f3ebcd5802ef14a11b04c04663c9" },
                { "de", "0bbf53423d4e22a4b3c0dba1f8a9f626ceaaab57fa2be788d52a57e26971b7b97c1e7d738b238ca2237887adce8f9a88d08fb25a669ab1e8f245c80e097ee9d4" },
                { "dsb", "05432c3cfe5ba885dd40fd4c66307a9a0d0ddb5e1529a68abd22119ddc7b310f9a884047132230e7e0ac2b513e558fcfffc4c800cda67a4fb42235a4aa3580fa" },
                { "el", "f717332de02191420831f936a957ca50b8258891893997cc918888baab50a34a9efdaca7e964e601b223845e8d65a025c24adebb6edd0af83bebec0261edfc1f" },
                { "en-CA", "f5944f90e2f56cfe9844eee36610564884b773e2b6f7246d6426c415ec7206075c9679e067d0dbb5357bcfabaacc3d1bcc78ddd83f9d15b4f51587e41b142c11" },
                { "en-GB", "05d9c25f872801a5a67c082bba3197522341e6ed5595b28517011d6d742baae51cb2f773ee938bb27640fe2615098937ff5fe0890bfd71cddcc92a1b4cbd49d1" },
                { "en-US", "0989cea6b4dad57c71779edbf695e997f77d3ccfa446cb220d34586f1c924d43678dd6974fba4dcfd82bd092b8e8ed89e06f78d4f5f01aa471f75572188cb5ae" },
                { "eo", "a6b7a17e9ca66b1b1198975a989ffb51e997766b08af3c3dce3d3bbd71cc6ecd42b62ca88d5b5e684c5be9d5bbdc1c0eb5327b412d69b5123355786b656a3f76" },
                { "es-AR", "37d1001c85b309412947832fa1d2e5e14d61ffe28fa94c130efc785cefb6de2d2a87ddbee22f9a1507010d64b298dc36f258898534fdba699531bb4ae652a5aa" },
                { "es-CL", "95ff541074ed0733019a1c3b0655e639baeea19d31db0a9ee05f7417c8c29de08dede1bbaac603730f24e51ba40f25235f753254d7b253373f6298f663e33083" },
                { "es-ES", "bc6ba1ee326864d994c55e6a3a1a7092b0672d997da9846f91c10854c6e20d1acca6184269dfa3321eb24a759b322b4635f44b84b7b8e1004c519d7e2ae137b4" },
                { "es-MX", "1d7b56887f4517ae627b6c305e6559bffcf2901ab35da56c5ef90ca9c5e47269077ec058826a3a7cba01f35c20e76bfd30534c363bd2280f2a324a469cd8835d" },
                { "et", "a6da34ee9dc5136f861a4e353a9152cf7aaf46d6b5b1df30d6265759466f7346fb5ec39947e14576b5caa16c2aded0d69f7a8f5f50e08b49e14b6c65e00fb189" },
                { "eu", "cc781c128fb985c26087e372cc3e7465a15e264a0917ab6c8595386f6e0bfa8a806fbc9cab1d4f13685f7050cea37d0f0c07acf91eb93383dc9009b6d7a0b760" },
                { "fa", "2a4f0c34cbcadc620064b7f3c44c3e3bcbaa31c53ef89fa30e739ec2485d01b93c2a54117b5fbf3f3268ed6a75dfe0ac6c17bf43ff68390fe852df57dac62459" },
                { "ff", "c9059007da0467d8d32a51411c5c25e62d246af496118b6299cfaf9c8754d6cfe8097c22ea6a708b7437316674360b9692f50540829b9d01a0c0537b55a06467" },
                { "fi", "296ef7ac18cc5f819bf2960ba785bc9a9fbd84a6a06f8ccc59fef744f971da6a787ef49fb1b401278e78acc0affe37ddff88b294bd606fd2bcbcb98b8d2fd9c7" },
                { "fr", "f9dd97cac94298cc2e30135458b05339075c884170181952afad7768fc9e4c07089fde93e30cb64bd2553b86e05e77728f1ab5c0ec9ed91fccd3e62cfa51b410" },
                { "fur", "2dba66ab85062da04bfde42aac24fe4d1e71db61553a1a8094a8988bcdcdbe527ddb140bffb3dfe6a08fcc837003c2b207764db56017f1eaf608c4f35414c5cc" },
                { "fy-NL", "8c2176b5ed4ddc874f7fa6b647542bcf20e2e1107d74d15df73866873e895ad25932fa41059cd4ad01efcbfa0e96e1c0449aad60e953a65438633a73c0c72c59" },
                { "ga-IE", "2bd704c5cbfb4e73d1085a8769e471937c021e7a1626688d6162d8fa531b101aee74fb0a8f7ec5d80a6b4a99944923f8d055125bce4fc42f2e2ad29aef4e8084" },
                { "gd", "3a01b24661ed876160b574082588d1e8585f29c6e0714b78a258f312cc88bc096db7fa3754be66127aaedd089045474d4d77b04db54f015cd001d63da74b0d6d" },
                { "gl", "2552295f709a20bf6476d3e1de5592f1d2e002a5649b843eab1279b81194f023e9f88599467436cb5808352a7114ae6492e01f0b2354fc767a57dbeafb3a52b4" },
                { "gn", "49386fe0013723c57dd9a32a437df37f13ffa1a3aded36922fa737289378703b153e6767c64879118a3c0a654becf1faa9cb8fed8c654c2bd29ee91c9a6e241a" },
                { "gu-IN", "cc808e668d229275d8b9ebee856a0b00f3d8879d8a37553d228c9556ea649946dbb1275a1c571dc6dadedfe8a96ac808e0de5856b04141cc026f0c5db095dddf" },
                { "he", "46b6e7645bf78bd0f9d5bded1ffb50c7f2e7337616e0480e252a6f96b842c9e1c65b00b213f8995defa8b7e2f00a31fea7de07efb8479b97b4c6701da8a6a1f0" },
                { "hi-IN", "9dd6fc7730b9a8c5b3dba57de3f30172ad61d2af91ac07f3b80b5dba97de10a9ce4ac786eb2e167a278c8fcdc3f2aee616138202895bd74b89400943775ba1bb" },
                { "hr", "ae10804ad30699833d56cbad48ade40ecc4066d0ebf5fef71a201e519245e8decde272ce8250197160115ee07af35be8805b6a9d254e3e1708f48345396b7cb2" },
                { "hsb", "e587fd38598cd4d59091474a9738124f92e48d9a27f1eb93b2f958975fb0e742d1abab0d2605435d99cad899b83e2dc892ea488f1fae7ab5e457f2e4b2051300" },
                { "hu", "37109641f654edabcdc1de78ea91660bea1713ff63fe488c0d53c3fb0011586fe7a8713ae54cc900eca83ef7e3d0bce41c28e16a2993e017303009ba215ba3b8" },
                { "hy-AM", "b8bc8a26f5fb44471ac90eb1fc2727516b92ff3b058ae2699142440048a8237bb29bf5b70a29e2840f661077d0aa82af7fcf9b876c00cf8eb7ac60ecf009d054" },
                { "ia", "985b05791089b98debb6129bc7d695f854bf776b5677a6b67e6da80496aa7e868d0018d0cf061f77e0e2b87e13f82ec07c91bc12ce74294a37bd2830f9c457bf" },
                { "id", "67b7f4efb45843067b399138647e3813817816050bc5f1dd8737a7d8286cf81f65a34e2cf94c6d36c04cb9b4b39452006f863f792105f07c4f3399dc06370b5b" },
                { "is", "f83e33bb5ad69057e3d9bf1a7b7cb95e52d6e69656aaacb1396374c53f8444addd8c5f1ec0ef2f484b44c25549f44ba58688992b5d65e06d85bf3af54a1f72c1" },
                { "it", "0f7b81778b75f6e9b029fd2f26b27cfa11b5446b09666ea0331cbc368ca77541a0a88b75b003ab61e9040b9d384f681f61a57d204886af91a42b3a1aed079199" },
                { "ja", "3d742b98e16099389022402354cba2706cf9dff912c4f7bf72f627b82a2c87a9aa37ac53f440cf089004f10f583077b6733cb54b5c8d7c0fc32fae8546f70277" },
                { "ka", "6c2303511c1e7d37f6a086fd5cce758ca5b4ae3820015a777cf1321d79c361b628f65229a646b442819dd37466ed70e8b4538aeb1fef56c7e1dfced5b4eeff20" },
                { "kab", "826f344d5da006338a005299cd4fe115931c02b0f47cc702f3573b9f82b874353c6478f5976f954f4a84ad468048b0f3dc26de8f90f7751e7396a9ab7943878a" },
                { "kk", "17e399afc6c2f860ce0c77d321578ff0cbb92946ce18058a2bd2cc9082f94cc141b1b67b68048649d8e52923a63c4303398a8d066d6ddc8f277085ef9adba1ef" },
                { "km", "5839df375bac65ac26bbb8d14f95619de3b5f55dd388a7beecf257932433bcc56bdd341a68a145833867adf46073c1220b753ca8be5c8806c93a0ac2efbc12bb" },
                { "kn", "e4c221cc3486ee78c55552f0badb21ecedbcc5c34b75e8b81987f84c932520e694d66074f8f9d244419fbc1686b266c1042cd349ccbcbfa8b18d231f7dfa0c3e" },
                { "ko", "76096faa3552eba462e23e6f60b6fe63ba8a548ff96319575e36632e95627d5e47ad1be7812a6cd628268ffbd3fade294b8d9590c323290ebe50c66ef2a019e8" },
                { "lij", "795b80c356823853f74c51089dd0adbdc99547f0f24134fee9762b8dae1a9e98fa4905596a6f3b3194cd3345aa3735d4241f1c4d549b48601468843db4ad3f7a" },
                { "lt", "ed32f0fa8ae8905d20909b618770778e33c04e71c5ead7d7d6d0f110cdd98c5295890fa625987ca710c6f5dbe379b1b6f89f08e86b1867a81e1e293d41929832" },
                { "lv", "1c370ace0222b10c3d14e0650b8219cbdc3ae2cbb8b48408427e17649927009b2f660365af07bcfb9144051309552e5c37cf6e23c188e7eafb117dfdfd4f8058" },
                { "mk", "9d20949e0191722bc6c00d4abaa598916d21e6bb742c30aefa9743fd16b6d3987e7ae38c455502548e0f1356a34238226dd481ca9deb982b37ce5160d4453899" },
                { "mr", "500dece48ab6c8314f0a66e0aa28c1f3f2b7a0206cd627f82ded4257d30d2bcd6606d957d1e3d067b396b1794fd6ad21ba8a24f6c9d463c8f95eacef455385d3" },
                { "ms", "7ee87ff004e90d6886cca0d1e8d670c62d6dd888607c0921346b5931f2a83a64338a06450b597f3bd147d013ea923660b159ccd166d210e888d44b9afc84fb7f" },
                { "my", "7104ae4f9bef3f3b2ea259819bee55d988715aa87e56c12fb4decd37fdb351c79644ad7557b2a46e7d61c73f8761c32edf1498c00c6c5cf99f525cdb4a9001e4" },
                { "nb-NO", "ca877fdfe2bcf80a3459210ee1c8fc3a9b3c57209d885cbcd394a8e71d2fe8709a75ede7482663cc1f166236606c83d41345e59232a8cb8be006d3d6426a6ebb" },
                { "ne-NP", "4cd871c77e33d6b0b64a9e6f91fec0df40262d57afeb83276863601782f2723ff4f02aa03e707354a21636f280e6d0ff22700fe3bcbc83cf1fcf3e4328336dcc" },
                { "nl", "c99a18086f4f538544c65751a3d6f4771a28390d42d75b269d0f84ab7edfde264cccd40811986afdebcbf1e3e619ca415999e71ca69852080da337c18aa1e303" },
                { "nn-NO", "93b6b4c9692a5b5bb4498dff0d2fd256263d623d0b8fd9277bb0f6fe4db7698dacc074e5b5c28f19324a1aeffb6b3ece98ac36eb7b89c3c447e7860c90445d1b" },
                { "oc", "23f749fe93d2f63962433e17969bf7b77c23861c824bf057b6b4b9cd23633ddf134f94dc96683c999c195b10dfe8d0ae4e625d87a344331dee9408f87fe8e43b" },
                { "pa-IN", "3a3f381b702a771db6adf9591fd8d7a9bcc108758cefb6a2cb0024b4f7e43793166453d03b7a8e452a2fb7ff47caa333f2d0dfddfcc365e3519933dc34d5f25c" },
                { "pl", "1113c24d4e645e4e9d62af5e243ffe789d146aca3637f3e5b73a89024b0bf76e6ae4fb7ab24b1bca92cee2f2c40c5db0f39997552f7d3d9846bfd801f4ff20df" },
                { "pt-BR", "0b501a5de2bab8cd9cad1dc359c61057bc02317c4a9038886650a1f4b69fb888b86a2e9bc42e71b251bd68604e8018dcc852dd2b3f840c2318c04c34f51e9d25" },
                { "pt-PT", "f3fb348c1e0a6cc91edb7d56573200afd68c88b5b613007d3381e5a3f3f55f4c1c669f41191d367442d70585ab41bea7713a48b384f223a83628c56440c873ae" },
                { "rm", "b77030637e954161e6857d6a990fd042fcda9f7e0fe35cc983c4f89953b2c1d799c5191db57b44b9f61c921ca6d1fa2654ce9e03431d65e9c9171b1df58faecb" },
                { "ro", "1c808a7d447d71778d80a0fee23cfa24a56f019e0fd6b84e1c5368ca84e62ed1947264e7214c173f111f6d60d98fa463a9c55c6bd6ad2f69d1c91c701af1baa0" },
                { "ru", "7969acb1e30a2825a943f28a2577846fb61264073baf727701eb7f63a54817bc20c7bc22e256c6e27ddc77070aa082f945d8736313526a900a2508330705cea3" },
                { "sc", "44d81802c225e4559ade15eed7474f3ae32ba234d1609e33475355c9d30d404d593a6c90b00f808b2a17cadb4d471ec24b1348ce9607c7c6e8459ddf2d62ec42" },
                { "sco", "5be0e9b51ee203e07356e0dfb2f8cff223d046b265d8ac2b1fcdc8d859b6b8b54885fda1d6c798d5a482c4835544fbb7da027f3ce1e5566490bf03a511490db2" },
                { "si", "8e5319d1c7645fca8e9ba7ffc2a53401dde06fe4be1ca25370b6118146fffa23a3ca11c10df7c4bb67023016e4392b841a0d45fadb302293a23b4115892c20f8" },
                { "sk", "6ab51845cb69b6a718f823f98fe50686f2bbb36553096825e166e2882770db77389c8015b026080cc100a9909f5b972da13910f77a5bca4cbd7e82be6d75b322" },
                { "sl", "1a8700a870d652f783e8a8609060c8153317d41a73963ed609956fb9e6e91e38134f2f46ab94094867ec5956f604b6eced81caa49aa4312c521b95b935e36a27" },
                { "son", "79f941893e2aef9b8ba6b264ef9440c9706b718dc248bf2d45d1a616db037ef24d46d50e9fd7a40c2f9eb739a9df165b4f20fe678eaf60a3d6ac8f86ed398d7b" },
                { "sq", "b1de6e8cf3744e897e0ff735663018feb808028ee5986cd4be7213b754f18b5986be7a732b8d051db01cabe28a11441a7a77f796b6f404191c8fa7f95e8e2f02" },
                { "sr", "d11feb72197a851872eb6dd1810d5fc821f4e257637fe534d313f56718ecd5ae9b42f7db9637be7a71d5bf54b8eda0bb6c6cefce01ef8c7f0e6f545042b8f3e7" },
                { "sv-SE", "f37269ae795fa2c9c8189234fb155f9c206636fa91c461878ab0a47cb55fe292f4675881de0002daa7aa371d140b7533974b98ead54b84f297bd669fe0597c3f" },
                { "szl", "ebb552177644bd8bc8da6c54cde869c0ede68ef445db7c8718d22cd82561fb199b4f93d720bb09be14f2bc992dfc69c4eec9e7b71442b335abf7df72163cdc37" },
                { "ta", "d56d3b6d490e0ebdde0b979d4cc58ff78b3ad029fe48d5c89c9f938336e6fe22d48501d9ec0f3f12bedefa01f413c5b8c064c7957a0f4633f280166e6e384747" },
                { "te", "183e7f1d686fa6c298521c259f15955a69c24c9306f4efad28ea252033e8996795fa04ad4c3b1a0e3e3fd89a76f0bd0a43d10f39c83a5b07d4a3b3eab9a4ad34" },
                { "th", "5f481b3e34e854912dcd93c800ade46876bbaddf5fd02d1ec86c48a3ea908ad31b01b822dc86849efb46785e8ac0b4aac1f25855c27d89c2af8df3662dd56f5d" },
                { "tl", "7ceb70fc55ab048c038080780d4391770cf806d05e5d3e41910c2b4e046c02483c046a9bfb592406533d25389106bd831c9d741dcb6605a74ad3279739528142" },
                { "tr", "6552acaf6875c5f360316d555e8b011370e40e75e10de59e2f717dca48030842e78657ebc8aa9645951204eee0f59814255d726ee0edc9c8f601965a8134dc4e" },
                { "trs", "27bdbf0869255046043416a98ffc50e6797c0c4b0f534161b5b81c9506ecf83952e15122215bba270e331b9f3d83a465219a16e468d612b60cc18660a4e3c981" },
                { "uk", "275d881422355baa8538a299615429f11c9d9858cd794265107bf89838480ee603c1ca6f975960800859257ce39cf6edd2f4cbaeffa7db1983d0190f9b77fad7" },
                { "ur", "9089e4f87c2dd09bfd8f924d956d8e2aa4664af42f33ed0d0c837d91d1aa43281c5dc7f2c8ed5f5234925228d2d3efa2638287ed837e20ae3c0c102a515fb037" },
                { "uz", "7fd9881bf00ba9e2e69ed9f515ea2c6197c777414cb2306ad19516f8280d40772eaea78b16b296ff9cb0a9eefd7546aefe73b1998f891a949a11d122f0beb5f0" },
                { "vi", "49c9e0fc3a76237ee3fc58ef607460e236da431281f76dee404d6646afc5a03aeb590e9ab2db9db77b7546f1f81366cc16e8fb3ed1c4f979a6ca3206b7f93ef9" },
                { "xh", "68c4d8b329233d530cdb3ec1a504816b35ff2dd8b68da60c9e693a4a6d112c22069b6d6e842f11abf7f81982d626279dbe799d626ac51e58573ec160c8d3e209" },
                { "zh-CN", "bcffd24d9e7018d39e871bff7db4404b882fb8ee67f011ae624326d8e7c63783893ff1a2d7a94d879c1b4e7295d0ffe38eec1356bb2792a4382898602ac66ad9" },
                { "zh-TW", "b5a5760bb4183bffe38163d75d71c2516032da20d31c3723a6b744bc237a55f8260c92c82549fba955d4828bb8da73e08a741713a6aeefe4c5d43e318ec79aa8" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/111.0b2/SHA512SUMS
            return new Dictionary<string, string>(99)
            {
                { "ach", "783e2fbf5266254fe005cc152759524fc4ddf6c312af4d92d84d2df496a2693c34ade4da03421a57cf08f1f74b0e41458f1d304cff5409e51652da89df95c8a8" },
                { "af", "328f123294b6fec451d886ee9e65c8b488ae3af69d8537dd3ea28c88450e330a35c7f6223161b3ac26bacf90335b1806ee975408556016536c0fdc18a1febbab" },
                { "an", "970eb6fe6061bbab2c4b6a26ca48df90b251f4f2349b58e61e2015a3a504fe224437a92bd7fae4b6769857910611ef45eb96c7de03d746d1b4d608c88de5a883" },
                { "ar", "ea8ef86eae68a4732767499e51625165d477116adb6da802926ff4b78cf1fa1a5fe188de9e32c54bb2284552b123bca9070798b9ff3578e99248195aa3f4ec0e" },
                { "ast", "b6bbc971f1349a5d65aa639857b73599855334828878eef35b7961629c988240987303c6731979cc3b3073c609d9eef4927234b8b6cceb5bfea87c3646c08637" },
                { "az", "91f60804b1b0344fcfb6b00a5a7144863e339a203fffd9ea0f5e7e1bc4e4ea01d83a1f529657b262f8f7ddf8c3827560e5e86b00fc85ae965e02c0cf7ac357dc" },
                { "be", "b551f62e40b3eb9d98268d066fb775d4cbae0abf8afc37f75f9eca35686cfb8d7b4af05d29c259e79ccaa54859046de3d78dbfee1a639c731909d078e8cbb57d" },
                { "bg", "30aec31819b5a6cbaa91df579e85f9f1ed1efa51d6d946fdc22e4a5fd923db90623ccce8e3751d76e1b933f0aa1ca3ccfac57c745c30e84feda6e4815d653423" },
                { "bn", "241566709fe80a85d5de26000251b06b539f04f41f569b1c1145f1ce02419a509172bb320b3d4f70efdb62f134bd4e500c438822780ff8889af37517b43569f0" },
                { "br", "6e14a5eff3fea705c05a08019cc7224a3fd616cc8e17b26d635a933b5ae4d5aff30d3523bd5d6e21dc7bb44494ae9423174ff238d6ee3da9091245cd37438d1c" },
                { "bs", "e3d22dd00a61b588b534b215e20602a16410d4ec8eb9eaf27ea5ff40e542bb8ab43b7c9a5128ff7db5eaf05fff547733ff19b6623c384f12a27246702e74d424" },
                { "ca", "3cdf4600133ef8d839003b2925dd3554ca81d4bafd9f328b68143038562c6a9ab8baf28757e164cc04669c2f1987b100fc968da193cda4e841d8547d0ecae00a" },
                { "cak", "160fece4114de006f3351a43e70a4a87c5c2f6664dfa3f5f01d06a1fad9f7588912c084fbbdabb0bbd5f9ee707f623a87bd71d0412e7f504a86eaff8cad476a8" },
                { "cs", "41e625e36dd9a148e27269ecddb88437e9d3d085bb90e702e933fa50cc783176b01a7cf8843527f0e92ff938afafc7814fbfd870f0893bcb7136b21419f3a86e" },
                { "cy", "915a24d650c09aeeff4e8bb1a419bd958a164dd3d887dc2016ef29593979f876474ad3931108dbe7ec62e39634e2b464c093bf79f746e749555fd4578dee2f56" },
                { "da", "bcdf29486f37d3b8a389c42e3df6834f6c6fe9d1553a52dae8743f66c27069180e3590214193c0a8d421732295c7eb3c9286298da19e2fb8ee25353f0c5f08e2" },
                { "de", "2dc9095764398bb7184e8274f72bfcf72b4e859c60e63afcf03b6f7decda35420ca59ed3ce63beaac2c517a60e5cce827db199cec8d6f360ef59ef004e0122bf" },
                { "dsb", "43fe61d60222882dc5491fdc3b6bd94472a1d6e1916236f578404fb15a95bbb72edfaa1247c71351e244e4fbd62c60ceb5054d94ac2b81225821c5294fda6e2c" },
                { "el", "3afcfd6afdb2ee265bbcf8d747d4d86ea5f6771a09573007479f6accab4baa2e10cd27a022dd2dfcfcda6df69347a7d8463b4602635497066dab8bd3f7848f90" },
                { "en-CA", "381907477dd081aaa93fc8cd63c7b14d20ca15a0605d39eb459607c8bbe1d8e4e56181d3ff4b5d3426579698b0a85206f65ec0df42dbbe99fdf2e41d8e2e7ad3" },
                { "en-GB", "7fa2e6b708d8b0ecff3b2bc4e14d472a83e0b9fb268c09e50aa4ef331b9f6ed6ccb181a2985323d4f862b457fcdc9303a38d7d2ac7931000099b9496d09aa575" },
                { "en-US", "199279187bf5c914885ee9a56a607e017400de749d8a497d3b7c241a4d038a237e5f68b7b6d86d91f0e0e79493cf7a4612f2af756643a6a6a6b5c2cb9242450b" },
                { "eo", "137a08b1d302e003558a31e5c7df1f901766b348b758d31b583dd41464afa48e59002d85de316d450d73638d8be7efefed149cae7fca7ff5b66a74e3cdb77839" },
                { "es-AR", "354991cdb2fcbae7c5d62b535f7d8504e66b901073a3f81adb9d4bbe298cd21ffdcafec4cd29006fb0f181fab1f5bc35246941a56cf0c0fa1be0b96ddd6a37a3" },
                { "es-CL", "f980b64fdd987b7df2fa9e59076a5791d1badfb75bb0e1a85eed443b92cb41e46b4c46134b47b1c4e2f5af135690fd57c930b8cd33b32625466660b171659b47" },
                { "es-ES", "25da72ad391fb73656359c2c817658fc61425622bba472c9b4dd591eedcbe13c7bef0cfe068a497459d14adaeba588b820322cc9a0e3dd1147d1a833ed4b796a" },
                { "es-MX", "815b82cb32fd40f69bda5bb45821c6c56876b95d21c408778d5f59b24d98e41e0372d6e0d056e25ddb79074e104f6ef34129d5d0834a09062fb6e23f8e49cdb8" },
                { "et", "09ca7ceaa3645209e33aac9ef08bca8e5cbe7e954f501a93171ff256fcb30fdbb6ff000a9dcb5941b2cef9c82c4a2d147ff207445ac36dc4c9761503b0f9ab6a" },
                { "eu", "e4ae44778eac4f8075716dbc5cb7d32c772334098cc58399a5f3515279bfb45dea25a7168ea5134583b474e13aed170c7198d30d63fa79bea2d5f0e9e38a42c0" },
                { "fa", "0e9eefc658e0eefb9ed77e2c379f7a939b97e29d6540bb2e2a5ad5f4e55a77d62b0e8ef26446dde81ef5ccde3ee0fca703e18684b0d203e80845c3cbfec88756" },
                { "ff", "5bcadd214984636946ddaa0542c06e75ce7fe18514b7ee818aabc2bab6e3ca4cc7fbb25bd8fa9da93ff8da8a247678f0b371cbb599951ce1d6df90c63609e89b" },
                { "fi", "9182cf3f58c7bd1ee3df2031ff443e3182186e3f78bc018c1b77d04a0eb3be2abad80c22e0c30a4199f123ac7be153645e4cea8d7f0b5a871acf4ec98eedd296" },
                { "fr", "092e95d3816dadf39cb62a1eeeeeaf5232d87f25fb6571a21e460f685ae884f938cacad91163688d9b3f481c878656e40523bb7ce2726afeb745297f9a8534ae" },
                { "fur", "f5b20aa308cbbcc5aab5d840d9463d6715da1339614176c2f9d3eeff280660d129d2ec276ebcf96f939764fc6553d57eb9a01d2c3ae5b0d73bebcddee9f15476" },
                { "fy-NL", "38a2a030172f7e41c9349d5da398d552afd7e81f3fb467e988585283168c0ed8ca99824fa0e6e9dbcfe4a61069ad2cac6f49e47d5e76754ebc4d8fa6d6f73667" },
                { "ga-IE", "ad6abf0a6eee8485a6a3e1e90ee7b2a9696d54a498d8ad82eeb373b2417c999082ed96de15e740dd1b24bfc64e87731e53b79b05347d41e21eff66b273d0d59d" },
                { "gd", "cd52cb44a0f45690241eea1c5b88961526564418733800e576b4dc6f30d2cdc887b055a9b62228fd3d46ebfffdfdf5b8c63f0824d840863e96cab70aa4bba88b" },
                { "gl", "fb23971781878ca9d745f54c21fa59a44653b677ee2559c66842c9467519751e7331e6372965cd1906348c8937493d7cfe79379917aeee05396a36f80fcb98c6" },
                { "gn", "ed4cad4d02fc8a460c6ac6e4e113e8f65a1b8ae5c5e0dfd1c604120b086e3bb3596b6281be0d88997d8aed60a0b24ade857352e20abef18eb5f5c9f41ad28278" },
                { "gu-IN", "b8e776653211c02a6805e2ab59637f596a4dd0a3b018a4567af04ebf6ce8bee0f5c488e6291f73d4b52bd4c5a7349fba783e477e449062e0868781e6c4928d2b" },
                { "he", "c3abd0f7d511c5196847c8740ca5e4c7034720efa3969921b0e03dc583832e718cff5bef9ff8b78f874a0ce12c451944cb7fdaa73028411716fa98c7ba4317c7" },
                { "hi-IN", "1093b2d2f45785a30749c6909dec789f1a37ad1a7e1c3cb9c47799c879bab4a7bb0589d94303e426c13d95fafb5cf892c0cde08ffd50b1648d52894d6af0d48b" },
                { "hr", "1d702a165d3fd12c2ae837e49c4df71438a73b3a9a0ab0cc12fc998bff3e9f83a0f7f45ed70e1a4726f47b28a51724785804b161cc704863a5f04414c0cb5491" },
                { "hsb", "1f3a9959215760e738b4a9e1d0f2ed389a4e15b4437a97e257539f4f3be2f111737da7c0267f2717f1289212b85e1f5ad3151d81fdee1461dcbcc5fa0432d6f5" },
                { "hu", "6694d1e85f55a64a5a9bb734bc84f02f0b34226fa0255d66b4c8b40eec83cb7eb67269b970b2ca24140dcdaf60b5e0cfc5200bf1be78c4f9e24b3bbba8603b5d" },
                { "hy-AM", "ec2a72d500575904217bf396eaa9eb37dc270a199234071f76655e32d87631c059138473054fee40be5d1c75b372f8559520171c06ebf7ef0195d6fe78cb5cca" },
                { "ia", "c4b475e66bf6ad540a80c84f67ce0f0e4d4bacbd27d9f25d7a3064d4cfec7e6ad1869763f87927d015a4cc8a14a76836e6520073ae15729ab45941af9182e53c" },
                { "id", "543f0c15a2daa537f5411388da2c709483572ccba5970c7dd26f86b581afe53be4f23b3410d883ab268140d0c00cd21a5580fb54d55f53ecc053fdc393b76434" },
                { "is", "7ecfddb018c6ec4bf40c1f0a7a0998f3103240524b798a90f14828f2ff8775440305e82a63a38041fd675e2abe7e86c293a2b8523a879cadfecb57e0e2baf3de" },
                { "it", "743ca1fca16f23c4be568de05a3cd9108fb28cc5edae316ac63a7324d84fa167baa7a84ef283b2e63c016078ecf3f46107c593e5ac742208674db8cef488122c" },
                { "ja", "b15f11861468cacec9a6cd51803b708524853f176d0bd5423afdb7b780a1fb167d0130ef85007d8591e769608550d6e54b60daeb0096df67547826d28162f5a7" },
                { "ka", "9355cf633b27df7efc13dcd2ec246678bfe732b0f717c2badceff28006a61138ebe9ab4d9862c25eb8c3d61076c24074a5dcae5eb43fe0669f5eafd68052c764" },
                { "kab", "ec0060e376b52322141a96b476b977f9434cb6778d30b7cba7a852222dd0716e6b85ed877cf0bb18b6097d93b5ccfcfe732126d4b476d76cd9b7628551a49ae8" },
                { "kk", "22d2cb87f6eb72515d5a9a8f2192532dce1f414a7b9dc9e9babe3e685b4f21e963e417626ce6ae5b92dbccc3aeaa45d558ce62008dd470f0c4bb6538e5b8e64f" },
                { "km", "76435455a8c5fc470249a4cf6ca8401a7926ec0d0a6751e21ae1ad21a385042c60e28ff89fa6ec81cf7ef1778b803655bb953d03cd728aeea443e89cb4a433e4" },
                { "kn", "32716a95707b19379cd9f782eb0690e44c9fc92c9333c46f44b42f8c800f46ff41f9277f06f75bb67532cae470f6784778c55726879baba1c3903a490cc2aef5" },
                { "ko", "ddcabd98ccf44d94adf15a6361ddf35a6d05fe5de5d3974c19fb261caaa201d78773da5cd3eb068fb3e27bbd4986ecba0332f4741a76b6b857dc9550ada65757" },
                { "lij", "418b4f66b70566a98ce111de5ee669c209ce9faedfe1d3b011d023de3f83c4124c98dceb08c562cb68af4f778e9997be2f374c83fc39e668c4a4f8a8c1674b59" },
                { "lt", "17ebba6bdb7f2e209d6c48837e7728f5693ac1560d98a64a58ded8f8279262dd5600fd9082614517c4c756e85238caf67f4eb60201a20ef1719dba82196d3de9" },
                { "lv", "4ac8baa6898a4ba1148f6e56f85f353390f204a06fc7246118936360a3c53c943362e437e32f2d8d325f1f70d59201652aef6b0ca8497cb8ba1828902e3a1081" },
                { "mk", "cfa36a3a1a3b844ace211f3f4357e958f72db5b9b905352413407efdd952aae14a1b92d900691e7cf06793f3292547cdeb7e801db0d654bcb1af6401d67800c3" },
                { "mr", "db0ce3cd3e01c483f04b02440a8d8de10db522bafd9922e1e3262662bea5529047bbe24c2cb118224664d78615ec0386c083c07e7af9ee2fb869e1353a12a814" },
                { "ms", "1cd664b72ae5689602b509b680e79467a7ed15bb06bae1b0ed3c87f399272a575ecf7ebdc57c909ef33e6683c416a58c97785a211211bfbdda5d63b21b296010" },
                { "my", "d428e407fafb2a414e8d4574a061364598364a43e1e683291d5783edec8da74bdd5e8b2a421cb7c1c51cf109744cc9be7ed7a720a0365db878633acdc0181418" },
                { "nb-NO", "0b177522fabb4aee634575725bc1f2f7dfdbc1352acfb931a3029ae05d6c8f32b015dbff2d94c916fa07171ff31dfa876c1ef94e821469bb2593dea8a4bdab64" },
                { "ne-NP", "94bba91cb08aab4ac9537cc2f39e40701b5b68ffb0633f52ccf756c82fe91789d2eb5979a9f6dd0b7b3f7d8c3bded7c563fa214735b423f2695b44ac00607539" },
                { "nl", "1c17a64c327015bf989947b23737c12627da1db7eb151ab2f6e472abb49f97d9517adc1ccbf3b7c9dfed0e0afb238d6a306635ed9aa8122ecb73ca754903c1cc" },
                { "nn-NO", "abb4531b377a0b98f6d874a3a3046ea61099eb678601f2479302bafa34c997d372cc842909a2fd86731d87f43452beeefd30615d03db07c5fcba872ff4125799" },
                { "oc", "94f2586f7d78df7bec54d90d52dd6ac02650b9d572af1129482b0def03bd40ebf08524ab8873922acc263dc501f5f4031e95699d05b06d311a726c125413f6d7" },
                { "pa-IN", "2e8ef6442b42e9ae00d3ed8e74d2feadcbb173b24096e615b8036abc9346cb4a188f5ed3120e8d226acb16ebdf6119bd4cfd1f2c6718ab9a297e9d4a5733d4ff" },
                { "pl", "1d1bc426fe9948aa5895e28b4f3d50086daf8c017af7f94de89c8c58fc2b438f591a0355d905ef591a846d80469cf1f6f5494d26782ca87c6947f5e24d911808" },
                { "pt-BR", "8acb7f3b81ead23db23994c3bf4cba3e702116ef2c40b67840caa2b0239d411a8a7abe7190220deaaf1b34602bb35e0efa62839016ba0fdcdfdfecd48bc0091f" },
                { "pt-PT", "aa2f0e6fbe4c9ff07a43feae9a0e4f971a03f5383bb2516fda847109982274f67939d1cf9579cef05ef0d474412c4d81027e6cfd17c42bc05fa9220ae7d415b3" },
                { "rm", "2ab3eec5edbd1397dc2dbb085952bbd8ef0d7ccff4dd44d41eff1c1360d6fac44038fa35873baf388f4c9133cc7c9296903cc78735912e2eeb9caec5faeb5641" },
                { "ro", "589af709b1a4aa4376e942cc03c1b5a6bb5f14b07b44e392b2637f257d06f961a94448786cbec5eeef676297fedff3d7653e35d97804ef59ed31c17b3886f927" },
                { "ru", "64e1fdb173e38fc1822cb258262e2c53fd95f2ff795580894f3f251852ed1a7fc5a2c00db43a55e892613fc029df52065d98f5dec983bdb0b92923faf44998eb" },
                { "sc", "0003676a7325de8998be3816f36c71956adf01d09602d5118016d89e46982c64ad17e067c8c52a0d65f125e993509a10a2d2e7d0abe2847c9c72220608adc0ed" },
                { "sco", "2d158ee7f9cd3fd4a326be1c40b3f644dfb3432947dc9bddc10b481652b73ba83fecb42fae137ce315d5177ea7e4e597d6db078f0654c9177e6b48be2e2213d7" },
                { "si", "33a9c9238e8618328cf81c88bcd857f6615ad9603201139080e15f1fdacdc5abc2ce2479c913d7d2755bfd14d5085706f1020d99285cfbb578c04346d54dfbe5" },
                { "sk", "04533a5e39c5fb9ea53b04cd22342388d63ac49484270c7edea1cd9ea1c2de2514d698360adf6fcba45f2e4b445c3ad7248f8d1f22b7f918d40cbc7413e82488" },
                { "sl", "83fab9b9b097a37020a94a38e1e037ff05cb1a305dc05a41fe958b951d048bf1d850d126938c98ca19d948a968258f8b7fa0e76269fb308efdced27feeec9a20" },
                { "son", "585f878a3e60d65f210112c3a7c5cef6debf867fb8baa81e23749df415c0afa1b3744b748204467499ee0a4044cf988a539ea92be47ea6c7d6714ccebb96b9e1" },
                { "sq", "e83197f8655d11dfad7512299c6de0c2eec3960e551f14fff301cdfc48f1a78d5e4a3c10d84d1d305fd542f0cb4447d2fd67db1e32d5ec94d1851f5e94964100" },
                { "sr", "d6d4b051430e1202db6ecba1fc52744e7c919301eb4d9a49faa83b40cd225261276367f71e464d5764ce09468fff1a7130cecc0f2dd1ad0a3e19d12678a27be9" },
                { "sv-SE", "029c1fec7efb2d4bd654a9dd81e42b9deedad0c9c737715819b71f41209618fcdad62d97f48d239e1022f7b35f863e951bc39b6f60cd6e7958a8b95356815949" },
                { "szl", "448c98306bc9cd0abcc3c557688ea5b87b3690bee5a1fb6c344a5bf3e7c7833dc462bd0fa0aa1a84ff9f079243577e81be2dba55725e9db27529249e13d17d72" },
                { "ta", "0b1495f216ce757a3ba7bcf6b2e41d84510631b5b609b2794ede0924767f6a9913b948445779b734c1bccd5f20b88dc2122be6243b82894b4bd4e7ce693838f8" },
                { "te", "c56288230a1c3af0c3cd7e33d80672a2bdc787e89629d3e1c3bdd24e04af86502fd83864ab7f0729ae8031ecc12e15e41c4b98bde88718a69af9403124c2be49" },
                { "th", "b06940143bff8b7732c79aa18b0b1dfc63aed607aa3fdb6bffa803b794a9b04e33aad13851e10968494e9c4a9864c9942480f68f65720e2046ddb74dc3f13434" },
                { "tl", "7807e23e21b31dc0210a5b8f3b378632a3286d2c76b0bbe215e172051f05a82cffd15016f10cff4d7d5954ce7ae2ae1763997400dd0de3a8313f2b69ca3e9480" },
                { "tr", "4628c62b38913c812b519b0966f8e80734179041d26562faa69800e77bc9d8ac5c5f4c2e13094d643b1cf9d3cc48cc9032d395da473d348b25078680171e0497" },
                { "trs", "2d882ef7daadd3fe2665eeaab6ed550409d0874702ee18b3e7858d8e7d031334db26e49b429c0098d271d8f7a2024e370feb75a09811de2b85fb92d027832f94" },
                { "uk", "53f6822c99aeccfbc31b48fe77afef76df790d6a1dca77fdf048ee827dab625b048bae997377b8ca6bf6a46f4c44b72764be55bdf7694eb4643c6664de12ca2e" },
                { "ur", "6c0a7574ad2d0087828834a72c6689354c93dc7b367a48fd8cb4d9d71ae225423cf6134009315f07cbeae7126e5c9791713e68eaef814284f3fac408b5cc4351" },
                { "uz", "476e96d0be3510744e18aaaed45ad858d9e94fd5d588c7192ddbca7e81ee504effd13c6eadd2ec3d03189515f84205f1565fb819235349a742a6ac225cd69b1a" },
                { "vi", "8aed1c0e31192486e8b5cbbb72f0b2162e759ceadcb0d35ba5a6e6698f7c165c0b0be21cbea243bc68cfa7cda45329bce3ddd1a949bd4bf8ba41e8538353a55e" },
                { "xh", "48f869b41c6ef9b3764291be4529ff744dd8a65c3e1aafafea3a970e0e0b346741924a374fd37d1e30069a349d78a2114fcbcbe2b3ac447a08b539474d68ed5d" },
                { "zh-CN", "3b7341aed2615a0939bdafee7dab1b41edbc4e38854d4df48e2351a9ca3fe4f340bf504f383d2d53e4d4e73263f24f7f3c0cb3c954a24b9a550c66e1fc4cf57d" },
                { "zh-TW", "5095cc6673e4848b7c37775e1f1fc3edcb254e054a281dc2b7f411c8087bcc67b52c59503dbdb30459f7588d19e96587bb27fa706f5bf40526d27ed2b69532cb" }
            };
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
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Firefox Developer Edition (" + languageCode + ")",
                currentVersion,
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win64/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
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

            string htmlContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                htmlContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox Developer Edition version: " + ex.Message);
                return null;
            }

            // HTML source contains something like "<a href="/pub/devedition/releases/54.0b11/">54.0b11/</a>"
            // for every version. We just collect them all and look for the newest version.
            var versions = new List<QuartetAurora>();
            var regEx = new Regex("<a href=\"/pub/devedition/releases/([0-9]+\\.[0-9]+[a-z][0-9]+)/\">([0-9]+\\.[0-9]+[a-z][0-9]+)/</a>");
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
        /// <returns>Returns a string array containing the checksums for 32 bit and 64 bit (in that order), if successful.
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
            string sha512SumsContent;
            if (!string.IsNullOrWhiteSpace(checksumsText) && (newerVersion == currentVersion))
            {
                // Use text from earlier request.
                sha512SumsContent = checksumsText;
            }
            else
            {
                // Get file content from Mozilla server.
                string url = "https://ftp.mozilla.org/pub/devedition/releases/" + newerVersion + "/SHA512SUMS";
                var client = HttpClientProvider.Provide();
                try
                {
                    var task = client.GetStringAsync(url);
                    task.Wait();
                    sha512SumsContent = task.Result;
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
                var reChecksum = new Regex("[0-9a-f]{128}  win" + bits + "/" + languageCode.Replace("-", "\\-")
                    + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
                Match matchChecksum = reChecksum.Match(sha512SumsContent);
                if (!matchChecksum.Success)
                    return null;
                // checksum is the first 128 characters of the match
                sums.Add(matchChecksum.Value[..128]);
            } // foreach
            // return list as array
            return sums.ToArray();
        }


        /// <summary>
        /// Takes the plain text from the checksum file (if already present) and extracts checksums from that file into a dictionary.
        /// </summary>
        private static void fillChecksumDictionaries()
        {
            if (!string.IsNullOrWhiteSpace(checksumsText))
            {
                if ((null == cs32) || (cs32.Count == 0))
                {
                    // look for lines with language code and version for 32 bit
                    var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value[..128]);
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64 bit
                    var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value[..128]);
                    }
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
            logger.Info("Searching for newer version of Firefox Developer Edition (" + languageCode + ")...");
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
        /// the application cannot be updated while it is running.
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
        private readonly string languageCode;


        /// <summary>
        /// checksum for the 32 bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private readonly string checksum64Bit;


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
