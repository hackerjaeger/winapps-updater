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
        private const string currentVersion = "107.0b8";

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
            // https://ftp.mozilla.org/pub/devedition/releases/107.0b8/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "de66ec4e98f436421218941ea7c07ecb4e39ad9efe2af192a5fb5504d74ac4012e01bdf0adaa1d422f10ec4c6cfa8b4d6ad77a4e8eeefba7620cc82ff8ba9c70" },
                { "af", "530a08a116b613c64bccbf14c69360102582c8a1e10656377b2d98a2a9777dfa244b7a57e39c1daf0f10419e2ab6dfe875c70f6be1833ea4d94e38a121c7b440" },
                { "an", "e0bcbee1c6771ed2bd38b85799a52436f274b92bbef085e464fcfd6d577af7c4a979dbb9ecb9c2a32a020ff6e3f8afe917b47e53d5cc34df318b1febc3ba62fa" },
                { "ar", "4f225e753f05d6baef65061d8292dcd749728ac8641f4c204c91f90b48e37a21ffb7067264d7ebd2a8ccb4b449c242f6f57302f5dcf1968aa118c3e55e13b73c" },
                { "ast", "4a4053f884e8d830d2a3accf678b13164eacdd3b58ac66bdc807ef35aea13b2e145e09caf88d0b3dc7070d82f2e8aebe2f743c6a62fd17de12f8766cb293d1c2" },
                { "az", "09e7bc477d131c1703d56d1ba563d17c3c52dce49c40c6d708e99565b11d3c3d0db8890419520d929597469b7941037b07528e68838c8e4decbf373e6afbc899" },
                { "be", "b5256b7d934340f9338e596e87e30bfa49110fe51aef07d62b465d00d0cbdcf601d77c3d27fe2c1f5ba49781eb331ea1ff82fceb1cab9bbfc99c6e962ef28f15" },
                { "bg", "7ae01440c96936bf181a4a91f8560a1c73017745b7976ae995794ca2aa922b34e68aa65d7ce7e1b587a03f9e89fbf88b66243379ca96503b78aaa924cb3bfe49" },
                { "bn", "096de5ecda6eb6e88d300ccb4255a8daae64f0722f4c9664a3149e8d791b67611c83cf2611e121aa03917ec1f12737db677036f874ed4ad7a091a8e4a169a7fc" },
                { "br", "7797382929b06f0ca92a8d388c2d18b48cfa2c8371dc9123a653608946f22d1ddfe1ce3848ffd7a243cf2e5d50bdeb394d8bd82cccba0c9c3a0798bee9eb6b58" },
                { "bs", "6fcf9401f8cff214bfe601c3692cc849b2f2bf7c7a718298cf305d080cbdbcb7c1d6bb968452c3c3f8d85e779e45fb6012bcdd9a8fd3af86225f87748d808800" },
                { "ca", "a08b3cad82249a37e616c963a517df08b2ce4ad3346b331450d1f97739326b542e08a12294feb865b819c615ceaef0cfe5960c836ecd6c58915a050a7d8cecc0" },
                { "cak", "fc2705e9b7e1dc76e6fcb8b34e7e65fb58c3a12c842e828a5485a71729e09d15add68f60516bad37f2bb927a1980704ca17a1a17e340f1b6e255e2a7b8e4cd81" },
                { "cs", "73f8c97b7c09ec2920f6b4f4b0c0f0f9217eae60ad2abdb5729ac6745718e6773fb468a2876a42e090f858a83f9de6bdf9e4604572de534bcff40e76687f9b59" },
                { "cy", "c38fa62d8ca51672f5e1e142a14c7e0f1392c31be608f5ed1ec2224e6e2c9bf9d501722e5d5599ac89805716771699a560870c60d04d743e165099f7c9b1990f" },
                { "da", "20566e83d3f34a04a6b462fa28dfc9aad5f66231adbb7151ac92aeef829c156d9ff08c85e8e9bf57a9e4f22c1e18438928443b9ea49619459b74531541ec9d57" },
                { "de", "1aab37ab28cc44fe417c2598cda50e6d58dec6973d1cb4f1de11f9b6e5a30cd060decab15399299ea2d32b0d15bbcf97d416c0f98770dd5115014af24688c7f6" },
                { "dsb", "0ebef441216456f979c51204945e8bafc96d94065001079211f23981e50fba96731abbd2c1a55b589b6b989b092fe2f3171f9cc6983099ae2e80284a418f0f32" },
                { "el", "635ccc9b065aed5aef6a1a0dca8753fcdf19292fcc87b05d86ffa77bcacc2318e00facd4067fa30b62498578ce543534970555b0f2eb2f7ab2933f9864f525ac" },
                { "en-CA", "d7d9edd7f611da279f71f36ec106d9b506d73d7f2ac1209f7cfca26a546655fa1037fe13a168ac60eab9b1c0196f9bf372a09b6b22e87686ce6c44a7641aeed0" },
                { "en-GB", "545d7f830e823e9fc236d689c092dfb242de1929d94949dd69be8b70612160034542c9e22ad30f2dc44630e152ae2cc9b15b47fa4b36908bdd4c405a82de44a7" },
                { "en-US", "a07e29c09ed0b8915afc0ab91702b8a8a36da7dc7e99c50a25401c7ac601dd770a57f2e49c2c025e4071bcacfaef51ec6fa2efab1eeff03bb323096d7be1012c" },
                { "eo", "7a24dc7b013321a25744444b5e1c52de23974a755f8daf19f06f315f013851f97536733205ae577d1745c71529a520f4cba561b90b5fba09421b811372149ae0" },
                { "es-AR", "0b86188af9432a4d5d245d61e17cf2655ed499cfbf6e0bd6744c837a2df78dac74f096b8bd27040045b449591b3c14b10192b2e4e3a7b29c26a130d727dfd924" },
                { "es-CL", "d110ee51d60afdaf415fd3c86448343002c17aaa0b41855be6bdfd2fe314747a969de4ecd3de4e4a2eb611e9d7b38d968e6d5aad7ca2e91061dfc87e0120493a" },
                { "es-ES", "e1ca496b1f8cb384a84abd451179e80389897e65ad97161920564255391d7cc82dd0d83be315b980d7cdada6c5a2d661992e684d089f9cf66e2f56c3bf7c0716" },
                { "es-MX", "6a75884baeaae947d0868be39bd3dbc34e6c639473cb1d17122024164d5618079672a651e79ea9fa82790cc979217005e0b6c29e187319e24922aa5d36a922cb" },
                { "et", "f4425371c2a282c46b089d2ce2d37ed5b1f699801e100ac9b8f8acbb7b4847192897b64a9d0472b1d690fc7a24b774eca737af3427a96a9f31b4ee51dae688e7" },
                { "eu", "96838987f8154c4995e56d9fad05751a06c03293ba6cbcd338805a9399250425dd9ebe3a0c58c4f4bf8387742d46c2c0e14aad93b5b0094097919a22e6d8e64d" },
                { "fa", "b92a48a93755044f037c8cc1dfcb6cdea02e7c9f8508a5d27e687bf77dfa7ece74c2d74fd7fe811e48305a4b9685fb9bb497b552b87133957e81adbd04f5e749" },
                { "ff", "72f2578b53b5ca1925a426279b3eb5531e08ef3ea18d3fe7491d2fdb7eadd04d8d5c6b79a3263860845fd2fb656763f7bd530efd148542445bdd1143eb829f9d" },
                { "fi", "bc27ecd2d4ed2e2ba03e7e0ed7b14e4be76da038d830764c7b26425ffa3a21f09c461001efb27d65a10ce263d01d771ef1c9e53c8c12568e3a1e4463cfe3ac60" },
                { "fr", "f81c133744b405ced84b5c929581f515ffdde1aa01dd6cab44bfb3a0b35a92d627541885dee8f0015ed7e90bdcb0a62a3d3645e9ec29c938c3da52a3cc60ff26" },
                { "fy-NL", "ce934cc5be0f0a4fe890cda3e2079e85e314caf61c8a6d777d0d7fe099460826b0be29e5095f0643fa189fe03b83e36f3523a7dbc2118ba4ab5419235f4d1d9d" },
                { "ga-IE", "77da9e346724a71ecb8d280e76bdac0cc72047cddbc040ab055bff39b790bfeefebbcea07ea6c5b9b40870263e76ed9d35834891fba25f9fff266fd36690f346" },
                { "gd", "058ea9abd05b3b5dfc5fb6a6d5b95d29ec2b1c4fc224a8f880027845c375cc5be2dc96eb118de06e5c022891b3bd8a64095caccbafff4f8125d52e629c8b8362" },
                { "gl", "02b86c74db497b6f45294aff72db99043eb06468b23dde7f393ea64f852fe3c41c1a9df4a7b8fb88c7efae3da781b077ad08ab10944fa7969850d5803345880c" },
                { "gn", "8859478317dd9d6ababdf65f953cd5d15e0da1396922f6f785c943597187b389141adb9eb9afb16a460e19e9caffa5b3076c897e9256d080cfb878ad170d8594" },
                { "gu-IN", "615898054b16f47f477ca48a14f421e6d6e7b329687e0c12f15acaa40161ba9ac422e00cd7ebecf004cadba98f7f640c12c60134e36190f81435b75cdee3a77d" },
                { "he", "f7b20333c0133c25acbd295d303829d9c8bc7acee10b638dbb106cb2f58f4f9302ab42a436c5784a3fb268289508b44ee3c494a0af56cc650ba74fab712c7b3c" },
                { "hi-IN", "92901476d1490aa0801b84f7a13e10f52caf0527d200cb18cddd722dbb53e0ff7a20a7705fc812d85bced7729e6929d61608c505bedc302dcf7f41cb4ca99dd2" },
                { "hr", "9761eadc787c0fbbe98e27e7b2486c3f2422c7d82b6da695fa3d9fd2d860828472cad1b0aa577b99bc094c71e5c82c78c4b7e334d10eee905a5a803836a30b62" },
                { "hsb", "50eadd52937f059c934495b3486abad19d53203420c718d2be3f89ad840a00d589061548db7498bd2cf90f76b0139c93a9d7a931b4dfd5541f421093100da4a7" },
                { "hu", "7d0529227208bace548eb43e84a9106415a2b095f700e66894939dc54b850027f19f122796a3eaa9bbdc486bff129acf252eefbf82d3fbb95344a9e628511774" },
                { "hy-AM", "2536b5d773ce0e652c8ea4711281a9909c24d4e147b844da5a5733c20df8473a9f28cfcbb410ba30cea1c6eba75f2a68d932ac01ca11a38b48d128a8e2048392" },
                { "ia", "7e3347da7c146d552e6994d460894a2a205229e945fa575435abc7810729cafb203854f3f1fabb4820f772e38d44e0156636999748e845e2806e390b85236275" },
                { "id", "40df6fb4240076e98328f42aaec71a5d1a5e3e39c6fd35ccd88388c4d70f5bc88b304ae631681e6525a0b93cdeca7bfefe88e75398e8a4297f73785aeaa33730" },
                { "is", "aaf4f4d40f4489a32e262d5710006c691378a422f639cade26323d383327330d54a52fd312eed16f67e4c4cbcacfbd08d630e0b9dfec28e9524b4e20e568c541" },
                { "it", "0c1c24c031af659c8eb991a7cc8987dab7edd0c099b869ffebc94a0cc334a8d46ee40061bb6638837c77d659238a7cd34ca232d5a8df474321be741e122f8b21" },
                { "ja", "80715a9ae4bf036123b99cc4124fa20f3ea58782f3d27a938a0559ac15a277d382a1820946e3349e1a1d472a39048fe9f88797c84afc35e568e909283b2c3941" },
                { "ka", "d29c727fde3f8666365ad54d90d3d689c8db5e5381ab1bc4a2e796fa2e20451a07e4e6286c5e2246ca70acf09d48d2b15a5250dc9b87db25c3b2d748e038eb6f" },
                { "kab", "022eaa4655c17d4dda4b5a67c2b85caddca5cd39935a0a049e27ceb9f7b3e0c8cac029e12121fa12f97992877fd2dda44640417d5d772b2a40ed4246e01b6ba6" },
                { "kk", "c995ae876b07bdd91cee4b0df0d8f076ed0b85e33e5fc8b07224870daf06c7b6142e4c5c02501793004404cd9dfa840db99dac5f2bcacb5b8a4b9570a413673c" },
                { "km", "eeecae5eb4f6118c1f79cf29e80b7f1a17e49da1c2b07aa06533dc6c0be6844d2ac067d7bbfce99d750fe311fe703cc6c186ee76c5ab001699bbc0ef440a74c3" },
                { "kn", "3b7c23b60b59aedb21692e67e3c85c42dd2d1aaafe80bc33968e07f6038674afb9ca23f186ce5709b40be5fdbb68dd93f5e06190237e09a0d8003d59da0eafb4" },
                { "ko", "7ea4cbc738dacf6a8ab8e2d02dd99ed806c66ee12fed634bf52d4cda255aef2bc20391cc3076de9859778665c23d2cfb8e8c19abd9b3942e29ec2643e87c9aad" },
                { "lij", "e7bf8ce27bf3c7143b2dbfc67c7ae4e400783d772e9b6c9d894f509d2cbf03a1f4f94544a2a908f77372451eb1143f2d4ab63a9fff6a3277229b6e1bb3b40117" },
                { "lt", "24a699725d68f6a91be0a996dd64be5761e9147bfc7d1e8a56aaef115923294b951e3f432be2b8a77d57dd5646c1f6101be0565ee8b5fafc6235496cf2dcfe4b" },
                { "lv", "9c132ea2e143e5f2b16f71ff7a68ddf614024f75821c60f3a468ee2908a0344566f490394ccf53628ee91a038e7df4a531295b01e39263063153fe059f44c050" },
                { "mk", "c0ae87542015153c3e486956ffbd108a754a8fb599b989b39db169a556fae30c78e1ec80102333ff1a713490656b58452bfc0258b73fb2d2eab97a840c4248af" },
                { "mr", "f463594068f0af716c4c7253a04362b5027e30188addbc32cc0a7fea712528f6b01b223abce592df35bc138498935103a695bdf96393458436bfb712df98b049" },
                { "ms", "3cda57097815fe15f3d88cff26c074ee34be63e61a1f1104de549f59f08c943c9577d09af6f2bb1287becb78ba238c24a48c34e9d9e2ac7c7e3db9554d9ab508" },
                { "my", "b3c212efe5af14eb73cc5685026f66681834f05fb05bf8552a80c2ba5449602a610591a1d64a1c0b4b36f0e3604d5a7859969afe39cbd63587102eda92cab61a" },
                { "nb-NO", "b9dd0fed373894355cbd4658de362bcfa8bd325e545cd3464eb935f5ca849ea1ed8fc5e2ff5b03e8a59d2d9d98868ddb134f5c4226b06b743ad7802c3b163ecb" },
                { "ne-NP", "c6f50f6dacbe05111100ccd46877a3ca90c6ffdc4bfcef33920c477164ea1089a12665e8fd8694e407425a135b7120c96484f55d7317b97fbfc34c3e2f94c42b" },
                { "nl", "fa242231b32ee468bcdb477972af09666759d73aa14af23becbd7d6b580cf9ec8b224c60606e10341292a1973e2a172532d57eb21c1640dd2e70f75ae0c46c78" },
                { "nn-NO", "be88360f5e08a4c7121e9406f1dc400345e540655e01253e8ba5a1c72bc159b3034292aaf553cfaa0c97d8f7db939c906b1208a56d2a1e3c2911864d68828ed7" },
                { "oc", "5300880e2698b267307af319447d5de6f4575541435802e6248155fcc30ffef60946346466d7de8e1fca56bd457e1476809577dfd715bee304b1b92e74b6d44f" },
                { "pa-IN", "2245d4894d5165449f6560873f54fc07308d33ce74020ac33db895d8e254b0a7184c8fb61434c38c294b84f9e0c32720220b9909b25ea688afc3ac4510dd3877" },
                { "pl", "452775394d722b285e243919e938276cc20ae6599afbe908376d99798b81f5afb572c8b7fe14ee7ef265e3c8b8f7dd049cdf9a4025bb671776b8cc99452eae3f" },
                { "pt-BR", "0546d68b0854c4acdf432510f56768689eb701997f8a6d6651e8e9babbfe5524c54bf1bd00dee91f886c5ab75d0dd9a0e868db0d930ebcc6ba6abf84751a5452" },
                { "pt-PT", "49f5c41549085da3681d1afa294f99ab47c6a91d7a2f762f29f71bbc04581fb41d81046cba501bba533faf5966790f6561520ab1082859ae5b2806eb514ad62a" },
                { "rm", "c6e75d28cabbdb18f7c02afcc7c030078c92b159a320fa87f0ec1e5974af2f83365738cedf52df2f9220b7ab5501dab82682f408dd33d9a850825d142714dbc0" },
                { "ro", "d3fd54e689f6473550e3ee6cea1f02f4dbfb3b1e8b2717880424a0aafad153b2064b504e189b8f8fc24f7e1230ef5a6f02969da4f938d37564e2a485f5a31cd6" },
                { "ru", "f98054e40134aa4103d1c779243924af3e9b7363eaa835da5a7f6f36edf44d19210c1d3bb21927dc0b9144539f15a7a204e7fb22419a9c9f5169f977fbc9289d" },
                { "sco", "b1adfaaafe1fc4307fc93eca787678fb14aea31d560f90aaea315c8d4e844eb1d41373b059dd6bca39b592151c4492bad2046716f011ecdfbbcf6269d22410f9" },
                { "si", "c21664a2035910ecbfda3668fc9a6ca7d0bde4a69b37061c12e5943c5ce294fcc05bfe4d2130f9c54d1146b2c3a570ec44e2a07b71ffca9c41e4eda3e98f558a" },
                { "sk", "d276e877085104f461b04fea0e6bf2e18adfe96066eb135c575c1285f7faeb6b9f725b3ca6268cf55b51e54700992e5b257ec96f87df1ec482c15f140d44c5c5" },
                { "sl", "3ec29859a29df5c08fc63759ec3484c87a3d138deca401ccc978328fef5a0bf854450c42ed953d8ac7f3052131044b9082e35da8f3a55a4914b9e30271f7a5c7" },
                { "son", "c074bbb2cee167e718225a0cb1aca8d97b7ff4ba41cb5366ffe088bf4f4e2c10ae684839b811d60416512f2122712148fdc7fc1b170b07483d4a036d5dd04f01" },
                { "sq", "0eac81fbd93522a577c0f93ec48206799672f954b9cd67790bb1446980afb4bc052e6a934f232abe78fa0fd5c0d8a8f3109127a2e49ff62aadd879ee13aa2fce" },
                { "sr", "9dd52a892667af3d8b9132a996f33a67387bc04eefcdf2934b17d5535adc0cca792428d31ebb78fe64a2818b8923b2495a2075d39c34109a6f2a5b66686de66a" },
                { "sv-SE", "4c3d01f2d3e16a0c16f21030ade0f301df14489f8cf8c967a44e6b1d4eb7338b07ec2e6b8d8324dd236a2b253fd90b090b3e100c4493a9d2f87dd33421de0f95" },
                { "szl", "4eb901d8dc99f6fd41bfbd3228c11f88797ee2b0db06adc5121215e8b4e31e7edae1822420447e8dbd915d1f1d10f0481dc0863e4ea4ba3863a68a3afc01c2f6" },
                { "ta", "9a44c64291fe4c66f5cd7a583fecd3cb141b4f10c16f645829b111e2dd473baed78332f6b13f7de3ef7a1419b387f6c576bfdb506c9c4df604d09315da4ee0e7" },
                { "te", "e14474b6c43c3a0e53f09af5e80c721162ae35875b2622725643b77c037d72c745ca0eedc4a30e55d95685890b624c4216662cbf7ec10aed154f99fe91254f15" },
                { "th", "26beff25e8995e32ed7e6341d406b0cba493f1e620b561b400f6643a702e09da8b25c75c54419e2525de8361e7a7ccb12bd3ff8f76b11f0f7d26d6994579dae3" },
                { "tl", "339a5962e0fe8c5fd7f3fd9b655667d1619d949331d1bf75033790fc518139376a4ccaff7260703980470de310e047f44ae16cacd5e8033abdd15f1916d4894c" },
                { "tr", "196f2096b0ace828be5bc3a598e581dccc15e5576b36de271a501b28635965b3d3573e31132fc2e24c30219cb88065b4e80474833d9c7a76fdc72d9a8f4f8a9e" },
                { "trs", "e8b4615cb64f1501531b1c73e12984fbf119ae918d219b3d8e154a7856995c06cc4ecc70e1f596672193af4fe1c4404fcbdec66fde7146136842a86fc4153b00" },
                { "uk", "4be4b7764870d17eb4a8bb4b20f81002ab5f80f7f2e9687004984a526898468430e111337a52fd1b5f61e5a9fa90d218e60cd3de8e5fc7ee0fcd970e514d45eb" },
                { "ur", "fdcb35672fcd5704f4fb1cf8f9d4343ed403dc8ba226f2044ff7cf3110d4af6e819a7b06304fe3592ce5b6465b6004b82ce5464d535db6b9143d433f72c74939" },
                { "uz", "194b5b3fee8eabafbf226c8399dc2d0a36b2d3fbfa4ae740bd0857b04e6b2462fbd30503edd3f8d54cd0bad41f77db8c2271773282550fc5ce928c60a6c9c773" },
                { "vi", "d382d88c28cf085c371232986f8fb3bd2b6da1442a9d4e58b85b78292230fd2289f1f16cb7f35a4948afc2352b030ded664607fc7db77a8730c8e33e6cd1945d" },
                { "xh", "e1eac0ad46d5e17b90ba0e6dfb91947aafe7a80b50e5d5734d1ef84340fc9cdb71948410a6791652f7108c10438b7c256132459ce486a9846b8922fc51e56933" },
                { "zh-CN", "55f995b4bf4feb38579f286b92f21ccb85d763964257e8da4e9fd33c8d51a34566ce874b58eea1f5dafb21bd2307fe7074f8ae71b95e238c688dc637c80050f8" },
                { "zh-TW", "e8df0117bf02c5e414cac8e7bd78e9b6c5e433e8fe6615034fa77112b8ea4f82149e4f5b85dc3f1da2359cc2109fc79a1ddd526f39c34f8d8cd22be2274d0402" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/107.0b8/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "e4a3bcdf81db97491baae4069016c297ec4f818c329486c7a051a5f3f63852e47d3ae3a8dd77eb8d8d290b6c9f4383721129d0149d319ad1713bc595713eb78a" },
                { "af", "a08966f50dc075f90e71475789b37e67f14b680eb3a22a79c25d8660bd1e9dc09066883502d08c574b3f521d73cdd2f578b7c10158d63a07ab843bd48c7f4f19" },
                { "an", "fe4093857370aaba88fdc645be4a6455821695304c6aaf5e2d1e7f7c0d3be9410334f4f91ab6f067206bbb1cff9dbbfd1ccfcd8b2be6015d1433991969da8180" },
                { "ar", "4bf4688e2f38dc076d81db87ce9d69d8dcde4e3412f23c551eaa7a626a9f90c83f308d559d7b0cdb3c0cab1bb712357f45a7159400e258947c80c8021d5c0367" },
                { "ast", "23f7eb91d6d3929cf6c05e5422ad67d27bd053ccff623c44f4af742c4dfaec56446077e053ec7e262023852b28d243a88dd19d4c53896f33fd3c797041d0de5c" },
                { "az", "d71b0f78478ff3e98ecec4cdb1049d11e4a72f624409e7391e0ecd2a0e0e720f2fe3edd584b653cd2376530ea61fb8af170f11665ddc4f03cea4bb028b2bb2b6" },
                { "be", "1e0fb6b29358eac2c2304478558f98fcca81967f4ee54eea2c136630f1e037e9ffd53366123b22684a03d63552b8838abb5030456ad42ac042df400eebb5d435" },
                { "bg", "479fe7b8b1b9d19666bb66e55d28f1bc6a96fa79060f39969d0ef865b3e1e53848732bccfbef47ad3f1fb580e936e85b8f2df63d38018eb972f6d5207e2428f7" },
                { "bn", "34132a646be3a019b1009d70e7480f3932cc9a5f632db3e75f1456af0547417d9fe2956d8982cecb7b801d131021ef961b6eddf3fb344b290a39475833e2c020" },
                { "br", "65abdb24dc1828245a58b1a2204218bbfd2eb9a2297856f0be50bc2759ba41790fe6a76f0ad79450e0097a7833238c12e3e573e72df81bdbb2dc15aec1a906b7" },
                { "bs", "2ab4a724233502d0fccbe66e20bd03231dbe1ce5552abb57b235bea09be81fbb6d90d6a8d7c18f1e215ca1ba09bc1696e64f10172aa185bdec508401150728f3" },
                { "ca", "93d2363735871c1c70ca400c4c9c58c71004bb44af60e71d47e111a60f655e3ec0276c0cce6418f1bd121001e4e29b20328572d45c2da6a0965da4e30c263aca" },
                { "cak", "97abb120b3b4ba46677a26f14288b876a2fa9707596f78dae6e790c3663eda7ce4c7a7113a3ba16ece5d35e3f568764c1f163bb1b33ce982f150327e8c587f20" },
                { "cs", "d759e3721df5c4221dbe82da519598ad80e53070798f0100eecabdbbfe5f5f93dadc1790d82f2a26075ceeb0ba72c411699c0dfc2191b19cc2c91e201a5ca0d1" },
                { "cy", "f412dbc9b3e304ad5dd2818a51f01340592b394a351b7a824f3c6ab6759cffd8696f84af0413c26a10eef410b5201accf3322da05a0968ea99a507ae6eac3223" },
                { "da", "f56633c054274f51dadc37d93849129533bc90d7590a0c9accfd5e1374e251cfc3e60b91dfcea14c5272c71cfb5ad87685d6a5866faa6843130bfadcee8e8d4a" },
                { "de", "b25a8e933c0e652931bf7aff57b2bf55a2fdddcf70464db45ffe397eee0d9319baad8554aea488429ebc70264b639692757b9c05fa39563f31b026662ac37fc7" },
                { "dsb", "b6df093284401039247b9bd26d479370e71885b3c8488d86b92b5b3a67e078aadd79c6d5c02a46e3ae4916e77820bbb368109a4c6d6b6731ac8874e27ff35ec2" },
                { "el", "32da1a07e89e29b492a0520a4586a5ea9e5820a8013a0beb0aeb98d743766487b30301821a2c447a9d2a0912c18d2c750fc963003e72881b46bea646fda54539" },
                { "en-CA", "dc4503d759a8f53908fa41e30b565df16d41b7eeb344a29763094cbc13991f6bff428dcc103f27e869e2fa1509da18e1aff7343cb24fed4ba18bbdf8fee4c8d4" },
                { "en-GB", "570ed677d7fb498977291d9627ad4083bb33dffe656edb33751e449cf1d585dfe92be7118cfea9a65af8988be293a99def94c2b75943186ce5bb5ba885c9c7c6" },
                { "en-US", "5911909254d9dc5cfcc326ccf904669aeb6bdc0e5afb804554a1e8e2c19d38186b9b0ea99acbe1a0c5b8bafa68b07d9f04bed10f237f95b7491f01b59e135eca" },
                { "eo", "28888a96c6026fbbcd1acbb0e6e6914bd4068345ec6dedbdabeb3fd9f88cd9ee524c2434360da7f3b300c75e6c0b6df96345289beaf3cb8d54225bbce6a5a504" },
                { "es-AR", "874882847a6f7cca5af9d1640cb8e5411f2eab97415f02a75cfb5caa83e64a6bb9859e5a5e8661a2b7a700e0a30ae184225a4ec6741e32aac274aa27816bf4d0" },
                { "es-CL", "6e82fd388c386ae043c51c801f6224818859df906476c369294cf16c3d8530ea5734458fc079f91bd8c6a30564fb33769ae9be0107c935451a4445c9bf2c026e" },
                { "es-ES", "5e0650870cf32db6a817220815b66398c84f33bf2e80b7294da421ce3ed538ccc6166cb82c12786e261af862ff7e7ed1679fc257461c9e5af42dce01e5955dd4" },
                { "es-MX", "90fbfa9a4995e3ea08780ff4a455e636a4164e372c0bf704299f1f218e60d2398595e4a135e244a001c72fc8a85faacf5f4a8e2c082490025c57b516039abf0a" },
                { "et", "cb7e9352f87fd1644a4aa200219ef6a0b3351c3d8492da675d6cc98d5c5e39bd529109f7f97e5ecaaff82ddfa92d1e8fae7959bca7a57e7e0c6ae39ee911690f" },
                { "eu", "4a327ddd6f6b285a7ce82f7b5e1f984edf87da58707f8b4f119a9387d0e2c75e7a217aae106575e7a31ea9473aafb4a014d7d75ac3b5f9fe99def2312c94aaa0" },
                { "fa", "aed7fbf488d8fffe9ce55dcc8e7f31a69831f0cd8ff6f0bfac1ade89ac5d1ccc3f4b2c5016b5d75a0732ecfcd44c4be06065bc1028beeae092a6dcb9386a1f95" },
                { "ff", "0840d1c38eed5a2aa9e6265ff49944bf9c09b4230caa72b97b55ef4b3609c9ce61d547be0d89af2a70f0843b71b1e1db36a4df15fdf37b168d175188949b29ef" },
                { "fi", "33b460224bb7c6e9bf5d8459894a25259ac27f234e2b1f5976fe0e2ef16c9a3eace811dbe8a9a4069e96c22bebdb32242550753d2601ab9a492888d9816fa824" },
                { "fr", "25d0887bb13694f780c199df36017a691387faa8bc6672de5f89e503085045130a6af3c964ce292d4e06520d41fa2d61aa244d5f626f92816d14ad9abb0fcc5f" },
                { "fy-NL", "74384bc1ebfdfd2ad439e46dfa219e5feb3ca1b48469e881377b1b5aa282980656956fe76828eb7c1a1402fb609ef6f6e238a068003548bb74f44538e0eaef43" },
                { "ga-IE", "385b63094c6331a7482bfd75e0230ff4ebefb34a170ed324e72104c4d722cff2c99cc0b0e29e17c8e2c724395b34144e1e974c8258ae9b1e92c20797efc84ff1" },
                { "gd", "3467e10d88abd41f62938b395551bcf3f51e042157a55c5a0ff08d233b12b201d9bd052f9a1ca44425fcb8a8178e3e79247406b8f295141363303188d2361754" },
                { "gl", "8d32ef2618b8ca66ce1831619544eadc9eab8d2325e86a4f677fad03eb5e3c3d7c1c25bfd0fdc7face1f5374720aeda8a284441d09f605954d0cfb6e6f032fe4" },
                { "gn", "850d433d8be4678751f03c01c5e4867059b06810826804d08974a27c63b795f21ae9f1fb956f1826d6dd75280229e63f83c5b377a731eec02d6597f6dd85dd3b" },
                { "gu-IN", "8825263e06c3462820d4c8f5aa108d7c57610d193a26ee751ed81465a724c7fca1bbf356dfc909039b4f9c2cfabd3db67c5167ba6a4496994bf2a4175df425cf" },
                { "he", "7c0de811e6144079318948a1d9ff20802aa3f168743a78da431acf7517cd0990a7ecad120cdddcd55ff8d534f1ed91bae5d552f9db228a800e524664e2148887" },
                { "hi-IN", "f2640ef6123095c08157c0513d058d9b821c0bfe9bdfdc9406b9eb5e0defd399a85a0e33cfb53366f87a69436eb467eee3feac18e5b001fe481d6117c50e7f2e" },
                { "hr", "0d4880b0a652bcf8e71b62b8b999b17f1c7ba3c3fd7391c1954bbd9286187d8859a3849d02d40531ce4f4ce74079a0eb6eeca1c3a7d94f814c5b4f635b239b83" },
                { "hsb", "cdc5606c86ad01cf1fa5a9921319ac24838a4639d25a83d8050a7f8f86ae9f00b507d294fc78f51a07ad829071e0dffe020df46c373d114839ba89622763e9ba" },
                { "hu", "ff14f474a268577137270bd37b2defee7ea58a490ac17d332d8e3c88a30d9a59c38f582356ad662206183e6e43e71832c95d9373afcdc3da04673d042336ed71" },
                { "hy-AM", "bfcd1f86138fb1025c963224994074007deda65b546cb97fc556415b82aeb8bd5eb6c7010ebd8ebece974d8395f0078675bca5a4188601bd1dd0b268062ba157" },
                { "ia", "a53dc348920baa83b68af7c70f6915bcc929a746492c32fe4ba9f472d60f4279691a0b2818003ea489b07654d98ac0817682af4a5555b3daddf4b776dd08756e" },
                { "id", "4fda54037fe740af37d310dc5e9c7138921a2bb114a23761302a98d8a619c7d612f53c38e874368aa51a59b5c0c0eb37e676020738da5bc1c36f0acca2c9263d" },
                { "is", "1d3cd94a0c10432e4a2425b6b95c76d75994ca2682de85891f25b13a50cce90364b8bfa0ad7680448fccea9692e58953dc28568d5ae8b515902c3b33ec473ef3" },
                { "it", "489c0418487a5fed3d4a08c5017c6a4b64a6bc72a7be0cf70604dd7481d7335ec665fbef8b4823b366bedd885a81325980b8512b14c801aa7e076eedd77351bc" },
                { "ja", "06daf5e99a892228d31d65770025b7603ea688e791180d24768c6a40df1e154291a111b866ff5d5725567962fd86288996229df83f84183664b9ff47a95425a5" },
                { "ka", "67bf349838c99a2cfc65d2105dffb6bc947e22f90d6114c6137c631a4535927c413a67025daacc270015efff71cf4d5edbd9a4090e6558e613e04139d718b6c7" },
                { "kab", "d955347fcb9eda15db0ecd4afd15808c867a098993d8451c5f89c5075544751296d4b9999eb793935e31c635e382dc7edab747935f534b83d22b2698a4100eff" },
                { "kk", "39f4408ab8baa62442b954291c6e417c67fabdbe352769e5c2d6c81748bb6df5f6cc26cd988f272f645629f03d8482ad6fec3da51d55096b60445b039166b9f2" },
                { "km", "550b3c5c04d342de7232d02201788998a06223ded75f8a2b4d34a3c5b77d2a12f5229a82fb7570c9512d4929a289f913a3e635bf21c8eadbc8ae9266c20e3b9d" },
                { "kn", "6d006cde7c06e393aca208d149393136e87e1599eca168b19c9cec47ae4bf83a3f123e110060aa09d4b831f64e7821d46edbe697de4fb18189363cb275ba3422" },
                { "ko", "b656f8096397db3250a923e243b31e0ba75c935203df5d7f3c45bf4dde7e6f45da7b497b49a7d17902e813e8df2049be940c5842cdc4ac61e3bed380c4871752" },
                { "lij", "d2a2d5b47b3dc3487fe1c53906fff7fa67a3fa4956e5954c8492adcf31adbc3345597139ea833d8113762c56644bc07ab17f8e37146ee69b4e80b072cfb20ee6" },
                { "lt", "18017ecca4d162c2dcd8f800f34cd153246f4f7f9e976b08489eb01409c95c1e4a75000e40d53ba3d24a5ff4f8d30047e244805a8ad8c04dabf190a3208fbf8a" },
                { "lv", "33e332b27f52ff3852cbdcc1c3a6f515b44a8820f7d50d9be270077e5a873f8792ea54c4655c4898b19220eaa602d8414a5e50df966da51dc9e639217f94dc8d" },
                { "mk", "7f54f53c21cafe210c518534ea8a40a0cec2aa661cbaecdf85b7f26e9f97c23d188fcfb06d86f31ae81d755d0f28c6a8e9e4c25b82a01e5d71d213b1e1a6be2f" },
                { "mr", "c25845f5ac001ff8b3730eb1e7dceb6dfafdd0b8d96d891369e6b368bf875f983c9679f05cb5df750b1aa98f753f12b46a0ef8a2b2b782a9ac82d9a3a8ebe165" },
                { "ms", "0cfcbb8ab33a0e2d2fad8fff7110ba4b4382ca6801936f3961fabb53f8cbb8146ade42a30369753ad681341c40370706ecd2a2b0b859fb4baa2e5ccec4248fa1" },
                { "my", "097269475bca2cfd21bccd84b8d90f8e850905c63395efd26d2384860db8c6bd17da528f0eda2633a29f3f01879fe42ef10c2f63a802ebd603105e447b50e7e1" },
                { "nb-NO", "72eca64decbda2b1fed572966038f5fd1193737419878dac15920031842085e522e184ff7e0bf4ced920014b3cfad03e1041b15d5b9f7004d028db446751a9a2" },
                { "ne-NP", "c4bbb868cdf2dfc683ecf6fff73975503e5818edf0c237ebd3a8fff3f4a4023727c8621f9ff1926568bf41b9ee9d02c283f9a038fe442acd5a570a611e078b02" },
                { "nl", "77327679334507251e6379465889140805e3751a35fb2b3f2f4c795fc836bc17d2fa8dd5234f8de33d7e881322a5a28a1ee10d41cd9e89586014a30d5bb3e74a" },
                { "nn-NO", "aaf1320fa35ddd5b124e9595e8a8ce24a8d4b891c0448e83f9bef7ac26603a2f73e0f359d5e3faee2b18a6fd7474cb83640b346848b1979de9b8441c85bcccc7" },
                { "oc", "db5d13be96ddff3bbc7e476faa9adfd818ce8bd055268be7bcde1062a0922ef3d0e8ff788ec528b328e1b6c28d7124ce13dfc07273b8e47c8247e53f07ce2279" },
                { "pa-IN", "aa1f41bc7e6711bf691e7d6545fa699a72a432f2e6d6e5130dbb1b32b8c46dd7b839c8c789eb1fbdbe425a27f7aea54b2314bf35e4f6bfb5e4230f9809f136f7" },
                { "pl", "89d457cfb0f4230b50c09a5e83a70e93c0e840796467f1a25b3085d1263856938f560564fc8815128ba3c26a12d9698c9d356cf3219c0192bccf8ee028761261" },
                { "pt-BR", "1cd7fe0faf995fb5aa6244939a24ee32fb7cabbbdb245568ed1e1a70db9e4855bb2d091c33d9d0c2ba11d1ae058355fb7aade62b4741a6643f4c45a004120b84" },
                { "pt-PT", "228fec40c74b06707fa172e693673b424c22b07382438c43ef10f76ec63ad3b2853534065f58c3aa2dde3470ab6d6f068b798e4fa5da602846ebf78b57a9c01f" },
                { "rm", "3448b188e3bfdec6e55d44eaa91881e10eb551b940a91c553deabead1f2a0a904767c0607c4171bf20eba2d8e21c51705f342cd9e1c2567d699234ab96568816" },
                { "ro", "a0c295b6e68524b990cc0cd3d79f0e72af41a67100a6eb0aa1815f70852baaddf16d1dba5e1b11f41b29f404cdb1eb75a5f85dc0878239e886dc3b1eb5cf457a" },
                { "ru", "ac19691cb15efa8959d8ac6b20e4ac583d0fd1cdf6affc694abf2dc9785ea0e89f47c7dab43d598fc9635e5596a5e5ef45e000c74477f77c8a002e21e5cf66a8" },
                { "sco", "fd71a711a00530f5dad8840728fd63ef20ca03dad5e246e988ae368ba328ac0c4fb48ad62c37fbfb3bf0543dabcc6b364f2be6a9ad42567aec6f8fdd6d5281ca" },
                { "si", "77b8d37f8032556ae1e2b87e6f3fe4c6230f0d14b65fcf0baf454742bf340aa5fb4e10cc6eab9dacbb4c3acbbec9580c0af288760b32ed8da7c42427c56c8a94" },
                { "sk", "74b99db5befaf431ab7a91813746646cd529d78503c8654d9e0563e7a23e6c3d2e6ff7b91d96d6046908b3a192cc7f34cd950a9ef5faa9e3ecba94dad1135de7" },
                { "sl", "30c2873f22368356844ba1aa61c172eff1f59e90f33705685aa6b51623e24e590ddea6f2bec5e8ca7b7f92686468e9b8a1beb5079d936d224da763c6f02b8a87" },
                { "son", "cf063b8d45182fc68959f4e8bf143d5e2287ee5f27bcaca3e6eebeb9cc996840c7c6826102aaf781d4204de0e6bfbfaee17434f9a87a378138981d2ee7f28697" },
                { "sq", "0af4b3a9d393be3bf8f97d5d31322387c85465d68d5bd3a79876e574b8c471e20a7e817338077985f52dc7e617ef5423ce8fcb45286e6141a4b3726be3a19512" },
                { "sr", "49f4293e1e698a723b9e08a70c9f01b1995b0752451ab085ab12adf0751be7b35b268afaed08dc5f92690f279f900e1837db81089f43abb0fac119526b8439d5" },
                { "sv-SE", "93cfd0d6922504c26c0382d56f163b01721429901a2dcdf8a8301b3476ca31e832d6b9c3233c5157de70620da5c35307946fe5de7e9508d93366e34d23ae4202" },
                { "szl", "af250670efd990856d641a3a0224c57f1efe0987a2e0a577fef72c3a0f164227685e7d79ddb01d2fcc9d3a3a45b6e53e7e78a90a90363b93f3839ca65cc054f6" },
                { "ta", "7cb3a20966224d1b1fd88b02bb82a8e1ad3e2de5aa4e9ad2bd785e1fcf2f28a168a5ccd00def0c14a4add516d2067417c8c2c9fdc0f5c72ca5e51608e6bd6428" },
                { "te", "f7efae5ad5e22b362a081d3b83825328cad4b69a705d042e3c29f61014df0b81e1e0f5e0e2357c5c635ff24fb929b878affe32582689d14a8068ee46570e040a" },
                { "th", "4c87c51a386faf6a5293c45268bea12045441fcc05120057c7411fdd5c4ac8f6d27695e457b396ed6ae2bca64bb17d5650f36dd3d99dc5d7d243aa7cf4a7878c" },
                { "tl", "0b4a3a777ed88aceb1430134b2724d050b426e76e94e9e7368ba2a085fe6a2e2286b21b42819f85f8b6004488033685b33083c3bd5af27fa23ea86e494ee19ba" },
                { "tr", "c8096e0754f31c66304952965133a1aa421f0836b376bd1f1292e6d25e2a4e7f5484fe2f8786bf7c392e725a9db8aaf4e2e41a7c581e5c097943391fc798800f" },
                { "trs", "3f02f6153d378b3a50c56a535770f22812efd0f859f53a42b710d2e6be477e9ecad4bdb05c660a6923e3428501fe709530bb48d3e022f1daeeff2099fa510e6f" },
                { "uk", "b825575a9d518d44b93a7900070fea6c935cc48436bf7c25615015fb104cc38d7b5835e6665c511f5980077a1fbc9142695a1a01eeecf045d7a5335f50fc7bb6" },
                { "ur", "31876c932ecdaec2e4e1c963839156c44bf8124eaddcc5581b41648bc6456f96311a57f00ce49484e7bc3de0f83273e60ac3bfd911df71dc1c0e26c5d2a05364" },
                { "uz", "0f42b2325c4b43081fa145670e7ccb5b4c4b2fabf649b4c9378a070a997285e9b8e617e23e16e5e89f861aaf6569a5bceb5fe717d9ffa556f6d06ca3db3d9e2a" },
                { "vi", "d7f6e0f32b93f90738d4a97eedf68454138f47d2c0eb198d474802ff3ceebe542dd68bbe35ea0fd87496bec283d1c46982facd16e2cc6601fbbabc0a46865e29" },
                { "xh", "981d6feae77d7f5ad0340414427757fc237d31231e3eab53c7e8a1348a21f200feaa2c5333483e25a4d0660afa5166828d93f128f86703364391826f5e9523f7" },
                { "zh-CN", "19d7597cbb1496ed785cbffe1925b86f100852db61249abb71a7fc83952a23a95b34e900b3bd25195da2b6453789875abccfd7f94a018ee85626a36e940c1913" },
                { "zh-TW", "3069c773dafc92150623c82e7b9f05fbce9fafc5c7337d5bd8194427b1df31b1623079fcb8a4c0141c014330c36c240c7f26a434820a22628770bcaef6008375" }
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
