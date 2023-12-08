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
        private const string currentVersion = "121.0b9";

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
            // https://ftp.mozilla.org/pub/devedition/releases/121.0b9/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "21a6ca2bbf7f6609e3a362d3ed15437b40cb70a23bcbf3f83cd281d918fd15cf80862cecf544447416738eaf3689baf74d7026b062654805aa6461519310ab7d" },
                { "af", "0077f3cf449f3eb7996fd6371d66c4d34ce140c2145c1751320170250940f990300dc6da208419d03b840afb410a691871ee1247739bf9bd1f16b5c5f35cfac9" },
                { "an", "f7b0e7e61917ced43d199132988d2938493150891e3b128fb30bf06b30fd3b6355724f9d426da2f8c02a039403a234f718a6eac6b07f9c1f2ed14bdeeaa841fb" },
                { "ar", "316584b68b44afe9b0141c3ec1d49db70e0574e4b5132839eb8f2a3feaac9542afb774af13e42afbc4a8f4cfcd36e46e19a39386c0e1de56a15ecbe7af3f25a4" },
                { "ast", "cef3806683f942379f9786da4820550db677f52023a2319ce6a688bcad5506f8a610a405592912a224e401b87584ac06dae79599abbf0425f312002ccf9d943c" },
                { "az", "411259dcfbc501243a5041d9cc5b5c1ae3139c61cd8bd6634a43a3d02301124469982b755d87d7835010dc79d51945639fbaa9d97a4a45b6fd5f448aaf8308f3" },
                { "be", "602a90992499a29afc7abc25a08f1144603ce2f27f1f19cd30e9c036ff95c664272c003f3fc46d81a70e5b53b605ab1769d315b0d06effaf05aceff7e9510a49" },
                { "bg", "5311e37f3fa091be484a80b421ba4ad84e679080226688ccf02f8e251408558429f871322716eb63e3f07b3c213b11e784b2ad70ecfd56c451008b402ce78d7e" },
                { "bn", "bde41d9a1d34ec37378419db964c75ee035033f2f28f46dcc440b97a459a7fde07841dfdfffe0380dbbc67a971af7dc1a0677ec41d5e03c1ccf1ab228439dd19" },
                { "br", "3a13d29f173a1f2c01bf029918ba3a49c0169f9a1d0a397a9fe2e4b9d246d94ce6f35969aabf5079d0c3216ff3055e80c381fe87302c5546a706ccb058a796bf" },
                { "bs", "2ceb605e8a8035dd97102057af2e8890586f3faaec73f5b8dc791147cbd66228c9953d6b0a3342d341b394fca9fcc8e266ea80e2cffc926ddf57660e7d27a94e" },
                { "ca", "9440be74add2eb98b71ea75594a7b3bdc28468a867750ec877b48618249621210003c8674486d1172c820a4d1d1bb7dcb951941fb66320da904f27c69ad9bd15" },
                { "cak", "7834bd78d890105e8c3eb675f64a0c8fd85b8596b13be3e39e577738215ad46c8e51388219c35fe7d3447e7feb0ea1d01523a5fcb27df514001a7a6be0c3a209" },
                { "cs", "ca0b484e3cac921bdf95410c38ab7a539d44cdea9f1a30bffa9a099564c2582c2d95e12bbebd20cf326674bc64f241c72928f7c846c3208a1bb8cfc2e9ad1d3b" },
                { "cy", "7a4c3bab48cd0a1faa1efcbae6ded961d0fd4ce9a1bfc24f8b5203c3478e819c97ee7e54fd811dd96f7dc086334769d78075930c1c30a3294fe08b6e9cfd5d09" },
                { "da", "9c189ac03f9a717d5ca48ed11982a19f96b30cf297b1ec5cb4bcc5d752435ff78176baf60fe001bbb27219286be3d179448cac0f88655cd37c9a92148c07fa80" },
                { "de", "946afd0b3d01a66ac3e9439ec6558dd38d6303bb8c725ea90c2c2136a77325bfdaedc26db8651969ddfd0d53613994e4fb405ab014c1e8b8ce3d370251a04046" },
                { "dsb", "755c5004bdc2fa732ef594890994f0d11892c775d575888877fa1d7d860fe2d37a9c21591ef7427d93f98d9227c8f6bd1c2822ab28d9ccea52f0927d06e57851" },
                { "el", "620fb9978b2887ae870de9d1b5e88d7875529603ddaeb8aa29c6d54bb35248a12303029036b404107dded636efebf3d333625bf2c95648f130f63521ad59ffbc" },
                { "en-CA", "ec0604fc2124e80182158bda098d9f5da9636b4d086e6f4a7f8da5eefa8b5b7170717214fb9655da169155163f772277a3136bacefa24b4a3727e40170720c72" },
                { "en-GB", "f35cfe4f1d30fd491167d9f64e166cdf7cac40fe21d5a81bb473c7ae69729ba0130c5bfe08e8cbb8c7ed903e36f2c8ead88676db54262c5c23fce0ce7de48306" },
                { "en-US", "3345d14042e61df600e9050bb4ff85effb1e33a367d1fbc3b27af29d393f019ddf5b1ffeff2271792c337ddfcc49286dc575cbb90a4aff30b77a87f93147d073" },
                { "eo", "a5009a6a929b4d0a0b823d8e3b6c5ae351cbdea95c3e92f8e9bcd0a8c56bcb155b56d8f7b109a7f7e0279c898a72572e8ea57bad4d067826ee639f56e3f2bebc" },
                { "es-AR", "b9526ac23d2c2bf62f3e53a7cc702c46568027c7046d12542f8fef21c34023a7ff4976bb30283f5a4f3a6b00a02e1d0436b1e581939ac5261f0c4deaa5b1e2ce" },
                { "es-CL", "e76f8e5b9d91b27f27ea5747974681ebefb3eb83dd3cf01ea9298d8f025c2af3fb2f5e530f7d5ac25fab44ca6ebfa18ce62063c28230ccfffbfdb4c6f924c0d8" },
                { "es-ES", "3e3d98ae5ec342b013580c654e40a59769315a4ac920111d7d2bd2b376273f0cfb6b5371ce1da2bd42a5c6bac6126bcf8f344b584c9fa11b6ddeb3f1ea56d3de" },
                { "es-MX", "52fa3b062926920e766440bf0833e9a68fdfe4fe8b1fba81cfc9d1cacfa2467dc76ebc0d3004fee564d9c458b24515b0f87e9c7e3f3a597474cf1ebd41993306" },
                { "et", "877c0650b00bfe77f5313fc88906de8fd16de01814407ab7ab4ca64f3df3bc235490e866090f997b068a013e3792b0519542be7ccfdae251b287e7e5d0796593" },
                { "eu", "648215d22fb16b453bb0fcc3843a19cc653159747155925b57100338cf71463b47af270eb702c87208d532b0508adc508e91c3255e24716377cf640004e8fcc1" },
                { "fa", "ebb9d320ace5455bbbedac631995d87df8cd090c5d9be63cda98feff11aba08d81dc5c26835b401e7c2290a62b460d0f777012bc59c4a6bb819fc87acd0279a2" },
                { "ff", "5393fee96aa397e799d443b520421e4d26e91e04b07451e5de83afb67b65afe84b51bcc5fda1ac79717095d19122dceea54540e6ff88c7f758440348acef4de1" },
                { "fi", "5c8a853842ff05315eced34feee32ff2192cbe73badb56c2d56a2e49dfb2c762a28774d73d91df3cc6c3763c220977393ebbe4903427def23d25e05de8247d39" },
                { "fr", "a2af7387b900f9153c97a356607752e30d1e465e069aab1cd0103a78e3cac5fd56f8a98a463d3e32e63831b11047bf875d2ee60215d4abf74c3df6c127b4dd78" },
                { "fur", "417bfe1532b0a9858fad2838b964412fefab50b3a0fa3b1e8eb9f53974013365bec7ea01f322f69737cd148797d1b7e53ffb4e6286c53a96545d265c8b4d015a" },
                { "fy-NL", "a31fdd601f7f2491ac2edbf69614faf5852abd704c630c1e6298cccfe1028599937cf8b2f5e2d124f57f301d606dbc7616b812079ddece610701f3e99f39930d" },
                { "ga-IE", "f5714284392dd5864dd7cd28c8862042eb30206521e7e812c8cd5891826cd8f7d570f7d4a4896a5d48e59f97100b76ae6fd4c19a6750d0f71737e4c43048cd79" },
                { "gd", "62a78e594c86bc7a43f683bb51cf8f628c5669a28986bf82521e03ddcb71a936a02b32d3587613f31db64405e4c9cc316fbedfe09ef0e963c650743a4586074c" },
                { "gl", "f3f02c58c15e02bb6fdffa162de8459112dcf8dbde1ea09234d9aa8bf02d378ac0eba6b5156632f8ee319c45921b524e7026fd29570518876117a13300a3b9a6" },
                { "gn", "ad70fdc836fd028736c373d907812ec06febf055c4d8ffccad16e4256d34850af5ec27b6caf74ce210a4b1f173ecf1a410974ad7e1fd2afabb1dcdde17f0bd5b" },
                { "gu-IN", "fa3710cf311ae717fbe79f2111013e2af874b5c368a2d4163212f9bebfb410b80cdb78a728b32661777301a2c0b05c50cd05cf3daeb6342d5c941843756a47a0" },
                { "he", "aae420d612ec4c7a86b248f7ca04e5e5f77b29c7981e53f7f858905d2fc100450e4e8a1dfd82a6ef09e6de8553f505680bc051fa7e78056af076973b3a3653b3" },
                { "hi-IN", "7e53311f91148919e098af8b43e08e4a553a8ac401794236cd47643a746ef2f8f8d5b70e5ce26031bb480314df8191b703eaddd5b1f81e15d71ace5729105e84" },
                { "hr", "a49cd9e3ce4db4c1cc58e967a263682789cb4164835bcb2d7bd80848c17b152dcdc65fe4538a09740bd21261e98ed1d12291962580abef5b57a51e09e3d7aecb" },
                { "hsb", "1a4fb5fd03e74f5575e8e29ca58e1abfae5e95390547452372761ac4d06ef924f0923745320013e90000ac2c8c6295ae4abd926e3ba091023d352721103b7924" },
                { "hu", "10f88b6a9d470bf1efcec64f311b88bd58096e6001de4dca501ca0db961816fbaa985583ad79acac3517c18ebb270aa98d5b57b404133f7ab4c4c1659fa31056" },
                { "hy-AM", "a1ccac90e9485e729875febf08e1cfd4ebdea7cd5a3cb90e69ae9af10ca9157d99fd242d74d4e52c86e8e68b67ea9eecee66c8faf045ef634e9607da8e9be6a9" },
                { "ia", "294c720da52de9dc07d313569255f43763ad416f9aebe9ba1dbdfc0cc2e334f66181baa6d9341f057b4d40c98f05215ca947a924e0755c20550ad261e8dd22f4" },
                { "id", "c1b4ca9053c2405ac1c353c7fc81f7e046f1d91e8da41a6f5fe89b4814feb704b2cb240c766dc925ed4018f9dfa176d8fbe2c748191a41c73d9ca08f1dc1fe15" },
                { "is", "58faef53cc3ae4268944eab53e6214f0724b27ea78799b9f933d05941f8ddaf9d072ca1d868912ca424f11db5a2259750563c0664f847a826e3693b1d59731b2" },
                { "it", "2108a992b8d7d8d36b34fc0f10bbc9839aaadc6bfc870d173b396a95420943d6dc9b941dd09a2b38d25bde328b0c0c8265b62a89df2eb29020809ea92092d90d" },
                { "ja", "d5fa9f9c4bbff84783ef87ad269fa1801071aed80d5be3e0965e59f8f819e4e59f4162c9db8dcae2333c27acb9a4c9ff19ad0570a38197ca2526b05c333488fc" },
                { "ka", "856feb55c55964514b6cf09e60a2a205308be859b137c9283c7827c45b055d102505454423651118c71daeb62a47ab44472b1b07299f4657cd827f09b404d33a" },
                { "kab", "aa3036f6d6d4c6533097c6883f541926177ba8c1770fbdcbd103b234fd62847ce1b59d48064c216f7ae061c73ec22f66e8d06b524abe83c5cf9ba417e5de1678" },
                { "kk", "c3fd0386db6925cb60b06ee4f413f697198d16bf4651ee5adbc465feb9da4eb4ad906398fcd28a8f2c3df377621b70852b1df88255572ec2f40383fbdd8cdf09" },
                { "km", "7ddede3b9569a7ceea954fb229830522756e236da3e4e71add8a2901a88977836104e164cabe77fe7d9c37636b1ee66bf4a5eaf6c4413da4ab0d754b96044028" },
                { "kn", "3d319c970d17d0fd626a3a44879f6e3d9283e405d0831c392758a51f6648b926aa95474b8c2246799cb0177391d7c0fdbcca03f0f5a63846e7981b6c5d3135bd" },
                { "ko", "a69859d49fab2b221d4d1f8fbc9ae15f8493971f18b9f31cceee111334e0cea5a0a0b704ed59884b5e6ca260c810dbd793bd6279500911ec158449dd438c74bd" },
                { "lij", "1d5b6151f8f358fa74fef1c7c83209b8d259b4b4f25734878f29eea0791910e30f885779c7ed7cae28b152a3c85430b2cdefe6a5b0900b88be459dd7bbeeef11" },
                { "lt", "3fe8eb2cdd1d977721f373e2cdfb97f5b20970958212d6341a33445ed518c9c7a439f745f3b1378cb4ec90f358e219c223ab2a112f2300de83f938c7367de1a4" },
                { "lv", "0e84ef2f25128c498e9ad7ed2bdc7355538ea9e29708a67d4a8a19cab78e10832658076503de8cc0f57257513fcf3134898e9d4d1a3941d6dd782bbd1c76ffb6" },
                { "mk", "705c8b868fea32aa724de3e81803870df0e2b45852ee8ff62012cee42a3c1d10c577b9dc9f57ccdcdbe54e24069f959ff5c14f66a56513be076085648a179f04" },
                { "mr", "b2401d68433df56aff1296fc525ad5b43df45228e077484729772fb3b877db0895a948268977bb69bd7a2de89630fbd984878286b43a0edc79f0740bd78aa052" },
                { "ms", "6af447c556b874373d2bd2f8308632d62c5f1843916ab16176bc9fabb945a9bff54f23455ff426434ef431627cb071521188c52b82fdb91d561d7507ad8b3a8d" },
                { "my", "83b1e168da72ff7d17e06e3492971b99ea55fd0f2b5a994028d9953d01e25768c1acc5b9bd0838bf1ed3c186d50a992ee7126c541c19303b9a2c6926ff90722c" },
                { "nb-NO", "fb242946f0cc263268035ca35bdb3b82edcb24337b30e189661bcf577e66ff65013e98dd4d1cb6db76d9d5943e9a0fb7569b1ae96fea52b5357de41f5d6ce55d" },
                { "ne-NP", "43e02d53afb022b138b0eef9643847c24a85550c8cd7a3ebed6a621c5ee450d3a7e16c4f0eea2d78af28b8760797e763b520592391ed9670408181363c6c6a41" },
                { "nl", "b098342d5271b6f2669f645eb2952e02138a6210cbcb055b3926514e71cd65e3e1da4772b868046f81c647d2cb3cef849b57c5182292fecec3506589e32217d7" },
                { "nn-NO", "16a5d9cd1182a1b542f41a1e019b7a2bce2a293ab19ae5373206ad3d7081b31ddaf7980cbfeb7e5e113c6016f1e328b322bde971ea9864bd280ea9bff764d05a" },
                { "oc", "58ae56f700e57944cb662c1c9a343ecb4b0c47ca7e3473c923d93b66d363c0c29b66b721d67defa1d0c47049c7f3ef7a2fe12ba00c440cf63c9895d74429d264" },
                { "pa-IN", "352f6bc0f086ebe1b12385490078b1d9a8aa68b9ccdff03e84835ef0b1ec249be8c3743f48ecf20616a6b9541cf3e3d96cca6a5d5ee349c25a71121b742cc574" },
                { "pl", "18ad1e5f38dcb26edef026c48cfce5a01b0042703b82e9a4a910ae6da741351c98b8e0093aaf4fd349c8df907f710414c07b4a78fbb720dff25446d7ac93760c" },
                { "pt-BR", "a3b6e7abbbd750db7d2dbf3225859f9d5d80281f2f2f5ba7cb48f263aa423fc9b09420a5c1a570bdd39de24dcfd1373ed3fde6529da3617efc13a4be8abfce93" },
                { "pt-PT", "e3d1ad76e9c9a506c62a4ee7cca0a0f85b12d89d0ab12045cf6e1cb62aec10d59191f8b3d25d41b895a0759fc1e408a99bd2c88ab9b7eb434161139cf611e229" },
                { "rm", "d4285e56523cfbd4f4b9662348c93630d3a92954b801e29bdb942ef86faa58b7f07c23c3b0d17306bc28604db4db2634ddad0d7218d441947daa7c1b21a180ab" },
                { "ro", "bde9a05ded1c9cc2142f9edaa4fbc0286f4e8981629dd71e4e2c62454297c20f48a8abb0397f75c4de90f98e554db81ae6f10205cdf71cd9b702e1a48650e8cd" },
                { "ru", "0ee6da0b50635d2783b6b68affd6e781b39ebc2f0c3da0672eded6e401d8498ac21215bf5c312c4eed2f0e9291fecc79f4966d859ad2928139cf50c6178a1739" },
                { "sat", "825c4f89ba4583fdb5785a9ea26582c9e4cdcd92c85d8869d932caddd3258b2c032adb1f582ea1a53b1367ae2755ad504cd7ed25367b727a4233c76c7f210a6d" },
                { "sc", "89891e31bfa97ebc7113d5c8a26c95c65b8cc5a3d5691491dcecb9423d70bd28776fa41299a5c9568f6e67c90ebc4bd4b0e3b1c74a0b21ab454fc14eddcf9407" },
                { "sco", "694a4c62332973a579539f10747c016ec0c88debfb94f04c5c050f0bcce95293963dbc641eea00d758099e8edf1336cdd39176dca4afc85e04a06bdb350b3bd6" },
                { "si", "c6096e13d49ba4e1b41415114873491183e5832693cc330d6cb5ef5b9e4c823ad292983cf1bfb3eabc354ae1852242f9c416b0cf5f27019300aa7c88db523bee" },
                { "sk", "5d2d334d38f568b6f88f84c858b328b27bc4d93b101e5bcb80f979d58a414b65cec1ffba5e9843bab5382c7795f360466e3c3eeeaa13da906f6894ee98078fee" },
                { "sl", "39e4ab178a1ad4dae444bb55bf7c6317c1ed5d3319ac50b6b34646122b3a1d2a0f5c14b291db79939c6044d0521e3ae3f29d9d9b131a9e9caa81eb22d02aced4" },
                { "son", "c2fb463190989279dd4821a2b1d03720cd872636077145129401c35c3107a11eabb2dc695da59e47b693d6576ea0c30235e475660866c61f0f1704a7054d10e6" },
                { "sq", "45054eac6bbcca7e871abd34148b01c898f43adb1309952e962fc6c931307278fd961e96746291b9cfd3872222ca9c3cbda765849d2a752a25a9d6421506e508" },
                { "sr", "bb657d9d51be890de820a860c8819adc5f0146a26912de3200837d9755c8f185cc470f0e750384972e36ce3e81dff75cb64cb5bc0710a382ce4090bc509d7b3e" },
                { "sv-SE", "4629074217fc309405ea3c9257a897c6e93c1c770399dde266e28b7e9d9dad0534c9b6fde22cbc598f5dbf393d6cf78939bc751821296d4be50a8a3f469d8aff" },
                { "szl", "4c493f2c38d8f678ca77c62cef97add451de5ee0d61121962800e9de8fedccc82c41cf47897ecdcc90478e238fd623072db9211678e8405ee8374e1eb5eeb847" },
                { "ta", "11ee4d5bab94b75513d38f24fb308f5189ee6685a5fa77d1805dd666802867aab9a57bd5fada01e96196011b48f6519e12b065b86a08759ab81efd9e802baee1" },
                { "te", "3a4d9998ef1a75ac40ea50ea4e88e8857372b5922712d1bb6cd43a173e04f5dc91ef8f2b7e2f84441d3662c83ef0f5ec787ba442f9f9d85af58dcedb584371a7" },
                { "tg", "55c2486c72e67463d020b0674fed1739718227cf89fc0939e093d0d0f589c145e292e52516d4b58c2cf54a7034883402ade91ef715f5325749bd78e603ab9004" },
                { "th", "7520794e151e3df8b39da98f7c63a6f9a491312994829930a354d36619b763f9c2038bfa00c8e5db1833c48a70f888e04b65967621fff5e87ba9c4a14792b8ad" },
                { "tl", "4dac49122fa0466b5d52708a050bca59999695934a9693fed39582277ac0761a3cf5127be43a15fef2812463aec7b0dc6a5ceeb3452552aeefe112287627dbc0" },
                { "tr", "cae6f533164c9b1a541f9615699d0b5e66156d7322a8d2bd17a6a7dd7aa89d1548b76c3280b00cc8d1b91dce33db62325538e4cb1dd5b19ab27a5b5461faa5d0" },
                { "trs", "caa65a8618776b1df0646abcdccfd5784d4f9de885fd7cc71b08137a55d64cc55f8a39075ce3d32c947fe7f5bcf4148ff6b8f57ac883b9720b928b27e9478364" },
                { "uk", "b1b11a49e470063c394bc3b56325696f557380edf1aad53311debf72716ad948b43d7c8139ec3a50f98a606410e2e19a964cca7c1927d7315e9503fb6943c894" },
                { "ur", "fce79ad243bc706279af474d678c6eec12dadb0a17125bcdfbc5689c1bb53ba5a05ef376c6a91fc1fa4fdf4e901c8a0a752342dc86dd26c85d9749ee361307cf" },
                { "uz", "fcf8e5b45ae551a4a3c6558f626c04da5d1a352c040c73e5580ca759feb99e5510fe3e4a2552ea3269d801bfabb7f247a90a228ddb4702cadcbdbb705e944488" },
                { "vi", "74671cc3d10c319b03ee982d3a50d91f4fca8537ac4582532da2e9eab84f3e51a9325e58236eda979f14a6b396477552a06be0e6bf1d21ac1093e97d9bbc4b09" },
                { "xh", "2e62c9e7dedfafdd6f32cbe686baf2edee2898bc38471c99d856614b684df696548aabc0e0deac20e11ab3de294667f5b4d79f8dc0c777210d79c66279e1a3bf" },
                { "zh-CN", "9aa6878bc850f5ad573c74e80b7674d27320b5f01d3c3afdbc836c2f1d486a3fa673cc7b119b14379fedf4f4e68bef3d376774827ea5be6be5bb8c888cec40a5" },
                { "zh-TW", "fab089b4c552b0fa371b8a6452c454c890e0e18b7e788d0e2d7cec9ed28b7ce8920f8856cf6b8666ff342b48aabb7e94f2bbca974c17cdfedda9f9953a530a66" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/121.0b9/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "81f6c5958a4bcd754ab93c633ceaa3dc3ac8b3d936019d318e551632a02f038cefe44cfd6a5b89f705bc58400f0673945acbebe7bdeebdec239f0abe3d89112a" },
                { "af", "eec7c6b7e92fcbe44d80290c36d6e1b7f204abf91668db32239a04b4ac43efb4a60dade1d8f0967a13e2f9faa9834d337fe68692dfef765359e6d936c7652d80" },
                { "an", "4bc64cb71a30ab176b0fbe7708cf4c28b95e2177f88a7908ae6868a5b06a01f1e0ab9f13eeabb2cbc83f2f921bf45100fa370b5a2be81b7c993527e1b3377ec8" },
                { "ar", "8c08b813d4e15e81584258ac5a5082b7b010c377fab0cc011e64635c9e7650b03aac188dc46acec569f294d6aae14805d33a08ad8cc26d49009c02a389621d5a" },
                { "ast", "ff64e56a711744295c5907d022c08bda7e755cfa10805fb96453dc255d56b522899388101ca434baf2608365ab43c5bb0c03ab3db31b69b532cc91e74a340235" },
                { "az", "cbe53b502e1ffe668d1029d85aef0c4f869d22be0d853b06709693eecec7f32fe59c2f39798a80d4b46d5499d3249a6c2f6215695c0929b7bde7ba84c4e3551f" },
                { "be", "22c21d90d26f7e5aeca8e79d02d4642bb167f11d6e420e709084dba8829bba1c15373fc1e8f7dccca7a06d01c64da8d08f08cbf49f7056eacacd371e0a2931bb" },
                { "bg", "2f3519f93e5f838964b4e5f09f3819f8da8e30e5f45fab36513b5318f9a1555492cc50553bc3447a9ee4e9c7fb2e86ce0e379ecb89d77a021bd0deb11ccbc3c4" },
                { "bn", "82149bcec76932301d02c2d905356daeef86361c72d1f68b7a80951c6ba2d6fb206e6f5bd8c36e8c44729da714343cce3a3a6a154dc65acb655fc2cb96e0cd20" },
                { "br", "dfcbd0cdd562f02d0c3937c5b2b8a34f9b5296d3b55d2964c366fa252804ae31a95332cfe633fc11787232569e71923441071c93fb70ed5b43c43fb8389908ba" },
                { "bs", "85f10e5d272b3308d052e75e3c1acc976edaa4b55a6e77154c0af1eb726be59daab4c5c33bfe8128c50485e77cadc2f857900fbe7f1a033d12dc58213453b409" },
                { "ca", "3000dbdd1cd0363fcdef55b4a6e4e2b0d4ae0c1de0926fee6f49187e3c4fdcc1dc7011443fb4b55b8472c502fd4cc354392b75a0233e7da8ceb035f288182335" },
                { "cak", "da5800c3f09d6450add0874351371053ebabdc3425135618f35eff431021160cd86d2819c2450e5b58e7c0f9ebae2318a5351f8c71d84e7f0471bb949add8525" },
                { "cs", "0c6cae259c996d0d92d93b9516aa2428a8286968f48004afe83958ebf68fdf1e86486742e7af62563d02869498c9dc8f5d25b74bb8acc0ad8d60924f15494588" },
                { "cy", "1dc6455e94617b5be6a8525d0dab74518bfecfa00c5819f9e45292826e6eea42d79fff83c802cb815a0981df728cc219f11f51b6ecf15e215f7a7c90d0617f63" },
                { "da", "c4493d57c92abebe3871fd82bf8c739345999c3b349dc5716156001269f6c49b40a94f7eba2b374b3a2535fe26cc6d681abfd985109dfd8e543fcbc485f74725" },
                { "de", "99f6bb53c457967a9802745933f240a880e282dc056669d7a1bde46a02af64da70af0864de266a13d787144aa9ea8f0303ef3d331625950fbc4bff68ca44b5eb" },
                { "dsb", "04d996b7d579403040505a02dcefef03915f8fe2b43cceec79e0dc855c961607fced3011616e5aa1aaaa64b576520b46c5bcb481d3e492f9f36d83206bf91804" },
                { "el", "d82e753a606e0acf57554ebc3b638f94e8be6814d6ec4b3a8f672c32448e4ab671eafdd5ecded6fdf3ff657ae0408ae458ac3d67ef038c5b453388fff8498588" },
                { "en-CA", "71104cec96eafc44dbc8ebcff5fd6de822115e51204f0f4fecda59a5175e55dc6da7dd63bd4b47fbe9607a473c7bca899d4ed8a8f829bcd4d7ab3af49671e651" },
                { "en-GB", "e424d848c210f80918ae236c27a17df4b139b16f93a351d249cf4542fd4224c08e682a2de210ff284dea3a594fc2834a6d22c6cfd659df282f55ee8c09c3d137" },
                { "en-US", "215894dd0b877866b72537a76b73f3352b3ca571b264963d8d407fc883f5a57e97d62da39ce1c5990a72323b79cab5b0602ea940da2fe0b5aed113de270b04dc" },
                { "eo", "8c59bafd7e032af96628b85c84ce818f0b121b3be80a00c32ff0d1e10d6ddf1408b685d94aca8d0d0378d1772c05dd63c5673d88cecdf3c063d23e91c2fc032c" },
                { "es-AR", "3886986c7de4534af38997cd9bc681a0d4ad329de6acc025c8311fa639577ad15c849aa4faf0887de7a176726690f5210729c2558e7212e62c7d7d0ed8293b13" },
                { "es-CL", "48d78af8a57bd49707def1de81589d0a9101a0aae754aec7ef9a70e965ba1b2b07b2ce3d4ded539fdf25d8a6a5625c92adb309671ac4c58c1d59901aef49ec1e" },
                { "es-ES", "03289e37da1d418f278cbc686d177e2235733ad8884b881782877eab1d91b428e09ee7f16bebf764d3acd216d8ea322b9c486dea01ba59524dcc7d29534678ef" },
                { "es-MX", "3a4770787e1f51880f946afb2e4873c1d12ac6d923f1e4c5539b263b7a72a380ae00a334888b0d69a2da4181bff0fed5b82b16ee87cd00f19748cc87c39321ae" },
                { "et", "bb8aaae64045c814a494a8c8515e8c9a014a1c39ed06875c663a9f99447f9fbb5d84da28c3c4ff4d0589ac20b0cc216a4f6bb788c0da0b0a6ba0f1ede5389f26" },
                { "eu", "af879c12080ab5538f781cbe1bea389ec3a0fb11dd823d9582450e3cbf39f7849b060e0543a1a944c0f50abc111082bf752b91e0192a6c0405049659db92141d" },
                { "fa", "64e7ad7e4cca799504a8a2725bdcec529f8314fbe411e8fe3dbdee02f17e4dac1f7eb88611034e54aa4ab6e7856cbdf47f853c5c5699f24546ef32facd8e6436" },
                { "ff", "ad1f4c81419a4e0681e7bda8cc7bc0c717d462a332b1b65fce640ed90a9c0d63e69c1aabb547ac339c1a437ca24cbf193f0e53cb0475bb58e06310ac70737b82" },
                { "fi", "b0617fd0aed4b622f22e59b05a8999f2a4470a911fdd917d6908d5c71daf4d9d0178c521a4277c7ea11cfb1f2aec8f439422dae03dd78de4897d3388b5d127c6" },
                { "fr", "4365bbfecc6a2899e8dba83ee912797fe2b32353aa811a25a001f2935646dd25b3de36c6a818359a1c9bf263aeeeb05b414e08976ef066212708d5ad1edcd9e1" },
                { "fur", "bbb769d467f4bc4e8dd4f1b7ace9fbdfef30bb9f04b75a796cf01b30d25207b9bdb7eb2c8a44e69ea2d37c02bb8fdf577dfe36a8db808f9cab62252f1c304c65" },
                { "fy-NL", "6c9b1b0e78a25cda229805f2ad981996538529398d2a2413f4451ada98c4579636b13d3782d8264b9ae3958995da18358b335b459daff5d071cc38af4acc301a" },
                { "ga-IE", "74dd77a5b834901a8f7f6afa30aad097d1597dc2b6396cffcc8137fdc11497c3b30841bb87a1c66edd8187c01bff0f906c5988b44cbdf8a01de0a08f4f55e43d" },
                { "gd", "f08119f66967910ab7144f754860b9fd4152d2b212d2147713b83f6347c6b0fbcd3b65e54970dfa2f99321c4b8fa5c8b2f740bc9ce17f93e38e37b27eb0dd039" },
                { "gl", "06ab40ee0f9309653eb9841d497db657480e90b27563668b64dc88c866ad9eac09089386f976e7afe65177a510bfd6f9b993e110c9df888315edd54e870cd745" },
                { "gn", "73071334be51aa567fd3b7b04c5f87db009808f0eb0080d37b4ab0faa6525a10d21cfec30c539aca5d94df0d8910cf96ee1f665fa7a4afac6107fba3c4f235ef" },
                { "gu-IN", "4eb198ea5b9df7cccf63c7db3f0e638ee8f6111f88b35545b5ab21ef7b819441b2c3b3721881ca0c7aafbeec7b84925892fafa55ee6265ac36abfa1b82290163" },
                { "he", "1c2c72ea80ab8c74c56099d76f8aa4f47405df301aba0df9f77110817038694dffa5738613ca24202e9980789eb4409812e472dc595c8a5a8381613f4796aa63" },
                { "hi-IN", "0474358eff3e357ee1ea3a3d7918c12f1cb00f043f2ff74ce529cbf2187e5ea1d5606ce3c7d678a55bc3d08b5a3ac35b8e6cfc383b6ff025b69a3ecda2867bcf" },
                { "hr", "72209dead849a6c987e394053d4efa20fcafaa386af07a4de3baaa39f570deafed795858e092b929d4a16681c1834e1c59bb95cba51e0a99d780fc83dda65cc5" },
                { "hsb", "4e2d3e0bf1d4458a7d711b2f2ae2998cef57943c85acdc6693785b41bec7b7e772b024f00b6fcacb94a1fa4712d7a257a51d74bd6f45289651b8bc01837a0b6a" },
                { "hu", "3fd069249eaa30a32798c15c7c81ce850a3a8a5c8914da18e7e632bf77e18f8c3f84295443ecfdfc35410f7de2c181f1931118564e1c11a1f88b305a72782087" },
                { "hy-AM", "b53ec8c88dadca7a96d1eddc8eb1b5b98c1de2aa210c9b7ed6b1398a00a13e05579aa8f5be8a9863a3937f988f698fb468ad04db511364be41401f242729d3d8" },
                { "ia", "76fe5780d092c4b456ab3398d21b4784e489b19ed0ac52892ae723b05b1b66e8ed2816390e3cc7181e1a9b348c9c5f2e21beed626a2cac34b795d7284a1332b3" },
                { "id", "c97ec41381c480232841805697e773783e0d684d9bdc4fa7e5dc90eae35418c617ef481950b9a44a856f50fcf47a7f1f60108bdd7eed3435647cad701bcb1950" },
                { "is", "5f1d0598341246a004533ac490f224d9abdf653d77edb5efcf997f70d7385459c1af6df43a96eca772184067af04204cf1acd8e87332cc3ab16414c628398262" },
                { "it", "1a03ff13e2e9660e0085abbe32b2fb19f3fad0a28da7567cc72edba07e047b02ba1237e47bacd0893d525317ac5ce3003e4167e56d9f41dc91ba5e3992d76f44" },
                { "ja", "87a207439dd4b13a9f130f636603aa682c3d3d042d71e3b89a4aef9005280a792e758c60a07bab4fcc4de465b86a0807a58623ffa1d91da5387c9d21903a4237" },
                { "ka", "4a2f0f9439d35577eb1642795e335b3241b319a44d0b7e3975032599b706cd794053fa951ac0b26cd33ea6d072f2d9c22bf78f79b1ae7c7f36b6441f6bd10334" },
                { "kab", "eea204b9f5c30e9c9130c61176ad157f3fcd662162b5fcbcdaf6149d059a89e0f40c7f1f2e6b09c8cbf0ef961eb00efff7b2812cf053bc6298b69c9a92d2a4cc" },
                { "kk", "603ef3aa3007699aab2b1364819884c12289157e255f4f2101d1d2ea0110f0bb86984b00973f099676ba461b41da231eba979682847eb40425464ff5b205f8fd" },
                { "km", "5c2ddf0bb0d7f2822c716d0ebc11a0d3e29d96d6f2245379b5ef59e7233c46bcb342ebce261b40d2d5723a1899f28ad82a519f694abcd35e35af57f9ce86bd47" },
                { "kn", "b53408cac6a8537106b264752499dc4ec069c4e433c3ee72a9c1502ab8a6f3a93bf5d566263bbf174580198450b6c8717f423e63c29f1213e09cd062d64ada38" },
                { "ko", "f1ad447a807bb093911968a1c441af031c3e098c05c74e59b6e64101bc1dc38cf9b0e253585e1fe98c2f503c8c040f6c3f5d6a6ee8ac80e09a1b4f44e7b558b3" },
                { "lij", "21e86a7d9842fdb89191bf4cc273b8899b4348e6b0f12c5452bd57992b27c545828cdc3fc6afa57b75a97b971bfc4ec8bcbae6070dadd00b05fdf3ab5264d49d" },
                { "lt", "69a188dc3298ab82ba8dc85878b7f99914abaf8ffd4f5893c06acb266059cf29acb19390f781139e401f0d59ee07e19ffd3763c7a3167070f510484e11a3db55" },
                { "lv", "bac2581685cffbdee3d2910be987171aa4072fcc678ef05ca4ae6162232dd31e6f3ef917ad2e5d3bcc584ce62e902d1b978fff984f7b8b7768299a3a9e28f34c" },
                { "mk", "716187578e229c320f9a7a6f26b597593276142d79bcf3deb203a75edd486c77ccbacd5c9a1df69112a709d8ef6ba6ea6e5d67f3a0556583d1b59c458b7d046d" },
                { "mr", "0699ebda1442b49d26ac89092a7d18e4c549df69a18a2b1b6ab586026edb9883262886dad187622decbeb12bf6f3943b1dd569ae39b8f5ba0a648d1cd36f6db8" },
                { "ms", "5f718afa7aaaf26cd3c24356a306ebcdceb0ffd5f9e1ad9bb8a3f07c4f231adf3d1816b60333fbe248de30c02be6eb33cdf916a4bac22918b0d8135833468096" },
                { "my", "9f4a8e5f2a182a129a618124acf9feeabe271d67bd0197338886e6ac7c4d162252934a12ee5e3975b02ecce98ae9ee8a3532ddcfe2b285d7e227a1cbb3295002" },
                { "nb-NO", "9a894dd34c37c0a6efbf3ae586ad4881f9dc835d703c283a6c35f13484e08099bc68926ff242a9be434ee62576a602a5644aa83b6b9394408ef8abc5e26cc496" },
                { "ne-NP", "4aaca01af7fd7afeaaf36e8759bf7e4db4afa02c097600a7aaa43bdd23eec4649e62ac3b699577e06a9b1a819239fa947be7d1d7e8f14c55fa4ba187c43657e0" },
                { "nl", "bcf78db50418a4bdfda7d03978339698393bdfca8807283b4e3273d96a3587f7afd3edfb165640fc5193b8b00f7eb50f243eb0cb6694e3750e39e9c15a6f8a91" },
                { "nn-NO", "b7be71a887e544a3d8692e902ae30a03b423aa4062f5f7f48b5033a22972385fd754e2d0f9355ec71d645b75a879543b6872d8ffb6252affdca2c9677bac41ad" },
                { "oc", "595bd96498f5bb56e5f3c0c024bc39d315e26d5367a6b13081d40a0ef85cc69638aedfdceba3214a81cc74841c607b3a7cd00f72d5f538cc8ed7664e5be4b2d2" },
                { "pa-IN", "1feb99d1a7ac1dbe2ebe4686ed395ece19e27cc967b3f61db3c64d165270c1a93eb61beb41f143392f3f1cc109d22c042e6802f8ad18fa6015a1c1c631029904" },
                { "pl", "fe0e0b505d464625dfb470a7aedff194108d54f9aa2bcd9c86b2bff15d2a4b59b119673d8e116e51b12bf987e4996094b5e49735f12faccb33c83fad8d0b3413" },
                { "pt-BR", "f179b222d1690c02ef3f4d245519cc6427334ed2ae1901975cba48d5bf517beac62354af116e69b110bae015bb1025be4285e5c5420c1de615d026e824a09335" },
                { "pt-PT", "5b031a4a48fcb5ab416c7e1782d18cc099deff55fbf5ee13b3be295ed3150996cb65b8a6c16ec0da543757cd2cf61251631ffdd1618ddf3530548169db0d3fe6" },
                { "rm", "50e13a71da174f888dade11debddce3dce641f4e6db3909917de1fbed8587d1b114bd30b5ed3d0083765ea3513316e083ca0351ef1bd8252477cf526a873fb6e" },
                { "ro", "7c31f0e0a9413610a312b6755e37f545e17269e44eed89db8a699f48cc17e609edf0494a7fc472905ee561f79d6c488bffa3c47a555bb3a96275adb83a93e651" },
                { "ru", "63fcbba528ff903ea34ece39c0ffdfef9503aedc5e3d60998ac0a374e9758a71e70ab69c3b0d9bee7c94c8f9048e3239fe5fb33786fa2e2c1ac9460d504b7d52" },
                { "sat", "cce00090b63095abc6709ba4b747ba6e6c403dc1e0022b188513d2dec7a424fa34bb89dc581297fb302f4917a88c561d4c814b120a911421c959d5fda6474971" },
                { "sc", "e4714b108745ee1da77b9e1c9ee472c546737504666ae7238c59fbdff9095fb229c73327d47cc6f9214669c3d3bbe58a7beb685617046507d99692a8d9513a94" },
                { "sco", "2a2ccdc02069c8d0496b0bdf4179e9d67cb6909d380b7c024c2d5a370aa9cbaa3c37e719a275c7c6f5d2a8b87e7c882811195caed52c7e61d944405aed1ca49d" },
                { "si", "cf581123a00e07e10ef0d60b5f0dc86ac7a4ed7d08838f9902d099000306117b2b64dbce7b7e0630da8f67f887daa9f8c5361079bd2450d084b0d049b4c6a870" },
                { "sk", "11587be6d2a03d24932e331768bb642376820724a6ba39338639f43fb7d67e2e145d727871cb7b8b59a34f96a7ebf563babd0155275be2a678264e0d01d6ba93" },
                { "sl", "62aa35cc166fe6989f2864bd527af2c13732e4fa14d878d3e6fccca802a4c0d08c4d2b9badf3f8ff032c81dfd23c8f7c57ed03dc7521a5710ab34a15c4b085e7" },
                { "son", "7c4f2b30079867bd25491eb5637e34c4cd962abf79811ede00fc77bf6cbfae72d427320d24fc7ddd02400e2cecb0be95753109ccfe2088e88b6cd7b001c36373" },
                { "sq", "ad3fefb9e918cdd97932170ff6f5fcfde214fe9056cc582864bc2751392a87ef163c815cca99f1ecb8e771f9ca541adbadcdc1e79ec8a489d443b212c8c65b1e" },
                { "sr", "44972a78fd39430b39ddec3dafba9ab2654d49b3c538dbf3a1abd67353505b024eced6afc65e5b21b6179d03c3f678a10fd29a420ff14344f56415c2030a6aa6" },
                { "sv-SE", "60f5876ae51ed37bcf9e6176e3536afe19642a899ef10b313050802c9935e94eb93b1f3cf6482eaf127720313b7b8f3972e7fd1a0fd29bab2cc356e618be6419" },
                { "szl", "f201b8264a1ae56e611d654b12484ea2c89c762a3444f493ec526ad674dadf4a88084d54fab42f0abcd4faed1c5f5fcb2daa009e199baa046bd7b86dc09b204c" },
                { "ta", "32fcb75fe0976e0fca3e52806b587cec6d01709c607495e5c9f545f28294efb2925e98625bc61cf75a6001142d371e7417a348326f401c06ba081ad7498376df" },
                { "te", "a7efdcc5ea540ba457461f640fc3560c35043275784aba965a6536511af41254704f71ab6a6a71a9fc5f43dc58881799efd359beb50bebf1aa04eb354a501ba3" },
                { "tg", "f202e0c1ffa304fd81cb6984c0ceee8bbb1a7d0996a917b574143b32c0fce85c298f99745207ef74e2a9627c822da7ff3184664ba7a31ef0e2cd6e6a97b4a6f8" },
                { "th", "d6bf37130e6f34978e7ed2094e52daa5260e93db09ff419351458c6e9370f800b3a39e51ad43eee311e4acba142896e7b7194400395b7c184e3878f5ad50365b" },
                { "tl", "2e3dfd50ef532a21f1e6f3bd6931b1f96c9015701015e1566849050d4bbcb349e01cf9aa5535751aced31a9981e3db1a38183f86f8434688784b0972212e4de6" },
                { "tr", "bc3443a2ac193d4d43260966c53f57a621b3e9f61aece931acf2f2c2b1e614718ab8256f7c2ab2657c40adf5d963100df37d35e5db8153b212d14b08660b68ea" },
                { "trs", "a946439e1d2b67b3d93daedf5d1689ebb127cd975e9858072d6c3ef101808f5f66c7cdd56056fa8c241bd5e5cefbd0bc1791dd04ba00446084626f41891c9c07" },
                { "uk", "d5ea903f64f35b04d604cdcc1c20ebd20ce0f9084d7f1a66df40aff40969e3673cfe90b37238abf25bd3fe5dd2cc01dcd32be2b67ad565cbff5e7dcfd49b4249" },
                { "ur", "dc4b22674d032b87a382dbd058baffbd17d7a274496d1772030385212f53d9039a3e60cf419c92263fdcf4c80018720257fa86f98735807e570365d4cc1998bc" },
                { "uz", "db3dc0f9513c9812c155011c7043385e7cab7fbf6f95d8d3e151aca3b5aaba95e19da7824e953a54369db89c45ef3f114f5d78a7e18165e196de90a050f94b11" },
                { "vi", "7ed3363fd10425201672f11bc053fa0197ee08638ca489af19306a8eb52a0f40aa04a52ade883d322d5d21c3e2b449760f6df90bbe2239656c0318c8f29c629f" },
                { "xh", "e67ee814e905267071d452c551863e1302f3e3a97ac712d5be5fd5d9f94e3caec6696372cd1b0a4005af1735c5c4e684cef037aae47bea8e14b7dc34b0e54ce2" },
                { "zh-CN", "06bfa60cd736876f1bae5edc43fbcfe0bbe3826bfc28e642aaed4ec614f8b3648d7f4be28d13d019717a323a56188bc25c0133a29cc539262edbfd4becf6dea0" },
                { "zh-TW", "69ded5092d263c27ad2651c26316fac788f5881c074626367354ce6013464cf4ee0528a263a0f656fa89c143b9e8bbd5fa09d76e615f5c16d6ecda44979efcc6" }
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
