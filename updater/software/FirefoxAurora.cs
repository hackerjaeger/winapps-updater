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
        private const string currentVersion = "124.0b7";

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
            // https://ftp.mozilla.org/pub/devedition/releases/124.0b7/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "f413af42aef6232e5953e16af1c0da3bcb1e6d99ad584e5678e1e0fe2a27f688fe701c64ec9214f455bd9788792fcbd327e170838c5deb1d0bc6510953125015" },
                { "af", "be7fb2fd9f497f874994411f0fe23d7635c82ab9bd160814dd9304c030ca1b0941ebdefe0c158d9eabeb151f8317285ecac472510784c50aa913fe118c46d325" },
                { "an", "b79652fd2aaee67c05f8f5d848eba1f2df5c937926c694970c9d130447e5ed0fe0192027b83883dabca8e17b2376a5f2121b1280f81391d563a4572e5f415042" },
                { "ar", "5123316a49ae658733491333dcf0e07d45ae6218b79db4c7900b46e90fac6537515bdd5f4bf43ad25809b61993df79baa0ca8e631e6115df65bab68de43524ea" },
                { "ast", "a1a1dc63d49dfb554165497c40f504becbd35f89835d90938984cbb1d417b8024c2a973d82b4e8f5bdfe2adf721b8ff3309d425ebcc82db3b3df15d24232f7b3" },
                { "az", "214ddda6ba89b2095bed3f015a8e1cfc0d08867ff6b4e7154ab89a395bec0411da47f79a67236529252506a66fab13e70ff4c9f7f7abbfd9f7a74794b34dc49a" },
                { "be", "7ebc7dbbe42aa00f784ccaa86edb1cc50288adc4f5a825304426ca613178579575802603ed2c992e06efd67deb91d6db25b64a348764879fcb8b71eda2173f1a" },
                { "bg", "04adc81eb939b0a7ac485dde7007c3be061fa888a7831015cb19345a6685121dbc5433544f473cf367f080f0d8fce76d914a41bef88c0af164bb2d9f88c048c5" },
                { "bn", "30c7c369d16b9cc014b3bb7a146cda9c64a8f74c83ff69794343cfb0b63a912a4ff0a7c5ab94942d8ec786c88be5f0404a0658c9f7b70a023b6c155b4040fdbd" },
                { "br", "46e6ed0382ce527c1ce9fa8cf510b9bdc1dcbc63d312d130c01d485811406e74c382a871b5267da05cb9b9055ec29e3ddd274c2a6f9dda596babea9e5243eab9" },
                { "bs", "dfd5670f7b3ef5d60512fdcb657ec14cb88ae9c2c5fbac82ba25ca0edf47787da9b0a0a7220d8bdc3ff2a3733db4a21187172dacc312ce9c0d43348eab708da5" },
                { "ca", "8f936bba3574f0a7a785a5c58069fa98254ae25151a30590ffd22e08eef8718e19a67f1290090b48cfbc0dcae7f2e500df5a67b1bdb2859ebfa5e2308a9fa4f5" },
                { "cak", "459dd8c8b623f9ed581861dd52104a3c785e502d51a768df57067aea4c59933d3fe96718396006f8b8516300b87f846a27744f5b7d709400ff1b5acc455862f7" },
                { "cs", "bb09abbc283ab78420b1bcaa0ec7031d6f889169b107d45a5da33f57a5b5e0f6f096b954629aeee18b10690a0b6d58867e3ad55ae7b3ed222970c587d5eafc24" },
                { "cy", "21f7633faf87a1e302d778d4a1d7171a3cc029cb1dff647c7b5df3cc49646b923be17397ef94ecd58e2786f1f57a5515d135f1afbb1fb6421afe3d8bb026747b" },
                { "da", "cbec552ed0ba8c10e80052a9d338d1d92583f53096aca188d212a7a330887eefa10f5265337bfc1359ae189efeec7f457b073f0a79e3576b79b78f755a50ab5c" },
                { "de", "42f4d09b07c968bdec22947e167d2fe5013e05517b96e5c01b44ba8090f1902826428b27e35bbe32c664002a8ab66c7ca42e09e69795f326273432da0cce3c3a" },
                { "dsb", "528f306d4756993ffec47e890cd701e55788977a1b6238e4184fbe25acc485fe29ae643f9acccdc7ff1d06e1a286e52c3ea9b90fc5cd1ce01fad7bd896a41bfb" },
                { "el", "0d9862a69726fec9e5f1df29ec477e7e3a4c5956dec0393582bde65887817bc2d9e1ed7fd3b7b5e1dba1e073be5ef9b309256dc40338a9e8d474cb615e389f47" },
                { "en-CA", "28797fba9b18e3945501ada3997392b8318cc9178df1584ee6bb6022315f6448d39e3eb4c2cc53a9e16f6aa5b7731cd278b9cd90835d865d8b89553d90e4b1f2" },
                { "en-GB", "e4be2c1b7b5a90f34449415b119e73f8904ebd3fff89b9bd76045d9c221e65f3c61d50c80c1fcd2320d5b1343a5a73c8135a3deee5dd5bfb4a91ad2e83811fda" },
                { "en-US", "2c9223b79d9c38c71fd3d1e312f4a844ecf94a4c2a2529270b64421b8925109dc930426779ad9c23929e1df9b0f562d946550723e936769b6f9230fd5192508f" },
                { "eo", "23b61858f856a09a371e198603b077ff4670f7faf1632f958fed160e16f098a54de341cef55331dc96ca1ef6c9496ce8cbf3466ea58832506e36ffda9e1a9901" },
                { "es-AR", "4f0de5c050ee0b35020c4b879bfb155ab8bdd61a1bb218fbca9c1a442d64ce67dca9c6e91c55f833b42d072daa42905d71a17ab690dbd92a3ff5e14d6f13b736" },
                { "es-CL", "a8aea07e5915e13ed3de6b23a86b3e0df2d8c8887300882d47b9e09b01809772c029384289ef7552fa7bcb415265a42bcc92ca30ba79624ce9e7f14318d284d3" },
                { "es-ES", "e42b27f6e795ab018ed2b21371e1b19a7874c35fd314ff999dd3ca95e924c092878a982a1671fe78cd8e5f29752a73216a0027321b29126b6fa080abb090eb95" },
                { "es-MX", "8480c7e97a33e1818a043dfb8104f771a44d736c23c5b754f1df6543621c5c6bf7a1bb8d6904ca4957465f1f7ebd75439aea549c026298906f5cdbf101b91542" },
                { "et", "b397e8647ed12365d48ed807b518098db632e9b2cccdfc9042fc82ed1537003fd68decded005a48de096e9c189bdb232d377bca4bb2543daa189c86e6ecaf908" },
                { "eu", "4888c95a57e5a715b85a698dc9bd72ce92c62d1c46f9b89a4ba4ee19df39e1bb44f01a42e055788081a3d3fbdb3c6423f885c684055e298afd862e774987a7ca" },
                { "fa", "319fd8811f87a26da8c7939f0e4d429ffbf247c6194a63fa8767a6363c1fec0eb1320eee29f8c88a58138f5d1b561750d0bec440b44ee287a677bc1a7c85e9d0" },
                { "ff", "6700a57d458a6367cd448366217e94fbc8bd6a8bd50a8b6291022096ab2c435d2b3a7262fd95ff03eca2eae7e2afc34fd22eae1353358868f496e59b95efe3c6" },
                { "fi", "6d5df2360107189a510e2b3f4abfdf90f5b5de092e5f85f8e591947c9ef01f7893300abdf7c9fa41048c55753e11d84e288862d77047fc469817a7907dcf067b" },
                { "fr", "75c7b522c08f45540d3de92af0a0a963d6223b2ca178de381b7e65ac95475cfa40d801fa08c573e040ef78dcc3bc12dacd0e07a6c77fe91b13449832a5465501" },
                { "fur", "77b4504f3dc2a1359df2da65e9a357f525c3acd8c96860d84a028dc50404abef65c716fdafed06197db9fc5ac64f8244a27a939564d23b4d634de0c6092e58d5" },
                { "fy-NL", "ed27b3e643782cae064564c467a7277277ff232b893d1c3cc7932c05d7d82b8e7eba9232615b48aa1eb8ffd4d15233d9acb391ba79dc31f34633c7db04992c7f" },
                { "ga-IE", "56a9b39c554c3f084d1fc4aedb0252101d369af542f3bc12c81b2dde86b1b01b97c4121d42719de385b570e3f0577aca388ec55ff578bbb58b25e68535877779" },
                { "gd", "7c7c2fb32f2aa67e653f740eaefc7d0f12258f3d32df8dd102cb0da3102ded9f1b0c6bd1e70773449ad0b6d946ca14cfcfc957b74fbea2e5fe9b17e355295ccb" },
                { "gl", "d2a4fb1e6297db70c9a5df34ac303de137887f61eea598d7b334ab76595605f01114d0dab3fec8af555be9ffb5206959bcf8598e31b2594e4609f5cd9622dd11" },
                { "gn", "2f9cd1b9765e127886b96df6bff642d912df4072005ffe99ef728d29a5b78dcbdd34f8af738b2b098620e76761f069b940a549839a135312fe5f52d42334cb8d" },
                { "gu-IN", "cd5a990fafaa3c8c23869cac68c04e185d29f0775cc9439d387de8e4a81a5195cf7e44375c1c548dc708a5ef63dd135e1f8e0a24952d0e8e988a6091146a7dda" },
                { "he", "b7b8cf975cfcf034cbfce851a28d2ce5d0defb4ac1e970da25f0ed337fa19011774d143a48ee085f30a3590ecaee79e4ae7bf4f55ae4e208c6a4f9737c3d19df" },
                { "hi-IN", "cb9a257b479ed709335cbc07893417366a25917dcce68d2755351e2252cf3ba08e8d43686eef9da0a4c8539526c978440f8c18af2fd94f5022a1fb46a0b015b5" },
                { "hr", "82dda5d0cc9f11d38b99b26b7fa134721e7aa5ccd09c20c1daa5c8dbd2927d1b70e72aa310bf0d44c23363bb92715e7babba6620e5cc55f6f6844150e60ddac2" },
                { "hsb", "b548dd8cebae308cbc0f436094af58fd00b0ad9477dfa18e0df7765338a861d6387cd596756668bd808c89fe1bd3beb7cd0c59a78776fa11c67eb70b63f183d7" },
                { "hu", "67695134d61d9224947880d6057d1bfb7c54e5b1fc1ad39815e87de78bd9003be796e5fde017a5eff6d52f2b327d0c27e503c76091ad1a89f6670fa0cf3cc289" },
                { "hy-AM", "e8c353a49e28d5df0ee8b78a1b25f4ad5dc036060f4f8e069aa4878195a3b8f8a8ea257633b533dd8f81869f341ef6278f678a22aa4af79878b8c5f778e5249f" },
                { "ia", "cd8d764431b8585d7a63217565b97ef66eb1b6fce3ec293d2ed2aae44564ed72b6b0c9a53926ee2a62a6d4b54acfdc403e496d473ce8a86f78e49151981bb935" },
                { "id", "0ceb7d8a759c19fc4971cbcf3c1bba71b0c972476935ff0850bfa6c34749426acdc2a058763ede1aa87c0bbadebb86f38898d7210287954c3729dc301bce5219" },
                { "is", "64ae2a182a88c782a04c8ddc5270802e69d0bd4f53bb5ecd7c5e7a115cdc43b36f798596bb15741fe7804bf3102688b62e271ee0f98a92b7b6a24e409e6c32e6" },
                { "it", "9ff3c8d0b173c5ac0c4e7f38fbb86583d1b14516058bdb0d016ca8e55f1503d8bfbae95658cf269c7c265a698b2899cb1c5028756550095e948e25b5fdf132c2" },
                { "ja", "d2183d84f62b9a54ea2bf6fa9a965da396d041ae80c030fce372ddab96013e7d414a6f2f85f57df294327b897daf4876be6d16099a383e19e7d7e59adb54bf70" },
                { "ka", "6357e0d17d08c3e98b6d7c8adb8d76af48e602ae9b4c04bbe978b62c4ad5b47bb05d61b605639efa0a1ac37ab41b40f10182fcd87a19ea7bfb76abeff4cea498" },
                { "kab", "c5e2834f2b66d3420f8c08557caa776e9cccc554d30340e043bafe03d8d00a400fc2fb52d489fd72902f32d5e4de57560e4a802c274943642ccd06538963f54b" },
                { "kk", "8ba395a8b417ab0595a128644bdc7a71c3b59d4ec2d648206c44166a50cc094c3148d946a7aa1d76ec3d1d9b5fcabab92818a4497b67c6fc3850066f158624eb" },
                { "km", "5c96e2696f3ab8e8524858d37cd678d0cceacb40f5f093053bf622943c0bcf886c14442fbc72758537a72de5845e2060c4744fe1b2d345e8ccef29b179e9b3a0" },
                { "kn", "359cb5cc5bf64fc3fa589da17b1b8e57c8c3f3d869ccfd6c96efa9da56fc50f1ed32101331115356feea333e418e86c6e4f0cc2683fbfff4932a23c9ce8945e5" },
                { "ko", "aba723719c545244ce761050cfc1ed468531072c2253a49822e748e674182ad887707245c833a494e73fd6434aa3404a5b512e50257fb0ab6355759682dd229f" },
                { "lij", "8662df818ead0694ad27f38c586720ba04d73ca5a0ea7a7de333e978762b504268f73445578d24508671844c309f405278dd548ba9cfb0090d2bbb27cb62a1f7" },
                { "lt", "7c639d1da8f41d3fc39fa441e2110e6e8762735fc9f403018c92f1e4fa062ee50e20f65db6ad94fac9f20818fbcf567768c7e87dc9ab79d2dca4cfe1f63dfa1b" },
                { "lv", "910e628a64afa48d4160db936468eb90ed69d80c38b6d78c8e0c771c78b0324ae2ec2f70c4cd6df7f74e7eb0809f834027c6ec7f90a6038a1d3c9b355978156c" },
                { "mk", "ba592e51ace4fd1e48f9fd9cd4c9c88f670e924355a632d2164d5f49d506cef0aa1cb293b6ef9049a2cebbaaaa927e9e17b3ec8be358c2612d33bcb4758d34ba" },
                { "mr", "ba3bad7a719ecdfaa9ffc6d1ef2238f0324f80a75ae11b42e945e0d7c27562315a3ca131bdcaa4508d67ffefeb55db6bea92796306081a2224a2786ba2df0d01" },
                { "ms", "19d1e9ff1a2e25f1b808d0dd05695816f844e9d74e4d20afa65ef85325f309a380943100f06a0e8349d616d123d09824ecabbb6eb31331a1e0286777b64c6673" },
                { "my", "2cac1b1181ad841ae78b5e1f15df09828e6aadd019365b9e769a7bdf22c20e914ecd57216627a0570d942faef55d29df9b957d7caa259e380d55c1b5d32affdf" },
                { "nb-NO", "3c146a5fd1fd38d2ba1b7b75568f4fcf62bd7997faa831d882681e922077f2e85a18cbb5a71cf028345ee8c5bbb322320eaec7fbbe352a54db523ba85b003976" },
                { "ne-NP", "2a036de29b939785c0750ba785993a1c04d46274b7a1f5cfb407987e94cb56fe75e52a99f5db8e07f0de2f2f219df47c5df0bc1755e14bc1148ffa19e1bc6374" },
                { "nl", "e7fdc73ab56cf2691be5a391fcd181be241f7261df1bd1f776d3b2a7498a92c3721c40d3bef02b74df6d3cbfac0140d31b3dc855b3e830aecce3a7fb5467abdf" },
                { "nn-NO", "d6d8558d3f15ac0de0e8ee66bdbb71989ecfc6d6f86f7218b0355ba2424fcb14997ee33e285ddb32c7349faeff5c83f33c8ab6f4abb93a5941bcfbc1741ed176" },
                { "oc", "508b8c701f449fe305069cc0399812b1bd2fd48865b5055eca532645c533b0c89328b6c53a49b96cbf459548dfd3129196cb709451e1821ef6831f7e1ad0d1f0" },
                { "pa-IN", "f7f04f443b44565a8876d170bda5b51dc0e1e7042a8d570204077777f48635717a0a8e6849b79f0bcfe477e654429c23917a994be8c4e96ceefdb32b5ee8b7f7" },
                { "pl", "90fff021de7d4bc039fac4422a7c7446cc6bab9f3e8c0c8f0b107958ffc5cf899acd82fff5afb632defddee44e5513b662c14eeaaa03079ab5f38f835505cad5" },
                { "pt-BR", "c4e6a9001d9a2f73876b37b89629283835708c1f01a53f0f0372367ca6f9764dde6635d8b6baec5c42488f426d81adeafb38939299bed7b2cd543827dee9fa33" },
                { "pt-PT", "6da1630ee9d19672a144ba96e50225777360dd5273969fbea876048068be153ccf1ccb19ba0cb26bdb30f65fa15fdf53834293f9354ee46793ed7156d03de044" },
                { "rm", "c21502a710f6a4bafec209b144e053dbbdd6c6df87bf911bc74d29f7a3954a49643406d6bd8807abaf5026caf73480c22b60dd907ec96f80d8d5bdeaa591b143" },
                { "ro", "f5bd88048b2340d8a1a28548f2403aae567430256d47f3354e2457b3b4f7b0057dce122f266065707f6804bfedb665b80ee27af6bbfd6f1587461c2d2a14d417" },
                { "ru", "93c7592fc0179d68ed8bba852fdd12964108bf19e8fce953129996a851653cfd657173b7e0d2fffca8a5390add0c786f51540687b310c7c6d780817f08ed8fec" },
                { "sat", "87afbd2968eac5f91489238eeb0e233f607d504851f5bd4c46d5a6ac4952305063d6f40d4a0bf408be100b6e46a5b04088404cde3cf0fa02664430603240d1c9" },
                { "sc", "1c3c0c30753db546e9b8dce00cda3ac321c1badaadc14834ffba19890d869c2537ee9f6e92a1b0b3cd527068dd7e3082b6dcc36bc29e5a2cef6998132549625d" },
                { "sco", "9dea9a0a1163be3e94903818a08462536b2977073cc8a7dd62c7d2c3c01ea9d716c9526051ed050b0600f9dd45cebe4935004dc1057dd2ae61d6e51c459eb70d" },
                { "si", "025b9e074339cc9623628e70e74ba20caff823a1964d8963b60c72e2fd6a46730d19365478b24535b3f924eea6bd5f1f5ff572278dc6674bbc5c203046b9941a" },
                { "sk", "df13991dc4b31a60e95b3f4184cc98131e0cd81959656a201be51144a80abaabb8a1844bdaa9f6462e7d119efd689caa0db28caa06e8ffc331f84f4a1ceba933" },
                { "sl", "659d24e90331eaf5880ca34eb150aea47e352b534217ade4d42a1360f9effad92e6ef81a2f35ef091e188df357b334d5b7ae3962a22394846fc5d41ab8c344fd" },
                { "son", "b1e661fdb70f8fefedfc0c05753719e2aac4dc840e17986b60ea134528300e9fd54ab2a7e18923275c42f77a1a438d2632aa2e82cf83fa19bacfc758b16662fd" },
                { "sq", "b932136728904a5dbd6d32fa9614795b3077afe48523fe735c8471cf8d16b291f884c6b8546e383c8e0218c122b03fe5992b938a6c222579985b161ee4ea12ac" },
                { "sr", "c891d9ee5aa6e855429eeb1d0b3fa0533690a5e1a7a87338a2516f5a5dfb129db00b865ce42b8c3afee929e7a6ef21aa8b6e044dae4a047b17bba5b66dc6a8e8" },
                { "sv-SE", "3737f570f4b90aa11148634a14bbfa9b7e4bbcffe2c11c14e602ea93ed23206e98f0fee83c10725d89f82cb0dd9ec84ba3e10fd1ab2062a554c87dee105efa31" },
                { "szl", "b0f45750d87fccece4a94748959299fd6304bc0793b9768233a91665374c9afb7ea348e4171e232a1b43fc454bc1b264c774d8fcb03faae7e411e722c7e0dc4f" },
                { "ta", "fe6bc7ce940e361a4e0a05d8e4baf755d6ab4ce68cd337b6fb92ca37dc8f5efc45bd5ebb0cb5095dfa5737047e94f7b72d6aebe0b9d096c4ad19ba73ca0b992f" },
                { "te", "430288d510e0239c488bbee40a6322ad2b4ebac3397df16120c5c4b78c6fdd0cc0280b854c830a93dc8e8654c34c7800d0fb4735bbcabac4ce1c4af9d6239db7" },
                { "tg", "f0f8c6bdec60f8c7367769d03e5539774e0b4923ad1ec6c31e625b1d5a983da3181cd1166fbe297fbc82f24078afdd5cfbef127696ca0ad517a4450d99c1da57" },
                { "th", "7bbf1c29f91da34f297b91084f3a8cfb588d48e804cf2723ef9dbe152a1bf6547bf2d2c440a7bd0d23837bd580907802cf4900d003755c0e2c9499c13384aeb2" },
                { "tl", "17b128ab03a4e4fae4649a5240bc84b586e60373a112f2eb4f7cbbb5f9f5762cc3748ea32d7b7ff0ce91390ba8c0be860ec52d33ff2fccc31d37a0661ba8e394" },
                { "tr", "33a46d7b072f734a56f08d84878af0d54935c8a5435ad0ab75963fa9c108c23ae417d9edd149f3b70817532b3b9a26aa82eaa9a9bfab25dfea8a341926bb5ae9" },
                { "trs", "1fa31686afad9b6562bc4dbde01f1cb110d9dda543894afecbb6a363ae310d477b6b8850c9b05db0d0a426e090042398e6f0c6403e69a00af12998d519f0911e" },
                { "uk", "5607872711e34e1431af2c0f15dffe6e0a7a3276ab76382eea0200255b028cd1b60b761807c46f519a545eaede1d3774e92e9514d3a18c490ec347e56ed29ec7" },
                { "ur", "14a937a4517686f97eef2ef6324f8a471b89ea713b058af082ba131be16bee9c1dccf51a4d45eb78605c8fce95ddbeca008d4008557c280f73d0709833698566" },
                { "uz", "7788e88d9dd03a3f820077aee6ca9927a697ff5feddf94ce2c90434937d9aa0fd0734a23afeb701e6fb6519bc36f5d7ee68048aa0aff27c756b51a705dd78e8c" },
                { "vi", "c17c074fbfcc4627a605fc8c753c154c6cb02b1524ae36011bb17a09a1724f6dd5281aaa660bea36851c3418d3b99e1720b2b6e5a70d219d81b04d4ca4f87f0d" },
                { "xh", "a641b937f154cd82a8c864e5c96eaaee862a3a54737e58ce8b2f4a9e901723367c7bbeb822e2a378fd1bd88ba8f523897b3dbbe7f2c74c2d29d04f2ceeef734c" },
                { "zh-CN", "26adc4b5863f46d9fa2363b2f471f709e6381f2c6a0c09b88fafe0c7e67028123a2efe6ba28246f5b96d47bb156bd398663481ce0d6c60beff48287beffbe02a" },
                { "zh-TW", "00ef7f1a67c6a7a46724e5122e0e5a1881afd75f4e0e2b77dde4d64617ca1e318cb1b4ecc3cc3d3b4757a7a72140a37daf5fd22c6fcea9f3bfd865a602be6318" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/124.0b7/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "bdfc519ee43d00a2311caccd932f18c7293807676982d66bf83897df47555c1fd8286c1e5cef71fef8ec2660dd609c61bdc5cee67c882fb8f25a05e1791a729f" },
                { "af", "5633579a87a747e61a87b4248a514403a2fca6b832a41b747ea8386f0d9eeb0afb8b53f3372877c32e0bbc6350c27aa6b7f389082988c639e7f010696dfa1d29" },
                { "an", "9300d229c3c6e667c0770e39f29a5f43d996f072e2525b57544bfb93baebc405da9ab7412ba530e9acccdd49296a63a7a410bdcd4dfe4cd1a38986b47f9bbb73" },
                { "ar", "77999993b91ab77fd1217c674175569660984338d1df6b2bdf1aa7c9c911d4461451bbd1df6a6f8a971e5127204f60f64e9edbcad34b1d362046d56ccceceef9" },
                { "ast", "e056fc47f6f7c950b0da68baf2469b103ecfa2563df8387dfefedeca60958cfc5a1846b4666a0e6ac9eddfb141108f473138a4c20bb2904569e2643f749383bf" },
                { "az", "fa272b963de2cf459fb3f61714189354a858799e711cdef81c6655e39ee2a4e084594176c21da85275761b89e6c7bca29438e7a01713cd548520c0cd8f1b5cd4" },
                { "be", "5ec9d8f020a8acb6b184622b8a326c29165920eeb4cf2802838ec4aa7b9422b8d9282a2123ab3f48ce85a61ddd20072c9069a98ee7d8538187d4df99f759911d" },
                { "bg", "fadcb7b001f4bbbe51f58606d944b7fb067f4b9686ec87c24dd5234e5116e5a028ec1e17ceea02ca1e41a44beb1bf0d22c4a70f1d460813c509f2058cc012ee3" },
                { "bn", "d2d4fbb37bbaf640388972b93ebdd8f761cd2fa20bf8742230f5278e9137cefa2c1537eb316e98b28bcaab0537d1e4873139b4d0e060630b56b90a7155e9582d" },
                { "br", "f0a028d8ca11337ddca7a7accb34eee07c5ed4d22cc554afe114607c8a970d597df820fcd3e8efbeffd6f2d6dff10482c5dbe5f0f02c52ae491f2fa87746eff0" },
                { "bs", "698d66a899f6965bfccad27a3bc7707a0ed7445de8ca28d4a60a0c93c256a565bd3c0533d7096a5aecef6b0787b302e76dce2b1e7b9ab67cc97bdb2e507a8be6" },
                { "ca", "49406ad0e1682a67f2826f3a95a4baa0b72087b25fdb8d2b64f4c7f365996b7f815d6a7fe41409eb12fcb472313f5e87c8d19569b3de8ca0550c0e0731438a6e" },
                { "cak", "91345aa9f7ddbaa0fc8703f0fd60579ebdf7d8bb1d6e00e69fa18da2e560c73000a3d86aa089bb09dd81d50c4ac456ad6f2f1dbbfe14580039f5304f3cc85785" },
                { "cs", "38a54b9ec6080603857715bc848e6211988cbff322bb99c8f59b5aa47ecae7290acdea1823df9fd7aa9237ba6c99a813ec50fedd9eaaee93283a6946e2baac53" },
                { "cy", "7a0d235aaced23a25dce8c5a2ed49d6cd4c45506bb4647f21fcf9911936e6f300564294d68e9385798f4f0c4231f53ad6e592aa2b2793a64f5c27fbbb95d9944" },
                { "da", "f2c3d37430db382718567b4026b1e2e70447f0e81104ebd8497267b2cbc2509a2a2a83cac05ab2ea7aaaa9d42d5196a4633dbcbaac09468cc8291eedfbe84c23" },
                { "de", "0f2583e250f5783ef4e9d055eabb7b8509714043bc6dc10e8366815f593e5b5eb872cc221ad29f30d4394b5a83ca37b84290fac22a96bfbe24a82975f2acf941" },
                { "dsb", "f973e155472668aeb3ce7bb9c04088eaead19105a659fec8d477b4f503ad57fdacc22653a38aa4fa8ede6ef95b7e4f834099ac116b53ffddd2889da94ffc6af8" },
                { "el", "4c76fe71285ac74fca81feee3b85b9951cb588a3fd98bf0734e078a51ae82c2a84988f045d0b876cf22c1bd0c4cff99d4405ca8d4b5daa88e3976e9766dc95ca" },
                { "en-CA", "ab91048536005fd113411f7d599861d2ae5fa2bf4b773b5107355057a6d715a5cecbee76dc27b789c984abac9572ced047ec87d776aff4b9beb0c9747416d09a" },
                { "en-GB", "cfc49e1885df81d03849afe7138e33155c9bb45df6c59ffa4d59429307470a010f3485085d7f2bcc7013cb044f5c66c30835ae7c6409672614cf457796efd95b" },
                { "en-US", "fcf4209174471dcf49f3433db0800b9f9c62bfeb861aab900f9dc41eed2f0cab79b9a067668c96fae19d44f84d44988938cab49b491cd72c24cafe100b3e6040" },
                { "eo", "c7c49954888ad60a614f4300763749f5925b593142aa7470bdfe433dd88ab304aea3d059791f419c5b993c33ac83026eeb983fb642528e757e791658c808cbb3" },
                { "es-AR", "3fe7067d0efc11d47530fdabdd309ae07c9d34c71f1a393bae2c840d8d4ffd779ef879afb742dae0ca56f62cdeb860d42b8fae99166d776561a6e2db566d55ea" },
                { "es-CL", "ff209d883d559d66f4bef30b8855ab591ec1fc52e79308e31fc2962839c28acc987b974fc8fa068d68695086a84f4e86277e9662ee7c51ebde2efa67c4691d16" },
                { "es-ES", "5d4d8f4441e46486077949d4cc61872f7a98981df72078c24252cd580c9d47b50d5d01a65668708314a4f7fd7d7afb9b95da14f97dc1fe5f78fc7cb8e439e72d" },
                { "es-MX", "cf9a4d10cefa7e249ed32bd6c9d2ca9c1cc62087890a863d4628e78a50e2fff863f28aba26602c07fde0baf6968b1473b4359675b9041ee170343722972cfe0a" },
                { "et", "b4aab67f246a42756749a34223eecc314986c9a0d2d3825e8e9cff57312b6aa20e071be007f16010444160ec77997589003558c720db72d5914f178bd9ae1e62" },
                { "eu", "2cb8f6893d4d663a075e9fde180ff164c25f6d09d1312dfef7eaeb839ded1757fb3fa304d90ad9bd6a662c6e5124e1ebf039bd580d3b95fb7d7cea5849d1a02e" },
                { "fa", "ca365c264945cb9517f68a9706c787562bbc2f913d80a18cbfe0cc34e41ce760eeed09ef554067fe153b58cfa1942a286d54d15318d25348fd8821b6d876ea33" },
                { "ff", "f5305cda6e3393f6d774ca91b311e9bc414433170c14e8974c9cc3f9efed9da23e0b45929a1d53b6ccbec9de24767d28d92846022148ea48e599f05fea6e6078" },
                { "fi", "dacee82f1fb8f6e340cb52bf69d7f6ced0621c99040c4ce27a680e445e1c26cfa653d26550459d10905517ae9dc424464c6df23c4c800023677189dcb644b6d0" },
                { "fr", "c305adeea23938db21fc1256694aee72b924f36cfb04eba5c8442fff9d63515f98141e5c3311771c80d3bffa559168c69bcabf50e5943ad1296eff68f9c12c46" },
                { "fur", "445942c0207eaa14eec21ba94c9970dc8fa684e6e5dd4a8fbb1d17dfd061d8b8c6eabde2d013b5f4892f8c0d57fdc433736af95a069d137053ef7d7f7c1c8216" },
                { "fy-NL", "a9108dd8d12d9ac16f418d916e6178a92fb3c081aa2eb080e64b72c0c2f62f777a5076006cd99407b3d21e59c2aa7760e3a21309f04b8101ee56332b4a3fc868" },
                { "ga-IE", "5f53c27d80eae5bd55fb5289b6356b5539e80f5aa6e4419fd31f503f4b276e0e15e4065e37f737bed00f8bc1202b8d81e847449a2dfffc9f7fc70add3e788d65" },
                { "gd", "e3dd70f9a17b5a70aadeb0728b0feb41f1fd0d636f9b0be906b76b5c2c4f40a72e4cb0413fa0d176e8222ef01ec68c63358df13756c73f76329494e797872fc5" },
                { "gl", "38396ba3bdaf910e1fbb3b04550878ddaee632475bd8440f7086d73cf6531e55aaabcd92aaca179d5aea81d8fde37031e87652aa87455e1a781f384c27528b2b" },
                { "gn", "b2cf0338f30f916fdf6c18bbb92fe2a2ae77fdb18e699fdf36e68cd825ce9bd547f9c739c1eb8e14ddcc8ba8612dca7d9031bec7b74fcd13863aed2e59e18994" },
                { "gu-IN", "d4db70fa1a17bd0813c60f454cd08c4c50a58bf5e2287335f8bbf3a492cc8291e50b47bf4d1b698f35d5255d32f60e9781587ee5af140d742a12e1f9e5ab6ccb" },
                { "he", "72d006f5a92b0dfab710b5fa5fe48b3eacf549d67c664ab723ae5b3c9e57f357b20175748900fd0dea491a681fc34df72bf610539a0095b3700289d8f36c619b" },
                { "hi-IN", "4029212261fcd5600fc3d0ee93e82dc30adb2506e1a973f75f1b62004707a20a02ecef6978ad4142bf19d5edca7c876fcd078097b281c9b41b86860dad4ecaa2" },
                { "hr", "5a72de5573de1019c75274bab2e3f41b7f5aabad512b01377310f88092cc78af087df736433b86aaec9ddf049d5c10d2d90d42fb3742c31f7000c1b4b5ed4614" },
                { "hsb", "eb8e5416632218c99c4562d2b1442f5eafe01bc3209c6bf4462f5242bc1bf520b6ebb741b5dba59c0166e5dd109c278c899724fef0f82cd80c29a9479aa57196" },
                { "hu", "3f0665715ad88225f8562561f869810dfd1bb7f3660e1b791cdbe691c69ff0a6f6c1eee7998d1ff21506d8ad3bcd873aef32d212ec0616ce815efec3e59a6d43" },
                { "hy-AM", "c58dfeb259388d83b4377f883a9088ad844448780831e3bc22663492c6a491ed6c3d6d8fafd723aae108b7250bb44d57a84203f70eaddd299bf06cfa7ad5aab0" },
                { "ia", "ef1747259eb914fbc300417099160545cb604786f985c422be51ff47ea532d8f9092849c5768081c61bbce6fcb01b98967665b6d69dee3cee22b21611d76785e" },
                { "id", "b670ef2c780b6e3b195a60f1bf9a22913cc041f2851557abfe1bc7f2a4b7108c1f4a9444f44f53d5aa5c2f533344ccade4f3008fe1222fb7eccc856ed56bf5c4" },
                { "is", "c44f435a6b9320736e074e69e26192e5774120ac3d5427702c33cb7ce3a5a8adffe42e5ebe851366f689de41b1fae7ba2b292f4dbaaf708b633f092448b017fd" },
                { "it", "c6f5ae41718256f27865308f6bbb7a2d59ad71f7376898258eacec144ddcff60b02724f429ece54a47fe8939683e51712984a9867e4b77e083be707eae4067a9" },
                { "ja", "f78c55c58fdf301674f08d04287fc2616f7b75736b1016043a576cb6d8bd6976a435947df796ab0d11e49133ecb0b57d9a2684aab93f15536629728cd9588b39" },
                { "ka", "539499e89b922ba97a434775d595678ca18767a93150af236503c1503660c2f228b3a5ca1c6ada73abeda6345b6f8bf1f558e2ce414b114e7ec740b578630958" },
                { "kab", "d377a6b469e847cca699422ab597d3d6688a9fc262b43b69af0fdb898628a99184b2624fc35a97ab8259e932c71562dd32d115164ee92018ce07fbb23b316a92" },
                { "kk", "26243b04a5cd5f86e2cf74bb9b8b86e054902c00ba363c075c5ea8a6b63b9a57fdac2e36e591a2f6d4499208dd9e655206639d53d6d4b669aef41e353385362f" },
                { "km", "5862f9fb06dd37543def3fb89320f3f0dab6e7b8007cbffad38938a5aada2a87096c6440ef643a6466de9bd58ee431beb3744b37f5523b0904a400c9f28c1447" },
                { "kn", "c8c922cb4cda27dd4402f807818d92ab7cc73dbdf2e5cf8d1120512807652cf0fa16608671b05cd5785ef88bb0334ad04f94f4e620260ea0994b987b064d14c3" },
                { "ko", "f273ee34bd4b6e4fa77c12a0f2c40030dcee649c72f81de1f5f0061240b4bc34e5c48c47d0865e53059f407b0f645ce1968754f1d7e17c5ff1a795b9ddf6c6c9" },
                { "lij", "5aef6b52d93d7da1a7fe6dd77fdd1c0afc30a5e40adea3634278f916ff1ff8b688283aec93bc005fb6fe66581ae4dd8c6513b1eb2dd8198a1cfe7485825d117c" },
                { "lt", "fc39da206feb88da5277246769d03d3092dabe25834ac8f46523956bfae2d1b707ebed4ebdccee20f306e06aa0b0ff9ffee78aaba450134fd5d43d722fa99889" },
                { "lv", "faa2646bed973e87a96572cafc44de5c39053d80421aba7723a05eaba8c0f1afe2f78a5f4ca8818610be6dea8d6dfbd30b286a2cbd04421a4d69cea6c3413831" },
                { "mk", "9152c6fd19d74094b8fbfc6cab9f1d3727e89c24ed216d1a9fbfcefcbbd88702cadcf03faaa75fb0c6f9012fd91196ae57cd72f128e122e90dded165e5809be3" },
                { "mr", "4e9940be4738ff6d740b5f527f0418de02fdb7a8f49d4d048be2c00dae3fb2a697a14d222ca2554535535c85f08b95c50237158e61d57c0bfc83a07302ffe65f" },
                { "ms", "f824392027902a98b837ab3f4b56c0036990e683238ce1c5d90fb0038a86c243428f0dd71c7c1d51d3791eb42e697f22a94325a9c5970f1df1a5d4cdb1931694" },
                { "my", "50748b6303fff67a6fd973e794ba9b2e4ad8f013ab0e75b7ccb364a97c06569fad35b9b491e4fc420197a51f7a255183ff8aac2efb5be21c744a8154d48263ea" },
                { "nb-NO", "e626413dce3abb9af05b60db7ecbf9ac14df34aa57434bdb7a98df375f694d0b89d2c0142975a9177dc3663ef8ad4b304123a5989a8df3c0819371e7c0bf4d51" },
                { "ne-NP", "bda1597cb64f6f4df5508ce96d5b29f5c395400a36127248d58c7701aa1febeeb84a5ad8acdbacb7a70fef2b2cb1ace58bce12fe379d10859c9439acb6d2b05f" },
                { "nl", "273fcde01194983f515ddb65f31b3e622a5350c472ec162d146cb700bf007ed26d327c0a1fdb52eed8ea61c4c922d9034916700cc9a28470f8c4b09ba936aaee" },
                { "nn-NO", "cb8da958923a9f51dcfe14cfe906dc9797f3d91d58582c7675dbe0764b4d5a68ffb47fc813c84a0cb238892bb5fafb926914e748a7e48350994a5e40670aebfe" },
                { "oc", "174a76e775911574e1db0d238a9d90bce264c1be6b42fa212b93285265fb7ea25c6c656da0e346083f7c58d6d6e77f3c16bb23e6d4a149d260a299ad53e4d22c" },
                { "pa-IN", "b667a8a0df4c76897dad4be62b718215666b2976ef8afc058d1edc45273b6578f4162cdb2244ac58b18acc548a4ea48165cfa013e3c7086595e54218622cd5ed" },
                { "pl", "cd324d7e7d278ac56b5a534ad5fe502bf7539229d9cd363b2c483ccb4f3ffc0e55c7bb6d7ff10929d4774933c37c33e6ff3eb3f169f8b3f7a9ba11a563d376b8" },
                { "pt-BR", "b490defef3af1fbf6965e0b82385e89ac8b13b893e6a468b6460a41132c63c59db12c51c7a5c658d754ea3f1577db305df40f58b35e8eb07f8f0e74a230a54e5" },
                { "pt-PT", "d6e1435c832c8c3d2e6d8ea1689e356807515fbf32c668825f390932680b0fd916d74aefff24e106f11301b03f1253a202ca2214f32f60cf73e810efe8147339" },
                { "rm", "2506ae4ae9f343465305f7190c1fdd5a1be28b7c24b99cab3b1e5b9297e3879bcaa07f1eb75a9d3e6ba807f75fc7e84504c5cb86238401bb4c94c89d3423255f" },
                { "ro", "2dd467cc912fa6767cb28a19287f19e39e0df80f802317fee4589caef93de8e86a13c9ca76b1ca00f3991db7b4cbd10c82364aecf122d8fa41ec99d9d9885b34" },
                { "ru", "26728ca20573b73a4fe5f7fd78adf255989566304927d15ae98d97a18f1084d21ea5068ae97f539b378777508960b334ab732e552fcd450cbd162ed0c785e78d" },
                { "sat", "1a7c3c8dadba1db6e4855854d59d68960e53c5f3413eee3d8decfaa362e5a6ba575b73bd8e5dbfb08a6fee9f6c79ef6e43d55b05e7cd71b85cb06204c0cb6ccc" },
                { "sc", "47c322c4fca6d1eb0dd8eb88319e87ce8bad66d6c90210b6268f3fe997f196ab4d322fbc113f0e099e2bfc0640583d52d6099bb7ca1053c8bd57943378e6103f" },
                { "sco", "967bbad53a5b8ca658b5b6a9135ea5acc3e3cc9ee9676e7990fe99f86ab12953e497b21e4271f387f8b1e617345bb7d2db8f97d5011a2180f53dffe57504a7a7" },
                { "si", "bfaa16a927ab539e7c9a34024f1b154bec67b335c25377f55a63b12988e05f4c95b036cf3bbebac24b16284d199d2b0c6a7e17b8f65b55c52a118ea7aba909a7" },
                { "sk", "e229d2adf41669c21cf2461f8e40920563f0c7ed723539552fe5074c4902dce8313a811557125c601c74b938f684c7b785a39cfc73257af1325d375aae6b1ed8" },
                { "sl", "65a591918b124eb617b0f080758265e37fec84ea1da98f2c0250e34d07b2c6550e282ab83a6c493c9fe1d61a3de42560e4db343f79863db4a080df4e023667bb" },
                { "son", "a910c878cdc4bc392dcdc721ae90b4e431d1f6c06a312c0804f0643952c3ee5b1c6e006475a10d339cf73e1e074b1ce9615fccd5913e1b61ab281ef768b54149" },
                { "sq", "c8db04256dce9119ac7adea2963f792952c0df37e7cb5bd9256913a79b352d5909f50829ba7151b9606d0f2342ef9138bfb2d58edc5eeaa9e66a87522b15c8f2" },
                { "sr", "310458947634d2d507c587d81413ef57309f10615393bd9f39c5ea9cb5ed446ba3a941e61430bcad2b17420101e0942e36b4c3661bdca3d74261f0bf09c3fd4b" },
                { "sv-SE", "b87892a2d5611e4c9a470ebd610c3aa50c6e6ad64dfc262a43ea99bfbd8a1418663a8e0a6c8013b399f7819e54a584087b5dddf0dba929de3cf6ddb6ca467472" },
                { "szl", "7f8941653cd45b8a900d3556d6bfa5e3bdc9bc88a80005010e5c19eb5f64e806c86697dbfd2828b322b5d9984426bf30a72a8f556b009dee72c19897807d6553" },
                { "ta", "9b29ac36eab6b57192f920eedda3a537f9012cc0bb7ec01a89bfed4fc61870142462569953da4baa8d4b92c1e6794fefc684a9d97ed5dea02c6db4f033b953ee" },
                { "te", "62b3b20f7abd9a8f7b4ad11886934d954d11e6a51970405ee552f9e19f9009a452619f1b801703235f75828c2714d2b45914c829275cdaf76864c2dc59987617" },
                { "tg", "4bbbab025da82b5e4895cbd929bfcce88d745f586ba52349aa7721e506b9f3eec7b1dd76318ef4db9975db4adb202ae8e691338a646d35f8d730349db8ee46c0" },
                { "th", "90a1a7e8928d5494f4ad1d76b7904ff204208d874f52db2c8bf50efd619a1034b9e356757f012e3efcf2e0498b8c4519cd62d35a885b425c95f70b9b48d64489" },
                { "tl", "ddd647b891933940ad3cfef75ba7a88bd7c5bc83205ac6e0502cc8a2a253027cf4f489235f91b43a63ee68a98bfa11b29ddea40e652ecb70c1af5f770da14db0" },
                { "tr", "b73a1bd5d356106eb3bd881ceaf883b1c77d82917c0bd9ccaa62c1c7e7832b8127135a1616edb1abe3456ba09338ad2ce012a3c341f8a1824e2153b1eea48989" },
                { "trs", "89a2a3e0df4dba0dacffe6851398ff3e162b2aa09605efff2f9ccd73a3b2b29bf5b407ee2f7935f4d8e104bca1ca99d70e897e9abdc65d8ee97c8cf722c8699d" },
                { "uk", "073f17530e5cc14502054a7e05a55c7dd7dd7d799917fbb1a8e8c815ca579bcf2b7c37e82f621238ba85c0ebc70f007c0cb76918d7a1eaf40d2a529638304d85" },
                { "ur", "9fd1007209978f35b3cb7fa02d31602db123bc5dcd7d046b3dfbf409faeed63ee2d4605d5ca4b265fe54ce12356151cab0703d8cb10a0c18121dfbe0d44b35cb" },
                { "uz", "55b47a72b8b9a25c78da7f1e0d11126deb2d258a6d0aceea2e591db9d19861dc2cf172ac615d0c5f82b8e8d645c6514009f54244734223a03279a89508609940" },
                { "vi", "f9715f97b5b4a037fdb008796fea75584b5760ba8803ac6a21d07e475b950fbaccb82d344938da23bc9f82a29a61b87cb98c9b11185550b5e2386956707c7cd1" },
                { "xh", "24e19b54a279f16356d5ef854be29f5b636319b55a3418319cc66234a0ad9bdec86f6bba4ce681833c6ae18e1276a38b1760a2d548820892924544a7c00ac8af" },
                { "zh-CN", "783404b15b59dd30ffeefa0ffba1d36230c46eccd342040431e1c9f853eb0b3807c0786597a0aaa5a2ce0579f9fb1c1380b71a5f10f236486108ee0ecb84b74e" },
                { "zh-TW", "a2af90fa6014f2b108999375c426830ffadd37a195723684addd1d70d4b52b289b954733878f2a11b2d71070abdbdcbe18f114f5f14a549f4f304b8078901c7e" }
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
