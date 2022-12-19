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
        private const string currentVersion = "109.0b4";

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
            // https://ftp.mozilla.org/pub/devedition/releases/109.0b4/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "ad1a3ad73fcf89792ee6bfa484cd1aaa9339251635300286c23165dc0cd6938c6ba0fd87f854b2eff917ecd72e16f154f6d119bbd1e98ff50073d693d33a7af2" },
                { "af", "0563e0003827ccd4203ab011302bd0bec2ca4726209992ee020d4a90a181985ef9c297548324cba79ce5d9ee2b220fbc726257501546ce98839d164f7eb66a23" },
                { "an", "1abf7d2e2090fae490844b304acfc95a23cf1ee39d67f37cd801e7aa33fc8daadb793943ef1d184835645e72080c03ec01df7305db64fdd92dfe38573b1399a7" },
                { "ar", "2a15b823176901c527546cd761925494e9dea94a0c0fdac57cc428b1ce25b17318891e9ecec0d131bedf621daf0a024d1e5d4617459a64de55fbd01c272d0f53" },
                { "ast", "8eb05f73146dcd2b6a39b0c308aa7e09054dcbf18326f235b60ac70ad65bf464880f14894fc82a04cd72143f64b571d91077c09bbecb663074f86bf74e410245" },
                { "az", "f95ddb6d23b9f9c83e9753476f2699f825cfd9257b6d64355df41de19234c080dd60f53450759aa66c3d73ec11e133ab12c35d13223f1f85e55fcdb69eff25ff" },
                { "be", "a977d3024f0e56b9b823deb9bafe4d0a765fb8f61a8342af1eb788eaef89fb51b79e2156ec8177a4c24ca5ecc268f5dd7caec215c778aaca7d0893ac8d460a1b" },
                { "bg", "20a033147bf59cfa20398fc89ff914cc06fc7c91bcc8a6c57c9d221ed1106dd6e46381b8d86f905a15777f261db56c51207a13f995f119c27248a58bd97ab99c" },
                { "bn", "0554006f0dfb0a98dae8362dac91a4c02fe92e888d2e9645536dce7eeb134fb4df3fa5cabc35e9bcc341203f6e7b31e247ec256699695e04ab6030867c73d080" },
                { "br", "c07d18b3ae1573c0b6d5dff057908497ea420732467bf374a02b811874547d82cbe94c1066c777ec5702fd8d6d44a76cdbc2808be8a2c898cfa5f59375671f84" },
                { "bs", "7aa3efb50c03148a3e45024665f3063fe88ea55c230b4fc8f306a17023ada69d2a19ba94741af16e973a79a628402dbdf17fc398e32973143df724d140735181" },
                { "ca", "a1e3f2f55200958c61900ed30f880612d3d6a691e8cd10685b1912c88ae1b5e0233b9941286754be4b6746d1aea150658e228a95489c1f541e42e2e5f65b62df" },
                { "cak", "fe765feb018da99e567450f199ef971353cd6e363de34a5fa7d6bb7fe32ffd7acecf4b633bf2bc8450e8356df15f49284ad417fe895d0a454b6c8c7be4790ff5" },
                { "cs", "fb125f02cb67a3242845da77cb0ecc1dc333814011b59117eaef657c87d0811ea92a5e080d39a57a9921e9336bb0bf7df5276c1b198df2dd3e5223cc453534ab" },
                { "cy", "762ee994ae231008cdd2c27c8ec3bbdc2871b49e7248f5061d85acbafed4199ec1f79cc8f8d6b32cf1cb70cbc3c26af2a2bebae479d11bdc1bb5c5fa4fde279c" },
                { "da", "adc9850f9ddda32b047032d834f17ed1290c8dbbea9603950675d88f491f9535b5f5bd763236282a5ea9049987137d8c8fa84b43226ed72cacd76caa48da42eb" },
                { "de", "9fec8cebb211b77ed45974d0e7617729daa56751950b051dcdf4d1331f5bce089576cecec85e2754c2e4ee641f6a371193d7e6403983a8199b70115f7bf06d3f" },
                { "dsb", "8aab010f78d7443c30f0ab8be87c0f7b935612f4c4709fdb7ad61912481d2fa54e8dac8db2e6f0ca84b0f52e3c0c8d3a55b9d4a9d73189bc4e7ea734465f2928" },
                { "el", "b926f9752b20964d343a88d3d0e7a9db35bd2354ddfa926c0026c395960f43881e46f986ec2f807c911363c65764ab6ce3ae8180ca54ef3fae6cf88103e5cd1c" },
                { "en-CA", "33b54034c2e8484cfc475ce3a49fcbe03c0349b6c5452c1dd2ef429f63c47dd6de3d9232c81af6451d8a0fb9b030f0eedba98d9a7305b4a4029b7d9fbf8b7ec2" },
                { "en-GB", "73bfbb31d7a13ae95394bb1ff1d767020511c1edf07615e88273d9f7eb368e37fc46c9049da489a7e32ab34963f712662370ed614e5692405ab72ff771e6cbfd" },
                { "en-US", "877bf55092308835010fce304ea03be3173ea0a31e2b88fa30188d112897026c95223b85e958c93e8c02bdb529195c8d36e0e3a079034443fb465b63ec117e93" },
                { "eo", "1732ac9791fb0e8e910828e173e582bd1545436ab6cf14619dd96268baf8625346200814121dc6a9ec62d24176489ec00c65c1acce52d1d8b7a88d3b6f95d971" },
                { "es-AR", "ad0c15a6e49b1b1dada8d0ec49eeacae2f720294c1dbc281a2507d28c4c0c61130b2e25b4c50a0aadcbe22c9d9251d07bd89718e5b51c4dde93fe18635582112" },
                { "es-CL", "c98de47b66b9a15e18f2b1e349df0145c72713180e2dd48fa477442eb7c462e638d8e090164d602a3b1a33d2e8497df8e44809edc1e26dda9ca50721bd6cfaf5" },
                { "es-ES", "663575a036be6f49558f24556412f5c9c0a2ac7654fd1c5eabe1a9e6391d8316ae06a29b680dacf905036a218808b42488d3c1167eb83ef74f7cd0485b3947ed" },
                { "es-MX", "8846b38017070cfe4b1fac5e7173a5c1c5a0d982d305dff43b63bc36be97e9aaba917790ebdb535865b27862847135c615fabc7251302d93128cdc3c924b6b9e" },
                { "et", "db11c2f937711e195c14a0cf800aecfdbfb07670b14dac5e530d01f13197ba1d1157ecd74516de37866a19ad2e6823a5fc4e62da67f482cef6137d043dfde3b4" },
                { "eu", "61a37687dfe95ef996e26e288d9fc7b04d7e936e79c775f4d883300ebf35d8005d91ca58b308f07dee10d3fef81028347705bc92de528e0e66996eac049d8201" },
                { "fa", "be170564d3ecc988f7ff85153bf1f307f3c27b1351dc30cae488f44bf4f4336637595d9b02943400346c77b16b59b6fcd851595748edf2cc4a54ccab22763f19" },
                { "ff", "49670a17441ee56ea957163784b4433ce3f47aec9c71c6adb207e7900b61d3650719b967e0af77b064bd140bd2194c390e4f6f9904a2caf121975cf4b4267fab" },
                { "fi", "7d6bfc10509360b04293f39f0b9069f5bde2880e38ed880476324471d060d06930b6c5e44a1c34a03084895ec0726405ef3f4573c1866e0e7339c8598f8d8af3" },
                { "fr", "17c2f7d98a57577c3ab780735b35923b74631d5bfa5b6cc167a3b0425f0024afd37c3fd393bc379eafba1dbf31f34297b529f21d906e64a5dd608cc57abde2a0" },
                { "fy-NL", "cc8239211cfe0073156b6359f9fd342339a8e41fa9b586e3a76b2d623cc5d2634aa4e195a59811b44381e29a8a26e2fb9560f1b715643af663325d658dcd1ab9" },
                { "ga-IE", "657c9340ace732743cb46631edb8eb641acdcdc9d4367558d0ecefbb54bbb049cd3aa785f58dc2bf04fec225ecd2739e13e24ee4a1683c99b46136464657e0f3" },
                { "gd", "e53d0b1761a1a68987b1715d77020f8d6791f8f800f190b4ac232d83492a7132f6b5e89fb1d068c56a383c6bbd9f0d625084c014c1b604d868951fd68b8a73b7" },
                { "gl", "8eddf5bfd4ad70354d43536c5a3d8c2d7ff2da524d0b7e15a683bc0fb000c10060f945eaff5e8c3a61086a612bb0c9226954218cb0aa7fa2cb332ffa45e0f912" },
                { "gn", "041317d420439b4e6eb0ee1af405e5f2a3ebdc557f735226b1642601411cd67dd381e3c259f0dc2c0d5d74b461889a63fff06aa83376ec12119775dc9aabb652" },
                { "gu-IN", "0242c19cea7860fe14296631c11a23172f4577997c7ecd4dd824576162a619eabddce6cd8b2e82495ac6eb9dee16a1432ef457dd2301da3fbceeb82b01d38483" },
                { "he", "902fa2524160c31be5b2b95303929175dd9b70f7143728de68b95f03334c00f1e9525506c8d6f5c0e0567a1660bba863243f6f75dda52dcbf794b315797b37b3" },
                { "hi-IN", "14c74e3a8aca53ee425edc10ef1842612a4622b22c8fb4701b8a62346bbe58032e3777cd90b7ef740918f5f49ab9635829c11335120f0377e7c15ac73af8d25a" },
                { "hr", "a36ff5008d2fbd53f815e13419da2dc085f30516739bd2a1b2f3bb29dc24d3d01d7d34e6ffd74ef611c1c51bf325e26e265ca4f4bfa591c7449d5826acee0a82" },
                { "hsb", "536dea40e2fc58c8e6e146f43e9c3c03e84d6e5e808aa9eedb5ce8668abd909327a22e4ae358d7dd322e182d55533a0ee9b7c635e67ac3eef085b7f722a1d2d0" },
                { "hu", "6149c69833a46afa8b4344c2cf62ac8f486fd0e73fdeeac82e912325651ae63073bb735f56004bca99c9c1f55a1432d2dbc1ab9575a157e648e79fc745e87215" },
                { "hy-AM", "bc511edebe7309d03bf0b8cf0a6a3f578954d13a1c1e9598c254a36cada55967cd76fa5f75af6ab59ee9a864775e72a2bfb90218cbde546674c1309823776027" },
                { "ia", "4a3ee28d12a2a93b3c681c4835be796693107196807ebf99ce9924678eafd617d457e5a40dbbd92555968a7ede852820738040494baf604e0a9fe53ba146d541" },
                { "id", "6fc77d1e70c4e7b1c7d46b11d14c33e6b44134a5767195c1725adf534d61a0a1ae90c9666542f1548ed70f99dac376d8754fc1bbed0be63c669ea577cef94133" },
                { "is", "b9cdc9fdc01eac572cad6f612c32882d7e1ab624e3b63bfaf606ed9dc100ebd6af3289b48058d887bfe675fcd2ad700a5e062ea085e92e822d02b8d2386cda54" },
                { "it", "4829fdf54e8cb92cddc95d095fa8ab0f7ce1025cef44331b521e009d7ca6c50557c0b22faa4cd590684acf15c3d53db66a7c1fd91add18db1b6930cf6576ac0d" },
                { "ja", "362a580f08fb077db8dec4ba586f73664ebd6b0660187f1ea3645b7b429beb47c34f97d3294e50b2609b485a446994a0cb401d1efb2078864f228a51019df559" },
                { "ka", "b9254d0028d2dbd2a78ff4ac4ab2fa9d0d2e5afcb41ce134f6180a03b72aa8e3e17cf67c44feb587bf278d19501f38100af6c783e69c9e0dd182e698f5849490" },
                { "kab", "50d810a6ab28459e103c85b4ed15e2836de3f23993db22479c0bc1e74cdde7fdc0ba61063957164e5dcf1138e7398c4c96154ec8e535ee1d5f042f0c18e7f806" },
                { "kk", "16a6bae4a7fe5916fee868e7ebab9727ea8fb813feda2b18ab42c32ab5c8357cd7541a65016379892d01ab48b40e7d57a1104ecd0dab279e563d56901a3b6410" },
                { "km", "b6cb89494b69c3abd62beb51f7020d35cc39abdb56db1f8196ab80f3eb6e8e50cb49b9c34f3bae8547879afe301d32cecac08f1198cfadc1896b23bbcd530a55" },
                { "kn", "8d7f240a58be290090a045374bfa2c54b326e8bf03d5e406ea3167e509d5271ed5be6ef67eefa9f26402f054260e4904a0eaff08576efb49d2b693b51b6ad0cd" },
                { "ko", "f0729483af142f1be3109f291149affa6697611626eb96bce1573866e0cfa65145d499e0bcbefc7bcf6d5b3dada9536b583de89f6e5858e1356570a985fd2628" },
                { "lij", "49c0ad64da1ff6a0e3cf933ef9c4aa3b65cc79c4a3dc5c0d7732e527f967fe4e54a41584772ac7623cb09c59d263ec8db4b04e9709e0ff1cb5bade7ed8a9f9cf" },
                { "lt", "fcce2305c63b854393364284a5ebc8ca839a75a86a1da3de190be2e0f8ec9145dbcb01df926423462d3908df4855d1f0526a7c1215d14387f4e09426fba79609" },
                { "lv", "6daad4dfbabf5ba175a89c168281e0985e373441edaffb0972f7ca3eaa5e471caf054a4237d90acda6619b4ffc5dd8efac0cc2223554bd4cf4fd880b66d0bf5e" },
                { "mk", "b834965abf0332f84b34a942c9d02dec9ebfdb6ccbdb88efb3c3531aa217c766809ffd6f796753b34a9d7d32cb9b7258ad54a22bc65ba43eee441d2f8211c216" },
                { "mr", "42fd2f12edac707f33dbf9cbb70eb151ae5573953518205207918a1a7d98e4f14f2079bfd1841a58c5ee0b6a3ef9030e67c12bcf4e3d70f794857d2094fbd145" },
                { "ms", "afd2a89ad85789ebf197871ee79f7f9e37b5a90ef13e73d8a45187f4ec92a4d5d5590d122224869ca6699f62464abea0f05e38323629eba4b0785f9c201c9fe3" },
                { "my", "f6d837ca67a6484c49087119bf0cd3551e7cac835c3f2dc2f6125f74259df7222d518774a6a8e1119b0192904a66a474cccff9bf28d851dd59a50f1d8cfae069" },
                { "nb-NO", "46c4e962ba9aefafcc0b98dc3f5c25429ccc777a96e8d5615d2c34963ace43be72651be5d24ddd84690b55f43768ff1a00b498b78304ddbe5367983f2f543c58" },
                { "ne-NP", "6d9670021d891964c702fb93c48e1c5367b25019ecd216a674c3cef1926f0d91137e3a323d927d6192d1848f62533912dd1293a9bc829fad3c57fcd376b6eb0a" },
                { "nl", "3ba3bc0268514c75dc77afbfbcae97ce1d54045942cdeed057a8c334e36ce5d38668da7aaa579cbae7abebc5c68b597b3bd8f89291d94a003ad2139e3d0b3ac2" },
                { "nn-NO", "974ca6f8ae0ebe4cf0b3ff8cb513014f4cd6f7805c6afd4028807370a7f5c3a7fc38ffb30e856e42e8a049e2b53847e1d288375beb8628c0612da254f8e1bb8d" },
                { "oc", "664a65f03338510c12be7226c37726d26fa353316c23fcaa9c1f9d4204be8033c709eedd976d40dfea184efa4034188c1a83c6310d5f53090dac6ef784b88fd9" },
                { "pa-IN", "51269839225406c8cd5683a05e59787f1f9ce20ebff8313587e8de0f8dee36cfac4609549bcfa741b95836c8d4714531d155a6904e3012a3ce878d5d426cab43" },
                { "pl", "39a83f56985798cf0087890486e70fbb3e2be317ffca32bf1d8cf8433a606ec3f16bceacb3b5258d587299f584f88f2bd03f3328488727a0ef209839b44a93c9" },
                { "pt-BR", "4f011d315a832bac98ef15702d080028a585f1846bbb22a98ba8766b3192c76e54f880c9277f3bde52255bce84dc4f105a2c84abf9d6ebdba0bd2d300c8e838a" },
                { "pt-PT", "806f13c1be4be7892bf330e2c8e090a4b653c7e5a9b95b913105af2847db9ac760433fbed6591f8ff6843f9927b67671bc0b68a77dadedc1268fcd07fe8e0b3b" },
                { "rm", "14f95a1427d08772e9234499bb49c1d2424caf1f64b473980d24cdf65800a8fc239b1ff929adc62a9fad0bfbf6843a2b2183d6fb8018b55097580c8355cc646f" },
                { "ro", "fed6c6823ec9b4f602b8b3fa940749611af1ccbb5d5628edcf599bb33688e2606f14326266eb9ea416fa479deaca0fa94e8963d1daf6ab58b3443261445775ce" },
                { "ru", "61a33b1b460ae98a8661b61e9b6af527d6c6c9b05af8bb7486d859a4346ae232b3287ee8dd03a811707193b56225198a4146246d758849959d71b05b4afe95db" },
                { "sco", "cd34db9f62137fc58f70dbf3b28bd119c4f7f713d1db885cc5d2ebfec872112a4cae19c4d8dc3913927631717982b8678f803a2680b1021cb4b63ac30ae990b0" },
                { "si", "59330006177393e6578cb83b3d586bec249c9f3fcb0888fefa6aa709911dfed79c4eaa7c60b4c2e6bc98bc9fc4eb77b961ef13fdc2ce737f65568f48bcc67017" },
                { "sk", "699bf9054d0af9ed23e94ebf82ec2ca7bb64ff881c8dfcbe8a9ec5394b9e800121a76956704d8983c2fc3a8da3826919056daabef194942a6376c31f704dcd0e" },
                { "sl", "0009f53481fabc3508df267ce32045e2a318115d5e20ea90a76cb16a179cbd7ab07202458db43bb67b31ec22fa043524429b9d95232b9fc8874add0e3c5899ae" },
                { "son", "565c39374cb396674f931005d07c355eab38ca3e763d07fe2bc0bb7637a8d8fb8a63d8117ea6fc01e9cfe6734e4fd7194b02f785198923ddfb1afc755d93612f" },
                { "sq", "0a668fce564396dbf647fc058ba6c89a5988c92ed1f6161957ff88519fec04e9cfa682030102e0de768e36f0372bd91ace85909133ba5d45a3f1004ead634b83" },
                { "sr", "a7225b81fde960f2a37efba9f311ca8e57548f892564f1fa557c2b23045fca1952a66eecbf87e0ad6b1f930efac78c33a090aab688bf15a8221a60c2f9e6ef29" },
                { "sv-SE", "b6ff358acc86ea5138d97f62ba9a6ea16976de9d2d7c95e46bac3714776cf918051a4de54b4588a2777ba679669a74caf76767cbebf379f1828e3a246de41513" },
                { "szl", "ab2f32a00cebf51f136217ea033abf4bdf691d989dfe7ac60b910e55c1adb42fa92861122930ba850eeec645325cc991c62e1b5290f0c3a132c588e07ed62146" },
                { "ta", "80304a193796c93b7edb0c7055986ea0c43cdf9deda3b9ccec98b57185351ddf16c205e5decfcde7ef37a6a3ed2e9de41870d04ccab11310ce15c176ba4980e4" },
                { "te", "5add55b2e74f47fe645af8d4d4a8176a1272c4a5d90064cbeab397794b58166dab973beade861f4d970ac69d19bfcb038eb358b58d92636b5228edba5ab792e6" },
                { "th", "c0f3f05abbcd55418daa515f9844a3698b50a765030bc6f2ead81cb6c4c8973ca9867c6724893cb8c77ad739a3d8d22f62eac951746a51bd42b11c695a01072b" },
                { "tl", "22cfc0be901be26d710fb91f2843b05cb2b6246cb411018bbc96ce6fffa74622ccb5ac27f3ff9d488de2bbfaad0974d42dd5c966ea09048fc1a67b8453bc673d" },
                { "tr", "b86f315169967737944e44e05d5dc4db26809bc737f616be47af583dbffea455e6b4781a092d26ee0220538b2d4fef1372563675c2f44ff9abcf8eddcafb0db2" },
                { "trs", "3d2e7630d503f39da6a7ef93b86481ff48c6a7cea76298923a02998d8e83c1ef6f895887fcf92b19b0d33f5feff1718a81b13d402f20b14943590b702541ebd4" },
                { "uk", "2acefc508fa80e38067372ef5c33d7681474aa294ffe5c225d44bce65c4e6cb23d5d3d1cc1f69b241baa719fe08bc9e1d44e618faab0f1ca5b3d842eea4d6e7e" },
                { "ur", "5faabc450840daec0bccd8d49897497076f6db7ff4a0e995fb40c48fb1b6ce170f03cf051bd7ebe66883d53c7f1d3d5796c3b93a9d43ed7e2f63f3f7d7fef813" },
                { "uz", "cbf3fded2b9922202798e28bcc83835acb5e3502b3ba46efe14db75c426ff32c1209f1f9ab5877861ed9f883c564d0ab13ce94a82e8c2ad8e31f7629a5f602ca" },
                { "vi", "fe90b192d09dffc7863efa02449b21df4f64ddc269bc7e1730e29bbe08c5d7057052006486062bd9bd6cf16fecbd18717fff8f5d022d9e957101482bd9f0ddca" },
                { "xh", "88384ef26082bc438f62c5b1b3c7e2d9b27b1645e13b268b7621da1cb9bcea0d8589f6c65f7143cfb6151ea1ca6ad6afcb3044844b15eee218026af3df22f9c1" },
                { "zh-CN", "94838f6e245c80e38c8b9c729c455dea5796fd744bbe99c3faef5838c2d75249dbace3cefc0ba4e4cb24a2ec0d521c2cc99e4a5b0983fd6c9a0e027af8d74425" },
                { "zh-TW", "f26de726ab3c9d376c263b1115ec9b087ce296806781fe01568fea56377a591a00bfaf5db2f5221c1082a38b4b2594a2b0906c1411ae3c385e64903b91466a54" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/109.0b4/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "e77cabaee8d229f0a64ff4cf05cbdc091929e0dd8caa46cc19820bc5e966b2d9e46a705c93c671e5c045a3ccc062fd78a8097b4ceda7a3a9302cc56c4cead49c" },
                { "af", "30f21e5c1bc1542b789fd77f29db1ebee9a3837c3ad4c8429c2a60992638d0fd1436870d77bc34a880b95a4dc927ff9a35ddeaf79ed3efa57b4f4fb1111942d3" },
                { "an", "8edc444e4edbc73b946cc5dad171277dda31abc2fb565d444adc7854b4735a2346e435aa789293a1c3adcbeee92d0437157ef39d07c4dd998a7814e8d8b9f8f7" },
                { "ar", "d2b57a79cbc38842b1bce2a4c07d2f1c376368ebd2ac82fef10ea191e7fe5b531944b9091ded0da9386426cff45140b0570bcbe6f63b81c565d8bb429787d016" },
                { "ast", "63d4f5f99c8c8d7925b35db8a7f67a36ef21d89a015b253f5fed30377271901310bc5f7593842724ff379c5340cbfc234c2e6190d12b4fa2f9e2f8611f8492b0" },
                { "az", "6139d25c945284eb428712b0c81d9f7e8db546b1d4e280434aa6603e033a84feebb7308ef458f8b5f94e4866f8ccb109e98a140148da0590f7d4738887a30859" },
                { "be", "85c32cbe1f12819dcf52371d44a1f3a5f3a69f442660bd3cd33402e8024b4481f35256ffa8dcba47e42b94e8cdda6c5883b919a24ff2537d88a48f545aba16c5" },
                { "bg", "55150db84751bd0f69fe3d547d751b5b9b8fe616b9666d0a51cbc81ae6d8291e96879a49f1930e93435d9a6a97383af64d846211fa591c11fbe8bf582801d6c9" },
                { "bn", "59c77f0d9c6a8524a05cd48761a92f9cb63754bf78a9f0cc38c9e3e909b17cc921705c1027707c6d1f5eb3cf24eb66dd132849f516e09958ea9761dbe63e00e8" },
                { "br", "44a082ac93d165abf8bbaab04de70ab163298795ef65e15bcf0b45cc711e51ab2d07e425017ef74ea81828fe2d36db482c0a6f96f80ac5123096293749deb26d" },
                { "bs", "453866ce9431ef6c4343aef4c6f7b56801e9713c3cdacace06f994ed6b53ce434889a274245c93ae7e874907a45f7c654e6266f59edfbf950059b23ce9d2439c" },
                { "ca", "269d5501c10933d2d38922a2a518ffc93534088892f471936171457e983134865d3e11de3d0f779b45d4dd7c37a77cf23e03b9eece4b5d5bd27940062ff652e2" },
                { "cak", "5a4089be15efcaa2edc497733f2653fc46a4bd6c13c0c2c3126093413f150a6f2fe9851c33e22779fd96606007114ba173079f02a915c55ecf52e21bcdd899a8" },
                { "cs", "40accce59c56c727a38ec49925f1c08dbf37d6d04f4bd4cf295d5058c876528c6f9a2a9b9fa2a0d3a07296da9891e6619e849f7c6a0ee766d39decbbec39082e" },
                { "cy", "495d5c7fb2cc4f5721b08e66203d8278a57c5858796f0844327f86dfb436aecc3ffc557d07850f0fda44cfe625578893d5ee91842b10615e39f3b1c57330b4ad" },
                { "da", "28f5f688725d49dfe44e73dee3233b7a3ace7a949d597632826605fcf1c3ac8d4dbb7d2635a389190c8f9fe5beb808599e534eed2150589842272bda6e8ca6d5" },
                { "de", "8a2ff1dbc466c6f846070e365f2314165019bd4ebbc71e432796450ac06197f64378befe1227b90785e5f22403d265812f7db46936d33963b33e57529c6b2d36" },
                { "dsb", "072999f5c48031cbf6326b80d6b0c2b243f012c03c6e37bfd4db3ae45e0ec0dac03639e0706e4b0f71c40e288674dee1a2b2cb398cddd9dec8e3d567f58e28fa" },
                { "el", "af8019421b1ce0a5ceb4e332d4b29e0cc9001fe2e4250d7d4ebcd2d6f73799445c953c74b8b59885cdfe7c9934b0403c9826d56c3cf280550a6141b7cd966dba" },
                { "en-CA", "3c117b6f7f089680441b12a96852db950bc6024708202a07396c96a1868df32b7e999f186214d259e1956f1d0bce72aaa4ca1ae0d5209b493e385dac11fca9b1" },
                { "en-GB", "22d249ef29170b3ddee9f99f7bf76eedee872850b309aa1196bce91ba35240edc05e65b00ef3022e3af4493a1258c90363e1feb17cab47c8d86afa8518517bd4" },
                { "en-US", "c0038b2001df62381bee611b2c667bc57705f47eaa1c5bf5e7a333b1172ddfa97b91a588bc30cb347216026d3ffed5eeb2a3d0d5b2866c97b93f05d6087ee58b" },
                { "eo", "f380861def6c25c33d228d0bb0b31bebb19d7cde2b10197a6b5f1b9244074037d1bf610497c5634a44378e764734287807ac150a1cb660d62feb79ac79e727d1" },
                { "es-AR", "b5ee24ea65c6c12501c3467bb877fc915f651e96cf43ceeafe1cd543b7cdf220cfe8f35e24bbabdd8039308eaecc7279b5c4f5210a65a795c19e466d37276b02" },
                { "es-CL", "bbcdfecf3cfa11acfb9b52da7d3cb558215956cd2e3a0e604ef84629a0bebd6dcd08ab30d6918bae2c573564eeaf2ebbf285b89e66b7ff102a1abbeaf70a1897" },
                { "es-ES", "7a024b9f86effdde8601a65ca62ecf5b1d928a907692d5eea65fb36e5987efd5cea64f1a0f48d30e3d261b3fd6f163f4641b80e78019a4fff548218f565cd279" },
                { "es-MX", "fc4ee8b73bbe4337be1e681e661605ac8b3d9fa37e393ad6a63e5db6ef895620a5598a284342b4562d35a88b49644deffd3c00eb1ec18d40d56c2b29e624c473" },
                { "et", "880a6c25adc113bee40e2bb1826398f7b031b055c0d9805d492806849a58b608007e6795780778a1145b71b409e7b71935990d6ab0d2c23282802f2b7cf7027b" },
                { "eu", "17c98af7694448fe19c220c78335d1be3fdd0473e78f8c56f1452f85674c87ce66d85ef4cf53c03d48cfc53465fbd248c9e60d28e08ae08e4097bbf0d95faa17" },
                { "fa", "054741d30780d5f9c1c2e4c5268aaa0f606b984f3c90c85ade0a9802f7c37e736d78e1dd64b72b4996385c78604bcdac74a5a0589f33fe44fdea1eb54267e9d6" },
                { "ff", "c13a7cf0fd9912414f317d3e25955df367e11ea19a5db3de8a397aac3f92e0c0575baaad69087195792f20d4b3d73f47e34cc4c94f30e676d10578d8ea25745f" },
                { "fi", "9165056b826166f7a576f9e64ac692cff216e49eee17e77b9645c0ea9ade82dce39d1e2c9103ab663dbe017b8c1718118c6b2ed6bc3112d81840ed31f5cbb30c" },
                { "fr", "b31877f556c3d256f3fd3534dfcf612a3d5d384aca610eabfcc1ca753dae062b374a81794259c73d787c27272f5717b6bf8c51796701a8915b53cd56d8047cd8" },
                { "fy-NL", "3d7e94c0e031f736b005edcb5203ecfe3672b31f5eb3afdfd99b214ec6370e167bcbd13f715618663a75dbdf0bdad122cf0b39aa5133277225464ddc30e1d267" },
                { "ga-IE", "618dc7fa64bb83cc25925d112916749ceca22cbaefd6891e1db4e0e188948f6ca56c0090c27811f3e4029f83c28fbaaae317f4b94ba27fd6a5b699a49db31371" },
                { "gd", "c7fd4ef47d5ad852e2fc146ea2ce98525705070fa918f6747be33f292f4db3536c398d9e480c3d4a4b074ce1184cc566b2bf63f2eb5f2ec44bbfe7a5e1d017fd" },
                { "gl", "8ab9fbf1b87ba54a0129d756de5bec8b6e2eaa936654937f659f1c7f5d579198837311e8772131ba51b541fe9c197cee7b1f19af6862a4cda2a6dd4366b7746f" },
                { "gn", "e0d060fc105b5e06973a92f1b4a72f14aa964bd0133767ec57fe9ec0adaf692b814cefa14f412c6113b7bd6198c992ed0ffcdc82dc36f68b45ffd1393b333222" },
                { "gu-IN", "64a31f7c3def403e711926385aee7e316b381f50f9d45e1bbb37b91d43b21917df89764f9e881727a52b10bd9ca60c75e775a5f642e84f95223cb269ff18cafe" },
                { "he", "2aea4241e48b8fa08d8c24e5abd2f9e0677e5d76a33cd3628f68042940113f92c59ce71f304096f4ba9fc70b783e0d768ad1d585059b993eabf1e4d6bff214d3" },
                { "hi-IN", "f09250dc635e0ec0bdcbc4e7ebf42d7858ec244a2555ba660f5587133315bb8d6b824bbfb12cd2d51466415bb30aa64d43854e23c8acc90146f18f673a8dad58" },
                { "hr", "ee158efc826d8c106fd1a9b5098f7b8617082f7ccf0dbded83aa3ff3932ab358fc88c4fc4223be285043df62d61afe5806a22f3ad65f15569b793c4819bcb70d" },
                { "hsb", "39ef454781353fbfd1d9a0f18f1f1ab7bb7cfad7d40c736158ed4d3442e9c691dddf5614458d47fd969ad9d13ecda0d45aa4caf6837eb32b7fd8751e83b4f3e9" },
                { "hu", "e95d377de44da34963c49c92555da33addd32e5254a744726de470dd4e6e1c30b5ba7b8d12bce01b1b4c6bfc4019d82ca30d88526d9eb8210b25f045301443d7" },
                { "hy-AM", "487d859ad274ada1393eb967cebb31424db2dce6cf978b58b8c07644ee92c59d395da1b4b4ea5d2ba301992e5175a5662d73acbf8f16e2d13a6bf16fdc7423b5" },
                { "ia", "a3562730db78d51acf918841d7d79350bc00ef698f3eb82aa7292de7a562c20b1eab6d1b4db0d41a932321fa8c9bfec6de960c5a83ae0f065b588c62a8a68d1a" },
                { "id", "83da9b27e5974061af0482732bfca7722e7e170a3b31fe2e32c0eae04783c3a332be09938fdb737ce6ff579fca763c64fd20888224208411873269d73a51519b" },
                { "is", "514aed6f107c10cb9c512135e8755cd9fd80338529b03c8ab87137ca2eb5e6857db6a8419868bf775e7049768c5ab5cf0fb37e6dcb0c23eb7aee578fe4266edc" },
                { "it", "78cd89e91db04cf9b59aef05e581a6d441e6404bce0583794dbea6d1f943536a8e312b67448c7ad14cb803e6cf9d81d6b7d219f2db63cd67dba570f6843abfb6" },
                { "ja", "6c9eb519b877994c91a1aed749ddeb9312f91e98bec3b5f1500a2253e36dd79c2ebebeeebfd16808b3b23148385e13fb01c5300061b8f0a13cedc87c99898abe" },
                { "ka", "b1ae720a8e548bc6d0a205804a535051f6fcb94fd6d1e6578f8dbdbedf674e2daec825cf156db105bb3a0a0afce59681fc01ff26fdc048e15a55e82586ec3f02" },
                { "kab", "9533cba242af3f89ff8b21f8f5998b471b7128c6b97bab27de3f41b58fb225bd3a8ff525e14c10e5b21dc89a822534000f135227f6a16b04c7860986afcb144c" },
                { "kk", "877db1f5fc095e6d74d984778c78f838a5758f6d88ffbeea889457bfee836e81e562f4a3cacc4726bc3df32d61bd431ffdab24d76640b9a7c78b31299cff2922" },
                { "km", "aeef8c61001618871489d67edfbea6ffe68bd6cab7100f6bd94f6c335f0e3f00a9548609c45c19122ce30bb0877f0dda75ee8ea057ddfe085563b1a98246bb95" },
                { "kn", "e229bd9331cb129117a7d71374c18fe0154958f65389751a1ed8735fc10a30669a8bad6f97304e7794e6e1d3f63228d2ef18389226fca76d07fc94ec9a3c0637" },
                { "ko", "890bd01afb7d9f104c9b72f024f04b9d4fcb1fbc360ec118cbe7c34cca4d10d363f017d1a06f759acf97b69d2faebfc52d955861d53364b2a0b418f45dcedb2d" },
                { "lij", "2eb6ddf16b1a9c407bc5ed818ab6711d86d88189121e36ec8c4bc046ec3afe7ed5fdc17565e56efe8fad8ad022cbe2c3d2e1e3475ed45460aafe8474b9a9385b" },
                { "lt", "410156ec5d3427d36367fd78b6ed822ea4d2530c83de105aa4726bc5516e0567aebad74435ba0aca73b5d2e79c9e3bdcbbd8afaa5af5cd7c611e353548dd7578" },
                { "lv", "6d6f4a5c7f8f8b94e4a93903f63b7d971a8e9644937925a8f54ffa7958dcaa6bd705ee94873cc1106a32eda282a93acf3ab627fbb01b7f9e8667835271c71b8c" },
                { "mk", "9f6921c1964983003e98eda1203bddcca128c4a0cb79f4a1bb582e60f005076d03d72f1061115943311d8a9a78de71cec50ee5377a49adc0158f9b8cda6b689c" },
                { "mr", "8abf79c6abdb15e4fd5fbfd7b5af6e530f0acdf55deed92905865b7a14a10fcfa217b5f8acdd271184e0788660dd1ce44ba013a9667b8b7fadb1c0e2c4fb9fd1" },
                { "ms", "c9a325f597cf67ba5d939a3531b64d36a0c4d90a31b6e3b398c6f0693675de000adf08cee73ed0e9ce70ad3410c083ba0cd879cd9ea960984b0d6e26ac064eec" },
                { "my", "5d897758ba3972901682abc77f76b605d134b70448cde584a1472af334a2caadfacfa4ba3c4a77cb31521ebbb17c4aa5c0f3ea1a58055166c73c6b11f45f9eb3" },
                { "nb-NO", "df05b7daefe1c66fb89773097502c449d45aa04d7e63688d84ab396d1278b8456bcbf9c798a59806d9683d59b86905869b7c942653b98bc8a60dff407916e470" },
                { "ne-NP", "98e30f92213f230d2eb575c6c580e9614a56776c65270289d0f4d89bd60a5f925bfad9ac0f5834d9fa6337a03690571cc66b90b7efe550c76d42f7d802f8430f" },
                { "nl", "768c2dfce69762978acbcd0ec00f41eddbf99778da31a935150b962c172312715274c74ccafd93726ede343247d959b15be019b839b0be2a554bf6f4dafbb31a" },
                { "nn-NO", "7b6d57e5a870491a715864a02e2d0d8845e3ab6646b8ec4ff95a959ba8d607415a6a436972fab9ac7e69faf3a11a57d0d6a1c75e1676146db4e1424412fb70f3" },
                { "oc", "d32c772f6d4d36175d26f546609e00a30e5b88286bc1913f3aff73ce8f9794b326c2379367207d811c655d3d291e33914e8bb806c47afbdebf97916a336cdbb2" },
                { "pa-IN", "a804853a14f22541e388efc38e53708ce070cef3b5d1697019e76d6dd5ac6fd96dd89af70422344a9297b473edb1018a989f986047a615543a8f97c55ea1d5c6" },
                { "pl", "067a66656f4130d6ec186bf679f02e45af7708dff1c863b198586a70728088097909ec4c498c4109b123270a462bf84f363e6bf11bccb9ea91b12313d240e40f" },
                { "pt-BR", "1e2236ad201bb1238da87fd5d595f8f192940a58e8b3ec4926b60643bd80f6085a037de91b80a0d7563addf00369ee086e35b2afcbb1bed38965bf5d3fc508cb" },
                { "pt-PT", "f76b03824f916ee7b52d4735868040170646aa2f9035d9b9b00c7c0b606b5c3fdf22fabb9f90ef5cba598f93e35f38db1b69388b4af4930e10aced0fdfeeee48" },
                { "rm", "227fd10251aa05b5ed2d1301ea91e48e3877c0be4998532264a29e67d6c17262dbef3d9f6d45ab8156132edcbd7be10dd61c9bc6fd99bf1c1a41b4ebbafeee64" },
                { "ro", "b5cb20572d27e6ebb1f27b8ebba167b8765aa2f62a8ce35d77e7679553cacd175a4a0cb69ab97124649cad7649c18bd23988a08fa3efb07042b7dcba2cfe5846" },
                { "ru", "8c44deb2df85d658bda0518551ebcb8ac47afbc3bcfe51706a2118307fe63eb107e1a88a90ce029d4ac09de5fce8f19697a15ef6357e861f0afa85a282bda9d6" },
                { "sco", "8b46a620a0af791b23c2b514b0bce6768131d1ac510c3a4124757c5597ed3437b1e93813ff8f4f7788639ad6609072e566dc78062a39489a6ff8d18aa167126f" },
                { "si", "a553cc0f27bec5c78c933df8abc909ca654081017c0ce32422d9dd1eddf32410975a87dab98585387185ac0e003bb5ccd0197def13da8634b594cc8f19d0f67b" },
                { "sk", "42b82dbf63c13e92918823352fa4a451d8c2e2bd6ad3001281268d24abedb68bf68f18c225452a1f047504e7bd6407c5cb14f30357b7280c50582bc50debc339" },
                { "sl", "6fbc34b2eea6d3a96e6dc1b77d16c78414d6e3cef55fa07282e54f4a3b358d9100e4aab8fc00c5ad40f86f2820698e76c582451834574074b67cfc7e969eaa9d" },
                { "son", "1e5f4f50afe122632d508dc389133c9ce44c1e7dc564614b612caa995ff28a9850de840241a755a88cdbac3e885f9178bd0379fc72da5f491ba5c242fc420fed" },
                { "sq", "fb36b303dbb4597daf058f8e7397b7909bbfa562905644d3bd83fe372d0c55b8a9ae2c06e0cf945f9ac6364cbc050de98bf748e17db7c835e610c849b24436ce" },
                { "sr", "707ea30685e9ee7c2c8de9cdd59ebdf4eeb6f2832dc71418d32b0df1cb3ee40ea190d41cfb0e683eeb156723d968f60ca192b10c2cfb451bd969b95d3faca2b0" },
                { "sv-SE", "63b0e6323cf4f6161335cf434e34a167bcb76615a0d351dfad301bb89429b9ce30972c4726d9ace3408fdf22b97de5ddf239356433881348813612426721c7e6" },
                { "szl", "25559f36f3e197c63494b28bc6e391f381ddc0e9a902ae7a91738d5f5242876e832c6d4a2c711008fa040c2728fedfee9117335ab8b6876628eed6d973a78424" },
                { "ta", "cffbc91ed53dd7b615fce9f6aca564d814b953f50b8361c337181c0f645956ba2c83f1f2deb2ca697399000d6df74f8af1be51268253f827456cd85cb6ba9205" },
                { "te", "9131cc0d3a8cd07c71cd353ded6778e41463a25e641a4fc244b69d107ce50df73d2fa0b160ab447a0e6672352d08d9dbcc134b04c3e6c6a48b69379986346ab8" },
                { "th", "6849578167c472a511043a37a474dd2334c218af808e798ed03d5db0a036a681a320230ad7c11a59588a4c125ed5959088987acaed43604d9cfeb4c277d4aea6" },
                { "tl", "1f459f5a94415711ff7ce1d40b39778ef2c26939a50bb19836edf225a19d982819d9e515dcff3a09359d596c8b6d5b8071f9b4eb15e0608430ddb8915c6390c1" },
                { "tr", "955dd1f5cf92a2283cecde15d6b7b742395a9921ec0fdf2b8a3c6352fb636ca1beca160e194a423415847787e78486e7afdbc8463b2e25444c2ff4f926ded3a8" },
                { "trs", "2be3f73eec1d9bd92e500c660be74773d777816621ae0fce74773d667c22f48ad5509e7bfe43a0d79232bb3231ffd5f6f1218d77b2ac1b5ca212cfaea71ecda6" },
                { "uk", "39dff75eb4b7d2d60ef911445dfabbc1b387d35610b1ddc011c4ee04e2e8a6aa979d1af4f54fbe846400365057d3ad1771b421bd80f45c46da64df0479449297" },
                { "ur", "57042ce8c34d932b664b1b60de6edc7eaa2f473b255627b033af6e44be51f68dde98e59a6604bd84ea03b452c6ad74c0137f94d8fd080db3b09c7a542831842c" },
                { "uz", "d23e43144d6d7d10de33b845ce8a949d337b5313ba16c9c9b76fdfac754876babd2933a31213fcf18e7fa2a8889c6f2aaa50574b253cfe97ecc03b0645afcb03" },
                { "vi", "4fddbab1aa96096a9dd6a2eb875b6f224a34709ec08c188b25e3a793ae36da9aba55d1c7f1fa8f5046c6750b1011af01ed9c73d7056490f01b715d4ec409b43a" },
                { "xh", "c96a7d77e7d4413701ae24ebcef903f44ff8449c42794b56af6b1168588668da088e07c839ff795c1cb4385812c7d270e4fb75babcd81cd2ac9cd3f33bb307d6" },
                { "zh-CN", "5fc622f95f66b177f3959678d666be9af68942d70a085322890de974ebc41e3568b7f320073e670f690dfa5d75eb4c217203d7ef92d97a0c5d9b4296d6482cd9" },
                { "zh-TW", "79475e1c7610724a7e9024a6abea8157de7e42a718577bf465d83dc320981106e20c9827132df1ef82d8938fd9f8f25db7cdbab9bfcca44096b405ad22e6913e" }
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
