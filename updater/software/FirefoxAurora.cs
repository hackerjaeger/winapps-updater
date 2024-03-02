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
        /// publisher name for signed executables of Firefox Aurora
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "124.0b6";

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
            // https://ftp.mozilla.org/pub/devedition/releases/124.0b6/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "6f23c662bc64ad060d6e6207e2a87f81903f54b715561a9321f962a7aff41ea61de231965c5a0b7cb9215c4115847c6d52cd34498eaf410591e442c7c56935e9" },
                { "af", "9936562ca583cae94a54bdd9d38bdad49576d11afde51b29564cd5ce6b4ee84c06079f8c05d6944e7550b4bfbb9e152127d5e0f317a8220eb32131fc63cb19a8" },
                { "an", "48ef91aa3473696b3e36f4792a663a2e821ff4f77f1893c98140d966287a4b938a766a0ad1108136f9c06a7185a7aa727120ac85c83e91962440fb70ada3e84a" },
                { "ar", "34afbfff18235a0eeb8c6cd819406f3b2689f2e7a9af26cccfa362c9c663fde32f4f139f4f9c1e462d4ef9302ef1982f1e105a6b9591c266e6adf0c13c97d149" },
                { "ast", "b5d29e6fd9c0933e955c501721079c88651ae4f1f3d22e484b0d576c4ab24eec8231ac4cac34442f3f5d5cd43b6e1a1979312b7ea3d56045a7fb6479ee917b80" },
                { "az", "4806d2e8a19d448f7b735cdc4a6c3235d083f530a5df2ceeb10df1c11ef27afe3b916509a90c54184737f0b7ae28dcf72f423aaba136f7414b0dfb46acbaca5c" },
                { "be", "a261240f58f960751047770cb4042a550a0d7c839c2955389ce4700a150c62eedc9119601dee5e6141870509b365f137e498b0d34b3897db7766217c7f1e346e" },
                { "bg", "3173a4a6530a3ad544d011749d8ebfa8b9f9e3e93d4234e4b6827172ccb0f5c1c90423bca1cf77ac5870bc9d3887e0f4d5f63a19c9d9bcb7677f022c8900f76a" },
                { "bn", "43153b4d92b149a3ea1080baed8eea316cd69ec8b979c70a23427192e96a2eb5fc1fef39632e00c227cba9e579bf636f18ddd7e3cd8c9b1dd1133f02356fa32d" },
                { "br", "96bca0529e9764ce091c87af2787b629fededc21838ada36e00975b359107e6e2007909eb3a25b035d802388a8bdbbafa3acd1566f162c399963ec6f56894afd" },
                { "bs", "8aafc22cd5f92522d3296c607bcdc3e91b2b1ae488b55b380722c0924e82863adff591ef7fb54fe9e970b0b14567fc3f09c54ffa6995eaa0d3c26f0f7460f3bf" },
                { "ca", "f38e6da643a6527aa21322fcb704e23eb4c85cefb43d8b48e393cabdf4e9f95128f3b20599aedc1ac72b28fe87d80e621882eb327ff3758e4cd03a1c7708d699" },
                { "cak", "6e3076143c00b5cbe679d6d4197c90a9682db6d88e24b582aab358a9d261c1d830bab3911bda3acfe6784cd399b718f46599b920343fb07fcf7d5d917a94b146" },
                { "cs", "1d7c453314686decb670c542348bcc265f3f5b217b66e6fb49b978e7cb1b1788489e0775c79e2ca40bdb0970b8864567c3fab785c53ab4eda927e1697d634f34" },
                { "cy", "a30290f5dc2e69319f0969e13d5fa519738494eb41b83111b20caf99141fa3703df41f3151e2dc54ced8fae94fb0b02e5e7a1bdbafa23b8281b4d854a33d22bc" },
                { "da", "3d012f1f65e3ae6b20e1a7f409b25cc0b3367a7bd0f23bebb372806b459c91e98fbe7c911e18e0db83b63b8772a634b93813b0eadebe31bf3308e25b7adacbe1" },
                { "de", "0290d0302d3ff2d8edc6aafaf189dff21a886688ff28aff07138c777b3eb2d361a98c7bba5650336f8df167122624e20bda73a79deb219eef8314fdc2ccfe1a6" },
                { "dsb", "0ad6396171bf768377227d3fb4b9314d501bc51647fc75a7756a5cacd80f9d6821c1afb9ba7ee62a891ac0fe0a7a1a08cf1dfa701f8eec688303b5182b726114" },
                { "el", "1006d28b9b48aa683c73a0fd83d9aa2a9c8347f05814b965af239d057d360a7b44157395816b3b4ee171d19fd7b316d5a77d7879c1b72cb295c25a2ef47e6a82" },
                { "en-CA", "43e5a86da3bd9ce70b16cd43457f9a1b7046d7141e7a25f34a3786b73d2034f86fc9b145300573a574e13ce39edacfde3a54843d950e8f8da0914234494c569e" },
                { "en-GB", "29e510ecc7e0535425f65b21d6b29c151c785053bd86035440305fb2cc5236c91eac6396fd68d357088cc5b26cb79be29e8524513340bf1d771902d448994612" },
                { "en-US", "2fdb953470660d9a577a647622bb252c986c52da0bb2c7894a448c4c032385f5e6accf7793a60ce282cc7c39ad8ad42aaabbfd2a8b97ac11f0f31ec887ec1e81" },
                { "eo", "f558e2aa8c2691fb842633fefb8fd23835a14ccb33cb1c6350623ee8a5845b6069da9b7e868926b04385bef963cc9ed0f54456e653f5f8770d4bb8ddd2ef3355" },
                { "es-AR", "2f0f85df077cda84f67448e036c1d5ddb66920bcdb9d6cbda684b3c8ce36fbe13a0a0fd0ebc1e1422cd19610e060ca26c855c286bd5a0dd335f5f70a81b80714" },
                { "es-CL", "169b0060ea065a0cdbe55f685c73814dfbf1421fcfb2159c22d6884a43f7ec1b58d996590de1cdbea785aa7ebc8f0c247e3af923da5005cb7819e9176626d61f" },
                { "es-ES", "bda537cff511c76081974050e953fcd96b45a772d01c5a03acfbf14eba1f2f29233d6ec26c7b7ef1d061fc7c4dd0011ec748dd1ea5ec851e2edd950af17fd786" },
                { "es-MX", "95a57c01f2794b9ad3ce7a41c6b448048007754afb3cb093de6c3be6885eb3e071fc342d92bde19442fa4aa3833d25f1ba62992b3918962d33952ecceddd4aaf" },
                { "et", "cef87a1c60476ee3097112264001d9900e49d1a8e1f8cbddb8385ce28c20f196334423a7d189403596dae322fa0a146bd4bd9c6a562ae7c79aa95090315107fa" },
                { "eu", "ebeffbbd8f8a5a85b5def0f37f5b771a5f766007a7932b6ece8816f5ea1303c0c3cb584032d19f8e1e24a60dc449c5f02a7b0385a646b0cc48d24bea20c044d7" },
                { "fa", "6bee0fa7e3c586530c046c73d95d51053e6cddf6c7519747561817aa55328116c9befaf8be0f4dbb75b628b0d118977c0bf9df90cfdc09a2802e1cf420b9fac1" },
                { "ff", "1807018d33173d81d52f9f07fc712a6ca619a354d5c777e201c83c4decb9769c53a1a060d7c62ac0d4757ffce427fcb1ace8ba590d4e64b5bfbaab2b17c1f525" },
                { "fi", "a2b9ece7ad0191d5dbffbc96528c98b668f4ff305fb65935c76786335d3210e5d663d08711d51eeddbf8b600d5f15dc80ea483f6f9b5f5b5aa9b4ba5cca7ef6b" },
                { "fr", "b6df9e97bbf25fc31bde48b55eb4bac01be9bee392cfc04ca975b8edb2445eb6881b182babc6d771be75490b3e72cbd2cc2f0e073b6d381b77c64fe67ea162fe" },
                { "fur", "6ec594bebadd17a774281c8024a0c33f52b25d3965db97c7fd74a3b6c010543716c90fd97715940d90734e85b9f1beaca157d1ebe0f32a39b78d0efd15cf35b5" },
                { "fy-NL", "32a51bccbb8470231f960b55beae54fe5bf755935ecc5dc0f2e4a8e50acd1153e7d46dcd74d46c29e737b71caf3ed3a63e443e15406d51543397c97455c8a353" },
                { "ga-IE", "997380696ccf7b2328c9ee2c4dd50e4a4bc2779eb0990893c251a60c34a7a387c923a95eae90b7d6efa8ede378155a53d41eec760b53f697c55c30bdb5ea3c53" },
                { "gd", "5573d361962711f1ecb231127a6dc2188dc21f68e5e1fdb5f827e8f85f90544c2d425de15895d35a91797757c93fb452544cf125b6b3c59f9154ff5182d587f8" },
                { "gl", "3d53173a84bf651258de50b9dd30950612606c849f52ecb6c414dc77511ea40261c2b7986559a63328c4a4917e36f5d464fe345cccc260adda39735d2826e21c" },
                { "gn", "f9452c24d61904ae7da7c34ff2e8670ef6ff30bcac6d5c2fd04fd85c65e8e64684ffb3c36609f3d02c95cbe29f82102a31d051d0324388b8c15bfe35e53b8618" },
                { "gu-IN", "96dcba2cfe0af70ca8ee59956dcd4df7e44dee3d3aee154a031b806e62b67e9bec7977a4a90b5e9c11c099ae03454728f7ee6ede93fea5685d9784658d53b58e" },
                { "he", "9ff508b04ed5e7715eb14a83fd0710a7c93be99af3889126e6ec67c9dcc0719b7d9d179fb974b0a81911cd5e5f46fec6717f5857f48f4d7a34115654ae451f31" },
                { "hi-IN", "113bf92b2bc16780c7d5397028ddc0e58554df2744de99b56910bbfaf43edbdaff79464efc3b6e8be11c054ff0211138cc90545a234bb1126267045101676b5d" },
                { "hr", "eb1d4c276c38e1bbf701b69c306e5b1f8e8976be8b4f4a876bb0a74a0981b751186dca9578595cde08eca0e082d9891aee071c69645db8cf6d75e3a8ad0b7b99" },
                { "hsb", "95e262ce3161061bbc3bea7178cc8dbea294d0069ab49a17df3183164254f8094c68ceaf8b602b78ac88f093d6ece863642dd17f56eb6dee8721bd957a8f8eec" },
                { "hu", "ecfd05eef182d66a4c46f588c21352fdcfc107ae3e28b8bc441729500023bccfaeb2e43d8036d2c66408c9adac4c09b1287da1da01315685fe1d5a9139667fb8" },
                { "hy-AM", "4e71eddbd4532e304b5e4aa08a28ae2fa9d516cfbab9591e69cd11379d4c025a8e3591dba7f7185c3f6048958f123491772738692e12a7e4b1c100a3a15caa88" },
                { "ia", "bd760501405a5f1d582e432ab48f8fd3f01ab5f753594030a9b2029a7e70f2b0b0a01a488561d63f21b5ec5817a27637364aa12878f7c65ea83f33fbe1807410" },
                { "id", "554e48c5ee053141ec9d25a50b1ae3b0ef9c54e8978ea82545487479adfa1d086a2459089ef7ddf14b4843c3db2a49d69c098e0d35c1499f185b985ef1c12962" },
                { "is", "8fa4578bde5e254735eae895c1321a8a8e7e3da04ec8a09a14d7c9a265dd61e1ca11702f7d0bafcd815bc2e69374562025688e54b592123dce165fcd9bd1f14c" },
                { "it", "f5e6609392173ab96824e97ce8f9f8ff1637ac283dae402aa90cba348ee441bc2de8e3aff90c4c1d0ea0a0cf0539751955f013e79be38f5c7d58e154ab7b7125" },
                { "ja", "822070c17ed4cc606816644d2b77980984a69261ba7b4a0a8a48407387b683f305776437399d1344c0a3b3222b4b0594f6f2b924208345a97a9072b3450d65b4" },
                { "ka", "3cc73344a38f1a6efbdb4015d0a9265ef8be9e207b67e1e2ed0d5c5fb4c4990738569274070436e29e100f951e3164fce2e9fb58120f8809d548b154ab2e5432" },
                { "kab", "6fc8bdda2b9b1bce6158cb0a9c6ed0523680760551a38f75f54ef5ccb3342c18acf6c3c39179f3f04a467191e3d53ced282bb20eb2ce659450c68a69f81a2cfd" },
                { "kk", "637fa65e68609d91f7f56797da4ef45b8520831d7bda6d0dee069a7c41af0dba70eb9ad7ed79d9a0e0e9e5fcac3fa11191e37f6c7da9b96fbacf93379e8ad2ad" },
                { "km", "8ef51b5399f1d89df5ace2a9d4c0d834316136b73685ed6e2e27dfd73446c1c4e915a8b87cde5d1f76dd59877f4941f0996741d418f54b34096076c533ce3668" },
                { "kn", "f7c0a2c55bc1a2a85ddd33335fb1f0523d3814aba4465d30327bb9174ce2b11187a583ef725feb5fa704424ca40a5633350b16b24a63f4afff1837b507f9e9ba" },
                { "ko", "3b38f0e35ba8fbf1bb43d6a6ba6be915aef610b0333f632565c8ad28cf2a7becae814d6556e2f922b318146b4bb84da203e2623901d57a0d2f363c295803bb95" },
                { "lij", "919018e6500f3c9d80e1472fa0faf7f10db8fbff459107fdca20850d6196848bfa7468eaa15509c3a7771530f961377193225962235a3ea66c0af99f09ed0786" },
                { "lt", "d627989ac821b7f4b5150e99b353fb09a54007e1734c445f3e6fbe8cb35cc5bc12f69ec23bc12d7677bc6b9fca81847066825ff94643989edb8d930d9e75503b" },
                { "lv", "60e080047b2f6a09d29ab0a46a0ba7231a59c6d8bd4c34333ef6c8fa93ecbf425fb6ab1f6d64b3afe94a1794bb9d9a4e2d528006e380659821cf40656beec517" },
                { "mk", "fdb9dd4addc0503ed2ecd2498e52eb3b3069a0360ca97a8ef5253ecdbaa0228fe910c035bb8d8c713fd297b355b480f0677a93ffeb7e6e40aa66a7a85cb2f229" },
                { "mr", "8d6e116b88daffc15937d5118f0dd9087bbc6be81a2889717da1f45d8ecfbf8aa81f7f58f458ffa2798ab220af58e85cb74104f4ae918fc30744feeb9a59cd40" },
                { "ms", "886903b38a7f41c3b85dc05b9d2bcaf691557411d3aefa5764f5efbd52168f50beb8d8f063c51aed635afcd90512cd53ff664be4a9816bb4d74bb27dd4e20727" },
                { "my", "494919473f1149c615a463acbecd31ae6b608eefab587fafe191cd170dab8ed1def848aa53de780d097eb0a8c25118dd9a8a45f88173947e3b61a037f84031e3" },
                { "nb-NO", "774759946273d20bbaa3f0afe34255b333d2aef37fa6f46a64aa26f7015ef763569dc4dd6a0e2715e232294696ba2bd0101ccd2c42250added8d6dab386f2309" },
                { "ne-NP", "cf554241c97e9e6359832fd10ffdf864b114955401f01172fd7c4a01d0a9108946a9893024cf326bc872c418a811825bc386381ba492eb5ff46ee5c30e055f80" },
                { "nl", "5551ae813dc724e466eb125aaada507ab368158c9cdbe1ec9d5f12505042216d99f156caa8aa107d4bd5af66b51564d2a1827a8f031f81f50beec8903d248d18" },
                { "nn-NO", "c65a0fcef4080baeac9d2e5562a779ee575be252d566d1f530458ad1cd0981185c582692d053ae3138bcd05f1fac50c3a1a1c841c724a73cba9b235197bd1115" },
                { "oc", "b81e1ebcd44794ac5cbf18a0fbe42bb78e1daeaae4044f07fac3742024d59561c2bcc9690b7c77147ef31f1d91438047f6e13fe93bd757ba56e9a5231508b99c" },
                { "pa-IN", "81589919e4bfb55d4c0e490aef0df02f833b454818cb9c787fd8c341bcad04aef0ae42a52ee65642c954f923cafe17a8d1e11ae538d35fbd1b4301bf10b3cee6" },
                { "pl", "c1a86b410b526f34e6c5381651d30afce4c794b0df36927f198ff849fb6bc052b65d4827351a8866982719432fcefbc400c86478a941117c8bae614393751296" },
                { "pt-BR", "458b4249bfdf96727097611f09ba39934e8791e11bc285e9a77a98045796c85d3a0997e3b70322646663d177401d6d86c32d5a244479ee363d71b09ec9bc17aa" },
                { "pt-PT", "cf433379a22bdcc40c6dc001aa9872a8f505f5e688c59787e22aa6f42ef83966da29f33caeae022ef3c86f9181fbea4969fb560fd85a2c2052807821836f4bbf" },
                { "rm", "e06aed6d22d9f68b9c713d32fec968c15f223a543c9bf57d215b6923f193c8340d53f4bfc6a7b47f6c3e829b3529cd095000365ce43b8ef157b29b845ad190f0" },
                { "ro", "3362a41066a214af6fdb1aa1f0759295d5ae891d44b327489a29f532b1478c84799a8a303af8004e4a35637dcb574770de955aae1f56198a1ee646eeddba6ab0" },
                { "ru", "f85959d6ebac98fc10beef271dfabaf8a0762a16768558543929adc0fe7ffb8cb271b32ddf14d16e9c263e8c1ebd0fd6adceeea69e6c3793c2ea4b7f376e7bd5" },
                { "sat", "fa23142eb65a4caa8d88ee0d0cee1c18a797fe0b4a8af26c8748d5affd1f430095942a56242d481d72ef10293f0f60067acf3e1c9b4140f86c9d1c75b676b95f" },
                { "sc", "12755fe501d694a65d4243d92220da881b09947306a5d8c4ed3bd9c7b10a5cb156a9b28e6071c45d8aa044d3fe33db1181a66603e24c69464bd994d22453f344" },
                { "sco", "d74a1ef92aa4aa5b7a14aebe60d0da4d794260031b22a6213ff61c89a14e4712c73aeb00366dbd14e6e8cfa006074a67a339c3d3eba16e101c16c6535201a86f" },
                { "si", "9d747ebdf78d50a2021dd6b94ca5fc6ba0a87637c69bce76cd6b956d9aec42fd8adb6faa1469eda2126e06cd559bf67bf4702b7ecfe82b259fdaa244c3a15d68" },
                { "sk", "7accefa9b0d614445a3070670ad24875df214ccfea72ffe620005c73596720accca6b24b462e48f1c6419415ed8c3b190849723be184b55eaaa0eda629ff2f80" },
                { "sl", "7d027af6baa3a9c9413bbc1721fbf59313a9912a9d30270e43666dd151e5f1acaef25b7a584c86701944a87daae76b809dc42bc45a49244f2fdba74053ec7aa1" },
                { "son", "a1eed91f0c0eb9dfec07745d133c9064d705f0d79923bb10f2041db9b0821d2b7d06400cc56745c16a9b17164713198b90ca2850379d774267d37af1986dceee" },
                { "sq", "180a7ebd5880b71c1d17b424c7ee6ac48f6f8113f11cc5e20bdf1f5e2dec71770bbbce8643ebccb5db375290e8138fce1328aaf7625543e21610bee9080e2d19" },
                { "sr", "5be33159813d8171d42c576569c3ccd1b9f471033d58681fda38b130027d9dcaa405f0c3cd5ff4f2135ec059b68e611e283c980811a02f1a8169fc420f8f51a6" },
                { "sv-SE", "ccfa3266db9a24833f32b14f7a1b54f2d0b8f5bfdb1713e25cae9f8b656367cd0dba475b21b59ec7fef29941d0c392e25d6dbd097b96c01ce7b0ccd95c54ac8e" },
                { "szl", "a851026acf9517f47600baa0091efa5f01c5572380178fb2fbd2b2de4b5dc7064fba3640f4f7ef46f38d9c5ba57b67ab3d328359ef070aee8ea6dc4862e500ec" },
                { "ta", "9eed08e745c331c316f868a90f8fe8b8fbd50cb75291ab0749fa7831fd7e684b78dc96f1010f8d04f3577c37a8bc79e2e9f0e402071b2c985185c66e14b6d72e" },
                { "te", "f3413d0512c1979ca3c597c28003ea834627ac766d092cff7284e134ea212b4a66c53987bc01579a0e22b941171d8ec4a00c9f828f2c076b0cc47f0eefc3cc0e" },
                { "tg", "c08766bd79b603a5989347a69c25d4a3d6692fa1e09ec691ed16d2bc96854aef3680b56446c5d7a75dfc3b5f454fac14c43ef21c86eed6bd01c493c44410fa39" },
                { "th", "f39712f1ec7a4ca86d8f30ae408ca700401e4bbce0dcc46d04af0a611e9a5ee303eb0343d9c0cab8ecdaef8976e3da2ab4afc2af4d307162c7501eeed37a39a5" },
                { "tl", "6a36b7f0a08fbf7f75a9ad4a4488afbdd9fee13e774e7f81da5423e0bfcc6a651121fc69fadebe7794078632a2418eae8e3dc51925a05bb86dcd7fd04b2780e5" },
                { "tr", "d54948222ca7a7b55bd3bec81d735a608cc10a9ebcc24ecb770a31c1242ec8f3117cb7063d58debcce595041187407f8c117b8f73f0ff724612163633420876c" },
                { "trs", "5b8e594898299c095fdbdf73045f5350dfccb7f7d5f097e0d8cc05b768ca3cd02ad11d20b8f39d8fe6a53145df8d601dc424e9a2069101001171d2ffbb2c55a0" },
                { "uk", "50814959409aefe903ae1529b71dcef0925b27b0234a05776a04d05387c8379427b6c7b2df0d13170eca5d30d7f9dcbf9c604e0b3ed242af802988ec07205fce" },
                { "ur", "919891fc59a860c8b60293062dd8fc85aba2a520ccdfd9490160c813257a9ac6c6ab6db633603f9da832a0dc272108873536e126b2f2ca4f13908feaaddb26c4" },
                { "uz", "9070b7e1bcc2b8ca1122bf57be24ad0a8a478a28460e7867692c69435999e0b9366f59fca8c1326c7d8d82e54d224933e49726ad4b73d71b90d5f44d52f20653" },
                { "vi", "64f11c69c4c16fbd289028867fae905c2892f64393f4d0ddce586bc5f80431515dd375991279f7d2de80fd54e9c3613ce92ed42cc0b640229d2a4a990569e87d" },
                { "xh", "c9764bd2e39b615c47510571b9b8047c04e1560284c2e33c9c67d821858c05254a16a675aeccb866a3c56654ae19df7c3a26cacd0c9ccd02c682d825de54de0b" },
                { "zh-CN", "cc3edc7d86d951c38b8fe65ac2028af948f4500d14e58b26c0f480229e184ec9b52af8b8b543232244f89b1a1f3d8752009aca26f2ea8c2673e01af83c581d2b" },
                { "zh-TW", "554443881867178784e88eca84a99cc21231592fab49ffb515e31d879596b6a818562ad43e3f2226a2e1bbb4ac0760e2910e95e3adec6ba362c68e4baa57c879" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/124.0b6/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "054071ef5c8f56a219513f012ed616f0f8579a4fedafcb919a0bd2f8a1a5a25c7167d10b3e2f5721ffef62a2fc10b65b8d4f946f3aafce36a436c1d0f1f97ca4" },
                { "af", "8ce6f75497b7721d34f60d9edc54a8edd94f852cfdacb56a6926e02fa1c425ffdd49d83c0ccd6dd28e378ead1761a057d3d85ba09674bbbd69c8fcc5da704b52" },
                { "an", "7c0f0b3517ed845b836129cbd9dc068c3766cf6de022aadb28f605b1430b68a606a688ec9d4d71307775272bdbcea1e6bd3bac4bf0396f9214d27eb54f406778" },
                { "ar", "56d4799d300a36d0eca460c303fdcf21585199fa83476ecf9b42dfb0db975d42b2b6cca5a87bed171fad419030062ba890c8d336ce454ef0ab757ad4403c7c5f" },
                { "ast", "2c189c3db1226b0e27f2e1e661c13a74af5c8e0862e8fac5a393682d3ae07ed78e0b9748a0c83c065a0e076592f61d47c1df5a1ab61715678cbe5e1738ad3708" },
                { "az", "5c0c620f3c66d7206501b9d79f039e5fdf104240da6b07db989ad63f1e4abe22ab4b7c887074608db62be4942794793fa46bef8a389f3791de0742e81816c549" },
                { "be", "0028ac1cb662ab96c2e1b4f296cefedb229a2b8773e67ff63a61bfc118c1c94b616616a0a5695a63b14ca6de88cb011f9e9804bda18e838a0c2a09500897966c" },
                { "bg", "4db47d9c792bf285f8048af8bdb2968303b88f507c1640f7d83cb544f11093faa892d81cb34e79adc8eabaa03a8e94675a6dabfdcab03607ccdef2e5ef625b43" },
                { "bn", "3ccf6c822df3e3c0e524cc5235b5258d957cf28e682a1ff5ecb44cfb43561609da9ddbf3c8521604fc86e441f0fb3c6ff159a1c8b52bf73368b8acb30860a4da" },
                { "br", "310809f01e1b177a8721511b6982b2cfb98310a64243716009c458fb112a159fec06a8a8a170f8b0a2bda5c3d7207b100a4723e346d21bbcb69086a74d35190c" },
                { "bs", "c87aaa92393a5be0b57eb03827da3fabf7abb08ee41103052478d6feef876ce3c4bd30d132986f6dfff89764db46ea1c125867b400feb86b0686df17f35939ac" },
                { "ca", "5befed096a03c3142c75f8956482b7e95d4a35ea841aaac4edf20ed21f20b321b25de6f9e03c3cb814ad2a8d188cc7a028740d223a931a61655ad3c228e22229" },
                { "cak", "20f48c2f4dbe2430b872e799651077b2eaa472b034fc30710d1b3098ea824ed2b5924f70c52a43874187f9961db72a05e1b3c727b004a6341bcd45c57cb093dd" },
                { "cs", "84def7acab63c44d1455bc8e772eb83639ff01054352f50f69f7ac1d38359ed1239e55a2afa32e2368d886c4102182d5ac380987cbf56beb1a86d6f77e1633cc" },
                { "cy", "51627b73b2c15000bd32c4e8ad861b2a79535d1910e84b179177b26c4782190ef083bff329f6fe8df7d234709db9c033b197a1fa893b40162eb97466873bb163" },
                { "da", "9b74bdcb0af6fce2e43605194949ef726ddeb1d834fbff593b100398d3c28e8644d98bd86bebaec8d73b2e0ccaa8f801f57c88f292ac482c8e9cc77c10dcc3d2" },
                { "de", "8bd05166ac8984c46396b4ebdd01d8b7019dd5f2743f6ec1005df597e541aa834a7e8ec0934f6acbc089d7bd1379b20fef3f1cfdc7eb636769080d007b134725" },
                { "dsb", "e8b8e87d7658dd35879d3d46d6a36149f5553deab9478723440a08fe670da904ac68e9183e54ca24fb4dc9b64bfbf49c8e523487fff68ff3bc4fd7aa76d5fb78" },
                { "el", "a29cd8bde9b1ef3f7d7f12fca120f107f8153b170639e3ea7d104e77961197a5784dacc84ec6b273b74bd0cfbddef464fbc06940598170db95cad5eebe3f62a8" },
                { "en-CA", "137e82eba93b877757bd59e59d55a2e89ff40763d14f9dc5a438458ee1ba529990876dc48e9c0f461dfb27ffc47e133d9f00deb201157e0d0d7ff46c85929ad0" },
                { "en-GB", "6494e04dd25903ba2f1392c449a51525315ee9465b6b104688edb2e7149d62e94c31d54d7a3e4662d5127b7ca9d52db69c3ee1ffb3890e424c9eeb466eac0171" },
                { "en-US", "60c70dac5a360aa965dcfbd2add677bba23afa8186277dcc452f38c0da5c9ca08ba86fe5ba1105adedd7431c0686913fc9f791fe5ea813bac39c10de0d48e493" },
                { "eo", "bd9798ac441e2b74b05f301c20941d7a0993323f2b01d2ce57ad4e87601704dc73378ecb48e68cef112d9911b1425b8c81487f69c05f04d2fa65dcdb9ed4825b" },
                { "es-AR", "fb6437e581183aeb8df3aa4815672f91a163b1417df6ee8df0e02923d893a0bb2140279ea74550669a9692e873dcc5bca3c28c1597464c8487855c90da57e273" },
                { "es-CL", "59ca12fd3af14827ab3c687aece4c65406d529810dc7e643a718a2bfafb56ad6434e874a675c3e7b706375fb920d3d9e5030bcbd1cb6b2e33076a2d01ba27ac5" },
                { "es-ES", "8069a62a356c9540a7401eeba782b8c8f541ea7b8f8da3c35f77fddb58eebc94dd5ce94cfc926014b6c1897e6b7606173288b24184de0c8029bedff34c48ef1a" },
                { "es-MX", "b70b5aef842a23abe8b8436fe1c8e1e941358dbe397f03e7714df499c283b32890f7ce041989ca6f4ff500854f1d2e2a1eff2cbc743a0dba95ba52658591fe63" },
                { "et", "0e3a36cb895fa871b2fb5f7b8d7613a1bdd5b9a8d4ff4b3c81cf264972acdf88edebca9bf3fdd6239f9eba1435d8b4baa34352fc0b818fb32204614ec4e5bee3" },
                { "eu", "27841997f79dac16d524a8fdfee17158585bf80d68d5bf7b383a08f15ea6a1a32103a97fbcb6be6d7b4a9e3ebf93ae89666d3e4812ef3ea04cfb8b0b70a52507" },
                { "fa", "d7c987b95f76e74959caec13b4ac4b7d9f5321d0551de91fd2bbeea0d6facad55198b7ac1fdcbc8cb631150ef7f0e2e3d1966d95971fe98a172bb797563fbf8e" },
                { "ff", "c6ac789602b0c3f5ddcd676d620054445846815acfd7aa91c09bc9bc06a4c68b079b186802e603871149dba7960b98734e2c890e15d8967a4ca382c6a9de188c" },
                { "fi", "19f3b92555e6c3be694786e1b741dbd7a4a66a7b7fb6a5d106686b80588dda72af22fbbe9900c6dc82e762190f0789ddeec8779d738e89612ee5abfc71843a4f" },
                { "fr", "f8aad0a3f93628d60d1f09e321c5eecfd309c629f9672ca3d035dc318c775a86b70104515fc382251ddd3ecc89ccba26712583e471492c40950a09478b6f690d" },
                { "fur", "ddf2bb976c110556c3f62deb328fa75f85380a01454902c721fea29d96bf3651820818e9b6eb29bf837f5276ee8cf57b0fc9fb07e60056940cd9dc5b7af12aff" },
                { "fy-NL", "179c7681e0286cde2abe20618ee260e7350ab1b11b810e81e15129718e80a8084e5175cd3a2601bcb82b239a97e6294ae7bd7edab6c1e9b4b2535b77f7793fcf" },
                { "ga-IE", "8ebbfe1fc5145e0ac20718569f4c484bfb309463cfa92b59a1c40d53df56bb8b85897fdfa7e439ce321dd4e8752ada221e3cdcd8a485c2afd8df5aa29eef3895" },
                { "gd", "d6cf80bb4fb613da90f5122c8f2ac1734ece4bda006517680b1dc70780287aa6d70e8123a16e1e7dc3f60e5423edbb53237595eb07329edeb041590a7d5252d8" },
                { "gl", "15e7b12d6693900dcc1f2684968d22caa9dbf1f45b89dc89c53a2a55a8a24280e18f88622b7a482a60b1c4370c7cbf5f9b77288734b4d3f1a859acc59d7dc1ce" },
                { "gn", "2cc4dfc5272938d7e886e1d77fecea08ace29510a432dd0ddfb26784db26bbf4d6942178b86cb80c2b355d1559d9aff6cb555fd184b9b5b525c8877a93dc42f7" },
                { "gu-IN", "bd3bffedd8c0ded0d496fd3be78947022b5447e9698d727eb0ab0ad45ce662e4c6ad8fb806876131b6db78e6579c5e82b37c3f89bca1568970fd32fb2d6647d7" },
                { "he", "11a5c45953bbaa8a272ae16e7584ef58770cf0462edc9de217d242ac125e33c686f9f38f5ccfa5d5a1cbe5bea99370fe55b6826aa6355ba61952a6b3b85bae1e" },
                { "hi-IN", "e031cecbce7fc265737c530ce7fbe6a20169e44fa136d7742091c87b926c4793757ba85347d6e3d25ef0ebfd873f317f068d16d47208384ef6a92651cd756ac9" },
                { "hr", "f784b5dfc62e0cd17ac9019af03533b99f4f9184fa57c9d2856a706ae1119a239ec4a55bdb4bcdc6fd217cb95dd24d6a9cd4d26992d9b721b9de4a9689686fae" },
                { "hsb", "8e6f2cfc89339edcf9f1e6434fb4c8e96e58cca5711d6023a0fcc4c3a77dce7cc892d051773aa052ef6bb4592827dbcf7e6aae1ccbff32386adc52e6d6d51738" },
                { "hu", "e1e1ea9dd89c6eb9167b3edc421462d4df040908f46692081993ecbc315700642f005bf521e75f181c458e5933166bbcaf629ec282bc12a04e6b15cb03e5f62f" },
                { "hy-AM", "31103218b0be629336ea473dcc23eedb09b80dc01143dcf200a0506f99c11cb40ef7a47254a8575755a043da54b7288dc5728232a98646ec0e80f3cd6259f5eb" },
                { "ia", "6568c2db5cd1f59b0f1be0161f5d4dcbfc88cae955405b8f0149fc21d5620d9b08469be9911a64ff4e01864a039fc18f823b32cb227638f1aa06438cbb41fea3" },
                { "id", "26fa0c55ac1cb85503bba7ffd5f41ab4f823e88887c51f77ccd0c3cfd8bae46044586b44e4fa422e3e50d192503880dc213a88d6f4dcec87974871c4e081949d" },
                { "is", "c504d9e431bd1e8f1d1da6189ae16c78704431b07ca08ca434ee4233fdc261366a7a0df0675c58cd8909b62311eee2fc453bea270f767f8b9d459305e7662b16" },
                { "it", "1c47045673c13f0c0a9d8730a4717d0940b6581c53ef2fd159178990af89fbf6dfb4d29221ec40d134b9749999d001dd27d4f9e1c1929e26ee3a819f8ed04c92" },
                { "ja", "a931d70d24465c1b3e246933ba346e1c0f330f9b62796f8c6c7a6f0b7efb5710506275da2d625b8790d3231d02c4e4ab030ba23556cad608b0bb91ea663ac7a5" },
                { "ka", "cb9cc759737093f6dd96d6fa67a4fb1a6caa8ac20e70458848b1577e64ab095d402a354ab40c7f8f931ef9cb4be87c434104f50a2724b5bc0fca89d59cd11334" },
                { "kab", "66773c3752bd7233ec567fb6918a21032d0a73065c108d6aea994ea00cde1aca60b3783653ec906dff8579000dddbafbf4dc3bc75f3b604ff319862ade19edf7" },
                { "kk", "88920761220245a36d70c0ec3bb14ec0397d1005a19d0752936bd52f2d25de6119ce86a4e47d51f646b738d500c63e93fb5cd2c5cad22407ee0270569a17f5d2" },
                { "km", "695d58350354adae90c1702bbf1597f78aa4d2f62e3585a66f542337587fd034ac497e34f6049580386d78184e342ae1330d1ed567bbd7600a9e9b0719d559a2" },
                { "kn", "ad7287be673931efa050bd3ea6e42a23c4f325545caf30748a9d3230517feb1199a4901aa36f51ea5745a79a5fbe8fd9ba08560b63f5afca498d1d0e2ece76bb" },
                { "ko", "180a7fcceedd0d93f1939da189edbc4193e2d92b9f750184b2127d77ac4b0ef587283408724a1394b2c4980b2f1209a63175ca18195ef2439c1e62c1a504cab0" },
                { "lij", "16a8c32a750aae35140fc50e7df6ef8bd76e7ba2cb73a5e8a84d1288ae5f0c94762c8b0457ebd4547ee837ea55ce3b2d7852ec6736cebbc1463b4b6ac1ab99c6" },
                { "lt", "c1fdeda0d69ede09c2cf9ad17995f5f3ff5b601c12a34219fa219681ccf4f697b1723f6f4278afbd659f3a66b8c3d86fa2ee6a9e6ba5bd51633ffc98ae98f32c" },
                { "lv", "7e171316e69ff4ef66938349aa209cef6ffc831fdbc10fffbcccaee6aa6268c40b1f4c0e750c1199455735fd6c10c20b0e5f4fef091b6120dba4e5e9a8dab18d" },
                { "mk", "c8c5e2a43747d0a7c31ac17aed2023a592b64e9ca1d682ee86caa20267cd3a362fb5ade11fa257fafb7a12d2d4b9dc7cb9b295f72eb0a6ee4286ca536f09e231" },
                { "mr", "8e9b9a96fabaff9f230b9a504592119edc100286eeed93830e47c6bcb0fc5e004e6cab5ba165bf43469e3f4387924b82c9c31a2755063da001b219430753f59d" },
                { "ms", "dae1036dc2cadc4728f6580021d6e47e72bdb5f8c4e8363e5b27e334add44b1b839160da2dbe838f59cc660685282ff1984263f51c97a9f1d5d6117af99cb059" },
                { "my", "29f67eeb901f79286ffc6d9d3868bc79aeaa52d90e368619f79711412f0a5118b696ff3b4121ecf5ab83288bb440b1a8ba5f9b8a91bd6fdc005200f3e671e841" },
                { "nb-NO", "f51a42bff2cf2c893f87dfd51523c8d9cd55c3029c319a9d98111c5b4b1394bcffa8091c80ff7f9947cf2972bd92c90c8c98dc30c1a9192b2100d2664c9fff12" },
                { "ne-NP", "4054bbc281b0c1a46496e7c53bce239c044556a6d4437361d821a2397b9226afd5f3ffb2e100dc9f7eeff948735a4ff5d350fb6b279f8f1ccda0c57897f1008e" },
                { "nl", "c85612916cf7766b66e0e09a4339923ab90d3fdf6ab5b3ad5fbab992b275153e67a18cc0c1c85713e044afd5ebde845e7a7f99fff4a88ca1b09408fc7f2a7c79" },
                { "nn-NO", "5fe0e9e040f990a99c87fbfd37798a8e9ac89aaa3c307dd32f472f5e1b42a5f14636a3aefd79f327473d4f1fac848a6b7d65b046ae97632c949543342d8dd0db" },
                { "oc", "c4aa304b52f4f5e84aec3fcce4d2177e326ba0fa089999f03f7d78c85a4d337fa6b9dc49c92c549ae38e5011538d6407f9e87b30171fd43259642e3773cefdeb" },
                { "pa-IN", "17388ada32d1725e451290dafd7dae03fd0e079ed84a25de979fc2502e097b72dea21a814be47102439c12951b5a04e99e2b3c0be0bd00bfca43a383dd72f11a" },
                { "pl", "40d0656802e8ff58c2ca9f625c0244f865512c53b41f3a4910db9c7c37873cba2532a0dece35fabdc3130b08216207bd9f4a2aa3736188ea914770cd41c0681e" },
                { "pt-BR", "8d95901c61b7fef820d94d25ea0178e28096d85625a5a212624e5a9cc877bb94f7f21649e61480d3dca841a97310828c6b5ff725387ee018e507ed2d8076b512" },
                { "pt-PT", "2e7e2654c5b4de20378050f26bc3b679d6113d18a75d0a71f40cc5026966e7e4e436099e34d02f74cb93e7658ffb5d99824ae57aeb40f2a9e81f1d0d27bd7da8" },
                { "rm", "068b01932482315b10f986f03d403e12843454dba2d34f706cef31210e34ff88165743a277d225243ba21388981f2c5580d367b8836a132e98720080c58a5334" },
                { "ro", "f67315e6b37a3fe8bb107be1cbfa634e124ea0ad40a96304e853cbc71da6342eb5997f991d26f82577806802241a486901a2d4c73e884b95a628e78aec07fee5" },
                { "ru", "6cd400c8929382ab47995311c74099d50e6691c8356e9987ee89e0e0ff8ab6ac653d1f130aa577d18e7b11098af5ef20e0a0e25a8059875b48c788cf44694538" },
                { "sat", "6ff23c9caebc701631abbffcdc6383decad4dc85654fdca1594670b8706dd3abb04e8a738636e9aa92bcaa06264f7cf0462db6143936fa05fdd002cf6e7ee7a1" },
                { "sc", "5e93f3484fea786b339ec198b3ac4d93e7a20aef18b1aadc885833ebd50fee91cbc7348987c393adc2ea0f5369b62e5b6d3b49385d5bcc8de9a9378761f9047d" },
                { "sco", "42edfbd56746cfd0e55f933bea275eea7e074ed8012541096478dc829f562cb7c94be06da6b273497582d76b94d2dc6879cd4c9086e76e3c66797db9fd3321f7" },
                { "si", "53559cb438adcb7548fc32cf7a20ece093c58589b4e49b94c1172aed062105de279a4df0a5ca710d0959de35b92009affa32b31f119299cf7d46804a21c6908e" },
                { "sk", "5735e38297f87e45372ded114d71d52c5e593cdd74250c3d8dca03f4855d4a101c58a20c021fe9a882066f6f97402e2cb008a3636b266414a9fa8d870903154b" },
                { "sl", "a80856915731cc20e15c6a18e44dad6c6dfc820618e75fe11fb4a94043f22712603a09ec8e5cf7154b6a76499a28466d582d46f8b17c7435e11efdef07c50c0a" },
                { "son", "4893de41aab8eaac2d699dec9cb69822c394a37f3d46376c54b8402839c3e6fc54c44084b25eceeb3c97d2294dce82bddd43c4e34aeae70ed9b9dcefe6dfa60d" },
                { "sq", "ae18eb4f843f34909098de4fc56e338fff06a107dcec5c0dc8374b6aa1179be3db4776b179609ff5fef4a39129065156552879281e037b769795938d8d8f7602" },
                { "sr", "9379f34d394a8af39c4e367b5d97b1a0c5551ee1e73b9f6f45afd7bb0947d83dc9e40138c72657d69f4d1ac77fe2e512bf8e42cf4411a262527c870642d1a3c7" },
                { "sv-SE", "eac4bb77a262fe6e30281087b9e000e08f9f29969ed9a7d37e89f2490cfcbe15323392183e83eef8e001c41cd89a8085fa04d64ffc48cc1839fc4f17700a74a1" },
                { "szl", "64ecd057b49d08d9be01b1c6474a07d05483c8b63fabde5e558c6d3e6847c496b9170f06a7a789d6137f99f4bcd74249a2cc94db12615592cb7d439986f4c4fc" },
                { "ta", "4d43ad83008862ae8bcf365ccce18da19cf4b79e0538aaa0630b1ce18302a1640a53093f775483c7df28bde66a169a94fcaa881656cc6d36267cba7219289eb7" },
                { "te", "a8529b86236080c45077ad70ba4187e90351e2726704640bd201255ceb639e60f21e6820b1ff76a259b57b75203e5ca3b9418b487147dc5fede8da996e1ee62e" },
                { "tg", "135e6efb68a938f13456a168fa96b869d2bbdac2d0be6d6d6c5c4fd1eeba3739954a7bf69600fe348d72ef6d69d525b7c1cb675c772c196ec9416856cc9bd377" },
                { "th", "5de655a3a86be71a9a4fc8e894741183ebcd0b2d1f2a3fa9a30085b6a282722bdafc55ebb1e5c2fb59d448257f5f464b56bcffb8f6ac821d6eddc98267c3a281" },
                { "tl", "928fb29623f0df5e20b2c3a7d987b8cc9d60ba8a59a416cd6020803a0504c1c3b75d83f00a1c7d68be5563a3e24c43531cdd3a62d748f5e78858b655df8bb4c2" },
                { "tr", "8eb9894de5177e824bf60d2d0c87690e2fbcfc68e56f7104a73f281624e65600fccab0feaf3f5f5ff689354b7faba3e775523c55f01271a5f7496dfc0086cd83" },
                { "trs", "da3f06756070b2afc6ca7c148e2bb42e81f87ec81c8f9df663cabb82764d2840525df7bde3240c228d0919a53e6e51b9566395fe152dbbc99057626965087e6a" },
                { "uk", "b66ef77e33a2f5bacde348bc5f6518945e5d57928e9a23d4c02c986774639061971f029399f8bd900c947f4c52432fc961629960d4e7bfa0c39da71d3817db37" },
                { "ur", "04c0dffb71cf7f5158e079cc9e9724bf108c22ba791be04b6d5c3e660a3579e63e508ea42b48829cc4c8d1ce50eff716630382ccd76848f1a7b069de78ee27e8" },
                { "uz", "6b5afcedf8000e943802f90ca1a13f6b1da01220637a97fe7f6dd6a21c403a1919fe3b16a71ddcff0339dfbe5b4f1db646775a19b7cee76b48d3a9f0d5895772" },
                { "vi", "8b4b8a7cfc03714c0574a56812ff13999375963d1a6c75903ece8a019d0b70c7f2ab5e6446222fdca169aef1660e6b0713464a7e8958afe652fc7df4786609f6" },
                { "xh", "ffb9a9cee925c6e8b662c9e53bc17466473d9df5b6f005ce834c253cec3fb6d684fab6a7e453dd5f0c0f81170466205f2f7447d0bb25dd7976a9107ced334f1f" },
                { "zh-CN", "f03b6e18cd3914f93001895d31cedfd3e08933d0ca26b1d7ed2497fa61d1e613bb18b72f390727bde2168f13689ba68d94b314c81a3fd1ab3ed6fc070f6df05e" },
                { "zh-TW", "6d7b53cd15213de23f3aed02e480b4d806e7c2bdaadb3af1c862743ba702adfd51aed74f5a663bb3feb27ca74d5a3f9168fa9a4ee9d48747bf2cbc3321508af3" }
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
        public static string determineNewestVersion()
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
