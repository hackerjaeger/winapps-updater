﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022  Dirk Stolle

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
using System.Net.Http;
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
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Firefox).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


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
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            if (!d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/107.0.1/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "c120dc1129e364ae2aa50fa256212adf4637a4ecca226edbfb87fa989eda796af0ed28e27ae43342f746cc9781edecdd6cac3b5996f6df605b9b051e70e35024" },
                { "af", "7a988c3321c8eb4ee54893aba5ad18104e4cc6342842f90d6f8bd29cc41af5f0e71c99a550b36531a9a2f128ebd9162c9fcca6d245fb2ed03540eb98d237becd" },
                { "an", "c9d24b766bb628930c4279a7ba0bda249bcfdf8e6ba0a101ac65ed54336814525c2aeaa799aa19dd64e2dab93f996db7122de68258848063de1b459049a70938" },
                { "ar", "12f4411eea4435e0dc4e6b7c1452121f53d902e57b0862458489faa0ffbe4bd07fc5ae5ae06ed381d9950c1a32cd7396bd3e29aa472c8c14297759482bf5be0e" },
                { "ast", "ccb94e0021c6cd4a0cc82fd7cc9e7bf2459301987100fcb270121386b9f15489fcb3ff241eebe76ea52442a3d15cbcf2676acb5ab3213e44eaa8183c14893d0a" },
                { "az", "449b39d6eee41a7ce9743f9183a3b3b18540d34f36cbe0c471193d1f9d5cb3e6907eb251e60b587d3a16e46712e3e05997bf9fbe435a2c82aa80312206969c09" },
                { "be", "1825416fdc1c08f8f41148d35966e3283f10d23a971f9f298f66d3191c57afa5cb8cafebf3abe646ca49cda674215b4c5c686383cf6f1ede06e0d7a0c63c0b81" },
                { "bg", "f8acefd0268173042f3314b6e52450627a5984fd79876cf1aea19e86ee38170bdd5191808ceba798d17f73505157f8f3eb891d166c4d2b23acb5c478ed762ffa" },
                { "bn", "8a38efaa5b91ed5bd091778c850c4fd26adee7fca8f3eff4c254cdca52db2aa44bc700503e6188655ef3110b55b5bda5fa14cea15facb520d53933ad01d1853c" },
                { "br", "1d61cf12dfdce746547be005f5b1b6cb57fda0a57a67117411022145edd3dac6a7877b9786ef66a226349d70c3cebf9fe4a17648e21c7228674a85a3873e95bc" },
                { "bs", "bc13e5732351bb9c02d7a736e96f4b07b80cee69d846ff918dd37bdf43ee244cc9ad38b723d49be8b0718b26aeea77bd324c205655dc05d818a493b02acb5ad8" },
                { "ca", "ca2d4ffc0e685511d7ec369274c4ce3b5eb44db60c5cdd9963e84b62f4a8c12bab94e43d72d7b98671f505c21aec36901560dabd95a7e50a63f0714e49d6db2b" },
                { "cak", "6e10231a357bb0c4ab7ce8f9aee22f089145fd886fdeb4a34cecddc8cbf86bcc377de4d1ed740c0846215c1d9c1aa0c1f91fd2de8821bfc0ab8e82ae985acd7d" },
                { "cs", "e246d59de9a4385b29e299249c39878cd79e396e67ddd8e8461a555b6f589bc9628903a1780e63e725c92e096e18114739917802e39d2e5a934094f9868c4e4d" },
                { "cy", "29d9ea86870f198c6d6c429a32696d7aa1529f535f0b70b6e49073fbb08af3be162a795ffe4668db499d178d7d8a317bf056dd17080cb11382aaf8d2f50e2b53" },
                { "da", "dfbb4e6d6b4558eb07a12ab0f5dff3acb1f5392d701fd447ae87b20dae4ed05d4045f18b527ad1c58a3508d97d277e289c96668b1f7ca2792878a4a4d8080bff" },
                { "de", "4fdbb6726d439d103ff861b20d41f47505008278a0993b85b58c58bcee154e86fd40e839eb1aa2ed35bc81c6cf7926d8f2d1e79c9f94c8d7ad29d91b3f1a62a7" },
                { "dsb", "f4dc28c3d2cfd00a942ddbf6aaffabca38feeffdd2e040e010c19c97c458400d3ea310d41b41a6862e98c4d7176a97e0642006345d18caecbcc1d75b82d6cfe9" },
                { "el", "461ec9268956cd54dff4921de10773284c03eb1112762ecf0e31df75877277a2b57ea1712dd27f46f999b115352b63bc4f34b31aaa9c0ad5d2cb504fcf5bfe2f" },
                { "en-CA", "16068a3c6ab43194475dff810d8d2d457bd6c4ec28b168bf6c8980ec9b757f716296257907c56c30c4bce81f6f242aecaf54e61bc0369b942563306d152825c3" },
                { "en-GB", "bbb1d18d6558c3fc04298dc90022571fc24573860c41bc5e5b9a52b2a4544a6e02dba507a64298568f349016986fe7e7800e83d5c56a1d26332d992611c5a6e7" },
                { "en-US", "a090152f5666a8265ef82a9e371ddb94c6d5d9c233675fc6f4db0d4acf03fec3502490ed92e734fbec561b7e8d0a25f85ccb50ca40a38147b52ea7727436a74d" },
                { "eo", "84593ca4111f2a49e21c281e53a75746ccf96215139c16449b8deab085dcfd84bd9b910ec0d505df48b43608a20761b1fdf80db0ae5aaead0fba24cb46a499d5" },
                { "es-AR", "16bc32188c7b4e8d59a09e293811e78e921fd4c871b786cd37d531e4e600108bf6b73dc9ff1c15e11a837b8f8a05578c3888843a4eff8b9f977765184ed6e82a" },
                { "es-CL", "9aa3a4cd7c1df61ea0d60c2ab9d971f67f47e9edbd1f3d79da101484dfa0fa8e020ecb2fffa9091d7efb57d900f5ecba57ccbc755aa5d5b17c2efbfb230b91a0" },
                { "es-ES", "cc58a23362ab8b4e094f4679027e8bc8267ef5a6b563087ace62491c28d2901bac60e1e4a0e7477202f5d58db5b075c4619641a4eef6a4a82f5f331c03fe4265" },
                { "es-MX", "281ed8798002c0cd99753df5765703d40e67abb0ea477914e4350d131f9a54b125dec6608d6ef08b19384afd5a1176af8527090d5102747711bd53f8b70b4f77" },
                { "et", "65001dc1d0d2500bcf411d176c77e288381d6176dd9876a556e4adc587d3ec9a5d12ec1159aebd2e9bf61ea56440089d005254858e3d6a2b88fb48ca18427b3a" },
                { "eu", "781670a1c72d1d484cb515ba21ab36ca38e3b2d33e20ba76c727b3c5424fc89444b1595b1b605ac3ab9cdc764ed869a1c06852e99ee74992f41954f25e246069" },
                { "fa", "c1fc1b6bb910587db3b03fcabc8b0e2490164beb456a11bcbcc7f10a1aefa6fe573fd7baf663e8ad10d01a19eb25fff847149f51fe301065ca4cdf1b047f7b58" },
                { "ff", "1584fbdac787a1b35e8d1f534b3853871a6d2a4461fc1d9c14436eac103b9311da79513ef31f623ef42a1420854f41418e5dba114eb5829e7221c79ed332db23" },
                { "fi", "34baf21bdbd532b02c2080340780a6876b1b363416b2e92b222701089fbee9ce8b90a0ce83bf6eb9fff5628f58976b426ff10d62659f1e2e8eb70eca50c90bd7" },
                { "fr", "a21bea74a91097222b189f5a2ff4e07e984790837f22a8e2f3923ec8ad5f3b8ee9b7174098334c2483bc66ca2959d60c1be17de4d0cc553e1149bcaaa2f35423" },
                { "fy-NL", "ad3944d32b8fe6d9c94193700a7f035f38676c162c53fcf340d91f2899025d52d4d7684157db26f2cbfd16f746f55c474d9fbfcb7b0faa4ad216ab9912320482" },
                { "ga-IE", "5f39082b608e4d5d7ca45bb87736044e3fd7ef4fa4092a538ffe08f5376c86bf109d29eda72f0df2750d265f7ae631b951dc1858dff40ecfa653705eb61f6151" },
                { "gd", "4eb575f94b4fd4b28fe5d0432ced1f244b8d61c18aa8810203b6d13e6e0a3c063f05e71b6a9bcfb5e18ef7f15814c567980999d5780ac306f622d04f569247c0" },
                { "gl", "25017ae6ac7c669d4ef1addc5443e16056f9bee36bddfc2902ea4b1c441bdced37fc48b3756fd8bccc0d89c79246ffd8bee05c95a41ac818262303037b55b96e" },
                { "gn", "35db7849d120a596463ebaa45193f27ce064a58c1a577e2b7405d9edc645afe98b09e37082b94bd143fecf4c8797ed5a6c1b3bda797ad521ad6fd2851a72ffd3" },
                { "gu-IN", "54efcc2915570574f1ecb52ba5224990a6829928e5bd0cbd1086ab2682fb258e5a7d4333510f3f203ab10f611a978d39f84c4f3fd6a2d8a2e6b154aa231919a6" },
                { "he", "a88afccd7f252c7d65f5cf483c5fb0aed0582521164aa4944032e3a24182165581487d1c27a1767c06cf011c8bc8e072d900d868fd4b3842874a2f2401150f47" },
                { "hi-IN", "b96f0c0a404aa9c25eab36a931476beff0ab8867c609d07f1a8e2366c621c7b3441f66a0fe4e10877e0cbc489e99cbb888df68e43bd64a610e63b921b5706e9f" },
                { "hr", "f3691d2990c88c9d43b7682b4b22be7fc2751f62aae07fe157888714458d5fabbecd9ee4fc7c8c6a7ed753215c7f2649abd73afc67c28f69d550862a21914fb2" },
                { "hsb", "20434c9599a00d2708a468986d94d74721f4b3035256d6fa9f0154b0f18df626e902b9b0a13a3091dc7c3e036be1596d47b2f91c83f8a41fae524cfc3708533d" },
                { "hu", "781707482d9d12cbc4da83333fb9153b9e362b8f325154649442f71906a01b9824368fcc2d90f26e9bc332ebed4e34f42e47f56da8cb8959fbc3e50f54e4b41b" },
                { "hy-AM", "22eea57e9957edc1a29874092a9f12ad63fae07b21993bf623c1d31832684a80eeb2aec32774bab3f7a52e8480ae1bea4ad6e99f4fad72a5a0b3407d659821e1" },
                { "ia", "ac734a0dfe0765b1695528f1522c445c6bc28e924f24d31f99390e4d4a0ecc5f8ec3f544b575122f427e7fd3c5313cd6963537e0fafd95dff15e804f311631cd" },
                { "id", "7fd80f259489e0c0614a32a67cf9f72eebc308347c63b225fb5d2132a99136dc97f00eb9af1c6d9641f8da8d9feabeb59fbdefd7e40d25ed2e93260f261cd26a" },
                { "is", "f4ae2c605af46f3e89d509ee8b5fe3d2df0858a029e4ffb6fc54d0f7e6fdf28798d8c64b6eab2ab5a084489478ca0287eaa604ad9eed015f78265af087faa05c" },
                { "it", "370ce5c5869e7df9cea11b881f7f0c2167ba50a8ab99d10d5b668fca11fb2925a86fa93b93cd80184b4c1fb1407f68f35faac0f0271d2e2928b1eeb0d09b5267" },
                { "ja", "641384ec8701e21d9f0440b354879b709bf72c4a85e76aed93eb39c781d6a36f898b9c5f75fc0ce1ccba2350b0110147ad30b5573bb002a54034122c49ac0f17" },
                { "ka", "22e1383a733d436451b6ca6000eaed3017f914347a6039124c58331cb45a2052470b52eae2d1ea462516fc4080e947f0a1f2fe7e109667ac1f43f3f4f44d81d7" },
                { "kab", "e1db9e1ba16a8fb0b0d6123d9095f0c98a0b474f011ba6fc0708d02e84e295dbaa846bdc5848f4aa652d44f10e2d02108bcf19eef06cf6355d3ddc9433a2632f" },
                { "kk", "5581c9bb7643988c4d6d0f9f232a2456bb055d5a83a0d5930c610a179315510f6e9c6d176d4bff961b4fe714d51ab1c28513ef2c76a9b061651c19b4919f8aca" },
                { "km", "e8535cda24d698dd7af84a862926bbd29068f762e56568ede8aed9ede1cdf2c691e5ccbd6a2c70652865b7e972386b095d7712b401c08cf48ef2279ff2d5059e" },
                { "kn", "016be014eac959880a896d1725da0cac79865e676296889230ba35fe134330cb0ba055418205f1a0bf203cb206ff7fd2d63612955fffb7d42cceb6ce94cfcb45" },
                { "ko", "a84579ac49d14625f31d792efeff19f6a95d5697c077f0b64d08f6c00da30f34a58ff02b39193ae4ee57d34e10001971148437e8e5c190f1183092f89bae69ba" },
                { "lij", "5a3fff30b4447b5c51850aca30a08125fe4edc4b4afd977b7fd9a59bb804667c8f00506ae550ec3cfe3ed49e0b32206121b27c3e80b9b91c3abca6095f4569b1" },
                { "lt", "706b65384cef5839ff8f52e916774baf558ff747b9a3e7961cbb0931ed241e668396146d7a7bbd10abe4f9264db0b7883b4920ff0dcac8390072f662c2481a47" },
                { "lv", "62435ad00997321edc27248fb8cf8262f18076d4e37b19bea9ae80bfae72de87f3096c3f980e0f1e8e44a395daa4cc73b44d852fac0ea7d4ab1d33256c31defa" },
                { "mk", "bb23cd643f8ef9fa74872faf19b3ddaadf2fe972026af65b18ba7715dc5d980eea189e00958462ab3c5e69e2d85788fbf546c4b553f444b3cd6b57e55090c091" },
                { "mr", "317fa14ab5916b34773c24dd43d4a2d882875213f68f808b8b0b1212c6f24376cf15edb43e96fefd7083797ab4f6fd00f968fe1b5428037be123b97e70b4d419" },
                { "ms", "e918084e87b28fca4732a6f97a465fa1c236f473947202000f5f6d0eb174cdc547579dfbbcba9a2b5d08953e55fb65584fedee747db0f733d7f8f2997008d61e" },
                { "my", "a3d25a8752bc13089d4b4abc0e7b815f5e40500c275ad0f48859ea179a6a3a96daed6643ebc8209af6f5c317040e208428c894bbe1867ea22771b0bf5c212c41" },
                { "nb-NO", "00dc5f0ec38baa3d662b1adbbb21479fb301d371130c4b7f6a6039dd5fdc26d379256dacdc16489b9273e6fe103c26d62914451f6c92e1c09510b083f074707c" },
                { "ne-NP", "e5d5892bd4ae659ff840f6410a6e590052258aaf9740271021708647e19d48d550c0c5c30f54606eab6b2c81e8a72aeea3e40f7ec44e238faa584fa7b6706e9a" },
                { "nl", "42fd39ace9cc32af3bbc811887aade722c4d5312a023e267fb00131176e68c13e08d375c99c9268d20cd24e35ac567b7de1b2839c7ceb4a00d932e530950f1c1" },
                { "nn-NO", "224c9451c7341b7cae8ad98431e57f821b062d5e9e83b83876c408fa40325a65f992454e700f2239ca2f8e4f9902ad1e861eb6e2c5fdced8671286c66b6f73ed" },
                { "oc", "eb2343efd571825fb9ad383e8f50559006ac275b8e12fa0ed84bc72d602bd912904d37bf36b43e7e3e2d7ddb2bb0e0e2d1c29a9ccc2f403bc4ca3da91cdd162f" },
                { "pa-IN", "1e47b4da176f282c3131e22ebc61b13b7395f153386cef410ee3ba3df24c81970bd0670c6ff873566f63ad563e07aec5be6babf94896a7df5954f56ad82f21cf" },
                { "pl", "2ee84131f6766248bcc8efca783f0d540607b887d08abf7c501c197651b379058bd632b6de1aea75b39b17231e0865f2da6cd0f96e99b3a7deb83f39c5fcbeeb" },
                { "pt-BR", "4e0e976da729974cb48f06a1862ff60718282a90c46ad733767d46d0c188d94b9002049c58f060f197ca8bbc0f9b6852350596b8bd35b796b5a9f6d48c537a41" },
                { "pt-PT", "bb9754171e9f51d607473568bc2733349051f309a9efb40cf165594c44bcf665b386420b96cf0298d16a3fb693e7de5ac03d866db1ecc137b8b494254b8174c0" },
                { "rm", "f01f3f21f90f344fa40cdb8d965e31e5ce2b221698fbc410c08a9904bb5bedd6571c193a94dca518afc5b39595d748684403a4000645746d0d00db4a2b2afda8" },
                { "ro", "72315dca5015220e2cf2863962781283f6be3774b1fcf2f48787e180bd698734be7e25819095de4271ddcfbf2d50990044b54ebfb7db52f5eaaa371ab40b835f" },
                { "ru", "74ca6bc770b6396dec2762d4237c744cf2345fb4b58d9cabba15e713e1b64979d425bb5e5626f17a8cddd29a8e0a2156cb7e6ab4d205290835cacbe75aabf1b8" },
                { "sco", "134556132e3ab189ceab9d191253e99ff3106d42c9efd8b143cce35f5d1441a2507a70bfc724714394de028516e39c059573c7917484f1274e4326577839155b" },
                { "si", "67afb3b5f0ac06c1c07b4aff1b161d685094bf1f83408272b21f61aed862fcda45361a82319af1d6382ff8dfe21af18efac341d0b01022abac52d073a1dfc54f" },
                { "sk", "b61bdb22b134e2ed0bf0c008449862d11e9e93afa3c9cdcd4fde7fa06517307114164d297f49c5318b4ee759570fbb60e0258bb74358488b68a41fecb622d054" },
                { "sl", "feec62e707226872f881fc78a022a0f34a96a757a2dff218683dc8b1ebe17524cd6e7180f3f49a5874d712c5726c9789b125a76eb968864ab9ca4ca1e3b03faf" },
                { "son", "8bdd8fd295318968fd1a494f47718b9b3ba9fea14c3965bb21f25bc74dfb6e60b02df2b133e616f44c0a78d4ccdc05b885c12c49e64a7a3a9516ee3bdd285ab9" },
                { "sq", "5cca9e1de0c711a5176b312ba1203743073c622793d0da2749c49aa88a0e89cae9bc85e821f0eeecdd1b8b664664036f0511bd00c41567fdd64de9850ef2748e" },
                { "sr", "218dff742eda38058458883eb291195aed4ae099242c544cf8f6aefe6419c06227667decc8bb91636a023d4fef600807efa378df1f0b73f94ad3b3f3a2f628d3" },
                { "sv-SE", "0d90a6873cbafd92b65ef05feab98fdb1b9249d444bcf1e9666aef4bdeb7a424e62dd6ac996ccc37c7c19a04f8d3a8ac087a48eec89b23f3e307b6bab1f24556" },
                { "szl", "0d79df84f521cf72a2eb2b634e73e8ee411a7613ea981aca4f11df1c6d4c3740fb5011a256d60ea814fc46f8faadce549b3cd197350496a261990b8c30c06967" },
                { "ta", "ad9ac8ceb7014017d1da2a596c1c6e4d97274e2f3102922387c9c64f4714433dd7b40e9bee010dd118974a9cfa1f208c3ff2b9abf01a5c1531ddb5da30757040" },
                { "te", "2777365cc2d168b74a59e9f21e3a92e0908dcfc10cdfa5a181783311f8d151f83a3aca2005f33329aa9e3ffe832a150a118986c17b29fa6c69becf4e7ff0eac5" },
                { "th", "3d13f90dbb93f8de9ba839e9c37fb571766a11f03e763bf34ebef9f52321bc740cb943210a26da34a7790043a60b3de98fb32c87b213569d968d4e421b23aa82" },
                { "tl", "a02807cc30ff503dc3ef0008b374c8b53a0d20bb85e127495c79e7076cced6282a6b215a166588ed6691d6daebf99cb4ff73bfa9012b2fefced192c19d8d6ff6" },
                { "tr", "b476a9cb98c649e8580629d0300c9b68c005e43bd47e64bb7fc7ea095c57a7b8a624a63615d838436ff77b0a0c868da448217cee8bb2d107f9e6f4a15b8f1e28" },
                { "trs", "bdc9d9b9308f07831ab4236acd5a2d38b0412c32ef6b7283b88d9c38fcf84019fa8a9eb31e2e955e3dd8fde5b6c4dc41c9af910feba518958639b5c3edd3e87f" },
                { "uk", "055358b4b1b217a39319b5968fa4e4eda5f85027b0aa2a9c69e8e1e6a72632e2782172935c155ccabca6920d14c1e8b9fadb9432e1a67326dc8fd02b9d9a5b38" },
                { "ur", "320d3035ed5ba615fde6d4a23421f7ba5edaebb59da119286534a24a873735b364d4c7f341101c9a5f5d9c05b641a295b4bffe74eae28fb5f6dc4667ee812867" },
                { "uz", "b1dba58384d4867117756ddf2001073d62482cc97fd7c6865fd2b756130dde1087cae3affa2b10a2821599485678f9ae1dae23a99e77fb7473bef9188bc1249d" },
                { "vi", "e74c52e1290c898cf79b86a17a5b565c923630d0196923e5b936f3cee51172da6972fb324d37b0b7880af4f59ec7a10c659bd7dbf34276e44887337e5d4ff9e5" },
                { "xh", "742d742e523aceffd32a6a46214b9ff940453500a01369e052fdf8825b806c6fbb7e57e7d52df798cbf7ed83ff2a7ade02854a04830d19d476988db0024823e9" },
                { "zh-CN", "ae5bba37c4c34baf015ef4464857a34a8c1cabcabd8ea244eb53d71dcfef309971358a7967c89d9d19c50bd8f216577ea8f783a8bea735ec9bfcadf76e0b22fb" },
                { "zh-TW", "8a4255f899a3614ba4287673b731102b10217547e084399e0ad004a5c7bf46d4d5a4b75e79d2e7f566725039456aa6a629811325662ba08e5b26b01a65ca5c97" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/107.0.1/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "d8f9ed8806428cc46181e2532ed7ab006e60bd5da6dacfebf6e2f656a78b13372f433f684fcc9e092ac898c65e333c69ab9d7180ca80fadcbd0e1e38946f0e20" },
                { "af", "c4504a46fa6ba56decab2746ef328e3fcf0f60de2ccaf4fa4a3acf3976066a5e1e3575e6d7316f952f386f7cf6bf156fceea9368f3339a08fcf2a81b845019b3" },
                { "an", "58eff19e503d4f4970f1441e48399acd942b6b1b5640e15896b9cf12449041fddc9a5068d01256fe10a59cef60fdacea3b925768192017617fb62d36986b5bc2" },
                { "ar", "e3f7799e8bf095f63c658d55127981a875a91cc9ef1e028d7ea3aaa7749ee187310c50bbe0953c0e1db9d3ff0ac51433ed703c44ea6ba9235e4b2a92a3973ead" },
                { "ast", "715e6767c0c5007921553a8fd7cd225fe6c929b10ac590d6ad92b484a1bbc9bc18894318a13708de1e0c5134e09be2f97215eedab904713842df4cb021427e85" },
                { "az", "a3ba71a5c2d03b91f36e5304b783b5963ceaf5d09c7686fd8108bcab1d7b75002e2e2756ae9dce4d942d71232cbbf157d94f4eda752153e3332d52e0ec9a499a" },
                { "be", "e66d8a6b3f05f217004e67e799ff2be5e241e052abd273cd3ca0871bebc52e0856fb23f06a2ddd6d18701313a31a8975c8d20c61ce63100526d30051e14a17b7" },
                { "bg", "197cfbed3da95f9dc1290569a021b3349e04cc13facb5d17c82ebbfeda5d8cced79e14bcfa157475c9ad5b8d596c671db869a7d6acb37de22e40b0d90b088c99" },
                { "bn", "2c3808523cd35f7819c777ceb798b9140a4fec6d3c8e862de307366d2875b5b5a2df19a5df14250e7a9e2249075c05f3ddde358398b5887749158a807de62fbd" },
                { "br", "db1d5d312ff3ff5199dcb1a2842e1823eff1d5b430d5d7dcad98a4fbfb7f49394e38a05f9afaf32a6a03ff52d3239aa10235a26e6b4fc4697a2ba2007796d2e1" },
                { "bs", "4e0d0d8234d4187c56462b1f53c1e8f7b0f2cad36fc2de8313ee36878888faa46ff9fb690e9c5dded31a9a48e35874b4c8d69d2c91f8cb09cd429743982effc0" },
                { "ca", "98a095fb51e6c70d02e5d1fb9c24e70b7d3a982cbf7c1b1d9c1223414798ec1cf1d5c97b0f5926b3f457b6df13c454657519e247f2dc2f12b44f1d69fe4e4215" },
                { "cak", "7ec8755979cf409dcd5c91521a634c816020973ec190d00801558b6daf4912facb80ab23a380b31b6e7ac0492173a4a6f585d786124f6fb58ec44e9bf6e667e0" },
                { "cs", "acd5575c600b54f4997a1f33a5b861abf0d03e1aba0756bf6073d3ce443f811adced072f6ffea488285759670e76ae84a75bbc86315e4f6236a1d6babaee5b0c" },
                { "cy", "fd540ba161e5c190e47fd2c8830caac86d42b0506514b85f1dd12885a3303e6dc8e26e6b153efdc1638438d4a96e636dce1cac50301918ec7343098a4253b977" },
                { "da", "673d85da25388d87d223555b9562aabd7633fd525ad1b85b29bee4c64a2477c1a1b32582f31e6d697c4503fd31e481e0b7dbc10082cf5eb221dbddbaa2de7274" },
                { "de", "dbbdb472ace6e8fd3996ad643b65fe95e22d486c35b0ab3c34233bb998e52174873fbdfdc8c63bbf4d558dd4e70739b6975d4dbcca4434b8e9183efa270e7799" },
                { "dsb", "bafd48c3d58f8cd4ca44e64c1a2b016597016856bde3cb378f023ce1e13902efefca02ccaa3df3814ddaab66668f1822fcddbe8d6d63bbc6fadd2f1d4692a0b0" },
                { "el", "02237535e6725ff0e29d455dd317c709146b3d790a47a3a54df6b30651b3f2aa41a5d54e5ffcf9d45b1f450f0ebb64fe86f0a69ba3968abcda14c2a6b01c30cf" },
                { "en-CA", "c60d666236a03bc41ff3f43e35be3a8ec6ce3ae6557390155a4df8c3a6f423196c36c80615152ed06c7ca6318405566e3721ce6ab452ea0e63a382923879a240" },
                { "en-GB", "7cc0ff90bb1bac3534d385acafd67c13135817173d0dda540dbcada2a3c9badfebdd35e08dbe6e12a6e45e1a814a7bc34be261d6e2fc904ff1b16f345ca1451e" },
                { "en-US", "3ce636d351186351b213af56c0d0f117940507d7a998342f7693efa09a3dd46a4e58f1709bc3328601a1a909bec47a41cb2e74ee6810d8c3d68767b46be8dd11" },
                { "eo", "d7d121c90d99d4065a3b9d9957e4c55bf88cb94cee2e21a20a88b8e09c4eae6a3c7307929c05ec2ca865cb90d1afb6598242855e99d469a6e2faed500ef45dda" },
                { "es-AR", "28d859e805abad38079b5da7428149ae5b4a83d5cd02ade35d9c46ba22a237aba8f201ea757a46040e07a11428df916e5dbbb34a61bb918724d9abbda10f07cc" },
                { "es-CL", "aa3dc1e6bda24e710b374a53faccf56a81d93cd032067fabd9356e2c1b4816cdd9eeda1d4a7b180ae5eee12bec689e2c342af7151cdf47bb60a24407aace5171" },
                { "es-ES", "a6a0a8b40a0a064988cf3263f36fcdf9224b178984867c1c5f0ffc2fc359c7074e7c24f7e0103b9d7aa5a99066ff538871e1d18b5bc4296c1c3911dce20ea4e0" },
                { "es-MX", "5c19888504bde2408b899f4a1232a945efd4afacd83b7bde45637de54bac2b5de690b2e7a45dabd299f82f7cfb6f8472f1753248e5117eaf94f55e8c6feaa3c9" },
                { "et", "f014320d0b0dc9e128bea3c239f62c5f339e2515ee5cb175e5c3e50b07bc545aa17693ae10425777d50eedf81c1f8b86a438567d283717394f6851323a592033" },
                { "eu", "4854de6e5126a73fa670d60f727d342210e563a9a08717d9c5e16652350c1b57dfde7773d749dbba268a52854bd7deeb64977cf824a5f60fbb569da80add7c14" },
                { "fa", "c7bcb54ccf11cddbd5b594d85f2c18dcc896cf594ba4319aa9c5670c400a822c055fcb41d1c473a08a8eac26b9071287a18cd9fc43ba4c08343825b22149b7e3" },
                { "ff", "a1dba386c81b16bb98c3e1aa65f0cefa35a4ccfbb4f660e45e95355ecb7ddf20145108b89e4e050df6eb3839df35439c92dc87e81b8558a855fcabb19cf4ffb7" },
                { "fi", "6949f0b39d93d44ddbf37da42e686cdc6e19eb1aceeafe880315e761696dcff8f8075294aeea5168119f64aeb8e1ba9b804f43b6c736c6915a6f4dd71382401d" },
                { "fr", "b62d5805f8418ec3fce4ad525320eda35f12735e74f0a953e07a2c6a5e6d25edcbdba329d228a8d11cfcfd4f60b3f9fc7160de44625ad6b19081437e19633d7e" },
                { "fy-NL", "cc8f0fed4251c9909cc2c7fb3332e922dcdce51c809b8f5092d92483ef7b6ed718cfc5b764bc3edb97cde8a5eee90096a056d64be9d3dd8bef6c8ca40798353f" },
                { "ga-IE", "548765939f68fb194d7b8608454ed192f6beb03a6e7cdd86039a6fdf903df95c26ba15a782ca9b10ac4830b59d1bea8ffdc4b23f91a396d1c6456ca7e42d5c9b" },
                { "gd", "24546625077f4ea620d2872c82c6b76f0fa07b8d05f7f3c5d64408b9b931e9876d32465a5c8ca1200f6e8272cf889d54af262c3459615404af2cec8ca833b4f8" },
                { "gl", "b8ab192a27831b1c1441b6eab9a0234dfa165de50d27567aa00cc4bfa82f29edd5a604559a918e888f250a37d555f80d4bf1cdea0a335694d1f2849497d4c1ee" },
                { "gn", "ebba6970711ed1a3c80947c0bac4af5a73e33d1c60cc6492fbc57da34e11bd377a7ccf112a7f2fb9b0e0012105119d11f76eaa1ba37d30cdb87e1ea22d506a18" },
                { "gu-IN", "8410086b0ff13b199a752620305da29ca18f1596a3e77bfe6acfb741f1f62c820ecee6983e05437b5b01eb11c20b3f4ab21ec434041b99b0b8babf31a5ce8c36" },
                { "he", "b38dccf9088bc5cb50b46dc181f7f375046f52f7533cc0ad888331436d386f6ec00f2e96512dd6bdf371f74ce6fdb328c20923f7a0f3bcde40faf0fa87218cb2" },
                { "hi-IN", "504e5d1db1a3d2bbe8d6ae6a47f2b5272422e416566e3f91b7312d98088d8c053ff8118ab5a5582c75f9e87a6e318507119531494d2fafeb6d5c21ebc2cedfe3" },
                { "hr", "0d63ace29692f9720030b755eef3cbf2dc6898e20143b9f5acb4cc64ff0694d162ee3d65830cd2c69c63f8d77ae965ef5d112440a6c12263f1e28a243d38b68f" },
                { "hsb", "2ffddea3421d52c4c1c80f500aa214a19e1d067cc5ddf55f0c7df5a4e85ab72f44eb533233c2cef634dfd683308ed6e4ec436e66df82106e60ffd8b7e51e2260" },
                { "hu", "8eabe5fcca31bbb4ea0374c3b4fd034d4b98a8805871c021d77de50cc719fd845aa4db26bda52b004c4ccb2f387196b8d87300d5071daf89554548a294e835fa" },
                { "hy-AM", "06be48ee86f5159832415c18925636a46d694d67a1f368f589039b3c585fd76017ec05a2040f9077c568f5424e5a6d1e31896629d86f8390a0af310e49ca2d74" },
                { "ia", "4a71e96bc752fad4ee2b974157079222666b6728030f4c064dd468e09c47292b7b72d71d0cd6faf57fbe7b87e0b7fc5f5bc5ff9abe68d2bef6e7bdf20d71961c" },
                { "id", "e0bb8279c41ffc72346c9eb8403caabba5e94c1828eef9485a815560d06d032e2ccc6da8f33d69d0c756112502805585006da5c8d743a711e24f2e50cb9c882c" },
                { "is", "70f77949ed803be3e85c2f884cb27d6259729aada022ccfbe686efdc50c86bb9d0863cafe901844d19ebfff50d206b6a345398ac3facd0110b54e8083e928ae1" },
                { "it", "dd2954f908a5c52553cbfcdb06520f7637d7f175a1919843eb617b0aeb9103b738e58a0042e9b8d8477f3247084776beefbfe83141a52481fdfb3654ce61b537" },
                { "ja", "ad7a6ab6c29fc4b0b796783a56eb83862b828ab48cda1ecb03ccf611c80ea84e7746e10e8527d7aea2dde531aa36a822f31f68ea796ac3bbfd98be07cef3b03a" },
                { "ka", "e4cbc88b66a68aebf063c6f51c066b40e8dfdb0ae3754404010c103f548e2e1be173ed5f50ce3bdd4d2ef739606d7db64e7f740e09ff393af2572b688335dd2c" },
                { "kab", "6bc04853c7ebc377c44df71f051d392a1f0f70844768f35c6e87832bc34a665849565ece07bd861c45e84c5810ecac67398fd7e3ee586e5d35a91b00d25083af" },
                { "kk", "77f42b596e9ab8e96a181924f97a66727495bf573612166f44a352a43426debc7575c768b10b245781eb8c78507f87264c865f3f553575c585734f22e1209d8b" },
                { "km", "4d595d1ef1ba833c0fdd9cf9311e06a7a81a17df654e19d3716581182983975c4f0ac7a2c02acffff776e1dbe671e3f8a275011495f00239e7c15891d7df2b56" },
                { "kn", "4efa007100f3d21aac1d6e3fcea0bcf9c1626d14c30ce75e3430090226f80e3b0d50f4f0a07e97363d1e0ecc71419b8754f7de3ad16d20ec6a249a315ccd0e54" },
                { "ko", "ab97918b8aef9d2140764868911eb42bfa5ca3344fddd9832282541525b450f2bd6795a49a1568abd99f78e82e5c3dee9ad5f9c57f5db130ff3d72e469679787" },
                { "lij", "ecaa8104eec22369fa1fce09b729c540b771e33935caaab3659575b4beeeede3db47460a10789daf9d6aaa6d347ed8d5ebfbfbc8eae0dd6abd4c616e1a2c1ca4" },
                { "lt", "cefe2d68e2fab05eaf5b0581d45d0a88d1e21d88f9062d7dcd1d4f6e14e2fb6e368b6d42e73bd7adc0e126c45dcc6124693e527c852415e865d21bead3320d78" },
                { "lv", "e3984b705576ed2b8effbd7ee5ba6d2f55eb2c509c870703faab22e6be3555d2f7b3964d8aabe9d59a19d3c4d58ba4d9bfdb447e40e56073fc6f955f02724fa6" },
                { "mk", "694d6e25effa272d20d46f15fe4b42b44650c02f23b1c615c95189becb148cb8611bad92543f0db7dff916383fda714aedc5c5731041801f7419fc1dc03dff98" },
                { "mr", "25da4d72e34cec856d99ed2f0a74f0cc8fdb8ce578f3eb682c045a2a91d76fe23e498ad062a967db1091ed2e0a3a424b346c7674e572d9968d114ccf07ae3aad" },
                { "ms", "6f003e115a085b4cbad996f78c41139f816654606b3ec7a9fe757571c12de29677d1d1a6d5d66e5162ccd4cde0494f39cfdfe666c6bc52c949ddbe778b398760" },
                { "my", "df0f35fa74822c36ebff8ac739e72afb5375cf5f716bbfd2361464dbf767afc05e8bf88c585eaccfd57bc6a74be201e6ccecb5bea1855157a11e0b7e86a8e900" },
                { "nb-NO", "cbb129d178c0a90f5fa001841089ace006ac82a4b72441bfbd4bb99ec6277fd1255eb1f5e754e4c90d59e258ae4396f0311afa2dd3ad7ea1e45bb4da33a861bc" },
                { "ne-NP", "16520eca5003bbc0370ecd74f4da16cfa1a99e378b816d3444a172cfaea765692a4f5e3db1a78919ec6eefdf5ac8f25720e4a32de0107a61ff59983e285ad9cd" },
                { "nl", "8b176c55f1838eaaeefeb436a2fcac21e9204cfc59a0e58378b8751c889a0b4fa9c73dd0c85d0453e08e2abfeb559d610b6ed4364e6cca4233637c81ab1fe699" },
                { "nn-NO", "a68615c652cc361d49d428b1c6194ac1e0595c38c9021e0000f3f3819295a03994dc5f8e1421f3b51f280cc3a18bc858a2de4cc88c7c731603e10570ef527230" },
                { "oc", "f80c74038adadccfc5d593cdf72e0925f4f6726531349c9e9fac011fbb83177cd966f599a97e26c31efa116727186da91d999caaa1a2a8b2a1a1c93195dc1dc8" },
                { "pa-IN", "62dcd0f0ece94c8e783db2bb12dcf71e67ff980965033be7005778f07dfdb0a122a055e319a01aaa95d5ac6a32f8605fd53c020883f7a5691bb1e314b22a24e4" },
                { "pl", "99ec53e37e423859cd0c151d0f687dfb90e2ceac39251107e0f0ca3b7e743183c39ae11ea82a829d885dbacc4c88999cc214bb38296fbd785b92343bcef58a6b" },
                { "pt-BR", "bfe2f1a419b35b585b7a277f04c73b1e66ef73df3bfdd15160cc523f646951d001f8c750fcb4751f219858c8e923986667ca212dbc61604f7a4c39a01b53955b" },
                { "pt-PT", "822e5c242b172a0757a3f77487b1b3786ddc3f6a79c01d9efa2e978f5684f13e4845a7e310e3fbd9f5b52e2371014b8cd8b610390f52a5eda24e7510cc4d4866" },
                { "rm", "148fc306eff6cdce39b58d70d3443df1637f1eebe5b83f2aec050601405d28aa3f43cfed8655f23f869d854b61a82f02e2e490fc20ab23826085185e55e38564" },
                { "ro", "70918a8f366d862724d6eab21cbfa9b6867f1228bd0d1d7809e91235f815d712b4c4680735a6b6847e9bbb68bd25b820169f4a05a6d504ef5a57d3f12e759d5e" },
                { "ru", "6ed7ddfda7d98e6ff7013fc9f2e7170a321caf888ed30da4fab70435e3d0df6897c70fa6707f54d30b4916f145a8019eca8033e9d31fc79e9506b5a6b194900a" },
                { "sco", "b6453bfac3af8f1d939622fb29fbd57aa69bf35923cfc68a35e0941e2da576b5610d32bac8963a093267fc67b000ed303802b5343ba0f29523386f255d36e37a" },
                { "si", "f09f2486967f591c342178c9266c8b861f4f21ffbb251fec0d1e4779410c2fe13d4e368f117d7136862732e0292fcb6cce60429f9e63ee651ca76ae419417705" },
                { "sk", "ec979fff97f111d9e963628673e359de56c207d161f8381cc0fd993cb9e878ee42d8144a0b14be8c776422b3b4c0bdba5b81eebd207f033dd48ed7de2ad795c2" },
                { "sl", "84372e74d9fffd9b45daa9ef0336e3bf2c8c430c09c6a3b7625b23108c98fbd3c90afdb2674f5e3f4fa236c928a4cfa936be8e16c29ed2fb65ad6dd289a5326e" },
                { "son", "1c4a4da205bc4ca4d31f5f8a8f7ab65010c123d72fa2966080cd1db4d37e827ceeeb21a9755802789c9cf0d2dc6161c34094aba81eed625bcf5cfea057421475" },
                { "sq", "cfed858eced9b1df013a4e010061f98a799fabed0429e375d85f18e317a4d8456ccf280c05817ae006e3b2ea813c77476d6c2d0b1a41d948f37d1e3a2861f2d4" },
                { "sr", "7c41282039082337f8398c56a6aeee592fcf445cd7ec6e07ffb99cb7987eccb3bcd70f502513bb6355e0985281ba9601e813a04d99276255109c775b97887c28" },
                { "sv-SE", "83ce2eeca9c0f6eaac2ab96136710b0c7b907226e0a7a9f2afea8234255255791a6b30e4f8e5089af74345144fc8ca7eff35afd9ef829f93d557aa68904560c8" },
                { "szl", "1ab4fe917718471be05a507e60eb5cc0d06b5091ed6471b2a138b05962aa953d9a5441fdc22c6268826450ace3afa62cebc48a4eafda4ba540582a8b61bcb79b" },
                { "ta", "f492886e8526ea3a5cac323176c1e71566143f3910ca8a0ba1ead4fa9372bf9a09dea20fc7015992431b0f0a51b75e675e4ace055fef2901c3aa057e46261648" },
                { "te", "4e2c388495e10b825451110b9ec4883f1f5aa6097e0837e7277771d49b1b35d96f35e1807a8d94d7f9c4d806bd6a82e85d9c15c89d1be543b3bd67c42e80c148" },
                { "th", "55ff4c257be8279ba3631a032980f15d0567d8ab6d73e95daa15b94cdaed4d9cf72b6cd72a888409b501307103b8e3a0b1f99c8caad7c2bc7103c26f09f3673e" },
                { "tl", "3fc5d3a50deb0573ca336fb7c1a04403a02cc4aa3edcdd4bf05b4dcefdd4d34be9e13fa5e13a93a394be2553823b0e5ccf8c4b1dda6bc18093571668c5f03faa" },
                { "tr", "9260928e2869e808f4b0b8fd54733e5bb769d1c7c4c8d08ab4e3d5ef3b8a93b69dc3d50a153e4ce8df532dc13306bd4ebdf93833ace933dd5076a9c820fd55c6" },
                { "trs", "f3fa77760b3a3618f3107398535488f857b6bb9316e0c377c3ee676be52aa1f6c8b5882663dd0df9f1e6e186201a2f4c6afecbd6465300e997c30fafc4c92a4c" },
                { "uk", "3de4692971ace0218fa66d3129e7b0ba7403c6d90e89e20f0a5da2570f3d9e067ad58fe38058597b461b692ad99ce2976b2c05e7ab06d09a6190958a149b4b81" },
                { "ur", "a37bd6dc61d17cff1f3e5a9f48f829beaf66862cf807fc52cc4271dac51f1b7ab19f9215b375aa7ba3a7e0b60133cd727dea0dd779ef489289d86e3fdff959e7" },
                { "uz", "c1317c02da8cedb81ccb4370d32c4372e8ace60e075c503fdad83a171e313c977e6eddddea56d8827f41042376e44729fcc51e8d294e8ecddc8184766cb8f308" },
                { "vi", "fde46cb09f52dae5ccf397ef3f140c0ce4f9fb6e6f4a0839acf84e1d8abdc6eb42a8b3b13a6d2761215e5e5c043b5a6d2a195ed291228b8097d57af0f841bb1a" },
                { "xh", "ae49c8eae6aa229d2a523269c918b41b9f95bcba43f6584851f7f7fc6c1b1a2a66838f53389600fdb300e7e5c95ea21da72559e7a24364cf7df4d568a2421557" },
                { "zh-CN", "a4bd900251676a27b47b7cd90d8bb829ece4938459cd70294756dc2023696ded69d19231e91bbc4315b3a66ea2ea19b8cbde1ca0c1383ea6b774552809ecccf1" },
                { "zh-TW", "f87f8504aa796739aab577b47bfe10ca299c1920121a9eec59105a03321b3f31edab266d2cb6c7bcdee23cf5c6c8676f57b4266448a8eab2f80632edb887253e" }
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
            const string knownVersion = "107.0.1";
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Mozilla Firefox (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
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
            return new string[] { "firefox", "firefox-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=firefox-latest&os=win&lang=" + languageCode;
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
                client = null;
                var reVersion = new Regex("[0-9]{2,3}\\.[0-9](\\.[0-9])?");
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
        /// Tries to get the checksums of the newer version.
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32 bit and 64 bit (in that order), if successful.
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
                logger.Warn("Exception occurred while checking for newer version of Firefox: " + ex.Message);
                return null;
            }

            // look for line with the correct language code and version for 32 bit
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return new string[] { matchChecksum32Bit.Value[..128], matchChecksum64Bit.Value[..128] };
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
            logger.Info("Searcing for newer version of Firefox...");
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
                // failure occurred
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
        /// language code for the Firefox ESR version
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
