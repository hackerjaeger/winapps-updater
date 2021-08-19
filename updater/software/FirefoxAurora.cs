﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021  Dirk Stolle

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
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(FirefoxAurora).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new DateTime(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "92.0b5";

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
            // https://ftp.mozilla.org/pub/devedition/releases/92.0b5/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "fdd262e9efc803f6f98ec120952fb822e941bd2530ee1ade1e6346d5d24c84456bfb4b9407cdfe3c7c5cc9dac2adb224f790625fb8be5fd7a07b3061585d9893" },
                { "af", "f0b2c79de1eca77c3b040dc716cda9367deff7c4124d3b105997656512d883b7f3c9c7574bdc18bb606b4e5cb2430fd890f6ff8ddac0e455f82f4c1e8b03e236" },
                { "an", "90e625be41c94131d0fcf44ace4f5e8c238dd5c8824befd5fde457d9538193255889b586223c94f3b05ed399efbcb4003f0317d249a1336d7c4a9eebde1c5545" },
                { "ar", "e1cd7d925fd6ca4e32935d042d5e84f5f49e11745ccebba23a39acd363586e92edcb3e3f9b2bcc4217bf227a47498f90cb8834efab31221babc858d099a07b77" },
                { "ast", "dec60113a123f1ce060be205890e37acaa79cfdb01287d7b43858352f6825197d6491a1788998bb431150b4f6db9baf3ad7d0531b6ac5a3043e8351fb632e788" },
                { "az", "f3ab9bdad14bb9b81e295079dfe14d4d6c40281be20e20d8dbe8514153f7df85d1f4920a87d057f7e421d22c473ce53f2017fa92874093c47bff8291e4eb402c" },
                { "be", "f996c7074fc122c0556bf68bdb8ad9510e68b273bfcf1354effd06f93815dbc8187ca0fcb6335c341083ae09792ce9f5c3ee6f81dec19b55accfa7ac2e6d02b4" },
                { "bg", "4c4f837eb33064c1bb4a61a95579cfdebca647b6d8c1c730adaceddb7713203503c3498912e281f27bd492411928fb710b636bc9931af1fc7e99e4aecb67d150" },
                { "bn", "b7b940dd6fbf8a284176cdf2f85a3588c6d4f6ee803a4c1717834f05f4d39a707f2e2f4985d8a46756b31d92a54de75ed6afd95fd839129667fe122a3451c71b" },
                { "br", "8c80d7f8be9d5ecdc4757d94c0b8bf5089441f01d53a059020c4f5d80e65ecf2e11dea6910c488d01e35a10325127e7007dd1531bb9f755455d6cb607315962a" },
                { "bs", "9fed5ecc3c69a626cfe6293086d19ac9ff4ae266e647015a1d507f8311af46677e26a969e36686416753de46b0ae5b8106cd74cea4074e9e7bbc53c22bcb14bd" },
                { "ca", "2dc67bc7c79e3b3b714dfd36f0292fb2f561cbfb3dfde544fa7357a52f1cc5dada795839357197fab913eeaff4f5fb2c91c6cf23177468209cba7a6a14f4aa1f" },
                { "cak", "bac7cc14f98bbee85f3c2324aac5edbf7651e05c421bd31efd42ea2bd424648451b653657f44e70ae70b7d01b3c097569f2baee143280687f4d7cc7f792050e6" },
                { "cs", "7b2c34bbd1e90ca14f4717a36ea63f3524f108a2e482942752cdeb4233180e96e3124b7c573752a8d4b669dc3d05c26f7acaa3dde50b90bfd168a9c2df875a6e" },
                { "cy", "04f7a0ac8e581da1b8bd9cab5a3aae846b826e7d6102e352f905905070dfce341e9a331819cd46390d87bdda7bd2201edc56231dcac3f9672c5691590d414067" },
                { "da", "4ef152a7140982c5d070ac5196a637efb354a2434d863d2c48956b6f757d9604259ed2e91f30c3743f7b487ae4a5b15475dadc7289c5c51ea5b723b4512c2f2b" },
                { "de", "c3eadc9103e395b1a2fabc15fde93cb60adda2e218155a4b9c84b37bbb69c494ad9ef15170101935c2bbc5dfdd64df00f763c1ee32ba1ce7a1c923461141b6b0" },
                { "dsb", "0f62314b107c30386a87e88374bdc92c9de538660b770e5af8d99105325386f7b05f8b9e675e9a078f66073ce01c0ca40d3646347b6c1ffc48cd31ebc53a0bb2" },
                { "el", "3f5e399203d31d7cd12a0b608defdb9108fadf6b360da492737a3887bef3c5e5483dda20b88add06217a9a472002c9d66ed32eb2740621fee1fda44b64472a88" },
                { "en-CA", "7e754c133aaf1e1afffdac88a0a1ac6ca787faeeabc6fb231d9d033166838699df0c83f951203f70743f17fcb55071eda0cb8b7950281377278037081d7dc6c6" },
                { "en-GB", "641d869033357f1687f857764793e4d2a98ef7c182f53b8c8c7f95887897e9258d53c1c9e13b570bee97cb53e2e2fb9df0569f6835f62811b58d584bea6b1130" },
                { "en-US", "b0d3668b9b5a0349a1fab27532c25219dc2651ecc02aa9de6ad712553406dfe4dd688e8bf5689eb2a9a49d1a1fb5ae97ba510c1e3395902df40c92eb7b58ff76" },
                { "eo", "d6aaa73b95d6a23f2fa1c06ef26c3320cc19511a5f1f66d7c014fa498bb771e339efb384fa6ed1297472582ec55ede35cfbdde40773bb3df353a1a3525820a5e" },
                { "es-AR", "1940e2d01bf6a3e30ffb00296b51f1caddfff2fc41b027a756891c0697a3a4c497e5ce8668ac5399684a7cb37c8ebfc706843463494c7e22ab55663f0a30936c" },
                { "es-CL", "e18d3e6994012ec63cde6fd2e8cfcb8ea07eb5a79fa87cd34ab6fb8994b9a501f0deee1c9deb312085967d50aab1205142efe99f569e7964463ec6bf9ca385c5" },
                { "es-ES", "baeac3863105ea7ded593a7862d687c9a710f3188fa64ead8e1fc3c77fd6ee23889c81cd56ad52a8d7b22357515220b452e6e0bbb73a6b1c3a7afe648d7ad7db" },
                { "es-MX", "0c27a2d22aaa5eb6a1ed90e8baf1e2e56a21dd31a1d60198fc8bbd1aeedebe2960b605e6acee09939dd8f01f9c056bf9642304d65d6a68878628c6bea8f40b16" },
                { "et", "db31f23731a29818b8c087f5c054e555029487dc69a88172f80413a9a85e477851aedb6854ba3e0bd5fea0982cad331974c35d49622dddfdfec4be252929c8b2" },
                { "eu", "beb4f70eadedbb43ab82e2be6f486b1d26f4bd64c53b05a755c5c2ada8e0d5ef9b1aaad3db4a4b5d6bbea362fe00776d986a964d2fc9a441e5dbc5a2af72d7e6" },
                { "fa", "5cbc5b1227b601cb56aea7e4e31f214b71acd9628b6acf8869aff0d1ce4375a67699957ef528bad6ed153d3470493b353646796723ab3425eddf71b509d6202d" },
                { "ff", "e545d19fa019bdb74761ea43171521029b1b6f71db2af70858ddc550e607d353817eff8f820505ab430144f863769d40e8fd05c77eaa5f28382d91ff34117f30" },
                { "fi", "078600f5db30c35bfee484f64e26b2b0a099f34e370986a2fea3a2741f5ffc09e3e7c82a265fc7473a1619302c9e1f9a68985ee57de1211706a41d17892a47d7" },
                { "fr", "8409057372ae51e5faf3fee23eb15ab408886ed490f498166850d3a969975f6e37256fb2a163e0edd60b66a422c85c8dc24f626870431f02cb3133a8b2011d07" },
                { "fy-NL", "9606f422730a56c6b7b066f20010eb2ceb7471697fc6902a9921646db47e1a5e0f0b21fe0cbbead117d76f49a66021f242795a5425148af76ed34a427e2310ae" },
                { "ga-IE", "c64cc22fdfb5f189fa9f01d1179c4ca13f189b55fa7e915362787f0662ed26049be61b95de5b45c357790ee45d00a889fe60b101fa5d4ce5e4a7e65049f64736" },
                { "gd", "c9a6bed9b94ad4dc842fe04eee8f8241dfab1904785b0ca28b25b2df3ec3e8027cec5eddb6bafa126fa993d022e6da71e84b387245ac28b53369f5170624bd9e" },
                { "gl", "b6466a1a52a18c1430eaf1a7963be8a1ffa025e4f1706cbbae18708e7554b8ff3fdc871574fa731474d94cbb57563140a9970e973156fbb8d29ee5dd4d7cbc17" },
                { "gn", "2fe79bb6a8f4196dacefd3ddd96b5cb110e57c76fd8a6c74753b62ccf5c329d407ef8948ed45f9d25ac23c92f4b637b99fdfbeb21f6eaca809704c762e464cd4" },
                { "gu-IN", "11863cb23afefbc51493c64fd3222de2f5bcc0450691fdb6d627dd7d3250ef4cf21a156b51b9530a6db79bac4f5253c111765697335695cc94f9203f1d4ec0e3" },
                { "he", "86d861a2fd64224250e754564e3081090eaf9e303d591f734a4ea16b8b3926a6a390cf281644c8b36f20d1477d6ac26c62def33620fa4c90a8a5bb5994342b89" },
                { "hi-IN", "6e5fc31ca83e516e3175400207ad2504a5b60d4884e1ddea08f4845087eda2d4b6e51fb5a1c30db04d7d875b1cb78312eeee4782d037a2dc6de699a413840106" },
                { "hr", "72489371b851466be54225d62129407eb6afe243a8894a073ae656375d615625c08dccd1c33cc3f78487a2a8a6ce7bd47338a2b7cd01982c9413962ff5548c24" },
                { "hsb", "79d0a78a371022557560013db882273b2953374da41d84e6fb2c7736a1aee667c0a038e946d3cc050e39963d8116bb97a050585d654fc70e08c00a8a0089f320" },
                { "hu", "4f62a8269cce32c2081347dd800f8fcf3cd478d2b2049f23e4009da0a5960aee8d0cf4e2a976c1d0b6196c959f06ffd5b2d3ab880e43d5d2b2f21d3429ce9b06" },
                { "hy-AM", "a70d569bf5166fa211ac4d66cb09297a81abdda0a6da1023aae22b9213e1832e31df675cb7e1b55ceaec4b361cb0a02a1eb9156b618977ce65122550d18a1d5e" },
                { "ia", "7830d67838ade6df25af6df39fe4d719fc4f5507e6d8c2994a4b6659ea024edaeec2dc037ff6bb942913b33c5a6afd1909ea6869277be6c0a26e4d48b12fc05b" },
                { "id", "dda577c58d3d613d9fceb567994534c043d4e135ec0098572ca2e8238a63ea46e2d5aacd9cc86b62122d065a38bbbf232ba7c76b3e0af57a9ce145d3d8a930aa" },
                { "is", "e5e4fa2eb213a451eb09c3c956083d8089bcc53f086e0621765fe1a1f701650520248578864e6bf8f525472b1236d117d11d486627156153b2afff3e80651f8b" },
                { "it", "d1961588ba0262653fbe53b9632f16edaeec2494d49a08a0101b3a0bec4e0d344e9ba5806d8037eac0b82d431451afc291d48b0de652873629f949f04124dfc0" },
                { "ja", "583c04bcfbea01b2f4f43bfd5ab835d45d5098597450161b59fff7ca7098205b7e1be1325784cc78032f5c02a54703c8b8512178886ce227b4eaee3dccd70a42" },
                { "ka", "c65b841430826c9f4b8005f29fa2929ae35057b4f8abf9c5f83681e104108d3a26dfeae116190270c0a7498a89fcadda8cc3d7e9e853b770acd7a483123f0afb" },
                { "kab", "79ec60f734ab0d05b8e643bff7d2b47f044777a1d345a1d79ad2585aa5423c900f165949bc9225f522263d2e37374da1adf9ab5e55cae29584686e8d6962e7f0" },
                { "kk", "4b95f484b04b0372bf3299d4978949e74def812840f970ff0a97f0a356453987c5686d4d0a11da86226cc5e78eed9b3e424febe6db960ed0abaa37597fe7d06c" },
                { "km", "38a1e37d4d16f97268db16d04364b57c2ad4d3f4cb4f7bab695b01d5eddd068cd24e4923992279f85f09edf2eba326118f8f6dcc5403cddf946e07ce5144ba37" },
                { "kn", "3c13b0e7091d3147480d31e93c107bc0c0fec8bc0830a7d38331d7c79fe6ef506001215d16ab8195428b3c629161d975ed5f5c45fff40c97eb2bcab35bbbbff6" },
                { "ko", "e89c951975b6663ccd82e616fd6b908345d63c211e11baf89a7191844de8b4148d4f89a03eb578f682ca0721c212884c46463d7488a6c82e58f5b654d9df6adf" },
                { "lij", "9c4006572f19b59b3e778520fc8dd5e62ef3ca1649df14ebad5e24d24959dc2cce96db216bb333df005574475c00bb408c24b74e4ee85c8d7549b6d45e749d81" },
                { "lt", "5f577cc7a59b5e0658e91a074f7966d79227f36cf93f8bb7118bba96d24899ec23e3435f854e84f71066972e498c55527a6dba366e35631a9e2ce2c818f07f92" },
                { "lv", "e5ada2d521eb1c7a53595846845bb821c4a95536256afcb508c2f287e648f71e65438c61e9c6151946d75c087daaf55a30c39ffa6936c823d81a6d8d1e9a883a" },
                { "mk", "2af6248dad9092f86cb1b802fbabcf013cf0570081a807a0fafa16bdf25a955eb3d8fdb12d3e442cfe1bdfa36386011a8559dcbf11d32ad1c0d8c44de638c70a" },
                { "mr", "b898942bd081cba2e3406b0a6b5241b1e6ff36483063fd7106a400811136288691013724d2fcaf3103f60cc17d33a638bb33acddcfa92c41438dbf925a164bb6" },
                { "ms", "5ce064f48ec9261b046a9aee8faebd05db804deef9e6264e78a3c23ebfe9e3b35fa32a5871297db5f065d6982e3ed32e06cef454d0f828d2eb60f18d3c00924b" },
                { "my", "cf9f6af97e57bf018cf46cbdd04c2b7b80c60d39f73ba6d7d6d98da10be099f704919b288e4d8a6d61ae4b5eb29a8d21c76dae9a90e00f199c1f823d4ab7b2ce" },
                { "nb-NO", "8c474ccef6dd0b405b9840ed23aab258eca83a1a6e0cea8862ebc6d4d50296d775d8c5e9b311bc40706629e90162b739abb1902c02d1ffc196056f279459de04" },
                { "ne-NP", "27e395d6b347400869f493159b1de204435e7373e3b06da654cf29f963386436d998cd73fe1143f1c531840b4981dacba4c62ff21ef0669682e96a86197037da" },
                { "nl", "978f6bcdbb2f08f7964af4269cb98661a0a085b208b7cd72274a1b0e91de5c86bb848cc2727b6486a30a090dea074f5544d39a15272686db51250c9708f14cd4" },
                { "nn-NO", "ea736d699f1916e16ec9070172736cd159067c77c487674e42e6f9d91359728f94158e0c0f36058940587764028b63be3cb4bd22c5df19d1d8a38573cea888d4" },
                { "oc", "f413d85fb5e94ee05a937078030a261934e126f887757d6aea2d493aea6a191f71c5365ba4cdc3ff0ba788744e1574b8ac5036b612aa87dc125371f43cd68e36" },
                { "pa-IN", "c34d83bab11f853278023f44a9edd2b0dd4551e4b77e1af7957a21ea1c1df00f69310bf7ecf5136ec719c9185c60c65176eddc1e317547c2ed0a1241014878d0" },
                { "pl", "208659593f21d68b3bae0cd4be38b67237679b441f7321c5b9200ca3115d95d72b3c7d2d2dc3a1a19f989fc862379fee33ba44c52c04dfa729b73de38635f1fa" },
                { "pt-BR", "890caf9183529a23c0ac5f58a3705409716a9f2c575eee327322dc2d5108602ccc843107cdeab081324a9612d9fd6d27eb46ab08ab3310d423f50f2f4a6f309e" },
                { "pt-PT", "a3e5a38f56877703d3d02ae7e32f47ca1a365995483cecc3a13f64fb88886b56e4a94607fb904d02f123396377ae6503f10b4409394246876cb63fe7a7f11ece" },
                { "rm", "0cd332a79d74884f6f6fdc27a062a747b79dc3e4ad875fedf7c313b9eadd332522036782899996858909bf878bc47212da88aaa7270572a68894708f3c6f2f21" },
                { "ro", "26adb1fa8f26493ad342748b7c5932156ec6687b359ad3468380b71c879a9129f35da068b4817f1164ccd35663bb16cf46b1c2ca97b5bf02fc96e41cca2346e5" },
                { "ru", "5a054940afe230cab7e4f30439951ef8301bb542bcf6a160f3ec7634d61b0a65ff577e1fe9e5b6f102f50756c4aca1e958d3177120199af6d3a089559ad655fc" },
                { "sco", "66e9f8d50b4163bd0468e284e3caa225962bdaf7a74ed95dada1ebfe32be5c50b30bd81b5b73566d4ab0705a8dee0b7732c88aa9db86cab47d4c4a5b097a6b67" },
                { "si", "1e7507f21859b578f016cd1cf453a244e61bf1fa25506a921f79c4bccdcdb66cc3d537981c4c20e66e2e65894437efa99389ed6a5a429037c2f29588c78d8872" },
                { "sk", "13fe957ea06782c63de0e7f8fc7b3eaa9416cadc7c48a996535fed9df03bacb5d40056ca22cb8f147e5622e54dd4a2ce7bb01cef72d7c8458236aedce272d1e5" },
                { "sl", "5bc3b292ec936c321b670d7e1cd311a94fa687b645d925ed84471e1cabf85b9d652f73055115064b4130f469067d9f8df495b074ba38af192160214494c4767b" },
                { "son", "eb6c6fd8d120ad4aaa62f31f3ac0f2cbe0431d4819dc9675c100fc8e63ae2283356fc520cca756c636a7eb38caa05bb28f9c9417b8e7a3ad9f5454a7f11b8d41" },
                { "sq", "00350aa5a2faf0254fd79c06e77dbec226bac2b01d6749c56df9299170b1e52228d3f4a1bbe7b80957951b7997a0f7b91ae5e0648b146785aefadd5f3e83c5f8" },
                { "sr", "d772b8a5b299b0817d551b76c582dab89f83fbc20727f3089dcd8b125f44a98967661d97154aa404beb0534ae0ec6f0c57d7ad2c4444297b5c370d3fa704cf86" },
                { "sv-SE", "f9f47b00366315befabb1bce2564603f638b172ac4c4950e0a9242bd120039d9775439ffa620d3c0dd39b08a885b0062563b23fd4b90b75696af43cc0eecd6d1" },
                { "szl", "8721816b3a0785df73f0ff0b3fde5be7f5aaa82937038f87177865e7c1201ee0808505d1d17458ead432e1409b5183e17a4476228b78e8a9ecc3188c6e6b37bb" },
                { "ta", "dc4d95916b9e6bfa5df39f4d94290216f76e16bfffb4d4321e6052061f8e01b508b874dcb2dae70fec6b956ef57b6cd157622f40254d0319a8048bee4a558529" },
                { "te", "62cb5db8f7a0e4d57b55b7802096fbd62f838b44fad5339d55d7a586a1b5e4ff70b95f69552af6d61b08add94a8a20d7887f968132f2edb7951b8911e2be50e2" },
                { "th", "997816184fc4930554e3e637753c8b47c45e15c0c187c7d58e9b11a93e8036619d4b6c5e255ec51640ce76fe4c42d8cb6840728d493a65bd767d871a3a8f6126" },
                { "tl", "d9962469b2e71cd507dedc3f0196aa0a8dc9067073c63e38a7cc80065d7d8a0d506dd058895e47f6203d5038443d6153a9bf5372f636bd8802c10de85b541afe" },
                { "tr", "84a2d49c7a75ffa1c61c3a437c308024d9d10e8154d7c5ae147dd6540a5d014b9b8f2c41f71f828cb71db91c87c413e6471fd84f93f01f7273d5d39ab4c12a29" },
                { "trs", "75df2088c11de3c431cb04ef4904081b9fee09913d52913c103e7b775eae1d1e330b126d1f35cee298fd38b43da52ada6fd8bf8c8634183e2c05b4ca0cc6080d" },
                { "uk", "3f00dffe340854b5b9fb9a1220906f53487495d41a62dfb87246c3fad798e9d58f7c0990b412a50d6c2f4d1177d4026515c48b15915de5e5639f97488bc7cb8a" },
                { "ur", "729071850e2bef86e393eb119399ec049b8efc7d180c9eb6635d6f368ed537850f79a213934ca8bfd461a4e3409357d7832a468723a03e43c1fad01f5bb26136" },
                { "uz", "298dc6c51baf081e1bfc12aceb17a4f132ebcf8cbaf52bd60fa86eee9f1163074cae20647c35497c5f66338765594dae9e84990338dbf834baa2853c8f2f3b7b" },
                { "vi", "64a83a561fc71eed75137e2619fc5283a10255bd93761e8f3b8fe7c412fb49758d42c640678ce5d8000b8c38666b9bcfec7b2a63034cd4ee0c882dcd21f83e66" },
                { "xh", "3a23fbb3a5a6ca7cac3a1db94519e481b8242f34f704d8ca4a5ad4ec6fce63130192903a566c3ed9ddec6a26e3302147aa262f04d32a81ff8f8cbbf72936ee68" },
                { "zh-CN", "5aed64dcca0f49fb8610108145a632db8bc0f2043deb241e6459fbecbb09f5a242248f2b0308f49679da8383631685e1e403d0233548b578f2954261dc293ff9" },
                { "zh-TW", "363e6341441d86ac7e7ea3a907719ce0df3a906d44fda8991b16da7002b26b9889e8ed0146e2c58c6274ca73bab36dfdd012f1d1660664c68d1bbaca42d86f51" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/92.0b5/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "4c401c7b16a16a3af40eaf23636b4e43a97ac6d08e1785d37d1719b0c8f73fc50a2387a5961dcdd4e68b456cbe8fb86e102e76244ca8cd2489b3179c5b6e4340" },
                { "af", "11f671feb6b67b17280826142c66546bcc968a7fd44a7c43a298e66bd4a259f39e95a631bcfc77e14a7a328032800c73d095a76ccf06bebeee7d446bf1c0a5b0" },
                { "an", "8b8301c51c4cb11b57c6e1f5a52cdad30f1b8c54716e6b175c335235fac183c3e5d455d51e8b5f34c8e93b219bc207dce500c8c35bf9763a0e5374a7d899ca28" },
                { "ar", "61fa4d57278fbc7cd8b7c3d92b6caa501ef027756edc7c7f63e19462d5713a6f45f78353af7720a28ff97052a636aa0c6d8f7a960142a5ddeedc57d7d5fb8e5e" },
                { "ast", "ef4c2f214b031d3cbd1e0903a8700dce7151c79829ab590742a0dcd3ba5a659a5d8b59150d4e6c36f6c0d6c06f0b24032d625d253994694983ec9023452e066d" },
                { "az", "e4aee23f7c0c10fd1e9ffc661ab3160c97e3118f51058ac35d2f7bd14e778947568e152ccdcfceb8b0f85a2eafdc3d38b2500d7a34112ea384d8768bd4a23774" },
                { "be", "8d536063993ce066accd2a6b67b6358bd4a697da60a546bfbece9212c94cb6502d1cb299cd460ee1faac463483cdaa9b27c39cf72a6d61fe3a1677fae27dee75" },
                { "bg", "4b2addbae6c26268120905b8391794ed8fec2cc4e9ef933f6f1f354849f791d43296e202be7bbe8c1c749c0d468bf3d7dec6701d272b6383afc336583871af55" },
                { "bn", "609c72813bed48e8c852e75e0f7342a5cf45920660c4ac421a384401859896cb08ba32340d2699710d52f48330cf20b29ae53811c0ee4bee5ba2c3328ffc68bb" },
                { "br", "5b355b4bc857eff08e24760ae77df4e72e4b50bb03daeec26aa773da4e91f06c172f9fa975fab665fe51b23d4429f2fde7becb64656b5dd72c8f68ebe4c50703" },
                { "bs", "9d72f8d3de6430699cd15a3e2598b0efee8511817fde091e32a240940dfc9af91cbfc70346c4bc13aa367f9ac5254a9168afb950708453aa41d30a9ef0e957fe" },
                { "ca", "c1875f8bf0d67e5281e83d36ac35ef579bbc9dcb4f8d26a8e74e800e12daf714544fa6b486708d9657ff1525a5ffe95aa3bac9733572becb4f243a43f6360ab8" },
                { "cak", "4d4a30e6201d3098af6373af689924e4e45f3a6713f7987246619fb4325bc163c36dbf8f7e752bf565f2c79369762816f35284399919f986cb4b232e7e1eebc7" },
                { "cs", "2550a7b10d11e7ae5019276564ce5b0aa851ac5281f6031d5bcda78b2915f0e535849f828e06422f13807dcfeca7e9f1b8a12a8cc6549b03b2826ac22c4974aa" },
                { "cy", "dec6baf36ef77dd78875cbbe3b8fa5017c0f4f2d9f71246b376122b7eaa162059bc5da24adc800d4eedea507200de507fcd9006ada3c71b660148ce8694ae010" },
                { "da", "b6487fb15bea5c43b530860c622609b28a92f3e8a0a59651e06068d17c9c24ba48c2215b280948cd9b56939de5693ffc6811fd4b6b0bbb6f79bf1597f52e6432" },
                { "de", "637724a598987f5820c149a59b4be1f5dac15afb866c969b3dc031e837353160f288ddcc909951a2683ab9a459d23c635d1f8059da54333eff3743b220595fa1" },
                { "dsb", "8ff76d2520c5907abdd571e9304fe4b1618c04a3743733b3eb8a18cf6b193fe4346d81514a61fa6b8f369cec663673a7042988ae43845cfed23f4920bc9ecdff" },
                { "el", "31b21972c0e16d485d8f15f16aa072ae2e09de4933d0b5f052f8f2845477dbc2d84dabd391a17232b0b2907e2fe55b67a7e3bae88bbf6c1b07d0f8f81c51fc33" },
                { "en-CA", "688cfc034a97ecb08908a994868ea50bf1ae5e1e05120ef4e66b1607120927346467036f29e6450655c1b0ab887b3366363846ed44286369de2db0eabd3b145f" },
                { "en-GB", "82f131f7de5a3f7f9f3a7ede83e359ba82eb7f0e01d69adca5c783b6720301d22eb7ec425f6e5b5aee80bc7179fa04d2432cabf8c988652406dbe106d1217283" },
                { "en-US", "24041c51fe950391f1134b81a9455248b97cf914dbe1d53e4c2d08ac5d09ec66ea34f8a52ed009a7db57ce3171be67fdf222e620261766f72450f6767e3885be" },
                { "eo", "12c5a4f111166ff1541a60d8ed109d1887d9e0d1207c97fee1da0ee2b865ba9d2e45d5729cef7f35b42da204fa63d8ad94b022f7bd7ec406565df35c7bdbe10d" },
                { "es-AR", "a4a1d7706fa51bd3848000991004e866d6f2141f567fc02ef12c7ba9677f9f21d6dcab63f5155e0bb3b341ba6f25e82f6d1f365866b5251d501fbfe1cf02e705" },
                { "es-CL", "8a75dbf2b0a75eec75abf2576257bdcc7cdaae6c9512e5b9f2c64c7108463d85f92ff76ddc3e256f2591083060488990183893fad782ae07beca1ef26a2f5c2c" },
                { "es-ES", "82fd354997cca11869cf73ad239ab611948c29881fdf238999fc9020b6dfa9094f7826522885587ddd00bdbc764f9a5ff8b0d18d87f8f237cdf5b36ce38d780f" },
                { "es-MX", "0f8e78283e18e6c36afec25290923887cbb987a38e9261979da248d65c24e4b668119b2035a6c8ac037996df8ef73d464729ff28cda6f36f373bc0e69077202f" },
                { "et", "e6964b790e7b925bebb756332b9fc5116cacfa504ba822d2007d87aba0318bffebe690e1087d7721c2fc7a265c04980b6e21c65bf0d8b7ba8f45a1cffe4d1929" },
                { "eu", "d8ea66da36b73f4e842a7b7dac64aa538f2b3402eea23b57dc6c8b4f7a3d9a4e41a8116017c4136d050c2bd2ee70f6fdc5122076c8a9d1b512fab6dfffe9bb64" },
                { "fa", "08261f1aa97de007fb5f831de426f5052c0dc0dcfe60c52fb245c7ebf830af4e10cfd0164aee0583572689e6e8f2be5ee5e358cac49c1e472a0c6a0c77ecb6a1" },
                { "ff", "6e58d8cfbc53a6ecabf7740a781554e9f26828a001cf487e22ad22677710b00085c564e972cb207e88d7b383a78d9cd4f8765b8e21444b2fa8f5292889f55304" },
                { "fi", "0dd499e044563890c79fa079b17c5d33ad1355754f2bb868e409c5f03c6c1aaf098b2656e397572f10ff7594fd48e707196446095728a98407957f3e60eb4311" },
                { "fr", "55172dd3aba41bcb821ab60228c62c445bb724519cd2f210aee308310148f1d676b9e425ccba3c6ac554c99d15dd1087fc61efbee15bfab7b4eb033a38dd23d0" },
                { "fy-NL", "c74b7c008362fe75d03e220f063b18b1aed1792055bc345fea1127f79dd62eefb97a57cb06d0a0cb49f9cc49ea353014a1ff763b6585213e64e7e9acc8c2c2cb" },
                { "ga-IE", "a187c68412a69798c720f4a04f475882c47a0f49673a52124778268941bc3e9debd811e7b85a428703784eced7088b7afbf5c65550b11bf5cdcf77f2d1ff2b63" },
                { "gd", "919abdc7a98a9fb2c692fd8f99538b3353dd8b7cbe523e2b02853c69ada864d4455bc50d6ac22d637eda79a77ebf5ba5b59c5323f6db5e4fdc40fd1ffec8efa6" },
                { "gl", "b18857180c15341a3f408eeb308f3f4c9d23fc6d058cf9f69a94c6ceedb9f5acbba2912c73e2bf13d16967b0898fecd4b7b2600768d1c6f379780be16b80eade" },
                { "gn", "f23068a2cef27d936063177c380ed2f52ed96451cf8d34856f3eb2b57afc2fa3b1c97a26049636cdd6bef5be4577a1737cbc9f82c592d7316837b78eab17694f" },
                { "gu-IN", "cf44752b9850e0221a1f9b7eff6b3a6f75417a48fe4fe0aeaa144f3ab4883420dc7894b33f4eeec051916c44531a42654c584cc3f8b7d4ea33e5a2b430550206" },
                { "he", "10aa6e3b0769e0f9f72581941b4e6fc459dcc900e8dcd60d7403ae8e1ec5da227649083c339c8e8976908d13dc62fd5e32372b36b7f2d5d504ad8375f1d9dd27" },
                { "hi-IN", "dd4dbdb2b6fb4a975d7caf75c65072c063fec9935b7546905cca7f54aa0549feb4133c673f135b5498ad0687159991d07c75e17aff62017ced415de1f54c373b" },
                { "hr", "2858ea70bca0a7d15b9c5573c1cf30bdf5b4280250f8a58b8cab62811dd47c8d56a3dc2e8faea4b7f2015de094fb77adc126fc4ead7f26d5247f4d983901e565" },
                { "hsb", "eb85d1ca3acf531e0a62ced544f5018ace7185756862e4cb6d3538149041bf6d86e73f963f648fa2bef425b7b8ef3027d5bcfa051b8e06ddf7bfaafb256bdd2e" },
                { "hu", "5785a26eae14c10118b241972ac5c0956c8d451ec7ac666c9330e027a10c7e6ba01e1c9ea84fe56b1bf41970a4838f2399cb0cf708b89346025d83e66b5cf19b" },
                { "hy-AM", "038cf1be5c6eb4b04c88692bcc6052f80f4388104b6a93164e80185d3aaaaed1da536cfab9e3eced7a795dad9b4783165af385a8114155dad08b02e14abfd6b6" },
                { "ia", "2a573cc3bf46328f4263ebd47a6862062e1dac684a9e0b79ed7e7008e97911f21b663c2c686b6c4eda2f3791f4633270e2d66f8e02a5635daae8b67008e8f9a5" },
                { "id", "99f19e7cfeb4e294bae9f48ac16505e90da3809105ca02a60f59d98e1e565b0bc0fbd0dd36ab151d9fe2c0f0dfbfdc1e033197caa8a40b7c5943b6758b430df1" },
                { "is", "8713d7abac9c7d7d2c6bdadf794ed063edb6b1fe41322ad03df6d377eb16c219931bd6a44616879d8e7b306f100d7f38af5e353ecba1bd48c2e9d806678530b5" },
                { "it", "518c66777719e7c574eb77e03de4071b8d8048d03d4da66757450bfe8b64a65d7e374ace7705be29921dfc845e5e9c0e940cdaff9b955373da93c69392a91ca5" },
                { "ja", "b542e83050233cdef136dcf3589f9722b55c8227caf3ef1e12dee203ace3ccd938d4566508b15f3c8d1179fc4108220a82bc9d4519397f7ee720301a1eff4624" },
                { "ka", "98da0939b78facabddd0b62efdae59a658a77c03e4a495b007eab1b5c939831a016ea2866f6100645853e2a52283b13b30a392b188dff7844ac3ec8f03852992" },
                { "kab", "09c116ff7d7c26417c1ee53af8e1f4d40ab90407500065ac802e83cf5810ece7d74b729a17f4faec646ad313166d6e65906b25e700e1f5c28c883683a750ac32" },
                { "kk", "19dd32622f177377082c6fb584890c0f11b65565edce1ad58686ec5c9529dc80d829a3f803591ecb1557593db85be4563e461c1302d51bb6843659ca64b1dab0" },
                { "km", "c03215c847861c5d7fb1b72a0d09cb7908273ecd4b6fafcd468cfde4b064e078f9a636bb510fa48c0af4961043779d7632da20106e394e520fbce6c0c5088e97" },
                { "kn", "168cf48c027161442d2fe38b0fc1092900024deacaab8c0c2e80bb236d689bc015157bafe9a4f18ecbae97f4a80fd067dc30852675c1b63a5687d6f6459a93f1" },
                { "ko", "eb17ba80156f4131966fc157f7072b6e2a12607cd19fcedfddc8be77c75d42d9f1fbcdff78e8e0ce621068d364ef9ece493cb23221254a09e388c1d2166fdcc1" },
                { "lij", "3ac736f46de87df02b291ef497209dec17bedb0edbcc213078ad9ae6ebf2d8380fcb8dde0bb924074e8ce9b0456c99949cbffd9928df209a799bc545ec3d710b" },
                { "lt", "95416e4fef4b6644102fce50839569161f152e264758bd4f72f5642b45cae47640394ba73a9bd83da29cf5dfd9b6bc14fae15d7b2f046c384d5fac30f6bb5744" },
                { "lv", "720112848c95eee9a9c9f6b108a82f473ae2b0bd15d9f68c93e171626adec00605ec6955fb9ad9027c7d4dc01c25d4a5164be262f5139ab53d1690229f7008c6" },
                { "mk", "180f9017fea075e46909f35e417a76d52645e010a19c3d2a0a621496358ef4e962c351ec49663113e26deffa8b9542fedda97731d36f243136b261993e08a8fb" },
                { "mr", "47facdadef4f4c4cce7af9a088c4b5de4ae0cdebc40f1d2da5c965a4459f301ed323abbd296864e2f9fee22c0ec7a3ad3a85bb655c13f8f4c2ee6f0dabeb8df2" },
                { "ms", "e4323c3bb72a96d6389acc5efc257c7129a302a8e592a3a5051a3a6ffdee3a2e4639139f01a14f9ed893a5490fd5146151ed84020dd03c91b9b041e3269d2c45" },
                { "my", "aab1a69cbe0d7a6225425e62136f736d94fe9611a260769a4031592473cb409a1f3070cf271c27622835216dccd2a09aeaec2b87639ad7242f820316ca42eb9d" },
                { "nb-NO", "558aa15b870b8de67a9885141774da691d4720b67e6b9fe1be96666ad896aad4f018e4ee4f51a4a35adf95222a5df03ba118e45769d1fa65fb21b22a2daddd00" },
                { "ne-NP", "20da58f65c3518537c212dc4bf4b00dbeb91c641d53a5693a7914c0881689732792cf6dca4da90e1183747a6780a4186dc1f1756e64ae8f0f8432583305945e6" },
                { "nl", "6b19fe63cb85ad7ac8b0d9d21e7898a7d121a34fd04a9ca69a0fdc1ac6ac91c4dea466c4fed0affe51bd4ecd8c792876192c3a071aaad6b1d35f045948077c19" },
                { "nn-NO", "ed874aa2196627fee26b3f3c6a7aeaedf1439550c09c06b5270930b6a544fb4c0a1169e9d959042264515021340f61ed88fb9b17740bf4ce1c566675a2469553" },
                { "oc", "319a3f52d0b703b1ded97165cc7ded01b889a0c038bd4c33c7bb3fa797ebad43ac6f8cfa18e1d8b56dd194c41e23b0c48061b95ccdd55ca10c4527fb3d289356" },
                { "pa-IN", "f734d287496c1618b140f57c335e608450de1991fccc5e9aec8353938679ace42fdd54d4d242af88fb8d7f530b81b581ffb6da2bb2e48185b8608ffebbf3515f" },
                { "pl", "3842d3283868fd3301d9430d5a97b69fcba65868f44a08deedbba4ba1757bc0e08f8fc087671c3bf0f1c7562f2ed9741c6b058ae306ea93b489d438f88f4a3dc" },
                { "pt-BR", "4226d33ca9f89ba1bac72409d9bfbafa709ad5a8885af2446496dbcce9e70cfdadc3bc1cb8dbae5a0136b647d881ed950feb72907445aeb03b44c16238dc2c24" },
                { "pt-PT", "6554877a12eefe1f134017f06604fe8c2b4c56847bb05d78909264e9111266107f413401e4ade0b2b6cb9994b9779ac2a1e555466a9a5d9ef02aa222414d359d" },
                { "rm", "2533fc89a6084bf1336feb71351c9ac455806210d3ccc0fabab2aea0e5dad645f2c3685d8d4ae192fe368cc143718bcea2f664fceb48058386cd81889eb08131" },
                { "ro", "a074d59a71f64fd651054af98a78cfb90c9574691c53998c5cc6fa8bbea83548ffceb27ec49e163385ba9bc196f3c2eee7705b0a51166ff030f2e4c67d834864" },
                { "ru", "86a3fbd831af8d4b6725806933a15da9112b4c4d5346d6816d077ca5d60a71aabf4f5461386fff513ac67a9f3a8cc6b38e4055b4eb0cc677290ca30a947453bf" },
                { "sco", "912ec477f37e380a94cadd1e6d2ff5f3ead74511f8d95893005438d8263268b3611860f8787a66dc7b1312ce0cf199a5207d3f241d60838541f86777a78a9181" },
                { "si", "dd0086f1e01e0912cf7946558e7e48913f5dfef2e9ac67bda30b0565ec303e5f30b6ecb39d4f753d2fab714a4d62f11d7f24c603309b83eff0dfe2cb649195b2" },
                { "sk", "f34a9cc9c2a4a6597cb39a4c1a212f7b439007de5a66393d4bda5461e74bbbabf51534a14b10546b415d85c96cfdf293f37c7a085fb5e92f0dda70971357a60f" },
                { "sl", "d7a013986bf0d974e00aea870ec4b3ab95bada1edf640721d5f2c992dd7dbce77546121cedcaa725a73ed4224e0469fdd2de349a8987107e2178609d27dd42d9" },
                { "son", "2e7d8f4f631cd7ff610c213e3ad13296ea50969d94863c9e66fb5f72c1417ae3812509524241644065e1f8fe9a18515e33611490e0dd5a9f106c048108b0c01e" },
                { "sq", "b8d8cb69e4f0b266242eee14c988a24b887f65663a7f473c00c121007834ce815967e7b3b09ce0a3f1ff47b37cd4a20dc397f83b477b0f1c622e1970ef500382" },
                { "sr", "76bb545de409a399c1169566efcc37446381e13705b47101edc54be788bdbd632e87f1a4677726e24d282b9d189139c300df5174e00bde8f837effeedc734e36" },
                { "sv-SE", "4b69fff1807c4ea568223ff8f803e586540f31a55084a818e4cabff2a1494f1ccb3b5004ed2589b9e9668bd1cb3b07ef2dda15c167357efac52685dc5a536865" },
                { "szl", "f12aa63bb0d90eff0fa3fc65f354873d0953cf1b632bc97ba2f534c24cb3b13e471f1788b7b6ba115b9d1f84e0ed446058e5e6f972ee57771293b5f2dc590b28" },
                { "ta", "a22f585c7be233c32dd12b4639de9f7350fb695b11033c960ffa15ae2f27ce38890d90a902a21567999d8113f2e1633c06e308858bb302570394deb9e6bda0c9" },
                { "te", "3303ae672d8589fd7c2a1d797e9770de08b6d9ae4c6e0e8a6f3763c26a178e42e04a351f2a1d508f5b6ced2d4bc05788a94eebd8b73466f803d7aa00dfe8c2df" },
                { "th", "da7d203012f2ac929f8e8681fed248bce86d2bcb6e552c43d771c7c152c06cd96b4ada73424f40d6826b4f2c667670787e2e04af80186f14b775d1af6e503b2e" },
                { "tl", "0ea5eefb7cd4d92726e37f0ccdbdee440df9913d9d396ef3b3086effbd839fd7cecaf6292871d28a43720b73248b96152542e4761e194ddc53dde3770a3b5a7b" },
                { "tr", "708ce4226a6fe3bcb9ce6db0972e65b0c0101c9a6be5bd2020caa7a653c2e8c508b4ff3b99dfe3682c86b45ee1aa59af34ab1059672944d45f904f90a83c8f2d" },
                { "trs", "0b0eb32d5aa680c12b1e1c5dbd2660ddb2db673cff0b7fb1a193dd438d23c8a799cd3b102d19dff4ffe788ce18e8bb6e028d260d5400ecb04728110f936edd8a" },
                { "uk", "fb9c1638d522f881f157c6f6e80df09cf8b2112f3a62f6ed9c108e0260d758dfbd6513577f600586eabf4991e600d014ca9c7d0d9da7e9c29cf31e9b803a011e" },
                { "ur", "16cd7d55b635711e4d5db3f92854dd3f4c2c1e658882ba01597eddb5bcc4c3981622579242ab7999c19e8787502308acc51a9edd2436e0f0bd9b973051eb8c7f" },
                { "uz", "ed2ca2dabb46828b68151b58b81edff57809723e6210821e20d4a556507d773d4ebfb9337430e12af7b32e3f5742650a240bb3e67834074d7bf95c4d8c17dcfb" },
                { "vi", "8860fe6e26d19a756e370369f2c0fec256dddc50f33f41f319af94fe35d3a181c87c842c0737c509c58bbf6a9e16a0f7d1f484d08f024f38766c4f2e3921308c" },
                { "xh", "fabc9dd16bdec64b1f51ed0417dfec0764f553b14ebbd95fcd28c5f948a5095c16aa7b4bff7839431563f7904dbd0db3c8be0f25a0911b40fa755316ed9abd63" },
                { "zh-CN", "f06b1a38f69e8019088c1a0e2c381f344b9f4b34684e58010fb7e9911768d49728bda1eb0ffc3b747fa3ee634ada5b58d72c604cbce4e9c1999199330983e0b0" },
                { "zh-TW", "a236ef72e309905f9609e91cf51fc4634668946d87130a49b281f7765b3bf3b2243c81f9b4f92e01f6c6b597d235416e5f098f5277f1456f1d8bd4c1fda58608" }
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
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64 bit
                    Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value.Substring(0, 128));
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
