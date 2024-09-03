﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022, 2023, 2024  Dirk Stolle

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
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=San Francisco, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2027, 6, 18, 23, 59, 59, DateTimeKind.Utc);


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
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/130.0/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "09f203b64539ef1b752749bc0c8f5a0c9bc9a40c9edccd7f0a19ce3bae47c3ca883f82c306c28f430e33b29a6d767c294ecffbf987b4de0f6eba929f7f3ab849" },
                { "af", "71251daee5b9090061c6e1aecf5f80b6eedc01dbee60276d4fc1369aef86cbc4e8c27c40a1909a5becd59d67b9cbf549ef17f5a448f6b81f7f8e7df8784ce3a7" },
                { "an", "e733a9c93d1ab36fd5f331a6209326ee673a1a21bee0cfc01944854437633d9c8f14fa918d8088af000b8f2696fcba988a1aaf22980ea65fc83014dfed1cba5a" },
                { "ar", "c9494013ac086c85450d866c9002e48942d8de5da3d1b63c53cefc673dbc5f29e3635b46b5b345a48c56b0f07ff86a503eb1133897e4e3ea69d3dcb4c7dc3acc" },
                { "ast", "1d2394de7ff7902adee89b5a07981e27b89e029d7518051e3c0e41081e6ac94e6dc4c41b5123a8488579290a4bd6cfce47f8ebdd1444ff559a4086b401aefcf7" },
                { "az", "ebaf81d3e4b2f6be0b5f8f1cb0b52b93661bcf828e37786235d2f7400abe6444780bf636e0c2916fa89c3fe182a2bac0ff1785e014c415bb929ba90338e232e9" },
                { "be", "0b797a457ac854b1ffb6b3394fa5c51a33d3bd53206dbeb5a24f76e017052edb82f5413409bde91ed5d7ad29374f48dade2819f4d3a90cbd657fd51e999360d6" },
                { "bg", "11dbe3e31ed82df998752e47a2038e665c7e1241a5941101d5eb6e74793a904d1b7ac419d9a05ddd5985a64bcd5b6a6bedae15bb3deaa1cadbb8ba71734130b8" },
                { "bn", "a298ac69f5b639578bb4b27cfa64598ec10e3115bd8361dace4bbb27982d4cc65ab779c1ac07af132d8c8b89333a5e13f65d1ce725526199fb6df68400cf37fd" },
                { "br", "f3e1f9dccc03dc7db7e2c6ce25e274e5583c8d7aef77a21686669970b375eebf3ba8357cfd7955a2a33303e53d4d89799ab049c1ecb6f5573bf1f697b56b36b1" },
                { "bs", "bcae82ba13bd2297654c49568faa1eb14c6a960312a49bf2875fd67ac40d3c0c2c50fad234d43910dd2ec7e6b16e1a6c97f87608eb2dc2b81e92aa1cc7297ed0" },
                { "ca", "15d3c64943ff3abce0768200af711ebcb8adc7475002f56445b1d02e9f27841bb9f979c9785180e97ab33e68933e546a27889a4bab731558b06bd7c63dd5381a" },
                { "cak", "7ec1dadc08fb056e6d572c0989fc19ce216124b79c9ffdddcaf55bbf969b1fbad3a284a3cf53a26139f1a0126822902785c214ab9e5aca4674cac5a39a0331b0" },
                { "cs", "79cf196def38daf9742e4838c050ac387addc0ab5b5ca13115ddec731aad73388004ada1a54b33f1c9e212441ec27a877d0aaa97a6c669821451cff5e60b6046" },
                { "cy", "2ae26f1851f6041def43f4f7a5b8f0e6dbc70368dc87c426a54e7b278b19032df31024e98d7ec1844493eb41184b4208f5fbb540d83c88958c541f33d77c5b10" },
                { "da", "f069799e25f7cf4a397ee25680cc11ae6ae7aad881a04b8750e32167cc4661bdc13c04a2c9e1af9fdfa0fcbc7dd5aae729f168831ddcd766373dad600eb63169" },
                { "de", "3f0bc699c8d32b042fb639d28d1cf4e314c1d7959f1aafb81cf0f150600c9bed297345f1a67d37a3615ea5e435b40939297393b4d622617a0bef62493235fba9" },
                { "dsb", "2f08522381129543baad4dcc47d80265b1d5dd5d9b44fe909db56f5375112f7fd5bf7f2b1442d12906e631601eba6c9f40f100b23be93043a9ea2d15bee0fbb7" },
                { "el", "4324ad016ef0afff320b9a0d5062f7d1b8781885a2976004858704b64f519d66f256e2ded84381041afbab4bf4f2b1be76acbadf3444b892130d7360893d320d" },
                { "en-CA", "27fd5cd6ac8c5d3fe71873a27332979e12a35ae922f349ca8dae611b800120ec89e83aa57e8e7d420a4473d85fcc7d601b7e0dcbb1ee058ca04f8cf947913b5e" },
                { "en-GB", "d67de02cf6d029e1d4bbb8ba942bb1b21bc79542edba7bbeb9148d924e148cb6db2803b9a3738de11254bd4326b8dbd51b6b79953886574d564b137fe42ad064" },
                { "en-US", "b1f607620c3a1ad5823912152e1477072294cb9fa624518a906e8c4408cc758be7b738bb3fded14968472f77011e1de4aa13165c191a50d513cf88f14464b36a" },
                { "eo", "02186c1614c3ed659a85a4d7483a2beb464ba8ce319064ac4bf4e657c4004539324736add9f769053e31d8dd6fb101e38c4f9d21b26045c4fa671740f38bb3d3" },
                { "es-AR", "5015e6800e61e2b1044f5e48a57a1603d41d80a844295e7f871a0dbe7f032688e093112a9b74d5d102b2e86b6e6800985c837d5daa15e56273881741f7dab161" },
                { "es-CL", "7baf9e9867baa6ce47e5320363dbef9791fc8b7d7f6e349a7a394dd9e2d9ad372fa028f6ac753d7e8a46678fbe65fdfa2942049b289d041f69606fdb6efe2b81" },
                { "es-ES", "93a907667677e1485647758109c70c9b827d7b97e09e4455a13360e25e4d4f18f7acb178274f0d56392bef7ff0f9fc1b923714e9cdf926dbac5f5870901b284f" },
                { "es-MX", "bf431ad8b3bb36d842164bc42594a4414f63967dc73cf897c26b17d1086af5839e6fd37d759097526aa9807645de2e882afc3f4e8b16c61fb60668edc494c31f" },
                { "et", "fbe6efd87c5bb243f42b6dcf690c19047e9cf941b805086d42800c0bb0867cff245b825e7749287cccfc00e8ce47a1dfdbbffca52c8b52efdeb2a8ddad06c4d4" },
                { "eu", "8c79e04eabda020d95712b813ae232d02c8dfbea497666e78606c7a350dbee31bf48f504261030b1f2b9ffe4607c341cb8aa440abd8c2c990f1b24bb46adf7a9" },
                { "fa", "9b7f89435304f98ea3e3646cac1a34505adaf981fa356b8abc6cffc325d3faf872f08372c94bdc47f2354662719d8c37d6bc5596cab730f17e39e42af1e0800a" },
                { "ff", "d85974d325c1d2b29ad5ed9e9d086a1c50808e3f1017f693a0bd57273302b1e77517aa396b94dfd99b2b3e206a78aa48a94af015adfffc473f020aa752cc3439" },
                { "fi", "74cb005556cef9b87afe1b6b30569d2add9cf4cd2db7905a5d1116b159ce8f31b64df8416026699cddfe6fe65ba9c758c45bc755fffa486a6da9259aeff2d8f7" },
                { "fr", "f2cc956a48bfa1fd80e0735f12e65773f9c782ee6ee4ea231dad236034b544e543d207b2a8c78425e411e139ba592305d221aa273c2b8b85f645a1bfd9da0f80" },
                { "fur", "8d1b4ab4141fbfa8795870ecf107eaf9fb68d9b85eda649124ef4a21f48231d4ff4ddc25a6d03f285fcad0ee33126616c1fe56e0f00b90ce0eb7ba54a494ba1d" },
                { "fy-NL", "176c4be37fb60afd6d220962353d3ae082eeeeb610550b08b99c691f933cb46d900afa8b175eed65a68e7dc8f6a4208afad1b77ddaa33fa5a2b326635d99a28a" },
                { "ga-IE", "2140a89c96590eef0ae6a7a6e31a0e3ef9d21f2b71a887549fee3c7597dc7d5bde52e1e123ac57d700dc951b5014406997e18adaa52f7ca7a8a2123d9b13c713" },
                { "gd", "fd38fbfa3a5507bcb3abef48b0bc5315b905656433937742799f156b5c762e2d7e7159a0cea5330d0220d988dec9a4a3b5f6a87613a549b9f5120478e64ba21c" },
                { "gl", "40f346b476519a9c7c2b361a02f21803ff582050ec5e514c3d3a75132ed15ec83448e73248a79e965da1aa0027d6b43aed633cc39563e074eb472d68c7859a42" },
                { "gn", "ce028ffb02216c7e627ff90648b070c3874526228aa9de8ac0cba8aee9bb75f1dd123b4dafed6f8d6406b730b1a72a32cd752da79524ef6ae59381e9278fe227" },
                { "gu-IN", "df2d9d25d68fb2a2015ecaa2bf38aa3b94e4a0b9a1af15b93c09f935fe8bca48a0532776dbe98ebc3cfbe7f497f352a93fe877ca87989d730b41565552c82e05" },
                { "he", "0f60160d0cc2d64c52268a9be81ee89c543f5ff5c101cd2c943205d76ecc8fbc5b64d1e4b6c8e19b52e6911536d74b1454d107935836a730814b1651ca4ab253" },
                { "hi-IN", "f2becf442d687acd5636cc8862928096d7de8fe8c4d00d21ba20a5b35c5d967587905a6f976ca4b8776f94557ab86ae94f74088e93e999fbf4eb676fafb8003a" },
                { "hr", "621c690fa4d7dd6945cee98b74ffc208ccef09bc44a07f3563023aa83196c536923be63cfdd8192fa95fe67fbafc266457f5d44c8990a330c8be851ee5a951fc" },
                { "hsb", "bb97908d185c68e0d321b05df755049e3cbfb5492ac84f307fc40202c3150e6be5e3922000da0d2c62f1b3173294ec3ce8c1a13c037217dd8d46e319fa11805e" },
                { "hu", "f47f2e839ba495176193822de514e34e9cd2b4c3453d0902e90368f59508a2a27ef68b9bc8030c247d57b3a009c930004a82fba9190e238e94536d1b8d1e075a" },
                { "hy-AM", "92f48dc4ff384b06b88ae7b51b94293234bb5329fc807ba590d41c3794dcee66e300b2f4961dabead7ee3dc10f523528611c7dd24cd5448b45268b97c7a1b72c" },
                { "ia", "7642e91db27eac9ae0abfaee6488b63b4af2bd5b5dd5d56ebfdf0b522a01b092cccde8c6c645f4a033419f2347c4cf2b77c55d20a5526c0cc162b5682f2beac1" },
                { "id", "bf300fd2ed95a540cf94c210829bdf0f3467fc7e07e8e32643df29bdf42e663e26bb6f753a7b3ac336045fa09047b729b3235bbfe9bde34248aa69e0ca24b572" },
                { "is", "0c3be71d0627bbfb0c24aff96b7ff4e7d29c64e27d0d0011e44672881c4262caff6358012224641c31012abb5090c885c66f7276de2bde2fc6fde2a74367ec8a" },
                { "it", "b70cb1f3b230ef5392de1dd580d1667abe4dcd6fa9095edf618474f8d726505b21e242021b54aff5b349f6b205d6ccaa58fcef4d935e38469b8e5acd03486a89" },
                { "ja", "0f523d939d63b2a17d709f72e2721cd5422878eced9845826ecd45f9b58dad6d8c05b084a79b38ee1c77e0b2c3cd7091ae6e6e33921b7b9e33f5ab13179ed392" },
                { "ka", "83ce01a801845d767033effe7dbc9a110a1101ee94057ba40e03ee55c59f481d0f7f52b222edc8c176e7c3164375ad3f0aec1a16db87aac48ac2a84375651bb0" },
                { "kab", "8a07af43b3226eaa3d736fbb9633b04a2db35c7a970f9c7162e4672b42555e47101421b664e468b9bacd45c1009fffcb0cce180c9f08417b7f9408ef35a465a7" },
                { "kk", "cb2e905ff6b3583faad157d0709c346da619e131cebee58a73e0d7b605ae21586e22537d9062d04d2441ba78ad3afe67be3b820272514652ee7af45b5784ca4d" },
                { "km", "29e66cfdc8016d08af52c2fbcc467b307f88956180ffb1e8b2f4b01ddbb432c4c4aafccb0362dc21ab71f9c1eeb4deb774317df305f68e27258a488c61d594cb" },
                { "kn", "d7b3ac38466f9d2cbcac481f2db686d5e5249b54d7ccd2930c6b0e19dfb01c61a2f127ed43f52aa14687d577d5664ba84b73c07d951b97e02698d2a815d5c6dd" },
                { "ko", "696c967c9cfd1d7edb585dc3487b08e10c2bb842a19e22c927302d71b0697652d7a21cafe32ac439b0adb536e2ba4bee5ac804d9a5c422b92948c19a98963150" },
                { "lij", "991cd493cc952122a367281875ad82bf6ae6f405cefb8a18345255a8b9435f0325dd3d19c7f2cac3c69224352b238370b29b8067da9c430e3f62c81a5a6933dd" },
                { "lt", "254fda114f071fdd7bafaaca42928a7c72e5ad09999093f21818659231cc8e70e44fee12c513d1f5b0efb185759fe8600430ed47a3b4a46961e51e5607653679" },
                { "lv", "06ce3a514e31838207fb9471892cc1ed99f004c560a4f29f11f45b7aa347c80955e79444fcc99545c7f9570bf1e39d421c45c724007dfaadf435deacb8b154fa" },
                { "mk", "025c1004814590055c65aa40e2fa11120667671178920ab27c094c6736965fa1c530fbb7f0ed0a9b92c11e3e1ddb0902412006a4e028f53509305312994f85ae" },
                { "mr", "36bb08ec835ba87c1dd25f188851ff461c5a7d6dda35865d9ba08c966041331297bc5bfc81b282b63871e8c413123f87465c8740fb56f3f0022bb075b2099682" },
                { "ms", "ae7b977786680280fddfbc126498e34dea3878f14939a06a4ba651e3e13ce15886f100528a2a3b6ebc07db9cf282c525e9fee75cf5cd437094bf5a5795df050a" },
                { "my", "b3d68d49eba18bd488e45a22c6014344990edc7013050a5d4991e123c77531488c1263674a76aa722317ab927ca229207a939fd0cc57a5705f5cc61e00e2a50b" },
                { "nb-NO", "53130d2de1ff0822aa5f18a7c0f2ed686376a7c9a5d788b79e14b6fe3db350b462e5f64a333a69853b46c344bc4418f1e64eb90e639a6dea4faa6186c1f75c9a" },
                { "ne-NP", "db81db7deddf84c0cbe073e368d3c1fc290bef3dd7f0fedff1d7f1acaa5358cdc3bcc16a6cd001b44e3e75b8097d11a2449348f8043cf240a6b415ff97aa71c8" },
                { "nl", "41da5ab9c021b8150260284d51f8f9feabd5a5b40dbb37ff28be03a8b157fbcbb53d9f01a4aed99576ae63872615b997a730b5e3404b6b6fdb3d5418611a3d0d" },
                { "nn-NO", "82b0a35ddfafde7eb1c73b772b7e08ceea9457e07d24e060c09f289f96105efa06f8048e152a1d66f1c0f39a1cb16140c71d9b2ce66596e24aae26c78b0c43c6" },
                { "oc", "b3e8c27229b26f2ab9ebbf1d3d273cf45a1d6d81d6402ec996da1d8e13f09e3433f6172589201a06fd466ff52c48006ac687cc0906dd819d0c63f40f665453e7" },
                { "pa-IN", "ba5cc42a5e892d435633a57e72b3f6c2f48e7830671d7b40cb2048a55ce2e396c7031421aee0f9b9661971917e24721327547d6afb4384de2954e61764363b14" },
                { "pl", "4c41880fc5a69f7e060150279a495b32bd08e18d5aa11db94dd7b171f247cfeff76669420582288b3d53a2e873d820051dae3aece9c20b86a126f5fc803e90a4" },
                { "pt-BR", "e3c28c8dc7921aea834a0ffaa50ab03cc3aa2300707812ab2affd99a8c434e8c6a15d1a5fce2196a55ef0aa228458db2a7f2f8681e59dc99eeb4fd17110be9bc" },
                { "pt-PT", "6b08458fd92495ac0c19e1319689ca1cec8f4b14325663aac123cb16e25b91e6adc54c512c19d96163f334f641a76756d94192f5116618ea6d03b75b0ada6f29" },
                { "rm", "fc079976c1eb9bc26a17ee695f54ddbc6d6a5e8e0af963692d1ab845ac2912e8960429a776d53666cb06668f0305018af2739ac6589cfd8d4948d1859e11a05e" },
                { "ro", "ac1ca5c1ca102dfeff6a581ac656e14ac148b7efa735bb31367800dee238c6f824dfa5c81284cecfec3fb3aa2f349bf004cfe1154f301860dc401c01d6a8a463" },
                { "ru", "a00c44e5ae4692fe2cbbd11e5e0a6d2730010caa4dc7894f2bebc350dc5491dbc05a6d2a8bb659a7adfb1ff52cacbcf671b771554da04453d685135673cfd810" },
                { "sat", "dee464280cd6ef7b6830e4d99679aafa9ed9c8435af656e8fcb069f590295ac3eb436a658376af3e76528c19f416ba5413affef604c1723109106289cbdad62b" },
                { "sc", "b6290abfd572ba10a41cabd0f118b779c2b00b7d9e8fb0236ae7b8836ec75ab460daec47f5c65bc696e8e88d87ed2f4f45395909ef11869fc07a16b19ce8e150" },
                { "sco", "9fbc0d58ef8fe62031b1c92012d3275cf5792282a1333a1c2384b55a7c977fd33f917a9be222d0686b26ded3ec31c90de4bc3e3553fb8dbc74903f33645f272b" },
                { "si", "afaa69e050c1ba551563458b44c085e322cb47108de6629d7765fe491afcdf2000687ac25b48e211e81c87e33decb074aa040838461555c7e4297fcb9af6682b" },
                { "sk", "cb44c1e5ee731c38a0bbec151af040694396bf4c9c0eb6a2e7863525d0256e80e75ef726b8fe1dc2c1614e06b4ab2154f66f9c96e5d7d35af01d57c5bdbedf02" },
                { "skr", "e68e88fc1b80c0b103df2bcb4fcf9bc8a3955fbab2d82b78d54d84a419e269bf36005f92be582cde14e89b20c818797f8210911e4f7f12a525283d0a9bbea593" },
                { "sl", "8c934f3520182b80428d8ca0b6a7bd8ef294a7786200cdc5b19a1e5ceaae3534d483a32ff189344ea7abd13007dc425d9ad64a2b588785fa57bd6831111f0b39" },
                { "son", "33f8fb4bbbe17cf9f1fcbf1e8df432aaf3e13ceb24aac7b3b6ee7ff4cd785c67b6e1f9af6459590216d7a2f271f64a789ed32afeeb3fee8746c67106024b6864" },
                { "sq", "6fc433e026e6164ec94fafb4017aa17f272e5cb844c65798982f5b3f57e2f2d9d8852ff512bd957c5f2d89d08260638900efccba7c0cb7bf08f7db9045f8106f" },
                { "sr", "64c0e535ee23b69a7961593186d37be6b00c3ad2fc8bb094fa54acb19d2f0e6e459d3b1303d0cb5d9c1cd18dda5e20157c4e35fb92e5feef4754bffa5f7ee97a" },
                { "sv-SE", "51b3303b53f404a2529f171fd7fcf129e7e190f4c995e85e7c449f090fea569ddb97e26e832c7cb011e539b8e13068aa6cb676fbb71cef0626678eb11336fc48" },
                { "szl", "40a1ca5034d45937e4ee484b8383a6fe829254cb04fc8f403ce065880a544592db630e3dad852d280df89bdf972c41121124a5f60e59bad53d073fdc6d43bd2a" },
                { "ta", "46138498c682d362cf9b7ea9e6a64ee736b9d3e9f40dffa526db235029155e61a883fa001012093459a2d22390f863c940cefe0cae4d355f1933653ebe6ea077" },
                { "te", "3419371be357b938d8d2985fb2d993016ffc9169a3b9408b1af64304c7d0f27d6b5de5e4cb51623a6bf266063e3bd7154421e16a24fc98887ee739e52889ddba" },
                { "tg", "73082e967193166b558f6dd41c1ea45bc1140c9d25ebfa29871db165523b1285b29a7663221713789c420377f0797f6b1d3c44e246af0dd165e5d2ee087aa623" },
                { "th", "94230fa106a7ec4ec37af4d043fd819c143f954619cbd4f6db34083006bf2987f3de5755df3054759bb433e83f479145f4d06dd3b0f060e9ec5d62751d1ffe72" },
                { "tl", "cb9aa21229446d7308fa7f8578af38bf7dcd7b2b16b71abe459bc614b496460230305e4234312ca89ca44ec85c9b41221fd700624eae2094dba1e4f5a6293552" },
                { "tr", "471290cabf41798158be36f8ab5d468f8f74520ebc57ecb64926b642fed66dd8a1592ead557b9719aa24ef968e36c1fd02163511dc9db3b9c9eb32ecd979c7c0" },
                { "trs", "b077f5da29ab2b534f21be1c64538fd45bc432ef05d1e2ee9aae01b8389368ffabb95614616a31b6dd51dc4fd6b747db289d0ef08011b52d778c3e5fe996421a" },
                { "uk", "f4b707d42b8e6f9ca3eef271d89cc2b12f49622b80729ec67c79e852fe2280790f48df18356a4eb16e7eef6a4df54e567dfc984f37f078e43ffd95657377a9ad" },
                { "ur", "1ff2bbc94ba6a3211aec5750c317bf57d7e9ed8ec4f39158bf096eec106b6f6fae252a5969b206b883bf2f529399cfcd104367fc8c8422819c5abf832ab4adff" },
                { "uz", "60310651f5cccf8a333990ed65524bcda73c850e166477677ac918b2a3a91c2e66e33a80ff8795fc3e6eddba0602b393658adbf39ec91c97c312499623c91045" },
                { "vi", "d9869edba889b936fc833faaa07cf08d0a5f33bc5231fa7074c8edbbeee0eb23911d31dcd6beca26a1662d6addbf633aea77dbeb736d054e813a339c09bdbfa8" },
                { "xh", "9ea041ca8e87b4776f0130a4eb1ce0791a188fb9b7f32174c057a249353214d3fde313b6ab1d036b4cdcba29a8420961d0311fc74313ad7c345c0db3fb32a761" },
                { "zh-CN", "a83063fc71ac3ff3212b70a0ce572cdaa52ed283e5d61c6c35b8965596cd954c4eec9a6a0317b9ab41c1b2215189ca79d668be2a013d8f13d77735ae3a2da67e" },
                { "zh-TW", "4e58cece7d143f539099df9050bcebd78055490f5709b6bf55c721b1ecfef061e1f3511f7484f52695749d9b07d81c3b46d1c7f581aabe2f35d1584571b72ac7" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/130.0/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "07fe9c13aa88b9ffe8e4247da942c3424f028938ce662c39fb98038b72744ebfea83de93b7732c7815aefd534da6a6eaf3b5d4332f12765dc8a72d6f1f3c17ad" },
                { "af", "83f78fb594c182aa95b53be735bc9a468ab73eed00603d7c8e36963134eb3f465ecb3b927f9819a80e2ca7080f17caaf6a546f92fd5bc2f2610fc06b848c149f" },
                { "an", "ea944f61060ba99da5c7cd8b5535108ab5554b263e7048e5ed46de4ffc2c9bcb8e0b6a2d78986f99bb96cfe4d98ef3b1cbd394d580312ed4d766c77f54c3f077" },
                { "ar", "d67d538f59c43c29c860d6b0f07002bb88e8123eecdace2849fadcf3ae6945b77cb3e776aaae5e6560cd9d140b5c7cebd90b464b294152b326752345c26268af" },
                { "ast", "b123cffb381428814d9d71563d10e45e6c0a9a4c757f85f2c1371780249fa946f9d5d5acfbe5bfb4f10500f2be7876d262db27f6dbc1dd69689deeb7708b7918" },
                { "az", "8c1bc31c4b43c7a5cbde3cc59fea120a9465284cd018eace7a406fbc338f902b23c609db390ed55ae03046545b4ff709e6d3a22a3e4534fef9d45c7acb63aa99" },
                { "be", "1a10d42b819b48f89b45e9cc1e2f5d19547a02bdfe18de33fb76d1eac0a574bf4c0aedde297b2d336248e456d1b0e8d2cd1d5dc33aacb83c073af49756ef03a4" },
                { "bg", "e4d0779bb79ee7a39e4ce8fcd2995a59f624f4185c05ec62a10efd5a1b65d9cb7844add149fa791b599faff95efb32d0be5fbcbc811675e010adc3db5cd78d5a" },
                { "bn", "c2e2252dd6b0e9f135f039042047ad135d56ae8ed6aba069393f93efc79d024c3797b7e5a914b1e2b1eaa74a93d7e5ea998bf5a0d99105bf9661c592a4cf7725" },
                { "br", "a278766ec09b4d42941599416c38ee174dcbe1785eae15bcf237a3f54f458decc89e89fcdc86d6e3266f99b7710fd838fee969bbb1806d364f39f46352843db7" },
                { "bs", "4fbd5fde228a9028b97ab37196584eb6c6b272387560e6d5e6272daaa82b329cf6288311e6b87dd8369370c58e7e147a64fd360ff69e95299d3aa4eaad83fc61" },
                { "ca", "5007be04d0be80164e0c3dbb09ac0e0f0991b09678f37336d66bdc39d9971c18ce68af69677a4924127a893d6065fa3cc3e8a01d40578ca651b8e1a34d0e8336" },
                { "cak", "6be571052042a34dc27eb5b286ee8c3e53f6bf5b15abedbfeda2518a0db3a5969f4d9a9effc66b3df897c8f2758aeeeeb6d72d6bb65612325f5fc1e27c418ed2" },
                { "cs", "f6a4e6e4e3863f80815f9dc24d64565ad633b7366f44b4fd9a0aa449c11751e76c073f23d296d91c2011f5e2eac4a16fb0f11e0aedd25742163da82856862e34" },
                { "cy", "1b6203441a705ffcafaeb08c52cf9824809647956b2c31d5127b12538d5bb3a0b6404b01cf79506c7bdb2639d67cbd991117e3c2aacda4c8cd15d63cb1b16c62" },
                { "da", "0a54226f0067465c12d29b61bd38aabf5d99d5f0bf4c72387d0db6b8a239004f52dd0437bd95b3558a583ca108ef17a91b4dad3823becf50c3f477a855bd5e8a" },
                { "de", "312c20f0c4432a1532d2c0a9ab72e01b8f5318116e78aed961b4e83503b270929f682303eebdea53b0de2bf36c8cb3645e79ac810baa667e00c2a449aa44fa57" },
                { "dsb", "029ea9c946c810276ea752c87da43ded6f543fb48e2def41e1a209966dbef566c78521c8f3142b9179fce88a54551ada05c73e0b4a403bd8f421ece27881fa6b" },
                { "el", "342ad8eb9804b5d7adef3018a32171ae804763074fc1cea72d2ac654c12076b9ea8e0bc46ce6990001f42dfce4ebb9b66660b58ae560cbf99b2b92c0725ec392" },
                { "en-CA", "0775834b2337d671177b1e897f424fad8ecfa018fb4eccfaebd25c2921bb95be0bd7a03683f1e1254153e39e6260a4f264debd055f1739b7acfc354919f8181d" },
                { "en-GB", "683265feaa1e47a8ba42ee51db22fe6e8ef37b6f881158f288cd48e77f18b376537d4a8b4d7c019ce8945ac338253ec85947e151daa9d08db20e2b6887e65823" },
                { "en-US", "e853b1792454cc7fc3f40904526b3a0283cbb356a103e6f6529e66da5f571833fb9b56702b255ceaac45387b7a0d60fd5277cc69ee4122c4f1d69874c90629bd" },
                { "eo", "855c7c1b210a438636d872e6250d3aeaf641ba9e5157fd8af9ff98060c5e9d2d3b7b03a930bfda148c6939fbf94125959364e683c9b701d0a7d1df4fea7c1aa1" },
                { "es-AR", "0f136293c9ef9d038110007a109e1ee376717f7b1210d0d1c4a3442e4cab848c21ff6ac8cd0a780d5d4bbb5ba0c8185ee597e138597d1364ce01ca63aee18611" },
                { "es-CL", "245f03c87fb5b85a05be9f1a79d7fc42d629582a99dd35d2c5faff590d2cdd743a673b9231a4bcfd7289bcf9f60a27c916fea1d1a609dc382b77eed48355666e" },
                { "es-ES", "7392c0fd86d7e40625ad37419668a6225148ea24330ddfca54d675bbeaea4051685e2fe45331c36ac69bb9697972b3b3386ca176569efc96ccc71ee16b7f47ec" },
                { "es-MX", "ac6d2ef1f88de19bbf6b6167f01a3b023fe4523b8dce6a80e5c8c5a431c61aefcab98f33d5d12fbffdf0ff06c428b4153b00268d2ba285ea7741326cfe58b49e" },
                { "et", "877dd35584b7157ea157d9ab1ed2e193b462f57d60f094e3b2e4d42f1633a2dc581b470263ae557cadc83afc888f168cb955d51f36112cb6b4f8785402519296" },
                { "eu", "0c225aa5b36488ae39c40af97b69768c33de5fb4bb3e712a20f6232b74d3113bbdedc59287b903bcf2257a29f52da41e8a15adf147bdb7def06a31d073031ee8" },
                { "fa", "ef2396825baedb81b4fc08aafc40fd69e511e0562b70acb29932519ca4cf79b5963b75fbdeae297da3ac92fa9729a96451b10c852d546f26509de49e6a3c147a" },
                { "ff", "6ccb3647f86c0e0a36645c95e9db0fe05945e5d147a4f38ce79a0f86ca25f82faaa647e35985fe6bb89d9f033d6f49cfc41a67df96f95edbf32b1d8768c0d845" },
                { "fi", "c1f2c5adc15ee4c02a5bb68939f5eb5eefd88fa1bce65df8086ad3dc12f7875db7959a542c9ae6e85e70e9b243e2a3c9e5c82e1f7c0ca9612d5f7e74c5b30168" },
                { "fr", "b7d180824ad94af15eb9df76e5ff9a1966f8e9d22852bc5f53b679fcf43b95c00fb13c1f03ac6e9e992bfb183131371a11900bea55c0b9442fca58c4ed48966f" },
                { "fur", "aece960694b240d4c9b385ed06294a0eba6ea4181f407157c169c132ba706aab78debcf0f41c4bb571781764ecc64006e4bf528703bf3fc1e75484d4b4af2b95" },
                { "fy-NL", "e611455562027f106c2a83e438d4bb4c5d861e2d6ab6d529b467db3a406e438ee3f432734c0ef5f12b9fdc506af30190b524cb9204ab4a3120f0a9d161853e84" },
                { "ga-IE", "b6c4bf290f0fbf16a0442a7ab0fa1f3a717df8d826a844995ce5a6e474d34aaed17915f05dce729698ff25a4cc46a39d2d76aae68435848fb2b4e69c23eb2900" },
                { "gd", "3cefb4d3396fc713b34d8d78e3e9016434357da8d02109d5dcb4757543d5658597520c3784a9955f28749bec37dc1f9b3205ecc3e0dc227dfdd386cb4a4ddd88" },
                { "gl", "eda7c197cd5c80370bb11d4c36a902498bc226de728c36c08aecddb62e7019a58d9141b3d5b582703fcc8b1914499c15c969ca88af63ef9221054928f680addb" },
                { "gn", "445ac0a066ce4b9f2e080c2e786724650d25b5a74f2cc37c0c3cb4bd7eafad63f48aa47a231c7107f8fa6433320401990727c3409faea34e1e4cf64223c0e6ed" },
                { "gu-IN", "8df47a158458cfbf4306979214dac0f3288e9168aa8cacbf88ef0e567874b0447652ac4ea950b8a6e26c90c2b3217a1f1b46b90a60a4c62eea94d9e9d78a232d" },
                { "he", "80a6978606c76db47f45019bb25cfa106c53c08b2eb455efb349bb1e04ec64568c7272d91efbbcd14e0e70cb78438fb6fd7d3ebbd9658d433e86c15f24146abb" },
                { "hi-IN", "d8e507338f18d4709cc72eb38a229adf182638a1a091dca855cb98585a507ae106713af7e5a60f87458cf823fa490e66b2df1fc0924bdd02ddadd97405f82959" },
                { "hr", "2a12bcaba29eaabe0d4c3340d97b8764d2c84f1d9045428c58f2ac0c0b86728c096eaba14e1f30f57bf2f891943923147b39174a30cc149843401469f1447b61" },
                { "hsb", "e0a36af36fd813d27c16f517c4a24db7bb47c12c4c50213855853ad265a204591c94ff6e808fe210c68b8f6f719bf6cd0692c0a9bbab01e7a58a1cdcdb76ca03" },
                { "hu", "699f4221286425fbe3c4aa1e6b7ef7d10d5efb28909c8d1f31699093ad28cc1284146c04699e0a146ee156184379477e736a987e9782e56e2c8a04ef3f38c1f2" },
                { "hy-AM", "1235df84c01df237ce431b7a8479ec8e40b53c5d8eb7c78c0d79219f7d0eb6dd2c228a16155f61fb3d23a124676e29e9e1f3de0b43c194e5f8dd07d0efe4d549" },
                { "ia", "16fbd3d35ce7aaf7f658ea74f1ecf99af7c1ece39f121976203ddd0a60eba5cce8c5c4509772596efba3951cebcd14fb9a3186eba4925b89bdd5496e9528ba64" },
                { "id", "416308ac3b423b883a4b8e5f28cb9c1c6ca1b405a7a069508c19ab2c81837cb596174fc751dfce87538b9e91ee8ac946e1320cb40a3fdec91ff5a023dc1ac821" },
                { "is", "33d1f6fef2fb98527dfff4cdc960a5d1da880b921f9bdf0905a99652f37adb3c455dd28dba1f3a700fb8b44ff402e5b5ff4e2f422f7e12303ae92a308b9bda3c" },
                { "it", "8873bf719afdd7c004be3bfbddd89984c975ede9666e70b904b5e021b0b2f0440195b4a3f77f3f099633763af8381f55d58caa922bb5f8170ce3c1f49b3a5151" },
                { "ja", "d954300c67b255ef0118b77ac0d0ddc0d26b3c12e1cd67e7b7d89f0f9d83db54770580480463126419383bd66a9cecebb3ae9cb3985d73907a88a375bcdc4942" },
                { "ka", "0bfe5a56e8ba4c69edd1faa4a3f67057cfc5fffbaddb264f5ee473af0c7f5ab3b9fa1e04a9dee6e626aa355541d4b0aff3f37bff3bb54c1b49777f1da8c3725d" },
                { "kab", "c6f258d8004fd1c6a9771d26785a160aad02bc573d68bffa39008bff0db72bc917d4bca61b36d53ebb19d5c6dbbc835f9394101672161616bd2e78b085daed74" },
                { "kk", "07f08749a7e70d3f02cdb4de3a6c636f573b0de515f50e433bf58d8a6f99e60759b0c826b1dc697f6afcaa6915ef4dd46d80d1870c301d877a968ac3b5bdba11" },
                { "km", "05dacb167c6c9ec2a341bfc17a5a75ba46b6d76f4556a5bfb8006a61670dc070fae94b6410b2a52e43d2a4dc426cf07397c0e13fb1e26ac12bfca3ed84340813" },
                { "kn", "22579bb68ccb4bce515110305f0501775a40d4f1041ecf939265bc9e2c87aaca29ce1c52ef25b3489c44278b14a7b81f8b12a585a4c66395e6fda5e458370396" },
                { "ko", "c75226bdda215ed7bbfc93fa84e707f944ca9524a346652baf1e7de0816252dcb082c68c9a14f70a3795580b541a74763f387349523ba7c85977fcbc2ebe2bbd" },
                { "lij", "2adee7c8ea8f5c09540946c40a99bf44bda8b904bdce2f5c400c9dbc6c62518b506108bb45f249895c72766cdc9c87422db6690bd6b9d5628c0943dcc3a8cf29" },
                { "lt", "107de1b0a4959c34fbbbb762758b8c26821877cd08f1926ff5df025654a76fd12fa3e73a2e1f35445b7cec54f11d354cef93b0a1bdf011a8321a159f152b97ae" },
                { "lv", "6cbc3b8fed1f9dc51c0f016763eef8de631f35fe6f34c72c9980d259ff79c901bd8c058cc0bb05a9fbf3beb4f36e499341b1eaaa8c07d0f23a67bffd2491c24f" },
                { "mk", "806aaead1159f7e0bac35997e78e90ff5e5ed326d5c1c2c1c8cde6e20c0c0ff801f743bc132c1e6535fcc56bc4af819b2acf1edfdafcda3fefac3fb28f4929fc" },
                { "mr", "12226ae097cd984ad0d2af8719a64ae5ec1c25eea2234e392a9f45fd819ef4a46a19f1fd58bc3b17c634d6aa637308a6bbe5f807140249c7408756ef2dd18083" },
                { "ms", "cddd6e9be22ac786748c63664a4c784ec02324762753ca672f6bced348186b954078024001c3541815757310cf4f133ad6672081949bc0dd03d26ec4d4d64636" },
                { "my", "4c9652931ae4562e0dbe95a9a723f721d4985a5cd5de51a7191ad8763e628eb178f648d90206fa32cd26bbf0f9311cc3d116513d9623925ed7b889aa12af5510" },
                { "nb-NO", "d717027ff1b55f7511c581c706f95fb7c84dab85521a9c94c54fb0571e24805cfb96c96fd6d76d5f16ebc62efc2a627a6fce787a563023341e20f4d177360ba9" },
                { "ne-NP", "723edba39c3e025ed8e852b10eb48e85f66bb9da4cdd4f15b849bc6147e1ce9de7e55502f672e789b1c329ea9002ff160e972353787c622a7d30ee6714123ad1" },
                { "nl", "4863ad6e725c4381ef57aa4418e395e365760b834fbb4bb33f874909ceaf4a71bdb6cd28d55f70af82106a9d2540b6ad8fc12a73d4b624862afdec8e1a20f02e" },
                { "nn-NO", "d4c56469e78f6027b7b4a9961d55c2a30ed5da86cfb7aa81b96febf504c27e7dcd03c5cd062eeeb278a94f3749a6a6b88962f7dd0b37c7de85aadeaf0c4291a7" },
                { "oc", "3c3304318dbddda035f87c802bbaacee5644b69f142e1a7e3ca7dfc6437fab72bc819cd07a3d0d44fead65928a4cf40fc518baf49d71dba83cddc01f3c36633a" },
                { "pa-IN", "ce6d3826b26e34b44ab8037497d7e857e6584ef639a88930f9ffa1b4f35361403a871ee44fe21a5d98c2a65c1f7cb189ea3b130caab0158c4d42670a3b248acc" },
                { "pl", "959524ffbb14c4123a87873ade69a63ae25fd7c3ea348ea9dd6101930f1ae1d0b323d9a25ef458320541d507c356585a4b7b958098af6f4301e781969069cc0c" },
                { "pt-BR", "a7d6c5b013f7b5687e5c5af84012a1f60f1f84639ef435b1724099bc75ac37143554030491e898db35714975e7648bdbd9c0c4452ef531d2d6a775bbe57b267e" },
                { "pt-PT", "938b2420032e53dcef832d0eb5d29d6d4d273a227119799c953af29e50e431175cd2ae2d36fe12b269e09dd62ca895d1310586f3ce1aadde05dc9e09987189c5" },
                { "rm", "d5b9d1fc49a73dc5f2119dd6de7258ee91bdbff8aa9269bf60eb48de69422750ae6bf888ac8c77bcac169594c9d6fdb32defb6f6001f08809e31753673d57501" },
                { "ro", "abbb99f2bf3d82ebd4f280644ab49de4e82d972fa003c5d70d54d61a0313206d14993fb7c172beee8a2da239c2a5423155af8af538c5c3b5969e6d64aa32d86b" },
                { "ru", "744111ee910ebc6e3a2b0d73c2cc3871902b72aff4f61736278552e0768c6b9899a0bba96344dcb4389fa9998e7070ec68df74c5d8fdeb95cfa5edd18251ba0f" },
                { "sat", "8eaca381c869929e4deed816d431fdcbf314a156471a7f372393495c407cf6ba7039ec08a194437a10c7a5785f44203636fbf328ef34775e6e539256fbd44945" },
                { "sc", "69d4cc49d5d007467e3b5a521e320aa3c9d56cf66e1945a194e60732c9c37e00411320fe4152528f41338bbb9e5f90d6f39d7bd86087dc72d17db474ab917b53" },
                { "sco", "e101e59b9b5b71cf6a3b4c0fcabab042268c4a5b0a40cb70bf3741f4670f6bdd5cfcb01cd9fe4b9a62c262d781f3170fb7b6539655df72f6ef45ff684f24a5ad" },
                { "si", "2b32608b8fb9bb7a7365044d6a45704bb60fb3e0e8574aab750efb2bbbfc711e7c6a902bce3d452b4c4b3cf50f5b17b0614783d9fc909a9382d9630bbc3474da" },
                { "sk", "de0aa87d0343a9fe4f7360fe946b06cf7334c5c0410601973a9d44369c401ba60d697e02f0378cabb5fb153c4076f480df6ab2efec67aa09ce5b34fc23d148fe" },
                { "skr", "c21ace825ae36d73d88c8cc951f2fe2f5d0bcc38e68ebbb594a58957cf1230602d71f349c41a34bc3ff39804f60beb387231925ca0379e5b7b871b9e5a3c664f" },
                { "sl", "7a5d4256c2d51ef7ea81f466b0ac9dbe20e88d86001556e2f2f3217ac7dce06e93991f9f89266e19f58a5de15018c58d66f601a404b4f4cc523d3e7e127ed43a" },
                { "son", "824ffa87e200e197a040e43dc1dfe04bb1530819b10ce13096acc586e4da5fc6fe419a38eb58ce337d27cb384ee236c4652391de1a148d138e6bbfe2c89209f3" },
                { "sq", "6b7b3295175dec4aaac7de5e04fbe0ab9217ba3e257154c81c102731f88e6fe41f1b45bd5458422b1288e1c4a5e9504157fb1d9db786e16969e9219cdbe8fd46" },
                { "sr", "d568bff86e25ab3013abe588f40fd4279a6a81ddfc1f5f7ff78cc9a22014434d575a8981a62fc0d0d7d82e044bcc78f73d2d6f4c2fa5c97dffd3bbd2ccbc5e8a" },
                { "sv-SE", "f8579fc916440772ffacee1020493e1c6ef16b0e072180ada76e549517a902611bf085d713a5cbfdba53dcc405798df5ad235e5969b44f20bfbea0e240bda6b8" },
                { "szl", "2826d2f286f17fa468ad3c82499662031404eb782b663085200dbf22c411efbe0723d2ae5044c6c53c7016d10e5b5c7cb8fe3db4cac5b7cf303c69e87d77be3f" },
                { "ta", "71fbde297c7f024d95c22330e5774bad21481f267e1461dccca7d4bd475ea8d68cd8a41188aa1b1230a67a966424921367dba50ca49d78f289b763ed02fb86b4" },
                { "te", "078cc0921012b933e908ebc03f856565ede035b3efd7395c01dfbc00569052d31f8382f5bffe5c8a3d67955bfefe170eeb4a028f3804b05a1575b94a71322a90" },
                { "tg", "750f87fefc719657c46ba0706c8c78a8c92e6b31e5f72b061f29c44fe36e567a96ce88c002fc7408841627ea3c079954d8982b9fb9d88c535cd4cf3a0a948830" },
                { "th", "53555fd7da6cf9fc3bc6f4616a67f3a3bbeaeb85679611211d6072fb9e0ef89744650a2dbc5dd80b2be0caec453b596b9d8c631ed94820acb77011bb6082ec6f" },
                { "tl", "15db84da35c8df3721b904d9909e486f10856d1051b72cc4d3b130df852a341927748dd51d12a08357a5406fa60a3da249596042af415dae156df713e25c0982" },
                { "tr", "0a13c2a38484425004b66a5e23463d0954d41ee15dc1b7845e764bd5458514d1b63c2b04531eb8f4dab93f38cf2d66a81bbd76719d833ad8773c22c9b34d7761" },
                { "trs", "ef7b5fb34c20e498221888ab99e5ebd1535e224d675d8cbecec25f734c288e9eca4ac9e1b9aa87de716ef7aea744d660172051a7beb1c9777a8b7abf1a8a9184" },
                { "uk", "bda77c7fd9e3ee783f41f3fe8f4e4fe0feea5aaeefa243a8e26077b91bf3922771b91cf17a5ae5a177ac3723ef286fbf081c369a8a2e6b8e31fc2ed8541bcbe2" },
                { "ur", "541f46802521be7cf6fe731bc11f5ffb5f6125b1676e1307be82a110e81dbbd8186c007fa4871a0bec5105aafc07c038186317475220dcbd87dedf7b8b55a6f8" },
                { "uz", "092fc007930bdbabd37ced5b93952d39368e1bf97e1811e5a5e16d44a5af63db80bd31bf345c3d522d81f2a358c879c5b109f6c75ca402e4e0a9aeb2f64c998d" },
                { "vi", "42a59fa8ebf9e03c40542e4ff942e9dcc3dd95dd237ef47ea8bd172882356a4517aa08e7f776623e3e763086e99aacce2bde10d27621986c4eceef80921ba887" },
                { "xh", "e745c982d0b3c7e77c9e77a392dfcd7ed7566e1142b19cbad0cf04fbc1b6d9c2c5c3e1bc6a09ed4598764f92c18e06d6e8b50b30c6cc6f64d4750d7651b35553" },
                { "zh-CN", "6a0f4e83bf09ed6fabd9447610c7e7d92291e07872e9040f4ca30e3504308ff7f3a4e0130fbf9df5a33ae4915c11ff6802a4877e1d7da929ac7c048d71b65e9f" },
                { "zh-TW", "80b7dfe4cacbe94a56f805d098eeb39ce528299401c40d6f644f66111057b1e64e196033bf972b15b3e0524b259369e97348be97c5c930e5a149d1d0342a171d" }
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
            const string knownVersion = "130.0";
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Mozilla Firefox (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32-bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64-bit installer
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
        /// <returns>Returns a string array containing the checksums for 32-bit and 64-bit (in that order), if successful.
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

            // look for line with the correct language code and version for 32-bit
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64-bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return new string[] { matchChecksum32Bit.Value[..128], matchChecksum64Bit.Value[..128] };
        }


        /// <summary>
        /// Determines whether the method searchForNewer() is implemented.
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
            logger.Info("Searching for newer version of Firefox...");
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
        /// checksum for the 32-bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64-bit installer
        /// </summary>
        private readonly string checksum64Bit;
    } // class
} // namespace
