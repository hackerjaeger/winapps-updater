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
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "87.0b4";

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
            // https://ftp.mozilla.org/pub/devedition/releases/87.0b4/SHA512SUMS
            return new Dictionary<string, string>(96)
            {
                { "ach", "30417ee8d8d1c93f234ef20beb35cb85d2e09e1768b89d0abf1fc472a57051a3710e7d9d246ee32789ae2990d6b4f087bf755566503887814fb64d5aedc585ec" },
                { "af", "6d2e50f7a9e0299c83be39b098fc7e15849ec790a530c388e5203888269c2adb115db7d6270921b82da75617ea7bde91a044d519afe084dbe7d3a0c8e3aa4975" },
                { "an", "fdeb4943263b5a6ded62e0c42b4c7742b339904da2a756a665d7e8a5e69d3daae03a580b81411baa3c23a0f20b4a125253db8abe706a07cfea9e799b1ef7541f" },
                { "ar", "ea534392667200cf8552ca1e7c2e6bbac96a923af6d257ad2c2ddad4d0293e6d6c9e201edc940b3aa8973121fdb28e9d26d63d0f59dae6f9bc28af424571e19b" },
                { "ast", "fc7640bd91bb21ab9613e1f79e322e17a7c77c5ee3f8cb21d359ccc20e5e85268b13eedc1f775311648172c3b00b68c126512b23a4c9723b1a40ac200c0b98df" },
                { "az", "0444d3a39ab3eb9e784b405ffee346c8e31e0a2cc8ef9badc20742d9255b7b1f5afd2e27a23ccbf7de83ddb2577038666eca6dca3a669e72c66e0bc250e5089f" },
                { "be", "8027a7906969472fe8b4e62fae6689c610b0c0276ab450d8baec79ac037f8735ee756b222ddaf1a68b88a1b4e80cb7e86053b2495776d9b238b98bf31ffee4f3" },
                { "bg", "46866c1817ed517580ee5e4420bde7da1a513c5a21c258dcea963ed536eb5334a6514e26c710b2e2b1c4fda05614ae7f0da668b9c680d63ed8b641b5bb4723b0" },
                { "bn", "58a48323bebf0573606802448916fb5e6cb43bf0ce31f579184dcad5a48339f6b04d894d0ab83a3b1479d80feecb38e21bda641b97524b8f55a90248d2ab5fe9" },
                { "br", "7b7fb6ed81442511232fc65518af223b76fcc8706d032db2848ba1d122f2976134e52ca57316258936bda825f3347f391554b9089ab192ab58a490d6b9d0c804" },
                { "bs", "366a941bbc75509f5037856986fb75d3dcc9f2dd5beb2653254d26cec6a01cc7a752a278ef761df691f277264fde95c32899dd240e9f5376bb764bc0fc062cc9" },
                { "ca", "268f7bc1437342df380cfa7b6fe0011f8b026ec59e2aba3ab3388f9d1630a1ab05269cd680be028bf5afcc60c4d493056907fe94933edf199cf5fa3ec629b4cd" },
                { "cak", "71492cf743b44619409cae2bd3af5509d69e47ed2dca68081b061d420ed9a687b6f688662a66a56ac48c7b72bd76c87f810568d20b889e43de318f961db9f80d" },
                { "cs", "3e40415f7b43220a15971a3118b5f083cc04e17e9a135da805aeb84e0c868e2690722ae2b9df84c14c712b0d053f6e1feefb8863bf91c4b4a62a3ca1aceca4c8" },
                { "cy", "eb35b8bf68dba8dc30dcb5eaf66f1dcf123366b0fdd7100053b66b6c70d408804c876669c067f825992fa405a328f83262a8791cd7ce0ebc947869f823a193d9" },
                { "da", "3481a374b38cde5d4d53f497ea2b5c6dfda4278586dbc457d04d8f4e787051a611026525800e3b472a677904227e3a6522a08d2520c3959ace80e0012de26f00" },
                { "de", "8e9c796f1a1c6c094d4f00e560f347f086eacd61aff2183a882191c4d9e3d97c023626b011af3372ad28754ab41d14a8d7a4234b3c29bf3eb311454fbef174bb" },
                { "dsb", "3f7f19474a433db5da0c54ae70657cf38001e4f5737b94e5e111d2d03eafccb4e44c42a5c54019931be17c8a7a9f10fed69d9999388d61febdcd9720dac6634b" },
                { "el", "56a49d6d5cb49aae06371876e50a41486203660a7d85fee084b09d676c805e909dc14179ff6a20e584096db639cf628308c21b71328da747aa1c430457944ca4" },
                { "en-CA", "5003ccebe305a5fa2bbf15c61a22548ac9ffac359a1f7ff500af3a11fc5851755b4fff20e71405a9439d9249ef18c95b0a62378b06938ab68d54c8ec1ba5058a" },
                { "en-GB", "6102651923441cdeca4b8e6c19b200e91cc00bd756909aa78f335c92254828dd0c0dc85383bd182f7b2b641162604b2bce112e3351ce104ed4696b71d08076ff" },
                { "en-US", "1d2734b9d66595b1e33a96c9f546e029e5d0349b657daaf0e77966eec9b42cc5909dd9ab00fa641a9888a04e43f7c70ad34ea1f7f208804a3a9c1ddf3b040d1f" },
                { "eo", "585bd9f92b9ef7339d38d8fb66989ada14cb14ffbd4d613d5ac59346ebdcfb01bc828067d619a6b058863166b2f28ac5e54866e0151fd083b25bf4f61b754ef8" },
                { "es-AR", "c7aa5b8ba1175fb5cdaee9dc7e7ac76036b03321556b25aab7042087cb236c68ae8d20ee71fcdd397bb7fb55d3b580126445d72a099c29e99e05eb1d5f8aeaea" },
                { "es-CL", "aee2d590e31de4d333ce3c7f2b66a21dbcb3defa0a1f817855a412bd75817b04b8fc735baaf80fe73bdc4a2f5cc02ff3d6836d35ac73d73d59bc5ac08335e8fa" },
                { "es-ES", "73c917110c3a9fe6b6934bb7dfc028296038a54300d4055fbf01a83e725410df9fa6a0778e6642b36539759a4a051af7895f32ae7d087a8c715210b12e02ae08" },
                { "es-MX", "7e3877c0127fb452e36254de8357824c6552dadb8360ea3072d49e3ac954aaf608de6a6ebcace4f03aab0dd14cb3257c33566d7eb848a8e8c7b41d95f13d3833" },
                { "et", "0dd8d1ea78d9c3122389489ea8e8602e6e666537bbe9715e894dfee199de0b76b8a031e7abd861a143561a0964ea4affb7142589b346b3f9d0a0d961c313e1b5" },
                { "eu", "de35fe59ceedda5e044c202d70ba72a1deedc3febaf5373e75b087d063a1e18d9d9bbca22c3d0d5a78d1954308ef5cb92a930ddd3adfcebcad172c5b2d8d6457" },
                { "fa", "9a15c6cd11a973032dba6caf541709ab485f9dbf55df20e1e3866e324532f255af6b478913d80b7ffca8c33994dcd74a554d57ed2886168e0932cfa3e1bc475b" },
                { "ff", "2fe55fb4c21cba6661e6afff402226531f9da271ab501220076ace6a3163237feccbc4e09d5d5322ab26604ab100fc44359e8f261fa0a1fed1767739bc813b77" },
                { "fi", "ed706d9e781ee767e7e7afd86361e7864bdfa79f3367cd0f622eb6528131ce96eaae99870432374916ddc9e4e6c39559bfbb451f8aeb3b5972093ca2c8d80d76" },
                { "fr", "f343fde1fc8ad06fc49381223d672c1f6df50667540441b8fceedddec5a64601690e6419a6bd04a66a117aa5b313213c8c6b6b1c0c7eb9614a9be2465e1b0eb3" },
                { "fy-NL", "702e7c12eeb37113e7b085636a85338908ab0690a504d432b605fea1ea615af8c07033245a0050aa98fc708ff0687b2116ce20d2fc12f27cfb741f4d173ed497" },
                { "ga-IE", "81fd320207b89aa4ad187b338888f2fcd3f51a875afdd2e707c265c12c94982fdb428df185cd58cda3e61eee5fb856824187773de7b4b2900f56b402d504c06a" },
                { "gd", "0b9aaec0ea0e2c4460c0b77beb55f7b9837232e2238a8f3c3d9829d00dd11cfcb08ef1dcf38e69180821c2965ed2aefe60e937e979c0958b20ececdfb3244d3d" },
                { "gl", "789187762b4bf7ebd157736f751d461ecba06f0866e7e3c7738e03e5d3e006938650c883737c1bf1af5053e9cb11bc8a84fadff76d24361860fdc12cd862a93a" },
                { "gn", "acf9e49982307cfe597bd7c7f08556b485792dd1a35efd4e93b0c73f6612612dfe2c1eb9dbf6e41e2d109493b308e1e003b06fbf27d3a8156b0e0c389ae7d738" },
                { "gu-IN", "aab3030710aca60091c8b37f2d0ed1cc1376df37566b2b61f89eb02df4dbcbaed61c54fd40d4f230728a8695ff120dc9f6e07ed39a031441e01656e08105ed40" },
                { "he", "df6906f6e3df5f2410e94078b08732a9ff0d63d26a946612d94228ea5c7140b0f963a7c4489d5789e340eaad7fff907d8ca931b0fac0a686ddca29ef7965e89f" },
                { "hi-IN", "ced608549c887dd3392ced98a9f3f368e1c77612da0fd04f75de3c71c24d47d7fb3556b5f02c3dd3f5dc0f339ea80a0c87711eeacb97800f813deacbe2635663" },
                { "hr", "75bf4501a4ec9b4a59a1cad7927aabc4e44e92ce258c52d8dc41b8438f97ccd5ab5a4fd7eb1ec25eed4e829ad12fea69910afe8844413eeb681d568261bc6717" },
                { "hsb", "431f965b6cd537aceb5b6d17802feab6e37050144d84f794291a07c190300fad1eca0c363e053fb8fb8adbf61bc16770b49d563b8e12a91e9e6168968f3066c6" },
                { "hu", "deb39815b71a96e4a8c7ebc28eb798b5aadee40e1ed76d0a75f8306332de660262f55ea22777f214729c2cdf1d2b8ff9fb34ca8e5031d9acf8440c758e8ca61b" },
                { "hy-AM", "ba7a8fcb912d36d6f7b3c1ccad37e605f1478d248bab9e82b127101604553d280fbca04bb9106c2b6cd6e7a67af2f7860ff2e804f415e491d2852f4947b44b6d" },
                { "ia", "8003636416c0652832db8ed419778c81c154d296c1655f68699a1e2248e60a612ed212785c363e0c80006de8c8f33de02d2b719e074aede0a455554d36c77140" },
                { "id", "813574238a2d109161e9b68254ead1d1d59524aa6a54465963f8ee6b177d467361557364e2e9f9ab1e32e80ee7e729e4f7fa1439c4e16e3a4beb87d4335c38e8" },
                { "is", "c3bf6a0f8168a486e54c4048e3e7aa5ad058c3abb18b4598a39eebc5f69489d0a83594e6ac72820e07e1620625439cf08f6ad639f6f281aaef6b1f0732bb9b9c" },
                { "it", "6c9d182992cc6d6241568876d8e2baccb44d293041e397380135a94ccc0817666abaf5607ae14987ead8446674baf73b5b18a774f3f3a9a70ae7c82678f71f20" },
                { "ja", "26f1899f25c4cf75beb62fc1cd43ef44ccb1d3caa5d8dc1063cbece906b97917885a600a48edb4a236b078bcc2c1541e68afc13a72a845ed4f73eba46b3d6819" },
                { "ka", "7229f9fb1b070224c71e10ad4a55d069c86ed6de26c85f170bf335cf65ff8e57f275c3c1cc0cd7ab928959cf989950a44614806ef6df7d0490a69af3bf88b713" },
                { "kab", "517205b744b75a7c0b6ca310bfa22511cfef53d6eb43999f3142a3d07721e3c0c417b695c166baa7d0eed11ed1696cc991c753310ccfc59e171aa406b3542ad4" },
                { "kk", "a0928f6a0f25a1e81ef36c450cc1806e4bf5867ce5d9c46e5da3fbb7dc86a172e636654602e3bf5dd79cd1174567ccb2f96bdf9e5e0ffba7236c7e7b34db4a22" },
                { "km", "46ab560517cadca9f0d6d5d77d72c67117f1e632d28a14c8dcabac99f573034ebd658300a4049a06141f46d70dfd3023ecb498e8257763744260eb5f9906a9e5" },
                { "kn", "e5ffd7a494e71c59d047e90a0cdee12b7637c22f37e3ce8cf30aac4f9ef7c3fdaa90ef458375f676e272b44e1d92dcbcdbb79972dd27d063c33e17d27491510d" },
                { "ko", "bd57b01c42975b6eae3f6665b018165c23a095d23309205da707c5e906b85760d70ce34c942f728cd46e57c77f88c8b2dc2b7c6f2dd0230fc8b36acb3620c447" },
                { "lij", "89e8bae344828a7d2833d424965d0bdfe9b1a36306f301c26196b319fd42f02f4e12ccd9687ee09cc4f42e88ad19cac6d956f9a566d1fee406d7e46a0ed65977" },
                { "lt", "c508c92edacb1f1abf53a5e668bb4514824f257131d99e5fe4aba138e09a0fd83997ed82bc7aa21c596d67df93031c17f9fc944c263df9f6c50043f355213b99" },
                { "lv", "d7b1e14eac10f9df8da257e488e800b3f53d863560e7b6719f0f3703396916ada3cbca2fa1ebb055cf90ce2ac52135ea60c515728a7eecc5b42eafb460f7332e" },
                { "mk", "80db0caf6185429e0e1164ac659f6299856e44e50fd10f93c4b9440db31377debf3c3dc96ae9bbed1c1a2985757e115e74ba4c95a13c021c9ed3ef9dffbf58f4" },
                { "mr", "ef901673f4d4c38db1ccde8fc6d259d415f175647eacfef6e5767c3dcb1cf073e489fad4f19de7124a964b7828862683f1d064d688214ef652e509596a3fa747" },
                { "ms", "5c0414b57593644935313eaf2324d63b1b3daf9a2f3d8d8759343332c36f6c65d81ac51e397f2e73f83400bb9a76c7ec262df06356526ed82dd648770e43894b" },
                { "my", "d73b07b57ed5134a42d077857f8f365705ec0533c87614c2f377b09bc506aacb4acaa2e849246f00cc0a8bdc57c8e483d9ea0524afbbe56f6bb029d9ab6439e7" },
                { "nb-NO", "03f01d6113bfea937998df9c8115508c4c2337c5d04916f0b56228351ceece5450e028ea4ebf353e49094b71a8d9804e2535823bbb8b3a3c3ea7cdba2fc8b01c" },
                { "ne-NP", "9db63b365a8a8c6335d66af414562026d24183f9ac61da715e9bf3914c00a992064f79ff01536c9f6033faee7c4aeb8909e392be2bd99411e247009f32fa8ff6" },
                { "nl", "cf9c974bb26831a28c14e6669fa7dd04508019e5dd2e184a4160c19d41fc15a8600554039aa4b23830a3dbb1e0d09352e98f548136aaf4752347e3ac902e180f" },
                { "nn-NO", "8e04515352f0da78e46ef66e7bdef2f854bb95bf7a86890e79c3a89af91c66d83eaa3e77801979e3fbe3ccd8a70e875e98c5ccf3c506000b9a9956dc719dd87f" },
                { "oc", "1132aee254d87fa556ac147539d125ad4a5fb784e4413131bd40142c9f6a4b55559eed7acaa5e60c339bd25d937ad8ba431d050b79cb7e797efe333bf35c6b0c" },
                { "pa-IN", "e4f80dbacc8ba0574849759cc1646bd2c99919ec0db21606044d2617493eed0b2332ce3ceb301831ddc942940d6928bd679137bd7d8503bec1fdc1028ad60149" },
                { "pl", "2d9167d5b562b0d0eaf7e73bdb809793bdd1243f3701a498d078b6f18a95b4adcc32f8a55ffa6949d3118e3f190d21fff1085220a0e009f10108c4608f0f9da8" },
                { "pt-BR", "60794d0050b7fb9b0a68415c27283bfc48dac2979997373ca79b1601b979dd7c9cc59b6706a75cdf248711c15f1726133cb120cc45128ab916e3094342b35e89" },
                { "pt-PT", "a0886459042abd89b398e04f84173bce006a5246f78b76d07db71c24b92cc76e51d2a86fe44576bcbc739ab359b134e88fef8e7831ebf56f088b8adc9d368296" },
                { "rm", "db9748a013a0d94180907b34df7b68619cac66ae691a29edd8c5bce29a6b429e44219256d8962161240bb9e3024091b28ba4439ee5fe63e42378f2b3931b6d09" },
                { "ro", "6af74249fe9f808846bf3e7a109ec61433fd5668a1c5e3a812059640e23318ddeb5fc6d6e8ebca28a0248f112548d2bfae01b1c02be0f47e50e56774cb42662d" },
                { "ru", "e53610cda5165c6e1a9b73703f6920a15531ebf9285ca6dd7e721f1426dd43df3bbc5031f85f90c9b60217eb0e4ee70f3fba6d84ac5c199b834336a8925c1f40" },
                { "si", "d5c010e59a8d3a9d1510fa785155b1c83eaf34b9a0c4acf622d174a722a78bd51245d70a064ad379ba2e18871cdaf418643c8e5b517efca699cc3f0129b58328" },
                { "sk", "f1b48fb1ca140e98256cadc4b7c2447f28b72079834c6b3288e67cb3c6e87acb57c0fb225424c55cb2b323c989b17dfe0d0061cdae4cb41083914f87774da626" },
                { "sl", "05a61a65c5ca031c3199fe949784fc90673eeb91ec7d025f1465a55a6ad85481283a6f48e231cb26b576c06bae82b2dd5d3e9d39e65cd9ed22eb6aba79a731d4" },
                { "son", "1ea19ac0dbff48168b86cc84cb102ef2839bb7d9cf11f262d58549d9feb4fa7c4f8fa079538a52eb6fbd900baa5ffff9aaf33d927ff56d1760022b11ee41ab41" },
                { "sq", "4bf3eebad6a2d20a366e957829de5c5ebec89a0bc2105cf2bf4f5e3a13bd7bfca21276edd09332567a0db44fa7a3ec988e79d8bead4db10970835c39896d3db9" },
                { "sr", "6712c64a91e8aa70c6c5a262f56b31719b498115b913fd338e6a967f9804c5f16bb7de76bbbd300a49b529318b9ba27dda399b84b321e847246ebac8b221d4e2" },
                { "sv-SE", "74597affa5bb0f060f5bff391173c6ba4c277cf90f7ae05bdf788c2dcdd35624d77f7fc67a689bcb223fd756ecb07ce95534855d712081c8ea4d32d95ce81db2" },
                { "szl", "991dfdcd8e2d87475fe91a392f8bfd52170fca91976e23eb235d617ecb6c7240f2e3bca3d1ddb9ec5a37cc2dd45583ec0ae708ef5560c562230df213fa90a4a9" },
                { "ta", "0ff989449c70082ec9cbed77fb68eeceead28085739a1cdabd302c009afbeb2fa3cee21e66274af26c81017cab7f27e83012fb407c9202fc4b4e366738a2aa3d" },
                { "te", "b8a276e5a311fc5a1675afd5924cbb1fba54b97f1875b11b5657339693cf67161b1b1e45a9193bb8e2ca18df966793fb280c3cdd3cdffd8070565b98152731df" },
                { "th", "c6640ea397f2ad5322dcc2de31677914836cceda4fff43c761bb878d48fbd1983790f26afe6997610715785c7e1ac2644c8ef36a7544b42c010fbcbf5941c018" },
                { "tl", "7ea7ebbf3d3c720e42c2b4ee60f14d900455bb0835afbae2ca91bb4e94742c1ae72c5d6126a6032b0445c562c89e5a14859338d315df407520c4a9c981c1d404" },
                { "tr", "6823b5424f285e315a58ec11699bc8b491cbdc45b86e63ed69673ee4c34747040d73706f5b9b10cd4fdcf4b3b550bd9790796b17059f6fbaf101a7b273c883fb" },
                { "trs", "eec2d0b049d5ac8a490fb650235a8c645adc6e8b05a9424336cd3ed0f6540e635309aba9fa1c906d6c8eb424b40c658b93ffef708db44d24ad35b49444ef9b10" },
                { "uk", "d557cf99182df0be989a8296795725a34c1325e5639105be84b3c4848cfe1b58730f43c3063aec8acd2c7a3af28062ef804c7ead19786b3c750dc43155c2eb02" },
                { "ur", "de064b39d98e5eb02939fdb368ebc99a8c6ba7eeba7622490dfcf0dc1d8aa26bca5417401bf2829bd98f6fbbacf9daff27de8cf1a5334beb7ed3cd1e602adce6" },
                { "uz", "d9956d5ecf7f387fc2fa1d5b532ccb6345b32e0f4b5a4d6fadc2109ce46dd99db2a846b54975a8a6bb8acb31e18218148a62d1ab2aaf7851fc670869b4c9839a" },
                { "vi", "cb82d5366fc9a7f39eb46a45db8be3bdff08e6dd5a5a09e1046a8e02203425509e106cc06824624711ba11e4a575d8677ed8ee8d03e9b58480a2b7b5fb802edb" },
                { "xh", "266e71899f4956a0afcbdc01b07826b4b65146e3d9ca232b6e63df5634c6ca826044a9bb44f4acfcd04cf41407ab90abe180d6a859be2ca5a1ee91674f381049" },
                { "zh-CN", "c00cfb8ecbaecd3afc20e27ee7271c121879fd0fa3ed733457d7534a1dd6006f98761d5443995615eddd00a0bedfcf99e048d43b64b8d6bad7c5078a6906f49a" },
                { "zh-TW", "f9276426f5a87dd06c9547a86db5bd9a3416cbc8f7e48a5e5391b349425c920912df7d5e9b3ec4566278869c119ad6ec9dd7641e57576a53c690b16e550c9aa5" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/87.0b4/SHA512SUMS
            return new Dictionary<string, string>(96)
            {
                { "ach", "ec565bf0e0191abfd8c07d6b714b9abf9aa6de06f97d008b414c90587ac8e4cd672e6e586772bef508c61c4174b935628c962ffe269273f31c52c1f98a3efd3f" },
                { "af", "5ba266c93b9077310eaaae1c3e28bdae3b111467f3ed17aba367c4ef71b9225fe5b0dae22578f577583422fba013170e6e3ea67420d302106efd5ae8fff3d031" },
                { "an", "78e86193231f06caa0eddd9e4932c754849f3c3b386a4c268dbe8b300d7307a9f9e924e4bf8f9a3738e74b972b76a6023899ac410b9c305730b86d0b9117b730" },
                { "ar", "acf9387ef23d8b2f89b3ddbe96cca5c463fe98a78f734b55ef4b5e3a347d8146bab41f726ebee8893de2f76a126933fa8c542b7345861983eef29b5b69480752" },
                { "ast", "285d3ca2cfcda24d075ee3dbee171fb6d4acfdb052f6f21f07650ffe71028def14fb02880644cea388ebe3b9fc884b9b830d029cbe5d6314abffb7a4dd839d75" },
                { "az", "a171371e9a1aa1f099d1ffbc99151de9f994f489882db95e720753b8a44ef4cef2405f8bc8a76544353149419cd59c3900bcb074315b321d9ba24015dee81c7a" },
                { "be", "8c2bdd129cac3dd2315932279c5dc34caff13a52ea06a8f9f2422cdf9d7d99f36454c6e842a85f24bc8c3ded437747585b70b786f03edfa96ec649208f1d60d0" },
                { "bg", "f66ce3f8d0eb6dee67bf7f3423905559bbb4de7d7a93834351c4bdf01d32f4ffd640e8158bdc11dec3e77074a7f3c3ea9c4c8e93acbe8633e127bef86bf205fc" },
                { "bn", "2d529168e3dd0d19603af07841c7d3353822bae7cefd9dd949d0bc39dab01c8b609389795d005a868e29687bf5f4b1628ce85d24a5f1b1752c4f6b8f4a832499" },
                { "br", "841ea8392468cd88c4505c0562289c612e72959e2d84a9db094ecf7b01fda24e864034a0e62b713a8bb2d7594adde426a37ef6568b420d286ea9b4420cb1ea14" },
                { "bs", "3f3488b31726003c942e342c8746aceb9737624a7d61d4862575aa4a10fd44ec8790edb86db3c9eaa5d09c2c530384b858911fbd31e3992e895ab55340d7f368" },
                { "ca", "d21c631df0d6ffbfd4a8da8c813ec16312682ebd699df76c80a894097910ef801692c8798592bc61a1941326b363616b7ee3ed3ae0338197b18a91bc26d0552e" },
                { "cak", "4df12c2486c88f3252856a136ac6c3d498de49f13c41616096e729c13618e6a9aa7109710455b68b1c5c7faaba6cabfc5660a4a987449b9f5eb29ee5abde96d5" },
                { "cs", "578e7e202ac8edee6e1fc68d609ac08ed3385fa678c2f1abf20a06209919ebb7f292014aa4de5ed76cc773e45aa9fb1bb864f8336ddfc9cc622da2c6fafde808" },
                { "cy", "a0279d2573dfb3b4f5ee77b60c9856fd9e1f5889fcb8909b54d34c1753c22021d2b0a3026af88984df307e61da1214b245b5329c8a62f151479a6add5d153585" },
                { "da", "a676a8af8946f692ef27ddf35c78e0b339c1bbab39f1179148a7c95bbec1a72272f14cc5435a6413fc33e11142356fb931a3f2dd8284ece012c3a86e63802b02" },
                { "de", "fa542d36c5220dbb623be16b7ebfb2198ba45c14390fc8896a729a4058013005230d2ba03240719b58888676bb91039cc1f9448532cfd2abe3df2c5e07b8a5a0" },
                { "dsb", "089aa1920bfcb386d8c3d0f61a655181019e234a11b6f232334f51bc3472505ae57455100295ae29766f05d9b78a20547bb73f23d14f5e1aae2b66f6a7530915" },
                { "el", "f545127343090a09de180196e80b4cad0b9a1c78bb9dd365cc43224d6dc0d9c69ca09a0cf0ca96e5095c8e81cdd9ee2996b4389e0e34c3a66635bfe167a7f383" },
                { "en-CA", "00eceac536d198be2a7611f456404c84e29255999a1566952629e293c2380e432b9af9ae43c7f2fc85bb2f1c61bbc6add0758e85480497f29c180c92e7836763" },
                { "en-GB", "57317a54487e5aa497fc10f632c3b9af51bbe11b9745e794d0f7a970050d049ba7ce4e3bcb5e27c9dc272f86ce8fd2d4f72a6989c50395f1e62e89e08f41a81e" },
                { "en-US", "cb18c16d8bc73c71f4331dbec6b45701923fa9712f3e2cab7cdc994039cc8c00b93550a601b5ef34a102cd3e171ce29235940b48fe6c8b6e6542ba0bedb904e6" },
                { "eo", "2e13fca286e9faed8ce87de5884e74f9b803cc6640465da3afe4c380fda93362dcfcb84be47d460a615d0cd0f0024ffa4f03bfbef9c23c3ac439795b2a475d38" },
                { "es-AR", "f952060f466a5300969e1c6fa52b8bf7ccbdf4b7bb793afdf9ee54dc6a1e32366db81a1f0602497dd054ee46792ab7d9dbfc54f49b1d9747d2a3fbefda37fb0e" },
                { "es-CL", "b1f0918da334fd36b3ede8a394d6234e9e13c3db4db23b11d639d913e6e5bc082d016b292551fade0ff1364db062f428ee59b95850a2f823eb3551572c530fca" },
                { "es-ES", "3e14f933d7c1c0d89ae1ac41d3610285d574b58fa59f463bca9dd7eb7309151ab9c7873c5d640e28440f088c03286c298fbeeac73ba2490001594367b325a955" },
                { "es-MX", "dd15e9e709543f18e0d5ede0e6ce826182943998d5ca267fab23cecc887f90376d91d2259a572dda7673deb445644c36340a68d2a5e3b9d3f4539a71915a79a0" },
                { "et", "a4b41f8ce5bb798c4ad382db227f510b3e2c159c774c323386020fdf645991b48ef09179ed4aadf9289651f547522f0cdc280b13832533553b55ce61b61942ef" },
                { "eu", "8eea1110537cde715c18c84544524e5a981a6b3fc11e73bcfb2cd2c35c7b1a05b4472244cc46ff59c7037f642d7b7448fdde740a912dbddeb62591474c02c231" },
                { "fa", "026eb392086558a673cfcbb21eb75dcd8fb034a921ac10ab7c9e15161af164881c83546f7bd5adde6be9fa339f2886aebe2a2e27170a0a185cf2ad79ac21dcfc" },
                { "ff", "81a5017db66737636233d268011b2f21e8b7d32c2fa51492fb3a2e8f35830c45579c63987e42401de132a6d8a07b68c7f21c2dc50cd505794ee2dcbfd8b2eb52" },
                { "fi", "e749b19ed6c994fea001c215b9ce5420abda8aa41322f7d3c08f14737dcf4342eb53d494bb04912d1b199ada8e32fc5a4b1f27456c044673367b496ca11ff08b" },
                { "fr", "bf7a035aac72053e46fac9f42cb66639cf864df396c063ce6682648c92170c01bb39e970b82d24c59997c64792f9017424a3a6602d78a3e03d457e713b66a8c7" },
                { "fy-NL", "7db63e10e5834596f0116b2de50a5aa232d089ba60b195bc923fbaf32f4adb200e1c87b3783e469a5697927213fa2ee786f12b1c5242b5701266a80c134dcb22" },
                { "ga-IE", "63e5160278f300237c2e6170815e4f4301d0d38b6fee139c1f62621b7a061b968bd2ae940b6971cd1b3fc11f49bbdef5b37fa3661a7e8bbeb732f8aa1ffe53d3" },
                { "gd", "134f4f2eeaf6cb2e97059d55dc8229d9ecde2de0cff9c14de851803e96e9aac29e33448ac298b39c27607fde60c94ed907642f8b6ef4adca8cb8e8e9a52b4da6" },
                { "gl", "4f58c0a2c78c3cdddbe52c6d7ab4610e63634c9f1c3c39bb1f65fdd3896ffa32845ee6acfaec02d321006af8e868df3827ac50e0dd71f470fa1f3746e3944058" },
                { "gn", "1b80bc238991ca1cee9c0fa0d04bfa6175091c7a4971c57d31e14a6b359c79c335db06126c324f73a6c39ca2b39d35d9e68aeabdec1b81d368ed00704bf5b337" },
                { "gu-IN", "8dfffedf60605443fe73a01fb4f089105f8763be653a9657e6d716afb0aaa503262005965253e9b26baca1feecb49849473c877f91e84d27426fa26f1fbb5e40" },
                { "he", "1266c06d2748352bb85142a010bc1c8c02fced200fcf13f3989c2c167cb12de8d3b905c78f33735e48c61b988eab364c4375ed708b687d4783d126565935c96d" },
                { "hi-IN", "b58f02fa37e88c7c9a7159e91ac0848c20bdbe6ad58b742dc9ff85831c7ee80350f3f7bb9b56983e18b7b7b7dc76449d1588f028098c1c0589eff1d9f89308d9" },
                { "hr", "236af1604ef5d2ef6e281a90fdb9876f293225c50212a97fbe071bd6e07e0142089669fad0fecf5daf3bfb3cea95f0314ad06430c40bc6a1d29523f2149b51bf" },
                { "hsb", "0874ad68e681f33dfb15aaf209671cb50446412c0874cd7ecf1f0a03bc84edfef51245fef8cc659a7e48f86a7533c879624b884066787b7a082bf101bf18ff78" },
                { "hu", "8164490d7a0709060a0f588eb7632c9bf950d1491a4e53a0beae66b4bc3f8d549b41d27d287e749e192db53d2879b08c71e1c0be9616b99be60c2c5687ccc6b7" },
                { "hy-AM", "474670f7f47dbd15d5cf7bee95052d11791936bf99f8e33131bc5e5f4f3329a6cb38747c46d7462961dba6f82a1c4265bbe65dcf0d383a59e6a326b65547de69" },
                { "ia", "44a8e0d2bba6243dd12ffe9505b03893ddd2673a326a838dffcee30da35a2e15849104ec2c01fa67f5e56e782ddd907e0f800f3fbda10e6286b12b290409b838" },
                { "id", "e9b2439757d6d5c081701afed2956bf90e6e50101aab8d5b69a9df71921aa2d8ac562afc37a3ebda1bc612c13d61af44948afac392a7c1e2dd6648d688703811" },
                { "is", "4ba26e02b1a67c1518fda91d604b2a423d71d68f088f0059c14d3866c970170514aac9101f4eff309b3bf4e2f25f827f0bd4be9d9ae1dc062d626ba4ca222ed3" },
                { "it", "c6c1df2057b32694795cc8b298ad2ac49efbd6094a791969b94c4e19fd3609fa5b0c5192888615b2af256ec6fa1bd22751952a12602de12f4b4ac4e29b11b346" },
                { "ja", "67724f3b47c7484c3ffd76c18a9e6b3bc41b8ddbcd4469c754bcfaa6cc0f54d663e8fcbd326ba92264b96307754d980b368ee46722606ef28e65c97a05500e5a" },
                { "ka", "9602440022c030c70526bf81209e6a6b33c064853ce9ebab44e20c2b20a8e7a82633a4ec4e9986f7f91c2ad365d7471968a6b705b27e91403e8c3f50af83a98c" },
                { "kab", "faaa8cf8eb9e39351a44061b2fae4ba5db51b5af3280fe14747471becf08340abea3601e86c88e99f9ff2083c87e09ab6dc07a0d01c59f21790ddfbfcf3d6786" },
                { "kk", "43bf24cb1f26633c5fcbe5d285b6491d62860c9366bcfdd9c025bd42d83d00e62824359804f11006f65ee8d97edc5bee6803ebbab06b6d8fabd81c095362ab94" },
                { "km", "f21b6c3a3c3583bcb2f8d4e759e4312ceade5acd6e0a140f5216bcd4cf23c9b06272b9da6992581f74de9db1246c1ca1ca8231828b8b6b94d92e25b304e22dcf" },
                { "kn", "e39f8aa082c424eb1160ed415a47444cd8fbbfbd0dc45f4ce2be0869b491c63f6cf3c345e92c65671a662b27756079bd7008be9be71ba96485b6cfed702a531a" },
                { "ko", "0b10c953d748df76d71c3a75736d3da0e8a302e6940bd20886547d2132d58a5f8233f4c248d9d096825b1aeb75f7d9ebfbf49304c7837a5e870d5017e7ea4904" },
                { "lij", "c6ca4887778e2d041f10c2f8100fa753e8821dba22890600b852a2d2c47cf6a240e12e0a3bac210e20d6dde75f90e9f4d94dd917e33f88581f4c582c137b424e" },
                { "lt", "8a9c6803495cc193a2800cbf624e3946980cde73bc177fd02b0a9a6040822144cc782c893701fce040c454c81df3c6973556db14303845e63566ff0e8c00ebd3" },
                { "lv", "8f9ab8367c5249ff2d2c27912422aec8ded21776c6ec1ed3ce7f96d6bea177d33f4aedd039cefcd4b44be7fef099d18543e308a2abf02d4e939a0ba5097e6840" },
                { "mk", "14599ab6ab317dd1d09ff6900664cede0645b29109065ef22e2a59d5aa9785d8afce3c43d41086c00a950ba6ff69b677a7e5cef5577ce2030e893530ac162e51" },
                { "mr", "5a2c04415c24fa97547a4451da400eb5d219db08f08ecbc5540ef84b65c6ad2141c3855d6ceea2943a0348a893ad995f31cee50eb631a473b2a1ff851abea8ab" },
                { "ms", "8c2fc8c9ae64d15be8e7286ce2d840d46bbd005037a7e06bdca74409610df39d4017778f72120942aa0548a0afc803ca159c4f760b71c0ae67848c3b0e2769d2" },
                { "my", "1314e1c2855987e24f6cb09e241bc22f1fa7c57043ebfa18440c8e3e3292609d7ee7678d5a8a844d5b53878ec096dd3a7cacf6222cdb5670f290f0095996cf24" },
                { "nb-NO", "38c63fcb6e505e52fb8f2c0afeddee7ae8cea755ad61251929896e7359126fe89c20ecd257b4e38192a1c238a0c328d13679b83b5f796a7ab559ea6a6ea98c76" },
                { "ne-NP", "f6b17e0eb0feee37bc29dbc87c50d98c4ae9a5739ba862731d5bda101a254723501c106d0110b8d3894a3b55aa5541ac3d89325622a355cf531e4dbd34c17ad5" },
                { "nl", "ffbcf833f1087f47f4aee8f2a76932e4bc1961d31b4dd5550d2949da8e6989bbb5bfaf11abfa7afc7a7c9b3c61648b5cc7ad789602329b7f57adade301811427" },
                { "nn-NO", "5053f2a27267c6f8687feff30a8a7cd9713455ef5c6bdc22c8f547aefb34d6d1f7bb222cca0c90313ccab22f41d8f2ff98a5096634d1390a7762df6b24c929b5" },
                { "oc", "d34eea28eb29805ab748974660f6c17ba72760cd10da7112ac5bb4882114df107d0f1568fbaec95193534df9692626126acb9953229b9c0e3716da7a125b6f9a" },
                { "pa-IN", "2a7c5bbb498005e8397ff1348e6fa61a46fcdbe7eba05e9530bb61ac7af3aaa22fc5b026fee1ce4179b1ab4f08256f4e52948f1ddc84f937897464ad0e5876ba" },
                { "pl", "fe2da8213e616f186480006158abd21fe7987d478de38f6873e7b575a97b199b03add3d889561486d5fe65aa9ab5899ec3be33ce8049346f705a667c022af9fe" },
                { "pt-BR", "bff4b749dc6a9f076f0d22b651fc04c572d55df5206379fa9b235aebbbdbbc39b28cf92925a62549262e4f62c6f5a4fa8778b53178a20981fdb92f96342dd65b" },
                { "pt-PT", "9eff35cea94081e432e8a55bcf5cfbf2eaab0ae0044db51189dd2f2a4cff3df4e76a9b6252b1f48fa438caef3c8222dcf851e982288139b3ec9975a8102f03bb" },
                { "rm", "85dec26159e0abd9184fea49d9541bed78fea7f79b67afee9a3b8607131dc19a3f29f392669f8fe71576f468ae307a8ad211966a0f2774ccb0b03025c2df31cf" },
                { "ro", "6030df95a9516325ee2c5c4b23b288c0243653cfbc86c54592fae3f232c3210a50980b718710b1aa01541af73158f6683258841a07dd90382850fa72e33f553b" },
                { "ru", "709c1d721b38c3e76a25ad3f5465b4259ee77ea93c47c5ee50eaafb8d86a2cbf23580dbb223a4daf1d3872b80541e896023c95e24f4e8f04a21e39641d209678" },
                { "si", "51d14eb70927ce0516ad41cf473817ed9a8e41a128de814795b5b4a966e751bd869e18e538e04dbb4c806d8f808e87fe9c0981d12f8efcc362963e14eb3feae6" },
                { "sk", "92bfa1e50d4c3789d71eff11f024c788ca3b8129d0d23328309003dc1160144dc91d3200e3fa284b0257dd003429c7b0f4455dfc395b6c4790f68c2939769ed1" },
                { "sl", "2b83b19570d0757c674b7e700e393d135fb2d3166d663f3b6ac7222878d9d3217184934fa5b021edd5a8976dfb96b44b1363a491828fceade6944057c0086aab" },
                { "son", "d2351e574eb39e5b34ea85744824b8e0cce4a88153aeaf020420fd900bd61dfc65d6ce2a9dc4e5a352bac6db16dcf9b98fae7096d64e1a549ce3fca6c682665a" },
                { "sq", "d9f9f556245833ca1c815b8b418288103090d91556dfed3299f645e90ebe29f98b8c230e5f1162830b36bed92759dc91679b74edd9df1ad75b5eb4eec77e595e" },
                { "sr", "f42a261e328ff4065fc52e1dd9b6e0382b873f635e6b904e17cad85dcfaa99a6ab180d5dea7fce89661de51ccab8c15f6bceb4fe2f1551214d1bf3d90e47ed03" },
                { "sv-SE", "c3d08ed1859ae195a419df7ba592ec724cb7e739823ec8cc225b1244e6a3db0b5a69ddf1be8a25f4897b747e84a5f741021f9422a948915885ca1621ad527526" },
                { "szl", "4985de0a2db80de5ef4ffd75df7b883c6ee2dd69dc97f4d9fa59bed2ea83539fb96b9c1922d88cbe1b1324d02367c104a0b8af8569037c1fbcbb3b4ac17b1f45" },
                { "ta", "c490306fa1c833031ecb61c56cacea3e9fab09e7c03aed45cecab33e0a08a29607439a2a4a8371bcfb8e04e68f9b6ec60e306f1bfa2377fc802af232b1b8d778" },
                { "te", "7b69fcdc45ab49fd65603ab48fb20ac82787646262b59b5dfe99ff7f38cc36ae1eef7da8ad4eaac4866daac8e4d824f3c91c1ad3d76a81bc97ea7753ad45a9e3" },
                { "th", "e0354fe52328da877bea377aca27dd2907169be66e77546d00bedb30713fe9739e9c4746e61a2d456b127e376606b945244aebe021517fd1927adbda1b77401e" },
                { "tl", "1ac8df837202095a46201d5e77f2cc2e024d66e973056403e415014ecda7f656956d448d3474a5ab2ba83e426d6edffd845741bad064ef6866276f54666b6a7e" },
                { "tr", "29442c433cd2591e0bfba27c46fb101d8f32c6a22f2d47689e7a99de7e948a1272241f141a7e001bd38ddfd5a2b4ab837c692b1666dede299bf6f6fa7851f3f7" },
                { "trs", "0d489e88304ac908abcf2ced3f126f9fecfbf29f03c8018fd9388370f3be7c4ba0738eff4ef335994179c28a5725215e7d3ac773627b1505af24a544bf66f6c1" },
                { "uk", "f9e9bd07529939bedb549f541176d60e20ee927fabff9f22d0b56a3450485e93202c8978881eedb6552a7e4e933e9998ac7cb07dd0d2226373fc9d574f8c6e49" },
                { "ur", "d2d10b283ca4c306ae628cfdaf9983b4c7f6b3b3d02c09dcbb5b172f65eb715238baf650b6d92968e5b7a0417965f298645ecca0c987d76c91259f4429de89aa" },
                { "uz", "b36fdde0ef8334ecb5f9e9efb27f8f502c36bac9477ee84151b81a5e483e1869d23a8cd207441f407004b64d034b732ba2b8b6ee909e7012790f606960490341" },
                { "vi", "ca205e48099f3efc9bdb1c11db4d605ab6073e15d705cc7324b7559235c17764024e1d00841aa8e4089d77861458c7ea803648b72a72c99602067a5fa68470de" },
                { "xh", "62ebe16e3909fc81e3d8466c5a6808f0014a0d8f031c04cfbcd0783529728868aa5989e370e757f9e0b659d1d3ab8864252ac2ae8852512c818661317dad3d73" },
                { "zh-CN", "c87924a1b75bf3f094f140f12c45f0d401d918b1bac61fd9ebdc6907dda3f8ef8ae3f36d2cdcf643d81a8627e5781c078e5cd34577cb5c3cbc01d8e8622804e2" },
                { "zh-TW", "00a37bfa1e04dc08bddaa041be7913fb775c367c596dd4480ebd1c7b079f1a627d6535af619414e7164406e3f4b5b661dc0f28f5c758fe679abdf38738dca425" }
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
                    Signature.None,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win64/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    Signature.None,
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
        /// <returns>Returns a string array containing the checksums for 32 bit and 64 bit (in that order), if successfull.
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
