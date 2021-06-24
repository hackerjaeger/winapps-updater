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
        private const string currentVersion = "90.0b11";

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
            // https://ftp.mozilla.org/pub/devedition/releases/90.0b11/SHA512SUMS
            return new Dictionary<string, string>(96)
            {
                { "ach", "edf6b0c93ec541281ff828cd266e42e2fb43cb68d220127d16bef74501fcac9ccec159b1c7235489a845f4fa447c370723ffabaf9c5830b1d81a091297d53a31" },
                { "af", "43b7e5f1a273d479280c317cbab470a681ba58508abddf221e655c5ff99973e95506804c0d7d705d67de92bf5c6e5a61c6b776a41956340121699ad00c77eb02" },
                { "an", "79ce37b0c66f164998c76d3a8d6b2c97310d797fec56c7cedde4f3c530dce3fe116045c94cda629a2a81858f5b820d38154cf5ec775c26115ceda7ee5ee2ed88" },
                { "ar", "0565dbf3ec1db799fe147e3c06d5e33a89a75a25976c9b63ab9e1d6490ec3190723d464fc0bb2d4f63f9d4cc8ae20cb50de604a8443c41eefd2c6f517d116dd7" },
                { "ast", "e879f7d5fbcbc9cba0b587778eb7f937b5e9704b389c0ee67154e9f03a57b669314bb811244da29abc583bc4add413aaeddf5b0d4ac40c70d4e4b613e816ceb9" },
                { "az", "0c280ea7bdf25a0287fe4353f70eec8d5d767fe6e5c5a4d1499b65b53266c4ef17ad30f800421101ad66ad27a23b316f47d4182112c86753c7e3ba738fbd82fe" },
                { "be", "0410c79931109f2975164dac9ad1024c3fc5c68c3bb10ce986ad5b4db6312a83b2cf4e29972cdacd42b983d650822e53633ba0da1be0cd21c248b942d4cd5bd5" },
                { "bg", "8b831fc163ae9d6b26baa431a43edd9b2eadac3304d33d3eec7508bac521ff8f49a1dd92d9462476d0e73a987a1ad8bfc2332c4ea6b6bd3757ccaa587fab2665" },
                { "bn", "340667dcb448d35fbf8dfb408d21726c4aa7b2bea39cdc276bb31ee019080d18d4715a237af7122a150bcab262112fba12f666828b093accc91f2cf97aaa9eb9" },
                { "br", "94c08a5f1950e587bfdb8586ed99dc61d9efd6ade54e282b40b2d8f06a45508d4482a230b1916a899c573e6f47443ad19d936ba5d9ceb381705ec4741d3039da" },
                { "bs", "1237343d6065b7e3179e2f598f1074ec65f57d51ac6804b2313b84cfb9f3674049aa7a4f5cee02150e0ffb83d744c01b6974adf07baf24473b302db70c9fd881" },
                { "ca", "e68fe94e14e15f2a14404d37aa4b83e2bc0a7fbb3aa786d4196a64536674db509e3a16090fb557fcc49dcbb3b6cfdebfdf20bf18a4195c10525e4b683a776fd1" },
                { "cak", "b3732269b969870e76939af51d060ee267d6813d766f840ad37d48bbc1618cfa35e1226fa3fdad9fc6d56dcb2ffc7f5fd8d212a59f964c5342842bed6e430f4d" },
                { "cs", "27ea7adee7abf2690583f39a87e28dd750cc1c5c40fe834539e1bbf85db02f9d226a9834120c7829c5a0a6e19870550431c88e54960e6abcfb73b201c4e4e087" },
                { "cy", "ebcc279c202e7c4d032374fdbc5cfffc4b385c4730a088b6bccbbe86ddfabfaf6b0bd7ad76377610ec8089908cb828ddd1e81a3bacc5885336f7688262d37c47" },
                { "da", "805dd43519c0b97d3a098e18147b53c090ea39fe97f97a3293e39bf363a28d0614ff987a89bee19839399291ec3389b855b4798212da942a30369fa4bd9433f5" },
                { "de", "98c65285a5186034d72272cb4f33321d6e6883fb635d4204f50851906241a0af724f433578e017f75f7bb4ab0e872fcec0c5487188650bd6c22d9461307db318" },
                { "dsb", "5ed54c97652372408268bb3147a7bcf8393bbc3fbf2a7a00a6864710c0079c97b408117c77b4a20f9fb0247b143c015feacbcedb3ad5a9e75a891db6281ed43f" },
                { "el", "3a6921c98425845d91aa50cd5c8a956f2b863f612d1f1f12c0ccd5c53b07877fcdb66149bd45789a09d310eb258e37f33a0702f79cfd681a28d907d39ff4b598" },
                { "en-CA", "44078d25ce2985c827765ca3477aa1ec0e951ca21eccb6c961b333c966951bd3fef7af2eed149672a114a256472a5dd61aeb6cefdde7a40156705cba976c4d52" },
                { "en-GB", "b4bf25fcf1cfc0a28ce3f1c39a5c1dd581f9b225b4d81403dfbaae3385a67d85c3e4e191db6005c3089de86a434280f40e5e527f59f2c0af6be0633ee6ee2a48" },
                { "en-US", "9fbca661da785b1f4e3ea58d4b433c0266ec1007f5e1ddf2b341e08ab66f1600b1bb3a14737a9fd165fa140a9fa1a44038a8c5ec63f7fe51eb4f641fa8597900" },
                { "eo", "06b2b27145dfa1c9b9adc10bf8cc1392cddc2f4f2a948c0f46da358d3ce07f78c46510603719b90140102dc44881275078ce82e934d985a3fc1c0b68057df051" },
                { "es-AR", "8beb759ff8ed56db53b30c42d1ac5856105b71fdfc50c661574bb9e124d9c475e005b3473408d61755ad0f95f288666d498d8014bd98c4bbeaf88f83c4aae576" },
                { "es-CL", "d9512820792cbaa07c7f65d5cc4ffede0ebbbaa8935a05be2cf058606a8498f544641fb68e642017f6b13860a57fee230eae67218ca99e39c2f3e7102a5cbffa" },
                { "es-ES", "36a503e987aa3d94a42ce9396e19aa8ee82a33c519cb074434e8e598c3446db5fe9238f481a5c9098bff5461ed2e11f4882115a00480dfb30f636386f9491ffe" },
                { "es-MX", "f6339a471246390240ebadbfa54a2aeab6973b711ad14a233f8a7779f4860c0bd11b4ce224999119a9e12b211ade7f99a2e7e03acdc007e4a30ba92a8819bebf" },
                { "et", "131feed1df92c8060458ebaad7e2bdb87d4b5ab2aef12f6741bff8fe237676431750005d16f32b582f9de958a32c7edfb4429a9312b598ec2727ab4a906aba73" },
                { "eu", "72c2e063c2e3b545b9367a75b337dd4c264354c6cda9a24b0346f2118da6db888a5e15162b373bb57679374b4db4a2f9b9c03c7c39053596fc8cccefdf6f3331" },
                { "fa", "946a02f2c2c741caa3b41f87b53b608a47ee0041088ea9ae7b8460f5d175b43ee05d265163839ea7d5d14a93540973b0b5a2b3949148e2de189aca4f6a3c941f" },
                { "ff", "fee92309b26642d1d8174c571695265bf448c46622fc5dbeb6b80af87978417d593172d638abd14f3301a43223c5f5d1a393e562a7f87f95348f02e6d4e453cb" },
                { "fi", "ca4e2db29e9478025a4faacf77bf61fc9ee4ccd96d688cfbdbff87bc8e9720ed0cf3ce5228c5dadcacca31b0b0a0b497fe3ae12424b45bf4cdb2b67bddf68c8e" },
                { "fr", "87a32c7008e853543e2e4918f70a458aa9af1800a81d429ff73ef98a86e71bd954c9430f452c6e8f10067d056b0bcf0de5fca4116bc6753e9a4c3b0497706e6a" },
                { "fy-NL", "aceab22abe171695b0e57ce52e05bc617f0147c239dba14e86737a98aba5959ed3b8dbb7a8b871231d2d002c5e0565cff0b15b36331b4bb95a66411d94b878ec" },
                { "ga-IE", "25cad55a5763889b0e462b030ec9140725b6e0b1fc051f15f9d1c4d3f59ca912dfa20bc59b7523dc71dc0d2a69f386aee69b5653c0a228fe48ff068801f0359b" },
                { "gd", "b1b5882bb091e31a5130124e18b1c4ea4f98be9ad552e695c98b658a4d9ac8fa4c4d8fc7d37c491dd736f8fd7c50502d228c824fb5dadde29d4632d2c6ead134" },
                { "gl", "6912a161a176e3e26666a1f32fee7274d3b0438e0a75f3feff9bbf944be84cabaefd1520fbbceb7a910a45aeab24692c3f7e523352f598026085c429aee0a50c" },
                { "gn", "964965d6d9eb8e3a87cd985c3ea8e234e66372c12f4794bbb7e878abc0e84c312352581afec38c7e983d18dd174d2d692df6a6a91eaa19ac93fd59eeb6eea509" },
                { "gu-IN", "f1b5a20bffc262b3d6608a9e3bb617c75e9be0e08791afa78c467a91618e5bba4edeada2be4ce9b62ccba2e054e3f9e486e9ae781728fbf42b0bea413692003a" },
                { "he", "5a02bd00085e6d80d5afc8a14303eba3313a07f282495f04b3c3bce094f0e18469e7481bc63de1cff7bb2ad039ff350e8ba1874d96ee2da737178d656577ac72" },
                { "hi-IN", "51d329320fab3406962eb6b274f2cf79ce918f9900bd08033387f47be23e612f923559251f6849cdc05a613573713e88e0bfe95f9c76145018eb778c5ee78821" },
                { "hr", "5053e8b3e2f57a9478ce9e9a55067cdb8aff60e36144b693ba511a1330d06a23812f17ad19ec1b17c78fd7ee9eaf3998795a58aedc2ddef39ecebb5968b474f9" },
                { "hsb", "95526c32c2c717722afbe26a663cb983f199e547b9c88facc400c5e369c29615540c20024ca13caf786f1459943e4f1145db729a3030203d7616abbf2fca485f" },
                { "hu", "e74ab3f14535bd9377cec4fdc0076e7ace2da445d1aea35c29af05d5bdad84dadb2f9ff6eb1663dc0c8006557c5a8c0a416b0ac5c02ee4b90f339867425211f8" },
                { "hy-AM", "52936f608cf737805bd324ca1dccdb37bca4b7d9e440855cf04ded11e27d7fd3a315441b5c9245893904509dab24b3c213d9226a1ac0163952dbdc5903570d5f" },
                { "ia", "a9e2bd26dcc5670a3265b39d90c034bb87ab2ef756ee1f66a1bef1db6eabc94438736b086df2a8d468bd90b731dfc2ba6e1fc6e5e2e0673601e8da816340f507" },
                { "id", "22afa5343aed6c608f4d0ac5d6f51d518194cbcf5ac9f6c0e9536b889e5544bac3735bcacfc1dcec046c142eea8ae5e3b5cee795fefb72e8f0adc94803bed1f0" },
                { "is", "f42b6689ec063282ac40556879256170fea8ab915daea9b7547ab5ef4df2e16d1dc2cd9fe0a5adef0798d2e1377b06b1f619ee3c1ca667684f4a96b98be21743" },
                { "it", "e160b72e779aaeb403dde97733b5d9e9e66246dbf7ea1d26121bdbddcb9b7fdbd02497b1f1f5107c7a426365da39c4a659005474188451df1f9a80505dd8343f" },
                { "ja", "470b5ad0101d78122f43b7bb81d5aeb75e6642ce06d5beb0d783dfca0308648dcc44ac2a302c3b96a12fee30bf4b40d41adb72e8ddd1f40b0ca8d7c685300b25" },
                { "ka", "79542e017afccad8ac341201f177bc7d6539ab2459dd5e122c88299a10e0ab6953853a1148c54ce7d5bb646d5037a77fb58f1c8a6ad1ba4b05df405d58e0e120" },
                { "kab", "169353b9d702eb951968a9e01825905f73a8e57dd230a1aca949dd3304dc800bc98f306f89c208e6832890ed6d9c46a42779d66eb0b89a90c52dc70efe66aeb1" },
                { "kk", "bfa928061b6e99a614db5b8d7b79e0cdb1cdbdc3a16b01bb032b343dfa8c49177d5b09ae2f1859346dcf1ac8a20b9985b73ded1f4886131945dc1026b5881baf" },
                { "km", "ffd9f2dbf7a6fc2cfc000f9a2905a8bfdbdba811b8356753bf7f93c62f03b796d89e44e052fc0df8f5606320eca5b94bef22a18c9af6df008d437efdba8e7816" },
                { "kn", "c7d82ba633c9d39e680885dc1ac4c092c414c748acf68b60be70f5756cfccd67c4e539870c191b0fd44ffa58d7fea196c7db90c50222a02ef9c56ba370729b04" },
                { "ko", "810c1173aa676102ead45babdb0dd93049ada3de728fa148410e2481e39010fb88b6ece1de12ff9af0294f974b5abf3b5e444e1aeda90189a603e83a51219807" },
                { "lij", "2f7901b17c3e5af5eb4edc8a0f2ca2ec721a9d39d0cf4abc29a4a8607e63aab265ab08cfd75e01284731132894670838503730d0375d9fa1036e32f1b46faee0" },
                { "lt", "6e4f6cf914b71cb17b5a16750233aa99a051c43fd3be41a340a0f20c030e27d154bd3a6a4c95a03230bfbf1e7d3759cac85a641fb9441c96efcad2013448cc12" },
                { "lv", "6bd62f908c0032c7f17f6671c64b485cfd35a265632ebd203ce3fa3b7d0c0a63bf555a16a465d4a18d24a60d43f3f753e6687d111eb78a9a79e545a63866bdae" },
                { "mk", "476e0e7bd3d5cff52fec06d939b1fc25e1507d30f5e8e5c5815c94b2b5056a385efb37a019ede11247d86f5c14ddaab15c38fe951ed3de78bcee17e1884a9250" },
                { "mr", "5dc55be6cacc5a67c856d95e359f0a931960924c322d3a43517ff23dd4585951a43e98aecc6bf963e333fdab000576e9ff17ba15a3077c9d6998aefb2dc93f15" },
                { "ms", "2f8660d5495408d2933a18c42a54c6791d63a49b63702123ab6f21aa0cbf4539003c8124827533b1dd739ec06891534c49706face9f12d61d7cc3f3a347725fc" },
                { "my", "2e01bcb219961a3b3c2f2ac4f86136eb273bf8b1d2c705e7f34f92bc35ba6d53d5a2478cab83eb3021a423713fbe85371fbeac651c8878084e0edb79cbdf4d82" },
                { "nb-NO", "b1f04a9404b8075f295aad1753ec4b21ac0ac88c9843a457d4cfef7412e617c58b6eeb0505817de087f9ae4b49b637e16422e3054bb1f4617873f0eda1f53b15" },
                { "ne-NP", "ede5b4a91f30691e74a2b0d42d3d9cf1b899efaf85f9b76e56fc1a716c3d1e69484df778362867638dc0c9896399f3994aa116784f56abe2a860396ecaa4a620" },
                { "nl", "ad9052782052319ccaf2a020e564b6d0e84177d6f4b9fab517bbb80dfbe759391f8e1c804803b8caa946437413b9f0bb63ff34e39abe8765995922d8ebf33f15" },
                { "nn-NO", "feb357efded19de04b2028f02abb812c55d7f7171b2ce509326a710fbfd90f2811fd21961e5cd1cd4c8670eeb7f02ded408d2db8bb6c2dc44971e97ea16f0ea2" },
                { "oc", "2f02eb4d4662f26099487b9a6fe314567ada0f34ebb1230eaf70c60e5b3bd5b1baca3134403c4e66b43ffe2cfa71a7d08face7005164bb450375f0fd6a13b646" },
                { "pa-IN", "57892e1eda80728e19e5bf48b0b624ac4d33b6575eb49605d653738cf6bdcc5127ec9a05e7c3b43e8783db0767e6e8f3af3ab9e277269bb5142b5ef2678eee16" },
                { "pl", "9b59e575f0ff189dc48fa844d160b62a0a0eaccce324770ddcea1b4d9e2a84c7177137a29af9969267a76c4214c2161cca9e64f1d92ce5406eb03402d7143612" },
                { "pt-BR", "e1a00bb5e695b73796d859a89cb2aed872275f7cc208511f5ef3d38258302c441e455202e3e08e5314d8b892ff3e5ec8fd4bd16d84a1ab3f40320b6d07f11f25" },
                { "pt-PT", "42e51b963f519faadd380df8cf250360129ebe462e51b07961ed19ffba160d69613101ec0689882f61ee8bbc52eb0c0d677f57c32428ca9a59583f20dbb88f5f" },
                { "rm", "a99153bbd8287d34a69f7e5bf52117a300407f99e02aef4b49210253eda9e273b150f2b873d68c3621583800e07730ec8f6905127ba80cba379ff2be90ceaf12" },
                { "ro", "1f2b542ffffacf538d7b8fbdc955b3c508c5bc7c43e3390be56bcbcfa571fc9d6b10b7ed7a60683b28502c628d1191db24844c6809326f42af2568ebcd2c6e0a" },
                { "ru", "a26a8ca412435e2f96122d654ada2ee7af95c1bfe317a96f76b3ca6424bccd4ada35a99661b6ac44dfc511e306e4d73470153d9055abd7826a5c878b225e7839" },
                { "si", "d9241240a8e53fe7ae07596b5b0cb1eb9cbae7e0c933fa7ba36478b039451817a13c29150875879b14e042d8ec9e0ba9482ff1703f41a2764e860a8fa9ef0fb2" },
                { "sk", "f10d75dcbc45d58337f2a641dd461222f1580ec50ce4f6cfaaf1261a6edae719c1106d43bbf25339174271aeaf059175057357eb649fac32b09560575954f7ee" },
                { "sl", "b6aaf3a503b93d86d88d63a46253a5284b9a5d4d905a12404d98af724292b0b4d39742ea75e57b55f6a5493c369e0f3b840e981aab09248f5b3d743b3cecd916" },
                { "son", "6ec22cd1f2227978d3a29df5e3819d7e7087a22a74f3064c4a1e906e7871a4a9cab15285856f74ba640452629db657435366777f20c4e53362b8a435ccb43a62" },
                { "sq", "96cc019abee0a07e8e69834cfa174c33d77f39114a27599d731b7382038fd79cf0371c7f3e8dd27f07624639a36de52dbf6b2c505b8921b45309f1624c65dcf6" },
                { "sr", "7584b9d6e10ff5402f515aa024dd476a072a5a9b0889dee77cb77b969cf80e39089aa3051619d2905e4718e6cad6c628782fe0c0d3563f062791bf469b5d2fb1" },
                { "sv-SE", "e53ad67f9aa721dd47cbbb87dcd28baacccee595e638003b8eb2515312e542e67512e604ae1e0b4c1cc586022830e980f5c35ed818ac8fd8591f0076a194f113" },
                { "szl", "1c2b66fe9fe538e3953c606c2f82fe7e0c9446dea6019c59059093ddaaa2c4008123a7900f442e9dfde142e3c3e845afe02af98637782b83664685155f67a4d2" },
                { "ta", "c6dbe6c1b4f6a8318bbf46f7b4aead60293b485653231a2d8f9ff914268978079e8e2cf95cbae2d34ec6f8e020c9c8332767069fe43d6a67cbd685cca568259d" },
                { "te", "db0ac1a2d41beb501bb82d8af15831a265cbc65fef5aff23a346746dadcd4f8070ca9c0ea85fe8503affe1bec0379ed2639f3732e3c964cb292586e1326567be" },
                { "th", "d1a2c8efa1a8ad0dfef3c9f333aa478ec3c3d28b63045c0d8cbcf861538a3967dbce25ab1fde4c7a5227de1abc7ae0d97613fda8868fa1051b141c78df1862ce" },
                { "tl", "0185d2e995333d0d0dfd6658139c7c48286efa49726346ccd3d0e4dc6d8d27ca0c308dfb84d2756f442b44d7b4c5fce22f95541d473e60ff784efb72b47afc82" },
                { "tr", "d0970d43de3fd03e2020fda98ebd705abda1cc21822a15942bdbaa419a2e8910a8b0a6dcf1216c253be818b539cf60dd7fa8868a88e9821a9bd3df9b19767bd0" },
                { "trs", "6e5f7fbdf6fd4121a31c5f3be5bf1d8fbe3c6cec03946d3ca5e987cb9a157c162a8ff7b553ddb4da834a1d45454fe93a0f9ecc15ca7dda1a4e861a8356c0b410" },
                { "uk", "9bc39c5232e39b1765c9d3939a8328e4222edcff28cb08b222b7a76c8175d3fc5bce1388873bd874ecf9deac93ffbe37750d320583b7935b8893d52b9d62e201" },
                { "ur", "b2e8da14d0140bb59d7afe63108fd1fbd2c66f4fa692c21ccc1c053d7f89b0cc5147640bb64f8b1b190451fc8caa990968911f0a77e4f10f42b9be3a74f0e30a" },
                { "uz", "3193c106ecc308487ac3a139ecb745457cbf3e25d6b332c537e330000861d88406991b3b4fadfe38460992628b2f8de43217b0e9a47ceee744b6f776794a55a6" },
                { "vi", "58e7d186d450260ebc59c470b7625909436325a06dcbbe8b01a81996c1809537b3740cf8edb9bf1242295a82b38b2d90a6c6a2d57d3e38fdcd51aeec9f81f223" },
                { "xh", "2defea6effcbd8c745e3dfc7b1e088047bc8c5ed4dd8441f3454afe333cf8c32c9f86801fed8b4e4459990b9f6d35c2254b36a1aec08075e574550710fa588a9" },
                { "zh-CN", "f3bc30a25b3c2878b1414b04f03782a9d84828f1b02f905fed66af0c307e398708d36118b9c2f0ac9747ccfbe8db456018468f18eb59f820df342d09b17d474b" },
                { "zh-TW", "55b85450f4eda2b09685ee3085f1a0d5ca646077bc41c502978de1e1bf9a39537d51eecab8432054fa148a90ebc08e21d1a72622653e368bbf630808554c40e6" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/90.0b11/SHA512SUMS
            return new Dictionary<string, string>(96)
            {
                { "ach", "c7d41a68d2262ec26ad2789f4852f619edbf255af31935f3eb88277cfe905a69d0ea4b4403169f944154d84aaee9e325b0c60824526fdd07e1c69e36d903a194" },
                { "af", "a096e19368022b38927fdce897a990e62000559b028e957e7c743609aa35835ea96ef2655ac65e5efba0dcf4067700f3dc690a210c3da96e8a1f3abce3fe8c29" },
                { "an", "04d6f13b817e05756998b98313848653b78d460f36642f157dd6539223397b1bc1d10c52751e46d47c5a48962debb1715faa6e1d148aa74a5165d06859323112" },
                { "ar", "cb22c47a5c19fe243ea844b3aa361ecf837e3f840673acf80916cb09ca3210ddc2e28221c2ba2df6da817ad78062c7c93cb0a368285e52be1999b8b8f0625253" },
                { "ast", "19462fc21f0b8ccf7dabbd2e79ed53a69f3a653a41c410b4550b5b187b4563801008ba321c36c5468d83f1b63ca34cefaa549c9545fd205c7e69242c9b9055ff" },
                { "az", "d7fba7e15307a6581fa9e41bc858a43fc82893bd194606070ca06495e72c172d5a0f7955978e853d1e4f7f44514f35a4cee965b63ee05fcb7c68e37eff022e31" },
                { "be", "e1b47dcf40c3a00e0a8adf4f199b594a4e37cce4756c767cba4579a2779459ef7d62b679c1e8de3d8b44d4861cf2114e99bbf91ef8c43c8335d43758f00f12a0" },
                { "bg", "c8fe810a0a90e866af3f2a99bcd9f5cc091727b903a5812963e19c8faeebfb8b1093ae3a9657e6a62181179643fefd8c108db0825a2b645f07cccd5ac404aa24" },
                { "bn", "9bdfb4f44e6ac42e5eb535eb68448796fe1b86ec64ded35e444974f3ce8c84ed1c5e96b8ea5fe6dc6a78020b8b7e3a06208dca0fe8507aef43bb2272bdf98036" },
                { "br", "aa5ae800acc1fdeaac951378e519cd38f48ec2c42e32a8d7fe9051e7da7e0dba66dc2cb108ac9439f5388266e7ea565c4dcfb268ec36266f4a58821fe6f62411" },
                { "bs", "bc4271e2a031c8c11b06789a289a42e3d1c524a66a77e8bf6151ce174668d1a153d6836d246b3787b3038645e98a139fe4ef09b6cc54c33a0995a6da19285888" },
                { "ca", "74d3fc993cb80aff1d3e116f9469a05e26b2d0f10abcf5671b2ae2dbfa72b70967b69f81c4c7e9ef48d956bd409095dce8f59cc1756b3f894f5a57e4323b373c" },
                { "cak", "9b58bdf438a350f098a173201d0032ca54aaac8550775f1ae5bffbc8774552f1e3eed4597126b6de413d47af515a4ebad9841263842c14b0cb3943728f947c67" },
                { "cs", "f5f065de76785e7c37467ec20f277514fae4abb58ed157cdc940e2540d6f842ffa836e312847c9bcfc1a6432e99cdfb063ac3639302ed6e675073431fbe8779d" },
                { "cy", "bcb91a05f71ba1edc0a372feca9c75dafee6729bf21079b2a91492aeafe10e8b28898d90d6532974d93b695f0978b2114aba371de85223bd959d1028e3327f6d" },
                { "da", "9eaa33d2da080435dc849d2b66e11fcae44c23b7fce20a941ecbd7a677c3067d961d86620b6c9c68ad0a6ac81004385654b9a982e2defcc14ed114fa56022312" },
                { "de", "284e81f0f97889bbfae22cb7061059c5de53c34f7b9e5d446ea0f05be6321d49047da6e248df248dc65b319c66fbd150e6b9f11f652c18b4c60c028fd11a6a2a" },
                { "dsb", "70c6680831756c8809aaf54a41cf9b4c9c8e2a3756df18f55c1f57926f9d4614277754069126b6c6d3b61628176af4e0f47a7728e874469b677e2a4ff43939c6" },
                { "el", "1474000d2054b0eecce55f4460d5a7be11e3596720e81d69ec870b8812161bd75357f0a614a2c940366fabd5f588eae175f3a5be1f78869f9b267194aa207a47" },
                { "en-CA", "f354e73adf4f1bb4fd0b722e9c87bcdae182a13ff27a3399b7dec58ae3499857fe7a89e9eeaed26159b3a3bc7446825efb2c5fed15119bbd819dbecc84f65ba6" },
                { "en-GB", "fbc2744e38bff33389647a15c0539b7e0f24fc72519854c1fa2eab12cfb9eddd660f8323cd837508db91427afa36e1edf3113a64641c5a59979284f0c8e35e21" },
                { "en-US", "c0868b09cf7b216fed9901ed8b0f094ac17de4e496abb6d1cbb610c578fb5e6d773bd76e46ef7202ff0ed62ce1f12e1d80f93c492dc9fb951dc7e5656d52858b" },
                { "eo", "f3a560c41a6ee7a998e74810f13a9972edb41ede0e02c7a648592041f040414246c88ac5ec04562d392230188a620124e931376de81b315b43ae95d54d955b9b" },
                { "es-AR", "d9f680ab5d0b4340817e454d8a8f26bc49328db7284d3105f0c1bd38151125f4b537981c63acb41b0c6a85c31ecd8754a10fe020d1a29dd79e87af1b21addde0" },
                { "es-CL", "bb13f5f324cf5187cdcbdf727016228ee0b560ea650e427d76b50e0b3648f6b4e702bc0f4c09aa8ef8d2ed302d316a285afadfd0b1f11798c943ef3fcfaf0789" },
                { "es-ES", "661c4d19a22b582f9832a19d198996ff984363a7e3af6628aa7902290dc1f93b6acffdfc082dba5f4ea9a776662ad28757e016dfdb183d793af037192e7bdf31" },
                { "es-MX", "c21d3929191425d0abb6b65e65b7e2de08f5e53c7cc2210d9d07ad156820ff0c42dc644438e8ee6261b85c71e145c8d6132ab007fb88335816fe65f6af0d7501" },
                { "et", "95b2f6b2b0795a18a186452e17b1b4eff645ea085a49e8514b38241c4ad944a1b88745ebc2931dc19e2fbe42a8e00b0945d3300ccb9a3bf00d6a789563d24edb" },
                { "eu", "1aa78f462eaddb455f91d1c4281a5d24ece0a4d18e4b674f8c335137c81cfd58e97861bc5b18cc7ba7dac067a49d6e702c17de5c4c9b76d500be85fd2701df89" },
                { "fa", "43b4134037d70c896ab25490cb14e8f8cd9a32c639d74a8da1d6bd6969b38c4d79e3e3806f33d679bedbd5941293f77daa30a7b82e452db700379a5968fa08cb" },
                { "ff", "5c1bc7cd5ebc323087e669120d80e113fbb96ca0e99f3400c96a1f4e8e006e648b68922fd820c09b1f4916811573de1bffbe084099a3f15d99db840a0d14404f" },
                { "fi", "f1f872129c920cba785da9ff20e5e594a72decc55868d91be4822c56a56f888d45c462fcd33aadc275f0a611637292c978c2040b7c789b73ea52f39af9249691" },
                { "fr", "25b304b9ea9bf01c84f1dfc7a0e2a71ad5227f2c0a6c4b80f6593b6f40b2935fa2b5d278fae98cfe2041a838ccb28ea372363d9d93dcfab3faf01e21864d178b" },
                { "fy-NL", "f83b344137285c84157a8b59e4d8c36edd8f756d606bb933eb628cb6ba224ee18daf8110704823ca8f741191237827394fd5c67b7cd009752bb3c687f3389ad7" },
                { "ga-IE", "85873c916b10bd4aa03273aee2dda640a6b42783e19428b433bf4764aae7a6d541c6c5b067b47efb5aa57c8cc004f60b92e2c3c54c8cc25af89443b35077e669" },
                { "gd", "f7d079b3e3287875d151538e6cf79f193b80e2d0d65b4d4032dd462ca82f24abfaccbf2ed656dcbaa6a7942fe0baea2d8f8c4a226f46e203177cf4d3628cbe71" },
                { "gl", "5760e6f9669c16d8ee7c8abe4c89cbd50c805e73dc6554d9cffcf2eb9ebf0e7931612ed2b37ed5f298993eaa70b49764c5cbd1f8a065bee523eebd4cf91247ab" },
                { "gn", "19db7eeb227b4b37bb90c1d2d35e60a6fb47657c14e1e0eef9414ccf8e530c798693ff1ce03c1cd29ac9e595b0225499dc016911036555ce2215d9e434e809d5" },
                { "gu-IN", "525a7e37ebef49fa3a1389903df16d8d72eb134f91ba793849fafb7980509934ce58351b1dff02dddf12c2f96915dce850a604ab5479b8cb61c28508f4e6ed15" },
                { "he", "429c829c6ef5282be861423a2c9e8687c282dbabf912311aa26f1d6bad96c41c8d2739d7612969718200e83e0e7acf62a6744cff05b9a090f7a3ae20c8ed91f8" },
                { "hi-IN", "6b6d9bca2affcae0a7823a7bebb67f5ece03f7a323cee07848f85cccfcce25575c5df26028c90e7f9ff2bbb68bcac41aa1476b942b40ad52b7a4b99a67de1373" },
                { "hr", "286bb53e71e92dda04b1a06f954ffa265f880edf00d4905023f9933593ae0191abfed503d2986f975d09f40a8e4ec39f1987bda14a73e661c9f65e80b2c06007" },
                { "hsb", "f8fd03c9a42805123e25d4cea158862debe257d23188647840b713c800b7acf7f44e76c1cd77ee557f8e0e42f850a8a9cfb55a91eca694c8a4d3dc15369dedf4" },
                { "hu", "3533aa8c47aca7e2c6f18643e28fa67f906d86dd8cabe37df3875f1f4251cce8656aefefa8abb8a669b8be3848036d33190ceaa101a0de809c70c28725947f9a" },
                { "hy-AM", "b876615cb99ebc1f8444539cf919a9eda9f50522274d8411e36a2a179c8c47e09754157acd9d99b93437bc8ec4fe9ccfa56cabb4524a492a1c8aa462f7c3c0af" },
                { "ia", "51d49f15564e737bf0f29cf1084dd46bacdcfd12f4c6382d39d36350da28589b10e72b8416ab9033e436f6b903d546cb85e7834a7c37f4b0dee0b7c3546e031d" },
                { "id", "7824e6306ab58f83b3dc76de06584ef730ed197500f9b7cfb578b00609c79972ee5ec8906d60fc064afabd4d88a94670e8aa7dd4b387230454281ab14004816f" },
                { "is", "6fb1dd04fe045167d174dbc325ca0abe63367f96677a7d44f8f33d5c712cce2f54f60503b369834e83058493fc88d20a4d6160dafa48a7f6cb5df4e9d78ed374" },
                { "it", "5d52685f3c32193c5770bff95b8fd8fab01def337e62b58692fd750584ee6c4a4d21043e2f9f62057adff2082e4166415dc07a40736b46c01ba4bd183035ab7c" },
                { "ja", "85f4754290ca7a2c4dcd25454da0e4b3b83cfcc02e69045e43b1fe10b31b06add4eea4061e151e9f39ae840c077d13add86e9d9feb06bc881f4cab363ca87614" },
                { "ka", "6bf409d2530572a0e630b8dd3ce005ce692f3cb2730afa6db159dab126c5e53598ec77dafcbfdd04ddab4a00ef489b87c0de7416dd4293afe7ee0966725d91c2" },
                { "kab", "0f207d64fcc057c1bf9c090690dae83877eea605a792b1e3f08aa443f7156fcada32965556695bd6df580b3b15aa81532724405711cee9cfb00f25313a677d9d" },
                { "kk", "48bbe8581de3e15c41127db81133b227f8570961aecd331a66f24b50d7bd6d68b693be0d09bc6012b2898736ceb4569997f5f724d05433165f07df6545aed3ef" },
                { "km", "4bcdd60926fc3ff6880c250e45221a9d66e7507f8996cf40983b28d83d202295e5d2cf5d2add6930bcdd1167881d291971186e6a754daacd7d44de35e9db543a" },
                { "kn", "dd8c050fdc969d45f50300a40d25efd3831bc62af8c4b730714fae941cc0d7526a847548d1340a5401d78110f9b2484397f11834d3b810ae4f4469ad78eb150e" },
                { "ko", "fa4a0f2d4b9bb5c0231ef557a7d97cc71d35a3d32d4cfba7d2fd6a57010c2b4506cd1cbd4b9afa3746d99bded0261bad0d2e2b29ff5282a648749cd7937c94c7" },
                { "lij", "ee0f6a47f9517347ba31288dbd5ed8e06669caad8c9c1264679ac467741e7e6d75bc69bc7720b954669e9203d07c799a5547aa96943fd70582703a52940d3893" },
                { "lt", "e76d6262e6fe13c95687b04525ce6b40d3bedc9678a256dbfb7a8cf7f5fae9c6fdb6f33174f2b1e8f5e5d3663934932f015a4744628bd7dad63a6847c1459ea5" },
                { "lv", "a3459446b8db86f734b2d5393b18d8637a575c83d9cdff55c1f71255ce068ac856b6973d662e332d53dc1dd7aa77d50e9bf6c88151af3ace4d32455fbeade63f" },
                { "mk", "d9cf2f700925c9f368f373d1c2e6656b407e389b1c499fea123a8821d2a7557d6b7877429db56919f128f38fe908bf90d0f4e39610be4e1aac9ff7629426a2a5" },
                { "mr", "01fdb5513c99c1f8cf4e6838591397f079f72071515c8d512a745c4f06ca60461aee1fd9793c7eb7cfdac4028772b2ff51701edec0c0c956dcde6f6f18344c73" },
                { "ms", "0d5a8f38cc52cda8db32551c00a53e8804cf8f2bfa505b1644882dacaadef7c94656becd0259668a6e8e297be1a1e3d3d31481687bc92474c095b3b31579f7a7" },
                { "my", "8ed175c7e0b928f43024448ed85fa6ccfe7f8f411522cc50f03216619c5ba0cd00f50de4c8fd2f4cddef5f7a04ab6418100b8cbe51d98b352696d1fadda86d96" },
                { "nb-NO", "6653c1a04fd59851860bdf4de7d78234a01c7a8d95b205c8b9f21eff621dd74c0d35fbca9d20f56b5751186dd474027df4ca701f705af533207cf0c5390561bb" },
                { "ne-NP", "2e24155f5e86833d77ef4e22d90a55b6e2c65b773046eae2b6cdf0ae2ff5693f97bfa21e4e910d4ceb4f8165c80008f76a43ccf960f19c730f1dbaf9892926f5" },
                { "nl", "a94d0e7b4776cf7d5644e2db73e7e1088c66e7a2a2b88df41cbd9fee02458003a6cf9134accc6ec71ee00b2131293e6d0d3dcdfade23d2a9848cdb5336fdb9d5" },
                { "nn-NO", "35ba5da540ccb883c36a5cd442f580fc373f52d9b5f5125c7339d3799991697455a79fb8dc79d1df706e3948f7754e8cf6eba9e9adde465f70b0dd7ac3bc1b70" },
                { "oc", "96c40b1608c529759370e12b5af8d17a49309b74ecd2a349cbd0ac13a5f8c5ac3ec8b0e96c64c065abf388e638a051794042b3dc11fff4070509e2e7f7b6ffa3" },
                { "pa-IN", "ef660d4d5dc7f5df0b9e63acb438ff829889c365cfff0457a278f07e72fcd6f3144237ea2c12d3ba14a413173ad26a7286a223bf600cf4e0d0a34b350f474778" },
                { "pl", "b226ba190fc58091f4d840d4c60c911aea0d246358a9bdbd19cc30fdb141afcd0cf1e848f5b4455f5f0baa92c54a6dcebec5c9b19409d2006542725732c2a4e9" },
                { "pt-BR", "fb5e3c5cb00a091b2ff08d75e86bc96185205be27f9edddf79471f2e2c33a8a45e37f17b1694c2394d561cefb99e2dcf0718883737aa5893f356ab6fd7b01223" },
                { "pt-PT", "ef8c76c85eac21e90d256b9393ea7e2ef84dea2e00b0ebfbb203d3f4666c96f070375054a4e3e11665f64eeeaecf3038c412ce740456f87fed0f18ed016c215a" },
                { "rm", "038644cf6e44e51a0cb5f5ffc3b242b5ffeec5964446d18cad521f73b43fe9dac9db8d7de1d3b6f607911bc5dd507d88bb87aef181a4232a765eb56fcb478274" },
                { "ro", "d73d5c244c746274af88b7a99648cd69eb63eb4c3bee216ca5d34e8772ee8365282abe22addb007b9728e8fe88e4d8d9ad3b0c518acc0d5a69a91f0d7281c71b" },
                { "ru", "49b167b383a685f24bf0db96ffba12d22b93be0ab0cb252128c0561839f0470e8c32b633f4a23f13b14aed5acd4e6d4bbf617aa8f5057e77e4b603d0ae1945a0" },
                { "si", "c754a366b9875b385508eb57985187acc92a7794a3e27f121e1d7fbd0bcf2ec49559f9ae8ddbbf93e53a170e1775f2047ad562208789346fe1367e255527497c" },
                { "sk", "c15a0ca64a41183e7a9a0a66c674c91a845092337680367fbf29e4eb7f0cb2fb58463549b0244b885d3ef7df40f0d22d6c7c0fbf68065a4e3ac2963796a86d80" },
                { "sl", "0d4bdc1e8a843e082fdff816d058424962f8b63ac8ba95901063ad58e0e9f35104fa6d58261a32c96c0d390bc30828b5bbc13c349c830e0cf0db5e1ec400b6cd" },
                { "son", "3863ef9db0b88030cd84f43c1295f3f50192b989657e95ea29fac0211f82217fb0bfbab6acaf0ad6288c4d8393a8a3d9df7c11914d64a4de97f39e2f6a8b89e9" },
                { "sq", "311c76e5945d86b650b3c51d215ffd8c1c968d64e42151e3a60bb71a5a8530ff6fda2df768fbb25ff9d2e25af7252e092bbf500f8c9ad62d8b6491fb35adde79" },
                { "sr", "b2daad562917575b2a8ce6e1a87de309f6c3bfc2019a8f550ce75f6df03dbddd7b54951ef720f03ea6035c1096204602c3f318dad65666c02309c3e07215bafe" },
                { "sv-SE", "5519e3a108b068039b86f7d975fea0ae43c234360e657f133eb02f2be50e7a5ff954119765002e390bb504c4cbd9e1cf875de9e991d2a2f3f506094e8c2eebe6" },
                { "szl", "eb38a2382a3730caa108f1726ea0d861a8c47c15b82d8dd6a42755bc334e675169ef2682417ed50e474c2e2bd96699e5e7f370db00fbd9ca03f430c81af660ae" },
                { "ta", "a3de712960501f2e84ba69f83be55308c01e3837fb8861c2a295522224030db66fa98029900b30a12669b2856e5d9cb4c1ee0f7e0258a2a473aec78f8bc4a63e" },
                { "te", "795712b02bf120137a5f36da861dd3989919450a6f2c1518130fa352dd264584329f188fd339bf78a91472fb5bcab3f4f1a75c4f24b286f4936d9d1696c49166" },
                { "th", "e7df34470a7af13d496be61fc8d54b038aa0963f3147ae9b2423a40a65cedc947557d4f8423818bf5266e9f4e3add55d2bc8c5326b10447ae2abfd60af4fafba" },
                { "tl", "57aa2fc55daea00f9dd417b5c5652a6b807bc212f577a632d6eee55a9889566379a0ac144d6fc151aa26993841d323f43763f3152053f2110f545021acd9c286" },
                { "tr", "42a915fd3a9202fe1ea525946ecded304438231da2ed44ac454da4408c8ca58dea35ddeb9a1baf39b82183672035d68a809f9d3897aa74d68ce1cf734e864aa1" },
                { "trs", "3d6a7e57f0360777b0410133938c8d729fd9ebf7a4442276a9464c857407502eeb1e79ea157436b0dbaf3c7e629fff3db7a1320bf2f5b644191fa28e284f7e4d" },
                { "uk", "5330878884d58c98070ab57cf889eea737dc4cc0190a60428f990d323fa3f4ad49f84897f89e68f9a7a42fae272748eb820f82a76b04cb3366114f6ada955cb7" },
                { "ur", "9ac94db323c0447ca11889a7d01be441cd0a1281d02bbbcbb8974c002a467d83f8331d139544287155af369b6f62870c073d8b61e11b9ce459063a1bc4fe5e8f" },
                { "uz", "e6d200796ac8d189597864a5c0f7cd673c1de111a42c8daf4deefaf40b54dd37dce39eb15ecfc16bc81f9186463a274b10e7df268213512cffd5cc584abe9296" },
                { "vi", "5ae27bccdb2f84dca2e0f850db1a26af4ccbd88d17cf7cffc5f126dce73fb685beb22bfe396a8f28dd9f3edadcd452d1c551e5879a35f6c71f170f14a54686a7" },
                { "xh", "7323dcf5dc4eb43ef601713eaeaecb66d32fd8176c3bda333b33bc976227f66afd1dc837ca45cbf3254ec4088b17b5e14f023ffb0310fabf9ee79f93718bb0c6" },
                { "zh-CN", "45f028ef3c900d6fb69c693c5fa66acd081e3875d96add1a343ad130f7b73022517879eb806b90237e8a22df29b038ec79dbec8da65ca76c3bc1df8890abe1e8" },
                { "zh-TW", "82400a86e3408ea55a54d7603f5849cdc1e31e69d7ea127b92b7be9153ca46a0d69e73edd7d6e31b23a070b7fbaf806ec6839b1f8484a4b31f08509f4e648488" }
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
                "^Firefox Developer Edition [0-9]{2}\\.[0-9]([a-z][0-9])? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Firefox Developer Edition [0-9]{2}\\.[0-9]([a-z][0-9])? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
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
