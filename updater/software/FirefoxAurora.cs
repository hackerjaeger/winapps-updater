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
        private const string currentVersion = "118.0b1";

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
            // https://ftp.mozilla.org/pub/devedition/releases/118.0b1/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "ead250f9c22e8d012c505e4346bce7969c6126066344424b144764c4e505dc0c897466e1da62fc2e2bd4315fff373cf27636ff2ba51b9f80fb68c22e3748c122" },
                { "af", "55e81b2eaffd99d753fe92b803710433c44bd92fc4a6ed5cbb27b917add3240789073d7c1ee1348cf56bc4f792ed551dd8b12c1fc23a9c2d85adcd5d8b4e321d" },
                { "an", "74d40bfcf14352ed7e314282f8a71e69878a4f07546da6af28725e553b445d40ff9afaa630d3390301ade3d767897170b0c2c2b9bed18f49677b2e6aa8c6ca58" },
                { "ar", "d117f21371f68974fc1aa33babbc273ea8216fbaae3529b7317d1609f9090291343f67235c236483c8dc3c3496e7eecd0b2e07e04df195b718ed9d6d91cc9dc1" },
                { "ast", "87ef54943b2396b474c4096ffb09b54517422d15927b68f2b5e3ee24295edd6b1f524df5083a0a33a725055a976e3358377e43e588cb1b051a1a7183d042246d" },
                { "az", "5de4fe2df683b5d9bf5990fceb0a08f293c3e9e29a5b34ed66fe43e697820bfa1436cd0696134c61f39df894115360ce83771c990268d61e62ad277169d1dbfc" },
                { "be", "c2b8d272c013eac99361e1c8c98a346dc3eeb1608cd6fa74fa92644b0d0e01c1a6d467b2d6652f0d171259c7ff0dda3ff2570f2206e5280e29a5f1f3872b09cc" },
                { "bg", "3eb149ea52515aec2320eb2bedcf32292b243e1c3d037e58d66d945b1d1773f2a8eabdb2d010c28a2cfabde1bfb9a9d556dcc7d2cfdd11596eee13534d1ad7a6" },
                { "bn", "8f52c2c2b09eca3c543a79cfa85306fa029bcc394c8282e3b1c736ac327b0988de6d0ff769df45eefe70caeb7cb9fc4526d257c9c43a02b6fb9f4e5a65494dd4" },
                { "br", "643fd64c4c0bcb985cdb2a997a7120158908fe8e3cc3d7c5e6b79bc8ca77c36f0dbb892114f1c397929a3da64f0c85a66cb77d74c8142dbc75f6e7787063ce61" },
                { "bs", "3d0548c97979764300c3ebd8d4db4ff356d1e7471dac621fcc3621fac067f28e7c42c2d52e7d21724d05524f3bb2daa779843eda397892235636c1880d330c49" },
                { "ca", "aabfbf332671216cbeb00249db9d6d0081179bab83a75fc11a58562a73512b2ce57a3b7807a8337e7ddc6f7de3b5a33448cb577f2c284a981735a6615d8970f7" },
                { "cak", "6ad582a6773b17e5707318316e4f997a2bae52ee7260a9349cb3f44a2df1b9395951300d9d5e66eb8e0c7e033c92fb903c5cf5a769484cb2673fc9ed73de12e9" },
                { "cs", "c2ee324767bd36012fb6600b19ef284252d70cbefd4d148c38dfa8d554900fbeebcd9378b9e7574b5e24bfc2eb4a637704c4ba7f05491686cd27f434b32074e0" },
                { "cy", "bc61ec35832e70f8f8bf321a57394a503a2a26a0fe514552c0ee232e98e0e95b62cc1961715223fdbc6dd220aec761520c44f1c1d2e1699d8d1c8535417b1535" },
                { "da", "53d5b4c6dfe0c959f896f3ee2fd80de0ce70c252ec24cfce5a8e45f08bb903d6fd18af63cd360b4ee8a231df84cae94a996c99b99c8665d0025d6f6c67dd23d2" },
                { "de", "b6a95d5d953336148e40f9645b4d5b9189a52950a952a870229de87af386d001a53a2e229948da787ffe24eac9a8318004d9025a925b7ac8dc02231b471cf053" },
                { "dsb", "54f03be1e36232443e1920c77af0228c3626af60ab5ba08f3bad208cc5c0b66c45a11d036a1c464e2017314bbe34dfe694e190d61481e6fbc81264c9b7f3cfdc" },
                { "el", "e075180483c93f85082251b64c68e4a096a01785a250942474bbb0b059d4da0cfaa71e9427760cf226003ba1eafc72f6ed7470bc4a7ca3b1b98030ccbd3e81a9" },
                { "en-CA", "a910217f471783c7a1c10ca154dc52d6a3bde5fba881d5b2440bb01423b0571063bf6e4ecb5fa0c745aa4b8b4fb81817cbb422d3157b3e493c064aec2f05960b" },
                { "en-GB", "2a0395384f62c179c31d2f56525d64b2ce226fe024ddd6f38952fa86e2d70c5035cb26c090f0d26f0d388bb436324d766b603b0faffd5610495b96320270b34c" },
                { "en-US", "ed2910cdbf63cf3f42b357f45f3b0e3218ff87f31a321007a610fed3e6dfc382c999fc15c8d6b6bf130bda138115642154d85f77da0535c02e5f63ba02d80cba" },
                { "eo", "2271b392253e20a025995d4a5d7cbbd47072924f93eb4d414a4b4bfb21cf7fbe7d904160f3458a5e88ee8714d17d15443fa73b204a9cdf534e1550526c37dd34" },
                { "es-AR", "38f7b0165e04112cc8b3b34323676ddbd956a1f0a259d29a75298a5879e467aaca41c990fbc37304da44dc1c3fec546f1a99b04cc6409a38c12251cff2eb2d6b" },
                { "es-CL", "47b39988c29411ea15885707448eb266d11eb28809ea941516c7d9a2d2d67e5af9aa84374038822d88371348686e8509ab7952846f8b03e3a534eb4ec637d40a" },
                { "es-ES", "a109adf8568f2e8b9f4a92e63c6b12b80c2b5fc391271ad59a5b4ed8890f1b778ce044969d6a9207f551edca7ec0a1b06c49614f52a245f52bfde5becad0bcf7" },
                { "es-MX", "2bf401e8527891a36f54721b04c13d29af0f97dcf6fff82abcff14b995803ce0488e69c690eddb67f9fab1af412ee939ce04178e3bfc0e58af202d40cf7881e0" },
                { "et", "e19b51c3c70eeb1fb15f06d0675f713c36cbb8daa26589241073dc5bda29a487292ecc0dc882d1590295fd19daccda38bc24cb39fde6e87f7e7589db1f0a553d" },
                { "eu", "bd11c66dfd8bdd2fa8ae5304882d3382431762f9249502aa50133be58aaa86e52a3bbaf28b19dcbf9de749360800f345ce13080366136fe326aeba0b9e4c5e89" },
                { "fa", "cf07901370e6b1c33eadd568e2842df92863ecf12c432eb6e190e7e837752632f6ef57303616dce297c814653470b06490383b3f8a894c2bdaf62609f3b7f4fe" },
                { "ff", "85144a24527996a4319f8d380740dceb3b23a2fde25a8cd8f327c16745f3ecc14512c3e0adeabf0f45a787aa38d01ac3c68893b8b29f48ff1907cffb35c5025c" },
                { "fi", "f05bb7a26a127e8529d6912a1e450b956ea176b8d1b24c7d8b900c70f9341618ee4fe7b72519a413b3a581c37e4d598aedf9d512e6b5e7b7b21b92152122dbd8" },
                { "fr", "f25118f74b50da1e74ebd3085be3c10fc2d2ff889afb2cad5c2b48240101560132bbafc678bd41af02d0728cabe2a33c1b6076fc75c2e4187cd438a0e89a5101" },
                { "fur", "c0ab8b2590b63d1bbb08a67e68f6a6410bb6f9684c83816c83f2d71714ba691ed76ef8398d9cdf4269d06b18f4c0d29e3584620850f7e3251f95a36575ac3858" },
                { "fy-NL", "979518c3ce2296a40582ee5dcd060a576f5e4e07c95434983f692289bb91dd6111309f9940b6b3690ae78a238fe187f33eb352ca4501cbee2d1bc8a14067c764" },
                { "ga-IE", "7ecf43aef9d1d2c861449f1bdacd5d9c717692b5a3e5725e41b87dd86eeb035f0b3c5ea5aac83ebc4ddc5e24aefcd26b07a61eb3e81b61d7f2000082f1b1dd37" },
                { "gd", "eed7de920ff8613d3eaf2a788859fa65de6d6949ece41938b0a2c41fbe2e85ed71e41d262ea4ad9f5990682df23a22a59ec3c8c9fa330d63b9826bd1c9c5ac7c" },
                { "gl", "c965819579905be2368f9d9e3b075038d5d6a3bcf74a9c1351863315afa7d36e748cade9fe97d25340bb575405af6f0e796fd2e1077bb13a55a9771fb75cf9ca" },
                { "gn", "d206a8fde75df1d19f0b6df65d18b0531476133c06cc7a829b1b8712d8eadebf089c5ac6521f6a27911455f1b5ff611df5382c135836cffb0bdb45194d2b4e32" },
                { "gu-IN", "fcc1451e928f40f239d6abb71098d89948f5852d13594f63c14e2f2487a311fbdafe9ae5b94c9dd643756eb9ef3c278820f09f0448b48a7c6074b4a44e0f035a" },
                { "he", "4df1735d937b322a172c847fa790c811ace386ef594b958b09c30150605733516abcfd311d914dbca0043d878d46f74199a88b14db4a19f4d170edc6d503a538" },
                { "hi-IN", "6ed599048a71d1bb8bfa089e8e5f9de570dcbceb276aaccc46d1d10237bec1b4ae31608a8e8f11cb73bd0bd69d1975c2a3bb745f095627448d9e0b5dc0490191" },
                { "hr", "0b528cad2495775f52566e73ca9936e376620759402ec8995f778947f8494c43614aee63aa98e684486da4d96fba6c7d31b1bcd75605d7f6b15e9049f58f2972" },
                { "hsb", "ba91e8347815bdd7d9f3e334d431dc6750ca9915ee424a4a80717b8000b1531224ebb158652e2fe7c4c5f172e244bcc4e6075c07864c1c6bc566dbc155b9476a" },
                { "hu", "1c0c494f4b5b9bc885bca73eda5b7c3027a85b9e2da3167ceb49be4a9e66037c4899404d4565649b53a7b4525bfc52540d959be87a60559a3cbf568844b5d1f6" },
                { "hy-AM", "f919aa53044891f4cfba20885afe8e5252627b79d055cfa8821486bb1e139ab728717dd98e60cec068a41b573c8ccb4af749cd89bee96d0d24d93fd6b94d2440" },
                { "ia", "632105a5e7f7631abccf83883649de06401bc6279911c80112ffb2904297957a1daafa1114e83980750102de3fc2c96529677471da332fcd32f17ac71522d361" },
                { "id", "8a869e8d6c4229722bf58efea6074a2adfcd84af4c48f8dcb39cfdcd7be9cca4e2e217cee954e9b90565cfdd3cc38b0ff5f092faad0c6bc0829f8e18bc00f7f4" },
                { "is", "2c2dafa85dd115179fe77a441aec6d336a521c9dc6aa6d16d253475b97e2bd6a5459f6592c829b7b552d3d10d4b0f2239a19564abf2db5f0ac41284e89e7c54b" },
                { "it", "0a275a7ed3c7c8382a662a3db1b878fcc13bdf37555d2bd2de452d3eb77ae94db364364989bd68d18738df82b952f466f3e081371ba384a5ff869f83032db0c8" },
                { "ja", "aa6405e727611bbf2b67addb3951f6d2654e48b643e74b597736e0e2f68e6b95b80d1efe81c21f1bcfa372069e8eab6d1edeb8bc4d26194698dc3b600f3c1301" },
                { "ka", "0316b7216f12cf6bdb8eccde09ff7a1da1944dd96b463094b288ccf23b7d77d59f8fbc9ce1e06bb7953cebd7494d5570068ebd8fd1a10acdec7bb9a7236e0abd" },
                { "kab", "2783220879e26ebf1a23dd7100494ce5414a4875d5a918051909b29502a280971018e58ee4d595760637d0c64a6def0c29799855c78db0e8c499f3aa9f89a505" },
                { "kk", "5c2eea795c3d97af45ff7b24c489038e11ddb86d37b79d9c133acf9bf85f416a69c210584ea7932b081c5f0aa9f6cecc6863088d831bb8099863a76175d60097" },
                { "km", "8891889579d0bdfa007847d61f7da523baf7b6c1aab0e51b3d605d4f12269c2b3069212ae01d76b605c9c3a509f978770ff361201de283cf15c5671ed6540657" },
                { "kn", "28509360500b7795825fc8ea76ada28476b7a4e8f74418d1e2369926ea604456a2e4f8d61c2d51df9bd3dbfa1dcd0eb9986f62befbb64d7a091978751b80764b" },
                { "ko", "500bcf4d6c3cd0e6ddd5ad44bb7e04af4a247ba90c3e27ff25d6ae0282cd9f3a8c92d052d6a5c5c64e60761523cd1104fb81e98fab23ce154beb6b8ce924ef01" },
                { "lij", "68b71c96c23f1ff8d5d9787b5af81039a756dd2bae2e3e62e4ce249dbc9b09e8a9c9d8bdeffe09bdd60dbd9dce46738c37a47251c3a9d50c4cb702f83922a5d5" },
                { "lt", "4391968414822b0409cfc94dc018763cdc7262435bc54642771e07eaaaf148d6ec47cf8d0a7da511011158ff038c8d68e551f9c81df5c69fd47e913d67391fa3" },
                { "lv", "8650599472184ee2c12723546925255fc26cb8c771298c6e1261a8bab065b1503653014d0ce071ed20582946841e6f160b3783f15c23a13c99a7e8e727d81fee" },
                { "mk", "4144c1e7fac185ed5e724ea438877cc561c10a7290f3a268afd2c284c0c11dbf61179c77f2452e350aa6d88e1ec8116e0f78080c663c80ef4a9e20342875b7c5" },
                { "mr", "b1f901725b35dd6b0e4a407d24de0c48fe46211f5e75c57801b7d6d7b13dd2f77cb9ebbdcbcb7868d2682f921943c9cb762a64c26c676361003cfe6d9d1bbe65" },
                { "ms", "e69c2d256e3399a3b59949673e489cbc9cc7fe586dc51edee47be4a30b70b1a7201819f093168738f06121b988a42ffac900311a4522bb29a0ece2d0151d346e" },
                { "my", "16dc30ee2df5b16279c193abe0f8ee123232a810adda16a673fb0a38f2b3e14b57dcabeee9b423e7a0002caefff31eb0b02785773a5d99d9b4095a7b7310ba66" },
                { "nb-NO", "4f02bf39176255452a6a885a96681da8c89190c58fcbba5adbd1ece142a81a5cab8001d8b9ff3e838055383e0b76bf9aab7e849f123873632a93c73c0ecd383a" },
                { "ne-NP", "d4f92572ce3fa8e75534df4023e6858a94211e3615458a2309349a73b40a2a6121de4e37e0a0b796c819c8e6ba3d602181e46b99380876462e2670fffe02f364" },
                { "nl", "3d738e0ed027842c22cdd25403841dcb6c4f18d30b4c13b6312ec77abb249baf37da0d75deb7b1145231cb7d1c590b73fca7c9f6a09911e9547cd3fc82c3e761" },
                { "nn-NO", "eb837ff03a57ed109fcb1d34f46b318616252149d7c901e88f11d9eafd3efff6187eb738e3e92e94675fe8e65c8308c7e880728d8be51e5cbb8504fdb32240fe" },
                { "oc", "66a0f50117beb29c78bf6e3691c02ca6efb5a7c8468f8ab2c4f0561dca265813131182bc88c29e57806b019bd73ee34b80c3768607458948031dd8b18b009cf1" },
                { "pa-IN", "9fd306217fd595bbbae48c2042d6c6d8572458da23b929f3ecc14dc5448b500e0f1ae9894a5df8f9d295ec2c7150206e0c83a90ba8623d3b95183bd1f70256f7" },
                { "pl", "d18ce86fab58f9c2304ded2c0a8b33d2a0748e7384c96fe252498fbcc176d383a3b203bbbde4fe2a82092fa19aca4b1b701f8387ab250c584df3e31f468cafdd" },
                { "pt-BR", "ea8c9f50cd23253fe351e71ce13b7562a6a08f9f2787e46533055565217d46e06c4842e3fb000c6e4415c56e41b3349d0d300d5a0e3a4fa33e856c9ff9d3ff06" },
                { "pt-PT", "c42be4498ee9ad0a99371f8e2cf5e631dee2cb212fbc70b6facbb2b127dac120e49f8be32f38a21bd18785e5eefa095a4d18ad428bfd4e80a5d3baff4deb9e8f" },
                { "rm", "18bce3776ec0f8cdb16adc1a0a5853fd401105f5ad971c124bfba5252c8c9cfb23c890eb16f5346839fc909cc5ff43fab4d0756e90ea779178713ee06e350c3d" },
                { "ro", "865cc91e34aece48dd44465dc777acd859feaf791103a9bc1e27eec088aae152bd269dd000caaca862ec94e9543ce8b9bb958de618caf050b351ff8405b86dae" },
                { "ru", "6654b65708da60f6f1ce1fc5493bf539c34678df69d2abf18560e290c5e148ebb46003d382f95c1d5d31b0dc7bb7af78527a436bb852891b96c9245c7544950c" },
                { "sc", "40f1b6fcb9831c65b2d853010c44d9d8de800060aece17e30e6f5fc6e8677f62b8a22ce1c47274e9ef0ad61d926503ac5e80911ca8033dcde9bbc46d088dbccd" },
                { "sco", "6eac67faf824ea2f5e8ae04383873375f112e364e87047a232c294f8d725af2bc586f397befd17e324b3f6a282b10628fa33cb4f002b9d65f6d080b39ce997f5" },
                { "si", "f3ea61a7c2db4da1e92f5a74c6aa532ae701b2f8c1365f4cffd8dba68af40fbff969aa658014afb2a1136e739cb415a60318ee98c43be6ee623ef3379cfaf3fd" },
                { "sk", "24b75caf320e9ae8fbc0070bb72205fd97fb06559bec87a0c71f5b962d3c6d5880969a37910d61d7f41df1dd99c89feb913d89d68b4add4b6e141cbf710f0bdf" },
                { "sl", "27dc4fc91158905a4193d197b7d0fec2287985093c335d157164d7d47c081040b0998323b928bb2a77036e492a986183a909c4cdb4b5077966a92e72ef7bc85d" },
                { "son", "70db90dcaae41a9614a8ec68b5f6515796af4cc732a0abfb700435c005602e5136cb8465628d9b621905e4f954d1051b2c607510447d2e87da83be187fe03d40" },
                { "sq", "5f6133bd8374abd9c7c2f28e04065ce32b0aef347eff3124e85e4fe623f612b47c928a46874a5f35a42891624a1f9f55b16edef7b6c83b56676cb114bd483eb1" },
                { "sr", "ec4c08ab71981d70db75575f6534ad575ef507d4aada3248245161a4629254f968f08d02f60d3826edaf5513adb86ddd1a85bb6b9bcaa3f52b944e642ac8fee1" },
                { "sv-SE", "b65ffa76f0df603bc701730ae255ad30a30ef87b4b7276495d0003552760af18a201e0863f6393e448003f1d98b8e89ce78fddd423d04f931e1e58f551f58cb5" },
                { "szl", "253d0d8917bd1f5801e63172fb7d04f4b5bd0f3f034c1e7ffea485f80219a43c14a3b401b0a378d52c02d584bf6b895fe4b764e37289728c91bf2da0f575e850" },
                { "ta", "0ab5545094303ec5db48e3e4c020ba309b279497b9e36835182cbc5feb2c0df6e7c2df9c2e0636905b51e363e1c8ae43fda09eee1424690962c5032007e66603" },
                { "te", "ae300788ab03cc14a41c12ff0aa644c2a59e490a79baa447104d3a455dbe937d88a08c63fd7147cd024d5215c105df8d87440c6244a2467169e369fef765f74b" },
                { "tg", "f4a600c553585b52d7072d8a3ddd12db46cdf64e7914f58faed6d2c27a21fb87e96ec65eb1ce26cc09ac48136c15e14701eada4cb8ac7e345ceb21fe1288535a" },
                { "th", "66df75f7c4f5f0485abb4b40d12a89e86cfeee9453e315a2322a4f6d9d7aebcb4f629137f611114ebc8f4fc973d937eaac6e6e45f3a6da777479e84a2b3b7367" },
                { "tl", "11eb080bb1614ad099434a524f88270d58989c9febb2a6cdf591c872ac40eedf5393b4dfe295fd4204b329e34c1ead69772bd1287f0df2e5c8d63e39e1a4747d" },
                { "tr", "4d58ccb39bbd01ce8721b189a4c0b2b678851e46262cd74c47ae01def6f1d03ddc532951bcecfa433c261f4f5ae8def26074a3d1a7f2d0ab2ccf61f15699ef3a" },
                { "trs", "829a1b113919a613aff72af24d3edca848f81a99f326c68936aa5b551678bc45e9a900b7d4ef74c0a4edb95b0ddc3e3221f25ad58547b835e62f6bbef46ca761" },
                { "uk", "bb6bdef30cbf899efbc396f53dbb3b0ef0d341df08727b5c3b71156d8e5fb23521e547c0ac67b457d41df5bb71aa0d6f6f80a3b532e19a23d8b4cd050383727b" },
                { "ur", "cd32cc667f26c26d0e4512fbbbe3a5a504c66bf5a90fac5f8509e6c167ecda84c45fe2068ec0a77ccc525297a44f20d7f17e991adee4ec2f6c725ef3e9b76468" },
                { "uz", "6473cee0b383a5c1e2d310dfe7387fdfe5c3558e6d24d19ba2805c92d465f6897a8664fc063dd223dfd01f87b8747297ffe1dd95042c3fdd606441ea3f8aa382" },
                { "vi", "98ac6d1e633947498540cd87a6401787084b376f63c7ff913c6cf4e0a0673c52b7b49db674988aa65f9bf116c0bc0bac033be16ff425e506c1739921a44ffe88" },
                { "xh", "8b85918af349fa090365b55c1ae839fc6eea1db1e879c3197aefb844fc9ee22553c6008107404d2595d03a9aa7e8df427611046939a5ced03774dd33c192d084" },
                { "zh-CN", "5e8493321e107f30c6cda1f8174494265eabad545aab360febbe9929eeb43e1c1b5274022ebdca2fb6d43376a0a35494cca9443db792654c87b8a7e6db34b4b0" },
                { "zh-TW", "9dd1ebf5c97e7d9a6bd896c14c774bf07e0a27f51ac107fc86758c822404f2391077575b24629f3eab51eb6fee194cc616b114576331b20b5a8954b5b12cdc74" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/118.0b1/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "17f67c239d5914758cb7b49bb47e870fa983494526465c8ca02703ac90097ad3cb2eb03c6f1634721f3f7d136ad50840daf56416a2561e37e6c1c069ff2c7859" },
                { "af", "dd33843839bc3d1d80194d7b4e92ae6da10d386e7c9e4cd02821cfe2f280b76f40a42d3487f991b3972c4eaf01f6ce172d53de4ac1e108d580f27e928514283c" },
                { "an", "3858088cd85731d9f8734a9df84447a7cf122ba5706c3d18371d06cbac856a9ecd733c1eb3af2aa8f05bc6c277c6a621c326d2af2acbfb61c0d82190ff34db8c" },
                { "ar", "49b8437296e867c070810f0cd6c5daab877a3174a40aab42023ff6129523266aff68f21c06e8779ef8faa9e0daf0ce64ffeba149d064d0fd6c4cb2b6b63ed765" },
                { "ast", "b41f27767c8010a28c602d9aca5d7eb92c6eb6ae5dacbee7537510678e70f1ef8cd02cf445bba924967416b5ad80e13cea400e1c0f13587bc34d93fa395bd5d1" },
                { "az", "4a5e931b2cb5f88163f223fce99da02546d1d5526e7e2c4159e4d26ce0a0484665b0313cd78f9dba22c7f7203ed806a8f19b77078bf3f343e7157f563498d399" },
                { "be", "a2d91700b673cebe8d559abb96ce8be316573c6a9d2ddb280279f4b04c77a7bb55665f5f2541acd18ef3850a1e9d503a3a171825ced5e94f38f0ce19c3440fce" },
                { "bg", "8c2747fa885dae58f85584584bed2459c2ce7ff17d94a728540af341ede211cde9858882f3ca68ee255d2f6f8402fcf29327f9431e4444a85823c156131c7a68" },
                { "bn", "0daab480652f48804a26ff9cc3ca008bf5ef4b3e363859cf4e5d34a2d451e343ca89ee0bdadd2638f2d566c2c391de7260e6081aa6bee3a5ce81fdbefedd3f5e" },
                { "br", "9fb4b102d99e69bb0d16c398c5695f558d9fb46bdc208e55bdd672a21fe5e0c52c7f0688ec1443e08ac17f85f6148161f0b2df2ee955fe86efda9e8754bc41e7" },
                { "bs", "4570e64b1dd1f047c76d20b9d0c3420cfe8392ab6bf972239599fe14e732c214c9f7a598d1ee195aa027e40fc5d786d5fa1e9f508434df258c675b45967d2665" },
                { "ca", "be33a19aae3353e79a6c836465ce52848f7e17c5bdad923aa10c3c1fb2559182687527c4666089d26da0b93ddfdff79e03cb54dea9e91298adc4c4cd4d6b19a3" },
                { "cak", "4d6eeaa14e55ebad0a9d081d38ac4a7e84e089aa8f6d1925620c616d3530545d993ad65f64ad9b18befcb3c85620edff262e63e89c910e9a03bdb38778fa1919" },
                { "cs", "c655d94694e9b3fab0199a13904a8eaaa9b05844888dfcc48c286d4073682fb58105dcbd322275662d983896775de17031f5337639094b1310572516b037cd0c" },
                { "cy", "e703fe947ea8b7036784df979eaf743d542fda8519184e62953bb91e7d6455b911e4e88da2d494d0a3b4dc78458864664d1d1a2bc97bbb2f4012bac5b0fac7e8" },
                { "da", "f4ad8e1d5b66bda1c079df4be1ad3bb4786d6cec64526f80a68e8e72fef7e6dcbb118600f96b1812c4d924c90455fdd806fcf5375c232507ceaa2d1baf8b7553" },
                { "de", "44124e97d614a862f8e1515c149f2086cbf2f4d3c59cd6f9d3ef60b9d88bbfcf010bc3435e49911359f9fa23cec8156e7743d9b078c579ee9189512298928b0c" },
                { "dsb", "9424f51d8cb04f0843c68a50189641262bdc17788d62c7a3691db63fe68e7e69df31693e8398792ad39d2852525889a1de731e03332a7fd0d3da4a5918b19ead" },
                { "el", "e342e46dbfe539e133b6bd41af538197d8e62d84de377b76d7e15c1e5cc164d9147a25bfadf5a36ac6997ac8365f508b92936bc6ed8307cbf5d2cc48afdd300a" },
                { "en-CA", "f7e4a715a9ca4b22b84acbb1a283634dd312ff4ab2e258bfd129cce579f8ef2af13aaf91802813e00db2da335d5844c2a4306b7430211629e25a4693cfa1121e" },
                { "en-GB", "b1ca7ddb039dcd4fa102a2bd103b245c48aeb8da1261cff4a1e7f976576e4a27351c3572db637e93383df39a2310ec6924d35d62490983588d3c56b782422aee" },
                { "en-US", "92fd5b9b0340a37e69d53f2ec4a93c191896d8046f74e1843c7617501dc428aeacb53d3c3400160b73d01f05e96b7ee9942b3225e50aa5493befd844dd98358f" },
                { "eo", "c9309319223f2228b9dc4d2d8bb4175a2b4cc4ed48c6a3cbe8ab1cc7d614e28993e2277493c35b540745b499f6818cce08183e30033d1adf04aa37544706dd96" },
                { "es-AR", "aa7ae5414f8aedbfecf35696e7acc3ac92d8da7fe4baf70e72023599b96c0ec63ce278f1bf28b71195df9c39dc3a63623e16c71c57a8ec67d412b813fcb8b46d" },
                { "es-CL", "314d251003e9bd685f164c3cda3beaf7821fe6fdcd37581e4ce0aa4e314d9275ddbf7f1301107bbc81dfa727082155d463ddf90ea2477b99a8cc4e959a455292" },
                { "es-ES", "bcbfb0dc817cf009a27695b87a8304549a910f69230f723d77cd2cceb7ee71f4f1f796d8deedd6b10d8caba9ac6f6b35d8da9af85967982cece6b7a74f2d6865" },
                { "es-MX", "0462e12ee09bc3c8e7cd4be51122e7c1cce834dc65ba70e0414ffb399f6fdfe9a04a611c57613f7bd21b7f7255656841e2b3c8fead24590357a00472241008fc" },
                { "et", "cd816cd3a7884a047e37343e7905d7d8c464159bd3a0600a70fe3c4ea1566899208dc911be9c9e072f3069a439314222f4aef57e2554d311b803497609a066b7" },
                { "eu", "556dc7077c0640ebdb12f4ac74e9c2067b33a5bd9b159b783eabbbd21e6ffc193a592a82fd71509d22d10a60e107af81d72ac8dc4bd2466e5ef3dfa4e0a88364" },
                { "fa", "3ce6585887968a200e422b3f2ba1cc9b34e8118b9f2c43848f75d2ea0e38c8377c428ac4798cf2f4047b3c5fffe1db00bbcba12ddecef319b7d557ae44503302" },
                { "ff", "e803bcab3908193c343166614bcc755bf6318d7b28c225bbeff87b30971c55be202406ec4ddb4a4ccc12ccfc0caa0beb8ac4bf6e8392b137224c0b9c92403dfd" },
                { "fi", "c5987928e00c116d3e2b9b123a75084914c8fd48aab6eb613332ef70de2d51ecf19d2b20f3db9890c37237e5883ed95112afa88a2f807259299bc1c6a4dc3182" },
                { "fr", "d965e6759cadd266b707be5556c678fec5b4b19f8abdcdbafc4173f2fc68fdc782ae33c4424c20e35821e6f4f6aec0af89617add6b94a6f98a177994c22a22a9" },
                { "fur", "1a9d552ff840a8029b486eedf9cc9c8a714e420d02c67b77fae3e086e01f02ee694f30eac65c43987e351af87ee6551caebbd5ef873435f21f62f98a8b975c06" },
                { "fy-NL", "9c00c2a96a82e492ab64e12027f04e20450ce7a38855914e802d690a30ffa86ea4093a56f84cb5e5db9b92e0ff882865ce8c9d21a78bb83b8d39174a85c3f4e4" },
                { "ga-IE", "31a9d55fa11f1dbecdef532834e41027707b18ab75e86d7a0a831253d81c7281ba914cfbd4a4aab51b73ce380d5a9d2d3d2f8a06bbdec348f75d070a68f71f7a" },
                { "gd", "710f5470d219d6f1091612ad3aee74eda7e48477b99e8a1efefad62b7fae00fb93399643c948a02244abcb3bb5dbcdc28cbdfc52df30b109ecae3ea5845d22b6" },
                { "gl", "ace57d7bb3f0f34eeb75dff3c3d050aee1ee1ee7d21db8ac50fcee3bd91473af712885b4075d7e6fc13be4960d74296352907b3b9cadde76ccd195b79670fcf9" },
                { "gn", "28ec4c68396ba158f95ca9e3b230fc2cfd8b392823a8b348fecc5f78acb3cd0ecca82ca738e282e6431011e8096977113e272c31c6313920cf4162a0c4449ff6" },
                { "gu-IN", "69a8058ebb3cfd0ac9af94ccb0c18b9764ff04938933dda53a462aa133f7a392525f27ce7b6f9565924d9ad40250e393f34e1492fbb6a89e3bbfdd086a52ed41" },
                { "he", "bc3f8c53e3d9668d4f8032ced82f9d88fb0a060de355a66fb27039dc362bb0e5b5405cc169a7ea60a834cd6a240efca8f37130aad3a8e0e540674ff756b6462a" },
                { "hi-IN", "25a9e65ca24ea6e321248ce4a1052b53d1757bc75c74784ca2b854ab8cf644b1e5f661ff0df70d6f3627f6921d767319237c1cdf8909405edbafb0051134986a" },
                { "hr", "054d5c6c33eee3f4cc472f99292eb18fa6a0953e2a123d613481d5db6bcf9f5d207e4aaad8b2bd27fac134d8f7361a6dcd7109c5d36e8b84d902c8382e9e2876" },
                { "hsb", "04f7444679b646122cdf697326eb604b9db0f74024893a1be9869003c9af287f516ca23073cb153758ed08c0b9a5663ba89eb0fd8152c37b1535506cb2f48e2b" },
                { "hu", "8231709c5595ae8087981f85278fd8b94476402df32402b8f2c9470864020aaf3b84183c8314c057bc0fa2989e5d4e50e0616f30d960580a148802606aa0226a" },
                { "hy-AM", "7540e5c4aec1f13fa01454f8ed2521773ac507a16c00f46b4c708a3b455f4748c459463ae3d3e0e78dda70e9ad6a85cca42a9aa16e2cc0c27dac38cdd6e3b661" },
                { "ia", "15b87d961ae2763db3ece40bce466e8ad46a0d5eb7461e7c9440c6649a730c414586478772407fb24ffd76f89704caee8c8b369d7700105fd88c1462af4ce60a" },
                { "id", "93e179f017f048948d37bc19f6ae3348255f8eeb0f3c4a9e54919803950ea5304764dadd57aff686b6d232c05b9cb4663c3b695c47d4383155d149bad1827eed" },
                { "is", "92b8f19d1f69da63155376e6eb145a5b940c49316116f1186c229494db4a19a8a2399ed38939c0e28526ca2c80c27d134ae245083236b2d2fb57f8909894b57b" },
                { "it", "e84d7cbcc56f516ecd1671f1b2f20cb900ed0722eb22847694d797dceb9c1320ca275f8c4782bab76e207af4bb718b341936867792c4ea2adec33e03dd38e71b" },
                { "ja", "140ec244baf6d1c488e6f0b84a0c2260a4b1e8e24ae9ef99d3aec60f30881c3f09e28e181386a5d204a0b6d32980ba2a34edc5f17eff3f78ca1dc54499f51d9e" },
                { "ka", "3b14e1005dd5d649f31e3c0679580f3c985f723f2aaa221d8c1da2dddd7b7b67923ac6207e2dbb193f3c4fc7b06b3db46d1cdfbaea23bec6e134db0b7982d66c" },
                { "kab", "07c63357f7d034757785230a5611210347c87504512f2419ac49d84219e7ca45a66320b0cf49c8dd4462a6e73c63093e4be2b39225f9fecd5b7f24a93a91487b" },
                { "kk", "515f493e142f8cd9f689819a2b90b5b0c02c1d3acf8e7475841aee42588bd25e74de9dd9f831baa301b5215dfd51b2d8d02f97f73c3e8f7da14e8bc420f86573" },
                { "km", "b71df18ca73b7052d5033b600aa1180897e0c0c63bfb08e599d7c5d8f1ebe6a679b9282d74275933d3f3959027444d7d3bd28987388be386b3f0161fd8a14038" },
                { "kn", "b38f0aff78456f909a2148bdf90d52f0ac30eaecc3ed781bb00d94f320171fb0460234a71248c0d904bf11c819513702fa6f5d23e1c6804a91043bf5a0b7fa76" },
                { "ko", "cbc38da779c40f3781bbc1465f664c8afdd0d9277943d33806f1cc1e4d677d952f6b34a50ee0d8d1b18bd18408c59d6e02b9fd308b362199141843255c05be32" },
                { "lij", "f60cec96ef17f2d84f84edd1b16571924b488cbb653591d2433270ed220bb1eda9146dcb2f70f300101fcca42e3c86c3c1f03f8336858b327bd9fdb764dde1b4" },
                { "lt", "07bb6e3b04766da354616b5b519e6497423e29747aa0c81c34e540d20856526def8682b17f15a4d2b5d6d767e76234756edd0d66fcd563d1934aeb65657e621d" },
                { "lv", "e2d340dd846e2058df810d7a25ba900eda6ebb5b311d50be8ddc42064c53daaa6c6e2a99e43827141e9da40e3edc15ad97895c4d8794842c3de241e1592bbfbf" },
                { "mk", "8339eacaa186eccd21d25bd741526e2180e048603fffb6c20750ac1ba851c2d8bbfa4f2aeac30ed731510e415ce9dcb31cafdd9c10105ca55eafd5080d19301d" },
                { "mr", "e8c4d9061ae8c0feb0375624ff101cec01799e88504f8516229351184b7b5bd3d1cd63a6f886e0b90f7a6616471223a5545305b18179ccf5da27c13ff2778e10" },
                { "ms", "b82fcd040e66c5cc8788ee9f23df45d73d5846c2efe7e68be9444e78fc9d8d0e1517bddc026c4ed37b3a5f93509711d536fb0f7eb2a397080ea5564615b8ad45" },
                { "my", "3797fbf6c44f42f2688c7340c57c9e8003f31eea827997ca0b4aa7d0051719f9f67ee64409a20121d08045d610548e7c28d5b557bc9bb002481f40c8c1f6b309" },
                { "nb-NO", "2861614d4935d353ab8bc653e55f6b9ac50cb7494063a82e2a8604b5945c8bdbac7e75cfc454ecb5f1db888aeabec0e56985fc5bfd298c14e5cf62714938049e" },
                { "ne-NP", "2880208f735bb3c25047a864ded26269ae34995d45d68e2938f1be46da7fe8fb50ae62ee0600c01246b0a548af3fe6d1d8d13db694701560b0a3408bb5f20c0b" },
                { "nl", "d6cafcaac4576b1402b296237483d73e4cda6584f8a42fd44c4e05e7eabbaed25e648f9ba9c93942cbd2f7b668a115f97243e7459396d3c8080690ceebfbb4e4" },
                { "nn-NO", "c54ffe9a5d632f7e13bd5f5d3a236434a2e4843c9a0f4e9d7dbfbe7f81331fc798699cd9e646a91268fc78edf716c17e4ece821600d94224ccd4465b3692d75e" },
                { "oc", "347e4b0b14ce5132ff4b6358a55d8aa1534d99f63a57997aae0be60932912bfb8cf51740182b3c717e5ca2a7a2098748bb38229b50438dc0455b4e077e14be49" },
                { "pa-IN", "187ca61f95563a8445324ac33cdfb645f810f0c272b793c273e841b5bcece540a0009b8b6de1c7fcfd4e1ab716add717aa91617b6dd6ec5aefb9d960a414e5bd" },
                { "pl", "b23843a761d878f49332990c68193e27d52cb765f4e99883e1d5ec22476d160e96a1cb44e611c9f69e37e441312e35644e0a868ee6dbf302d94cdf57d34bb252" },
                { "pt-BR", "5246e9d4257bb8509b104a57e10bc86019445a9f200c918a5db9341ed30a8efd27a30b8805c730d55e0770288f04f2d4b02e2d8d78b7a6b9070ad0e9dd2d3627" },
                { "pt-PT", "e5eae18b89075b6394b4e09bd29b09f87a2e8f7eb01fb9c04f4415299f5191e957c1da20c92f8e696fac333bd598df2a283f2b9702e3ef18d1423a8bbdf713d4" },
                { "rm", "e7a0b89f30bc56f51a403a7c28a73c7e35f24a4990afd4b67663a1e87a3f58f2a274a3ad1a8c66a81367fe0cc223ca32dd902786709d61f94d8cacbf3a7a68a2" },
                { "ro", "0ff4d3dd8f35729cb60e93df7a66d8ee6669148ae40012b61b4a7134701502ce31cc55488987cab4fd357ee9e560be683c9cd0d3ce62c03915e86acd821b98df" },
                { "ru", "cfc45e531d3a4c670c087a137f546841d4d6d8c7aaf1dc8b2c07cbca0fb68bc7ebac0a6eae48aae70fb50a8d667fe3b677b1c729550e96411698916a52e87a2f" },
                { "sc", "21e40ec2a3700385bee674f679bdd3bc760a3c6d490eb9389185cfee9095776ef07bcb5bd37ffb87b4b6c30896780bd441df4562ccff6983d04666fb33e13e82" },
                { "sco", "24dc0df57a86e82d633b1f2c3e76bfe9bef6cd02f809dd3e2226487fbae355adac101b5cbb9849176464266085d913f105cf59167f3062d4444a37ccf9f2511e" },
                { "si", "6f51e6350976545332929fc28a47bd2180b9cff0d2ad9140c47be8cd84a995178dbcfbd277f427385ddc15a75986c069d5a5c9ec6aa82bef588c8c2bf2d0d3fe" },
                { "sk", "c5004e3027da383b1e85bdb0cef1d03f58e5206ececf937c20d59ba23b11c7a71d35cceb2988d344a1ea2430e2cf6d8231f82646dc581a7083252a028a9e6735" },
                { "sl", "bbc2ff4a4bbab4b3afa96e9faeed5ed8eda881cc604f649d43e6dd9e32473766cd54c09e33fb0b237f104befbf440aab876cd93286eb8b0c8461ed47b49438d9" },
                { "son", "337e6d52aec4c9d5a7e48caf2bebdc5aff84a62bdda5c8011908c18b48ee5c0915c14576d5f7a0448250315fa0b364b29bbe8ae8aeb46bfc59218b895da6e724" },
                { "sq", "e5c31fc7419c93bd5751a9105c7153c485478498ef71d141b134cef6715f861bb556c3c55bd7517e0991f6fa1a79e3e9beb8824423b76c37a4f9991f7e853954" },
                { "sr", "9450e40c1784aafa759c237bd9d0a0dcf4185b70d9249ff8dd6ce5df3947752510324b539830281bb153455feaab0b0850aee662884e2ccd685185c04b33ec9b" },
                { "sv-SE", "53ce9b03c68b30e048b27887c9bc1f8a85fc7dd43c37e0401ef08d5dbf6c506809dd84882d4842f9e1647ecd35f55796a1bdc0ef3af651125041188005d4746a" },
                { "szl", "5af8c14727f987568d3293eecfc41922d5fcab5df813a3ea139db6ea7c7782611c093a558992d86d80de6077e3a711614b956c9366c8a28cc98049c72563ccec" },
                { "ta", "19517e6b46cc398a5b34f6a3eb040821a5e8e9b65e76b7e310dd4f30934edda16f6613e8a6fcd90e61af90d5d0c8e5f2ed6a994b57b9217ee6a627c62cc22a80" },
                { "te", "c0bba13144e83967c956224c6acbda4ad1e788ea0f1be87d6966d0074f23d82cccf850b4823908a5b0283ae422fa9028a4477b572d4e8911183ec551d9c70970" },
                { "tg", "16642e455789098fd1ffc3f6d77d3678a16eb49291e60fefc323e3f1acd8d27a32716e4ea5cacf5f6d45d169f11368c67bcfe93dd1ee45ba903dd8745fb1f757" },
                { "th", "d9db4839b76f683c8c6e0266a7b7df0a7c24ae886edff72c3bc37077d09a554d8bbfbdcdbfd4931cbfbe3bef0fd3da98c67e3f4fa27c1903feab1a1537f14871" },
                { "tl", "be74fa6a4c437c34cfca04d239a2c529049b0446bd8e2e30d6dc5e866b2d71b29c0804b9866f75fc7d1a0b795cd8d2f45bb7d0f0fe43f730cb73d1361e073cee" },
                { "tr", "5cd4b5be85c2b5d86adf32610ae0cc64c3e4df84447c986a4e7460ff33d479f0520261a869185e1afabdd8a44b444c90dcc78fe2b6f26674ff00a2a8b7513ec2" },
                { "trs", "66eec98eb82ba155a3c9017e8bf84067a1b335eb2a8e8c0976309061dff636ea6576e0bcbd22042bdb8198bede5e4dcac654cda1b357e4e7227812e2f56da084" },
                { "uk", "0a94951979e6925031fb61f26fd36936f5c18fa99fe37e3f8dc96bbad815f4ce4c8a4eccfd520a217aa7d2acc17a7b5ed0e74124110df53063bfb5e33f64b6b4" },
                { "ur", "abb57b6a56d3e1aedfe35f72da01269e715a9ca9854b74c26cecb8bd989c262cde11df195ae20175943e23db2126befcbef22390162bf10dc0514493f9f867e2" },
                { "uz", "962150768f50d36f09fce06c8c2b89a24f8f33415d40dc55ea28d86130a3355286868f38f4e2cb9336b020eefedc05a1efd80d11df5521b950b65814578ed1f4" },
                { "vi", "35a5bcf065644821d22bd077699b87dba29992df8919c0df5f3409c133f394af2a4d50b23bf1bf0a3801c308e7307029dc1169543f2617b567b8fd332e1020c8" },
                { "xh", "a62f8f29fd790a6660f35a6e3aff029aa489a3fd931af772d10b7568a8a34efe54c489bc5eff7b44b8d3475d13a259f05cb14b404dada674936664e227231a30" },
                { "zh-CN", "58885b725298092bef870b01a7e726c8847b02e9de601a927b66314ffd75ecf35f1ca8e9588c7e95a5cabbb88fa25a74f6ec7c6c9bb6e981a4dda79bc9ba05ef" },
                { "zh-TW", "a37e22126b1529b1cb5e314b5f9960395a72a2039c62f4c34119d269c20e6096808eeccacc01aec0a41c1da55d5471687db2519a0fd7ff26d614769f974a500e" }
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
