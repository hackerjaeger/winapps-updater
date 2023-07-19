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
        private const string currentVersion = "116.0b7";

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
            // https://ftp.mozilla.org/pub/devedition/releases/116.0b7/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "cf6260f65fa130167478b269cda47168c6ea672e9ea9e12b8b1127af624bbdc7d21b74ed57256a5e0813463d84905a300e2f44cdc8b780ef239cf22e72692977" },
                { "af", "6f3350ea7d7cc46e42f1bd9616f422a985ceff46d88d03ddd9f689e6e5a233de467da5370ba4005bbfc42e25c406ea37bafc9e4a4ef1c3539e8edaf0a4b60253" },
                { "an", "c1755f76da4b668438345e155df0dd753a7de00ac0354d5a2787980316f9439fdeebd255f7aae5eab10470235e7fba9fcc7eb58e4b9cf27754ec249f932ae8ff" },
                { "ar", "5e1b7eff90bb5714ade0ac028ff2b05930e71875c7188c45b0dcd5b2fa773f7b996bc51852430acfdb8b7b1d315b35b98725cfaa4aa1ce17970f1fa62c9fd8a9" },
                { "ast", "1df47f1deb8a4deac3b0eec76fdd4a7f73e651ed9c9274b953206d3ebc53c228207c3bc549a867c190d15cf8a48b5c82d3bcd79cf33a274ca13e25fe5b51f3c0" },
                { "az", "5801621d781471911adba9ba4b001e37df7bded6abf68b0f724949b6af18347a86e2d05df3d68e86279030bfdc6a698b9b586887ae1903d89f4d04088f2a02dc" },
                { "be", "95358392466c2e708c153d70b50d05bf20f70b0f35f2ce1a59363427c01b9d645b07d0e1a16cd9742f5fa4d4e709a5280f2ab6ced1f7b15dd7b4a1c54f09d9f0" },
                { "bg", "1797b09a282d87961250ed55a10d3cd96dca39401cf8771d648ac0597b6d6a401d26f98c740d252192d6ceef15f64c3ab3a2811c5513ef1df46aa815412bfff5" },
                { "bn", "ca82ffc9e098f74a6d0b07279ee6ce8244947cd3c778d842be14ae6329776062e08f9e418d8a87cdf7d1613af1f578e08efaa148c8b157bc427bce4f726d37c9" },
                { "br", "66c59edbe3eae552657b4143bd9ca93b085c103f45d3b35241624202611edbfaab2270b91f215fd5dac6ee105a4c62804ca9f5d3f0fa55c1fa95be1fa90b930f" },
                { "bs", "532f0af08fc6c641ef7087e74e23873028a8715a32e8205f8fef0be5727f0d14222691ab580fc39e61eced6e5548f650c3ba3cd19fee8ce995c732ec1dc5037f" },
                { "ca", "c819325255fd7c9641b31ec3f25e85bc959d888d4303eb4b0006121f2e7990554482ad3d2d38e0ed3768f0f845ce48bbf0182fe9613dddca81ca4eeddac6fd12" },
                { "cak", "4ef718244a73e9903b605be5da49773a884a9800251409aca0bb5a5160b84f48b2930659f56dd2f5f83fbbe649a3174fd68e61cd4408528753f20a084f5bdcba" },
                { "cs", "842988a4bd59275f7c8bde5f68a7aefd5845c974c48fd5cc41ecb63308d61a7775d22ca4b8ed5176792a29dd694f71ea0885b67dca0672895cc2c4a98b734f88" },
                { "cy", "853342a3757d1870436be62d1d13929db1d1dbd50521057bb09aa57108761249f0daf7d1c6c15e28c7258ccd555988cf3f0cbe63f94f83677aed274d78803627" },
                { "da", "a04e4c8d07d9fbdfcfa406e53acbd2a363ad0b8e6bfeb614e03cf0baab49dddb9ada5f2811cd3e5edecfcdfab115b993c1e7b6b755bfd6e4e14c578e8af14551" },
                { "de", "df8e0cc866bf1c87492695bf7638b8493670c90a098481a1a3643a67eeddc5698e9623b3dca06a0a4b5edd294f7ee4dbf138a75c422fe7b214c9ffb2cd855ede" },
                { "dsb", "22606e05c6a52ae48017069b6cb82ee600409388abe910368a3511806c7c9f19422e79a9e84ce674c555d55b15419ae4ed6091d573cdc95cad27214cb393f58d" },
                { "el", "13b1908de14b202a896d881fa8b147fde11dba76f9c6a8686e52f582c49bc3c5a884b2b7c67d2ef668c78fee1e424bd8eea8a8b9f3ae32857620b21708d6d537" },
                { "en-CA", "373463c7aade52bbc1f19d2178433eabaa67805efda8ab2f45691c48cffb40575fc349df71724c6d20826bcaaa5ca8a3cb968f9dfa6b69f14959f09242da0ca5" },
                { "en-GB", "fc7f952fdd4574a26cc829964d14d5cb887271051aacbfbbd0d037b76a70d83cd1c4d60f45b5efcdf6f1b3bd1fe78b9880402d72fd93f48212af24b031798423" },
                { "en-US", "a21e9795b0c266c52da26fa0ff0c11874cd8350f120d3263806d8c378d38c77ec80100bce800cccda3895ca3d1c2c30b2fd9628521f9560936f9fbc50b5cc2a0" },
                { "eo", "df80505528645bdc8429991af9b90d1ec8613e2007516d75785df17e9a9dd75c78f56b5ed297718c469573c7e99cc053da9ffbd4e091b972840e5a33a197fb44" },
                { "es-AR", "28e1804d1baf9eaa052e116f4b6fd7e63576a8adec05030a67ef7a9a3552729ab1788f631155183f21d46cfe091dc9831a0a0417592a18c4c328940a8bdce36b" },
                { "es-CL", "deaa890d90a07f4ce66116078856d8ab801a109520ff1d51b94fbc26b614fb6af71575f667d318a2ed1a8f3f71e3b3eeac1c21e07f8f7a3d66c6ac89aa7de721" },
                { "es-ES", "4069dfd9a2a9cc87ac4822ce9526d4e89ac8658ae38b93adc62e5362979cbde1e46a176fc725c30a70ef495c90abab06c3561c16df70da68994dc2d3a4d11890" },
                { "es-MX", "27e4dc1933510b9aee5113fb0eec4845aaa763ba40a88ae7509c93e5adb10cc9d617c7491b390d151fbabdb40aa81557f4fe5a281c0986ef84336292bc79544f" },
                { "et", "bf97b012b5bec67eaa09ab7161031f79ab7b45a1b7ab635aea98d26c4e6578d8acce7a46e216de750f208de1053371b244b81a7737504a3c861c42ec925a1d70" },
                { "eu", "221f6bfb1a92d4e35a8b731a898699928ee3b93de1e24eb022b59234ea085e6a8878a266f9f7be507dbf5ce873c61ee9854488f3512a25bc51a3c95839ea666b" },
                { "fa", "4f1e3b0b4fb2b3f63fea3e4de3248b25d16be1afe1aafe669a87d07f256602bc1db9baa15354c7b35055b37106e6675ec7baf0bdc9f60cacab1c292b9a5a48f6" },
                { "ff", "71e7fc0fd38263c98f2d6ad3d2340d71bdbb052aac828fea852589cfd8cad4faa949ee7fe31a8c79cd36f65ec062be3359eb7eaabb19db405272ab5aaf727af8" },
                { "fi", "fa5cc88606645710e6af07813ddd7dbfb5a6a3cf77c0709c1cf8b7ad9b61ba89530cad4f47b55c938adff2895e5a792e96dfea600b5b402c3966fa0e3278060d" },
                { "fr", "8a423504fc28558e87fa6576ebad433bc96731f6ee29a5e5adea3965c0cf40001e1aa1e2264ec8a9d4a30a05b186044ae4ad9af7490b4311f14445c228e1792a" },
                { "fur", "d9a53dc6852211e14a7aac9a90e277aae92ff0fab96da68b5c2747103888de55a512e7ee7c220f43ef3ed5b8c532cffa9c425b2034ecc4b4496b7f9669645b13" },
                { "fy-NL", "c7fb3debc2caa0074fdea0e1c797449c6efe4fdc7e6adea5b7b35af8eaf30b7089defe2f27e4006871a73d1477fc23dcf025e23a35beedb00a020d1ab71a088f" },
                { "ga-IE", "8d0d5ca2ba81c693249555d063574f55cdd468f401e23043f59bab2b9f1dca56c82469f8fa7f38a15554c9dc549754ccf8b889fe8ecd5003405b5b4e2b3db4de" },
                { "gd", "b6881fcbd995c31a8aff99a4638a4fdabf7c8c5190dc9de345f539e2f2e8ae1378bd995d0bb375fc905d5e1d4487ed5cd16b47bfb304d821b3ebd207056ed278" },
                { "gl", "f7ce3081aaca952e1079995e44aa31382791a8eb1d18b61db9e739a7d82d3db054c027fd0976b7932ec1d65cc7f214550f5c478da0f594e08bf5a0c89e487f4c" },
                { "gn", "3b0f93fcf8ac4a797cb73777d40224b500a7038665b99c2fbc32f02718447ffb1dc2e8a3ef6a039e734d89c85b1f0d49398208cbce6e4a7a7839094fd33289bc" },
                { "gu-IN", "c1acddb059a5c7e1cd18619625b44e0203a26a6d9c48031063f100c6b8b85e7f32d82908fd560cfd67d5cadf9a71fab7e231d9c7c55b97713afc7593cef2eed6" },
                { "he", "78cab381017249b8a021318cfc351fe142bc041799171a425179855cba05cbe381f8187980561eb0768d55d36188d52b4f621868f53f723051cf139374789313" },
                { "hi-IN", "25bf7fecc9146db06df8526d11cd6481c02aa50c2e40cad984b3d596d23d33555c67cc64d196f4d338a1d0ce16c9bcb36dc2db96f5873247052248a05a4c04cd" },
                { "hr", "16dcc3d65defc91ceb2bbb6d9b48e59a1acb00e4babcc91ccfa8cf7541194cee7f73181d3b0e7437358fa820f72ac8befd0b80e3e6d1baae2fcd2a2f13c632ea" },
                { "hsb", "8a80a4c3efa87038e98edb5bf3868aeb19da327c9c34883873b7fbbaad1ca98e75ec6417e50e4dc3ec34c774e9ed73c305fb048f2f489e3172997ceccb1fcba2" },
                { "hu", "428ad3b522c98679afccac3fca3888e7378ab03c7330682eb228b471081c32f665f22c988e93dfa75aedb8bbef6d3a01d9c392c4cf379bff90f5e4b5d3702cd9" },
                { "hy-AM", "412b8defc0453c77bcc01807911e9d236c1ae1cef7b14951da333b480c33fa5dd28b9f50abcbf55512859cf61e7099e65847a0e30796c833de8f60a2b6d9f739" },
                { "ia", "c8e5aef7cdae7f91acb4931510f39c644cde84c37632eefc53617ed8678c89e2e0569f3398dc684afd041391bc16fb99ae88171dce436de86043b0b54318519b" },
                { "id", "92ee5640af8ef026459bca1bb11d2f777ad3c959764367819042db5051414134143dea6941a33116a15cc77b80e387f496bd72abf1919392d8d8d893def9085e" },
                { "is", "2983d1ff53035c3d4af5d699600e7c0c683fea98510013e9a30f80e6f7675b9d63f57bbe0022f9bf48ebef399980ff7cfe0016dfbeb1b20ce97fcce3b0e8691d" },
                { "it", "dbdb68ba382ebc7f4c3d95efb3ad6149de2f394891c221ba5bdfd6092bd608e1c9aa35d110ba33c8ec5ac3522d790dd0c19a2213cad52367bbec4d424d73e0a2" },
                { "ja", "e994f56af4d2449431845b1dda98d231d5a9df5e5722a4041c775caa9cdc59aecd98422d7b9be050396a3de54a0e146612324e9c07fd19f7cf9189140c0819ff" },
                { "ka", "5c2c0bffd1451246c5c17a06ac08fdf8ae6e2973f0c942ae0b6440ebbf8600c37dc9d99feebc3ef545a283eea891c8bab63aecc7fd4b79e2d3db409c9ade08d9" },
                { "kab", "5a8ffa1a92da611399e43431c550575cb90e3cde6787a7700aa35498d773054d833a19f58894bf06308d610b189fb086c58089a2004606e29ff021fb4e7037eb" },
                { "kk", "7263acde77889083f2b6ae2f9fde78a3a851049c808de1bf4046a3d2995f22ce2369a2f2793e1808146a03a80bb983341b112e5ca61d57d2f51d2f052d8a9c9f" },
                { "km", "9df2c67324c8abaa2eb872028df62ef7e1a61bd0927f799d99f1d155f4ce07679e90ad3d492378a122dc69f75e8ce37bf2a941a10cd75c3084d501fd67b4d430" },
                { "kn", "8da3ea64ffd01ae8c18a0dc6707d7d01084f0e1109599416d5c592b5e1d00cec12a7d7f67c0129a21d5f9ec221e9b601db28c2a857194b0d9c269b03127f9832" },
                { "ko", "ca68090464a4c6ab516a6a13f02d5d67263eebfbb99d3569080d09afa969ac6732fdd09ecb049a07348b7f94b7d1141aa12c028f0c2e86026ebf2279341dc2c7" },
                { "lij", "ffa2850ec8399572ab6dac97c59f6aaa47316b81a0902637d0b8696091f5224545de3ea4245e25322d53e1fc64ffb2f441c4d97f77fcb31bba1b749023a45ddd" },
                { "lt", "257d3ab68586ea1fa8f2530c5d1595be2fb8c679de8e06762ca4d1cd6c8d303707b948bd86b379f11859229a15d9855db41d57bf1cf27451b1f511335d5e6c06" },
                { "lv", "12eed178fe2b920eb6a8691a20b3b29d17ba2437eaa805954a39f34fda8cdd55f36968a4861408dbcc7805b8e8ebe412043e080c894efd2574156732aa909ca5" },
                { "mk", "f1b36976a5c40808caff95779206db0285c802448fcc0392facf16c0f20f904de718a2352780e8fcb787fc1ed87d44171536c732cf466acf5d85cd169a6f6c51" },
                { "mr", "454eb33ba21aa75222a8b4d9e66e8fbb0ef5eda0174d3e194a2c0db8b285d69257ce147111c4442b2022d504ad0b0e9aa86dab8cd67075672860db9e07e4ac54" },
                { "ms", "9125c1aeba367ca7c18db3c7280de0b4cf1a4999ec4e29268bcc837acbf6eca33591be3ef112dce81d48baf0d6dbca3ab77df0956f7c4d120eccdc9c972de289" },
                { "my", "d004094c6e392adf44bd010a0b1ff5c05911098b77e2fb7621d5c897638c5684ad6c8617a1fe3d5f2820be83524d2e875a9e416bb65ff35faf6c8dcf6e6b9740" },
                { "nb-NO", "a548ad8d3c398fb5ea89525ff8c5b5ddc3577ce5b0d00f4ab2b317d600d15aadb2f5cfa80a6022ba46e7950430b773082e30290f20532a9c7a42b990bd2399e0" },
                { "ne-NP", "09c61f103d5fbfd60abb71bbc8a42428b21bef084846b8a3ace53ecac61fa1dca899cc6f1cb0603349fa42ae842c10d3783a773afdf632c1ead86c800fe6d58f" },
                { "nl", "82196e01500c5bdee3c6ca7af145da57b3cea5515d07076c077cc9f1071b77e73533078a44cae7af0fe921b43af04df20df011c10bbe2bf7761f826b83b28efe" },
                { "nn-NO", "8d62430948f179ee0b81aa40cbe2392193ce530f12954c8d339b9a0faf900d1a573b991d0f91a3b44622e55cbd38e724f8a240bdf1464f83005e7443527ef679" },
                { "oc", "2c2fe0ac68bc0e21bdede33d58875b10549ae805e5456ea6b66c047fa6ac2c7ad4b4a8d241476cecc27ceb575fb84dc5a456825399db50d37484f755e9fbd6b5" },
                { "pa-IN", "c7d13e8ffca0095977abb1560342b40922d354b5f09a9a8ec4e1146173c31754f38895ec630313c34e2966705c4e591b28f156388489341c850c41e3cdeb9fe0" },
                { "pl", "0e3be1e147da6d8c2517414ab740a2e81ad69bc9781ff9789169177912581cfa471263bbbeada939e3fabfd4dba1af4575e6ed8ef19d6aeebe898863e6aec36e" },
                { "pt-BR", "0d0c9276af2246e050598b274c6a245e89820eea99a1f1798ddd9eefdd9ba98c252ce9f9aa6334b597937ba1098dc6280d27bd68237aa65eb1e716cb59069e38" },
                { "pt-PT", "b6cc8a555c0d666efd1accff13754e157808fa9412e937024313e27dfd6325f92c2c0f11322db0b39e59aa1cb93521276693235cbacecc5aaacf50d845e2a9ae" },
                { "rm", "c75c92bc044cc191f1d9bec61ed58d96a87f53f943b4aaec4c26333032c59156f24e91bb06cc2b783cecbfd68cc6b15666ee6773fddff79b65a58814dff0a213" },
                { "ro", "4dc30d2878b4c7b92fa00e62ace0bbc32582a4da97b9c3f8026b803df2cc8194c525341a805454887ed6d9c0b38c80719feaed3092453b1d74a37d995e7b9e3a" },
                { "ru", "50d6125fff6fcac98ed118d3a66267160a5c7c5d00340a2981c80b1e0fc0de763d698e9283fb1664fba627ee61daeec920d48fe994a8b175f5f4071cef89334a" },
                { "sc", "e1d073ed88783c83b64ec2ebd6596cc72b1b8884a5fb76acbe04b7ca4459f1e8533b7eb389d4fa3582458d1d996581c3e5b38f02bb544c0e23e3f3fcb27e27fa" },
                { "sco", "99aad901cccf2c2e3fbcf8fe024b66839ccdf387f03d94052d251d78a2df7e530083c7fb50a2d5a7e4a8739e1dea2178d3c9bf87bef7baf6f229e4cef01cbb49" },
                { "si", "a5464b49d6b0ec925ca623041bd6e3ffaa235d6be41584a53190689b33a43aa211c59d77cfa9d7575ac76eefad0bec24fc97f8c8bd2cb40dae3c17b867de3e32" },
                { "sk", "bb6e8c4b59a5be421b718bc2a6b67d84d83ec7a50957012b4b13d158e70867db003ce3891a53f733b6476acd4d45d68001f607a2d4400bbd1e84cfea9256735d" },
                { "sl", "3f6707c621fb4f811b769de54f73e7d9cfd606716c43cb49ea36e624734c0c2cbcc40a19817cf84607b4ae526024136ee39029e6d34be415b770ccfa12d2857f" },
                { "son", "29bc47ce4b95d0817f1bf385c685fc68dab9d39d65661b623bb10f9bd663cdaf5ff87d17c9b3af1ab5a98356ddb8a10920bc298ec7328982e6faeddde1f0cafc" },
                { "sq", "85e25ff8ea4a8e821d12e6434f6a89527f14ca83260e663f2d4a560943c93fddb3415f0f36f05f2e934ef79a4536974a5a9ddbcea3fd2e67fbb2d2f52c21f651" },
                { "sr", "6b79984ef2ee6e4441058b98171d15899b8ea158c440ad2969e618fe9c839d4674b177b4b2c0947d507a1704b8e102687080ececb2e49718b7f52592cad97eb3" },
                { "sv-SE", "a907ba6f1789c2d1f0471ae2391cb56a2a50d0669c6ffa8676f093e9c5b7c187cc924f50f89165b4bf6046c81aaf3f657dd2d619eb084f91a718c35a1ea5254a" },
                { "szl", "7d1f087b0ecfe377a9617b42ee5845140938d5f08309791e297d622d2cb59cd2a22cf4025fae820503869d9969dc57be07058a1cb5eb5f92645addbb12f16d12" },
                { "ta", "fd71b4cdd1065ae3cdce6b4c3bd6a4fa311bee903180b5ad84943231c8c9512a9a01fba10a3d4b94554b9f6e4bf7f3a5dd3f2b4dc70142d67e806f6aed88f566" },
                { "te", "1957d9156e4e2690de33bb84c2ae7ee8b1f0637c10eed82c61b580f5e3bda52326cc4b0710a2c9aac34577afdc17e22aabb0bf9feda5b889e72838cb09dbf1a2" },
                { "tg", "b55f8f10ddfbbd2444eb7fe7d15d54477d66233684c2016c83e63463689c02d13967fd6b086eae25de5fc726f3b843639c91686d7e605825b2062b0379c5eef9" },
                { "th", "f9fee0207a9ed24f0787ab48d456ef1b2808f40751999b2bf32b61aef7291ddbd4f64418dd8203890867344ed56a2aa755de1fe25702167a7eb4f27f944f3391" },
                { "tl", "db82d9d722df1c567f58890f933692ab63529454a95c9420761b8d94a9869ad71c768a8c4daa092ff95838ee9b713396849c7b4e24f60eea1b0344b68e4d4b71" },
                { "tr", "69fd69a44916fe5f4033054443193da1b8c38e403238959956ff7df2fbad7c8affa3afb94fabb2efbf6a0908152f1dd1a14c97b19ee3a7e05e38d640e5221294" },
                { "trs", "3a91d195a363191d349e9ff4a0081edd7216baab731c49f712f57d7ebe15f6ce497fc8dff49a5cc7af46c79adae9b6fd67c795049e1c78080007dbf8cc151718" },
                { "uk", "bb2a4de427518467b4d019400221199e3301569f5bd4dc568a18cea742e27e472587bf61d994cb5d60e8c160c3762a5afb4e30dfbc31e6ffc1835f19cc501def" },
                { "ur", "8b45612a478fad4ef2e2a06766b16ec0b8ed131e73a638927d5796c5ae6d5fa19f2d615b0709c364182254079e8f0e0327303aeb037ef6ec157a4416e0d4b288" },
                { "uz", "2c62914c8ac46912af2e7a1371f9d43c43c0f71c005b007fa090b1ac4732625fe138c7be7f4759eab78933ff4aa1eb2b6ad04cae7679e9523ee4bfa7cffcbfd4" },
                { "vi", "4b32a2ac35d18463dd6826bb9a667d79285503e9e4b4cae9159abe4e04e8c4144e1bab790d58f93f8d8955b409415fa162415001d055029b55f43a149a8f8852" },
                { "xh", "edd01ca5ab1f4e5c12af61f77ce30bfd60b26a95d3c2699be5a27a80d0a7eaff38acc51d48b250e8e878ba78fd1737c9dba441f13f637a60c0e4e14ab4c96633" },
                { "zh-CN", "ab5cad7603176661ea618c5e5e3d804e0cb513e7ddb5681c9083466f11a6159a32310129627ca216e093d7dea39da0f1e6e14f9693acbba1c7805d7cd649206e" },
                { "zh-TW", "3f37190613cbad58adbcf34cf9d79ddd9b28b87a93704fcba4806732a88617e91ddb0d5532aa3a6b95034983b10ac08b4d08dbfa5eb42e150353ee2760a093a3" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/116.0b7/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "f2e182f86519284c8639f6e089d7bfbc070aea501168238f612cee4a1b919085515b17a2247ec9d833f8c71b604ec7b445d338c121ba5f8464d5b0c3d50f0b68" },
                { "af", "36c41a7e5dd3488ed037042b115875b1f16faaa5a14b7460f7bd6ee36a99bd95b6d06669ddc504b6ee1d1c745e35133cc955afa785e897221cca08386c11ffd1" },
                { "an", "79e689cb81ed702d8ac7f1197caa4a6073ad339056197fff1332d276ad98352205fe17111443f80378ebfcc5d7ed0b73cbef071a99c34a9385bcca7f98cc494b" },
                { "ar", "f31303ee30d6c1b7fbf55c6c02ed0967df720ef73877781e7ca9aa3df9b67fb9200417aa36769246bddf6ad15165e05dadbc8b87e09c434047db51551ba91d94" },
                { "ast", "c39ac5f581920ab3ee49b7261d63b3000b000db99ef5e74ed9a96f08edd482b1e215b921ca47c6c200e6fa3a185fe5c3ad2d636b9a16da7a0deb7dbe8c827221" },
                { "az", "2a0f2d5cd36476399171aacad81d5049bce017eb912228680c402bdb40d80faebc1b183ad7e973b5c72f917b00cff75cbade4498465ccb4cd49e53933933bdef" },
                { "be", "9750e1d1b462d2e847b2f2baac69abceb6d2b6fad2c7e642e1d530c4c078b858628a6a4aa6fe77c14ac5e2566eef8c203f5d2426cce787c995decef007faa2a5" },
                { "bg", "4d7194a469cefaadee6aac16f3cceefba600f0f18e582e3df7b6c1c404d07ca444564a6e13b9860cb02081acda98941e642deb03f4a419d4b741d3921c8f027e" },
                { "bn", "8517631284340cf5272f916e8005826af2c4706c372a30b8662aa3daa252cc3ce68e1ec0b054eef179de72685be8582a3a4a2804685f3a4015476535a3c067b0" },
                { "br", "64f065430c829a4cdb12ade1a2bec5e780531257fe3a61ee54b72dd3fe68520bd54ad8cf967f3e7ce4668c52958fb85049d06ab283de7a0c0b9d3ad8f28dac0f" },
                { "bs", "d122d0031d92998ed842a572a22063990403fa1607d9dcfb8699b6ac74e7312630677beceb17b71ea984b03fef66047dfe494ad63916ecfa838ab7dae853cd7d" },
                { "ca", "9fe03a0e228666346daf61a39ccf086f907548ce1902476ffafbb86b91d9a45ebf03ff4dfd50838fd9e4a0776acd42cc12e31958cceaa296751631c4b4c07e1c" },
                { "cak", "e102f999472001821f207c43ba1c98cf6bef72d9412026e52c1d627ce663d320c292259db820e7bac27c9e0550e20f6b24e3dddeca2eb7671febffa2f11c2d58" },
                { "cs", "5fae188ef8d26309cf9749e2e865d96867bb7f8fda64dc7700c030be2dd22de369ae5c038352b8d4c7a6bf764005f0d56b82a2a268448abf3afc4e317241b064" },
                { "cy", "ed9b267f8a9153adf4cddbbc12f027a04a8ef01836e0b884b57962e6f028b446fbc546bd3fba65a2e7f9dc13746a33aa6ee4fceb3519b3244f342498a3248209" },
                { "da", "d5b736baf82c23923aed7e2a40d7cce64a590aa2e707b226591c9d9d9ec4b1206acc6ad154168d508d82043c705e520c3d7492d4f1d027f89da2ef701000f5e2" },
                { "de", "156604ab9fbf4d648090b4e52449fee3e2bc514cbfcf5be52153eac78f3c9935bc1cfbffb130e7571c669d7c0631d90f8645f1db2984cd966f232ed77d639bb2" },
                { "dsb", "2ee4b57a6439dc57984cd1b3bc90e94b47689f1205e75fa91c15de840bb51e3eb2b30034b1de110a27159bd8607b5060c255bd84bf4a389e42e1a956a078706a" },
                { "el", "c7b500eedc40f45f96eb34ee7f53e1bcaeae731b3fafef35d4ea7924d138a4fca17ce81f6c9609dc7debbe3d33dbe77abccaf5bc7718770da08161e96c78bfc1" },
                { "en-CA", "44f23ace6a80c18587f6cccd949dc222eb11de44e9cbd93ca4900b5c642ca85199555ee48e6eedbe60d2497e2a0dd7a0be7fc6c3470ea1d72af74c3b562b2430" },
                { "en-GB", "8bd38839bd7995ed3807d8b08537dce2a04b67284ca4895f7cfd6d78a8a649c502d5af0a41be69069e8c2a010e3999f1ee23f19549305b7742214b535d34e5e6" },
                { "en-US", "ddcf52650c326a35718aabec29346ba6f076a3e15d9113907d470ac201992ee732b861ea95a3aeacfa0a9c54d5977bd9d085be0436487b268d90f97a7f094677" },
                { "eo", "5de18addfe6e1797bc8d004ad9e6d3b3d6fa791f3215ae7d536f18731a528daeeabc00efac431f46343751f1b5f4699845f3830b4644a619b2c8c534d800d6e6" },
                { "es-AR", "f466584ce228387900da7e6204cf973dde5a9fb6c1b81ce0e7762b9a1af9292593f63e7273055b5552227bb2c6380d22102bb0db797e0dd83a9b8cb51e8bc48b" },
                { "es-CL", "10ee6fd7ef2b9cd3cdc4d1050355155a381db9a30303ba85145cfc8afb20a1a4aea0798f30218cbf6655040289ba75de938079892050a320b510a849ea60589c" },
                { "es-ES", "42e0b0ecd35157a0f84114078ea61b5fa7176c7945a8ffd88aca859c87be531cbe3517ac88728ef16fab4e8dcfc4a5bf362ef9e6db399ab80fe2c0cdc2715b56" },
                { "es-MX", "431547581f842fb6dfbe9dae28712b38f774a85da00d8c3fcf94241b3e7e3faa658de4b85f114b0e0f489523b04a686601fdc3c38c1d561f57e2d1d1c0c89eb5" },
                { "et", "ec2540c959bc4faa0a58e2eb2d5ab53c10db6a7f55d58f5aa873678b059416d95013c75f3b3c2b42fce86912e56ea3ba1606f61494941c54d74aa87c0c08c0ae" },
                { "eu", "974fc06ab62314346aecde87975473994e8bd283fba68061f1d34411ce2e7604f6f2b7ef297ae541dcb479772d71bb738ea5001170fd9b301627509dbfee65e7" },
                { "fa", "fbbe2e10b0c01112d92bfc3a2a64a0e9df510e18f6539e0114db5a397914f495780b965722c40c221407aa63a272ad00bb1b5737e56b8e0284701163f37f7015" },
                { "ff", "4fb696f205090b9d060f428cd7fc7f6ddd697ea9851460a56ac027d3d2788986ea374467a06c05572f979f7361b8eba950889c872953c72a558b79f534a4862e" },
                { "fi", "4c905b573ca122aa9091d044469ef4fb70dc2b590b0edaa4a6939c3c4b21c73a71f05281673df6155f852794a0a9fceeb1c1c8e8de959c4374ff37aa66e4cccb" },
                { "fr", "c7e5eb419240cd89d68d7982783b05037d4f9149398c00ffe8df926224d735cd0fa4ed304d273e8342fb4840ae32013309ac2298e19dc52e00f6c30bf0bda6ec" },
                { "fur", "58ca3efbd306a34b824400e85ae2e8e38f427651e9d27eab0f3012c51340ac9d45c452b84191e62973806a8978dc4eaa6411e330e8c8b4597076ce7aec08ae01" },
                { "fy-NL", "b4050e06b0a02027142a08e7c4a0d71cbdfde87fba807f753053a5cb6ad92269b3ea80f98ca87ff1c39c4c824ec41af7f1e1f2d176d59ad40f51cf5b0b6a63cd" },
                { "ga-IE", "860d25e4e11eef16a96158b0d752de62fb0816d9d044ebd745b115c77ccccfa0e093268338d5cdcbb0a4a3103f0aa1f75209025081b79693f7b322c726d815b2" },
                { "gd", "05b8c1dedb537b59a9050469d66b5da91a80fdf2e8324261606f64911c46a985b7a41971a2340aa3743e8228f24074f07d3214ea5ebe7a38752d64088d43e18b" },
                { "gl", "55f98231ac005c59e1de790dbabbd77bffc64865380eae5e41659a27b800c07a20643e1af8878e4bc377bbdfcc65f102ec2d325446a5999b15315411351d54e5" },
                { "gn", "514d24fa6b8a180fc0f76c2a10f6c1616eddd0d4739b82053419fcc15a598b474d7057728bb18e4682657ace23d0ca1d2aae2a069891ec538090570fd367e5b6" },
                { "gu-IN", "945334cbb12f8056d3a6f712131553b52e046797e6d34df988a06fb2e611b42f6126c1c3d9323e35825412de0e6cc3ff586b16fbac0e6e674991da03a7b090a5" },
                { "he", "36703b162701164f874da779d6d05de10cf678c703c0991e9c7c6deacfc408149385a88c289381f9c6381edcefb955e147566ee458ff82e42f40a6232f949caf" },
                { "hi-IN", "eff2127995adf22176f3f1cd842beea322ccffc8bfd8988f72bf3aa55e7697026c76434670db99654ae79ca92ebf286af28690d6aa2ab8fb37d33414aaa2b105" },
                { "hr", "f64e88952710aa98389072ab00d22d81423232f21a44d7d0aab20f51c1200fe68bc47966d200410a1909662ec8964a4fa425d27593b18f2350999a1b7e3ae1aa" },
                { "hsb", "92107d3cdd97936ea5404e9b9a1703245c650f11924141cfdd160f9ec5fa20a4f1b2d2638c88ca05e3a4ae0ad2582339c6e27ea3071700e6a978267df247fa4a" },
                { "hu", "f6b4ae726cadeaebbbc3df97694c812afcf7e022e493f3395fd7929a4926cabf25c8857f1edac15bcf5d977f3118551322eee9377312e9240804bdc617e88c4a" },
                { "hy-AM", "ff9eb3211bd56480aca3f17a3912ac844ca1f223ed6cc8ab0dae767709abafc4431ae73ac93748275ba9957d80444fd2052a388b73623e59c6b1c8c94e15afc9" },
                { "ia", "de7f29ed4ec7b7fa1993e5713a02e7e467610be767a79427cac13d1bed4f70b2445629d7b8abc2a0f857e6540a2aa4242ad5aa007672a880f6e8e81243207c59" },
                { "id", "55cf0167ecd842cba4d9bfa6adf0ef98bf643c4885cbed8ca95f7909d516ae173aed56ea16cb144f5b59e5e0978cffcb0d602484303e60509bde05fc34cbdee6" },
                { "is", "53bf7fced78a2e00f2c003d1cb28e5936111e728a48efea15d110281a84fbef053799a25de9b1b9d2db69d7e6a9e19468871d24715a316670c666fe1758353a5" },
                { "it", "816c778687e885ec5d394f3915dab4b04a3c608ec9fa9f359cf1c9c7110033d59af36b003eada7a455d1f796c3b58985684ea5bcd5026b7d5d0517350e1155dd" },
                { "ja", "363b551e1c8011ad9fc08c424352b2718e244d206ba3132dcd0adfcd590c9aa5d831e52bda0127e66b14f9b4068e762fa4edec1a81920e2cda2e9fa2aa70cfc7" },
                { "ka", "261af787aad14fe1147e9ba1238011a0af532d0584c8f9666a624f868d2300e10db6a5ad72d0c0a06ac418d4eef851a26acb050b7fd0495e96a54a9080a28299" },
                { "kab", "5711ae75c4093cf2a2493daf5a83bef44b0e791f54af902e85b496722bd2c84f63567db96a520adecd4bee963a5bfacfe4c286dbcaa0a31e43f60b2dc04db54e" },
                { "kk", "1b6a57f03d179276708873bdf2062a9e64492f86f451d245accc0e1edb3dc8f4495666d77dfa1da28b2b615f7007af28d3b472667b3bc10f224ec82018910825" },
                { "km", "4399b89312c80fbaa5a437fd5b719a5af3d49f67438ce20e26f15c6ec5e028f5cc171edca6caca27421601b9492f28d9522a57e798ab8ba4e006a63328118aca" },
                { "kn", "52e269de1076f2fd79f6ef3b53deb39ff46bfa7bd4540cba9bf6c653a04396f548191f1dd13b3f36d614ad7d7c2fde9430df69fdc7035148c4565fa6ee763b01" },
                { "ko", "6001e40d096705815df6666bb12d009130fc0969ba7dd50a9e82d63711b518458e7542e3bfed9a4f51f3b791328ef38bd68c72a5df9adac397a914ba02475037" },
                { "lij", "b92e868b6329dcc5c090d5da1124e1cfe175bc5c05d44ed7bb1690263b49c0242db19826d99a11614abfbed24a48d32bb9d1613e2df847ded4f2d0c466e1edb2" },
                { "lt", "0d2ad987752006c22138b6caa60331f8099050d956a7b124862e403e60f54e56deacce4964193e10f6658c3435a8faf822523fac208dcac2a277c1b50b93f0b0" },
                { "lv", "df25e42d0c6edf7f1ee5afaa7d6a4c790200418df5535dff17ab319e3e2ee69c1c159a38e22efbdc4647bc8bf55c871568791b478baeca8fc22371febaca1585" },
                { "mk", "ccc3a9fa3ede60d6ef5743210d38d8e91349112dae9bae86c5921d5cfcf887eed6b29777574eb1b6d83ca6d5a2973918e5e4c1ae21db821123f8d3d25f84b2a3" },
                { "mr", "f711b483fcf8c11c31b57296b03d423643ba52e5c6c4b88cdf17d709a95ee7db0ac8346e224675b7478c8d734bd44b1fa136d486637bb52d8a350509ce5403b6" },
                { "ms", "53d1bcacd2c86748168410df159f1ba67497df8b951c2889197b09efd2b6a19990ec87121c4ab8265cd63fec5abcc4e592c7ff80192b7785bc16a7fc07e00633" },
                { "my", "706f51389175d08c3882bb0d85d895ba29eede1857a0e06abe65a0b351baf9bbb463b39cf24798630bc7c25810d1280c40c5b4a5f033ad6a6cad1bb3c03e07aa" },
                { "nb-NO", "f18e144edccdcaed80b95b64670a88c7b5f1e04d98c8c529865e6704335b61700a4f38961f20841c82260405c57e8e207e60d6a32c6730376e12a5d86505aeb4" },
                { "ne-NP", "1ba680bde5d75350baf971c392cc074da8a37df048f15a5828347a13d15c1e47554d28647709b70b80cb32f3297b9bd30f97698798b1bde3559d920da5c11d7d" },
                { "nl", "7f6c8978705bcc5fe9defbf32f2fd684d8691d6773fe379422671d60fccde79a0c34b3fdaa8e5b28e80636a69f7fe8d6b90879f95a00257ad4fe12b2589c5e7b" },
                { "nn-NO", "90a2b383db67b2c4f06d614f18c5475f64591ea03d18d25963a6d44e76f7cdd8206a9c8a2bc452651eb119a4724175e2e714e40ff6b4dadbdef34dafa2e94ad5" },
                { "oc", "1e5b1c08fd5908e470308f1667733ab29f6e9a6c3d7656ed2d3cec617371f1c1197ade6ea05b3205a889aa57f918110356905a548227a7a850cabb87ba7e521d" },
                { "pa-IN", "a5b4331602edfe98ca54199d37d986f5f937a242da182d1c6eee1b4689fa29387ce965da1cab4bf056dbb01888d504d191195a84cf73f5bf5f418941b41cb0e1" },
                { "pl", "1c6246f11c5fb02cd075e75eed7273892df397cdc59ea7c9854dabadc8d688752759612f2081ded201ee145286e7e328e83f88b8ce75d183fb0d31d589d831aa" },
                { "pt-BR", "146ef42da4cfaa62006bbf34efc46d2cbad2eda0a5c0076c6d4505ed7ef8a73805cec2f698e31df15ab1ddff129773d1d26066edfe7b3ac534ff8021aa171bfc" },
                { "pt-PT", "109f933012c3fd690bb99e0196446b239408ae7f0389353591fe9582a58f010cb86108cf6170734112c97ef80ad2057ecc00197559579789a3c37681f542e308" },
                { "rm", "9e5c81a108efd5d261e9be39d6c85991b6c0cb0e4f7b0d898d55188d504fd06486e6cf7792cba6ade1bf8438839e1cfe9b1d50a3ecd034cd83c6145473493072" },
                { "ro", "02da2db3d0372a6a34ba32e5f1e1592180086e4d4092044cbe6f525220765813ea55bf37cb2245884abaf1047829bc2956089314c842f75110cad99965b2b12f" },
                { "ru", "8297cc08c1a8fbd138b0636fb76bf4dca433b9777885efbabe13f005b639befde90d485948722c4f8cb05934706c6d27a11fb9496761a815772ec047b1298146" },
                { "sc", "f62410c68d5ff6cc9abc9a2ff48bde6e58c97e7b691c107bbd25c1f22cd67cf8cffd8f00815ef27ed9b01e3df625e33ab0ac8348ea097c75fba90124e2c8c88d" },
                { "sco", "095bb79bae99dac4a54479e899f346a9c4689c07cca8f272ff737e5830589593a040ac19e8167b093c7904b33b2ae16b19df7ce7e64f52ccce85d42fcaddfc86" },
                { "si", "3aabaddc526e17b7f1762a4c06bc6757abd3e5739a043ac1a8decde1d94d75484c34ff3208653f366f3043feaa13ffb9519f3fb983278ebbe9a920c8e5ce814e" },
                { "sk", "cf1fcfd22dcdd38e09b8280c98d1d6b5721c6819294c347ebd5a47596ec479387905e9ea6fdd27d1337247e7761938c18576876ac954d75771c3d905777f163e" },
                { "sl", "9caa29944de434df700065a2f3e0a97cc256dc6442b1257a2673fd432a1f909fe9571d7ea2abbf4b056395b8b704ab4e806202e10982d014a5077ea1f8c4924c" },
                { "son", "4bd60143b30a1d19de91f4158754f10b497750c54828fd4928cc1cd436c344edb5e71250941a29e13ecfa319fe1fb186f9a67e90f24e64e3a2daac39c9103a9e" },
                { "sq", "e9d64c856daf43a013f3e692ac23cb30e41107781e92396aeac6823c1215974b7138755b2086c624a1c1c4f3e56d2a4963eeae5da6904b0cee633687d432182b" },
                { "sr", "c529af2871e6db2e3a75ac0d716a8c26fdb69b653d2c05fffbf54c86c4f5f49d6a407efd6c134e17fafb3818f6bba4483fb3a1525e5e36a08ba80fdd2a26ae66" },
                { "sv-SE", "859e2cca9bcb915beb32a357f9ed3ceb0ba6554c8bc52482bc8e0c197962338c017da8e601cbf367e4d8d7ae07e21ee0be815edc47f6725131ef7cbebb738349" },
                { "szl", "b039a8ffb5c4617839de824e3ffaf950be0b53b13a2b5b3cf41806202c6e93595c0ec799aa7fbddbdabf6ae2f525c510192e6b771de69f102ab23d535cd43fa2" },
                { "ta", "7087cbab087f86dcb181f6afa7008347ba4ab2abff51b0c7ffddff380496fafc0ed57da8d0b598114493c3854bd7ead74a0fb8e9aaa916afe5395578bf296633" },
                { "te", "e30d8c942b18f8e38262b197f5a2de4840f292567014ada6a455e58bbefecc49c470e0635f4a65aa0257e2b0cb132d8df4e5cb0453cbaf26a7b889a4d0fc30b4" },
                { "tg", "778c16a90f7aca72bf3d35a7d63049bed3f121f1ab6a06346f051a169ba25523a9a0694023aa955b99e830a4d15492a77a54c08a5d8bc67ed6d0d9e39f93c0fe" },
                { "th", "7a58ee0b5dd90b4f1a5e90948de3fa13320af1262c898decd2d8934ba2b99ce87a7f3a4c124e3ed66cce4ec93d6a306613bd044907eac19089ff4437a6224b09" },
                { "tl", "c2a5c14287c58b3be555bc9b7f190bbb4667e23c74660d58896a93d32aa18add60b4cc455073df6d76e94a4a8a95339f14ac92a289e9530a6d23529f7f15acb7" },
                { "tr", "855a2f90c04a7b463fe9d59b1ce2e05cf5e67342d803afe4c3d3cc3bd7cf4403086bd6fcdf2430bc05118f38f3816c6b0ecbd5219f593161a5ae488b69d0f412" },
                { "trs", "13ab589d70d484336348006a28a1cc2b56760f4987cd784c1765aabdc38587dbebdcd75ba1972acea02aec5bc59833ca18e517dc3c093612ac66626216359d1b" },
                { "uk", "51d7c9188757025e3c11286e57b451a000a8882bc2d1dabcf4c4ef0cdaa464ee1de6a28c9f82fd510225113fcbf199df9c158ed3e3b3a0fa0af0573daeb524a9" },
                { "ur", "c1d012e17abf16991cf6fcab25aa73e6115836c7e8ac9941de9ca3ae4d27dffc09005fc1f3cd7511977944ab9162cccfdf977297f9f1ed37644def2cbd28a1d5" },
                { "uz", "1a46690822ce075e238763d8bf74cbc4ac2912e0df2c7799f8ece5decfd830c9e37264739e8a230c3bca3c526c23ae362ef054ec7302d518d82a8aebb32afdbe" },
                { "vi", "daebc4ee1122c5f89112a95428a67f343da16228c6304ffba54e770297da34f997024df03977e89318b6cef885ffa416dbc54c75b5aa2651fa010298ffdd2a37" },
                { "xh", "c53adebcecc897dfc09ba7f3e9d7f1619c7d0748dc483a32272840fbfb8007d6975139466e0f7f4e1c76f928f49a07345f07335edbd90616136ee19f02e0488c" },
                { "zh-CN", "5b7801547874c02e81ce45c04b3ddb08df653b8aa56ac4f03fd1d6087e56c0e107b2ef0aeb3c0602ab6bdad2b4bd1729f663d522e0e004658d4e02e85d25dd9a" },
                { "zh-TW", "a27aaaf9cb4a4638cd1158503cc4f8d42f856bdea7432c834db4883e2add74cc2bb37e5fa850c98a123326fbc5e02f325f68d8339d6819e125b33a8ea2086dac" }
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
