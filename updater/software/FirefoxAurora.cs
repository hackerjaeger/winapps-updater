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
        private const string currentVersion = "103.0b2";

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
            // https://ftp.mozilla.org/pub/devedition/releases/103.0b2/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "ae62aabf58b0f5e8c7f80da6b06223265a33c8bb902d8cd60c8671317db543d84bd72a1b791b6820106cd4e76e64156cdce07be18024ade6590fc841b700b515" },
                { "af", "5217006fcd6b8c2b0107344e9f3f95d9935a1453eeb6f8cd16caf59a45a4feee8eb70898ed8cd079e1f60ce97482da9b139f81f50cc4df13e1c1aa6fce8d54b3" },
                { "an", "fcaf3d6dbf183818c7c9c107e62580b8302453f3fabbd0705335c3883f019feec10df9de0467b28a43c86f56914dba0d690971bda8e92fce510cc5d8dc5af553" },
                { "ar", "f3f48a66dee5c62339c0d4357d9e1d6a24b7b7c6f19d69013ce07840f6cfb11cecbad8206fb73f212814d4e826fa8b8bb3d20dc8494a5150a963c75c569cfa09" },
                { "ast", "60df4e727861ad18bba174d1f68fa2e906fc1e09bcbf1899883c8892f8d6251586b86ccf72a7751d10f2263d08e2fcd89849028dc66281063938b37149276e2e" },
                { "az", "328b9703779ddf040cef8486d75ff5233686e97e4441faee66a717e8d609a4c6edbcac6eebd93cd331f56391e2c29691cb7deeaacbdfdbb83f366bf599a6fc01" },
                { "be", "e85233c53fb97dd76b21b47befe9307aecf241d85c73d3623d436ff85d480413854cd6b72047d67d708f0730db35124b1e7cdaee055aea7fa829e079e633fdf7" },
                { "bg", "bee32bc4227dedf2a0995ee5e9e1948f4e37962758d5b035e0fda235313a63df628f28f92ef0bf1f09083ff38b36c4c2d067d58b1289e9fb890174c88f45a8b4" },
                { "bn", "c077eccea1ae2da91ecd5b8b47f11a2417ffedc21ed981605db9e52c1ddf0150b911ecc2da7759e090a149e3e75f4e7296950451d0c629c20d57e19c22228abe" },
                { "br", "ae8934acbefae6adaa4ff58dfdb00cafda01df613eaaddd006248a486eb6d7988d1e43f39634aafc7e28c6eec72d70143b1c2d160d504e9b92d0bdefad896eb7" },
                { "bs", "d44784e4c1df5f21bcf03d76146c3f3125a28619574354da93de16eeea2a8db6fd04e23242327bf8bcafba87db3224947ae47def18e6748c21ce980105299242" },
                { "ca", "248d7facba65eb34d95b65c5e0c072a49b92e7e18373f24c37c2e9d62708d834271d2f5da0af179c9da76cdaed7862cf46d0c4c18108e9b15bcaa5b8d955317d" },
                { "cak", "e4def2326862582165c40ac9092c5614dac52dbbb47d171c6e3e9ccc982dd0b8bcd82e0352a0648dbf9a22fcf25930bfbc658298098331f62e8f99921105a9ec" },
                { "cs", "866ca89d74ff27a2c62bbc25ae611474c68a5c87fc5d4a7af1fdec5d4176af3bbdce824ecdd68c422084d8466929583c472f96ee9aaa8f750906da9ed3ba9102" },
                { "cy", "1e65c46fb8dec556e85d1bc2d9ce436246e69bc48003c5ba5d1d392030e44c662294ef181d8c7318c29ad1b567c0c7511582e03290b86b0252237b4fdcd5c69a" },
                { "da", "b04be8f3a9939ab4d4d37d36d0cd803087769f1bfb23ab4b004a9eb7f5289ba239eb1d58dd1a7e2fca27ec045df0a768c57ba6cd3577f7d85355fa021bd6a238" },
                { "de", "7860cdbf77e1132fc081035cdae738b087b83ca33b78c2ff43581c74cc3463d38c53f7ef54ab227474909682d9d8edac92cdfb1415e5df168cc95fd754a7dfab" },
                { "dsb", "4ee82152a0fe4ed09602912e6b58e2cb084e8c88ab364cad98987fb5d0a6c7f14b66ee9e8439c1e46595af0c86eed963e23a5e0125e3708e8418fd99e16e89f4" },
                { "el", "02826c806d05e4e4a80753a7d40e90a323b2adbaaf128a87a292f9d126638c466254e63b7b3bc4d21112b44ccf5b86953c7b579cea153dcb879b3e2da4052be3" },
                { "en-CA", "589774a01cfe7fd311998474e00a5ed6242cbe4e0dc0a7b83bc960631d165de2a9b8215492e48363f35f3b53cb96709b090f802d2558ecb4a6193f2967161470" },
                { "en-GB", "08125501ba60e0e7dc92a8bc0269b0cffc6b8a5e19637e950cec0acfe81683c7cca0d7722c5214d2be8f22151d7b369d36c3bb05b50d439e10f0b574d0ccfafa" },
                { "en-US", "9fc7151f8a61a8d5e1b6655f6a6d588092abffe2aca801e6042a9a8599bac138f303c8c28261f5dea9ca1371465f4a9516343442fbfe4f20363f9f9bd301219c" },
                { "eo", "32bc0fdc49626eff6e678c8ec7a793280c3b4d1ef4fa47d0d9fbb0df3fd72299ecf6237fd6fceb31e4cebef13070b5defa345360399f0a08062e9b0df9ff2c72" },
                { "es-AR", "3d82bb31ff38ba01f74c7c3a8faa81885f36a3a6f6bd367f37394ccb1f23aad26fc3447a376452911f1dbb098ee0671b1f707e77e4fd2bc4fdc9f043d64d5041" },
                { "es-CL", "ad906e97163da6efeee8c8b4944754736578dc127a4dd75a63de8e8bae95c04f7e0f2b36f80e06f6be189092cb3c0fbbb8fbf1e16dc5125cff1edba3d779b412" },
                { "es-ES", "cb54fd23b312aafbd417c38f22572b664c3be18ab49486d3554b86f1a59251d1e5dde047bed14a783c53093c8701223c566ef47f06923b2d5590c958fb9d6a92" },
                { "es-MX", "e1735735251b94161e9d78285330ae0e9270bc0f97a43226e14a708b9e183c357263a20ebced3ce9d813d3bfb68926da23ba09de527ca24b51d6a0e6362899b1" },
                { "et", "4d7ae731814cad07554a181905993d0e30cd6116fdfb139b0ea809815c6d05bce8aad186f127c69c77210774dbe114fe394b65ff134a63d122162570528a7a73" },
                { "eu", "bec957080c098d17c2a5e1f66c498e309b6aec6f41cb70daed2f7c5cb90873a8f4b0e52b7650bdf944f8466fa638ee0f80424f9a48659a99b547048b4423081d" },
                { "fa", "b60db09bc3e9f9e3593b0ae77b9c3f9209c44805f5526d142780e04ff9c44a85fce1950bae7649cae282149f91d08230eb387cd47d0b83bb494ff600a8a9ecaf" },
                { "ff", "d2a1b4d3b35adf37fcc3741af67e44b0bc497e2e3ada5a20b8991dbe19ffc85fbb93127075d88e591b029a363e5d630ec6feaaafede45ff7e03e0407a1f59058" },
                { "fi", "42a9aeb515330aea87885711389445f49a7dc9c6d6e517842e0f0f274547062bf7f4e2a41d2bdc70144e1a02d279bedf77a1948ce88fded6a00e15432ac697ae" },
                { "fr", "4c24280e8e3ea985ea7b3d4be8d6dd598e2a1563c96492df144d5d604e74bcf084299a40701db3a6c899839749e02a08c6bac14d1746c1c4f4ee59f30e86fbc2" },
                { "fy-NL", "3f93c2fd41ef02e2a373e7e670bac8675b22a6c62fe67304fc6279c7f5a1649230a59922db0cbebc24cdbf0742f8561b923bf703bdb4cc48b37c2a8c544a93ca" },
                { "ga-IE", "b2413b1d71d619fa4d69c16d12043cbe74a22464f66188e329dec01a4a912ddb156e844975668cba661a718b1ba61277d2aa0bcb626add82ac537a43f20f5040" },
                { "gd", "6667fc685fb407f1c79b20cfd07d0f16396619dca01ee43607534724bce5876b70b5f8772cc8a2f4a2b8c3c1e75e99930c208d98dda39d7d689079649a2e6b3e" },
                { "gl", "651693c55f808fe82fd547869f964f065f6c771fd4b398ee21e4543405a289c493104837e6269d39d4a9e815e99ef03cb488b5b5d2b966034485dc64f815127e" },
                { "gn", "8754ba4a4839c1540edfb8e9b3693e78ce05a45c636d5f2350bb8ecaa81be85916a31557409950a55898572ba43c12b10bf2c91abdbc5667077dba7b405ca241" },
                { "gu-IN", "e5a0665e79dd37a25cc2a77894bbe14b304ba0903ece17a73f110217a243b9e0800a9932eed84cf04dca41df48eb89b94308b7bae3a2ac3638f80af4cf28a223" },
                { "he", "d2fad6fac23a602b949e675e96ff90e15c93a9e686d189b054960f48f5711ec58982b9d18e6f0629337a0946c0c35855f6ca2f32df4e80477db9acd89e3816f5" },
                { "hi-IN", "3215f916b64778c6b211fdc1a4d3dce302e11c7ff99bccd03f6d5cfce145015f479a0304204cbd3595039878bed75a46ca0790e64ff1dcc7481c6ef631b8a298" },
                { "hr", "9f358cf0fcab4845dd44288b97c805c998b77cf6abec3014b58851bb832b3dbc6b2e804ada523fcd10fee9aacea4b46a23e9239f6235530478a3997d450a4885" },
                { "hsb", "a4711b04a7f7e3b838f956490f9d1a9b0272ab513824dc95ca9ba1ee21f7301e5cacf41b894c7036dc17590bac948af16de57a9a3a714f46c0e17705f656ca89" },
                { "hu", "541c617cfb51b476f9108cfabf36251a673bd63bf68c4717445e8296c59a0b16cf95762a659d90303a967e407e2a5c3222feab16528109bffdcb09ecfb33d882" },
                { "hy-AM", "ce55a630a33b14a6288252bb2fc60738878717895ae8a1f78d17a3847763fff52089bf256a7e7c544922fe0f1f2bd8c30efd1690eddb3e4e3d5ec33190316ef9" },
                { "ia", "475d8480fc6c6d71cd650a55d147d28343233efcc313624dbcb3c09cf87df65d34b8d901b31d39e4e987de1f3715f1bad50b4f6ed84d3428d8b5be87bcbfbeb4" },
                { "id", "afc8fa96250e27bea10a21d16b6e456999986380ed1e786782c9ebdcfc41d41f4eb204193b57fa6e26a287d8408b91531c8e70c131ad1da135a7bc7bdeaaa7c7" },
                { "is", "d5c7c0a00e956010bf241ca11daadec5d43d013c41204a967f1857746c28548610ead6bd81fc3f143280f600f1afb7b94ec80caca91c565d71023e9fc69bc60a" },
                { "it", "5671ddec1339b825149f5c97a766c2cba336dab513c4597889417009818832bb404a5e9c1b582269a0cbaf52b533d2bf8e387aa25feb1433896c59a032a5cafe" },
                { "ja", "3bfabb5a25e0eae531b5b88bc0d5b67ffc8dffc8d0dcd0bf45f97a1f65228c448f12373ebff47a2f8332613564b97bfc64e994b69844b3d9e1b8223dffb53013" },
                { "ka", "cd1fbcffc3c698b5f277d01abb86efb4059755e475bb2a947a15444724eadf14c5e00b84c71f0e6a4079a3ae72793a09d0f462a93fccda83c96f4b452dfd241c" },
                { "kab", "1993b1b74d05eacaaae5301e3ee2af1844c736eb9bfb209c9e631763d379e06fdc83ce13496fd4cfe52019c9e60531e659909ef80a81fb20642f75e574f822a1" },
                { "kk", "254aeba91f329572423b2ad4ce36d2c5cfa90518937df8d575a0e5c901615109acc32c92db3fa5a81c0b3c631913ce97dbb04ad9408fd1a498e9536000625089" },
                { "km", "5caad6686747241a640d658c01bc7477a58a2737dc8e5717e46eb9dffa40194bd74248dafc4195fb3066687a7b152f20bdd493c99166c5749dbcfccd6e7954c5" },
                { "kn", "c8fc4dc551a7f8072f5f6373b29f329cfdf7de0b14ed5c82b280ebae28c3fd4718e6a5bff7d6cd2c9c6b7b61f5b4996168815c932fcfa2bd358651add7b8b454" },
                { "ko", "dfa82765a30953b195e087fac80061b02b2e1ec331cdcabeba0bdda92b3c28417a2aff904b5644d4a36b7150cbcdb39ffef4946083ad3d0ece52762555f3bcc5" },
                { "lij", "eac234259eedcfc4e92b4ccae49043ebb194ea589428fde9c8c1273ec636d20181d4a5f16f2c94bbeb2fd5a7dbd13ed9d444da0cc072edcd0e0530e9a7149a02" },
                { "lt", "c85a69a87f9d2894c03fe8f7f860079fc829d2872be707d64becf2a28ef86480ba3657a523dbd0306120575d199abd7e4a3aa3897bd856c0957e3b567d46aa60" },
                { "lv", "adc9efafa89365b2c60764e3aa455037458326f32331bdb47b9bc7357e8cfb94fc1b22dc7291cb8603788fe7a67e25ba2b8e8c963469389ae8d26f2ae2dbdebc" },
                { "mk", "85fd35e572485258e41ee0ac69c68a41a014415382e04508a82cb8fd2dd39b7db915e25f8fe8761f18c722f25f9c83e4e61b575e1c0043b1dca73bf63db2a090" },
                { "mr", "5eb623ea91daf86c66edaf7cb27c006e6d275a71fa2891baa460bd2627f8a08811a1984015f2067f90b60cada2429e35846b76ae92e1f2b9fd15ac5fdee8aa04" },
                { "ms", "9364ab0b34b46697253f8f0c39736034d0694e704be9dd417dfd03c38d3e050d1a25ecc2bb1d5f998a113f7f4ad10c68faa7659229710e0efae9e25c0a4e704c" },
                { "my", "fbaf9ed35123ac750783abcaf88624655345c282d8338245b934faca179b612029b758fdd5c444d56d3651a4d1745d71e36a902454d80339fee851699db70a34" },
                { "nb-NO", "1dcdc079ee64340c9bdcf9ee3fc9b8d28c39472d60e7503035faea7df3e5a48030d3b9ef6ecf160d97f79862a65f399ad21e451d734c6b5a7ab1c47b03aa146d" },
                { "ne-NP", "a8514533ba35c1ac8fe5e4943166484105f3856d2785ce1b6c8dc2c10bc19c8880a2e4666960b8ed1adb34ea3d37d29fbdf4606be5bdc58a3f6d8436c0dfe46c" },
                { "nl", "2e2f418f1d1a960d1771912de4156e61ca95ed141d3e1c33d00dfc51e060e51fbceb6d28a1a00a7fc0a3d63556af4c6870d5ced2ef5b59983cd9a954ee45bd63" },
                { "nn-NO", "7da998589cb44ccdfad70a98fbc583508132766dee621bd01c57fd8d7a4d30a1699cc6eeef45130ec768af210df8a13d98caeb3351d421c757d877c10549d46e" },
                { "oc", "169ec9e9d259e2286f76993033e15030352f0fd5400c92ec254f81070f530a41746907f0d53faf9fd60f8514422c34ae50eca683634ad0a4c705e67943f943d3" },
                { "pa-IN", "e77f6729b196ec7b30a16f06d9dd41d381211f72b934e2b221e87df635f95d6e1367328f7e7b48f776fa42b351d1dc8ead50a05e7387240721bf8547c0cf42ff" },
                { "pl", "01de421dd47317c655a044452ef9fade1c1ea33766ba3b20c3edde0de6ff17ec43d952b9d044712b0f40945c38c38a7e23f428e4b04a576d185710ac43a8c397" },
                { "pt-BR", "e2129fac708f92ea2e4b6fa40d25fad102b827162b919e3cf364e169cf78d4dd0a384644dd56cbedfe4a304f8c8db0bf6e87edfffda6584588a5f478347263d7" },
                { "pt-PT", "2757d79226d530bc666b08038d827dfa8a5e11a7f12431347113a0d8f1bbf3fd7eeb7241d180b39fb63aec9846f3c73e5c289fac485aa111ebd05d894f0b1f40" },
                { "rm", "837e8e9ed719bd4a83997c3c75e3924c938f466b0a89bce3867b348f3cd877bc31ed1ae4073a7c9702417dbddfe778b8dc1b0bf71f678b7d65c6d918db8b08a2" },
                { "ro", "fc5454baffdc6ba5ff9ba993e5952481aa8c7bd5e5b77a8fdc40703beda59f779b0db10b039088b8cab4af9af56cdd76a6135f612d1edddad7bdaae1bff5fd1c" },
                { "ru", "cd482d83712401b6d14940fd46904fd2090829745535034c8f033a1b86c1bbae52e72536d7abf42310fa0e45bac43d8fa731b16a28b7ea859a1c2b70ae5f40cf" },
                { "sco", "98f1a111f8e5d2656f66002469c494d2e51127581ee61548213a0297596ec06a630898e410772f092ed98262c690e9c82784fb68a4900c4a574502777240df60" },
                { "si", "9b0a1e00b18d80332d500125baea6419f5740ced6cdf86226a3540acfd2534f2d0f9495c3c8d6538631c1b0d7d69ff9dfa6e7b7cf882d1d7db6deda6a5055af5" },
                { "sk", "54223355060d831e974e03b2b68fdbad7a0ac5e661e9728acef189fee1db48a5e0fa818abae91b85d455f2b108a06dc00d7c8928e62080e2ad4f592773f73db1" },
                { "sl", "a303e945a758a652484d860822fdb1eff660eeb0493b447ffea6e186b234521145f8b094b2d9035c3e2ab2d46ccdf22025dd50b8c3fa3ee7d746620adcd3b5d4" },
                { "son", "90b6f1f4eee47fc50da68354b1285f59a2b48123e63081f9a4a089d50573df9aab10bb15e49cea5c6b38b0443a25c5287a45124fd0df117f9877040dc4aadaa8" },
                { "sq", "5b568c35a7040ee9589febcd21aa64424226e8e06255773f2522d3f7b3b2be4890b6b28bd0db1809de63b37c2569cae051cf428c795df36fca0c2fceb7ef080d" },
                { "sr", "e8e27d0508c46c50074f7234e57e729d28a121ce1fcdbbe71ea3a85493dfe19daef0b16a4d089870c28346719d4997ba07bc160e91825f84cf3d3e3a36efa7e0" },
                { "sv-SE", "431b5874eacd4d4a8db19f87176c160efdc9db80300f7ca9bd51d66c141d9d0f954ee36791aaa354106b5abbdf3e38e6a83b9361ee1e6f8dfcb9d5184225e3c4" },
                { "szl", "c86b5b043516b0f11a4a34d1640d4a13e001529ff2e562e4ec70d5ed83c3886919203ebfb7594ad5f47b3a2595d23d2402be11888cf11ff6919879c486b3b335" },
                { "ta", "de914a692839bac9d9e9640fe895c9e540c07b472aa700a053776d782d077eb3681553099d40bc7dca6a43c57d81b1e7965050ce5cda8cc35cb4b87879962de6" },
                { "te", "883c7b4dfd7820cc452abda277590e394966a7c0a647bee5f5c382c2691e6648281fdfe60bac5f14d56e7c6fbce9a136d0d92e9742f953cf8050259079808cdc" },
                { "th", "834584a0aaaafe4206fb176bf9b108aa71f621203e9e974f99dc3672a54691526566dd8e5507cbce2cdee49cf73e035f02299d6ce7737c5498ada286101b6b22" },
                { "tl", "2f9ebd518c7e1043e5f71c6c47398a8c76eab31d0cac594befdf2e6eb2c6558080b0786f92d34bc56289ad9f3336d6815d51ac6e0f2bf7e843fd5471ced90816" },
                { "tr", "e9065a4cb49da818fa395ac7dfde684af1d6b09a780cc43a8bb7dc1ceab5e3f388148af586d76b796d30a88816c269a25c167059057a9519270485d5e69a82af" },
                { "trs", "7bb1a15b86470847fc76273c438cd20920e8a7e38d77fc8e719394c244b4188da94442588aa941933b5e356bb646880b9135c9bd5dd7bdffc23db9d57ebce1f1" },
                { "uk", "4c90cc00c046eed31842439ac78e698ded0d57261a3beadda8ffab6a10a63dadc26a0855cad63fc5f343b7305b102d68aea5556cf1c8db54d6d0176cf2f791ea" },
                { "ur", "fe3aedd0d802272e9a37c65284695a5f75f7cd0111f2b7c1fb26d51a05e9aabd1aaf73da8c0c832ead69f3ba514383589f33214548e01a6ff937706e8f233971" },
                { "uz", "482543f2f599e07a52cd60ddc1589f3212a089cbc1f5b60b87b222a844ce172c2a5a04bac0ce67b1a852a94677ee0f41adceaaf5659d80085a428dd8d5e2246f" },
                { "vi", "544cb11adf5dcd7a92a63da32b1a0701716c43f97d1d8cefe9509990080a796ac732ad84b08da7c55e0704e4d12db7d0a8baedc1623c787535e9cf764ec2de18" },
                { "xh", "c674ffba6bc24ee9ad3e1c072ba5bb0f4bdfd7c4116938bb43402ce0b292d050e0396ebf0c2c368bf102665d28845b25493137301b810514efddd4557067977a" },
                { "zh-CN", "655e607e90a3d801a1a183e921ac9fe9d021a822ed1c102c7b6e96f4e68bfa7bc96e8df392fb964d6855fb8981bcfdd517b703e7d2ec39a631b6e353b349572f" },
                { "zh-TW", "d300d356431fe7aca174fda2a4a9bb7b75ec4e4d374cc7e761843ece3390df98969cd6a31d4abbd60a0e80fd55e44fa74c861384290afb1a926f3138d4fd6b16" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/103.0b2/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "c4971de6529522f9149dbc3a0abd9c32916793df77f4c697393a673fd96590f3649d4ae5f73115021cfb59d3425bbcce306306e8408c9b7dc5f26314e55ddc56" },
                { "af", "77c298709226d1f93637afe0328057f0878e13be995872e88876b2a72c0e2c10fa44dcd750855c5e0e3d23666c2a82513828bfc9fe59b391f18cec769ec2111b" },
                { "an", "b3e5ddaf6573f39228bae64b375beb3ea410d0b69469fe0e764ced8867a8e5955830f6bb7860045832a1d8576ba8405fd2b3b6715bf2daf02c18e8dc15f4625d" },
                { "ar", "561c87901217b753dd2a996c984f7b9140a9889c2aa63efe6a2e24222de3e2288369dfcfe864425056e1616f855488bd790d420edf56db7da7270187709b1a49" },
                { "ast", "0502f32520f19344c3b268932bf32e9a888f094ab7a649c96a964c0fa970b01f11040105f2154e5896bada2c3a58a707f065c120624e3f0a2dfecfd1c6f76c86" },
                { "az", "4949b396dcaf5b133c0822bb5dbe3baa6c1f7366b9d7390f9ba362b31d70ae71edff58ef526eece18756b62b95aec9c154706e9c269e6e05997be2af3aa8a01b" },
                { "be", "2010755aae4ecb357c462015cc7df5dcb72228948e9176c37b59fa645172b3c921904a7319e2ea3155f77bf945b187ffcb054ac911f35ab403953c9191ea8a55" },
                { "bg", "075affef8a427c74bffb0e42a62a3de1a805e29564f86ded1ee98fbe0a83068705ad7b27b398b67026ff7ca04853087538d652f4f6225a18f255a2fe08273174" },
                { "bn", "4b76dc54c4462f6d336435860058baf7227aea1dda70dd69e41bbcf51909f595ea0259b1de357a79b6fc8cece69879da4809a3b3a5408fb459fe90a0ca3b058f" },
                { "br", "dd6a11836d1bd5ccfc7015a0ce4c6448ca3aadc7b806cb8ebf6fdba9e505bafa8013c26bc6b167023d05e0cedadf6748a1c33f3575e60c8a19491a54a958ca95" },
                { "bs", "44909dd6d6f8be048e41058f69bf07198d65a6f67073d042d1c27cba58ea005f33d5c335d707c4166a538fbc00f6c5f52588b53dc010223ee13a282df71147e0" },
                { "ca", "45eda23c5a39537261bcc914c07546d11ece48e6f99abcac1d1dee82168db3509cc77bec844a5d039b2948a4b453fff7fd90a342dd85fe3e8468c64d988071a2" },
                { "cak", "493b2338bd34005d1683e3e95b78402d02f57f5423a0f62a9f82a5d0df25e6213b477af74f8c5749c142a80f4ee58ca971c4404b0bfaec767f5a9be698b2e287" },
                { "cs", "a64c55436cbe1fa932dd95bf8e945381e28376e9eb69c79aa0e3a50efc2aa907c2337fbb5df5b02110cd2bf5680ed8ea3e03a96fad53727c8b57f55a19bb2999" },
                { "cy", "ed431205f9ca6ee3aa848a63b8aa44f410603030628eac360e515981e5740a6df734036228e4599b4c52bc99e38813d4b816d1e66b1a1cc66d34081f9c06baf8" },
                { "da", "1cf708a139f749dfe08d942b80e88ff34afc7a420f47136b61ea2101c5629708de30a910456fac66a318e4526005daf64a93849bb59e04f4fe51f826cc83e4bd" },
                { "de", "51ae0b7091b267c4a7f512ff3d179609fd39a6d2d37f162dfcfdc1e9185afd3bd3db6d4a5bf3785350f7fca8932c9b5bec1b1992ea2c9e21838842c036ee9402" },
                { "dsb", "e566e404a7e809e9eaf41555ee79043ee03673315fa0b99a37d1d4bba1f0209df3e05f935c6d1b22663797f442c0aa1e08912d0a09c21ee97d98e06881c68289" },
                { "el", "08fa6b420051245e2d07aec136f1134bbb3e7cd6fc1dfab62a2483975b850448233c14ae743777e6568391f8508731e831f8536f0fa5c35323ab526f8567f093" },
                { "en-CA", "c7ee807f6aed8e74e8dff556a36f2cc5337aaa07de74b00be4328e472a0da2ac547814c42e66a89fa2ff2ff1e65a98e1156accfe89f2626b39b01eb2e4dc5264" },
                { "en-GB", "ea3ad3bebb0e6dac4633a8327a5856dcb41ad4636313dc1d9e09a04498c1f2b36425bb2a78ad3fcb3a42d8e0da20a3b9694a47ad2dec01f457fc94f467b04b29" },
                { "en-US", "3a93171def34e3cc99ccad1e5b1793ceb6348a506db863809d171fdd68fc08bb8bba3dbcb0e5aaee32edcc240025024be987d058adfd569c552f389eba5e619f" },
                { "eo", "da5111b597e5d5e9b0508cd93f98bb983130c1288e8cf98f54bb94620829de3903b75c64bfae2519d77e34b9cf549a8215f1c1c411202a4590a2606ea5f1233e" },
                { "es-AR", "b467e4cb8c2b479445cf8b84076c29a1dc097c6bd7f511ce779f217165d0203b72b4e62a89044471fb18ce6ad66cbe4b5fd5ca70aa1dcf840cfbc937df3ef93b" },
                { "es-CL", "1cc2a0f39d95197903345c9b53bde28fcb34e5a840fdd5afcb85e7783286dc8e05b1fc08283111147bdd94b075acffadee17b552c3bc455d85d016cc235f5b6a" },
                { "es-ES", "8afc239d112272386ddf13725640678d3ef9f5908cb9d4fd529772d785419c58c126a62623b54940ff39e0d3dc1654bedd268a1e2a41d8ba4b4dcefc567d29e3" },
                { "es-MX", "1d42c9e199b1a79ad60125145a539a8a9ac794fc892a3379b2b254e67ecf55fe194aeff7ad209ef34632618629f63b2d3b6c230ea1737565fcf5f06115e83bd0" },
                { "et", "8399e74ce3916f20336eeae9598a3551f1e6085f18b8bf7267ec9c1d904579a477f64830581cc70b28e95abf1a7e7e92c8f8bbb721556ee60e0c3230de46e4fe" },
                { "eu", "35835b5d8b098deb19052de908f5617220b476f144360ffc3f538b24423c23598593a746c471d8416662e52fedda8a9b8b7d3c2bca435f92240d6c8448d2dd05" },
                { "fa", "bba9e48e83306cef931f713d77430d2fc5bcd4cb060dd80a1ff9831c1cfb491c827ff33a1894dbfd193c4dc09f65ca0cdb68ca1fa213bd6483cfee3ec384bc35" },
                { "ff", "3e64a28d7dd9087f32bebd63eaaa730c2989f32c42622911674103cc32df58376e9e9ff6d4639f36c658fd98c95005e5e4a399e73760643da910d6ab2e738c81" },
                { "fi", "b5b40a9965bfe6bbe914e4a03a1608bb2c26f3b0823a2afe2662e54dad640b9f4379302c36c654a25792ae324f7aadcd5245933a6177fc01cac8031bbf463b4d" },
                { "fr", "f62a71ed2173f01a6d6327f75e8289556fa40458fa5e350c3e2ac84a26651a0f1b8d30a2e9d770cf20ebf84939129ab19ccb25b700cc0ab7253306eaaf8ac8a7" },
                { "fy-NL", "55405539373454146dac5510d7f0cd30cc3dc7b37fec4671a1104cd7fb051d4c920248c68bac241d595cdf5707b211ac2d582a28299c16545dbb7dbc9f226c3f" },
                { "ga-IE", "e90595f3235a29a04a54bc27248a119335afa58bd3ce96ef15bce680b258e6d186b2a04a0a56c374eed0db049c1849843b94356e23c3da9074a666b09d423fdb" },
                { "gd", "9b9d407efe03e03a10f22205f0f12940ba0019b8384cbb240f29ecf94a8f78d63883b50c445035eac25c5e00053b3a11c35bc446ff9c980250fc4d200f957d06" },
                { "gl", "ca9161e2ae3cd6e78d6f712771d985513648766b43db3852745d69f8b9436a40c9920bc146b962d7efcd362d787eaedd46bca155578a15e801b3c727258b6350" },
                { "gn", "cbde2be7b4dda8bb763c917b093aafd13b89f0d2c83216ba75ab6f48b68dcc961cb03ebff33289cfb8e0d5d6a65f6fe94a6319c272e8cc598a569213365394cf" },
                { "gu-IN", "e4b33c44fb480891921d6b89ee73d3093eb67fd1b3d2f30e167ddd99538540edf1a20c99fac60064f8f47bf80f5f7491c5c414db7f8430e941cc34b4d11e4a77" },
                { "he", "5a49b65e211491202982a94f9191779f39268244dc53274155a532b4885421e74c02be20669883eb4756e96592b64fdbe1a567e0c0fbc5733673448da56b8b4e" },
                { "hi-IN", "3bd4bb58f6ca05266b879ee909b891505c961a17ef5db69b98821001a6d6830e319fd16404346edf37f774177d9405bafb2070f0dc6f945c5c7da66a2a8a5ff6" },
                { "hr", "2856847fbfde9027c8b7c545367645634746eb5aff982c69570bf2498907aee99a13ff8117c54651fae45e86a833b1c110363f7b244b847d84c0276321b1349e" },
                { "hsb", "270ddd5924c82fa146a2461645aa08e4fb58790c6eca92c15d75043374e901b86b63994a307bb38289aef50b92e7aa1dc5fc6f3d05235a1e6250a5525d2f126b" },
                { "hu", "12a2a6223de05b71362331a91e9d90bf4c7a42d8cc27a212c83231d2b3b6819c17e4083e40efeb58768f127d77e9f0ab0e42f278d681813103a1f782a05acc7a" },
                { "hy-AM", "8343b63e4558a0a09b91b9edcc26d0ff542a8967c96498ba9336b37b6fd49a226ed6dd0f24ff0f44444d7e7efbb0ce454d1974dea34266fb6bc4a30a52b2ab59" },
                { "ia", "83cb078a81597f6dffc49de372d2dade1a4dafdcdb44b81c9210f853e5b8820d2dcd9e931157239643448d26c6dc7d3219e606c3c2eb425dc03b0f024b1a4c1d" },
                { "id", "d2a5475ac665e166514a470dcd8dfe88be55b9189aa835f29dd8a207ef3d79171130c7944f3ad6fb496d572d7a6c05bcc3b9dac22d5e68d88d8ba9982b4b3a67" },
                { "is", "0d102042790584d546828fe8b1e3a844c7e83b5546763f453a6c9d3aa795f9a6d6bc894debf48d8842786d3bfecddb6cf4212e586a049d445c48261fec720e20" },
                { "it", "044dff917a9b46a07ed7ebd2b13cdb497e49e86bf407f8973c95385594ec13535d710e827a00350e382bf69ba42ca8842374016e0aa78d274ae4a438937c456d" },
                { "ja", "4a9138516c59821850d7cfcd53327e43db77b4e2786dbd0f031c976512a83891782583c31b38222b5ec32816a0df624fdfb98e95cc2b904ca036b53be4b7dad0" },
                { "ka", "a4852419a3e57906595a4f7c51e36e1f53a43a598a26a959cb7a5a4eb2b57301bde9ad86c35d71a1792c2e59e2aceb305bc6512af6ddf183afba5316f3e79a31" },
                { "kab", "02febd6d5b67ed17da482c3ccc1101a116d0327fdd57393abeb5d8fdd5648da861bc8effa0c9caaf0c3294be7913deb1abfbe8e0198c13cee226c3ac1ed25ca6" },
                { "kk", "f7e844cf36263fc714da193c5daf6d1e05e949f20a5366b1f755d5da0ae5ca457d0a0a86e2694398993c705951aa9e7c558564298bcd89878758730edc0a1a4c" },
                { "km", "60a91344981344f1cea8d512b2b81231925da24f93f46c322d9c0a35078e9177f91afed380224c2d4add446de5aea96c2214b67887c42a6e053197a762000098" },
                { "kn", "1991ab78dcbc36f3bd34529c9244e82ac85e8ff070364c383896a46f1e46c9f59f8e59fb2da773fa40c61d510b5b389095edaa06b0c423f296dcdc452d9ff70f" },
                { "ko", "0961d6cb31640a0c89ff7874385a300851c3a9a79d34312dd8d1632f9d0760d0ea50c35075ed88ecb57ed3d5497ffdff194bffd15e0941c11042d78766f06890" },
                { "lij", "2bfa09e039ed5c44689cc9f5bc88b692025f258a74e6af960ad5f54f9ffff0536a6023a6f508f0c6d5be7ed8e44205102116eff7e14df34b38b0b5524d5cd6ad" },
                { "lt", "a512d1653f026832f80dc2a7466f34b9b293772a1ad4cd13e4c2fed612f7e5562f7879b3c638b10db228442a0ca11f39222900a354f093b2e034aad61cb81528" },
                { "lv", "d74bf6bc176a2267a501e014768209846082ac1ee73673171d5435dffca8dae39e8c480d2e269c893dcb167eb778d02cb042bcc55eca04979b5d3bf464d9060c" },
                { "mk", "ec389040125aa114fbac08555f847a237d6a8181614ca9bf250e4181f4d3e27b5bbbeae4cd5f1cbdbee8058b01db50277c1788108405dba4139223c7dcc88f03" },
                { "mr", "2f33ef197f4babe57aebb17a55d43ebf11a9ce65e705bce869b58abcc522d642c142b8c3487b7056c864d3ad32e5eb17462d2149fc5e3ce0792c8f4410c7ea45" },
                { "ms", "e5245c4a78d2fc5196e3a6406e23768daac6ec5d59d506c8725303967929d8e702a341c9ab89b2a48db72e44375b194555228e892600cef326d3aa2c396d71a6" },
                { "my", "fd3222bc8b6a39ad9714d8d31c40240f8c35db71cba806d91281c0c3c9c74c883fd3588913bd04063c308b09a142a85c1c1ff740e6f2d8b83106f8dc6d7ab676" },
                { "nb-NO", "16c59cf53aa7f5d02fcd413872644562517974738c987791c4c9560564ba6f8406bbbf2bc3b1a4622b7bb803ccdac2d3c9c716d40b91ff8999f65d28e05b9254" },
                { "ne-NP", "8c0c56a6f440acce8662b5e5818d843eb656782d317da3232f702b5d8f1895ae9ab045c46e90acc69fabf7fd17eebebb01fe6f065bc7e732072e115bf7a19de0" },
                { "nl", "1b5360a7c7eba5108539f70c37fc92142587e7861f3985570d053b43a4295b72855d8dabc6d89c2b3739cb4c2b7a7f4c0317972c54152e7ba3be169930a661a1" },
                { "nn-NO", "03f693bb411db15e078b33bd97ffcc7b4b257d384db8b24e915d1d00d8de3dbbc83715fb3cc0898a3c9d566bbe89e8bdc6ac25a72a97021ad3bd94b452390a4f" },
                { "oc", "6fdb4468598917f0e7ef58d60db697d269c113c559a66173baadc62bd7f2c583d6cfde6a3fa0435c2e5ff2b4188ec7ad39ab288f8816fe52f47eb8aa16a5b2b3" },
                { "pa-IN", "c2fad1ce2d3d474ecbff04f7fa4cf76389ba4b4f24e06b535aacc059706d5ff74086fb6da30f307bf4ddc44fa84a8e491ba4ad85d824769b13155c469ab46249" },
                { "pl", "27fdfebee6444b3b3e1789cdcbe127f765e76e2e4b6ec56aad519ddde6bc391404488c4aeee17f95005f2d68a66e2af9b0aa08cbe531c0cab8f63e02a6825b23" },
                { "pt-BR", "66cc9019256103d69aa9345bf19bf9bb143f0ef3b95073c04a011df6dddcf35a88486527b79746731fff1ca5343b31b8c1a7754df082ecf982d8328820abb9f4" },
                { "pt-PT", "4e67e5ec37bb86911571f09cde484ab55da5eda5c36b1e0bb604132dcbdf7c98521b01e25562de7827a1943795ff16c18968430da96c9e9f061aa485be7b81c7" },
                { "rm", "775767e297a86d3e59acc68840c3382609c01ee53aaad56057cc4b163cd9b1c85b65d3abd058060148f0ae2eed0b2d2e2904f682236d42fa8b15e972efb91916" },
                { "ro", "3e79ce0f85f3ed91caff093cb7a8da3f785562b9efc970208671919a422694f9e0edceb8fffa89f6313a7e4ff5e67baac3dfae755101b8a08714117f69e77c6a" },
                { "ru", "351ffb2987241d02261fb31343ca97d846263e8111cd6384cb28c97a85b3caa8170725a85e5ae036e111efac67a5e2c3c7a411426f026f0d92983add385b4fe0" },
                { "sco", "86bea16470f5f39ea256f5ab371a0f27fb739c6d10391e2f478e59fbdb27800e264be4aacd96bfefe890a204a01580e8cd29ffc5530d214b3a28ff23422156b2" },
                { "si", "259b537e84252062eff437cb2ec0dc1a00a2de157b408fc67fc7f2da3848128858761553dd9cee04e997f324882c3b2c578d24c1cf97b960d1387d2840aba7ca" },
                { "sk", "0d3452e9a1a9878d32d37ae0c0fb2f15c5fbf30ebd432d48d4350c370ca9bdb2e4b8e5b9bbbf9ace9363b12f8fdddbcc3323ed0853859fcb64d6b9c83ad36d65" },
                { "sl", "5360e573de2417b8e14c89d39184145066c6d34371f6080aba36bb4b0f17bba83500a7e67de1b68db5ceb795a3e56c91cd016fcc39de0de2a34504b0ff24bd54" },
                { "son", "b39ef70ea42263f3015147e97d0bff8348dd9658d65f292c5025f7b75cf90a3cf5c6763a25e6fb56bcb58b6781062664f3d3c4ddd9ed54e4912533451e15bcf3" },
                { "sq", "75e6d9d223d7ec17025a89502eecf1bdc14da775f83c017a147d6a09aca144750247c7fb2ef7c838a2f8d9ac39dd8f9dfa8a4792e0d703df2ce645760a955d8d" },
                { "sr", "c261e13093073ac1908759388bb402914ba5b1bf95da23c7628cbed228301a52c2a5fe27b5b12358f5d0f4abaeaf681f0d711d3d083e04f13fbfe9a56a84fd46" },
                { "sv-SE", "39a1844383f60b1b72d0b2033c9217d09a34f36df81f15c24389d113dd11aeef4ffa28dd995220ff2855f3c9fffb6cca54ce60cba9351b4a982ade8ca59288ad" },
                { "szl", "09c559a90bd5c42d113e4ac487e5eefe866110cc064d5cc42beacd230505c40abaab81405fb2cbbe434c5a45bbe6a131e50b495caba1df9d5af8b1e9fe1250dd" },
                { "ta", "8f0e9970d4193def283af9081b5d0a4350097189fada4722d9e86b64ce69125a4589cef9ee409ddc240daf51137b2762d17ac546280d5aad652f07310937a997" },
                { "te", "4c9e7f598e965a9cf55cce670dd2308b2362fb3e81d2cff3c30f6ad0c708051fc7ddcd5c8e29af54d7b6e1e18e349c8e3f4dee0a26baf7bf86b693aaa66c0d58" },
                { "th", "8189d2d4bfe4291bf4b492ab2d9bbaf238dab45c56c67dcf2799e099e5dec12837a683ada918be52e377594d4983173b0fafb2285524fd6aa6f81b0a3410c075" },
                { "tl", "5f0f0fe28ff2bff2f3d9abe026748d2770442f026c47c1723fe792b7405a38138abc02eab56a01086a4cc0e1ea8a2e0cf9bf5ef31dc7ea9e35dd188651a2d244" },
                { "tr", "e6c896db1c1c78145b5f120b225e887d80919328a7d961a3d02a2a27c27deb29dcb584f6a5cb707823cea847488cfe3325353aab78e1c058b72c25f6cce2a98c" },
                { "trs", "d252e231b7997fcac893bdefb15fc2caa1d3b2ce3f97bf39d14d7b34147857f418d66af4ce1798ac89696bdd9ad80d986094a48cf5b489d980dccf23ded2f048" },
                { "uk", "ae71c2fc5eaf19b791ffb244d2b79b4e53e353b3abcb6cce816607aea1f18f66de8d5fc0730ff4e2f041dd54a7bf73d5930ba195d07482a3f22f90043f6eb95d" },
                { "ur", "73a64e2562f8e4d66aef186f987115a0ec9737d937385016d9983e0547f2b690087384d1b0aff28dfb6ae5c987c5e17ef29d5274efd207556a7c74acc3c52a8b" },
                { "uz", "d2df66eb421db7bb601c4324590deeb6b91ad391750f3be9da7e5ed32ebf2b83dc03ba3132cefaefa2991ce866d47c457d522523a2ce704dd8ce8b66a3b1faca" },
                { "vi", "7a494690072ec7308573f1a3abb9227eb31c98741bc24640a1d6f4e11f7dfa31230220e6e353f32edb40d0ca2473f3502c5f07618ab111d2eefd4dd032153c4c" },
                { "xh", "1892c5f4285185d1f59d008b55fa66370667acdebf975f4b7d3424ea95023b42dd9cec37e8123df5c5f02c28e69c12f9f3d241563d474d5ca03a6e24220b1161" },
                { "zh-CN", "56a0019390afd0e435e1be2f378074ef27dffcaa9ab589a29d6fa513e23a38e308ba7bd259e7c626a68f4052030ab6b378d5668635951ae9dc76bdeea4f5dada" },
                { "zh-TW", "0c664712c6214dcb57b6ab1ca64dfd049e5928d175ffc094fb148bb103163ef518eb036da4c57b44f5c13896f5ecc0a1b28a91c864d1132dd5b77d4bc913a19b" }
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
