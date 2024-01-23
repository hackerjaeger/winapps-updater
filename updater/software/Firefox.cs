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
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


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
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/122.0/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "2abf0b9e4d0c4a32f1d940e7031c36a0ffb05729c8feea8595fe594815abc03d3c1ea1880151527238139a7b302b4ba67c0d02f1a9d1b7ba03520081f507a5c1" },
                { "af", "60213f4b1f2a0fcb174f1c37333058e973a32c6848a7c660005020f52ab6529459f1892e9859ff94608543fb0c7959ce7db4e8531ea8527b75f1a15de84c4cbc" },
                { "an", "373bbbd64a774e539f7a6e97919372106411f3a32539270d5f51a3d8de5cf78cde026d33a048b3afd9cdcf3051520215853fc0e731bcb1ec97568c8f3f2a9172" },
                { "ar", "02adce172b99240239abdc19ac037d4740b9a3d8a15b451d37c438e6d4289b9ab74d311c480a81a5820a43b5eca8f6f55d4f45d307082ad1f06b353a85a9146c" },
                { "ast", "b16e6b518356e0a1774ca68bea42741a0f111af8e4ca7c8e71bb2432d8bc23672bd73c67b6fea9f462ee350fab6686803ac1520279ae8cd6c4a13cb7bb85a51e" },
                { "az", "ca5e3e5a0d4285c56c8007f9e0ddad44d67573760fb53d23e88f465a54326699e37ee13d28006459865114249ee6095ad5de304856339ed3ef205929d0fa492b" },
                { "be", "f2ee87fefd07e953737c84ae6638bbe9a18410af4690c4761c67ca7531b0dde34ba18c7c7bda9a33cdab9ca88140486f2b25b3fd8e03112456241d603315bd39" },
                { "bg", "d6668aca67eed3f72e88bc7f61d3900c5da9b16bb6eaf17afde075882f64fcadc61fc62de25b39d26a8fffbc50bd6912b3442749002d232992c1f0502fb7f673" },
                { "bn", "20cca79b3e32294cd6ebba6c6af86991c68df8d8de91165cf98e4c8c116ab2ebd74f26f4c0fc48b7e8eb8feb6df93a16f525aff22cebfa327d53407b8095bf8e" },
                { "br", "c8d95e15f4b60b8f10c644a5226ad81dd3e62c15a10e357fc7fde49f46c2ea9ac6fcf0ef3052b87d4aec26b62cbaf6a815d9b21ab3d9a00778fe5670483ba833" },
                { "bs", "4958661c6ea38d52f4b785076fd1597f0e5679085299c00943b2ca24b603a13ef07105b12a582722bbb49083cc134f4526aa84d5f1ad7627b019223c556648f2" },
                { "ca", "186401c232ca1fe39fa66f4675c62eea0202213a6ccf6630f58d2db6d13c4b0daa6d5194c0f684224b9bcd1ffbceb7b3bd42893e426b0f1fb2fab19dddfe7dda" },
                { "cak", "c93ca5bfeb01f03bc4ccc26515b6daa2822d2793510c3837de6ab5c3185f0aa2ac3ce8046083754d61e5ee5f8d6a8290af1be4c466bbe1fa2ae063c9d06dd2d4" },
                { "cs", "2c305824e2dfcc82001fb57ed85daedb380d4ba59ee1d41dc9f0e962cb87465bcd200253318815b0faddd62f91c66805781cddac0a9c837578edec7957d02c44" },
                { "cy", "47d20e7c86131b8f2921acf0876c5add1b8b2e2b18992aec32a2f1f03cbe1808d8e11a130163cfe2901b5c110ab6743b536b7cc09dcf2089453ad41cfd9f6b3d" },
                { "da", "5822a85d631fd00f5f3518df6cab02927df19061481c169610c1cf03af540a7ca554d5e275b37063cb1b43e45256fbfb6a4699e4f3430fc576518be3fa7f4c91" },
                { "de", "f9a42da6b557c81a722e8edda17b176ec8966b611b08fd32845dad3eec20a2015ed97060b05fc2d59341cc08ddacf6312987f2d8e1c6e698c759842f815c94a4" },
                { "dsb", "01d8cec37f24436d8ee62b811b2dc62822f46dfd614ae2f40ee52c239d621a008584d534ac951dd2e9b4aaf36ab69aa97b8a6777ed13d07abf797c08870f191a" },
                { "el", "67238446e39025142d1c7100848fb582296f22b84a61ab9501bc83ac59b09975a25a57a6503687088144cbe82bc2b512fa48d1a6271c66f8dcb436fc7c2ae739" },
                { "en-CA", "dc65142faeca0be9ed54b6ae9038f2f7e3aaf70b09101ff3b760701acdae72b80159cf00c45c0989150195f0b335035ce5fd49055d2c362828da2e6fd9276cdb" },
                { "en-GB", "104a0c697ef936eebf5be95608d956ca8ab2d8dd1853340f0132f2cd8db5a86d1fc70a9d30cc1255e40beae9e1b973b8a812cde4a47769c44e185215e772993e" },
                { "en-US", "6aa18143082a5e41b5b2dd834bd5d6479c5819cdb128ed59925ef34098873bbd201176c46b1a0a7e0b276b318f76135e6ca059a1bc0aef67f295cc755f0fad2e" },
                { "eo", "24f126e9fa0f352c649fa04b72cf14bd0ad5f893e1e087986ca95353110c37894b90968c9e75dddf6ed53c42e138592223cc4c4314fb77d8710a99ca41baea90" },
                { "es-AR", "8abd56a8577ae0e8ef61ff7accbe7dc280d3f2c2be8463cebc239de6972277208176375f3853da9534907f4e0ae326097f5c38d1a64ab0b3c79b624ff3a37354" },
                { "es-CL", "86f9c47d7f7615f1db6b912d1bfed95b147728682157efe465a3add5bc865a073905ec18652d3c9bf6f7031ed26418ed929116d4cfa3ead9fc923e5ef91a2228" },
                { "es-ES", "a257c49675a214211144d2415a37e4bdea44779a4474f63c2d53028a36f1703e73d5c336f507c8f84980c80e91860ec98511b4d3d3d8b9e1edf75d18a6012611" },
                { "es-MX", "d8fd0f0c433d02ea86e7e66ee739d91e4a03f87c0b56d522794f1b0ce46d6d97ff90eb7b4213875400179e8598bcfb5f1bedbba52061403c689fc896d39c0847" },
                { "et", "01c0dbb8b351f16d4c5c3e931fd5a4a603cc39ddd3a22b8ae8d3468e9fd78a4278b2b42dcdcd8447d9fb4968a825a4fd24ac5a02467b1fe9d568b67f1e69d080" },
                { "eu", "ae5f0870bb1ed45a7d482c6d2f4ae4f2abaf07517a1248e7cc7b9d2b78bd502fc8a6e2e32965c54cea91c15fdf167ae62bef91f7add3d07cd2eec7161c779e28" },
                { "fa", "318e02f5280fef77cb6fb6431d3beb9b306414b4b73e211f269fcec079d345ecf677d98053b47fcd7eef1bba282b82899c62eb479beee5f2572881ef353ab6a2" },
                { "ff", "cd7b8688d7e199848d656ae48c233517ec2c1bcced398662a654c1ce64b8cfbf2ce7829273ec0486aa227ffc24008f0fa7d280050c181b4e581aa30f10a60fcf" },
                { "fi", "f607a757f44fab1e7589c8ae1fc85813ab56de9656bf704897a921afd7638fe21c7470da4a43dbbeda22dd17790777bc80932f45752311078d2447cf094158f5" },
                { "fr", "fa5ac87ea96ea000f01f9a4f74e1a4032f39dc3e5d715724c839698464e7a23c54a586d272a888565e7f76635cf614f8434d8ba979c8e8c0afdaad8c1be26975" },
                { "fur", "7891c76b5675201c8cbc457fdacbc704453f05dc138976b5f7ea282543b0a7436c5d21d50552910ee81ce273b1b5b7a96226ae12efab17f2b317de8f698e24b1" },
                { "fy-NL", "4673561a8fd3c6d87298e3ebee5763fe160e29ba450ade2fdf60f9c17e416e851907bd58de55ecad8175fc26bcbeda7e5b3024a399df381d0ac10a491a897f83" },
                { "ga-IE", "0311b99ed75ccedbd82248dfc1186bc31c64f50594ba0cdef14b784091b25ec8b6b90e458348c7aa914b5cc3ba782655b86a7390eebdbb83bfa199375cfdc321" },
                { "gd", "7705f8b4504cfecc0d1b527afdc8c275b3192859aeaf82e9419a15ebdb4399644e94d0e7475a0acca68f4d0d83dcbdc70e39586c1c4ee8cb81e884b8352feaa5" },
                { "gl", "03549907f5d8e9b16a29fdad3db9f25f056b67931b803436e511584b4b714963e5fe28df4d73a56d617d4865eb7a40fb2e0d8577b978d4d907d81af73e4335c3" },
                { "gn", "d36892a9db3448cd62aa92216711a416d93f879a79850ded277bcd9748a1d585d1de0544db9a4828e352276fb759aedb9fd271af151f1a009d1601109bc0aee2" },
                { "gu-IN", "4a5a0fddd98e8641b6329bfc4b50eb65c96ae024ff951580e185822939b981bb7b2cb102e8a423a88d982ac169840e6c6f249f7856241279ba0cb4bf5f1116b4" },
                { "he", "10aafae13b05c4f594bb2ff6481e832d8487de5e827aab29d5e3311a0901ca8205727d671702867df9f6913e630b895fb6c995368ca66435b47d3ad7592009bf" },
                { "hi-IN", "df84292dfd8d7f17d5ccfbcc535b697d432acc49d88b6c52add78f2a89cd0af7d41b6631ab28ada948e063b989bd21112861ee5e7035e44f114c32c65f20a53a" },
                { "hr", "ee7b5693554ee0457c443c926728570934066171db6ecbc7c591d47f9c693171fab761b85aecfedf55d71c59342529d095af349d654f52dab110b73b6ac9b2ee" },
                { "hsb", "a0a163a0a3ccd91bbf0cd7392b1bb92c366b0bfae58aeb8076ddfa6e9d9d8158a249e6817800a5eae719e5c9054031995ebd836db2a4c2765043803d2608a9af" },
                { "hu", "6b24aa7b30347450e6241ece3eae84c08ec38081faab94bd42b9957cae3268d66c8e68e2db45df361e0010ae794476deb8d65e774d761dc9d8720554f856857b" },
                { "hy-AM", "06edd8ea41d6fc859378ea730e4dffba8e0332338ae0d0f9e651abee16c5f4ea4ee107825f1c1b0b28a25576fe3625e6e681724e876ab549ed62283d90b7498f" },
                { "ia", "479548f1272281e2dcdcb2413c8277efc3ed9928e250670b62a0af047cd3d9811499dbeedfa4e964a7476db91c8f0605995dd6e1e8779cb692b035b8b738e800" },
                { "id", "aa2742aa512361c460a656d1fb1b106313dbd8188ff7101df887a8ae25236fd0d45c7fdb5b7752f80efd953a56df4834fd3713d5aa28867f2f0657add5211413" },
                { "is", "59075af7757380df2c11537e26be392ad313f3ec61ef52544f203285f04e84f316ce9396133f3f80a80fa1465c4a3d6e8dd6a8d048102a4fe8206f6d2fe964d5" },
                { "it", "a4cc33750869d0cee9cf9d9c8d71ee75788ccfd76248bd771d10fe1c9187699b7a72ba1350c07e399f6411aa297087059bef7eea4f95f5eb3962ebc01bb6c669" },
                { "ja", "3e914bbbd87b3e3feba603677ce639d62a1426e261b6318ee4b45efac99685b05605cf0d7b4f9007d47c33c3adcad40beb2f78e0803db43c9c252b618f9dfa5c" },
                { "ka", "8aa246884bdbed20a7b09ff32437b84ae5e8dfcff0130e73bf90a8d2da9b1d4084157f4fb3900608022ceed6d0ad9bf1e0467b37e99e24c271c54dd386dc3675" },
                { "kab", "ef0857e83c5a58defd048795bdd774f8441811009b74ceee829f6af0ad73ed9e83e36d88f756d3938dd17d349c39c0e5a5c88cf6cd66955ce2c2d860813fdad3" },
                { "kk", "2fad1c089b69db36be97ae339bf21a9ebeb1d1db986668b85f33627551acfad23d527662525b6e565115a592b9e8d56ab29cb3d99e72032b4109dd18741c6df2" },
                { "km", "5b70aa1e3bd7fa17274009a04c3b52c719a5dc15de194272ede953c4b59edad0c3e21b8c628ae41b7885b65ca22012f72e92491496520ab227c9c59f5b4525e8" },
                { "kn", "deb6c20d70b9c29fcbb19c58f9f42fcec2f5496b4d501f7f43adc1ce8a87f4ae509f7ce0aa5f3365b2630ec94838cf8a880adc7e1163f0b2738d91edb70ec963" },
                { "ko", "ef816878d4ce73fc30113730fd8a7d63c527a942b144d2d3229af18c1981c68d4717ef7fbc5be961db1e9fde9e82b07719d260fd9388c0077c801419bcd6c72a" },
                { "lij", "3c7a75fe0604545a8247e27f2921f1ad996523d5726b1cecf04392a46bc042cb0b5ce138c4f6dc00e4a43f735b623788dab6ca6d7b6d1bef1a80ea41892a16a1" },
                { "lt", "1ca7e71da2db54371d516f80c2a0db3c931431fa50f173f62c647133a2ea1817c1a243572cfe9602188641133863650946e29f2bd859e0a50e8630d0a7eb43e5" },
                { "lv", "7b1e3ce168dacb606dce379c1bb90350e27a1b76920325b4a4ae4c805c9cfd0b1720abf533d46338704605f199257868850873d4eb891b1ac9ecada4adbe1ebe" },
                { "mk", "8b68ffb160cd356e1ec9632623072c842fb7d6e5eb6b99df6c695f0623a3d9f05353d6efe41a1a4704b3d0ae71f0aa2ec8c206cd8154835ff34a151217ac7d56" },
                { "mr", "097473ade1efaaa4cdd83b350b706835c36a07853ee61f2d433224489e1857b598311301084455666ef488b0d9ca63550d005c504a147b93631462bc45f13e14" },
                { "ms", "c35741ef6841294986a712db8b0813784d9bbfc2091ecc5c5789f0a9a4608603d85a0d9ffff13505714a41e22a576892a473368aba981947ee0bf860394e3a6d" },
                { "my", "0488aaa71addd218d5dccb6182d19118b56f0786c4b049d27411038879e8ebfa04afd8f3cbd80378ba50f6989d7f4b2751401d9a018dea188e01c21aa12b929f" },
                { "nb-NO", "95572b9ad6255e22981aaf43aa6963ed2508c3bbfa1b46affc6f4566fa6f0dd427dd9a415ddde32d7d4f8edde962905cfca7e3c0366365530283fc370bfe25f4" },
                { "ne-NP", "a7a7ecd234001d279e1fd37204022928dded8631770008a54ec11f50ecf41285791c010090ebfb2ae340206f7706bfbf3ec1a6629d5669c6b8b973ec0f8dbf92" },
                { "nl", "89e922937fb5564d2ddc9b61354a98c70a63d8f898c94927ae2c654942cfcceb2df46b993f41505ff60b3dccb711e52906427bc12a6d4698718100a3aa53db14" },
                { "nn-NO", "d192c5f897b83f704511c0a4fa36e2305302c6dd8e2f1771c98b1dc85ced68d08b40904f584e382e11aae869a7dabc454c6063bbbe09d1752bcbc90acc415869" },
                { "oc", "f51f1a02d1fe6a1a173bbec225893409af3b5a5d35e30fb95e713e65069d1e1185e7b6ce4d64ced1b2b16057b3f439ce72a5353ebfdae511222652c584584715" },
                { "pa-IN", "288f1b08b93d7f0c2f461dfa218d9cc8de39ed0be4fbfd4d1799d9523382b46ee1af3380f7eb79c0e7ef0256e2e89b03f55c8df8c7b5f635c2ed9c5a12d1b857" },
                { "pl", "a9b47776d802e6d68f7ad1f10a612ffe6b5d4b136f0a7e96f6e57fab98fe9776d3f609cc07117c10ff28a4e58633322f1a3e16a976d15b04181812752aaedd8c" },
                { "pt-BR", "6e343eab7b03133a7479445045f68b010b8d4811e38ce3890f591da9878b599562b3735d4a56d5c05882f0e6191ab0d3de9ae5946d17dd2bb3ac1e4fd45940e1" },
                { "pt-PT", "10753663c701cef61e1681220de8a10fcc9d4ea6e8b2d473eb73884f2b106badb03e4a390367c6988ebefa007b83ce78e6eb20c074450e55f6b7060d62abfdaa" },
                { "rm", "ec8b88c4285771052c5895ec1d08c8578fe03141b4b9ac24941b3708cf8fbba5523302852cad56a4460eb74a6947c541958f9e8a8e1457f29e8315a0c4d7b438" },
                { "ro", "03023335fc498767f875f85e995d541e21db0f1bfdd7508b72d6bb11fc3fb88ffa3408e85c359084a029778a4a91528d5a541838d835e763677e3ebed34a1775" },
                { "ru", "e36aa3bb18de77a1cffe0f1a56cbc0cc0310a64d6228dfd3e0f1b7ea1909059ac2758dec7023407b91428d1e501fec694051847932ca4f083d73eb7d3d9d83a1" },
                { "sat", "bbd983c13bceaf632a1ee14cf0ad65660cf3717ea61f01d84cb1c145b0b8be3f31777e5405a7f178cbff90585779d9b39daa444ee0b87bb7ed76c7a017fd2f34" },
                { "sc", "86d1d6c981912cee9e208af84cd2861e42a851d4993381257f6435ee9bce8d81b87df7b10f2c5f2c6d2726aa27321c012a41015396eaa286fd02c1abdc3a669c" },
                { "sco", "1c73d61d1833e964970edbcf22f2527556452fb4920d2cd4c8d02c58a8411f89ff69c7d3928287e02cd6ff8f1e42b30b430162f4f1c68b3b19fe59b3de05d5d6" },
                { "si", "b06d8f6676d07803c7c0aafdbd7bc3e58b1c99f4ec016e4c2423ad27e62fb96173d239087f2fee17fec7d9f74fe9347210a964ec3f4baf2171ce9f0304aff6f8" },
                { "sk", "f90484971d923c87fae5e83f55635882b1d7d6a21c80d54b443b7db88da207d3fc1604bfcfc61f997b73f5d797acdbe64051283aa4a0c4064e08385c5b92b031" },
                { "sl", "fe6bd5bae8f0f3928f27c6edd2705edac045e1f46ee3911ef448b612220ad9204677d31d7033077dd4ca493bea1e6ce85fd2e383e2733e0bfbfac33e23d5cfbc" },
                { "son", "efd4fc83585e8950f3b54ec24224515e8c1b15cfb763e367f48e71e216b04fe7c4fac94c719b90326b0db444bda93ec7ec49da9c31e81e426032304a8b3990d4" },
                { "sq", "1883c55a21d6fc21e14247cc84fec72fc9b735cfbdc9a692f3ae8c679dbb899051276ec328da6777da9afe710b60f48a6360a99d9bf10b5d845fcc9a89e9999c" },
                { "sr", "2e366d08b8708be6dc159131befdfcc5ed8d933a0be8c821037d280cfb2e1fc0e204aa914a3a2cd37492426635a67371d54be5f909e87f8d8427deeaf3e3da1a" },
                { "sv-SE", "536759a3a47a35c8375cefdf45d4c564200e997bffc5c4555a75fef253cb0ebb15ee8233cb6101f6dc8d06791fe8dbd4c59683afcd35d5a7f5380d9820d44b77" },
                { "szl", "5bf96f3de7bc11696bc019bf949f14cad7d8e0e0540de49b4214dfe9563febebfc5fda7bd5c019a471dc624a88fd886f26174a839b3af239370d26d36f27e72b" },
                { "ta", "c6b81a1b506fbc347cde5a456ce4844e3221708974c0ac81cb373233a1caae4729469b0117ae3a3ef45502fd2b6e6c22d6b2841dfaf08c4c5f7769684ba25fa0" },
                { "te", "65f6745557f81aa8d806b8533d09524c50c0e9be84fe252fd7bf35f368878bae18023be2cc6a08c9d539136f8acadba329e04087e51c624b4948e37c25eca4fe" },
                { "tg", "c245fbf3aa104e6492a8d0e311f5f70358b78cae9c4de7e237590a42c07d66977680f3f3410a8b283978fdc1370f5da39148888c507f6413b3c7b1ad03ae678c" },
                { "th", "3dc99d803f689009b210924ac4210187d857dab01e4f77f72cde0f4a1b33bb3c58a517db81db34d9ede381b945542164082140913a6bde6c215207f388e81fa9" },
                { "tl", "429d6acd24127eae399c87fbfd1d05df9d61c81c7a4e67def5768cb262cc87bf90523f8063eed77095889e466ee140df49cd8ebdf093e448bb9f8e96844cca49" },
                { "tr", "7cf35e6906e2a4c48bdb5152457faf2d940d8cc8d0e9c7c2b7e157429235d37b4d7ddf0a03c0a809dbb5beff59ccbedd4bc9b68ece4353a9aa5d9eb1c18b2f9f" },
                { "trs", "702075ac8b94fecf7ea995f07411e319502ebd710ec5031c0517beaa0c3da8e9bbff51afd2f4b1d5bb75c21c78aa3badf59fb2a62e29a8d2fd6aca45cbd80559" },
                { "uk", "44a35794af3d410630c5142820658782665f33ffeb7c428dd16f3f099f94e443b4e7853d6b6d79e3b2af5c7494c4470749266ff7515cf94daefa98d3bec28ae9" },
                { "ur", "b74ab1e0edb2290cc56a0797d6a0abc1240eaa849271b9a5c09ad0fc9a82bfc37301c98b00ef5c4b998f7833655f1579994c91ba370a318a3af4802ae0bebc92" },
                { "uz", "c6a460b8a999b997b7d896f0a51a43bc6b356bb1659bc9cb15635f3afaf646e0335dc433a76ca384fd9df3ce30a96f316075535ec7c9f5447b1a6f6efae9c377" },
                { "vi", "579ef03c154a95e5501d1bb100cb4b97cd22a0ed8b682fef69703c1ae17a321987fa9b75c86dbd729613371d1ab4de32225182808444edb70b3e29362e08ec3c" },
                { "xh", "1f09dfb97e0d2c3cf556d3b14e1770ab6c04cb52cc15f0f195f5b47eef326f3829109d9a87b662bce4c31ae2c7a2992d180478b1a2a17856be4c80ef7ff30a5e" },
                { "zh-CN", "d2fd5fc9f74bb09c7344e1be1ce6b31459d5992bb6aae6f2476fd919654e101a71ac0035ae835e6103fc8b33fab7da28bc5b1b6dda43f8b8e4aa6d1608f3d31a" },
                { "zh-TW", "4d64e4f95cb73b1b64a0a1d0394e3f6f4407fa9cff31a80517c9ecbde47780a2f44b10d455a61556b66d281826c8e8a97f3d48c78810536a77fc92545edf8f90" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/122.0/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "267f2778fec20b4290aec4edd3b3a615f5dc6a6b07ae88195c51747f71b21170638f7d4c167dab14ab6eece093dab385b421c4643105d320814d3130eee7c6fa" },
                { "af", "45cc7c9664aba6a8d93106d5dfcb0b697e81c94bc3ac34cbeabd683d5dad228b86bf5ec4fe0f67179a2fe7933e9113ffd24ac0a2e4a3c8451f05455fa162f674" },
                { "an", "df3238e3b9e8f1d00c51bdc05d517c3a703b707a55a0b117452a55c6e632fd240612861a18ac627afc2088f96ec5998baa6b2e30f3b5eafb504abbf23ef6c873" },
                { "ar", "fa42003e40fdfd5ea83767366781a2558d461a0f0e627dc65d3250be205cbd08fe49e405ca7712d2d69105b8284b7855ecfba0daa6ce0716c70c48fbfaa33427" },
                { "ast", "426c1beb2bf396c9595bd65db6b5580c8ee75ab784c033deb5ef5f6ee43339abd106fc8e06324d7f6800fde701191095502fbefea9bb36d571c34ffdd699bd5d" },
                { "az", "0cfb26e3ceaa0788898dc04a0a24748580290cfe7fe9978312b7e652d3f7d7c457fe62c3ec99146f3ec2bd83010eea93cfbfd90138784c5130f7e68507784fd9" },
                { "be", "a9b77ed7026ed112669cf2145b2fc4f193f217961256a73266e9bc080efc5779a98b97b788d6145787fcb4b5b23898a1d1da968266cdaed118a7ab67f4046674" },
                { "bg", "66d8877125c135638e9b1cd2851ad0212825737c91b0ab3bdb8a2d26cf29998be21cb4f10c734b82ce87de47715f4021ec6e4722299f145f8d52fedbe76ec63b" },
                { "bn", "fede8d25159cb7c19abace976221b6ca9e15f6e3867024c6b2d0a1c8423aee0e12a2ecb2420c2dca40b308646c17a042e07b7b92f1e5dbcfd8ae95129b5c0bc6" },
                { "br", "ecb3b4944338b0feb5516cda69de418cdf7ba6a582acbf09625643ccdaa4d92f43304b0486218ae5d9dc950ca525ba56c0cb8120aae25b73bcec29fd7695bb96" },
                { "bs", "9258a6440ef3d5aa8f4972c4aa14c4c2d6dfc1cb9100bd3e0ffe83cfae72041aecf2d64cb1bfb6fc60e924601bd5c2fde7b0c25cab9db44d58b09f8b6f2a8222" },
                { "ca", "e0bd6fce54136ad18b32edc35bf99e905166a787d966661dcfc7924682ea7fa708a34d090dc7cb69a1a5eb038b26fa8b5e39c3eccb97af08e67bad9d0137a680" },
                { "cak", "22597a8973adcf5f3a00ddf5226df2acf1e814f61a45d336d72088f2cc31543e4cd96061c5d15481c7d1147e16f035431e7ea029e0397adb7be4fed0944febba" },
                { "cs", "994acbba3700e9e2d08b581855b9f836c5a5ef9aa1d412bd5acc3e8ef37372fa1e76d981c770830543d71ef18476f909fa3a27406b93625842a00239b2e31a4a" },
                { "cy", "441805d7f233ed2faf7bfe23f6e30de3b8d197c7bf378428dde5906d84356e515b99ca92ec4705685de28c65de1c289e995577179c3821c2889339bd0c51dba7" },
                { "da", "20ad447ac150e26a575531c15e0f734fb9bff5e570347ac8e1883945813251e9204406d194249c86bd82b934a8f4fd983763f1ceb5244f1df8b347dcc84329d1" },
                { "de", "3db44c5f27801fc1d5471fb735b12095719179e93453e4bc6d0ef4cd9e90277f1cd9b163914ac8ebea01613b451e228e8a2320c2fe0d93b983f77675ddac594a" },
                { "dsb", "4197d25d86d169af895aeafd9f9debc3056d0f28588636fd177d1845c3f3236ab03ddf9efb10fddaa62f1f92bbbea13d46482e4a591497ebb4708daca22d7f69" },
                { "el", "e2bb1d39cd3fe2d06448280383f61c70c48cb17c7af946173b94f9bfb8359501b56e4d5c9554530883cbc103c03c50814c51774bbf3650e6dbf63ee5f39d5f9a" },
                { "en-CA", "a3bac67ab5cd472274111832aa0e198609051455c60e5ebdc15676a51aaf726b67fad0f52d7fe4b20de14c815d0bfecb6ac8b67f1749a9cc97226b2595c9a0f2" },
                { "en-GB", "ab281b709beddf68b572cab5b46a896cc8378c3865221d575edf692bd474112ecf7d85b35dc924645f2f1013f7713689f0b8e0e93a5f4cf90f9e338592d0d120" },
                { "en-US", "8c13bf6a01211ee5d5794a3dfa9f3d418edc12b5150e6b779f47fe4deb2f522147a799f1e2ad07b607d8ff947dd558538aa8f1c97758dc3842fc7a6302ca79a7" },
                { "eo", "dcc0f1e58416771281001717afa88e77476f5ac7f376f9af418ce587918e23ddafa5237c8b332002c6331ff84e52a0c46d9fdd5545e03377127181b60cd00e66" },
                { "es-AR", "ebfe41a5e6e8b2387a7c88be870fa7d3a8f88a717232900f159afcade7ebdf3804527215cde423404b56615b2d532278fcbee9bfa9c79083b1f606ddcb996ddb" },
                { "es-CL", "dec42fac5cf7c00c7af681c6de8352b2e51e4afbb7d97f83525cfb1f8c697f7f92d4579f84f46d5d85faf1d3212ce67d97ed5121d3250eba5ae10a9f229c221d" },
                { "es-ES", "21d8394f58decb06853a6b2dd8eff923d99bc5d8f68a6e84f1bf7f3a4b51f5945c442c2b5c07e12dbeb30e9d6cf00ccced621612b0400a752a2e6c84faeb10b8" },
                { "es-MX", "45f88472867ab6c10893caca40000a57a916de2d28ec94ea15ce8b6e6ddd7c2b3bbedfa4512ce24459fc010d0479c37334ce46822678403bae374e0dfe9b8b8d" },
                { "et", "faaaa2276393a54a29cf4b4b6ef571e1930ad6ea0895b69fd292d22d0bbc777a90b975c8b885324ffe2fb29a50d045e7af721ac30f3bbb00f6d7a8abf3c6cfc2" },
                { "eu", "2e2f57c687cdb6880bac6c4c39671e326fe4a3187d543b7a5a2205fa6e40850266f2aaa0450b65c004d37b591368bbe3d8f6d1cf7d687f4bbf131add2a9bd7b9" },
                { "fa", "e5d784752338d7616717c636ea3d7f9ce05f5310f359351649ae4409926b946aeb5162f6b9695b09b657c56ec1977c7e26988201094b91fb37d32e6aaa0a1663" },
                { "ff", "0ad987e4e9389661c35872eac4848a40455c891288979fa052bb5521786173c1d100b73c0747f61a491def2e53152f8cc6bff1114a0047f64a5522ad40a68563" },
                { "fi", "82590943c8780d08693ad782de940a544a61b0d5373e77cd475965de6f6b6c0e64724917aa4310bc2a68d21e28655606d2726dd73cf4f506a3347f342b67bca1" },
                { "fr", "7442432e9457af9d04abf2ea26fc26418c6e691f6555268fc037dc38aa7fb12664747c41a8722619f5d3cb4fea6d42271aab55f6b86ae17de0fe4c1f1005a944" },
                { "fur", "aceee8e2fae5a6d3992612640c4917d5783be3bb0602dfd71b65b50d48b3d2fd72d909a2b40ef86605c406299b69c37e7a3ffb46279aa320f64d6603330b1d26" },
                { "fy-NL", "e5220a437130538d1061d08154a2c5badd38bd7db19e03ef307b388c95a0fa574494562bebfa1715d413b267249553320522a4da223fc57c4df091c2287b0bc5" },
                { "ga-IE", "71360a0a52644b255dbc5025375ff090f993a4793f01609dee48933ff7c9064676c39068aee071677aa8e9808d710de622e3cfa4ef7111a7ffc60d1c0fa5f6c2" },
                { "gd", "a300dc74a9e43f5c58d591ef47e2c5bf8ed26065b1a52001925b45e65e7f918e7bd7f8acea7740b48c8536a2eb87ef035cd6721b9fa26f62cf671efaa6ac988d" },
                { "gl", "eb17f6d5eaaa8334157f6114b4a7b9957567bb55cde38366c5c3860c94c3f452444fe0a79259fd45fa3754f08bac3d3f7e9e37a92deb939baa350e5907a5fa16" },
                { "gn", "3a73595318ca9d32cb8d5969467097c52c08c28e250867467304bb750b578748b42c9908dc180fc53ef6406bdad43cd748abdb381baf043f82861aa4dcf08f4b" },
                { "gu-IN", "8fbcbc0457998dccc9d711d375d0696143c24d93c9bad7e653e26b610be76d639eb9c40392741ba78a2557d0a84cb57dadf5c1347ffaf59b991896edfaa44f5a" },
                { "he", "236c6c050414a7982f5c7dcc47ac3b83c7dd5913575f8e9035c3b0d81e4b3efd01f6e2d4e85847ba377e29caaf07cb2a6a701e3fd3391cd3475d65d8cceaf039" },
                { "hi-IN", "ea0369fc9b82dc850a4a36c950c5f025168c810475e6e4cd00e06fdaca597a8b644d933280277c9bd36082c8ce11bff4cb17881e6049d5fbba9332546933d867" },
                { "hr", "f1bd4d7e1e86f37d73aec5e6beb70a3f12756b2de4b3f1981d97f56b569e7ee7ac39c5e4cd79e6dd08e50b018ff9daaa2dce1097e5e73aadc42cd7e47ef788bd" },
                { "hsb", "f24a0e8562144a468c808e7796fd2c15bcb1304f8b3c7572612d46bb054e975d319871aefd468fd7e09644b2b5d19324e6cad2327705208eeab0d03c88f67987" },
                { "hu", "7e87e0fdc1a1093f354bb44013b8e0ce4f9a1718fa750228b0f1e619f88ce17d308b452968db359653d9d63949aef1ac106b3e6d0ef6c618e09f2799372a7fe6" },
                { "hy-AM", "8cdd3224ca610456bbf4f1d1502e0ce3016e1bbe1e66a4f9e603c3babe38a356a4f237981616faf478322dc673722681067cdfc565264f950e9f9311d32410c9" },
                { "ia", "5ee644c89a520b2f4f05d18f0d5355fef55caab61692fb99eb63a2cb9005663889fb92b620a60a2148d64c012440e2a50067bfcc7969c0366529a06ef2a7454e" },
                { "id", "ce5d18706e3c3fc6d27abfa3d4f24ab567c4e6b1253e6cf18f498eb47bebf2321d0723d3fcb3f8a1d0511f967326d8fbf4b6be61cd38200c0f1f6c139d6f6a77" },
                { "is", "a1bdcb5648564bfadd680b9dfbbf3099485f44966be94d7a8d119fc2d0d64047029e123b15d4c15d9484bb62a6a6eb11d3b9d89b984079460cd5abc9c684f150" },
                { "it", "ed5da9b61d0437cf4c53f831188548cb8920b318afce492053f1f7e97e50423c3fca2e629f9b5e6035942560c33e682fdb366562078e17f893e74a6645559496" },
                { "ja", "2d8b79af5530a723bdd9a212cc043f08242958785d1e69c7c4c3156a69034bf40a8ca104934c4cac4d4671bc691336311b2a26cbd578daecbc975001fa71f4c2" },
                { "ka", "31df9b8ea455d631585b5250f47fdec62737ee86a07d00a9aa3c336367786fc86f789352df000be0516f0e4db25c65061ccbf7504377d5183a53c6d64caf6900" },
                { "kab", "9c266992483d08ff4c95b9c00a72c615ab272a9a57585033f02472703da2ab612e6348ef3e75e579cbe042063e7d2aa9996197f7bc1264515ea45dea544d8872" },
                { "kk", "177970ac620d7ef56261076cac16b61b838b5b85ba38d12d21c83845866d3b1e9d65e69ed9477546cdfbf113438697ef51b8a11bfbb42d625bf3236141a2e75c" },
                { "km", "b62e8f1e4451e5464d907478375c20f090dbde3962a153a28836696b294320dff7821d701b325c9eabf993ac77c248833c5b95211e782148e71e31282c1c65b4" },
                { "kn", "cacc9307d0ecd7a15a15e747af1ceca2e5802824dc3a9f4f75c1aa6c9376ae47dff531eee2d62f5fecd296a2a24970a3ec1b24522034979ffb797159e36c0d4a" },
                { "ko", "5d7349badd035c99e6a652095f987935fe38f9c3ab57dedeef24a91e8d945b145bfdebba5cfa7576b4cdf65a710c1bfa30e1b50e5e2c8b22cce7c28cc6eb6f7a" },
                { "lij", "efa0a37f33ef26b5a89d39626b20b25e1e0f37603f9be000a7672f2e4344747b7deeef63051d5bd1502b62c6be5dd8c1e0f8d5867e75fd98c540478aff448242" },
                { "lt", "62985897ad121f396eecdd000a7662fd8d1197488946ee82913a137010c8df8b87d492937f7f9b45574adc69ae5c035542c25596aa4caa5eebd050813e115dc4" },
                { "lv", "164f737b4b1c728b9872ffac2f58ed9138b419542c0407fe39666bb2f37aa605de7a10a411d335044517e9d25ddb994537b4f9793f81e4a9f1e8340f350bc3e5" },
                { "mk", "600a137b8751c619065e0a62237f56ae22ba46b08242f60dd053985f558ef130356104dfbd6d6e0708c6230276306c7941bb2833f060f8a4d7fd0a7a35e09888" },
                { "mr", "f918d4ba98ec3d46ca4a7fd382cc0a73c6914cbfb999a002e781505e11aae5012c64348113b53bf7b90ffb3607e77eb628ff2d0d4cd047654f09beeb85afcf87" },
                { "ms", "ef5476a3147e9fb3baf59cd1ef65fc793936173c8853328066bcea3716b0fceed8b9dc72464ad6d5a3c8e0924826da11a9568ea6c5d6bf2c5ba876ef5601f837" },
                { "my", "4d75821322faaf9e84027f89e841a066d29a7eb2334d7017c687a8b208c167c2ae401bd31c7d568a935c4fb5e9679052eeb5bb5ddf8ca947d4e24ecd6fdcd6d0" },
                { "nb-NO", "bf23033e3e4743c3adf1a37620d64993f672a0c91c8071b66a517555cffee4e0b20fa10681195397a7af5405b46d270c85db6eb99da2904b3e14209b150e4264" },
                { "ne-NP", "3b95d04876a8fb6f83860f17d89c0c053cf144cc27e0f0683525f0d21032ed69298e40cc6ff21b5caebb548dc1fa15553607ad31d102a3ede9d444cacc9eb648" },
                { "nl", "3a202d675ba5190b07668ec4d3b30624d745ce122f5f147037bd75ca5c06a8424a4e3e4627cce714585d5196273444c79c048d9f2af70bc781d82ef39b5f1aba" },
                { "nn-NO", "5c318b82e3a81c22ca8801fd1ef4596c3911893041d58a315c49a31a389d8f971e1ba9561c6da3c303817a6bea33f1c1864158d289aff95f5a6cb0b149ba2a4a" },
                { "oc", "1ce02cc87ea13420d62d0d9e1166108c62b071b40700d2d7a622b0a80827a1884d2e1e655cbb1e5847cacffb175af6fac441b7645c140af9134c9febe575d598" },
                { "pa-IN", "86f76c27239700d3fe5ee72d974bd7db7fcd6cfe15657d227cd769490434172e707f53b7041ed6f6ed6f0a17603a222cf0d53355c3d19ab127caec64e9159811" },
                { "pl", "3bf1fc6eb0aff1d384f6a29b07e414847ab6d829bbbb2823230d2d03ea478098e8ef942686b0120f1af4e376a70e70d5232a6e31ac81bf557bf815b4efb3f254" },
                { "pt-BR", "057cba3d3b7a4f0c886aa0079bfeffa76de9fa09a54cd05629ac673dee1463791dbe2a9f99e25c75ba411652cbb643bec6a15cdad9d2963b27e51b390917c9bb" },
                { "pt-PT", "9b283694a29810a4df919784da080057f1e520e36d74a8fa338bebbe9edaa2d7ddec26ccc16518bde1ea840bc097e68c9feb02f3d5c8d099b6d505e7dde193a2" },
                { "rm", "321a0244a4acb1d8b7d5f4bf6ac22ae90d4eb76ad2a64523223f930006344e29d718b1105d7e65db004d4ec63bdcca1a3a68fd81e5146e4c609069046a491a4d" },
                { "ro", "418e1d808c8e086e8c3b74d3e3e14dcebf5dd38d91b577b2d419197565af63cc6cbb3f5fcbc6f0e8ea789d54de811e0c91de41212c1bbae4caaa271feb033c93" },
                { "ru", "8e62cf4b94eb3ff0df39833a0d3a4b0e09c841df8ae769eb071394b0c9174616058c7ddd90c75a04423e952dc67ebaf9912b3c14fda46755b69900b934c5e8d5" },
                { "sat", "e931392d4859343f4729ba2ef9be0060736b706237718fd9f90f67fd7f6b20e3eb641e641e6a0ede1b7a217883da9b21861b2cd171f142d04279e6a5b23fa80c" },
                { "sc", "bd73c7d3cf1f395a0efe7c3b14d5de08de2dcb9cb5ca2bdbaf1e9f6fd570ca11f93396686339f8547ad7f907161b3d87e6423e335ede6d953f213b039cb898bb" },
                { "sco", "cc32835def79bfbf32576fc48b3e31abd8fbcafb5f3e22e95d60abf3aa43915955ca4b3483ac93aa24f833cae0e55b52fb401a9e5dd1e3508081fb0199db5e97" },
                { "si", "99d7e95a061a946e172e1cc3a844ecbb7f5b90189a798c0c9fc85ba0544b97747cb3304fa4ad7f1eca289f3b51db1cc8639a72a1c92fee83e4559126edc605d4" },
                { "sk", "ce52eeb0cbfca5c4debf6d18f8232d76fc1d5ab5a9ca9d2c3e411a5c2945d7cf1905d0a261a9f30d9cef17dd4d9f80d6cbe128bf087d6b8fd239d4d45a7ead00" },
                { "sl", "d08f7ba1db96dadfb3ef4f750c64b8cde2e23d81c060238605e7f1700ec67ea62531e3967bf2a452a049f7cb2815a5a8956072ff979f62ba1f48b8da3c2cf2a3" },
                { "son", "d57df3dd6ab7a5b02edd6a49a189be7640f9f2659e667c3a5005694607e7619c83a072ed774e745990e21b490366c2f4f7f9d00414e8dc1bababa26914962118" },
                { "sq", "855b94e06d4c19222b1c11279ceb58d43149dd696911f4ea7fd1cdaca5c593d8a08bdbbec486b03e17f9358901499d94f99a980cbae45da6da5b3ab029574ea2" },
                { "sr", "99f34f75fdbe2943e8482dda1ec10cae9523e2df8a44ea6caed30ef26b2b213d53cd6a5cb15208b925cf18571a6ebf62ac453ddf13093c2df168b72472850f7a" },
                { "sv-SE", "4df90573e741a6d407a65cbf0e1e706244ae372a3b72d70e360b38d8045bc7285ca6d1642e79c1a67e95343e67cf6492944c33310ebdee1baac7387f939c4c7e" },
                { "szl", "74eb949dd410bc136d1e8f73e1a7a0738018df2230f06ec845d65e290072904056013bf3f5778fc82c3d66a3c2c2fcd89424a9c719ff67feee4deb54892a1da3" },
                { "ta", "1ff866242b7e47dec638329ddbeed9cda3e9e097b1d713a3da78f0619ec7da66916ee60a0a7b8401000d092279ba41933f041f28d60ef8bec0293f058d2b4f8d" },
                { "te", "c601ce3f1e2c575635b2dab7c81aae1adacedc58bf2d1dfe74281e7d43e27316ec2c0c625032c9269290f849c66762d5a48a795559d1cd72cb726dacfdcf738f" },
                { "tg", "e2f900ee8732fcd96b9aeb04c76abf82139fbbf70d32baf3c2067a5b25adb504fb05ff15fc2c62c7984b1dfdb670b56aea0cfc25d33b4f0123d9dedd014bf56a" },
                { "th", "4b096459a52c8ef32c0950891eb4d5d47dbbcb8b939ba7708f8f5b8a52b669cb5bfafa3dd5c4d00dfc3a95106651483b12ec9eaecca785e6e7254d6eecdab2e0" },
                { "tl", "231df65a912cbc37a9db34fe8d3332298bcf745ac517737dff53f0271a604f47da3d3eb03ee5bdccbec862abe857269512a89bb84e8edc3e58d1b08c302c8c1c" },
                { "tr", "724b5a39c9612120cd2f61c346655a0e8a55706e6b806e14762c3f1d04ed69f6bfab8d6f2be77d76517871d8633b410d4cd4b5da4dbb6e492112d5fecec4d609" },
                { "trs", "53e5f01e2137b9efc49189c0cc94e21bf08d1db5b7cf85d8643bd0fc2552551a7e434c1522a1d3a6cc6830c2ed1ce4bb80cf4460869ec6c39267bbbd9c4e30bb" },
                { "uk", "ea279cf4d417f6b199f1f46983aea18961c1d7ae27cd52ae09bf8f24e1c3912d2afbebf101a8e62106fbf94b4cc93b698afb31bd139a4655dcb160f8ff00f143" },
                { "ur", "676fdda4841ab06f8165ee22eeefcd3e6fb7eec2e8a32a12b27c3cc7368a624cc7bf6c649fa8d5e83cc16794c0492092313371d0cb3fe83641d2348bc11ca8a3" },
                { "uz", "0c2d2289c316c3b0cdcde6f92a68f78621c83911ad28ab28c0a3f37efa2fd0962b13cb0a162ebdd7d20f66caf3df5bbf117d1fbd52e6319a72a377aa7811c224" },
                { "vi", "a768ce336fc198caab149df75250ece2c2552227df8467a58d55f3a41d119d3b0c54d092a352db1cd3403b3bf87939868bc201f834d929f04b6bd5f57617d9cf" },
                { "xh", "69133319018d9787c4cb913a4c5fad7b87cb8d7a4fc142bdfbf2635a51f3b83f66d9bb3cd1cecc32f66fa61914b4dd709be5cbb5b581c77d4a48eaa91d3c91c3" },
                { "zh-CN", "4e0ead7c74b4664469d6c45f7223e9a32f7915dd6ec6ce8c58c2ccfb7281556e426a91c9192eb929c569d5463a5cb7b86ec4cac7ac7b71762af965b0c179caf2" },
                { "zh-TW", "adecfb48ac577e5696657347be9f535459408c424812131bb53bcb51c44b32a56b1560b89544b7c84f15b639936c2f2745f6811eb55ad6c2d62d2cb828b0374f" }
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
            const string knownVersion = "122.0";
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Mozilla Firefox (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
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
        /// <returns>Returns a string array containing the checksums for 32 bit and 64 bit (in that order), if successful.
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

            // look for line with the correct language code and version for 32 bit
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return new string[] { matchChecksum32Bit.Value[..128], matchChecksum64Bit.Value[..128] };
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
            logger.Info("Searcing for newer version of Firefox...");
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
        /// checksum for the 32 bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private readonly string checksum64Bit;
    } // class
} // namespace
