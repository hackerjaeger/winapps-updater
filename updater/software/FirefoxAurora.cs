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
        private const string currentVersion = "118.0b3";

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
            // https://ftp.mozilla.org/pub/devedition/releases/118.0b3/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "0e76067f22111343f5f16d9342556ff0179e930c50e635101172da02ac6d14c93459a2171d53a1c95ac2b4cdf1a5e071bebf1da54f77c8896985e9d92ec1a25b" },
                { "af", "3ae25a465e6513cdd784a1ea7b4debef7c92a0fdb7aeae43a930fe72724f54106f4e98c81590f23fd66e1a4dc5c12b470ba2485eac6129ba9be4a98379375430" },
                { "an", "d8f94c90e0bf617104676eaa958e769a51783f1007f19c305a1ad145364c23f3017a1eb2b73d9307bb1c6fc366ca33d15679835a5ce5225c2a7ea96a758bf71b" },
                { "ar", "f08a1bdb38ebfcece6f14818fd96b156a096d6923521f019e991566fa21d8cacae41a97cb12e5cc20505bb5ea54671d0e096406c6f5ff51efe68ce25a798094a" },
                { "ast", "77c9ca15299040a08c2345e0b0eeaab94313e617f5f2195ab89abd696236086c1586b6126fe4a853a2a7c68e1df49fc7f3719ec2e76623ee9175bed7f21c4b22" },
                { "az", "a99d56f91228cc3ddd816884fbda5ef09ba147e307281c5c52cc6138f22a26fe3644a05abd3ab7b4290771d9f5e7b97d1934ad59340c14916ba127012239b3b4" },
                { "be", "ccc9bb2ecc7898f1ec57c1bdccff37f68681bc248dec17d1727205fbb894d7b2aa65f3335d8b0f03c321d2c63fbff796534e75174a35aeabef9985b829a994bb" },
                { "bg", "a5d38598e58719de6c505309b8fc68bc3b9b6eb56b3ca33cf412c604e5ad07af5240d9c8074fa07fc9ee3f461762f95a598e77241a01e9fc162cce23e3ec379d" },
                { "bn", "6795d21a8db6de5c6e4dd0137f9cf270cf6cbaba927a2a9368a5b046109807956352b680573a1e48d3a30204ad94d66d8158dff272d42ebfa8a29fa0442f5942" },
                { "br", "a26623201141feda29b75b7667c570a9319404cb3cefa25fae9d30de3a642466a3377edfc3d465a3493c0153c55e65c044c57e49d054ca352a853aa4a5b2b00e" },
                { "bs", "2d8ad6f8abe15e0f6f50c25f94e2787588ed51fec592915b9f6b1f229943be256d919b290e6c39ec9a948a253919141b69c666efc0148b737dde6bb357443d53" },
                { "ca", "b600cb47437a71751faa20f062afae63c60e8eb48a2ad35a5ca7656c30f3cddaee987a9c0e22bb15112ab26cc9b55c93fe2647fba71a5397fef904cf03c11c3d" },
                { "cak", "26eafbd7584d58efd4490ddaaa244f3fa7ad371aa0c555408d59b720852e849c5b46e7b3edeb9fd3716330c88477a06a89a78b972560b1e009edb3a18c188d19" },
                { "cs", "047c57ff11ec8cb98d22bc75bb4367479349d675ffb5173f5554ef1babe5fcc55107a12cdb002e002fbf3b9a3113af499f5d008d5c6850c1c84525e9c9c2a522" },
                { "cy", "b64b6147ece65899b8ff10d186363f539e742a951a2149e95837bb90fd67d40560e278a9341657c9b8949a3fed702c6fc32a7c84515fe613c6e11ac5af3138f0" },
                { "da", "d371371a70ea1cefe5100d570f58686c6f29f699bf4d542bf09c05c34045152f506123d8a87dff45483689cbd7dce9483e4b47b4fb30f8c52ede6c939e6bf974" },
                { "de", "7b2b4373d18f219d313cac997205a12a18a15987dd1247c3a044e752802710891aacb659e7c459116e58c8dfc5a761fd6048aadd2a1db73773b6e0866b3e2144" },
                { "dsb", "6a0fb1bf4567ca36529ca053ef33155b5ca1e9ac4596b23bee82a33bfb5aea87b6ff14498f14b3b737857a182b9c9f3ba96efd85590cd4ec4a234e29624fc3bb" },
                { "el", "a53a33fd9f9ec17ad944bfc428c0d47fb498669818538b48d910cb91f4b95d12a8461ceb6dd5fcdec9dee6e51c348bf78fd68781b014e950a4604895a0599236" },
                { "en-CA", "c8b152a79f013d2e2c7f943b5240c927a35444337119982ed96baf49e4ec2170f8875e2182f3ebec696118cd010afdec7fab31b26b89ec4d5d22589c4337be10" },
                { "en-GB", "5410845b1d67dd6dc9e1357f68d76521858261a05d16d3d1fdb517fb6cfe50c18e5c4a9146ddd046948b6d55be37a6524abca1baf85e3daa31642f59206a90a3" },
                { "en-US", "a3c5bfdf9480ecfca002e672a6b1241e9ea33bf009f8dfb7b6b45616a20fca659d5a5dfc6cc630767968bcd965a39f248ce6f5e265d5fa4521af1fa5bea1d85e" },
                { "eo", "132b0fe4a5636f1741f2142f23aad194a1c1c33ed4ce9dfb64c60386a9ff5b5963f6db186d65655876dc35db30ad6c292558aa9f2a61d6ee8e1d711eda97a969" },
                { "es-AR", "c326c5d4475af13a66aa0ea1de720ed36d6e8479398a0c4a41349692c95b71c46d7dda53d32f4f73c1f9468eb0bc76daabbebb98851c1c295a978761092c1e11" },
                { "es-CL", "8f8acd2a439ca7837803f830a89bedd2e2eb70e3761d9d7231a92f24a20d666d3c5829459522f154d474bad22c1418ea48b72412f33f88f4acf05246a2d9aede" },
                { "es-ES", "c0f4622cba06cf7c05bffa79b0f7b18f3043a876bad7f9f6b0b318569cc130c591741fd2e9da588d0657ff09d01c03d097a37f9d366b143b9e914f877e62dec0" },
                { "es-MX", "d262b7b40de0a16891fb6422845311e189bc97737aa80719f667317c5a02ba906e91abfc2d6f7b67c1eda6354be2a24e9f1c3e23b3acf1908184516360c23bd8" },
                { "et", "96006629c63189e9f8154f0453b502db0ad0087b9e7d6e079da905496f7c42510bdd3da57769d98a52d9bbd3e906ec8711aa4396a19250db25ea881083dcdf72" },
                { "eu", "e6fdf14b9406ae15466d8cf77f2619b191fec71edca7575a4a94379b191549ff61fa62231956bf9a142036c171b24eed501e0c031e90decdbf160effc2980409" },
                { "fa", "219915ce50b2fe94aa7da8018e43620c4a251019a7ec05b56b475e4aac2f3649dd46bc3bc86468341a0501864faa6e5e020ffafefa668508208a32d83edf56f3" },
                { "ff", "33e7008af14aa298f39b62f9ee0881eeac9c6f8970cc10280fb37c865e7db89307e0c52d9583461713afe6ce88824dad038534c7de6e58d63292df4d1167c936" },
                { "fi", "fe5da13f98c22915b728e5df37666bd5e8ebddeaa93af834ba887a313eacc8133c9dcff1d7e1b1da079d65c8d9ed2f1745302eb3bc55a6230dcc66b196ca48b2" },
                { "fr", "fad89828010051e4768643e80feb47f6c2823c68ce3466ab1e08386c344df9473a999079f5c9fac000abc16ef1c419bc5a131589d8b80693c792cf1640dbd2a9" },
                { "fur", "b3c8c914bda15a71d19aa6c7e29be98c00c8693f29417b52343c72ade881073c97e8ba121de8a272858c475d457beb61d00ebff652c0eebf4a0fe55b012a3130" },
                { "fy-NL", "cdfc04b13d5be9e7370d466063630abab0321ae77e65a69b098e49f37961fd3ead97af921a2d3f4981275234b7c7c86e861efb97b5fbe9df9fab4bb541d98011" },
                { "ga-IE", "a7cb7cecd66ff94172db0a50e1e35d58336e9cc18b2b12f7c7210126c326c839444306e3cf4c91ac1619af21d93a95d042ec4ef5b0149c908c61ec882bb9e865" },
                { "gd", "4cc4945b795e7b44bfe89c1c1edca57463fcc1eb80fe301e03fdd0664ab2c786e597fac72f528f9b93ce313cd84c958e3336b3867b9828e700adfa3c0ef8cb09" },
                { "gl", "bb341abd274a13d7ecc79235e5dec841c01b9acb4dfa0e11094d5b87d8f06a02d165fb129d284dc673003787e1a2f77be7740ef7f03ff9c506f1056eb192a4ad" },
                { "gn", "29b80b947f29fde01f89ab00bc6a3b2c5b2dc84dfa5d9e6bc66781ea302eec6cec03c8139951ebab17910b387019987f93a2d470005789c9bd11143ab0699f66" },
                { "gu-IN", "99feaedf277ae5b37065ed95a7471bbd7c337af05054a01a46921007bb0d1341621b8d1d1ee9b00ddde0b538b4afc8ddcd8088f9c8cff4520f3e5a11c1f2e460" },
                { "he", "e392c9458d4d7bdca24f32e356267fdb63280b54be2e5de2fa42d222a1e0bac5d529e5ed34358aa25ad0038c54d615382f85e3b8eddfa5b63b41b946af9e92f6" },
                { "hi-IN", "0c7bcecfc58d3429a1f11bb78fea305dca01905ef7679424637272daefbc389aab81ff109604d27f722b9019b707888c45072c23d229de238ef7d4fbf245df6d" },
                { "hr", "d10a406cb2b139c94176749e240d79286a5b5a8b190f0555fcbb25f2c3fdbafbf2946a45a046656a9ee159ee50df18ea40aad6dc9cb99e246caa53e89fd95978" },
                { "hsb", "b6ff0a2c091244553b529bea1249acdcdde3db4335526f506cafed24f015b6b2d1785786ca89379a1737c63d2b6ecb5ce09dadbe0e9badd61a9a801c1197faca" },
                { "hu", "845ad68f3b1408539ec105e91a49ffde7aa04749f5f067fa6c294d932ed619446bdd998a556b82d00490e9066756dee8aa37fd6802c5833dd6e53751ce164e03" },
                { "hy-AM", "241bd1562cbd062564d79ac8e1ab1906a5371ee28e1028d99e838a380a673d7185b058032d03273f7f77f3460f6f724663667eed17470c94514778e5c355414a" },
                { "ia", "707c3427b21b37f7686436a6fab3c6a5d60233b858d0bb2f9d7888d59a53f32398dda790d8f736b08068af8e2360b9b7b3fcfb396e60c2689f344555a6fe3f39" },
                { "id", "ed7347face7e896355b99a74271eaa05d11be6293ea7d83811aef960a8924acc778de6e86b57d6a8a9f0f8ed248f02e87a87b059e9f210559bfbf51f637d3c10" },
                { "is", "753f838159c0c7b6a814ab5192136ce11dfddb850f19b1a0c52c39d906c3fbed8b1acd23063825b61add57aa0421e9c0320cb06cd65181e7dce7de6b9b048316" },
                { "it", "718ce11859c1a6820e24b3fa1eecfc41c1b0f2fe57b45932ae528596ebb93c1c3d9c2d4317cd790011339b21cc94a33ece6f3d8c43588a9c5bc1f8bd9a9a5728" },
                { "ja", "4fd7fd7e769dc84cd9a912afbc0651088ddc786c63898ddbaec9450f7cecd79cd100c9bbb3b3b6e1be056f2ca736cffdcc5001f523483f561307e3bac449699a" },
                { "ka", "77e673b1a4af5b8b58d7f3855b9b5c6f3f1a9cacdb6f97873211020dd111792d898ff17e00a2b4fd5cf14756258102214adba28f05593ebf7951ec4d60c0fba5" },
                { "kab", "e69fc2d4a7b28299ebaa079502ed73b5f394ad02a18adc0d001b571c714a30f70a05cf8188aa51cdfe5f39bdc61ab24b06aa0b5b34023ddec09264a9217bebd0" },
                { "kk", "365785bae21ca320de53dff33415bd3f731d79dc79cccae75ee50d446a20098279e9ab5355c73826c9de86e8603db980f290e6888123c0632645619a01c2cc04" },
                { "km", "9ced7c0bb676a115c3a7fe3bec59c1919f3d2406682d850f1d2a6e4fa4381eeb9c483f4c16b6190ebcaa1bda048b9137fad27cc01b3e237b073a8b00a6c497f7" },
                { "kn", "ca80bddb0c9f90ec50c9f683b09d1e714ce35f0d2c010b3ad57745174e864d5da876963a53bd9e8d511cc01ca778245e182ff3ea3fa57fbe51bbc5b5827809a0" },
                { "ko", "ec587d7c5eb337a04d2819c2120c85fa1cc17c70dbbe5b5dd8bc5f5ad54f33a693e3b9d0fe72f1af4f16dfe7482b109287a74ee65da72c78a08c15d99015c358" },
                { "lij", "cc46143b8a041268f014336ece1517d3c973f439220aeb1150f729fbbfa8d71118f175ac37196b7d1f70e751e7c5113ed51446b1c2fb28e0dedb0b0633bb1219" },
                { "lt", "4c511575018175675adfe8bb80b56602d2c52a99a7304b00777647e1c716c4c75f7955daa0636543ebe654038fddb2a076caff5c5f6d34e8b7d104769843349f" },
                { "lv", "e8352d4c2d5a7ffee6c5a110bf0bfdfb95abd17dca2b1dfdda51bdb0c025e5d2317e61c1009ccfe1bc5c03ae0013ace72e772f8e22bf407a6747e493f2415a40" },
                { "mk", "64efbeb520610ed2edcf745f3d1b8a46f422d89c9a32e521131f52f45e91019cb78bebc5dba8eb6d813c6c214a13f75bd0827cab0cb87a75d3bc97e53c4c65ea" },
                { "mr", "fc02132dfd53134dfe13d5149e0bde051aeee6373a7574e793558b32884a4e6d4690bbeb6d842d79cf6af887b8feaf6ae50fbe1b06cccf8fcb7688fd245912eb" },
                { "ms", "c9872722f9a36267c2db7f4b31d40544b9396d2ddfcd6a7ad6a85bfd22c97363185ea2276bb130e233d1dfec2b9ae54d118b4d0f3dd455e034bd9c3009777f09" },
                { "my", "5af90b74f4474454cb01dd400fa488e91ad98ec796a6b2fb5cd22feae8cc1dd2d0b7b0e120bf2556e4fab095471158d4d0f4d0df13ee675fd2024cf587d8a958" },
                { "nb-NO", "56f294c99b99080a03ea3ae84cb97056b04d09bd0da74363ac209b330f03b6e4e491739677bbc73862194c229c9edbf19feb221b1c0a400040810cd4dddc77d7" },
                { "ne-NP", "bb0f807f60cbb5a79fcc6f0baec3a2906ee3c8863018cae54ae2fedb371ef2e247e6087737c8a3a2793c50bc02806fd9f5220dd22e2831492138909965d4140f" },
                { "nl", "bf4cf1e8e4bc67e9b94ab19f246f468fc6cb69c8cda164a07856c367c53795db9876f835c4b04655a09604ef42d01e5a50a62feebad55e7601be186c4027485f" },
                { "nn-NO", "b73ef0cef1849eb8b20732323f37b8eaa20f12d2e342645e5d6029a5852bd0e055e33528dc26737aeaf060d51be4d97e041258ed00912583ef47c6f1ad124c05" },
                { "oc", "761d74318391df4f5aa85e72f5ce4e9ddbf0f5f2ace29b790b561cf9a1c8cdef9b606d6f0283a733e372c4b9c3832703a12e591b74b1362598626cc8e653ff86" },
                { "pa-IN", "bbd2186534af6fb49b4e2fd7cf81ed1d18d38b63f89e3956c106c747d917507ed046cb4b8aa0242c3a8f3f7c52875ef74f914639d89c53f06b5c9420781bbeaf" },
                { "pl", "f488d625b1a8f904ab0caa4a52e529f613ce55bf782b7cb526a0dd54a2439c093fa34ad38a2ebcf7c0ca6beb22c0da079939aadb4a218c8f984b8033ee98c866" },
                { "pt-BR", "5c542e13267476894c012f41baea69a686904569aaf9254c3a7153591fe58cbd4bcf0c4af5e59a4d10bb34aac4dc776efa8e7734887763449033a5786956ae2e" },
                { "pt-PT", "3a32c3934a77a4d2ec8a9e94ce858ae5377aa9fa01e223e7855c69fa8b86b47bccb0093a90234cfc3bd77a9ab21adeaa9cf72a0e0eb7e386ff30aae3761f96a7" },
                { "rm", "883fd51ad3b2743bdd260570fbf7cd0741bac00dcff6b11cae39b8d0102c701730d90b349f482b0f66f79cce3c930b1216fb2329af4706898265cd3d1e8d50ac" },
                { "ro", "8c0569f328cbc5399d0585e3864a7c1984dd68c9ac7949a0c3352c0e4564a4dda6f2e8c4b439cee2536d5394b15ec14f792800d21532326ed3b4185c39fd1bf9" },
                { "ru", "f9462c17b8d8a07afde5da0d058160fa08f3bfab808f79a3a15e3be61eea2f1d561f853dcb8c06e59a71562f40d87a6cab3cc712d9722dafc505985cb57d8612" },
                { "sc", "a079859c7eb2c0f76aafd824a386f11e4ea1b2ecbac40a2f7fd4e2fc2f3cdd66e47a5f11087724d3b48333dcfb651f9b160b01416cfed1114128e1ada0ce0acb" },
                { "sco", "4676facf1de0c3c4ab332532b1c208d77e1bac49c12e72b1ce011da411e321b88825dbf9560993e06c383ec84122608f1f6fb435f64486a50ba0d3b4314adb49" },
                { "si", "d150eb5df1cd725f741ec012f456f5e6485fddb7832c054884e58a62edcc73d80dec75c1b695e558628cd852fa3c9bd354bea8c8e60a917d516c25b3ef75bc54" },
                { "sk", "d511d11efcb4a99f3c10d70fc2b300072423b52a69840b8ab9da9094d1bb122b1cd0c17ccd2772e093cca8f8d0a5bbea01a6f877081676d93c91177abb451ad9" },
                { "sl", "028d14db1fc3343572fb5f1d92b9459c44a59fe0bc4d1fea411e6923db35c94f71e4aa7bf48040553986584ba57729b18ade6efbc78cdfc2e85389de6ec8a068" },
                { "son", "b11a31ad79b9a3ea90eb61f5f68cd18d2a15d5c3183c333550f1e8a707d55919cb831e419177a8a3713e3527813b9de48cc1a98b75ad3c5ddd07b66f88774226" },
                { "sq", "1fa763da29e62d7976205be53f173192c52832dfff758a598ac22e34d30e9bebf82347de8388f34945cb58476e0ee4382804f89108b373cbb3945d08a7066d3d" },
                { "sr", "f66dcc90215e79baeb202dbebf23ee564c81aa14135c632a711c5f95b914d9959feec05c1fd95d76d3093b6beef529ef33b46e287743bb23f19754104f6608b4" },
                { "sv-SE", "04378a4e72207cbe248dfb6a96696f1003abccd1f43d0edc702a0cf122c9c16cae76676211beb6b9981fedf218c02063e1dcebac4a246c3ad223794e6584a07f" },
                { "szl", "dfacb9751d1178323b09ef4fc7a09bcd1cab19b278bbd4f020f58728524506ba0d61ca2d477575b3358ed000cf03104643f49041b65bf0c75cc5f1e42ee7ebb7" },
                { "ta", "9cc27ac0783a80871d75972bbee5285ded0ab9b6d21310545650f6728ad8279788eaa9d4c8dcb62ff7f4f75e2c30b72edd17edfb82a88050250f42adeb5dc7c5" },
                { "te", "4292e308972ad6a4a2d45bdade971fe2508c9badbdf00c0b5b16cff6a601c71990159f9a53a5b2d6a12eeab1e740062c303b93fbf58bcb118647cc77bc1ea635" },
                { "tg", "8958c12de943b6f6df830e5b1632431d79c55f3ca9bf5d0f2bc20c51053e5677c652ab3314315880cff9b1d8d9041ff04e00d76b628d9112eb89c26acc535063" },
                { "th", "e170c6e0971d0b30be2045e579f2304fa1b5f79fe005c6cd8327c06ee91371c8470e2d815cc5fca41d961264945644bf7a536f0fcd1764f7f028fa3289a4e170" },
                { "tl", "6c85d3fd19950f006938f6c25e02c9f79c147cb391ac8a029f5fff37e58b546e1688b368213bd2f9e0b04ff90b235d9bdacd9b48e4f64cc4ade582f545d5e656" },
                { "tr", "8b9c14b4a05c7ce91399c02c50d62a0c20e362808d2f828e46f95f22244f9b3e91c393beb668ca0458eabdeacdc88ad1344f516984d784f9248cb8904bb40658" },
                { "trs", "5be60fc286f650f78ccc8f695190d7d1ac012ce521635e218ead6c220db18aae09dbda66376f49bf30eab04738cd7906a71b16b188877c01bf1728729796b696" },
                { "uk", "00e23857d8c0f536fb70461a597ed059d73b061b8516312a2e6c3c26ce28cc2da2c722474ce6277a0f5acc520b3365bf823d41b08cbf5490353d1c058e719083" },
                { "ur", "7b75a530eafd114bca5aabad8fb86af5583a03afb3ed919be68a561745d9b286f547ee0d305c9a6f4fbc6d8de3db5c11c3399c1ce497f527538601f774eb780a" },
                { "uz", "4836ca7c55b7d31e69be2b1a1179e761a3a79439558b85d4f356fc1240af2609de83c0ebd39368877ec16ee101608b522100a2703464705237c447148badfc2a" },
                { "vi", "37a820720bf0584704196251168544b4f3f75a9bcb3ee464f7558ac0c2011ff785450039fa55865101ee748a081fdd274c148fbc6330e42adb77ece564b84295" },
                { "xh", "94136c2eec7643144d02edafc7c18402663aafc2aac3868f9ece43fa7954387c171b2cf1140be2e767a5516e41f9d8ac69feddb1f10b448c80d0a32d661e4a97" },
                { "zh-CN", "099c8d132f0e308108d73d7d73ffcda0d9e76a9cead168b3672872a1e269ea45ed6f291171c7e4cedd9f62962f5ab93c34385fd844d7bfced013cc0c80d016ba" },
                { "zh-TW", "c45c9c79955e05ea235b1620e285a7254226f30b3a196ee611710a080c5a6f0f4e93266a95f92b49e11577686457f70db618e105cbbe19681c595a9adc220ae1" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/118.0b3/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "83720075b9a0fe0d40173fef809969ea6921e174ea460287e63b3aa0f2a452d90c84de28a1be71a32190713a716b54fedd2409a009466b34a9c8ec456c0eda90" },
                { "af", "90aee0105c9f86f45666116d8b67e34a81951a7fdc3e5d5a240de49b0c496648f19f1166af55913a1cb9c7de6958bc61c7a4dc9c6ec498fbbfa882a3439434ad" },
                { "an", "e9f660c187d0247598b7316b2e610d2883231f5a524890510724a1983e2819bd8c0602f7c860aa47b1c7aa1721326b3c37c1d13d51f84f63010abf540b1d8d99" },
                { "ar", "da90977d2d02267cc33efee7978e6cd5fc0107d81fba8af1c672a4db31715d35e57b47d1413f60d9850cb96f64e7a4e708588a5b3900c9de343e18db65dc904d" },
                { "ast", "3254b16586de2d560796833cedf786ca6c4b53d60f216144b95dbcd234c4247d0bf848f3caa4537159e8bb92f1f0dcdff4a444b89990ba8b16dea335a84b85e1" },
                { "az", "1e26d4b63e1cf01f8acfc5cd4d24484850a4f8f72d3892c3353f428f86033315ace66832c9a2aea7715af23ed23d4ee752e1b16c2df11603ac8bec42ce7b2d04" },
                { "be", "cd091c5eff363589e1257e24dfd581a2dce0b718ca22f1e9292befe4d8e5aaffec6cf0c5899ac6bed3c049b91ab563178ede2cafd47474a58a12280387c39837" },
                { "bg", "3dd796751acb22980ea3b1990df8ac2b4f6e671812314a4575fbb29b52dc71f3f854302d17ef8d15794b3cca0e772147780053d26fbbb2d6d9039721e8d84d9a" },
                { "bn", "de59637555893a5f417e9f5a34911ff48c7bbc984bc5d8e4e181e1ceae573fb73b3bcdfcd28a25146d5a6a0e0de9eb3b19dc84fd11e8fdb780a4fb33fa34b0e8" },
                { "br", "c7157e3fb6444108eb605d6dc4bc806b2dec8fe16b7094054300579c3ad974286bbb1a6d77f5096571f49244dd5129d3900fc33cac230db2b39f9150cb94e657" },
                { "bs", "99e02f628617e50fbb31a19e9edb09872dcd3d7c95371515b8542349bc3167d2e13352696f6b8ffb4058bc0dd2d8f6efdac548980916873d0ab90291d5cc7065" },
                { "ca", "bb9e5374a05137e9f2bdb2013d66901a1770c3a00bdf2a5e2c8587480b1edec913f55565c8b55880f2ec8e7cc87cf764edf5dec17d6b1c48d324bd97b0b67b4a" },
                { "cak", "532cfdba07f1bd3d328efddb8072d1bf80765247ea0cb1c78fce513f1be223b553f5b37b212b04f2cc80e8360acab788a3a04e12e093969381f5519700a75b8c" },
                { "cs", "bea0ded977ab20b36f6c63e4ffceccb3176530120afe8741d6b7f6ead6ddf0f01fc84ab0ff5a9861d6a2e37a905c7618c4f7b580805c69d405f8f88503b3318b" },
                { "cy", "2119e47cd7c42798212b326bce1dc98cf8ca3dbb0aa0f0a7a540cc6bce564ea0593078385ee84f4ae0e1a4602fd68e85e00ce20498ab5596753f842f9fe9275b" },
                { "da", "18e4876c327668b8366ce810615a1154b7fbcff0ed6f123da834aee60de302cfb4581607fd544e223cf2574fbaa3dca7be18d4f63106aa1214fe44e7678eb19d" },
                { "de", "ebeccd994e79dd3e5c3aa282e09f31ef020149b3ff1a13a4e1bb3061800d4a25633189112c3a1e487984bb06e676acad12758e629aa113f2e61ae4456634c3c0" },
                { "dsb", "46d854bc2041b4c2e68398cab310c9484fd8b1677816aa5df9316f61287c259ea57b8764a310deff977509cea80f194a25173db88db44f9de5ab4622f9d569b0" },
                { "el", "2dcadd059b7bf0d076977a5b02d7bc00bad385aa714ff4405bc78a82f835c311271c1dc848ce3b1792192cfce0f702e2557d56e0eb8ed351b8ff23199d0cff89" },
                { "en-CA", "b513e7695d658267134e70a1e6d8aa36d3c07a8fc3bec44b6091723c3b2eac2dd420f0786d8aa0810df0e17e473ebbe72c13a876bd57bf86361a8481ed6c7489" },
                { "en-GB", "1eba79f67aa0fa397d7649c7f661c44def5205f3ca6adbc95f419acac95575a3f2feae38584676b1c1fa97f6756329fafdc33429eeb8c7db8f33dc61f2b9ff96" },
                { "en-US", "b3d068bf2f9709c7de2803f21f54f82a9aa5c394a595637d2d75a5806294abfa2e684dd9ab5a9a6c7891fb8a4cc3c2280f86ee81ea13c7e4b0442119005d9975" },
                { "eo", "3126725848f70c2da6d613698fd2593bd40cf5e35fb9cc46590dfb5eaa2f50177fef29c2e83d536cbdbb73e7db88be1b490c54e59fff0d85a71757863a69d0d4" },
                { "es-AR", "fd6e48d90a9269053e3dad8908ae8ff1925e39938a84d2e09f8716c8d802080575121e52a61a5b0818001c0de7d3641a67f13c75d2c6c3c6cff9af1b09e0a544" },
                { "es-CL", "e889fddb4b2e86f4ef6dd925239ce061343d4062244d4b7dd8a273c24c96d94d87bc194df0a5d8e0c9a27f096af822a3a6aae8cf48bf652f808ba813b5f9497c" },
                { "es-ES", "44ab18f9ee99326ab6e9a67d77f10d77c1fd62dfc68bf403e31ba2391a1c8cf8ff60ca48dbf8bd53ac3089ce59b1aa4a838d5b50e3a90f319f0df60c255d2779" },
                { "es-MX", "b312f1007185354184f083af5d2edc1e1c1d56ae71cb62a1a3753bacca44b1cf2642019a9114f89b5976209b3eaf5d5d40a441635e1a3ff227f2124fd85289f4" },
                { "et", "b5f8ba0856918987ff37219d6af8ea5e9ca987a3189d83a1a00954ec0e42c5057322a833282fe7d7ba481ce6fd44bc58b5f40ccdb74d2e1db1f9b79efe24b812" },
                { "eu", "2cf0ebb5463e9a08e81ab5d2798542213bdce13b30a2b4b415c5a1e43a66e32bdd177edf8c5c8962e00a05b7972cc07139949a69b288cdcbf19da420efa504bb" },
                { "fa", "8b8fbaa6da00c9083848d1a0132f7928d8dffd9cc2f9abe98b09c81b1fbe199a0e37f3902495bebf6ba14c0d1ab0a68d0a84df8b7c774f494494a27544bba3dc" },
                { "ff", "7edaeb5c2cff036f608fef5c554c706bb0a66f964e8158509f279b99dbdc96ff4297e59e819d5a1855aab4ce64b35fcf0f792717122118759a4a46f22d159298" },
                { "fi", "37420946339494189c7d44885605c2883523ad03a86d682a11be2e5dae9ab424190bdfddc3584f2114ff85134381b18c9adddec4ac15e9b4a6a763c21296c6a3" },
                { "fr", "7a7560c246af267f5c2737dc19469207f14f3e1cc837e4c9c895e3dc0f62cb32816733cc9879c8937f772392dd38ba94ffb08018beb1d74a1c37c7fb7a41b13e" },
                { "fur", "6a4f38ecd36168772e1573df9627eaf20039ea02f158484fdbcd3392a83cacfedc0455f3baa07e7f427dee6a6acfe0f8ecf8533e382ee634bb0a0769b5449cd3" },
                { "fy-NL", "2aa705aba1c9eabf1352674edb0d6145ca60ddad8a838cd275b1e11db535421d18271a0dee258ce13fcd2f15d970ff0b2e3545e968273e6c01ddb00019c7ec9f" },
                { "ga-IE", "c237c855307388905d11b3101577545517ba090a632a5fd121a6e747cbceea7c1bf047b8bcc2f228c9dc159d8c26e07368ba8df7c7627df8a777af8819f04671" },
                { "gd", "8209807b4f191c0e02802e681ee69e57b5a89459dae5e9f44ae1f1927eb688f92ade5514ea5feebc39c13954ffde7635a8b2094090d97bf75a1d60247abfa054" },
                { "gl", "5870c4d492690c54a491d2516944357ed9d8babec5a6cee651be8d638f3667a446c2fb540a6fe099ef37a32e60a7c1d71a8fbebd6e4530ccb3d0e7158f26a3ed" },
                { "gn", "083ed0273f83ceaff3f9480f2d6c9cac18299c860bb4a018ff9679da4f0a9a4c9f9f7b3c3fa949fd6795b935b8d66b9286ad616825d3079b59761eb5955fde08" },
                { "gu-IN", "a367c5040957066911116c53fcf35362c17946cad4ceabbe72ad10f61d4c921734e11545632a1a5d9a209987461fad7507a67b5ae86226beba2a3578cf7b7617" },
                { "he", "0ee87c33325e5b30993e70420d00c5f7161bb59d4540e2bd9c12c3c2eaafd575b2fa87be048b99566ffaf6e5cda8b54696b8f8e72c1b5f124c05d1c1bf91f8e7" },
                { "hi-IN", "f16ac84aaa4c90fca7b12885acabeb158d6892c9ef3ea937961af123902b58c07eacf300bf1aa5d6cade7915b416260d7fdd49bf574bd7af7fe3f37f33b12468" },
                { "hr", "3ece86d63065811612be769c69311bce96e88ef0918f8897c4d224c108c92337522d518246cd6fa6bc0c9cf944cdd08b81ce235851950f794891e628a1d70253" },
                { "hsb", "26cd0614548b70ffb5196d1f1dcb8b9a60a1fbd4507ec793a9f105284e667cbc343ecba4fc8f56e0b527020e7e6bb01ba48a4a5c44b1eb90d57849e1d9397032" },
                { "hu", "e9a4361af5c09406f8ac1f29a011aade42a48ee037c4b824b1381fa0af4a4aa929081c3bd3c340f217592bc9bd19a3ab71853ed57d4d1648f6f419808b82f5db" },
                { "hy-AM", "2625ad55f74cebb3f88e59bc03b7a25a019404ddfc9735401682906cd7bf7692ad3659e5ef13583c150efbdc9ed6340a7dc8644ccc34763d81dfdebf3d23cd00" },
                { "ia", "e67b6d2e9f99c43b892fb3550a979d7e488769df9d77756e166cf2732ab9f80e44ce945867bb2e4c088251b4d991ce43fa8fda5f084a8c207d5a4d85f49b522d" },
                { "id", "a5f9dac85c8287af954d90cc9cb01dddada84b5160663512d4ff19e340ff626251748dda69c6179bf7daef60ed396063aafc3354ca026ad7ca526f2e145f90c4" },
                { "is", "ebcf73ad91d55424030ec5ea26aa4cf1f8bf7804c5a598675a7b32d2e2395697a230afebbf1e84aa9b1394d1f803eb6b9897b47878a1314fff76bf1e8c22a9ab" },
                { "it", "125b4e99249038d968c3a4f9acda6e38b55e804c1b4d90810a47590576614ec260659c791307afe3091ddcc9b60ff834bb66d926205bf46af7c3f4fdf742bfca" },
                { "ja", "6d47a9a5cc5924c3bf205c300296d01898d309981582afc51b5f981fc200399619f3c251d13bf5b49a0471eb761a0bd44ab5ff888065861cea2129a6839c8c7e" },
                { "ka", "3d69a5713f438519c312466b779d2d4c98f868b464c73c47d7fb6935ee858b87a0fa9abe08d382dafc193f88a553a7ebcd64e3adb08f729be4c96be89ef497d4" },
                { "kab", "53532d03cd5ca369b262a92a0d1043368a9662e04841b6bee5774acba1d9a418c24fe8b3cf2dabdb0cd6e2312d51a69dea8d7451dd121b5c08ac734ca2163ef9" },
                { "kk", "3ee12ec66a10ac2af2058671a42408133db8d420f11771817a46a68ed83e9fd5932fc88398ce73e23495bbaac624a85ead19fa7d897c81c819a82f3e587fc42f" },
                { "km", "406e78f6051c6b91592dbbc655261ea000213f7f65663fe82baf26b5f6c6f602e3a5ae38c9d6f99fa2a2725c929e52ac6504f9168918df66af2b60961d19883e" },
                { "kn", "26ba4467c1ccba83f0125e6a259e03421a2b0009fbc98c279f75a5bfeb9b209eec2e16fba80181a22ae57625a19f227ed2cf6264b396e3a3094fa32f9bf8f0e1" },
                { "ko", "1f67056cc2054bb0caf0be22670e35ac9e1549320252617be8bfe5cf412295eabecf92852fe9aeade0a78637fbce8eb6f4add2b73ce88ac484e7111ee3dd58b2" },
                { "lij", "2f664ca6308e51368daee85dd65740aaac227a4c90961301b233681ab3f5f3a70a1dcd166f5870414e38d7868dc156caa0dde3c06bea193510636ed64853c49f" },
                { "lt", "a22dfef477c693526da3e636e3b8a1f4ab3b3168a0f363fa76ed79ee2e51916eab6b18ded86baf70d6037c68e2281fcd0f2f346c1059cfcb97441d7d7df5c7d2" },
                { "lv", "258958ff24a6a9c73337dafcacaa83c24f8a41c33a9b047bafe9963d6545fe6123fe9864600a70d9df6d898d2d86802460d92473a017bf59df51f05c210ca49b" },
                { "mk", "cebed1c8b55762376b2956e080b3fc36357149520b19522918e29ddf8b4ec54a04cee2e2a11d070feefb8e3bfcf85da734441adb147f5e16753928f3b3aadb32" },
                { "mr", "4eb7a27bbedb62e95d4cbe2bfda7a94d32c5b16f84cc7c829a2bb43cf8bb007ad1d49c2837a7977834314169fbb2a867477448abfbe35aa9f37a92af4ed6ea2c" },
                { "ms", "a3b4562706ee16e2bfbb3ad831c82457b203676351de51c1660e860d516de97c7d522980493c0c98e19c6ee75af673d3f2729f0509014dd9396518e9075db5ca" },
                { "my", "451e06535b1828ad8be322f54d27b99e51364218996a8da694f272cc0c0aeddecfafc9e5aeb5824947ba0a1feda991e99dc60a67f7773360b8591dae3d4e529c" },
                { "nb-NO", "c0ce6a51950ddf591b181c52169efc146eb2b2fb011aa4babf709ae31f5d1962034b89f30f7b725c16945f0366fb9d3351deb482bc43fc79185cd51c57838749" },
                { "ne-NP", "76d622da731d2aca236d3f772d51cb8431b4ee80e8d64167882c7fb63fd3df2bccbf0434e4a4cb30ebf10c8c9ce84c7d4d826d1f1f125462c0e418b0c341dbdd" },
                { "nl", "e7c0e4fc82196a018eb43d1718720cdf5a7a0068ebadc09c60830a51cd19d2882801c25cbcfa8ea6e6b6cf2650cf7820134d6adc0e5b502881a9a37d2369a5b4" },
                { "nn-NO", "a964b79c48ec37c02d0814775e8a0a8f7b0a55a78abcf5085c75a69a15a96dce57fa675031ad6045f41563a72f15c415e6cb4448f941ce60afcacd5600637876" },
                { "oc", "bce2243c496dd1050e5be317ab6bf5a3fec663129207cdfaa7066ddadb1ebfba2fa9f4c7f3dc56c9d458a93c8c037c2130441dc104d10abe7dab249701a3e0cb" },
                { "pa-IN", "2d6fb02a8518298e0b15b77f697f1ea3c170657f4d1c974b3dfca9cc3a8ca46dee3e924241952f564bcbfda787161bd2443f393cf9b095eea6aafd757d24a19b" },
                { "pl", "7210d64439ed377d4b23ade6a10cf535d6f86c18b7a81f465d189244ddabc485efd5b31c6d748bba56c920a6382f93cb5d85dcab8a76d2436dcc1e6f7b6498a9" },
                { "pt-BR", "a2f14009719765c6acd6c8c7acd432638b21e75bd1c7df5698abcde7ad0d9da00e19a182fbee6ff625bcbb525168577372119656161ce718867a18a2c80b4595" },
                { "pt-PT", "a5d15a226965be41644b9601d260bc256fe04aedb0a0c925c565fcb7a82376f6a1aa9ffd2cae6169d8ebba94bcec7763a161429a734325790d60f69c078916fe" },
                { "rm", "a6d7ccd668dc73a240183490e684799ca3d378a2eb9615105f0f232bbf786f087d91ddcdefd646581e7b4866d3617276ca8b9884474132cd31b73efe300641e8" },
                { "ro", "56e8deb89e3ba23f96985ef41d500597983964a0883f6c476eecf5437a7c64f99012640b8bc61e18d173b8bfcd699356e83cef3e3b787d264c74f4e099014167" },
                { "ru", "78f909f62ad2bf5519e5536dd866848c54f5f14f1ee0e7de882ebe835435a1aa7ae0c223f0e610909b2fe841c2d80ecef096f6c41360e6482c92856599ca61e9" },
                { "sc", "3fb5c122937e6779fca8fb3a8366b83f53b711b8358e08e6b74d559b41a6ccb6f928f7829510afacea187dfcf2208072475c2837f6d5ac80b577538e1f9f5ccd" },
                { "sco", "b6966a379ebd5a2b92da61e59f48301e8cebaa904da8e260ff5b52d61f54491d94fe9ddeb672f7876da3d26cf2467ac01cbedc8569c758e6c194a5127857c84a" },
                { "si", "1123c78a267f33b2d427658d9bd8995782e46c541db6be5b7ec090aabcb6dac3eb8b25d20e1419aac4e7410cb3f5d2ac05f665bbfa23c33f7138950248ce23b7" },
                { "sk", "889b364e69f3f15ec6fcb6e4e4fd30a9d5c6a4881bc2722b8557c139292d71385993cd651a7adbca1ac2e508af93e9273924f36cd91a34b48bf73ba83cd4c991" },
                { "sl", "c9ed53d3dc68703f7dfa8f40774a3bb464d0794ff741fce6d56f926101f7087394ed9a7f0ec6fc4b98e8d3e6aee406078871e0582ab97221ab609a2ea1ee4cfa" },
                { "son", "3cce1ddf8591e2f98cb40857567ebf832ec7b4bd21641098b261e0cdd9fae7c14b420e10cc27d22bca744660592147035bd8143e8c636dd9cebe60d1eb7cfd4b" },
                { "sq", "578bf2d606c6eac95e9f2bbe5c47e8c011bbea139726797abe2f58f8754cf7e7440bb81b3e419cc1274d75d63ace4e6dd117f483c74bf02c4cb867aae831ec78" },
                { "sr", "e7d273eb3cd5af6677530fd83700427e6c351ba2b2405c54dd3fd9cd0d69eb77eee39132dfae464112c17ec63ef608e3c91488999f4cf0e3f4b383ec76dfec73" },
                { "sv-SE", "3fd3e459b937b2e765c424df77d9877410627eec3a76be78cf7c34362f15d2694ca32292ce6b6b60157d2204399d76097fe9a08a8719ea51fc3a210f85d1a975" },
                { "szl", "fc6e033b1512dab0c16d6642b0d397642cece4abeb7c58d7752c9ff29dbe98dfb9183b71d105d8c6a752833367250e18a2380f61c64303c243dd6460a698e1b2" },
                { "ta", "251f5da84fa829379b2c9da3d86cc7f3b02f13865666bd4258dfea393e67031acdec26b543beb9e29cdcbbe462033112e0bc9d45256a27d438159780a02317cd" },
                { "te", "4ff79627ea829ae4578ad0ca9457e1f273bcfc9c8715a9926ac79412187ba9df7107f157e779da6d6683af33337d85a2ae4fdd7dad75344dab6d9c83716d762c" },
                { "tg", "6a111d5404b370f60a59d5029b8b3bd14fbd702c844ba0a83a7a037025d782b664e71434bb8415da76badb021b5d8ecc2123f16f36130b97a999fcd48e154a28" },
                { "th", "ecf1a14557b378a73ae5c94c523471ed584de5885b811f74423b533c3597453bb269ae34711553561f6f0875bdf88ebdd346d688089ddcfa12069f8e5fcef83d" },
                { "tl", "415d2e5ca2762e0f7ef3feebecd56b71bc7773d58ee0a1d1c82c2337a5c3325fc34a3b28612f755bb40df106636bd43766e611c95ce4f5fc04d35b791c263640" },
                { "tr", "39a59a098908c67e48f0c26c434faf5326020014ee8e72192bcab1c45b05ca2aebc00ba7d2ef146b922be705d635918c1e87fc8bae1cb940d965b6effa27c61e" },
                { "trs", "c2c3a922485b7ba8651e707d345eedc8815ec1ee7776f18294cf4544f0bb42042371baf91cc0194f4eabecdba93f693472bad57b1294d876a0a254fb0422ec98" },
                { "uk", "529ada05ace2f1a05aa92c7ffcf185022684766e72a485b53c5aa248fd86745722d0184d9b1e54883bd39032d72a2e0463b037e1c01715ceea48c20df7c06380" },
                { "ur", "80808f6d297d6c3bbb0c5c3957df2afb1937a1f145333ea3f1ecf897f36eaea6361d1bd0102026370df38143dfacdff7ef1b62521263afe99d1faf1b89f63c59" },
                { "uz", "7560a2400ad6c30d59d943fbd15ecb5e4fd61861b7b4570f9b712a09033637c30656f8b43598dd626d5f6c56ed87e1ce50361d01508f00bf252560cb75cc51cf" },
                { "vi", "62139e867fb319c1609f25084b9ebd329955640de5710f6ff5ad6d71e7018859f8864368bb9f1f984097aa5bf00605ad7a25dbefc4f5159ba77a6ba2604d175e" },
                { "xh", "30f850be6a5879be1c1066e47769d1c5b0aca47d9bb42b251403c4f86f8f76337ee34b363cd39341ea4e8498e4c8e3d57b3bd8844123fdb9a57cec3fb0524118" },
                { "zh-CN", "be99a504de6cfe9a2c0c667b78d61d87341fad0d4a059bb0ab0d9a6c0ee074ce96c4f09906f2bf9131c6efba898c0feb4b402e703f7e485895629f9dfc928cd6" },
                { "zh-TW", "7cb1ae0684e893cb0ed9586c6adb1df95329a1a9e98252ff6725bda5887976ed39238f955413c6c952a42bb606df6f8ceffc710888f759bf92a4bea4593233cc" }
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
