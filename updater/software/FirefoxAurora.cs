﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022  Dirk Stolle

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
        private const string currentVersion = "107.0b9";

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
            if (!validCodes.Contains<string>(languageCode))
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
            // https://ftp.mozilla.org/pub/devedition/releases/107.0b9/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "b075ff0931c7bfadac32832ad3248cfcf801d238039f388588f35ddd91b4dc1bd93518149d5f540e34b5fb245b7fff89fc4a34711e543be5a906c45c8a3aabbd" },
                { "af", "6402062e763a8151e07319b0d2970e7c504f64ce9de01ee27ddd954be4f1f4463c75dec54cb6d621c6b973c7f7e68def230a1bb243bd4b31db39de0bfe960669" },
                { "an", "00e67457e2123417509239174e885f47f59ec5014cd757d7c1198dd6675a6429b818b8ed8488696f0ee23b87731d3fa080fb4ea20f82223ea3cfca42fdea2ae2" },
                { "ar", "28217efc154896b029a4a7f366fc054049b4c5b09650e3e533915f77d5f6d0fa7a35b01f8ec7ebef566447ece6dda8997350d5d2f555c87b527515d93e30f58e" },
                { "ast", "f0e1adce0f2eb85210d599f7fb84c174626eafef85278cc712a75247f440a8cb0bd5a958b7b186c30adf3e6712ac67941336a1b3d9436d5e4a6a761ff1eae9fb" },
                { "az", "2e0f34fe7693a84d00404ffacb49d9243134252b50aeb082b6b60e6dd7636be88ea10aad8b1336333f8b99c8fb45fbb04e941e184ba15aff0d42b8c6966afa00" },
                { "be", "f307f96f271fb83a080bd72d7175b0fc645678d04e5ca1215b2bbc41e700d370ce9e6bf989ed0da07626c5d735eddc1593782cc98963ca934106b5723fbd50e1" },
                { "bg", "43f055f0f925d46d17cd558cb7b743d36b69b53585f4b691583f7e38ea358b090f260f8190e35750d8ec101662e88e0982bf3ddfbfdb5d8e4fc88ac842539cdc" },
                { "bn", "edf88a4601f6302412fcbe92b34836e7156f174b2f291e556f80ad32293ede31f77a82544803cd6dd81524476619092756b95d7ddabb93365f2494018adf2022" },
                { "br", "c9d2c1e0ddde6273e76e01d7b06ffd373048c226dbe9c1dd2b5d3cf30e9536d75ddba8cdc0c4b0c4b048548bd4040b401fd44ccbf1f18118cdab829bc3c6f79d" },
                { "bs", "e4b56de88a9946af904d86962212b4009399dc4dde5a867f562a34f7a6303da5c5ca16e54d0db9db8e00ecfadbb72bc1be6ff8865040b0337df0a226b2d95c92" },
                { "ca", "99d8377229e3392f816e7b8988aaa8e234403e4957ea158e441dffcadfe8d779b062194c0c91431eff432e2400fabb33eff865fadde1a7770ab2f85f906dd637" },
                { "cak", "e5e75e338c6c04e24f5d5d48214213a169e044153acac626c2fce7ab05807a36aea7ef8a5bbc9e6838fcb3bc0d7b6b1a8a20b0633a25825886adcdf1c23e119d" },
                { "cs", "23677c8b7309de88dd3d0ed98af5a52969f9a46ac9f066334df38cf1d12e1f81e6ecbe5301ca9b80226d54368737d93d19df743d35fb957d79ac686b671016cd" },
                { "cy", "b514418d399a53c5cf8c059b11d9f393ff5de5282d90d201e5515af6ef34626708cf8bd751fd75f1d0dd94f165d1b13c984314fd5bf215910a3fb9cdd955323c" },
                { "da", "f306cbae1fc5adba41851ce6058e9cee96e643737274ffab563ab050ef0bd8159c0643b2281761cf997249159ae2b7896e43674fd997a76094e1aced1cca7601" },
                { "de", "f03ab59b056a986a0bd625ca2f7461134b109d2e5ca6231196195170ba1934dbdb14842054ab47c4bf96b1dab2e7f51e63fbc19afb69883d90dd709c23c0389a" },
                { "dsb", "cad25126db295da08ee4884c6e7a3bb50fc7b014080ebc21ac778d91f0dc04fea330a9f1f055ce8b4f931deb75fa752f6818095b16b70cd72d6a98e60d43d393" },
                { "el", "f3c755ab527f6ee99ed1d2ece36f8d2bc615910d6827f3a9c1cf64bab72d6955c436186f7c6a553a74d7328b08b97ae4e0f510547dcb7361070a3ae89ec8062d" },
                { "en-CA", "d05c53ca25ffd75fd1eacc42fcdcc81c0c85c6580b90778283a4d5392137a39b00e02ab294721db7ba1b04492b8e2b1e714d5821aa9150b937f50d4b1b879d89" },
                { "en-GB", "6921bb18eaa1b4ddba646949c5f2a6a21badf488923bffbd51fd22b20ddaed2052846bb1b614fa8e326bd2fa267359a723f05cfea9f977327ca849a9b7f7ca88" },
                { "en-US", "d1684c29c556aafad9f549cec86628ffece664ff43d0af3404dfbee09ad972d4210e37eacdb492d40316059e8e07a10fe82cf5a1a70b7de317a24b7313a3f8e8" },
                { "eo", "4996884b11c6a1730725e215244d3b945910b7b4b3b9b71a94ba07db6e6249738e0fd40e1a6db1b542d0410d4993eb54dd30382ec52dd88bb3a9bd08e1016f8a" },
                { "es-AR", "eb615a647b46a9275ad8f119b83cb162420803850127f44edefa18120feab0c17e3d2b8d72bb513e98e78c97876ed34b15c256560d8e86740ccdd75f6ced3bda" },
                { "es-CL", "03fd8dafcb21a16b2f84132901a7434ac6a086026e81c063ad6d115531031c562cf3447fd893f4d40b514fac6d23c4856b2cad667f970aef82670d89b1cfaa1c" },
                { "es-ES", "dafda13e29b65989489dd2f0e34c96cf87eaad83ce147e213802bdadd25be29bddc7a34a8032193cafea97bdc6ff7a333dc7a01b304e53428b07d2bf54018874" },
                { "es-MX", "8da70770b86f506de578b2e89a943f6699f36382b1a78f53110faab1ec975448631ba60b4ed1046a5b91ceb9d5b50860799c14654b1a5dad6f985e81c6980954" },
                { "et", "7d87d82e05696f5ecb83b9a53fdb1474cf17eefa2e138faceb43c483eb7e1226bf8c3e47ba81e6854a294aadb6c94f6fe4263d10e222718c217a16a61186563c" },
                { "eu", "87bcc7ccb4ba7af1b7e5a7aac4b5ce40120886e3df053d70ce2f0a4991cff031c9872d518da6c4a57a1d7bc6d48e68117e5ed459bc831b53807dad0d0dae08ac" },
                { "fa", "833c059bc09d0a2e7ee2c0286c64721e29c3d28225519d9437c6b1a66dc6d91e4c6aa28af4161580822495a565729f19ddbd9202b09987037554f6449e992ce3" },
                { "ff", "3f6c5d4d11b1fc831e4b171e569b17023acd5c1e9b15df3624006465d4d2c7202b268496d86427bf330c1963e879cddd2a1ceeffd43befda85864fda2cea0d61" },
                { "fi", "7776233438cd279bc718b7e780d0330287a0c42c4a3e5a1bb75bf4ff7ce8de2427144aefe761d0ae845782ed742f75b1f33c8bb95fedb799535cce18d06e9b19" },
                { "fr", "b76c67efdc0159940319808d8495687a32abfd44a99a226e2bb078c86a419028531547702d289858cfa4e4cd3a173582c951fd1ecd47b5906a547c67bb90c9a3" },
                { "fy-NL", "d990d8012466c49316d178e2a7192e3076187a70c44cae21adab4e8c4825bf000e78547e843b23596d43514001391673937d359e416631085717bd346b87f718" },
                { "ga-IE", "ee17942fb834e171d85f0b1b2f15c1c310e52c8223f26255dc34575700688ab96ff6eb95e2b95a029dc22937b1851a5d12fddcc2d88875ce9c3ef37d9cbc5dab" },
                { "gd", "4890335b7c133fc3494f4cd3ab77bfb9874755dfd3342da8c8ce6a0b61a490f97d3aff5a41fdc3a6ebf320fbecc304b9c2654a9e92c73f6ea017e617b9f79975" },
                { "gl", "74edd27e6c69a22f7714fb4b7fc9ee29ba276f3623e6fe1a742f87472e2baa68549db3bbf7eedd3dcd67dcc119c0c0d5ca329a13b39f21eb76cc085ff4b827c2" },
                { "gn", "e257d1176604220bdc1e0253d82383fa995a117c4b3f1761e76e0672910e72e97525288a0361cfd233672b6b83aa1db89b461c2aa85467239f976838cd0f538a" },
                { "gu-IN", "6904e2836ad85e1b6a8d0f86a643209ff9930d96f6efb4a989f0e999b50bf685a00d8d7ccee2e87e99a9716cb2df7092f5261acfdcdfb6dc267501e725ddbe97" },
                { "he", "be9c6cb38a8362ee2941fe4bd781a1f10a042ba8064dc23cd000d0cff67f661264063f9c278693d974560a3419c9e0042a827693ca69a99cff553fa98c51e480" },
                { "hi-IN", "090265ed9958c90a2df0a48107e68f96480be201c20c66742811cffef23aa22282e7c6caad1cb6c8e3c7a83c830029db83370141008435f57c8f7e5e5b97f2b4" },
                { "hr", "69e4aed00a6d054235f306bd955f815c35a05a6f0d9df1ad2eb0dac9af25eb2d564be6b7812c2d4e9d50fcdb1877de1c5cb2e95b45b0eb5ba5abff1e4bec50b6" },
                { "hsb", "9c981881cffc980e6b2097ed0f740301b0918e1fb8a307ce9b74afd75029ca25d49b21986875d025b2cea10db682164a1b37ce387b0ac5bbffa4dd59777b1cd9" },
                { "hu", "666d6c625806cefa80a071dc2d204b950c61ea155c0dbdbd43b73325bd1738827efe858a01cfecd25b09e7fb310281aceb5ae712d220a19c2d7ac910ea2ee815" },
                { "hy-AM", "7b9b00f13c3e022c864f2e386fbe0e5374455978355f21c59a25002f2b1ae4ea72df47fad38bd0095a0d00c23de8bf73103d9bc77aefc5e20e1ef24c8ac55abc" },
                { "ia", "4bfa0bf9f36913e67ca77c033019c7e9d27a2b58c6d49a6f2f5faa779c2ba3bca8c85511f099465fe4bd681b7a9397f69f855f1113863d8ac835fdf6ed8ba279" },
                { "id", "d69c3855b76a03bfc14d9cc562ce7b696a48fb181cbd347593a0d6eb1ab28fc1fa543621c93da01ff0f6f1a463359da59bec9b9dd5eff35e419107550c1e4257" },
                { "is", "42533ac3c68a8d60108ec9dd8fb148450451575bd55c00b1598be3318e86706fe992e8442c2c53cbd316600968db7e7fc955f91bc7d3e9601ea26b123b668a84" },
                { "it", "f34fac59122c4c418f4ef178c835eb33eb03a34f430ea3be4569ee5a5a475ff0bfa8aad50828844b5a2254c59122d737eeab4f8f551c465156561450e02cc124" },
                { "ja", "ba13a4908e7fad0d1a1026e1dc1f27aea25f687055341293de97ddbac1b8841cb4faafa860e8b95d102b454e96f12ed915dac3dd7de97f6c1b63bccf8aa26ba7" },
                { "ka", "acc9c3f685b3e15054cd70a0f0ae75b7b749b66a0360a9b964eb4708e839b0c5f06c79e0b6e42c6a2b0c9e2ecc391aa96547dbaad4e881e3597d75a99e4abd72" },
                { "kab", "bdc2e180972a01270523b086cd3879819e85d69cfa58c65a203587a5af21499360bb8be8d9566f8a0c267de0eb8887a717a791894b0b20cd834565d94727a00f" },
                { "kk", "d315a4aed0339f63c1df6d1c980fbc2182b32da06bb8fe70a277a00f8787feacb27c6e1639359610e8cf81dffed53231be13d5994bfbcd0b49fbea001252b8f6" },
                { "km", "6985bdd5ba33efd26dc97ed2427333abaa294286d9c1dbddad22f16d50bb32dae9ebbf97ad45c0a7fa1960c26a5cd7d3b5661cd6e6ee333a9c1a63bcb4b3ac5c" },
                { "kn", "c77b419f66828e60acbebe89f032493d72846d94effce0159cf6c168aab0e63ce6d5866f5cdd3fe2d447984adc97fda3871188b3c1f2f2868dc767b90f4781c6" },
                { "ko", "8caf4a549df8d4553b653a105d344ec37f8c937a99619f34c78765e8a3e490b258c9e85685a83d14bd0034a0ce6e7cb9d1d7ab242b83520f2755ab040cd3c74e" },
                { "lij", "29097b650cf93be732ac8b3fe7a7592390673e81ce20e91ef945790081aae3e614033a0f3b5bfa8e2d0fc975c9e75c5a23502bd1204f59a6f7e8b60228d0cc92" },
                { "lt", "cf604d744f04cfba5425d0281b8a58df2995093a868d0091c425dddde896fb8c8867f1b49f11ddf3f18537854f9c0c698940f9e824624fae64957bbbd864428f" },
                { "lv", "f06801b7395177da5a55770479cdc3469ea63b77854bcbdfbc131f6c9ab82885e139838028f876fb01d8cabc764c1b0112ff70a95f0005557d252e7245624b74" },
                { "mk", "2a8e08f9644afe6a062d734182442b61cb79829ed1539ebfcc4c4a0830a86b60dc12bd6a8549ebc69293a895871cdd4be5eb03cddbf16128d0afaa077dc2236b" },
                { "mr", "67887cdf30fc5fede95d1bd45eb337e1173942f34754a7a48dc85bd39bc3c6c496cbca10aba6177ee2a61e56fdaee2d272910c194ec0a66fb3cc66d552bb259d" },
                { "ms", "7ea6cc1e32d3edefffc8b3243e71a96e481c47d9b24d6b8ce3bb760ac7c5d3e50602d87bb17bddfa44eedc27e3e61cac3497da0a531c1032e4b2dced276e8777" },
                { "my", "54e47c4feb69f5d6581df2792cd54d10f8c02f837b898ae565b0f96de98bf4ec4e6d4cda4edabe93acff8f2093e5f1fef9c48cc83c1640a4a04b3030f91b5964" },
                { "nb-NO", "71a48acbae65caa1e3668fc4573bf5f4184981b7d59bbdaf41a610ef59319d00088fbf34bb9554af1fc60437554d5f38f18a5208787ada38fba55c67d31e1b74" },
                { "ne-NP", "345c8e1fdc8d3e960617c99cac908aed3d5e8e6f5703ce310ac7b88cea09b8d87cb4afac418ba196c111be732be58ef209fbb85dc499e5495600ade46083fb3d" },
                { "nl", "93eb54008cdafcb196536a6c1998a8d181fb6c59caca6fded8c83868d94e77c4bc10fcfe8e8208cc8b3cf8187523e7cebd24240775912e8ef21494f48f7b9e28" },
                { "nn-NO", "5aa7866b4eb6970cab615b9964d65480cfb07655ced013c545736a75774440e1a11fe12a2822b52fcd9e9b4b91b61339f43376e9432585489dc91d67fe482f43" },
                { "oc", "748ba5d1cd6cee63ee76b08d3cd0c38bf18b7decacbc6a9d5954ddc460a577ed88348105ea12c2d76ba133ec3b061ee3e56e4654363d213412605a2e38f2c38d" },
                { "pa-IN", "5cf62fb1b9e3d20dd2d01a135265672a43dfda20ba65a871121a39f510e351e65e64125f2a3c72cd1c82844a305b94e02bc6b6bcadd599818485cc1205676d7d" },
                { "pl", "272cd8bce6b7d35fd1b8f831a01d98596dec56e60c6dbf37319e535063e6f91fe18bd0972fa102bbfdbfbcc1d6998a6762b1ff008c236cbd86ac6af54105aa42" },
                { "pt-BR", "1a4c4be7d9658752739ac0b3c84af2a156cbb18917def44d1cbbc1adcee3f09355943f63b92d8a5f61b1c47ab4fb108bcd796c388ecc15894ab2bebfcd89a7e0" },
                { "pt-PT", "0dd85eedc888ce14fe9f0dca7e0819d47a71ed79395bb23484bf6aa85def32a8406ad98c2900cc1fbb054cd26a5f3a3ee9188ca137eae61126d15f47fd78101c" },
                { "rm", "7e6d8a2e30702bf37180a8820f49e700234068704ee2f5a5ac74b120268238d61f291541ec98127e33043f886da974967aeeb39cc11d12bee59fbbef602cdb55" },
                { "ro", "396dc806b458425af098a63007e54c567e0cde6e947c267a6d7e5f0d87bf35d87fba62e4072b648c3ff34be109e24381d6fe0fe3b7a3b2f53e3790ea0e12791e" },
                { "ru", "4bb72b517665fc88e89196442e8e8085ba6922c77cc60b717c137e59bb1f843760375313d09023e3fe73dbf1d2aeb555ef2128a2d76dc3476452a6bcac84961d" },
                { "sco", "224e74655e08cc3f6771edfaffd0fc25ce1c867793506838983a5e069813651e1029418aec755acb26a0402f16e0cae08c3a4d0c42b93ccc974435b12f82adde" },
                { "si", "f22cac278bb09d92d2e23177f5a6666b50f5d3383d7a15a0dada81820e8c9f70bb7c286b2d9a840c206ca922f4e669765a46c4ee22b497921e71837962fa5b44" },
                { "sk", "0393b8c1f45ee7e8ceabe7999e3f561863adc5347adb9d0d27c5c57757d6e9bf120318a4efbb65ee29cd55220e3b642667b7b14a55b8d6b2bdb8a04349ebc7ef" },
                { "sl", "532b13c8cddbd49f641a48007f9f48b8c21bee611690ca384f9bb5e2abebdd3f67ef724bb21b0b73b179efef96fe238c9586fe6383ac3c86006426ad75079366" },
                { "son", "ab505b459dbdcef339e35baeb8c9c5787289051824d66860bf1b0bf39d89ff161b7d49ff1097c0467e4aa90f2397d60a82ee1404370e8db1830796f1d36eac80" },
                { "sq", "f469dd6463bb02353908f8791f90ba50ec8720ab26c3ae4447a2b5beb7491aae768059cefed2b18694a6a172bde2b7feab342e236cf7511f6b36bcdc882a5d52" },
                { "sr", "cd18f6d19c28f39bbf0d4ac33870f85a916f4e3f3371f15a3a3c90c618e63ae8e0bd5227e05b08e699507c5a3f9f24d44ce8f6804dc35ddd644011f6f7c79351" },
                { "sv-SE", "01ae2055cb7f44dab51310c0495df2d4493252d5fd42c87b743c4441776b22e41c7e749afb927fb45d40ba784731ee42984a1742828d280f25ee1e618c34a266" },
                { "szl", "bc63c20890cb5faa5adb408eed754e32cf9c978a3caa6ede653d269ec981e5bd9075ed1fec1d7c808e84c26ae4a01c7631cf8892fbc22110c18310dbfd3020bd" },
                { "ta", "5fc93b6dc64e16efb8e2d00011a61a729ee977b822164b90a67251c7cb4bf73b12eaff20cf4d582054e7851f1ecba6a1765a7473e88935b900a15007acd5cd16" },
                { "te", "86aeaf0b47cfc78559911a80d167b15b4f1292780385d83dbb84041a45a98ffe9782fd211be338645f96d59c5b0b711465e26cbb2f3db6ea53b12a00bd2db8fe" },
                { "th", "de0fa88df69302b5a33d315fe2a097470fbd4d2f3181206ed0111c1ea65324745a97eee8c2c675d43541dcde53a2dd4c9dfb7106d55e1b05330c4e86090f7b68" },
                { "tl", "282312f1c680675b28a361bac0d9928176b2ae66197c539cd69f564aa00910d38a04fe9e5cd68004449ed4fbdf40832f63e684cbfd86afc4db6e8be247bb162b" },
                { "tr", "bc1bb5b20ff4a9feaedbafe2517f2d198ade352d5a0483b287a6f976984aff535dac87ef9c25d5356bd113e941cbde60950b5acbbf7983752dabc59a45b87920" },
                { "trs", "1509d9af63a41d5327bff60f9560df237b9d220a2fec7a9633ad547de5317e4e7b3607c2c6b41c7ef782caccd0f5c59793b3eaf740b2c4798abe24107a82968b" },
                { "uk", "e6b163bc65511b01e057fcd7fdf8caef715e928c631e0557df794d9d9f74a5d4f4d097715a669e124db6aa67fae1ccfb465f02077c231bc18733230d9e40958c" },
                { "ur", "15e6322eddcae6149c7bb44baecb6b35d0ffed5a0aa1557f9985100d7cda00edd9b659ce29e8a2ec72e8a20714f364999b0b68c56094a94fba4660d413efef1c" },
                { "uz", "830461e1a68f18bbb1621440ce37a671a8debf4aadb24e691f440fc4990f1793c119a593b038ec7bf9b176b69543da8f1a1f6011b2d53b9fab658858966c6552" },
                { "vi", "58c50800469d904fc317ac53d62701f893def646dc450c7d443cb5be81feb37f783a032fcd913a400ffa921857fad95fae9a108fa7c6d292920071bdd7cebc9a" },
                { "xh", "5000f21d490ab20d792c86bb1ce3de320c0e526e52c22d9df0cb2a7f293eab2d2d98974e843599b554f484b12e537d6a01e7c09c951381c22ca35aa080c2fc00" },
                { "zh-CN", "2fe073b6facf0d97fb6c1c1ebf17db93307ba6ab3839de77300329f337c10139b633b1536cc1f3a387b8f6db03a09e49da199ac49bfb732d7dcc14c801bffc3f" },
                { "zh-TW", "bc4a8bcdd3e3cd27020ddd6988acdf10d3f9eda5dae89d0ed1f8a48952906381161d815e561d247b449cc8a45259cc20026d2569732a707d09eb53f710575478" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/107.0b9/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "31dedc57e339d667a7c9919e2641bfe0c912b47d178de52f69f5524f5dd349fa132daaff9c4a3487dd69cff6b576236af13e1fc2886fcc0ae010e557bcc6e4e4" },
                { "af", "b0a4f211b9817e3f618f9dd7a4dc1d71c7a6098a5d589380125a777647728c822ca1a0401565d15d05ad7470a71a8d83ffc4b1b87cd9e32661281e8dbf849a9e" },
                { "an", "39c934d1749df5716781463b75740be9cef5f0ce317e89435549138c357936c276db2297102504eca0e116391aba721aea3377653b34f2fae950e8ac1545ec5f" },
                { "ar", "93e28194fcc1c482093992aa966b1035d08e64b7f08f0cf55fe6c06a67e0a6d7081c150f5461d11b472115bf6cfb065353b3b9836903786e4f9d1522aae68555" },
                { "ast", "699c3664f68d006e5837130b1cf727da1b9a9b43962506463118a6c153d05d2f638601737c423c0f5df648a8845974fe5e8c4b198d811d93f2beeddd205ea923" },
                { "az", "c960bcb29f550b0d819ac587f95d0908b1e07fdca9f2f3eae05670b058ed7a8e8e544e3ed4793b5894da228145f7d06650a5f53330ae67e506a17c26c986cff0" },
                { "be", "7f79d89eec1796e0c46cac73a02d22a22caccff593ec988ab68c67e0580748367bea4eacb04ca55927eb78cc710bdac5ccad81cea0934893f82c44eb98f01fb7" },
                { "bg", "7c36365633810fd1605ce355e0e5812a466b3b074f83d773fe4766da58c2da70e12edeb9921f4b860a4e6253f43970d474daf2f65ad02eef08a52205e7fd281f" },
                { "bn", "ab8edd19b4dcf30fbdcd383c39088b149875ee53337f92f1c05a5a8f976572046b1f66a454b65536515ffa2e8ddf1ab8e3d1a1ced41235dc1546447348cef6db" },
                { "br", "3fbf2e563f18fa9022279c4cd4807e25f2017b4c39159ab60c95cdc42cf218c4470dc2fb534b0820ecb8bd6a8c5cf5b2b54216691e53a7bd999f2eb355830c94" },
                { "bs", "5f02e487b1c7a674bcf65185296b5d5602bda336eb1fcf60ecf0e8b06ed33bce088d01120f17bee996fc87f23fdba616f850fe25d266cf373734e420929bad9a" },
                { "ca", "0bacf41fecad52181c01adafda71a8056d8ec8b9a45ae1127b6a1af554b8583c79f1ee50ffbae1a87125afd323037cd8c080057fbf054b14e18b47db2e59ecd9" },
                { "cak", "17f0c53e151f3504c3c540d1d5b1f93847087376e7ee7ed806aaea05fcec2c6d9516eac630adf766d54439438bd2b631baf7c4a34f6cc146cd082d5bf9650398" },
                { "cs", "1ab984e4ff54fd5e50f2c076cc4f994c4dfa2291ecb51d045e1a64d2e98f5cb8b3fb95cb7478264c57095d781d36e7c3a36d4b4316039e5de8e2b79efd9c2cb3" },
                { "cy", "3f445fef8b1e1039a5f6320cd930f19b9b04509a2c8e651ed4ab2c42803bccfa022f8a9e4c779ed2228f6857f38765ffe16aafb749af1b5a56c9df7326482c42" },
                { "da", "d10e08b9c8a24da4e01856cf21435542bad8fe35b5f2ffb67254cf8785667d1d3f232a1685ea492d82a053a9bb2205882f8e92cc8428c05db522455ec39922a1" },
                { "de", "3340674712892a435d5ce2bc33e3d8f3e331e63a014f6ccbf84cb4b0db3bd84e7f27690be7f9cb5739c52e74fb0d8bb8cb6ce9dd7ce473a9194f27b8e102b1b0" },
                { "dsb", "3e7ede66811dacb011f585bb3f6bb1b6c5c910a34ec85420bfba99c3045abc9579ed70128a3ca8b86c070f3bdbcdd668c6b3d38ef84840c9151b82419ed5ecd9" },
                { "el", "9cbe89cf013039abd13a9c8f665abe55d564d104a43dc1fa56e8b8e4b1ad345cea8a9d75619e9ff7d869ac0db477115c8cd7f3798a264c5609e28a8bba4933fd" },
                { "en-CA", "051cd4098de0e1d24c0fdd600796da6e521262b073ab31480a4b6f721cd033593345afdc96054836f248bc5fea255f1f18ff6f7e14f9245e7a28630ae6d24cc3" },
                { "en-GB", "f355b2251ca542dd771abf64e68be4a7938443478cc2ac64ca391660947dfd2f9fc730f2e726ed364f9cfca36d9999931becaa9b427ce3b05c690daf9972038b" },
                { "en-US", "bb7eeb5a1f8d0003217f5940848e7efd860843b596666b64cc29ab4f74394e663b55ee7f3a6cb27988cfb84761ef76de624d785981f41e38204c96e7b080a329" },
                { "eo", "2f3100d55a20f8742922c958d07be4d7f020d290484235336a679d1db800a66a439e731441aea70e2d9f5bddcec37526800c348a7672a24ac106e7aa9620a301" },
                { "es-AR", "b3e36885cddf1f71f8d4b0cec2712216bce2e71fdacde51c9a191cab8ef7137d4131acecdb9d3fbd135d0127ad26cbb4bced0ad6f66c6554da0919ab26a11dfd" },
                { "es-CL", "060143bfeec175b0f1a3df83130587c3bab879c88672f0ef04e1d8388a0ad2894a01eda9d2a711f3bd4753b36aeb8fbb010e474568ec9f0d7d47fc89fefeceba" },
                { "es-ES", "7a1a811783cdf912e18683fec35f4fa73daaf463d2b876cd9d19602709258de1ccbb9d0698c3df431163979f6befcc5273147c81f86945ab35cc0c55f16b7f7e" },
                { "es-MX", "20228df1981e9a179c5a1761864467bf1ccc4cef96cc386e75d5022db755b1503d32e98f6b0a06a47295a181b961ed83801656b0214c2457670802c2aa67eccc" },
                { "et", "a04c8cc73af97381e0b7f5df04ab53a9dc628c325eaed60e99858821243d540fbbbf8556b64848c490ff815ab73ff260e2beb38a543faebd48c839110d64e7c4" },
                { "eu", "81e3da64888eff7dda1d5c7f111b0a04eae14aafc0e54f2761050dc0831f8be33fcaa03c31546c37e8741fcd191f084c0a099458300d81722294cb12fb4521e9" },
                { "fa", "48d1a36d447c30b2e8ca0f96fd107e0b2996cb2cd19430ae8282d4cc59bb23031d0a2692a58a105e9692805621d0cc2a9eeddbfd84923e6ab4f68e7b21c2b13b" },
                { "ff", "8741afefa8891925a89fdd2e5b7d5b8983cfde1c7ac49184f9008ee25d151d556ecc9a5a68b3615f4d4bbf35bdc7c3afda26a10699435e8809621092f5665d99" },
                { "fi", "551c1cc7be6d940ca41d74b318e2d891a60b1498f7dd909a1b512bcd1350f48b680fd1c35a1d2b0a789b27fe3084aa8608fe201a032067bbc379d3ad5dab10ef" },
                { "fr", "ed50e7c02309e74f03bf26e02d02b06bed6f9c5bab45bcf2ae3c4f6d8a26e533fd74cdc7629a9f17bacf5ec017401e80b85dde0f28f7e3768c62e09d2af30974" },
                { "fy-NL", "7d3eb77c357db7cbe7f68914924bebef12bb85d0b3430f6a25966de0ab0d81fea800c964983151755260784495002657ddc246c0b2380b7347faf4dabdec8f36" },
                { "ga-IE", "4754b023fbcdbff22c6a52e12b09c61edd3a080ddf8c5b7d0ef21f896c165417567e9ad0b3a1cd88e942a38a4fe5b0d71be8bfcccb8107edf15f6d9afa91c59e" },
                { "gd", "338e338338894cf15c780c844c6e15c1a57b74300795154a4408fa01430af9ea37b99e7f6c220a037ff6c211f2dbf5a5e4a72b6c759c66a5c58bcf7b8b8e7f7b" },
                { "gl", "663fe214278728f61d695f6b7591af90db859ea2732c17c33fa3f7c63d2f900c283dcee94547557892754c4c14e6653b9d1cd12ffc695259dce43dacf99bc6fe" },
                { "gn", "6902a1e440e546b7c54c0d3d9ca17abc0feaa129446edc8050c78c1a3d0d39122d3752f4929c33cbf95ecf2193e9eeb0f3c3f370e5f7339b3905e6afae10ecee" },
                { "gu-IN", "f5c08f1acdc8f2a64d9bc66ff892b5b677c2e1eaa1927c6470b2151b75058a332b1fe8573b88eba0446fd93ae34451a4c3e0cda1b15ecb1bece3c1f6438ba1f7" },
                { "he", "4d3ac474137be1ffc35f6310ef9f0f58a0a3a3eaa0fd639e19c658fa81e035eb9dbc6b2fec16761054024c6d5ff57e939a31cfca1c433eb62f1bf02acd8ed6e8" },
                { "hi-IN", "acfdb02bbef6253b65eea39e637d7cf47307bd2953165285afc35b80c4245cd44aac73d485d1d2990507497ec9f42741a7b4790e691bfb8d5e1de4b0a7ede542" },
                { "hr", "9c69e9bbfc3eb9378302b9f86041a5abda053d483c09728c8f5c6b4e2434f2a91395eba54d127958f81cd9d332cf765573526167b83f002280ffa5ee304ebe21" },
                { "hsb", "147a67385f171dcca14493e6d230b5671f7640961cf9f3e2364bd3693026188c09c2c39425116451cd37286725b2ceaf3d4531fb5ad23529f6257156db3e3e03" },
                { "hu", "99a4e20ec78a15a64ba06123e7683cf1795824e58f786da58fa181186abb911d86b1a275c2a5ace4512dcfb7f2adac600ef7869131dbf71081108e75d777821e" },
                { "hy-AM", "3f5859f2c2d4ae5f05ad7ce4449c5c7a9a355fe8a1db470e39172ab2dff61ed5f27075c201332ea3eddf3f2220ade0b3c49c4a0b2f2ed6766f84c16512eebe21" },
                { "ia", "089439890e7f1f9a7448b5281de6363cbcd9b7ca5a9640eebffa0e2a2a475cefb3be9829a25fa6f2e0f94c602b669ec9da19d574f1db32ddb31ab5a7a7ffed50" },
                { "id", "c2e08b27c6e62aa9b0564a5eb5f1b2ccf8e7d94835dab7a184ba78edd040d0bf4a839274ccc0831343645d975c9a6f2a6fb1c37bba43285f46f638582a5f39e9" },
                { "is", "09f8a335d715669b53baf0a41e6d2807d6cfa8405d7fdebed0d57db90402660c561d26873fbb56dff1ebda4a26e573ce6dc6d4a6d0c4dc3bd1354fa0f5e8fbbf" },
                { "it", "16a40a3a2b1f37536d7573710d119c0bb58c9658830efb1bca771895d5f9acc042bb106ccc65b0aae9a3cd4691a2a12abbaa0b62fa3db0cdde47d5f6dcea56eb" },
                { "ja", "e0da603fb27ebc3998bc95775ee0b35f2c1934f8f0d300e9071baefa8be38c6636dcc8ce100be78447e67931f399885fa643275005233ba4896e3689c4a6391d" },
                { "ka", "5c1671aa6c8c17130bab200eed883fdf82d5c4c2d3645a3e3f30e84b385dee9f1680f5ef6f16df47b320bc610639854727c631a38f3e26ff5d56fc3475388e2f" },
                { "kab", "ac32fc56d013d9114c72b9d8a9ce980ac14a89294199c487c6daed993c63331c9783a37d93514ec1d783d8f559743e22e7a22efde08c89d02bf822ae89866796" },
                { "kk", "f2436c642f8937ebf98b2c76aad60cc5ab640c9bef4ceb065d6ae3b421ad505f57e8576c9f08bd8c7d3142e4b84da961d1d5192a743e0267d84fc5e899eab6b9" },
                { "km", "8ddfaf16d7896a0a21e0a0ec29c6d16be0a9db30eaeead68dd15fb8ca2ed796ca244673719820d5bc1cf14ffb31691815b0daf73d5a7dd2babadac2abac50aad" },
                { "kn", "03008641d5c424f26f48e98b8286f29a53305e68976fb25ee64cf57b115ae4ce940807eb8e97539818586a7c89a197b59a40aa5c0f14e2b3ec3b3441a03767e8" },
                { "ko", "92148a8722a908b55f13b950c22bfd0c63e5c18730683fa848d00b647ea4478506502a29b87b0e04a22a2322aaa1dce184307cfd26e766a4ee0c3b838a0f274f" },
                { "lij", "430c8615ccbd1af2055993a65485542dc1a79d8a837ff5e8323af65456c2870c6511cbb87b09acb86048b4d1825e09319a4c6846c034b552fdf52f9bf39a1802" },
                { "lt", "a69dfb3bd3b396b53bd54b237b528c98298716a7a72fa40f8075f2f95e1fdcda30a9731f2ff499b43733b524667798178544a9eba09917762689e803ffedfcaf" },
                { "lv", "3766ab34739c2832c7839431ef2a7434d7627a13ce3bd174fff3e7aed75d75d8d653d5c048580a0a7f07f1d428566bbe2951cc0b13f462502d0d0c9c0c6f90b9" },
                { "mk", "dc405806ec96e9bc6d2c1501d913b51bceef784f74887a5086fd2178f2adcc429eca6b6364824fbdda95fafdb56bd02f50fbb297d412a088383b20072cf64d68" },
                { "mr", "de78033c0e1e43fe6198356a8c93377825b77162f53a29c008c4f8756bf1f5df5308ee77921228a67cd1b0c50d0bac9e730c149cb996184a633b128e7060fc74" },
                { "ms", "aea4068f326df2b1dad60b2c18fb024c2e87524f933f8f050636977b5582c8eb68f076ee341a6c44b4551f8fe988038aa84cdde06a0f2daebc1e03ac6c7d126d" },
                { "my", "b9741a17aa0489ad8f76a6778a962a52bcb383f1a0664e9f8e72b6fb6b80549b12e4554b15241307afc845e5183a0050ab86eab824fc82bd9cf7e4ba5acbe2d7" },
                { "nb-NO", "df786735a4cda164b22057a0ebbf101fdf955ea5d41e123fc3de64fbb28470112b309febe3ae7f7313e22b4ca7dea953118bb858441fb31e0dc189161c53be43" },
                { "ne-NP", "334bf28f4d59fb2f38eaba9e49fe10f3dfda597303548872dd282895e66a4861a31e75dd08e988bf002a1c785cde3614da02046911552f31bf5535e19f58b3d1" },
                { "nl", "1e78e6a84c039c8158a854e36bf4f6d8d066cba0540a1f9c0ff33907e982f59df58416ef416293063cfbed0887243ff64abbde6743b3d4ae99dac3a96553f2ec" },
                { "nn-NO", "c43bc05cd59a6b754d252bdc9efe0208fe40305a77a97797db8e1e774d05988669ac326ad064aabf431b82579eecfd52703e1c42008bb0c2bf4e2057ebb136e7" },
                { "oc", "37cf13d382d3ba4bf9c3da2988990a3fdd66d6c0381f3c56aff1ee32dc2cb852b3ecb001c47a6fe127d1ff7a3f2e6a4494305ef915f412628dbeaa53008e4e12" },
                { "pa-IN", "5f1b620fed36c49ecaed7b4464f6c54536abac413e48763eda7e33b5cc80e9f59c2ac6f0c15415ab949bf6131a9e9dce78fb2132d3224c69218843c7d821e8d5" },
                { "pl", "867e318960b9e15cd637fa92b16f02071fa3e8e9c0a0713bb44ccc73e0be58fc09346f72da56b03745a2c0551c77e7644ddc517b3aa3b7795d65c95478f0954e" },
                { "pt-BR", "f421af53fdba71b61a9b7b963ffc2a589c74e415e7f5b008d9e5d02aacac67371ae6ebb1b866d61a420a32aee38b8ae6908114ae5eda22fcb53b25d76aa875b0" },
                { "pt-PT", "e2122afdf7188a6a7549e9af499cdb575883e9d2f56a844ec99168d9a60c6a6787aeb5bcff71323d2d3d38ab63464510c9a8375c6b356d958c71aa1b504a3714" },
                { "rm", "4548c0f969a9cd1101be1ddebf209acafbe1ada2e3ee4f3f74b08cb1975a689401b6a1347d0d9898d28a76b824bea7eba475c7764539bf99f1e99c8c20984264" },
                { "ro", "ac1f519b4e97e67e8bd52e904e513abad78c02be95d99085d4c530cc7ef370593ebe14f0a730f406f3f4f739907ebe242c9a95d9ce9702eb639cf1957d7d6941" },
                { "ru", "9632c9f74358ca23abffc7fd319ca91747db5939d951694ed00a363be986be80f05092cd72b8c916440234c0bd032c0f935de236872c515b068c8397b207a09a" },
                { "sco", "8cd5c0718c7fd695c66458acec59ad396abfc19c163671128b05e31ef992f4dd86bdfa9c5c81d2a14baf5754f874c0018398b58ce58af61ba9959ed484100563" },
                { "si", "38b409d3672e0d6efcc1d1e0d72812cbe8bc0fea5cd6dd4e4604260d695c8ae08a10d1e0aa762979b01d08e52f7317741698b4b209d89ee29b8960a2b9f3602c" },
                { "sk", "b039c189961ed58127710c60fd0004b3145c804fc5413c34bc7ca653c9328461f6954727faf4af06b3b7a7078c0f2860abe86853aa6028daee98155173ee3d5b" },
                { "sl", "bb72830194d8d6549bae5c6e0efe90251d6703b265db12ebbc188707cc03bca775e87ea579730cd8ebfca0d85af31abf14822387454e9d2fa0c8a5e69ba098c3" },
                { "son", "2927d3256deb84c707e51d1d7f97a2041493bf4e1c45964501367d9aa202c0082e7f5e08e9774403edcc8a6564d88022a0ea15d76bf7002fde928726723bfbd0" },
                { "sq", "48be9a574e59b0167dc3620b9444e9cbdd001d490ce016756acf854358fe119d173665f0334dfda343bf893eee9dce97aab1fe1b6e61982d695a224b646554a4" },
                { "sr", "e162b5a5f9c6e07bc4b5a5593ebef57b6855188455ef2b510bb7c5b320358fc978d4d206c2442ec16572835df2cbc41018680d1de5da001a34058fad882624c4" },
                { "sv-SE", "7580a36bf3ca64d2b619d6d05e14bbcab16465aeb01b99c3946388b209d5041c4b1673665e98c0f916a86248fda7cfa28552dd0056bfaeb9a6a6a1669149ce45" },
                { "szl", "b7f2dd980c3b876d99deb172cbbaa2f4ceb40976cb1f3e06901438538674147458a5663219fcaeec1634e3955f03f5163e9dcc13091d1010b9a20aee7cf8a263" },
                { "ta", "0a30743719770cba730b29a116189758e3c1540c70e2fcf5a06a2803f0d0685b40f4126e2c5c02b79638ed15f66c2685eb8cfad9f5fdcf9b12b7ac7df77c119e" },
                { "te", "4a972e9b3e0a22b68665af3b52be964f64b5938498fbb3696768699e5c93adfbbe89f9904ec414acf1b55479253c4237db30e0f039319bc437795f2fe8cefc28" },
                { "th", "961541762586752876242163b7016ba992beff65b4d12864963740803c1e86b5ed6bc32474cb9c29183de22a9f5fb8ec2de44fd9685e9f9c93a8402023ca1ab5" },
                { "tl", "e73171dcf8e3e3a4161aa38f81b27fef6c9f013d085bc585f4cac7f4064242cbb0623bb1a9e679ac245cec193d380a380103d13e736a9c285e393f57304edc4f" },
                { "tr", "8259f6dedf2850db6622efadc52dd5eedddccb62303f59a53f49489e808affb083fb2a60fe8d235309f280db685364e03e5223b16058ec9cd7bfb499de47c9c0" },
                { "trs", "0b2be9a23b249cc72926d2ee2d049a8c7327eed08a2dfc3827df684e04fae065e3a3c20be741af89eb869f76fb34c85766dc6372232525f80ad9d2696b64351f" },
                { "uk", "4195004347a78f0693b66c3c4565d4671b4761215d7237ba1d0e0f459519d06af75bd03043ca0a74c26e8fd1d678f10e3997bc1a19807645d73d1ab797af12a1" },
                { "ur", "53d56ff06e6f6bfcf3613bd9070ec1a46a85f7e4f00098ddb4ee4f65f8ead42eb1903817255d3cde640253472a676c8177f8b60721bbb4a1bbc1ac4b7b90ac2d" },
                { "uz", "e4301fafb51b463dc8c06b5780a37078be4c6ce1f875ef5e63e947cae24a2fa33b2319fa2f38baea049e3e4851c44f219505ed5e0a14edf63e18fbd325af3dd5" },
                { "vi", "ee8bcacdc0897a42378e8ed6f4e8a2c0024dfd193f120a31675b92781a1d59cb6ececac250d192f53bccb269fc600a66236501b6be794274e38480dd1a1d4382" },
                { "xh", "e77dae5f00d158f27c33df1079010b2fd44eb853f62aa18b6add76bea28ffe234159cf00b1b6830ed78d3c328af980a748f73f7b60b2cd98141e6c77bd3d4b5a" },
                { "zh-CN", "ccd1762d06513cc11b4dcc1ffde9ecdd0cf5376930a219d1634b150135b0ff45bc04c8974f765d029a6d067a11409b7d262c47c553f0ba6618a9e2131bbba8c5" },
                { "zh-TW", "38fe68499302d74a947f453d5f25ca337a80c79ede8fc40b764a2afb21c5c84fb9314fa043d06869837116c235cbd6ea3e90c96853c32c6d89396940b1a1c688" }
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
