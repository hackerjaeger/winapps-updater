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
        private const string currentVersion = "117.0b2";

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
            // https://ftp.mozilla.org/pub/devedition/releases/117.0b2/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "b70bf3dc917c1108b7a9148a838f58165fe697c7fe33528de4e765792b773ebacfe86dec2adaf4ca0476f00770cbe45c248d2107bc1e7882be24498e47d0fa74" },
                { "af", "39186f693ee646827b5384adf79ace6bb0df27acb54fb5c774a38c62e2f6675cc11d70063380fc05982cdde8953465ecbccf9a976ef5f856667db61778232323" },
                { "an", "34872582a06b281cea4880c50ef3e794019de7e23a2ac84fb3cea4df0e0f778a2d2fe94a2b1da773702540a7189db00d55e1b61ff1ba07fcc14eb7a3b65048a2" },
                { "ar", "c78a0e01aa60fc9ddd796b001bfac97b769a7393e329f6790c475349f683b9b3cd51ecabfda9e49d1ce5f3bed23b121b5f6070b4152267b445c4d4794d84db0f" },
                { "ast", "0d4359b9442b44d7e8b103f26231255a3db9df8ef81781ac7b9bfebe9d309e6bf973a61b6fa91eaac34a03038869c82ce2e80ad038847060ae96398b664b345d" },
                { "az", "676d19ae1a7b3da6155efb76164f4424585bfe4629c9bdc36d5051105ebf97ce83f600f115d9809cf37f1ea9b62f111d32669f607919ee962e73c247df97b17f" },
                { "be", "f7fd780f7f5c49df636686ce616fdd7d8727a9688fb4371e9ac785280fadc8e58437f1d3d9df7c5e2bbb0893d349f39a4fbddf729c231bdb3ad670d0f3398504" },
                { "bg", "971fadb1a63b8e44b7961febf640e1f414342d8c9e8d8aea57e4482f52d00848232110fb24a68dc5a56702a4e8b80018482ed87243e25552b89b8492d5e37dd6" },
                { "bn", "53075c97018e37a3b7be635ea4515e58abcb463d1881beaafe35ea4a10728fbc4454a36423cec068f25a28366e59ab7fed04a33dfc4193de62e2fbe511824fe2" },
                { "br", "756c14fef4ca928a7eacea5fe1141d3c35b662dd44bfac0266e181e8f2f75b4e2e07b16e01b752ca0442862d19c1930928733c47eb0a1021cb33687747101cec" },
                { "bs", "a2584f79b6bc1bb79b484e5949b34a06405ff3bac0da8ffe591708e3a056f903c0d37854944f248d4224d241fb9438dbed6cc2e5c96b4c5417ad79fdb7596ddd" },
                { "ca", "7e1a9bf24e84978b4e96be055ec9d219d97bc01ed4551330a154659ffd2ab8af13797d9637a992a434e054dd1443c972adf20d937d515fabc0ba0eae480c86eb" },
                { "cak", "e90eeeec0b5c8f0c851785e8a1fb6885dab36861d0e5684c830498eede35df91e078d6dd45b806890f7b273a71432e58e0987a679fbdef1c65baead15190134e" },
                { "cs", "9d868e078c24992e525b72290bb6646ad06fde69d209daea95cf7994be95dc40bf5bc97481871453da1dd09bbfbba30913440d12ca1faa6f2b4d28b9b85f7cd3" },
                { "cy", "7194b045982753d18061f4433c3e2c0284679b3cbb12637629f0b22ac4194dcce16707fce3111ac10148f7e8fe987031b04c6747b9f5a12cc3ca55add9a4ae35" },
                { "da", "9d5b1118b6f1ec551d9735e58a6b579139992ec1361da2b940dab73a18863a96ac8587cf91675c3c89c5c132ec85750f31630a3bfe721452b6128ecd2a8eb152" },
                { "de", "0f741c3dd097bf2e946a77d670702b2985098d75b4442570c122c902898e34ff0509128688821db0c2ce7139c9a2d094d6d9d3de896b08c1854b6d8e0fcc3403" },
                { "dsb", "a480544f5b0d64867602221258f2f78a8439680c38b003af7e1b998dd8d3209db1d08a6db9e8a95cac845df4514ee9ee5df21c4dcc9a4bb3e0a13ee7be455f72" },
                { "el", "e6949ce1091b97bbca0e3c496c387d592c3d0d8dd1c5bf5701a35eda5cb20f2859073e9d75d47e675fdaabc82cc4b868617e987c73310be34a78081c00f8bf72" },
                { "en-CA", "a16e1f64fd3dca58fcb830b67725c34229b32f56f7d2dd0c11de3160e516aa5de8c330ebeb16083592bdbd910dcadd32369c4e31101e9d03188b807324c1f7f0" },
                { "en-GB", "2913a64468b6ea69b9b5092f04b9bd910bee0ee01ce0fded45129ea0beaefb22fc04851a454554ecbb9b058566e234bc7629a6e5cbcf6a70fcab32e9551b9e53" },
                { "en-US", "93393505a8c254e8287dd9e6f6eb171284e1eb7f8dff6b88fdf51002523986b8502443b41b03e5f341ecc3920257d240dc86d399e4c2cf1d87eb8e7262644e9c" },
                { "eo", "d0d452fc7ff61effd368f05ba75239c6c59f2a6027495a9152bf0da663306c38faaca368a0e5ed4fc0779fb8c0c48d02e77644e0561328aa25e81d3d71f5bd3b" },
                { "es-AR", "20c575bd81894407d5fd30d7e1385587c85671312ce2d1b8a37de9c8dcccfb8fdd3f9e8d7efba08c4a677d5b36165554a1c980487bb1cb7fd277004b391079f1" },
                { "es-CL", "b5ee22824981916efe43353b29457c01d159dd1e265b3fc6b1c3954d16fc7a23eb2a9b59701f3feb3e84223fa1ed8945885d7acdf0ee1728722ec80224db7dca" },
                { "es-ES", "5bc4011e69625135fb2498c80d35ec25f342c6d42dd6365a42240b9375cba653f0533ab893893661af29c3067994f3fcc84a06a4152e82f1720d6f4e7a5b5b6b" },
                { "es-MX", "bab18c6f8d9f7531dd83f0a92d2959c3b5eccf722127d83d80befeb99df5455888860de48fe8933f94664345d974c2e0c7cdd2ed54d074d6fc8ced55fdb08252" },
                { "et", "9ef6cad237b994d2ace7691b8d030c8931b7393fcdd04def73956028f7ae2f7bebdc0fcc92f8796d0ba974c0d83691db08d32ab204c7ac2aeb147cd9c20b52f7" },
                { "eu", "791d6466a4f59202593eada641284cbe0d54e679928465b974eb301ee874f139a8c670800b805f86a7b238c17cf8b3db6ec9cccdfdd2c4217fedb0a26d1297c1" },
                { "fa", "a60eb9f2a2fa79de64d2dfe791290ad2a0a70a32fb625ef87e6d566fe34796ba0791b12c17d3c9373546330fa43b01407570823db7a1494f7509c5474cde471e" },
                { "ff", "2a1bee6a5cdfe208d2528df51fccf43c923b8d0ac164e9feb8688d15796996ee27fbf460fc3ecdb222270ad4b620ea3e94764d1a38ff5273268561b0333740fe" },
                { "fi", "84b62e9eabc4957fe86460c89872de8d56deb435f1ba18441685e442e3f670ad19d51b3b474bb49721234d35be4a78a96ab72dd24603d824b0bb0a7448d0fc61" },
                { "fr", "5891484acc5798e986010ecd449406f5fd319c80b9b494438259c54cb0ce359e3bcdf73a4fb78d26e83c809cd650d7e32ffd4c8bcb9223778ffd7e9101146a4f" },
                { "fur", "15345fd874e20b054f77e0e9fd988990fc22ee0ac4a5b87838f280acf7d6331585be70419def9d2cd90d7336979d11b72ae16eec357bbff258ffc25aa88feaea" },
                { "fy-NL", "2d43ee03d65576de4a125577dadb19ef28a1bcf6ecd90a83f480890442fe11911130bea4ce9e279e8c1745d117ab2bca7a3a94814d15386df3cb32c74c3609bd" },
                { "ga-IE", "f9f99eeb04aaf8e8b8a785790ff76ad8086bcf479bbdbbf3d43233fee29e2350c3acabf41c7f0cdef2eec42db35a759ac0f0557f48cb2bfaa555740f39be131d" },
                { "gd", "0ced54589ff0a75c093e600b12e4be94f39db5d219f48e67fdc28743575923fa836dc1c9680a97083ee85e124aab41ba99db8cb9e91fd1c42726d2d5433dddb2" },
                { "gl", "97d49cc02385471f0728cf38e94552707615503a434666dee6769b210204a8c1c7af68344aacc10c0706616b5f238e9411cd7984700053863eec70bb38ab69fb" },
                { "gn", "311a37e7cfa008968aff4a8e626d4dc6577afc9ee7572f04749639a338d93554ba6596f00119a2e79f91284d3566a90c504ddd82fed9b5bb55ba38cfa6494e61" },
                { "gu-IN", "f31946763fa7602cbaff4f30e817a874b7320d715a3ac97c92537872714a544c345bceaf994fbc347159a4a6c42c13741de246c455e5f077f2eb8ba57a79a9e6" },
                { "he", "cc63254269379d7049ff0b169ceb21687ca33779ad7183315518d46bdf8aa73c94754639a9dc05adf346cb637cdf78331724b00a842557ffe71667b905ab31b3" },
                { "hi-IN", "35813e6dc04b9351cdba68434264e071b9234a50cd7fcad2d819e82e216dc2405d24a20635a3afb83f0015a4561916306da7fe09dfd07df72d1e08956954856c" },
                { "hr", "4f304c5eab31e72af27229b8256042efc872e69a922ef47dee782159ba5a40c0b387c9aab0c3fa6717f43cd70c83f2d9f6f1f7dfb3586f0bf37dee252e47ed65" },
                { "hsb", "0e5e5bce44e958fb1391bfd8d4d3b62d926bad58118bd1bcf18794e856fe58673e16f77cfa2d574200ccffda9380427d29f39c32256945803bc1532b715fced1" },
                { "hu", "439dd289d773f62d60108b3c697c8fb880fe19209f0bce073748aca6f25d024408367b35972026e2b6adcd7c69a8b6b1f2abb0b962ed17af242e62b6ec3aa49f" },
                { "hy-AM", "62f2f70ee0a6c968b9fd1ee7183de0961e2b358bf74027287f2429f1473af09631aa43c58a0dd11689b26c47b0c7ff5a024af8f006561e8e8ae1f672f52af84d" },
                { "ia", "64930f00b59b2d2ade594896ada3441973012ced1413fa2f8f331375b94440808fc739e5a8331d64e3e6d1cf4ccebd377656067c05b4f862cfde63746c782c0b" },
                { "id", "1ad2364422af336da7fb75cc13a8507165a6ba2746b25179847302487a902706fbb8ffffa22abc92026c6de237302654da396f2fd5bbea85704ef30a829196ad" },
                { "is", "d69356bd316a6de47ab7fceb4d37513c82528dc7151d82702bc430124fa8ef5fb686a555be1e85d7ed88f0c3710b5ef288b31694011323eb16d9bf85db5e47fb" },
                { "it", "4054090879e02dc81510091e00e2c903a61d451f084913b2ba9545856ca67a9772302d55f2430c53b1e6e960556d4c82ed0b912d90ba143a05cfa42538016877" },
                { "ja", "e9c219187d46a6691d48b2042311e439022a57227f4d36a1171177fdaca3eb56cc94bb22be4ff2181b0e53a0229b3387c1bbbf931b96faa07866e21780a47e17" },
                { "ka", "f2873da690d13f8815b4e2e4ccaf4d463addc1a0b800b4b5d53c98defe2442b1aa4f25a9ee21f9c99a21f7d2b8d6a724ef13f6dae6ad356d7bdcfb4d5a9d257b" },
                { "kab", "503e30e137d1367cb2c8b724e137175b8047ce690034e4221d84afe647f7cad645d8eb4ee78c224fb25311fd3fa93230340ac4b0ef6121a906745ab4ea22d9f3" },
                { "kk", "ee96c84ac52acc3345e15ac02a78c6d955be2940c3e9c9e42cf876022fb3624122e5bad4fabb10e1f3dc736fae29f9c01260c72cebd045e779411a8e089fa267" },
                { "km", "0196ccaa84e384a8f8978c5396fb6ed138e770c93d972192f85be5cf4b117b36bfd8d100c3bb45428f094edbdb973d1b8177c2285a777fe2716fe8f44644d4ba" },
                { "kn", "9c548431d8b1123fd4fa0ae2f01bd672e5351d35ebddde9814274fb0202785e1da8de47222a220804fae6ccf28e5e0971394d208ecee4f8cae61c882b416d7aa" },
                { "ko", "133010edadb9a045ee8b09c620029a0e2726a589fec87ed8fc69a99e7a595980b39dc8a99250e0f3228d1731acfd2b98c38ec6870241f53541edf017b2209aaa" },
                { "lij", "93134ee75404df53c3d9aa6721b113878f88ce0b01d6022cdfb5c0543d35069694a1e2ed99e3ece01ca46a392a5d5393759c4c1014229ac1f8cc130821db654a" },
                { "lt", "412e243e12829d3ab5e13a7ac5242238c5e2e1b457d1e9f897a414aa82f3bf79711de682295cb61da2339c674329f4b2e15d521c81e674b1072b405b54dcab72" },
                { "lv", "55d2c240134350375463b9a7a458f96dc0db352f3c1b69386d87b115179f62ef54f354fb1726ebe6292f2c1f977f6673372aea39163343b8efe16aca582f762a" },
                { "mk", "f6c436f82cff49b222aa30f9282a46c6e0bcd9c999b6b4c5413bd1601db202f9a0b34ec019720b866d4342e3d965752c9ff709adfaf571ffe0af7451599f290b" },
                { "mr", "0dae1353da584566daad299411bcfb47a91e50d7fa9659add5763ffb468e68932d1ab3b10a274a7f66514f1851973ea5497f49b15531ec622f7a5aa8f3edafcf" },
                { "ms", "243fabd45f32df404197336c0c229d2fb2f802064955743f5abc413f8eff2d6d315d7f9b7345833ce4a6d485a7bfd2a496a54ba965d0c40e5499ab9c66e393e4" },
                { "my", "e75cf83643ad98d87d66416d0d6fdfdf5b7dd967239c3245ad709e8cf1e9bb34eb4faa6c6d8b87fe4340222ac6384e58046676ba027ae8b779dea1921d306d87" },
                { "nb-NO", "4f78c587666e9f4bd325a883a9332239b1bfe6dfde011f3ac8614662b1e2c12c680ca2d4011f65e2dac4b1150197a7235a0f5b1ccf65683c60615c212f640d09" },
                { "ne-NP", "16a005edf1ba421791adf3438bc0ab83c8c936d89c56ee6bdf518b8ab152dad65c2f56bed192458c17215cc68df827bdc3f4dc5448320a4d243cf4adc3aad220" },
                { "nl", "cf3c26104377677083c3580a3791f314ac33f3436dd6b4c93d5c48094ca3188b42d94248687e18be85c476a35c91bb9924af35b8d130a19f596dbae1cdded192" },
                { "nn-NO", "58938c38ae44a01369804250fa3de2237c00d0313ccbb54de4cdefa3707254995d1aefcc6a2a460d0e5cf9773ae04d0035126201ac34aab7dbbd2aead7e6f517" },
                { "oc", "b6c414bba1b78877efc685932a40aa269acf39f872138a8ddd703d2f3ceb2cda81996da72bfa9c385120fb9a1942318c63afdcf35abef05a269cadcda5522236" },
                { "pa-IN", "ff7db10c2ce4756ddbebb572753e3c13d86efc7d63f76c859a6ee9e93c61d7703bd4c2690fb1a57340dcd3764cc3b15f28a0e841cccba1105b97ef0d857bc7b1" },
                { "pl", "9181431f299c081ab19c56f4b4155a57a4d5b3b82331c3313cd505157d3ec18e7827294b540d474aafa3bcfb743688fd74aa6db2becfaa03328f0304cab19b92" },
                { "pt-BR", "2ff7235350fdf3e63dd24c0460f3b182d0d8244e9792f33a9494bd87d1992cd3d50e7eca551c41995bfd1b1eab15a2d0238b9a64ec55fd1aef56367b47206563" },
                { "pt-PT", "47b72e1a4032029bd0e71f553bee2fcdd6d4d6ee57d5bf26ed6aed03c090bf251b168c521f8c33a07352a86423b70fd274e0ac95f5328662c8f9db20e56223d7" },
                { "rm", "0c607482a881a00c3dcb6e3a131c7ccd730cb204bebe59bfdb39c535ea851dfcb56f1d7dace3a557d142e92d0577e6810dcd8bb5892fbfa21d1f18ea08adeda0" },
                { "ro", "b03846817842aedd5ed499da2b9248e2d1dc4e8eebfc79bbac094afa33baf524df418297d801dfd1cce4a2b4bdf55e5d1a71ed3af8a8ac6da5f9960ad931931c" },
                { "ru", "469c1fa191b4ae30eb4b1831d0b6db053ac167996ea64e68704b57d28d2d1e1c716e9f377947c6ba9dc1c7fd40c1e3323a965c82ec532a07dd7dee36649daa55" },
                { "sc", "d16373b3ac6ec70ede00601cda8e43011c6fe56374fb458c65dea3d3a4233620203813eb2cc2bd2d612130e5f1d15d7127a92fe6e209cf8dc219ff5ce7d64c91" },
                { "sco", "11ce64808213c6c4d53d832b6b0400b6dfce65a693b6202cc214618d3d2517ec1dfea27f553641e84f7279dcbe2c038d4f4496bf4991b70ae19c037611521d57" },
                { "si", "180f11fa9c2512575e75a4d2ae9e85f9dec56fd63be84fb7263e6a6a1fb21bdd8da5142b222016dd31193b4755b02b8fa73ac191f5a8458b66e8b5bda8809d2f" },
                { "sk", "c663b5aa8bb448c9524e72605fcbf91bea5ed834e898cbba659400af90c6def12c8c3078ebaf5470e1b3e43a18bb11041297a80316e0639c7c237c141c192be3" },
                { "sl", "5d6c5e90c86f7a9ce61b53b4b9e74df90b28750d53e5381e57633f2ab12b1a75a8e44d2148debaf0a58d62c277fce7884ea8e71016c7764b29d6a739d415e494" },
                { "son", "fa5d55f2d2a08f40fe7ecd7816e07e74dc8d2798d157d793692f199ae5333a4d709486398d66e1ea1652a0913298a2426e6cf925637c7cbab6bc77d305411ced" },
                { "sq", "28c373da61953802e7a1c78faadf6556191380e9460c30d7fff30df7af2f272c7607794b1fa570a7ec9a3f1f1055010f9952477306d50282377789a280bbdb69" },
                { "sr", "d69069653e4b9f66856c6f7421dc8e7a915d8a1b98a6d01a972ec16591163a2bb87ffc875208c4ed828250ab7c7adc18404e957f5bd14a259f3dd7713837e59a" },
                { "sv-SE", "496302adf82ee6a9b20b2f7192a6078c792cb337c9b207379413d83a5b9977c34b36bf05fcbd794b93f825a1d3c3909d28222caae1fc844b38b283ea9046ddfd" },
                { "szl", "da719374f64d362158bbe4aee0d81aff99f175edb4d64495bf7bc7a9ee94109978da28fed42e0e2561e24e0c9dc17555a5106b2278cf36272fd2148a9752f621" },
                { "ta", "19bc87bc7ad8432e870e77b0473c6e14524feff4cfd3cb4e0abf9b34d7b668ae5f1bf4c70147bd4abe8fe33f325daf554df154dda1538c860e15a7d75793cdef" },
                { "te", "f4b44ce83a2e81f7b796e91c12706b1f89f0e3960e94ff8a2014963f8ac8e22afc73706f21402b992f709f34c8990c9811d916fa59cf59bd39b0946c42cf24da" },
                { "tg", "096c1c567464f3af3beb40a374d49a1ff1a54ad585640445da0d22aeb637a0ff7abc10d4d8f2ba5960f9984b1313f23508873584c00e9774299ac2e00880ebe6" },
                { "th", "c5507bee068739316223e91a86059b62a3673b964c425b87b1007c84754712746d22ff308b1ce665c5c798174b736e0ee264580bce02e6b260743e8f1e71f2bb" },
                { "tl", "7135bb98cdbafc71953aae4513ef02788acffa82e2c38cbe15e0e019db89935ae45aa187cc4e34a180e377de0d5b0121961a92448d819b50a678aa18f49c0e06" },
                { "tr", "70d82b6b66df9b9877d4cb84974de3b3de881e8c9f0a261d72b175198917a93e59af4ebf9916adfd501bc12e8a5869295100c347f39e8d371caf27e7565467a2" },
                { "trs", "fdee2631578719b3643cb28c2e05e485efff0e963c7c9d8635e2cd7c2e035489354313e1a74a1207a6876f856a6606f291aec6aecf28c21ba4e87a9c20274626" },
                { "uk", "ba4a4b7296d98e8a423d5bee8cb69b39fb4f368577d93dccda157892bb3c30f1274442988d33e3e0778363453ded46b37e7190ba0aef8fc114f4e14471b3e440" },
                { "ur", "3b7e7c7a230acc682665a170c25df1124783066c17667a91dc3e976af7218d4bf18c7eb37c9e476684ea58bf527451b0028bba478f717354b3c80f6709ac5c0f" },
                { "uz", "fb40297bdca39ac2e1d16e5c95a5aa171a90dcab4c1248b9d7e1b0eff621d05618d027d2c31b6909796b21b60105aceeb07f486ed48bb24c49017732b05654a2" },
                { "vi", "49170b29fb5a4904d6dc347b119ddc190aace85d54bc075ca8083a81f3fcc70c7b9f6ff7156b51f7166fc6855d13e8e65183cf9370b8e0c6399e1032cc1f157a" },
                { "xh", "39f4b7e0cebda80d6d60ddb096a21a91097fa6106f6a40cf83956535f64a09c69f11a2af82ddbbc35d414df03d6a256b648f684cc97df47c896cad3eb6ed11a8" },
                { "zh-CN", "e7159fe224cab715117c3a6a9398839fd304da173fb6ced29a5193ac00570f678defbc91b54badeae16c212c87e05612537c1a1ad8ac1e116ca83e347319490f" },
                { "zh-TW", "b17155dc806f79d1c5a6167ac2e638efaa363f1335a5502463f4920ad005c665b0d6c80488e4506ed627f8a32e4d628119cc37fdfbd59de9a9dac0fadb4a1f8e" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/117.0b2/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "f4ae549be68b71c1b175340257f5d2ea0dc59e4cc953db9f7c9a886c92ec14b11b4baecd5eba95d87dd0babde7c1ad98074182b0170bfd3a44291d80b0e4262c" },
                { "af", "fe75f75dfc807fc911092fcc898361baa049ff63e2b5687ee60c7a46da94b8ddb7056306953dd60357b0ed92cd7bacba06c0efd3001907e0f6d7bf4c9b137e46" },
                { "an", "70c915579cb9f87362c9bfe9b53cbdb60ef22da34d29ed5c652b77edb717c1ad8081f5221723890b3cfbdd3a76911fc60d51460e29fd57a64f5217aa4ceaa478" },
                { "ar", "4f76888336e0b1c4517533b65d99c60bcd448faf27534896b02aee15b685b941f063827e12f0c2949c8707c98e0ac17041deb72a5c443aafdb54508ec79455f2" },
                { "ast", "7a8e4339308915e0073ae4c148bbba4eaf4126c8d3ff8244e5980cdd1317af2f4dc203691443a6f3eebbf845460c3a648e4ffc3ced9b369c0b456b1deb32652e" },
                { "az", "cebffc235c4433b4830caa251c9c1a82af96fa79739325e221dd0daedce9b0ee7ed766bee71a2270decdc7d720b6cd3e83a59b6865f81e70d9b01263d25d8ee7" },
                { "be", "c290c16769de7bb3da65c18656cebd0bc89211164c4af52bc3147158d76cab1777ac9c9ff4cfb3feabee0f6a9eabdf4c82cf19bb96649f63df3662bb113e9e0c" },
                { "bg", "f18d4cab6dc476349549e01b1cf357ced01e7f4045dd5aecbe530df74ab9735b9fe4e2193abe42539226b81d7727d29817a6c7f19ac862fe4a80aa7ea9b5e996" },
                { "bn", "da9bdac09df96e2dcbc5243c81a47425ffb424c9486de82b355667632eebad9eda48ee144b5e69c16509e6880ad205020dacb563218ab29fab1c5e68aa89c0bf" },
                { "br", "882f623a18382ac9858936ebe13f2d909ae12dc416e3f5f160bdc0ec19cc996da8a6c3375bb7a55ab21e16eceb0be9e8ddded16e997b737f77f3f2c1529f86a3" },
                { "bs", "cc9da23805ef852fc675d7e91cee68fb28543bd2d3c0d4a9c95eecbbf28b971d0d75a7ef93efcfb11d85bfa675cb00841ab9f691ea216e679ad86631761853b6" },
                { "ca", "d9bfa0c21254561c3c732583ab54e74d7923d198919cfc741059a0a27f72ab1292bb2845a0e2332970adedff7e8df09830cdae411b291e98cc9d29ca53f7c03e" },
                { "cak", "fd0d1f82765df19b02c73b7ee343d5cc157a8a06ef676dfcd97d22852fceef450eb532eb8ebb736e0aa4ba879a497941bca8b3948313c17876f3049724cded69" },
                { "cs", "6bd547f6bd56bdff1268655e12c333af1ae6d9d34a368da3b4e37c3cb8334905f1a315d0a0d54c057534c13c038b469de96157cc811a94043b38fb2dc9d23c6f" },
                { "cy", "2707a7f7b7781dbcd9c50e90436bb4a9520767bcffd0f9d4823291e6b18cb3b9aa35d0fb3c614d529a4f5b5af54c8ad29e9ac663938fabbff061e03b26145eb6" },
                { "da", "1cc8e9a6e79ec43283f53812abe7def741836a062e5547a75cd4d72c71b968dbd04d066a14a97bf999a2b587b2c626ac767d6d327840f7f2af3d7c4fcc5c491e" },
                { "de", "6b65b1c7b8cd60100baa08995f370f70949553742514c66ea21c66ad475074db7fcad6a1990430189fb7653e542196b8b5348257707c49b97de4ede5f70fe78b" },
                { "dsb", "1efa401a9d32dfe365ce8dd6ee6e0a39dc200d41b79198491ac417e9ecfbe9a1312d94e9864d95e6e9d981fe10da15cdbb8b7151930593155ad8a130d9755e14" },
                { "el", "4bc56a2d2433a80d0e04de1af60966fcfaadaa33d18869b94fe2472dfe5624053ee6525c1aaa99789483ad590b11329b803fde03460a8f8a69e33484bc01b5c2" },
                { "en-CA", "665e585807577d51f767474e45423b261a6ccdba8c7c44c91de019fd34d6520800f0d9ba4c9f841c20883ddb3c53707fd61298d3feeb83b8907f42c6706fb486" },
                { "en-GB", "d0faecb2c2769a486d29aafbea7c6e430fe8c23155d2d2a0b2a3a0e3be608e73841acb62dc7187e96a426f978538221c33f3cb68901022d7eccbcb15ddc3c8b2" },
                { "en-US", "3f34927cca1d4d2c048b583278f00728ea2e73bece6edb72cdb02ede539faedc45709c6f325bc3a371b7279050245ab3fc1d00e6cf90bc1e99a8b0ce9d7e4ab9" },
                { "eo", "3d2c8f350657045d0c100531ddf865a5fb8b789384eaba6e6de4cce95c3094b34546c6f5b4306a5045b7a5b014038759c7371a741011a882884b6cc868e2b5ad" },
                { "es-AR", "d1c94e85766b445e7748d4da2369979d144af8f43782ad4ea7220607e3b4c9cde3dc738400f509fe2259259f336dbb97e3570ffcaf6e9c3e87e74b28d1e82348" },
                { "es-CL", "b8d1753c4b4a465bbad989fbec60518b431568dcf56c7d0a510870b9a06a484145e08d839a1aa8dd81fbfa25da8385ec4560c9dba3f745f48d4e0efbebf52620" },
                { "es-ES", "d2a90c50e4e5df1758cd03400eea02f3c3725e14ee77b7b5fcb8c3a9e80d4bf921578f76b56fcefc53596dc24e7c6d5cb049868fec6f82e062a1ba2182ee7eae" },
                { "es-MX", "0be1e38e0ec01115cec022468145de7eba25bce2de753d6e87495e8eeb7eb578608fd054225e9395fd6aa5641a270e507fd3533540863146335cd011d8458ccf" },
                { "et", "c0ff39b7a812ab43d785bb9c82d8b82720e5b106c2e5381a840cf1da42c3e5a5e8c3bd1d80b6ff437570dacf86c3aec00ea0d225b3e12fb3046b948dc2c637ea" },
                { "eu", "445bde328aafd03295361c967a6f8cac4bd128e54472bcc76232ffc940ab8ddf4870acc4c0be6e61285697112228a70fbb313ffbaf9da37e62da573241845381" },
                { "fa", "2520ef7e3e5508ebc7284ecc777b6d67b7cdaf4ba7937def4ddcf164f310c43f8a46e7602818d7741d20069ea0ecdd741eefacf81bf42cc83cc8fbab666c5004" },
                { "ff", "e83bbbbc066e48aef1c6a182514289eb159b0ebedc4c98b25576dce6752e6a6655d7b68bf0fdb0f1aaf57cee842232b40683096f097ab81687dfb308562e8905" },
                { "fi", "e8a59c9e13ba0ab7aa4c83d9972447d2612f9063abbb4c01533dd3da2371eed14e249ff4ede51115c9ac013ff3824c7d1802ce41d884a77472d65c2777529890" },
                { "fr", "039de9f0ac362763333f6b949a6a70e504e2356ce51644d46ced9603925f7d9534f4e71f0f66c005ae70c6a965bcd55fc81485cacfbd31f231f569a9c601d639" },
                { "fur", "c29eeb366e16d11f8ba33a19ff387a08924bc5d93c600afeaf18f0388ceb1c5f6c25675636f508eeb4b00908531793b91395b2e876c33f6f751db0f2dbaba4d2" },
                { "fy-NL", "9f755f6c223261c0371abbdfd4aaf29870bb4f63c14bf0aa683b91c9c7b46e9d3436f71ea36c6b571ab2853e51efb9f12bfcde7d0aea1778080c6c5556ede477" },
                { "ga-IE", "d19f96ddf78591fb14a1a82cb43fbefa9d0a5faf4425d1b52e95bd0451b99d44afe59ee53300aad8606fafbf09c30fea4079ec5c95ddff723ccc126a6ae4548b" },
                { "gd", "2683ac5894cf75ab8166f31a918df518a60bcfdeff6935d214ed2661f89d68599bf5013fb221a29a2e6961b846fec6856550a3b069e18c74520d817290cc4e7a" },
                { "gl", "27614967c2cb5b28af2c97d3e7fc173e0258fce896a2482bb06b3b845bb267d2c29f090d4b775fff747f350ee3448b2b3347fc12c89016aff2da0e950610ff97" },
                { "gn", "36105f344c7fe8450f567ca68b3821012f36e7d878ae97f4fcc0dd7ae4d9aa999b401a0fcc9c98082bb50c71d9199bcbe9a66c071d75cc6a5ab52a74769d1a42" },
                { "gu-IN", "b14598b12321befb7ed7f6e83e02f34fe146333806e3f149e58ed98ab776e7a4b8af0daa0fb991fe5cc5e5d980cc6076c68b047b9e0dc61fc7bca2e0343a9977" },
                { "he", "688495491019b881f1297bcddf20793006ec1b70a64dfb7d8c938ce73ea00119362ae691fa4b4c6b233b2c113c34b0be8d1d83f4682f87f36dd489d3fc3faf46" },
                { "hi-IN", "99549b7705e9cbb94cfc509e431ea0805464e7ce193739986287eae6534981de2b34f273ceeb676ba7320b78094dd085d89e3cba1574e24dbcc5daa0cce53b2e" },
                { "hr", "38de04aa1c95e1f10495c75a3b8dff95c32cc250443583357ca22c3fca188cf1b1ac03f1894913a822a2980e6e6a6ac7b7ba13db0033b31ddb52a09ea40a8478" },
                { "hsb", "4fec62f383c844f6866458b93854adabcdf4c40fe8d3aba7250273ccb16664de59dff975253db35bccbaf8b2869c1a241c56e3aa8d82b2ef651ec0ff7392a63e" },
                { "hu", "619222bf8e89c0981068d7e6c0227e56440558b754516d8c993894c4110596f06cfb908dfe344f8ffecf880721e96950142782d82b12d25a1bcc95cd6fdbd24a" },
                { "hy-AM", "738094029bfdbbb6c96ef064e3c9bd01113e62f4a5fd99f970e80ee9cea6bc7deb5db5a0b020f4fbe5eb119a143c3386c22c4fdb8f934b5b9a818376db84a55f" },
                { "ia", "ac028d2ce132e5b1f76699ab615790add847a80ec293170a062041fd41fc1d6c570adc44a9471634a47b4ad695acbc5cf28d8707b152196153b3f01289c4ab43" },
                { "id", "ad8fb7f894f069aaf1e7ef4df12ae75a344c5a28fe669546d15b1e796b9b267f30cbc5e0606e50341fd84bf2967c55e5f06bf4554f284eb2173aa64d536f053b" },
                { "is", "9622f3a4866153483358ff2a5b20ece4a958baa2c01fda11bf9970fa2d5010d225b2cb23979fb52e9ad7ab333aabcb9f179f643af564292a94155d7a5ab487ea" },
                { "it", "882e633e4ee629927d2b3c10d4fd18869a4f7b5da04e057bc33bc0543eb1e04db0469c19eb899e3ff1db260ccfaea98bc075669cf37cb35897dea893699183a3" },
                { "ja", "a17ea405daf9af311e7f6667888f0edeb400f87bedb1ff9092516701e055f39716e7adf5ddbb9926e9972a3f7dc664e331c423283238898cae561362d37c0936" },
                { "ka", "f05e3f0a82743d8bd9b96b56d1a54d76b856f21f896b9f5a1ff4d484083598ae3d169dcc1b8e959eb4c80f714202c93ac52b1c07c1e790a00056206334ad21a0" },
                { "kab", "319e6706d7150c379801fd6beacdaff92270d56b19eedc4b740b6679ca74e1182e993b5506b029c6ef828eb4e4c83b52b6c51587dff4e9a91ed982c6f4fde09b" },
                { "kk", "92ca67482f7429fc905294b1c0faebe7e83433d4fb39c436ef73d15134d0f682ab82bb0d96cc9ade86e2c6ca39769bbf22033ae84bc0288f162ff337a73d1ef1" },
                { "km", "6dc377cc56d6452a91b45009f0e5036145fbc15f8f5573e8c897740db6535a46c80d6e968c079708cd3fef51a70d9b240a1a76b95c76ff482ff089cd5a6f5c72" },
                { "kn", "2905eeb0173695dcc9298289c5b28b4c47228b5a0cc5761c7a4f0171139536b78cdaa73bcc3ce00d3db962676739d1a5b3550ff86a9c64b91437cd281a90cccd" },
                { "ko", "eeed213bdb56933024b85ab2a129982691892baaeee2273852b9a86d0c366239d396d365ddad069fd0448bce7a8d17efb9bcbd18516d683b81c49af4286368f6" },
                { "lij", "0627429b0cb0e39b05d08afcaf0b793e896e698023b843d6a30b22128b9224ec4889df005e8aaf345c5a10f5e45bef781b5a1fa45e1ae8a80775839f92ce1b53" },
                { "lt", "a503568bb62d97ffaebdf5c95e654c4cb831ced3063ddef8e11af3de4eb4cdf9313c045ba423316c8b67d585bdd5b3c44ee1e466dfb90221eaa957d6612b6efa" },
                { "lv", "6b901981cf26a7cfa43116650bdec3c9489897bd1c7e979d376c6d21069b15a7a94831c709673893ff4719731efdf3b8a0c2c9acd86e61b9d692a9e8f7d29f41" },
                { "mk", "c1c776241a9848cf8c3065f8d7226706ce8e803f88887b6f07f86b499bc52b41c7bf87003361050e8d511287c32af5d7f0cae5cfdfcecaf34cbe0584925a8ae3" },
                { "mr", "968ad78bbc004c34ed515249cce93160f43ba50f863c265b12faf2002cd995eb4cb1be9d1854c9107dc746f4f9746bc9051528d34d29671ddd105140c2118e3a" },
                { "ms", "f98f73ea30fd1caa3141eb4ae513041f73f92a15caa747c3ff502ceef88d06555f15d3a3667b686d48dbf328372ed9a5e87e7cc39ce4235727789e2cac601427" },
                { "my", "4bbe660906db2ff59ef6b556c983083fd36a85495270f6640cfd47254df76fbad41a088f431e12e7930025a24bc61172fa262d575393826509d27c9ea2851ef8" },
                { "nb-NO", "56d610ce449213a1052b8eef59ce509aef70858d7d08d40aba76b052b34b4483949927d24d64d2e34a67ff7b87d2cb0c0c78f0b77b9cf812c79684a0c98da8e2" },
                { "ne-NP", "a714922bd36884a95157c5f9eae9ab18def4092f8e0c0820b1cf52d6eff93fe3e87c2b86428fb8696c26b19bf69135c8b7b97f9565378cde6d0ec9ae9d613176" },
                { "nl", "55fe275b67900c7cacaa2917ea622b3199fb4762fed8e13d4ee342e89897bea4b70b58e2a13b18437a9307a8d1944450d1c6993c085be97445f066b73e7b6290" },
                { "nn-NO", "944d652c61a1613e2ee48ad382507711e15c7c4e9ad90dd1865882de8ba1bf4af5c9640f1f6256976af629c87e492b98772a97c494ceb9e4f044af239dd0c1ae" },
                { "oc", "b0e2565b0bf5002e87fd9775bd480bd6b1efac5468ae17bcd60b5054456508a94a0cd8f3249480eab45b7d7043caea1f473d308d197212d3ee23e717dfa216e8" },
                { "pa-IN", "54f22bd68b390ed7cbef293d4ac01b6dc607ed8e3162c9c016c775d324e4869f753a3a5d53506d533d43dc790d765954bb460a07efada313dbc96f24de57e677" },
                { "pl", "6f8196ae0b83160544f3914a4f3fa685ab286a49c410a1ef0c8aa310505273c6f0ea8fee7f6bfb91b493b921fbff5d06065220f749342784446f6ae26e7e03b9" },
                { "pt-BR", "21860361cf5c758884ac19db8638f582e2bdc6ada9a5b93655f14db802ca12ffc33a3dc3aa931be3d33fd653fec56f2f523debcb4cff37797dfee5a67f2e1cbc" },
                { "pt-PT", "a174e943fde15f6241671ad52a2aacf4643b0e2200515b0a05d3503d750d7d3c6c7dd071bc3e6accda2170959a2c4765bb66b4cc48577af28a080216cfca658e" },
                { "rm", "c7f18a1e9e88d668e1614ce92112e215a6f6c5b3b5ba5afa34544bbde935037faae721fa396e8bdd19adf7947bfcbb43c712be87781c7b2f2ed11d9c5d8ba86b" },
                { "ro", "71812538e5caa27aa8458073cdecd2b61227506d07a0322073415ac9749d192721573ca6ffa55521c1a03dc53e7129e4856568f2d39c214b86d86bb5828a7f36" },
                { "ru", "2b8822047b41deff36038fd3baa9a1927d9edb8cec2afc13f96965319d56c23fc55628e85f256ad1d4374fedef927b7ef4bdb3a0c9256e1457db9812153f083f" },
                { "sc", "d1fe247b38e2330ba0fc7c7f17d779db42b95850e105f5ba2300ff9718bf278aea4eebd92d1daac95dbaa2c7c6bce5b81a5280be83afbb56340359daa63a2ebc" },
                { "sco", "e332bd1e901afa485e626d4021a6a994dcc8428afa78af7f9a5ec7653c6f7cd6e9afe7b1f9f54b0a89b4822052bc06b73c624faf48f8bdbcd145301d76c02c50" },
                { "si", "13ceb1c2a97873cd60fef2a191da03a6226e0d69d45785ad74d9436fc4168c09451e6b78a3e578d564c352a99e35726bf3c99125211b44198516506ba8802740" },
                { "sk", "8abce3033cbe779a319a30f10cd547fdc2de6306cb68f17b8ef4d8e638cb5dd51272d742070496f66157d4850e34b84165de8bfdad4ca6a3cfb4759a44ae903d" },
                { "sl", "961b80bf3576742c685a3cae1331fbdc05c1c3a69b009c481030a229e7ff74073ca12081aaa5b9b82bbd174ce796730def00a8d85ac607b4141bc2a12dc8b58f" },
                { "son", "9227306c4a72703491314227cd03ebfc775862fcb6681d90b94902ce613188d4cf1d82f650d17e4533924c16d0924c1374b50bff8feb639fbf5004fdedb007eb" },
                { "sq", "eda9882fe8a1daa5c8caa49a4100b7314b57032923053a47aa8e3f2e9f7b96f7e3ef2bc1b9e5f8ae1f2b4e3c74def6e2f0da85a5f4099dfe03aa8fc3821476f7" },
                { "sr", "059d2e550a6840b5de90cdd549335202809107854e23543f2cbfb4d53d2e6054a51aa56971486e81edfcad2aa465af8436e28867c9f0fb7805e4283b93eebdc1" },
                { "sv-SE", "60c927ce4234f216eecdb08e4503fddbd966d73659e5edb93138d07eaf94d9392f306afb98af54b565b8d0183a9dc9ba2df0050ce2bcb619c3a9e08a43867529" },
                { "szl", "5a8f671350f825b13d082ce16eb89db8024fe473259957f3d680b4fe252a83b0fb445930e2f1db24563d29f8509e052cdd2db02de924446964fa8a0c54abdd84" },
                { "ta", "347223f3045c6f11e9342142d46da85498167bb936f91a760c2182f97388be315d2d170a03a7a27038cc86d1097e1cea0cb68938a6e6af63a5c69f6a69f965aa" },
                { "te", "b4351032c2a3ee2a99b22a312317fbbc4e7832332d8b84265c36a2fdb2cfa0e34b0ea0a349b357fa8e688018cb9dedab7514472a12be2f456fcd3fd21b396ed1" },
                { "tg", "661de9ab96b6188157e7266cfbc5c54151d48d4b03c893965945d2b144fc88fafb43b5ef4c25192758abb2a25dd25d2ad01fdd6f0b735d767df040cfb8011650" },
                { "th", "72b976c035b69efd711157ef8d16c84b2bb52849b51f64ec7ea51cf34010b9c23382df07f79e468c572c69c5c16f9db76094ff1dbf544ac06cfcce227c178bcd" },
                { "tl", "596ec0a68b5f00a053b93cbac03341c0086d2fbf8fccdfaebdd97537b65bd75e8f6d956adf21f8c8494f72e84e27534f1121e8b9b99ebde9ee61abd7c25132a0" },
                { "tr", "e9da14c7f7d39f1ed188997eb0a3d4dd3e8be9eafa7aed03c6ba90e80a2654b33666be6fdc47999084ffb42d89dd5b9752d132d4f73301e9325298c7c8fb874f" },
                { "trs", "688b60f55a7f6ed46a274b59fecf93bd6ac386513fcf62d1661feb6e41ca5db176c48b6ca6f2b0634ae39b3d32ed92fe1959f38faab47c544db8b3e889d6e479" },
                { "uk", "68c77741204610a232450389b726574ee5d0e82876b046a5edad7ac800b05ee0395ef9ff63b7270fcfad803a64e096c5d9e98c5f758a8c10cff9cc47a0fa4b3b" },
                { "ur", "178706fc53ca44794f11c56ee8326d2ec41e70e3b65390d6094561b012c4d89ca5af8aa87203f6962e1535b40251be05270beb8ee34a7767f9485b30b14e881a" },
                { "uz", "ac0a98e2ed36d78cb4b57fc4906d89521c652a4b8e0fb5d7dd2f124c0031cf93dd57aa403abb0cf208ddbe692a19f8aa562f9a321df73bda34a69787a6246fe5" },
                { "vi", "ccdf2bfb53af4cc09ddc0076f9d3264d6522a1b4d3c7e35955e9614ad60c5bab42ae41e24a54424a59b37828f2dafaca96b9ed921c86261797ce4b79b83b48b8" },
                { "xh", "ce70cca4a00959fb3705383ad59a5a4a2728033c56210c5a103b35ec19dfcbd890fa16f1c6f21e67ed944c45792b7f2dc17dc43514727ccb5de9a27d5254e305" },
                { "zh-CN", "6a96559b392d98ff48c9b7c3919bf1d6a5c6723527d1b5df8139b689c829e0b30d481784b1dcb605e58039c3b6d1418ee199db6467680649b3dd68720847eefc" },
                { "zh-TW", "34eae59f49159e0ededcd0a33bdefc2a0b63dcc32aceafd89c31c1647de76062620ff9adbdc28b976cd6420075abe3786fabfaaae17a8020b62e253ce6e7c900" }
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
