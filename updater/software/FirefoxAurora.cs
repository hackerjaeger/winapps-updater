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
        private const string currentVersion = "112.0b1";

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
            // https://ftp.mozilla.org/pub/devedition/releases/112.0b1/SHA512SUMS
            return new Dictionary<string, string>(99)
            {
                { "ach", "de9d1ae55a210d5079693179230665057018f8f3c59b878f7b75c5c46296fc319d39fb831e2460f0e0963e6d9c2b98b5321fc0dfcb9811793f7a1b3c1f95e446" },
                { "af", "38ccce20bdde49457e57ff26995c852295134ef52be53a567e74ffdc0ff03c54a21cc86d3ea4035ea285736c4f42a68f5b8e8671b8bb6cf9c11db40cffeeffdc" },
                { "an", "9589f4441e40230302bdad8ecbfcf007c28dad6bbea92a408cac7fd9e2011b2396359c47faaed57167426ad517ff81bf449f08f309bb94323da16b4dbce23862" },
                { "ar", "63f1e6294ecadaa698c8b3532b5bea56f2922e315ca6b5cde7b2e8a3fcaccf005239560328a16d2646dadf806f60dcf94050be80573cb0981c2c019516887135" },
                { "ast", "b9a54cd5839ae0e229a8cef5db96c553c7e478fdfde49511f8bd2f3bf55c190e49e22d08a82e047db40e9ab7a3098dfc4d90c5435ffe4390f91cae93ea0f1648" },
                { "az", "e2ac3f623aaac0cf9cc6d690f3901fa9746f557ea2f060e871baa8448d1347b83ba4eb9d675aacc22021244f1004be7e5b3a158276b87c7669723b2d1b97d7b1" },
                { "be", "6a677106401be548f5cad7c41adecc702ff664df0c7d424002b3a70f3e98689d9972d6bba0b5bde5ba7fccda1ed7f9a0e5f9c0e9b1dead45f428b7efea979e8c" },
                { "bg", "4fa761c7f339167797accf3443f978b673e11b98a697c8ddfd1e3c3e0d34b53a8caee65c056253fcbb2460d16399f53d3f2f4ebf4510bb5746e0a34c3548fcc4" },
                { "bn", "87e4cc8d137d286d34901712d5e6923016ad2bfffe0bbd87314c466af8eaccc9e16fc32c3472a9de2b95a4176da00842b4fc9e4e57c5c871daac64ccee0b409c" },
                { "br", "03d9205331077db18554ecd33e165b3937f995732c8c10c31c29bd9401ae90b3bb1214d9cf241b05179740ef61fb911098ffaa0a19b7d69cdd812567efab7529" },
                { "bs", "19a8bb85f5831adaf9a9f3d90a227bf1ac1c9e6a2abdd4b0eac6d048c3ec73b770e90464eb315b2480c78d9c839460b5a8aaf5b9a0dc0c82cee10ceb53664745" },
                { "ca", "59f30800dea567b106ae39e051036714ca64bf628d866ff8c81a89b3858de0b5f6aabe287243e2e79ed695542eaff8903de51d656dfa6b79b5bb9ec52a80b135" },
                { "cak", "6984ecd3158340135ee47613f7260311fdda41156ce2553d909eaabe86ea71df754ab9d4b32912c72bd99583e9aae6796d77027476b592bc3b5ae7dc0be24ed1" },
                { "cs", "27668e8fea0830b1a7351acc19a1a306864431c7ce6edf4c24a4d4322e4cb43f8dc1bea1de012419f6460f65f7770466b3411bfb9450162fd23f6e8f7f373899" },
                { "cy", "8c41c6e0f2c5a2a0b012cf42c8e2ff6cf23bfadd04d52d18612ff34f21f04da40d262c74729c50f10dbc43b46d48e79265c8950ccc80d3f5a50913079038c19c" },
                { "da", "5b6668bf73fddacbbcc5ac4455997ba1c046e8f575618992ae2eb43e191cf5748fa55d32ef9541979f0fc1577bb8254f122b2044394f2f2813cad83419705151" },
                { "de", "d176d1ff6ed9edc917cfd145677a57c22599b9a3cf3082ec26fa75b09711c19ccaba41321cf7d05ed023ef838e09df7ea4fdccceb698e672874b9d06618c0ac8" },
                { "dsb", "64b293d2ccff11e594cf80d394295c86fe9c6b8970eca7b79f3e7798295dd39a902f347860a097de9c410554d7c1e8b24600ffa1532a3cde69ffae06e83bac9c" },
                { "el", "10320b0608034dd3a7859b957e4ea2e44b835317046f5474d766cd8271ee12290fe5e7e661e3b528f3b906208067c5ea73c4614a847ff5b5238ba49fa91c4127" },
                { "en-CA", "d28cbdad3bda3053222b3bb2758e5ddf1ce6e8b1fb71ca79fd8d187901091310cbe8d4c260b50ed75f7acfdaf6050a7a23296b6c5267ace6b21b55757b8425a3" },
                { "en-GB", "a1df57191717756bcbd91f1c814f3cd606fe1914b0a6af09d3218d8240542f613c62d27e69f83da29102b531a9dad04d3329374d5f725a0b4bf45bb5c727a3d6" },
                { "en-US", "71696c3fcd54fa77172dc039b7f1dd621213ae3e672ad39b83426fe1558f37c143a4da82fdd12c2874ab561663a4c9edc9faf1fab25c8239a5513cfc0243e532" },
                { "eo", "e27b23318ace1ca0cfba63e94ce82a4e197b65a7c7b6083a3d0f25d9d8b5c2e6e1c49745dec2c041e9457360b67232a43006aa0b3fd128b167c7663b8b7b11df" },
                { "es-AR", "3569550a08cfa2f7d6840e67c4abbec9f9bc1752a8ee4ac4b50abc18ef6dd3713cb33007f9ae4a54a2364970623759228491ded8c804b65abc6ee5c9cf9d47e1" },
                { "es-CL", "f129ae06f4b8d02ec62f4fe613f3589281c0a86c2bfb126aa903cd643692d00ea6b7e17d894bf48153be2a38945da7120d661a97595e3de2751f376bd0dc505d" },
                { "es-ES", "0a450550b4d42165d0e4b02007da3b6cddb62d1051731e36c69eb552aa7ce928198c966761741fdee56759a28aeca6a7948e6d727ad67265aad9afe49e502496" },
                { "es-MX", "e54e23babc4b8bb30226504cc4a69a514b0bea47d8d3340141bcde3ce51472d6e1233e0899802ee345c46b7ab7acec23b9fc2fe1dbd15431e3ba959707ed5cde" },
                { "et", "1c6046bbf0a5abb22ea557675ae91b728216cb24a684ca35e2dce88cc607b1c22b4b8850cf64fa0c5319d2e4a097bd791e2f247711f752b877ac02994eeafb4a" },
                { "eu", "20a3fc8fa4a73c36b3f6691e1d80cb8a48f4cea1302c677f30c04dbebf4f8ff06a8519d8abcd2f0ee82b719fd28d19d1aff123102e079029b718a63f6dc4f4af" },
                { "fa", "f666b5eb28935df67ec868d46d9d84bc6d9487c2084a34b6cc9496f8b27a7ea9814217bf498a710306c5eafa958e627ba140f3f5630887a22a963207e8419b5e" },
                { "ff", "c9a9210bc7fd21e1413e6db0508e9233d61789983f8fc1850cca75202ab0295796cd628ef2fbe558d8901c0bf6373c15971f7d171b5b005b68998792ab003f0e" },
                { "fi", "6324687a45dfb6ce11059ff85906b77386e1706f7570930ee58670e76ad7f856af050475d390c072afd0922fea9117bd03f772273176211289a9023ea2d3b259" },
                { "fr", "606c1411049dd64589bc036441d68a9cd405343d6e43f6fcf90544f446be172ea3abb9b461fcf38ce9ae21072d3db7ce085eced0faf88eb1698d3c833df5405a" },
                { "fur", "d3dd22fd980c576659f1ecb534c4de39dba4185b4aeba2e86a9d4b2c130e38d826ca9115356a11e0e6988dbea8d433fac398f44ffc501ffcb5fcf1fda30eb237" },
                { "fy-NL", "77444c8d0601927dfa0a4183bf3d6b43cef770c6bc5121fc15cca78e1dc3c2adc8f8651655ce27c429f76aae077b06978107ad64c15143bd7037a7e36bca372d" },
                { "ga-IE", "8da9afab533b86d18cb1d6e347834bea0cfe1d6d2c0a98ff01d0459ab630666d54cf3ed7eef847ff51bbefa9e9272ca45f95d6ab6303bed7f8ba408b3adb6696" },
                { "gd", "1e628a30a322f9abbfd00295df8c0e03c46683fbe1189b1934741c2ad1d2d576526e88948a3eafddac016e1c103c09cfc27a5dc094af84c4c42de9f7c860f3a9" },
                { "gl", "ae10d536a45f4fec5b6232d00e54455668115e1f9f232f81bb242d94bbeada04e0dff866e6055a97a0ba43dd71ad5809c85d02e1ac5738ea2fd6aa7552673283" },
                { "gn", "a6041e84989b717fed4b79ad92bf5bd82fd44848e5188c1f37beb5dc026e6e63529bb42baa399e7a4879bacf330f024fe22406c1582353a247343fd6c7d2c21c" },
                { "gu-IN", "715157580cb95718f12dc5a1e61ed4acc7bbff9f266ecb6d95e6871a48529a02e02c54e6ff46d8823c702e48b38aa9e0f52d6f0c75e2f24bba7b326a88ad765e" },
                { "he", "351acd24ce8c6c51c9372e7c20bcb400cc087f69461923429d82d213c6e34df949fddcc8fb6a6effcfc0599e9f28dd8b70918670761dcc5e829293a8ce0b4180" },
                { "hi-IN", "cd7a098e1c659b03e274654c80b6ba69cd274940abb191d162fb779d42438d28fb6ab712ee55390b846d30dcf35baf786abc5269a673c899b5d424948e991c91" },
                { "hr", "238ae9e5cd8983be034446709d4d526ad2e59678beeece321c7e2cc01bfaf7854ead6e9c2481300dcf4ec4f8fb577e6fffb0fbcc144ef0c285b96b428bf4b290" },
                { "hsb", "3af327b2efe7c7cd98da6b2011c429d4c336b25d43457963c7a8519561d0783e7ef68b753fa977bf86b44a3b20c6a6e54e6cdca926419903821672a8b4fd040b" },
                { "hu", "8a0ae122b126881a197d3f9b15eddf5ebc7241c4febd0b6388346a046bab7ba4810d45caedb85ad4d1ed15c6c8ed7db794a0ae256933bebafdba81e7ffdb75ed" },
                { "hy-AM", "bd64ef559beb5a843bb5050e5324f68bcaeaf566616dd0f8746cfa2c34ad7da6ff7bf5b2e5117ad4ddc046bcd3075abfb2b49be1728467df584ee416cdd22acf" },
                { "ia", "0e25ead86551c4ed2aa82b906653c92fef7815d31b35a3322567db57f19c90f9d00c4b2b7d0a050b058ed3eddcfe2b2c2332a9bf14e361a7a24b6c62e964b26b" },
                { "id", "dc1535e0e19ad59409583f6dc5c6e5d2ca4a63379cf1271b5eecde18abe0e07078addec91c9480ded334b500aa4de3c1b2a58062b74834cff4de7811dda1d429" },
                { "is", "00ab099f972071fab0eaf1bb7829d4f9b49f552a4717e3c492fc3aa7a644eaa0d63eb803a0238124c3fcf014e54ad3160f2a9c0e6b67b09cdcfcfd61808d313a" },
                { "it", "32ce6598d16568b343583b9c2c7aa31460023a12aeb5792ec7f33414972217b894fbf738bf98aa0c4717c61e21ffe299a728ccf564ec241406b94bc4343d9f38" },
                { "ja", "5ee46acefab01ad904f6df48d0ec88f51bcd39fd97c9a4860b5b4244bdf125fed075cf5fbcad2d3a50aa6287132d265174eaa64816c97f153472b79325a598ac" },
                { "ka", "e9bd650e6c24c444d3eb80c7a740e5a92b47e1d1fb30c920c88d5931e4342d2b60d80f230916c3e124bf3cc979d1c41420778bdf15caf653b965bbd55b529d30" },
                { "kab", "3f6a6e3e1c4875f3b9e55dedc3ebb5c328252162d939a337b465c7c0315cee29d2fce2397cc1d7550e9105ab2e60df2583ecd3db39211425bad306cfa400762b" },
                { "kk", "9bcd91c4c0fd65c2cf63a6d0330f1dd27b3c370ba4b3a293b96a32aef2922b5d93ce96ef130fdc7adbdee9aa0b279917a3b03813c15787306a9ede2e75ef7f1b" },
                { "km", "33ec444ef09dac4758d8517e9aca0339a5d83e5bc64887b8927749b67335eece8315612057fee50bf41b3b1aa3b1859588706b692a0a2b24c2549df1d0508a7d" },
                { "kn", "7f3f29cdbc06100a864ea0a728e307a6130c147d1904a46ed790fce67222de704313804a592ca7262c57467b77720c870e1df6c9dbf33639f76959a69aad086d" },
                { "ko", "16421f6dd34b4de8d37f6d770639aa24485a3f78211f971295259e65309fb1381ed4dbd333e24d6492353027fc916d05cf0ee5d028f1c72b31a910bf6ce16a43" },
                { "lij", "48b8fd86d68b5dea4febb5553e480dbaa74536bcbec0248b7e0ffd8948fcd7d1e6ed0473e6b8834895dfd85adf2c86e99d808e96bf415b56b5b5eb8d5bf90bd1" },
                { "lt", "c72e9111675b926d967264e8f2ca9e2bc2b4f4701e1488e531d4a9fa25be5916228d208e833663101e59294694be5b34169a9707d195cb738ca9b9610b4b1256" },
                { "lv", "e5ad89d8a3d4f5946a4dd01dd66ea257ddde85734a828cb66d48544e016369de9816f20b0741e01b54af4b5666292dc13e46de044b250e78fe86de01137f1b93" },
                { "mk", "4c669d75280a08386e05716c43ab5299b8d26264632a093b1f53b8ed1b9979a346ec0feb824a501838bb19443d1560edb56b29e66fb31a57b2b8be64d4c1513b" },
                { "mr", "bd883e72e7c4f94872b083ec25f348b4b392c77c36a15b04d13bddd567fdd0b42453da96269902fe74e95a03a5d41175095be34772a360e51ac3c69517a4d38a" },
                { "ms", "d3afe1e1d3366c85ac5992fbb3773f2217c12a78801f34077dbf02717958abf0955eb325bd9c2747ca77fe0322784791fae6649736eb2f1a4514d6e49cea13ce" },
                { "my", "cddbc6c06c586b29c5516a1c2d182fc3d810e74cdb12ce15fb70b9489ca1db80be4bbf3a9e290f2f4aa0c7cf1321ae008fe9bbf36a10a6e83558ed747a0bffc5" },
                { "nb-NO", "852c5efefc0564281474fde3b7ce87a154b40f20668da6bf0f0203c47ec32076b58c6b57c7d54479d08868f1e3cbcc32db129566d0db578ad353bf9a94106def" },
                { "ne-NP", "4e1d044c7a1e74666dcefeee582851e6fcec51517ab9d3b3bc968faaccd0a14f7b62643f63c4210e2ef9f245762061f4f2e57e855fa5b2285d1f8f794c2f04d5" },
                { "nl", "e007290556f6ac498d378fc47f6a7dc2f53f31776c055c3dfbdff6aa2bf2962bd9988f8f45be0889db11a7dac40fceddd8ecd35855fbc4c9a0635db352771195" },
                { "nn-NO", "fdaad5813cbb1d178ffdfbff696f5bdc7ff5ef4db3d727dbff74954e547ac49cb7283330294fd3cf13f0c7b513e54a6f5136cc060e00bbff4181e02cb8b42b52" },
                { "oc", "646f7c1cee877619736488ee86cf17a6f5dc5a27fda30671a66546c70731b62dbe20593d8225c97e77ba541782bcc562215fc433747a5a184130d5e553fb40bc" },
                { "pa-IN", "03ad0b7d8dd5e7304f0d29eb30fde27c512a22235e9014b1434ae9f9bb655bc5b461f97db3e4e7e856d2f5c22c3e66785472da91e7f39957a4423df91139ec69" },
                { "pl", "203ab993adef87aed74ea5eefb813d1cbc4250251de603b2b4df8b5a788add78d8764951582755442027b2ca8c120e02833d5d69eb094ca0e9b83fd3a6ba1ed9" },
                { "pt-BR", "cc57dda40dfcc0f2f2ceaca6a2039a015d59ad919078f43d9d406b1dcc3aaaee836f38a29a417c4273466586e3dfa9c7d26a75b6a31818f04233908b33242369" },
                { "pt-PT", "8dd9aed8671c14b12aad2a85219a8b2925d04519e7adecbff46d1fad5fe55fbc0e14c93b92a37a65edec856d03ecefb7fe07827f055bb63feeb82aba02b7efb9" },
                { "rm", "152d873aaf5ea085cd46ebe49f72571a4a2b93584a6a9bfaa855bc3a0271a906a94d0e70e565bec1f38528b3f34420e83fcf6b3f96aa68e0fc3822235f6215bc" },
                { "ro", "a6c4201a4daabc851f6ba340f14380a79497d5014205c5cb8eccc6516fad76c8a897337a059456cad7ab70699dbd240b3be2172171e4bcb548e2fb8c9ab30beb" },
                { "ru", "8cb0cca8defea20ee5a9f997e00e27f83325ac7716e24b1069663440750dc0f44dc6b5d1270696579b170cbf16ba9673e18feecacd7958fd8434a7e0ede49868" },
                { "sc", "2fb864d5c67c40a7cea322c8e808fdc4c48c6b895bee8a75ee20dae343d847ced0700f21d95cc055e80357602540bbda3233809c31658115a8cac8d820104ebe" },
                { "sco", "3cd2efcca31e492cf699e76f23f0eb500022e34359e43aa570822227390be236e7c466b2a0b80e0d8e1b96f1cc91ba17df671885684774c9cbae9ca10d3a8383" },
                { "si", "5a2f9940f111e93092a98b9860d3e77119b25c99dc84c46ac2224eb054d93ef7a21fe1e98cfae76a5c198343231ba47aaf4e0d4faadbdb2c8e2602eff0b10c4a" },
                { "sk", "49284c1bc2e6990c55841822c176701b942eed0d6861dee77ae864859b01e020f7ab98d6e4e0dab5272a1ebba1aa21201a368bf2c08072bdb8bdd21969b7308f" },
                { "sl", "868e21550134bcf3ecaf4f0ef8319e522713a961027c91f77ab96e6577833eee7c38063894a2974770ffe126d7b210dcec6955ab0d4bfe0ca918f7a07959f117" },
                { "son", "c5de6bb8b340430996f6c14d3dd0bfb7be0e49f32e94b7e7ae68708198f04759952daf5716cfc86fe27900bf3c8d27823d6f103de06b6962689e834139bcd4cd" },
                { "sq", "e1da421e8cfeebcd18692ed6df8e07d66ff530cd890faef039ce5f4ea116b3654a277051457e9cc8b78d62e218198741b8b6096831b6c02dc483af1be41c89b7" },
                { "sr", "96a2c55dd7f6b414619ced056ed187a3944762fb0e774315967370cde1cd655f9217c883e5c7cdd5f1637d5f922cfedd9e88b79e7c43ddea16c61d319f2bf658" },
                { "sv-SE", "752ad19adfebb144f27f59e3f84e335105a2962f5f6b52a38f0c70cb176e35617e53ae5e0840239b86feed6ea556c2fda7c6b0ebdbfc3e361b273f83d3448c78" },
                { "szl", "5daa442f4f59490f8e9352a5ced8a2989b23578579cb9fa69d7b2d9d2753f0e1f0f742220b0dfc3f4c779fb1eb7a4b00e2aad41b76d2f022b53a5a418d5252cc" },
                { "ta", "a2e227777d2001a465297799718e5d38f802d634bebc4ef3014e02295393fdc6c2a06cbc0fc665274d321546d272e81889c04e7aef6f8c0e894204cf8fd9b623" },
                { "te", "e3a4ae4089108719816c7b81219d6cee534dc23f62813c199f425cd083118b3f1e1e1daad2059bde1ab173c42a1c0ce3b06248aa1ca1f2518e1c1635a1f137a5" },
                { "th", "c2f7faa9c3205dbfe37546ce9b7c444cb5308381d25a7f9f6b13f79acb9f0a2ad8e37eb630a36bd3fe3d92d4e17070942bffbf51c82ea95e019397672eeffa4e" },
                { "tl", "933bd1a4579e84139e56de93d516e0b21cfe9b9c53b51ff0bbf9f95ace489f60d6632ee731291d137c4658be979ae916998e96035a89ac4df6a43b84183bae05" },
                { "tr", "9d12567e9cbd1bd5b7c2a8b64627816c0ab5feaf8e062628e2fe14378a69259e6561629d4103b9711ad8a47d4700ab2a097df0bc85306c86086178d95b7c3d1a" },
                { "trs", "f805a524ea5ba1c4ac1adacd2de6f6aa146d87b0567dc7ea3e615f9f1059f18309e0b542a29dda24d8e5eb6f0409dbd8f91968d5bab42ae35ada505862bbc858" },
                { "uk", "7b31724db703c63a273a29ae471db7d011342cc7b2b55aed5100d5675653df612c1ace3955e6d81d090d7d33e42985aef6aad02c9f5642ae80e4edcedff4c247" },
                { "ur", "fd508967c6791b43f12e2d25bb3dcde26b772d15d725bec4b70baec6ad7bba3a2ee42c0d8b5cf3af6241e3016c0d47c031be3f29e61777af35b0e286fbf8ba4a" },
                { "uz", "2523cc30f38bdc5ebc2b0f54f5ad4761fa862ec3969bb914c711f1444fa23d3664b76aa40d48e37795737fd4e1e21936eb3a1ccaf171f178e6bf3d2178c00e31" },
                { "vi", "3742ce2d2ac5563f1f9d1e532290626b1e6a21442c3eb50b3fc8fb52ff4e5d869eef85aeb3c0ab92d83c4dbd2bd0f1fc1e8b6d3a710ddbf875aa960727ceeea2" },
                { "xh", "99ed1865b893301848c20e40a131e79b6a2b4d0c5d3e45d2193f91beb791358e5fb1f53539f61626b316d300085c98cf8299136452f80c0b66a21af2cfa184be" },
                { "zh-CN", "476150922278730ff694b0abdc473bee02ae26faa4a7c628e3c9d5e38a49af6ac94280c38f5e03c118c449f40ebe1f8462ccbeb2aae1bc018566c88f2c1bc51e" },
                { "zh-TW", "d439988d94788e3355da87cf5e98f8761764e48f92b07bf6e16bf87c239a20ccc858cc7f097f7fda4f9755297335f6b3f3fe4b73bae57dd3b0aab0e926785124" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/112.0b1/SHA512SUMS
            return new Dictionary<string, string>(99)
            {
                { "ach", "532f107016a79781e81988a5bb566e14e95dbcc1d6e9d309a3a708ccb918972b25b70fd9a37060e423cede62177df96a4e98163bf238c093cb54e3c5dc4135f8" },
                { "af", "eb92cad09430b56c8d6abd512324e6c56166acffb407fd4b54a0adec23b2604bb6203fe8719f0e6c5d453ea2e744bad1fa1c26ab07191c47fdc2ecd2ec8bfecf" },
                { "an", "a194c89c89978001338320f193200e9113b1bece74bcd26a2b2e5755ee8b7c4914c05e2f3ec202e5f16decaacd2b79d6bdd6cc13e1a27723f21699acd98abc83" },
                { "ar", "413d13d099824f95992b51f5668cbf556f8e7035b28d1546e1e01c4457d2bb36856674c182d5a98893dae7d754c6e165288e133f8a539348368ece1862579a45" },
                { "ast", "6e7f576a7b8cbbbf7554bfc72e34869850830da109a2d73dcd4ed556dc9ced1f1e906acdaa075408ceea6e137d49d775aa24e705843a5ef2d8d9904b69a22bda" },
                { "az", "e4d3b9661dcff9df8e694944240e8f542bdcc820343985a472dbe2598dcd4ca4946aa6a89d2e8f5c112dd6cb561bf1bfbb02c17378cd114b1f0610ceee07bd95" },
                { "be", "b4316a129b1ae4de658639ac4df919bebd7fd20cb7f5a9f249d09bed88c4fcd00826988515b7677cc3680914e8499fc91ad1fa48fd84cff936ad74334de0bea3" },
                { "bg", "ef63bd5c4068db6703a414c0c94964323632b8b936c79a171cd5609d74d9844ec89b40dbfec071de48c4aeba25bcf07378b1d82feea9f5b650a5b7ffb384a089" },
                { "bn", "dca103fa2ea23a9829d3387328392e8ea8059356d9d6400e5b024e8b2df1197ef3674b468a787d0c4bfeb6e3b543115616264bd41cc32818b73ce75b39fd1fea" },
                { "br", "74373fb1f5d3942a05714d5b155a475249c84d1d49dc94d308b79dbd537a425d4152f8a7fc0617f6da67bb077e84f97fdfdb8f0ffdf176cc2e579225c323e174" },
                { "bs", "cc2027df077468aa10f52d45a90e78b673fce2a0faea04b17c8f0766d442c4cd8a971a149c7b96c434495dbfacb77f5f65c65a38e4491efebedc9d3e9377bfef" },
                { "ca", "c6221dea2e9c3a885b5bc6a08be30335420169fa02ea7d90c1facc5745e2accd156509327017e6199e58b3cbab90e9934c55363fd19e1b15495dae7f2cfa6a9b" },
                { "cak", "820aace8c5aff12134247dec2194086041d86d98bcc72eb1122ca79f139898368f62815bb83cf460e2389fdbc96117e8a905664e38132e5f2a83d3d4964e5662" },
                { "cs", "ce6cb5af8ba0894d63b5dead9f68b990146c6fc496ad8a2e2b52c76bd8e9ef4d8df402a3084dd4459a31f701e3d59c733002b1f7b2b565092faf9518d4fa7634" },
                { "cy", "80c66819457c6f8a5b273132457c42920a858e49842e49f05ac14ac269cefb5569d75810f8ce1355f31add531c3fd18ff7c6a9368a9633ff21fda2e0503d936c" },
                { "da", "82ee106a58fa68ed12da6e5b138db3dc0cd4430bf9fe256cf95fdc451a71287037afb23c916ce96e9f14e94792925e8718d60e8966c0cc300e309fb1f4d4c528" },
                { "de", "6726ef9413fa3eaf9c0cb841c2372ef00beaccc1049aedbf16bcc8cc4130737544dc4d799f9496cf2c4b6ccdc5b6131a03502ce43b973352a31963545452e572" },
                { "dsb", "f135ff7327d46a291f22f6e6654a8c131b9d2a7a8f09a4cfcbdad85ecaeaa695864cf3c9f4dac465a3f608e7a820665b252624a91b3d1ed13241b134c0a5ce79" },
                { "el", "69f312f48e9ba74abb03d1f6ad324b938adb8a52c80aef8b87e9061a88e2b7287592d87c1ef2d1cddf29b7d320209da4cc9a3023eb90e314ed3adbbb32815e21" },
                { "en-CA", "1e458ea6408004c0496d52575a0c8e05f6881274aa03f1aeb26b1ffaa235ffa1b969dcd5bf8f047c943bdf6382f72d793346ca49d2c7ac46bb7178ac8189ce43" },
                { "en-GB", "5f281403e433de549659ce28069cf0a11e20b82c6316501371c4f481c5e9ef02cd267077dd0b84aab1df75575434772ca22b588ca3040006035ae54ead4949e6" },
                { "en-US", "217dab3eb05176476cee8ccd89902240bea574879437af13ae6cd09905605635c7d55d804246941d971a93a589a59f52f5fae5ec86e12863591553739907687d" },
                { "eo", "d96b1048fa321101977b63c1bced74797e35ee211bec0f18e43e4a416a683e9cbf238974df0e9d122f19f406791f1dc3972e55543987f08735b6d548ff50c28e" },
                { "es-AR", "77fc615b4420d94f240e1902fce13c3b098341d0f3c8d0b4197ace4625f87068fe98791a8390485ebdacf8c94b494668ec82cbc6a8c5c81f57f30752af00ed19" },
                { "es-CL", "34140692bba22ac1dc5782465daef1f5ce78670d6cb998c165e38c239e803077289b6188c6d2777a3bd7055d12bcf2fb3b233953406d2bf14978b8f9c1b17232" },
                { "es-ES", "d2b8bf9352681feb8d9180ad43c1907480dda10b0ca09a77550061d26a03416b1cb9a3e7c169f4f46a26b52bc4779fdcd1918bccbb60d1d79e3663074c6d755f" },
                { "es-MX", "49580820e546e6f4d90c4714b2895f7038958f14f2dbd013a96e73efadaaa7ca043d26073b9974a97c9dbbfc6ce3c978beccd3b064590ba80f0a6ff179883b77" },
                { "et", "c444142046ecbce3fc30604f68198cc7546c837d76f5ae85ae6dc69182479685fc35da23306ba3f0f3d6f5a4e92a78bec4ad7aeb6f116dc21d1e01d34c0517e6" },
                { "eu", "69115f4d117b575ce3982205ea776eaf31eb5401568cee21245db54bae940e93d5b196619aae6a1b4062d5d3cc47b2212e9f8039a799e4d3280b27c8a562cba8" },
                { "fa", "b09b9a41b8424c0164c02534451a464e99727870ef9138c47ff9e2abb8e27220dcdaf67c4b8289d3026a05041e23c61277c056ad07140dfff0447d142bf4e097" },
                { "ff", "0274607b250137c0b7aab019e0f7f4f305ae98654dd9816c88d41808ff278a566316c9689ec7c47788127a91a4992937ab5a5821c06291d8432684a9feacde81" },
                { "fi", "7b7bbfa7062c8bfc88c595beae871cfc8bb18fcbc284b024bd838d498588dba0cb5957dd59f0d2e4d4b122db38dc4306639ae9c92095c004bacf73079c970774" },
                { "fr", "9fcaa9f06a695866834e11c26b6c61d882f8d98b630298bce8ea92bfb78d52f3abb514a375d357373fc2d070bdc4d8ffec02509b888b33ab2b0a4aa5e3b641e3" },
                { "fur", "21d18d4fbaa2726af5b4356787dd8b2085450af0d66a2b3e382f459a296c56764ecfe01046303b2777c8cc78e2144ff6035bdc478312e2156823fe0fd8b85ef4" },
                { "fy-NL", "aa676e96e41c78dd6e5b0f06b54534a8739acdb61d3a6cb553becc15038386e1dec998ed619a1596a55e40342c1341b5b79d0069e4de40d1a51a115117d63e8d" },
                { "ga-IE", "bf0053be7f1e1453ea279c4d895a05640c59c6629ae8808ad1a4f3b26285a28ba788195c5151fea1d63d04792f7af04687e1a8e23c779e40b07c11afe5775fe7" },
                { "gd", "d3b52094e410ddde364e9038ae4a860942e71f85b76a3a6abe9759ae3a0557d9d93d05164adc039b6e727593b83e7b2fea12ca4f9cd13d396c89efb9e29a3332" },
                { "gl", "56492ae64a7f473b7010bba1734e82ea3bfd3d4a30dd9d1626b26e4734c2670f253531749c8372d739e400616a53919ffeaaf166e4d34913d93c893ba86b812e" },
                { "gn", "7e3104c92df810bbeb20e4f7b68d261deac2ed020cd544a2ff3d541d2e168b74ffd4b5fdf9c71c0b06b14a64a15c8219daca323b7d981fba7e752161a8633a81" },
                { "gu-IN", "f2e33dc512fea447006ee5b5db33f3a42bb24c2fbd9cf40ead60d4aa3e6b2b1127d3de392c183f779852bf739dc3c9f861b586b4e52fdec7daf7e7a7a157a5ee" },
                { "he", "75689dfafccdb54b27cd9dd582c8202e8e8f81683be9fc2768b296d9804d0b0ba00b537e170e2a55d9eba61f698de364fd5bc4fa936860ab680b254546c57752" },
                { "hi-IN", "89352f2fc4f1ddbdd74f65e8d1a4da22c610d2f9c37b5152437809ba3caaf44636cee6704daec08ba91360f947837f4c160238ad58882c57c0190c5cf78f1f4f" },
                { "hr", "37de504d63766cfc95959107c91bb9053be4a1d5f57bb1e93473f2e0d9a1111a2db986afa6bd56c73433aa8c5335608539f7b18e0835a7a60d52f609428293de" },
                { "hsb", "b479c171d32c4ef9703804b616d1ac6e99c7560e2558ea2274fad2387d15bf264b4a87c9b3cec3fffb6706e2d37084dbdb978fdfcf65c5b2802c15df5791b3e8" },
                { "hu", "ecd00127e78be6d652f053f113bfd93d058dc25f0f01d1dc351456a835b05faceccd0d5ea5579c889bff49d252fe8423aa24f386f016a882d54bc383345562cd" },
                { "hy-AM", "887a04ab0c32f025cb359dea7b861ac0f19a45f7fd186e6752e7386886ed17858bfc1bf4c3f1a52421375b10a6373c7f08243dcca007e7b249a5eeeb088b288b" },
                { "ia", "e1c453891831893cecf0ac0bb066039f722c3bf57d4f6e54651fe133ba586af792422bbc2459124ccc201090094212013d69c956253ba5d559980c6e137e690c" },
                { "id", "5bfce5b063c769863c6e7cafaae384b1d54464209b0ff48672861f5153b751f219d943e76676e81cb7da6b6adf69a77907daeb76f3d8b5606ce7361faf52f200" },
                { "is", "41f05a916b75a0362b999d3d6dffc11258ce841a3354ac45085f9fb59b07ea6c69e3d479abdc825bb0369fefa88c482be1a2b9fba58152e8157c942a18973a31" },
                { "it", "608dafb63de1a757418e35e6e69324eca9fb2e65f12ed243a62290d8bf7cc2d96a3557fe4dd941a320c7ac335b9553bbe0c72ce49a0bf6d3d5c9f78cac004441" },
                { "ja", "5b2538fde6546e8aed9e1aee7e68f1e191ad165e10d573b42cf3b3bfb92987d3dcf8129b7812fd7e2b01061e3a625bc1956fbd6bbfc61b568af318e570cdc1c5" },
                { "ka", "e3f1563fe8077421f0b58c8e5f6c43c612368d076a20cccdb9b21169246213ac07dad300638befc35f3bfd9368544908934ed6c2f93a6eb3279ca756a4e27e78" },
                { "kab", "f90a0d4e03ca883685d2dc609b2bb278d9b5c2bc5cdaa7713978d1e32cbe4f478f51e580fafe890afe200e5c55242782938320cfbf1a929032740e50c2ef14fa" },
                { "kk", "cb67862e05fad9d373a2acb647a908c26f7e2b5d9e47443e2a6e15cca6c43906e4daebdcbe07eb07debbedeb05a8fc5ac39e10567ab856cae3328758b04dc336" },
                { "km", "cd0dac3cfee9d2f5a50381a877f551994318bca51c8960d6016cacf99baa3e4401e9f94eac186d757b1cc845ead693b7a8bc20ac3ab7fbee8c621389533ad443" },
                { "kn", "73052cdecc7ff45659bacd1bd22dbc4518e55a411cbbeaa2e69b6949188988aaf6991aa6729adb16ae9d9377e339cda079b55e728215db3afaf6218cf62fcd86" },
                { "ko", "272fb8ae7002a4163174e7ddfb3bdc22a2457f06494a975c3d80f6d78f07eac902a45f4ab67b7ec4d7d87945d883db15ed3c5bc49f26da4903c10addc1329412" },
                { "lij", "9f15477033babf8bcd03ffae9259c92a327e1bc223bf5c63d56c17ce7aa9dc78de8cae4aaf1fb2d1d82637cdd2926eff2c42a153636f45420f780a8d234a5e44" },
                { "lt", "59258bb1ee68e9ff6e745799ba32516b5bb7cb4cfd073c5df6cb9bddcc7713a1aea41cf7ea23d60580768c0893322d7ecc30cc9bfee59f9a19cd6cf1f92fea5c" },
                { "lv", "f83811de44201b3e6f598ce3c67f356449855035429933bbe086324adae571b50d9e6afc756d7a9ea0c7c7a2f643877cb6f35bc4ff4945ef2fd9d9ecbaa267d0" },
                { "mk", "1154dee16af7cf92d8ddbe57c653be90a6e02902174518a9194af9b5300fc03e19302175787ad82a7a51338a65ec94cb1f50de22311f91bbd702f494ba68d3b9" },
                { "mr", "b054ff7281e22601d413a404068af1dada28844f9a9b318629151b737fa35e791717eab90dddbbfc23d4a5771e80de7c6d848eb84d6fea00673ed24ed5259725" },
                { "ms", "630262c0495383792036b8b5ace7e74489d9e0e6e4b3cede4c05d0023eb0389bd1c6731a668bfb4995b9d5dbd8a1a9a4000a45ef18584b2d2dc0aefe1c179393" },
                { "my", "5b7bd28a9cbac052e34ac0be478c1f659fb78cd2b75dc6544aaab7c1f80a1a086feef690bb507e0cd1d736d3194edcd989f3def8ed0878894d116887ab28a0c6" },
                { "nb-NO", "d60367bfbcf5fd408cee0526ef7b1c9a59c26d4d709adc3fb2a82b7be8f7a6203e8a126fb6dd6dc1ea7a97646e72b224f0466b8afe8dd76c08ab5a0d9ae4d31d" },
                { "ne-NP", "b080d8d7e0185f53426c786cd6834cd15d6d55b1d9a195aab49fc555e62c95c03233154693b0378928811ec7f7e726b4d6f32527e0f3964a098da5afcc0565c6" },
                { "nl", "31fc29eff7d0c87d3569b3054b22dfc83569f862ca6381c4e7eabdd0c4dd329b865594d0fdc1865ea73fbbf667a0e7d66e92a1c58847050b817307b0ac7d4565" },
                { "nn-NO", "a1dc08bbc3edfefc5a2b2efaf42ce431757969bd492a68781da358b4bb8f60246d4606a4478153ac71531752d07fcdd0f16bb6df855dfeefe3672c3f6c9787a6" },
                { "oc", "564454325025c62726db548030b20ba348dc9697be73375f4ef9717300612c9d2b7a2b8cfacffee46c639a11fdadd7168756e8e1da3b7f21accdbfe0824efa7b" },
                { "pa-IN", "d534905b8a90b9cbccdccaecc596efe7cda11705aeb67c67165fe4f18899ed4d98e65061b23954eac352692865652fe09dd1fda256a118e7108bfcee0b518229" },
                { "pl", "beffebfb3180e9de962602d8e80aaa3292c321d02bb306bf28199714aa10cd708515d85b834ea7c5b69d40ac58a20a5b4b279fec2eaba4ae953a3a602ceb9a9f" },
                { "pt-BR", "2a5ab546a3d56d447315ed69db0e261f2fbdd88899a9509500236b937648890e628dcdf74e779784dcee5a6bc49341bbef494a9bfdaae3bd1e61524dbab246cf" },
                { "pt-PT", "8dfbacb6ea2a33eeabe6ccc5bf56e45d8b1759bab8c735efeb53e5ec28157e4f5c86fa13043d45a9df73978c4dd7ca6185ed77f157edf829d35826843b5345d4" },
                { "rm", "794d9483e11653407f7c730ecf1c86d058682cdf8bd6c5e81fdd34018e413f666c4be27a19b837ffcf8e7c2a846c1b69845a86b17e26b29dabe75c512e2ef594" },
                { "ro", "38cff8553295792bd23fb1293bf652aee391609de06c7b65959c20fbb1ff6b5ac6cc0c479e5cce1378b163a7cef1e87feed8f2c2e065ce127ef977c9a2ad9d7a" },
                { "ru", "67663086345f9ad76ef5e982c85ed88d9d3680ead1f81fd15584d0b0fc53f5444bbd19725e167316715b00cbbfc7d96332d797ea78a14e6df5d0adf9e215ee8c" },
                { "sc", "e1b7e47326c8562c63d9e6b01b60553a6cfdc6184079a634133b8370b56ba59cbd9f643a6460cbb21cf1bc48736a173c95e6dab58670eaf636f1dcb0ef9ca1c4" },
                { "sco", "b0faf00bdf0ec3eee82b9a0999955381ebab20190a1c23ce1fdba5029a98dc2039f4f4ea862249c0661bb472af593b54f5ea4e09d522827dedfb83b973c16dfe" },
                { "si", "e1e598e79ffd30d883d627eb002bab50cd0cbcbd6d008ea7f4c0bb637de8dd787248f2538a6bf039cf88b4b586cdb595f03171cbe5144f96fb44469650507b0e" },
                { "sk", "35e9a8e010b7a4b11b343e583fc001d4cb6eef2415a2114e68b6bb00ccc535c4eb2fff9a66975b583e4a9a7360271b889fe4b62be2e98e436f70b8cc1944f35a" },
                { "sl", "98950b8c266262bf2ac2f669cb6943a228129511937e3a4459338223a17875d1a602e35f65b61a43ae859bba69a2bcc139a7bfd824e3d588050e49ef7f248fc8" },
                { "son", "94652711e696bab742ebf2d44a9f79029cace1da40e683badea1d2a1d6325bae3f3ec7d8785138d8e887aacf2be6988931173ef912f44438f41f19306ad408b1" },
                { "sq", "4f6e6071b8f9d481087a745cf017a09d94d798492b482fdf667a5a86a93f73ec08d8e3f46ff1383a1ac82266a2a6ad88f55195a026b9e272d11d771d5b91d0d9" },
                { "sr", "df2a11b806cf4ea226523acef8b0b1f2767334de631463e380e48fa6cf74c0936af8f999d2a40a311cdae00d4c1f5d35d92c89c9632f8840afa891894ac7003a" },
                { "sv-SE", "cb684f84d0becefe7356929d37759c828f619ea101aadae501e7a0d61640ea33fb58083adcc73fd5057d055e4dc2ae3cb3f9f421fc955b966917901097f0a56e" },
                { "szl", "ef8ed5163b12ca500c2d1a9e6fde8fefad71bdcbb885ccaab27cf992f558e61ff82fc48d61650a4602aa5b968c8c1af15d81df906b736c736c537ef4938bc3d5" },
                { "ta", "642310be0f07c771f0c1fbc345c8b22b7440460b374c99fe07d3d695f0137eff79d7af54a0d8fbf350d6f0bb29b9ea161d05bb863c606963421465ed9e6f6260" },
                { "te", "dd600b7c684436289f1746f41916ac2c199faef2dfbff36eb929459c63e54425d300404a561b51c92855591c765ff249c650445ea7cdb5afb5268eb6d59ca3c5" },
                { "th", "cd43cf9f3c60b262190b8b815608f7a87833d87556012937fef1ea597b8c1518df24874c5c3d2ca0ff314749190382d808d29554c473c1c3de550654b3b0575c" },
                { "tl", "dc8a912f256c5d9857ebf79ab5224cc14575d943bb092b2bfeca7c2e578a103c0c9b2b0b9a602027886990638cb8a87163be4ad8db546e6279a1dff2068bbef0" },
                { "tr", "499eb9adb0f7b6e75b7d44f2e1c074ea39b570db50c41eea70b16a6faa7a9738cf03c8d5e3304da6923a6dbada3f020f9b79c2f8cb3fbe12de704a6c51ff96f4" },
                { "trs", "55166f2918c0d4282d7208654d6373dbf253022617864e0aed2733d0cff1471657e6068c2863c941c2d6f6e7641fa8ca11476eae761b37bc40d8f0e29b032c14" },
                { "uk", "36af56fffc6172a475a2bf32b719be264a89dac2163465da8b3ae16da394643ae67f581a1f60e02dc87bc779801ee0f05999e6db8b06756f79d2d5a26b362e75" },
                { "ur", "e22c39278cfe22785c0106b1fa42b4c8a8a48ba93406d4e2617e02874b57ca0f26559c52379493a3c6c819d0df83b2403b4726dea66ba7a3b4bd41a42e3084b6" },
                { "uz", "88e6ce84d4bf6cb7d4a857317bde1addcabb1fee3d3bb9f830f817ac0e0936e65905572a61d1e21a51dfb8d069a6bf5f22bc3750765066dd638bc8e68b37ad58" },
                { "vi", "8222265980544b79eca84a0260827b97dca4820f77fa2d48117fde076e94d56f79d97915742c722ef9fb6c4039205491ef21e38fc43981da4435ae6738df0804" },
                { "xh", "ae81186903a963781f8c73c811e86019b4becb6d36c521df4889c7f6ba432499b053efa44278571758d471b251753484f95705b778f04236b272614048bf8041" },
                { "zh-CN", "f75b422e29168514cdb58b5b0a0341618a17c06cf5750326d74956086ff1f9600000d999f4bba63128d4a7d9fcbb07d6ccd012ff50fbfaf2b9a0f7b1d5e1e618" },
                { "zh-TW", "25290bd4772cd00724b31835cd7119c41c7ffa3a2030cda87fcfe9cba3f15b32f7990b3446139f172bb2e692377e9e93cab394bd6e35a891e52efd1cf702aadd" }
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
