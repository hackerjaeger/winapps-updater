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
        private const string currentVersion = "120.0b6";

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
            // https://ftp.mozilla.org/pub/devedition/releases/120.0b6/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "29f6ceba336d43ffaff363d7100988b986d4d196b24d1a93eda3161cb7ed1c477d7ca6e3b1cdffac8547caacb89e3354b9989fee2c0be740b8e94dd693e41067" },
                { "af", "2e9fcfc8aa1ec0dff633d6f711e2a9d2af08942126d603977da617a8f3c4227b341ac278ff38eb70848890847da4042963bce62ed3460da88a44870902b0c3a1" },
                { "an", "af0b5c201954a7c1afd81a28c36b6524d832f5850927ac9d521094870f27540bde55165614cfda184b1447f5ba89dd4e6c8db9fe7995e8cfb7d9e5bcf7722eba" },
                { "ar", "30205afdbdee1e5cba1ed693ea41455c0312b694da8cb3bc66aeec84983c237c4b07ecfb7e094b8b8302bc63404ccdf2754ae0dea822d9b57f1970abe65017e2" },
                { "ast", "0ff48dd2b585d4f15ca8a7e4ae2b841ed91138d91edb87b93b227c3023b818c071d53ed85cb059a34303e29986a4f9352a55a8b603c6331f0e17615a74157ea4" },
                { "az", "abd48cd1d9c27e7b0955904da33145db2711f0db3931c178b286e74903a65eead6d339b8a301355e0c757a452d448c293a4882c1da80f5471c19f4e42aa548bd" },
                { "be", "934920a3776235473a63be21c73e1b415dee03ff5043f68d8ca4f01b1fcf6192855b085bf5e47fa029d646e90823745c24f4eeaa581e33f2fc139d4b1ac3965f" },
                { "bg", "5d5aecec13633916d023ad8b79266597d6f37d60bac75c42e17b432cd7e2b881a8e55fa5447792dc5357a943a4f4f9da07a9d545bd127022b8f120db0e8575ee" },
                { "bn", "ad6bf4ed31f3e443bd1f7be295729c811ed6d8ee5be11b1f37c8a1e3ee91c44434e8401b9a83e1a81328f93c4cc6bd2780d0e5b571fc7ba92d537d2cb134c9d7" },
                { "br", "a4ff7a8e06958af46f78f2cc38089d46d1c4ddbe47def2b023ccf572e3f66cb3772300e10d6b6fd55feaaa759786deae44ff8cc37c9fe7d72578446b3d0259e4" },
                { "bs", "021b159a47502400087916980804d7e47df4b0af1fd3bd62b741b8a8eb6509f6ca589d7f496c97f470f5cfaea1fea8b78be07ef483319126155351ee9380a1b4" },
                { "ca", "b494376495df1b283dbb784f3a3003f65b9adde2d0fd68bd10630a598f665975737778de65506488ad937de662b3da1a314a071c644280bea8b741184800bcdc" },
                { "cak", "71025636bfab07977547ec82104e95c2ef186bdf98e708aeb43d9737bc1c1516503391fab9fb7536dae7ab893cac3b8c45a965f09d0c3bc9ae4e176400306e9d" },
                { "cs", "ea7c364e4ed692d9f71441d23d63030f3d9a158bfe6bb3345cb43ed61197b7d6a118c7b4a6f01c7c1ea0f5e4ea2a2255bd96f42a61702b5fed45fe1516f7d799" },
                { "cy", "5aa49a42f90f9816d350a46ac0bf1990f2ae5156e373805f6f901ed4c11644d4943bda70fcc89fc61901a51b39e75010c9bb668b4b758680c8dde54c5790dac9" },
                { "da", "91cf1b1107cc29b87d01d899388399d7772d97ef2311d2e3b1d3d6b8df3b8c4b20a75ae327030897dc3f185b5e1cf37880d32943892fdf878f2f32a7379a55b4" },
                { "de", "e91a1dccbdd456c444a1a6a49e79da67625a461e32a3af4bedf2be791e1f0ecbd96f9b5d634707bdfb4f30511e01cf554e5174a6226a2a4214772fc234716a03" },
                { "dsb", "be330bea47dea1a85512b0c8a536f43f7cccb54ded7ed4fceda0dfda2be50d0ca3ab7bc82a72908f4b8b117d7f624f5dd1466bdfe7a1150c9061d8126a31472f" },
                { "el", "9237ef9b05f58beef3ce407fba0bcb98d01bd15fcce615b0b062d495ad56c543ff5162646aac5c6b1de6c92548cd4e398bbd2850ee8f20c7eb40e77c07308388" },
                { "en-CA", "b9b2dc9030f047d5be99687d224bcf1850defab96729e34e34f104e30f22feb652ccde683bebdbb8ce17b81c45068e97cb82882ecf776bad0420ddab4bf50d89" },
                { "en-GB", "4f7eee1a4e83e27c6408ab18e4e6dba189fda4294ced1e071cc4e8e62510c787456f9abbbe62ff2d38358ef682f8b7ad8defda070da0b1b870c1387225bf8aa8" },
                { "en-US", "dc3f07f0590407f1beb17476b69d1973cdc0871e0cbcea59468d330874bf9cbc801955fdc485f7c66b261e55346714ca0d081b3e7fd1193aa83233c76e4f7f9f" },
                { "eo", "1f29f86ba82d3bf52312d321c484288404fa16a64e679dd2bc9fbfa4b3d1c555a708e9c75f8fabfdfad5c3340b2d372d91cf5e2a852dfa9a0c9780a75bd3cffb" },
                { "es-AR", "1fca2e2c26a12bc6336ad8d5cb501c3c51a62c19802abf05da8bed6c44877f7d17ede7cb2f4087e16f300f992ee74275589a2d2a4dbad613c2e04d9da216b6ba" },
                { "es-CL", "2f244fe7a25f7099193220f5d33443d21950c2ee26844e216df546cec08424f1a8e2db24fe7ab04576440d61a5a849fdde2f00529b57008a7d44267c8a4a57ab" },
                { "es-ES", "7d0270eb293663fd382d4a3e400d8626e97bab9b20c2d1ef0b907bfb5dcfe1db9460268d8e5e315e36c22001b61f09978f238137fc1090964318249a71981fc1" },
                { "es-MX", "5a8b19a143db351fea4e642269ae365ceb41a94333b0e38af19d20f3e732d664a5d5b2a46a9f0bb09b74c8c0c75a3132658bfe68a82c8044f710ecb997569d6e" },
                { "et", "ee1789f79c1165be85006e2b6b0940555ccb71c0434786be1f34cab19123a25c8489108ab5541b483fdd75f959e0b816cfe6ebbe4fb5c48f0ea376761936a031" },
                { "eu", "dbe85630a208075b300073f0074a265f5db02010b462420bf9b3915306713adbe2e62b6b595159e36fba509a0da4a1ac6d3227135789a265fba9a7137bd01094" },
                { "fa", "729456da9690ed10c3001e5e7b75f436d41c59aac0c9e2a0852a52616c00af0eb3fe7781e9aba6c2797b757dc2e264338031059803ce21adbb4a120bc889a6fb" },
                { "ff", "c20a0952a93b7180a737e13933bfa21c5d129617218df145324f984d0c1bc9b6cda93669d1b6212451518b873a743c4a523b491a9348b9a3101c27245d50113d" },
                { "fi", "65e37c28296fbac9c2a84de1efd4942a84679db7571646cac452a21c2db4a36b85e7896dbd2add43ee056d33ca818b857b428f0a42981b1cee8cd80c4c2f7e7b" },
                { "fr", "1624d13cd054aa5da930ce0e5f1f98de00af904859e958f31c6dfbf042f350d7aef8487b42df7ff441c20246b59bbd108fce29ac8b382160d56734195169f509" },
                { "fur", "e08b1d7c6a47b2e1c23a3f6c944b045373ac30a4d85b913d6f256669f4c60269b3546a4909c591236ef6ac8d52ff856e6c5ed07447563c2350288a7aeb36c9f6" },
                { "fy-NL", "f377dab3f3b3d4e29b8540a15b363c45c659b383d7ba0805c0c527145431e28caba4e39f51d01bc8b320a6383b8356d73adc392491b8b7be8fedf93ee922ed5d" },
                { "ga-IE", "9ee7a7022b3106b4c48b692f5dcc7340ad8dbb0669376b1a798bd494173c23968e313524c2df2b32886e55f7627935c64b34d754c13027c47f595fd03adfe95d" },
                { "gd", "6cfeaa1c44f5fc4066625970caf5313a24a84f4956dad744bd3b3bf11134deeb47ef5e2a21adde27fc30f284e6892a52fb79a98ac38b6382e3218b8b0061a524" },
                { "gl", "99c8fbf1e1854935ccfaf98985f5d27c7c9830534fc3030b97068b9e09ee4c90088118be9de57bb6b06cde1a425eb68c07ba599f41f1c2f32eb4baa6feb9015c" },
                { "gn", "794c8920e046b97afe9c7c5c34741320a6d15bba80525d3976d412f996505b1ec954fcff94eeb8fa75419db738fc729318fc3a10bc8a212f70770fdbfff196b7" },
                { "gu-IN", "2e64f17eb213b5a38b6157ee96df42fdda48b3b6dbab1087cd4b16946b70c32884c33254f6b925f3720b3ad0133b806e235a5b34542bfced8e4876cedc2d3b4a" },
                { "he", "4159ab72e9dbafffb5b1b326ff3cb7ae4b76ea8a8103517f1dbf196794b93a4df53f9585bd24fd8f2403cdedf1acf11604a0463cd93d9fc79547e1bb346fd591" },
                { "hi-IN", "894d1651e718223de33b64b083652baf3ae0981857c1f8e7101cd87636f82a00fb1dbfd66cf3b65afe78a13ca4cb5052b0ab67f625312dcfed4b2af696c48908" },
                { "hr", "e99ef5b61951d9bb2035b8d1e8b346cf8cd99864ebd80db6e0950d9e217c5fa572eb3385cae23906d14b0eff53d103332ddf2d6a90b4b1a5a8c1f3e2a930b6cc" },
                { "hsb", "9920ab417083f2dd23e2e94058d536ed94f030c52bff950640c449cc2b0959f8e25eb0694c28d7214a8cf5355ca3d37c110381d6aeaaed26ac39d3c3dd65e38e" },
                { "hu", "b25c319f70aab2cd5f2785cb8995d7205d01692b6a34443d248ae7cbd7af13b9fb22d0820f3532f6ac587b8f4d5515748d159a1a4463abe412a8287fa3c4f320" },
                { "hy-AM", "80cc14f5709c90942059d1f1ab8151860bc8762a713859e446c15d94e7a1233eddc500c067c3b1b2ede2d497267c20e36efac5bc726e6572221872fe7341ad43" },
                { "ia", "3ff43d73a102cb249def024bfbd620603f33295fe88d95865ba8bbc598c12c9daee3c86a953a727a9428269808caa79b13ed5418dbf1634e52b5919af1d8cac6" },
                { "id", "dca99d9c1fcb1c0584ed5aab205526907d3f9cc3cfd0432ae27d981c789c0227a89c89418bed869829783dd55a46d34c1a079709d6bb9bbab993d7ce14721f22" },
                { "is", "28945ec2e9ad2b10dd6ec0330cbf192f32b30288d46279f4ef1269d5e36a042497522ed57ee27ac90806b557a43fbac5084f82df2c0b30171400c8e70288d985" },
                { "it", "9f344537986a481f964e54504f9b403b1d51943c17962bde5f192c68eb7c0d3458003a9f90918bf2032f90a6f22e1c1c1bfbcf4c69d578936050aa538dd8ca75" },
                { "ja", "13f2c433a1711bbb7418824f2575f835300d5d0bf1f457443da8142b05a2d67e21386f7105147631db84e238932e4b7e14d6d200f049202d9f2b3092143282a4" },
                { "ka", "11e42c1a49f0641cd278efc45b051ae6f58e25d76be634382a4748b9d0b562f580481ad5294be2eca883866b54f054921f958c2b68bd00169486c17721c0e564" },
                { "kab", "30eb2942cd2f3c09f554129d381cd215520bff7038a8f53a3f0365c725e72461a8754e2cb374223e369d0fb93463fad44af84b021b3eaedb50699c1ee0b89168" },
                { "kk", "afe348b9f7671146d22ca2ddcdfce0d40e7c56548305121fb5a003b8db27f3e0b324181ba2019e8595f54709e44a8668dbace069c04fb98c1479bb3a4a8566e8" },
                { "km", "6fe5945b6fcefd4c889382e383ffe30f8c0b7093dcfb8300ad545635edb7e8242d74dfb26718d9fa345ebd1ee01ead28b4545f6dba2499e9eebf5dcb549bdbbe" },
                { "kn", "603c43fdc471d08973bcc9e77e55a2fa446245b6c8ff17d67991833a5074f29822cd56cdf15ccfb0472b94be975eb34df43cc98453a4fcae7c292bf15d1d431f" },
                { "ko", "95c8529069204a5d23e727652fff0753d9aa0c40fda2abc613e1b24542924a324974abb96413a304a1d6d254cb46b57ffab94b964ec3a16b117a9c2da1b114c0" },
                { "lij", "921a9526ca9d16a565043e4c7a696ca50555190cb0a8d9128d9833e70329b1abfc61ec7cdcabd66e0b2d4957507ca015147fbc1882d4fccf32ddc478885c4f9b" },
                { "lt", "28153c9c179a93f7c620ed3de8b81da631d77831d9875bfd95db32afd2d4dbcf137f8af0d2a7c3a5c1e014fbc5ffa6a40c367eabc886829edd8b80f35b277c02" },
                { "lv", "82fc4ca3a61e81270e631b99af4f366cfea4f78cd88a9eba0ef5242870fcd4ec830fde2f244f86e3241feb8946436ef07a51858a2bbcf1858a4fc4c9cff92160" },
                { "mk", "c329c992607fd1175ee5b58afe845cadb22cc689a29921862b5e57b8a2b69795938a0a4b0747c2b254847cdeb87c2988bd648270d7cc0e97ff7f96c199b39146" },
                { "mr", "82e567c4ff06c9a6a8d925e0d16838eddb2a21c48385b0ae41dfcffc864d4d52f96b27e5ebb3b3959b7605db9f30126235c590d2bfbf9f26ea59ff033eeb49ee" },
                { "ms", "f23cbb5b085f3f417213272ad8cf41f5d5be595c04d207b24b8f74d62331e2adbe229432ed4891ca9faa780443ae609e1cc3782a1d7c007d5c149b12c5383067" },
                { "my", "d071fcef3ae02b65332dd209daf71150abb303c0dbf90c2a26b972b06fe5570c179e657c6eba9c9d15fd1a53440a95337b855c55d267919e4b33a2c8ad6e8259" },
                { "nb-NO", "3b78f809fd5053b51b7e22a40513158cd7507b80382d232109cc7a8bba7d5f2c6ece39a4b2db8f1b1b07f80c1b43a457ad5268e0663f981d74a2fadf69b73d91" },
                { "ne-NP", "6e01a3ab3978610027b90a577299b06c9a314c9494f880a51129942691ab85990cc52fa42a6c010e246d3fbbf8d718a46240aeb4aa6b6234f2f20aeb2a340e6c" },
                { "nl", "512a28aa3edb7f45df6170fa765a9ba730bbf3e7fad5efab59595ee1e1baee428a34d20103071c3e448196a967bf8422da378227dfd609beea241f5d93d59909" },
                { "nn-NO", "91dc89f488c15c64271325357160681af7941890faad3531c76f713a7e7706348d624069351678d70dc419a3333ef4c652825e1c05f7615f882684308f227cde" },
                { "oc", "07e26c2840be65459b311a0244657bec7e7b0910fcf208ade26fcd8e46cd24cf0f5281dc3050b1aed07890774ab19b6bd541f3e25ad0e56d05bcb54082b045b4" },
                { "pa-IN", "ce36ce50e4e6cfa73ff60769ced239dde7cc135174f1f4287a0791f7409803c1cbf72add07240119964f20b61d31cef5cff9741b60c6dda98001ab8e4509729a" },
                { "pl", "b4f3cffce7eefd0b88dbf53f44a598b4236a2986ef5659ea7116c61e83383302ecccf39f3d0e9a9f625577327f360e14622721c5a3519a3b055b499730466823" },
                { "pt-BR", "cab35083d31218a0aff91295ea2a7926004eab1e6a9d95b5c208291d0330be8ef192c3e7d01ae9f96ca8f61592f6565469b59af90ef55f3c051fa939627254e5" },
                { "pt-PT", "f2e34903aa61311c603fc9cb2f78030ee7262749c3b247f71070c26d971dafca70302223e52678fb785a680036fea02c59ff99f60d48f2078ac0500739d526e8" },
                { "rm", "fcc5433712d4511c02e27afc7060cac69b64c0bef9b7880df13b1466541edf0ef96afb687de5209f34299065d391ad6a5f39cf421e7334f3732c350a104b9985" },
                { "ro", "2c7da807db9eccf8ae59aef6993bcb85dd2d93dad17678e3c20320f0a3894b1b5c24c067a9e7f33d639c8efb8a22ed7c0d7aca9345006357f0222f764e062429" },
                { "ru", "15f71eaa5df3af2869f2a6e675b3b27b4f5d1718ca79647bde32cb79e1bb494e6ba067fc417968f285c0b4bff30db5059daf58d827fee28e2f91a97a4b37e96d" },
                { "sat", "5034fa5a88309a1025c32d3c66c446c2218c0d9ab465c9c8adb63511846f9a34855a8480fc58b03b484c2d1b9f80336a5e5f0d7404ae2fb394cc7dcb2b4f1f47" },
                { "sc", "f4150dbdfa50a7685e7cb551ce43fe84aa9979e02540a0b0ed39b9a064a727f41b99842094183343646a8fe7eda55167e594dc5066fc45f389f59cb53fb48737" },
                { "sco", "c527a161b52a57c532f7cb35ad0fb118e9ed8470ce675367d8caa09d8bf5eddc4a1b57de1a5beb1730cffc734918685aca8bcf05d2561a0f85e7e50df43f6c31" },
                { "si", "a8743c63bd56235920efae6f602fba3850d0f28f686ed2e1ce10c73d0322f01c446849fe6a6bb2994ef77a2cce2c1f561b63d58513a37b3112c88281dfd2d4cd" },
                { "sk", "cbd8c005976ec1a52cb34d470ff2855ee350d029cf9f5bf8792eaa05b3daccfb738ec6cb9a1886ae3cdea7a677f1bcfccc61ad5fb708007810160dfe17642be2" },
                { "sl", "fec92001cc22a1ea19329b821fa33e572c10c50ecfc96239ca6f851af9c7372c531875c159382b5bc4c304679839431f9e8627b0f0de340db47affea9204d790" },
                { "son", "1948cac5939d157573880f49158c50387758cd65590f4da9173304375d0df962dd1c5c5766ec866999cc06ccb42acf678855b855755976b141ed09f0b79d6f0f" },
                { "sq", "3b06f10a87909be635f7a63285e39779173fa2e575292c8aabb273c0b1b381b456936284b815859de604f788f6c29aa14794f6ef61066fcdb046cda0cd20e32b" },
                { "sr", "ee63d9f2e7f542c2129ea059ac90bc051e4e3c85646c2ae8fbe6697d88bca4907d73fc92ffbe76c841d25213c68575e161d322143ac0937d53f8c2133a6aa748" },
                { "sv-SE", "7b59eca7259899c64acb411130297abb5cdb4def8dcfe84193bbc359cb1a1348218fc22fc7e6b5e38178532a9a7a8b809027deb2ce74aada982862c962638fd8" },
                { "szl", "9525d0cf9a277c7253ed299fa870e75531650a5027d118db62ec8c491189311beb2b6bd64a6a24e9aced8f5997c5e15f45032785b44d98b8c98703fa9d1a43a4" },
                { "ta", "5bbb736e8396be5f2c62e43b36ea513c6b5342853d449611f13ac608ef1b833135d8dea7c387dc2cce4f52b4e9a5258ffcfb9b978e587f08e52c03a5cb34b514" },
                { "te", "a720384e5d10b1e1d365884bfaec94e203f61144256631f7d77c3aaee35ec4c7a721be6ac9c2d1a65c813d0ac26035d41c0a58ecff34cb3f6ce9bc47c3a20b70" },
                { "tg", "a016003af1c76269a5fc7b4c31e3eb16d903d3f4d9b55d2c04328d1bd699c375aa289e981b79a66e5d73324d516bd2bb3ffcdac1064d0643a42297d82652b574" },
                { "th", "3ecbbbb6b4bdebb8f3d2042a73a5833d21b4893e5cef0412be829b73af327a7eab29f002fe9496e869e408658b560bb696fa26f7bc0af877e7ccfd6a8238fdc5" },
                { "tl", "738f556db61108055d5067d308cae897d3c60c087a477161cea4fb0b977ab0c763e88e699e16218f76bb06f23dc02be08fdc6c6e996dd20756ca1c252e9c1631" },
                { "tr", "02d438df8bce422ff57cf375eebe7499e3437ea987ecd81c008cf30e333ef5b26bb4831d7e0e0895ab35fdf9e5436c2fadcac9e2376d492fb8e9d0a9e75c8b0b" },
                { "trs", "fbb8ba61dfe4d68e06c9f015efac4a9a9f76e321f82e457d284b8d6625ea76e2dcb3fc4e6241468cc43400adfb33b362f71ce7e23d834ac0a554acca2125c241" },
                { "uk", "a164f6b43b6371e959bbaac8e54a546c54007681490c9ce7631f0e113b5f75fccd934cce4654cf2c078ce19512636d305fb2e6342b49bae8605c57be56567173" },
                { "ur", "572e34b8b525bf7c65c01bbad987c8598a81532f7ae646a9c69f2d3bedc98fd291bb1c73792a3d30c0492ebde083154b8fcc051cdb5c343c10de8d08aebdc416" },
                { "uz", "31bee33894659049e54e9a61d096e7e493a7f983ff97bb88287d0a1c6bd68ab3bd2f33229239c73bef9ce5ec39c6e6f4f95654dec71f8a8f3b93e6ee5b65bb56" },
                { "vi", "71a9c1666dcae8e832d517b89110d263a4a64084562c71a9bdc3ad17b135886a0735cefffef4aa355d7d52a0782f0bec2293a9aadf0ac1a36f3758943bf412be" },
                { "xh", "937cf71ae9e85bc5e40e6dfb1dcc1e99a7c1ac0d2ed4af6af2cb5cb33d772c70459b112d3bf9e9e58c6ebf81b190088278945a4f354aa79cdf6d29ece54bc202" },
                { "zh-CN", "417683d0b81d5ea0f7e5b86ab48c1bbb2743821ee84f00122307750b96d62f9d2c93929de58c5939e0493bf6e11926761f8cfd1fb05a2e91924d5b8a46047a05" },
                { "zh-TW", "83d6a8e7fdc404beb4c1c1f0d526348fab737f0d0f0e4e0c2ef9865c33ee00661fef2edc00fcce42a99754848964f8a4a18cd19300cfb18edd2a7c29939158e3" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/120.0b6/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "fcab301a87ae1aaf35a889a4ce77628c72a404728af0c1d07a7a21a4abfa679c512ce7c52833374df7bf9d14a880aac9ac0e708118c71b19ce12294b34b19b97" },
                { "af", "e639988865eadbb1704ab965e1f5e7f0d1c8f5952f8dc8d4b33b198d45dedb0f512562aa5af7484eaae1b4e7ff40c817f64c8ccfa1714f075dc9b5b595ef2e33" },
                { "an", "a5db155066a30a08c79de248badcfecd11a9da7de5729068d031779c597034619a3fe84950d31b40399f02a3d80d142e2f8cdb0f3d7b3f6223c199bc6334b742" },
                { "ar", "7352dcfb6d5279539c1b2464d50678ddd38aa84322fc7c30d5e94e485257d93bdb2e1d451769ea767c0c0e3b5bd37e1313d8294185d2666ad639d14558cc6acf" },
                { "ast", "ed1b9a1e0813052ba1b06bf0be384391a4e3a7fc824e2b5d8adf7dd089b19dd4a4378416187855aa0f34d0cf743f7ba58b56ba26375e41c0d924b202cbfe1730" },
                { "az", "449543f0411dd276054cc5bb68fff0c9cf06b8b8880b9b3f9fb4e439db9bb29cfb5437baea978640472bb5e3393b42ece1f4858ef279c75edabcc2cd0a958e7f" },
                { "be", "203a2c5915a3f8700d422708460c59299e536f5c62a51d52be1ddfc8f19d34d81db9563cc94095df50aa5a2a342b0a5d0dc3afd3028851bd888727493992505b" },
                { "bg", "877581f5d082f989fb6b9e56f552a49af3fa4b4d08e22dafc00c3c6b91437526e7a922acf5b57ba4b9a70748bbf8162fc0f7f6fbff3aa6d93e09fbb4cdf90b5b" },
                { "bn", "f5e6a09a0bd15c1bb4ec6905bb13eeaae9887a6b1a21daad6b3ee4ad007ad863e40897ef89fa9dedc259170cdd34d676ae89d77a92404b287b282752693feb73" },
                { "br", "947fe2486f7b1df90da3a2e93e6c563ebb950ae6dbee213849962a555a57dd1920ad7d75360391ee55e39e1b6a986ffd224aa38ed0c9aecc92936448ccf247e1" },
                { "bs", "9e7316cfff195097c37271f53a20fce41cf4bef1927d2cf250f0522c25666abe3316f51ed3215ba58c0e45924e932394b4ed9c63031f84f7631cf7a0369ade55" },
                { "ca", "e71d8bfcd2803abf67842727adeb1ce9b855971e42d5f6bc0fe959f5f5a82fa09fc3c4744de49bdfe9cedcfacd067a093cfc7ff9894e2dedd10b5a5a4ce0e3e8" },
                { "cak", "a22a818c6106f046758506d49d8a2a59e7360db5ba7b20521058445cb9dcb69cd4f29bdbfa4f73d9fa7276287b514507d477cadd77cbfe1c25ff78adf36bd8be" },
                { "cs", "90d2566838867c8b71abf31551c7ab00bdbc698b276b1679244456f8209be7f85a22a8bcfdb3370266d15fef4158c05c834ea1172ea1f15eba68a45380c2f636" },
                { "cy", "713d0089dc750f2364cb8f741d63cea3c996441393be5f58246c9fb6506dcc83c5d6bcd1a35f4bea05cdc175064bdca056e4fbc5bdff580504cf07406be241de" },
                { "da", "e1e92d0ac29fb1bb6f59e03e9f5a0b97525efcd947a63497dd333d7b53c89be0daa5973006b187a46a8d527bc077aa2ea7eb0efeabe975e9e88f47a868ed39bf" },
                { "de", "47835e1a1eef449c33f069127c8eb47c2611a97bc144baca57c765cab44285b6306b153dfbd54ef1b9888865a135a6b5a8ef421ef42a49d982399089146b6fea" },
                { "dsb", "3ffc8bdd20bde4469c5c582272814aefa8b45ec25118aaac6e9a0780e5d38e84df16611aea3481de1628cecc64d97316faca4de39654d0ba76b89b9a0fb1bea2" },
                { "el", "4f2030055aaadae73165996a06853a553242ec98a36a92f130b6182b081c407367f92ec40c75410febaf572c82ef12da9daf7a290efc099186fdf0de4154b1d8" },
                { "en-CA", "30aee3d666a17b2f55c0e17abed8b8fc040b00fef7e2f6997804522c3e4b02faf76e4a7f848bbd7dfa9d3e17c10a897abd9eaf0e48d20420d1cde63773511feb" },
                { "en-GB", "0345a81e27eea0ec5159de9d91a7c567630fa0ff860e4ff07947a5ac4b321d00ab8daf4bca3b015d6ead87816e3b699c739ff28c588b3e8f19c311f1805238a5" },
                { "en-US", "3735781895d7df4d590a49bdc2290d7d2a18ac0f6cea9bd342903f137334df68268575afd246863c5578e9649909881b009e2997f3fc2571a9b267d98a0a7b50" },
                { "eo", "d399fe3029568b59e23add4f6a6fd5318972a14b6752992ee175900842bc5f8a35e6d00b0f162293feb8012d3cda2a891a30b6fd4533c837a2a6c9c0a148d269" },
                { "es-AR", "7b71a99b0eb6f8157dc0bf97f645622ee0b1d723a739afc9b467f47a6a1e565633b3677225ffa950072eb46ae1954b2aee1d3df4b0d3b18782e5065f8c5e9e78" },
                { "es-CL", "c4b18a2a85e37786f28451fc4020b2c158a643e05b3a3b6317ae1960799a3262f9e7fea276ee5f81d6f49ca76361bbe768620212a7831703c7e663da468ce8d0" },
                { "es-ES", "426efcce3a45b983dc110c4cddbd3102bed3e510b0b2c8f7297176fc136366834492c6c1576d82acbfe787184961a548ab3cf09c9b5447a83f73ebe9be74ae56" },
                { "es-MX", "1c26e1812fb26f56ab5ce06c1c3bf5658028c793ed7d81a38da59853ed83a2ffb3b350b73625ce80f8d9de1aa635d83c2b157bb8a4fbc573ac2908675bcd264c" },
                { "et", "c9b91ffbe915631eba28ec2f7b7cb5e690b988a13072a1b9a58e5013a400a9bde77ed1e9b24e4d5cd9624f3aff5037acff9b5f44394127d56e7b3841b0e1eb3d" },
                { "eu", "cc6daaec5c13ac37d915dc1ebc19f01877c84f6b5481cda83768ee54cc8536ea6491e51a0f2bcec60b3909e70d7f75df8e2a1eaaecb24c86d0c3428bdb2ba9f1" },
                { "fa", "f979719197d4d37dc6578ba1bb60f776cf0b34db698d1af837e88cdaef44c265751d7ca6494f6b52aa7fcda06ebfce893d6a1407a935b5461b01d2b5133e0871" },
                { "ff", "6cbf50bf1d9c73b24dcc522727382516e491e3c6f155ae5a4a5730b83ad6fb0548e40bc9a6ac9982da0a08f869c6bf0972d75252c6ac5becbea68f612776f193" },
                { "fi", "26163843a01c6db5b848e2cb594009f10f20c062e1abaeaf7ffffdfe126c2014bd43d44e46ca890715dd57f82afedbafa5554c636d6a2c67853e65cc8fad236a" },
                { "fr", "c00b2560e1371e8c6b46a961c442852ae1266bb0142e0b6f6ba683b2fe10d14069cb2f722080592f2a214868a3b992d47c9a1d79b3a801640766b2bddb5c0602" },
                { "fur", "20590bc3b64e254f6b73061bc2d23bcd495255a9908cd1d13fb6d8ebe80c0bb2e70bcaee02876014355e19362642978f8dabd189f60d3a3b21099a6226f93bb0" },
                { "fy-NL", "a8b7d591feb1072f6bff1c7c439c27e064929a5c41d27671202910a0b719f2a2691c220ab2038b5de641fc33582b8ad511d77aecf29416e0d3edd5bdee32722a" },
                { "ga-IE", "a4f77b32d42345a6436fb961e7e897bb974314f95061326be0ab6d1b31bcd3360f572dfa88caa77e97cef6ac527088fcf461d34ad2bdeb4428ede2a35031ac08" },
                { "gd", "9266b777d0c407805d77b7e3cc8fec20a2f3099747fb5372506fdc600cb2e40cac843cb7770375704fd9f0e9f6cfa50d2ff82fb44ad94d41fa243b3764b252d7" },
                { "gl", "46df7c4dcc7c8b7cf3ecb786827faa36e0e7462249e1ae2bca8ffea3d09eeb38c6d7c463b29c9f9390a47866f85c3261c23df110e4be9ccb2b99e080e7772cab" },
                { "gn", "166548863f91206a9e0d526c1add8f21cb4dd76390b9fd3932e750366976d926b8c64f7db071fa6cee6a9f638631a4241afc84d5f61552b98f0a0c37200007fb" },
                { "gu-IN", "cde72347ce975876f2a96235799f0bd7279fbc2ef66f0977feec2ce240609597e999a1f232a96114cc3a6a33c787f48ea6017396b78c8aab308c86f1a8d9e53c" },
                { "he", "5edddf7c52c116cd6069a443b7a720b8792a2f858b334d95be22f59bd711d362e6915f5946475b0d57a73d893d6b0f9c760d2742fdbf9c9d5b8514a8b8ce3c65" },
                { "hi-IN", "5943a43cf46e1bf02e1727d9b0feebedfacaead822f6c5ab0bae23ac81915ed58aaa86b3591d3da0faf321836a281c43d9adad2008eb78e19485c11d229e175f" },
                { "hr", "f558e6d36d7ead838db3f40f06ed8a13c3ddf70d43f230b8e608fc5077d93521e5580db3c1f81c5f0813e3c25b355b99d258d5235c24b9143bf4e7af1423e121" },
                { "hsb", "46fd2f3c363cca0bb4b87e715d99fb39905729f8e8b26b2f0098da31adb9e868470355a320915b532831dbbe66e925e389d346807108870d7599e6c466d4f864" },
                { "hu", "e02bf66b00312fe4c9f9618f26315b1a953af6478d9d6928c58040ada412612ca838628341414bb9a57ef540e4c36aa4009e73485b3482b80da4425262b346c6" },
                { "hy-AM", "47d175ff62474dd6aded622d3ba31df23a8241f9b8f1c055e58b072ef839952c778139dde6e8603c6fbe491d3142fb9a590273841a40c925fa13263cacd31793" },
                { "ia", "a71225931ade5262ad41de585897127bf0fcb032b2162984f7809a94356e1bd0aee239b0b36abbc45641b27b07a85a3aee5cfc86e404d4f42149a9287660dbd6" },
                { "id", "da12d2ab1bd558d06d004119735db35a076a3e7e37c7b815bd7a08d9f4989a1500f3334b7c18900b95889e07d8bc0ef4ed4822efa5a73627f5ca6004fc0a32b0" },
                { "is", "98d61355799410d965056c9bf3d963172b30e1364af7b77fdaa98f4373844044975a5a7a4fd77c66cb558430424be1bc95e3c50e29320aa7e73c0a840148e123" },
                { "it", "9afdb7da24303377875a19dbc5ded2b6e2de5ee53ee23f720fab378f5925e10cd1886c2a9d7b793596fefeac7d6593de5312bb40e1a42e1d3dc6099bf57621c0" },
                { "ja", "1390e8a35c246edef6fb12cc6f95dd5dbc2ad75bfbb448dacfc886ab6b80ff921edd69e7eabbbae9f56abf6ee5451182615ec35acaa15f818fd769393c29ab0b" },
                { "ka", "317fe578125c2c92d2f70366ebe1a381dbe89ef73facd873badd7af7fbe097fd9b9ac600720e83adb43707eeb1ee02ffb7fa8e5df10651fba940db6adb92b447" },
                { "kab", "ec509ed781303c54d4af8c902da97ee760f5dedb1aee15ffa350a3f98eb07b9424c6ee59d340cc1ece889886215d2417c17b6ea0b75378448144a2e1a2b8177b" },
                { "kk", "e8b92b8cc750507d09c243e58440e9e3dcf33005d7f26aa5361382db044bddbbda201aa10330117b743c3fe1be08cba00223ad273b07ee8f2a64d2a7bb28cb19" },
                { "km", "7bba4bcad9355d9c228db096eabb5ec2a5a086d3e67155cd5406e6151081353177db597bee4abb94de77375ccc43c5ae3e964fda64924cb3ed3e7e83a87d8ede" },
                { "kn", "a40add1cd2d6aafaa1af474337918366a4325b7ed4af81ea88b5418635c7a7728136a0dc48492e91827261e39af913a3ae18c237bc43bfb716e5c2a1441a569a" },
                { "ko", "03a832873c2a2c4bced4a991c94c0e4f2dce40fca47545b1b3476da8e767aa4d54b0904aaccf8b93d42d4cca78197baeb34b0f2031646cc51831e2a6f5c66784" },
                { "lij", "efafcb70b39fd669f314cc7227324b834e811162eb9cd0154224113d3baad406320e6af9c3a75e15baf1559396f07ffb597211b2e97132da1f4090a5c7df4e13" },
                { "lt", "615f2451fd96c2245c0f2902d46dee341f1adcc70d9681a3a1985c5d4aadfe189c5cc3554a39d81de26324aaecd250f0336e98a2a3c15b35e93c1112350927b3" },
                { "lv", "1a6a0fe751656fe4cc62900e12d210189e1b79f27c9a283544b9166a4daced6969564199b7e91ce860d9f6613253fef0af4a19c8311deb9a381a0bf22621e8e1" },
                { "mk", "12e09aebf7f7228ecc305a5a5ca4e53d31238e912b90890c4a86e5c384cd929de7d9807478903d995659e4b9e0c5cc50a375c0e2a9586fe1cd50befe2d84c00d" },
                { "mr", "cd90d5d777cbdf86f43b5bde12e2366f74a74102cff6d6b4f3fab4065b00a32a87faeb0e646a6f8586429e7043a7db8a36d536332e3880c57016defc879df408" },
                { "ms", "d9997249f02466b76e4e24c6fab11a5a2a8103ad60dfa25ff4ddca259da4a98126114b845ea282c2a72e82ea4b4fa1caf24001e89087169bf20f7269e7a42402" },
                { "my", "95b151ea47fc74167c8e91d3eef909c6025aacb2361945980a2eceee034c08a1659028bdbfeeecbf304ff5af11b128eb428aa8785ab553a853f3eff30cc9f42a" },
                { "nb-NO", "d03775c7ea8d9b70679f9997a630a72e12a46e4ad10b7588981f38cd6da5852f4004ac327d85a8b5ed5be1f7fbbc7a40a837565029333c997e4151516cb408d5" },
                { "ne-NP", "dc15cc4ed72cc9526b155fcb880667d5019fc874b1ad87bd006829146b37531ddf7b7a599db24576a49dc98ceaf2a41060d63f832a4ac83f16742dfd9242e6e6" },
                { "nl", "865d437378b458f3c3a52d7250e30eb6833a27a92997129341d0fd90bf04570657b75dc56c50682a8c8070e7ac6599c556c3c939a12b543bd379cb5c7c6f6dee" },
                { "nn-NO", "63f767ec06141da75d4dc20ebb2baef4f0177778c105003f2dfd6814373a33de554849314a7f7df2126008e7705e2d3de4221bf8c1883030d68bb79a79f43b2e" },
                { "oc", "4d89ff530479c9391b62f06f8c666b47fbff98c2a228c9ac1590eaba7e7845b7af71223f23d585385438a1b27520e73e0de4fd2aa8d9951fe74e8fc73c687a9f" },
                { "pa-IN", "b2bb90451c77930d5ee089947dcca02adcbd24b88372d7a2569a2a077e895187d0b3e47a99edccc86ba301f08fd5e0eb5d11a0e6e24f5637887304c7d1683a5a" },
                { "pl", "00d952cce059f1224ba085ca7fac00d07d4591aef73285df2b0c0c3301138e28a40b9884b40c870ef7957b879ada5c376e0a08e464398c9abf251ea80948cf92" },
                { "pt-BR", "adf3d79d36d2a360cdc9328bd09774a55f590f7f1a98a00a231ecb4e883789f734dee5ba6efb8e2d3c02cc06d1806019c00003c5de491ee8f47f21efefdcea63" },
                { "pt-PT", "9ff0b7e3cc7852e7b845cdd5216a00d86339dd55984675a0cd235c7ef723150c6fa2f8c4f43ba36852801ebba2beff1a0b2b6aef81109c6b160ca27fab1d91b4" },
                { "rm", "3ee45fa179a9fb4e5c4bb246963d53862b2e0f92d000419c022a711cf16706a201c8fbb34b7f98295c479693e8f665b63039b348cbda924f0cbd08d2ed39107f" },
                { "ro", "0c3798715b76e5d2af26ead05b6a554cdaff7ac714b714e3dbb8eb1d35558d165c5340752c04f17433d99e68f505a8409499a369766c35c52afb67f420ff8cd9" },
                { "ru", "c04cb9446830c5ead825044bbd12fc195213187435c35f6a5db727e30f2e9c0ce1d97f0467d5de639f9afa1fba1b66a848cc3b805af145110953460a37c18d66" },
                { "sat", "7e1e140f173b0d19ef5de42e8f18b47a349433511942e4635b120029797a96c98693679325eb0cab935e8a5acd3fe748a05f0b23ef8178c55440bac5935d8610" },
                { "sc", "365abff40a175471ccf162d9a02afe5e8d870d8c242daee58e1b23fa5b35d42d24fb7d31f082a9e4fb2f7e255377b3d2a7196e42819ce59796e2eeb66aee5c3a" },
                { "sco", "9886a08cc565de232499533417671e9abd6f96f04fcbc45701d447692c4d64a1c10835c37a9761bd884e93f440836522edd971beb5c6f2756faa32927b386a72" },
                { "si", "3ad1e7f123ef8f9a061d8ab8f835c13dd62cd7820f0e4331a59b7426485e81e2f8de6d8375634fe8836a9a68e8ecc8330d432af8d63f9957eeb752d9e8b520c7" },
                { "sk", "18d31b216e449c39f27cb3a880165a6e0a430a1029f06210b6e63c1298b3c2646566bd3d48467884e69337fce9c69a225d8a4252c25e46095bbcda38412dcfcc" },
                { "sl", "0aa1abdb11c4b3274c2aa55bed31c89cb2a2fe496a5b848344e87d4006cef3437f92a8d657966b30d0cce4201244724505cc58ca7e9c7ebd64e7ee92b3801d75" },
                { "son", "ec3e1efa9ed92cc47d02a6df6682050606315da3ea8acd0f5dbbf6e6ac5a278cdd96282d57cf0b85dfb3308cf2dc59806c50c03d72dbf4c65db7594d0f935133" },
                { "sq", "0e2609b24a868ab7c983d526ff28f1588176ebcabc8fe0761651840609587063a4b7bb85162d85ed450a2175603865ad72a3c1a08613efd39eb4336b7f67c1ee" },
                { "sr", "a0b71016ad56c94ed283ab00c61d6315db1042053a21a553f061a5ea5b6a4e1de10f83bbe694268d810d0b3caca6b0bd3d29ea52871820e3137ef31ea05a9760" },
                { "sv-SE", "5ec09608dd0016dca77e7a76b5a05761267948298402534780874499f65bb367991989fd7d3757896e734915458a7d0f00cf3b3ff3f141ab294aec535c72ad55" },
                { "szl", "05879f1f887ad38b27c9c7abf6f7453299f8caefcc4cd48a356cbde6a1cfa794604a7509d9f2c10690098023c563092d6539f0345111b88cfeae7991667f147d" },
                { "ta", "9158165658cef0ee30fd1d190e3f22bc7546b0e3df17c58c334f263db1bf91a3b802f2989227ce405fb440a4d382872d176a8b89436fd88eaf206ae0799cbfb0" },
                { "te", "2772a3bd2030c296128efcda56f0fefd98ac778016993c8858b4e923b8c79e160962261a60acb0be321277660bd420141f0df53095341455c67a23e50a68a34b" },
                { "tg", "44511fef8e0416d53d0d2b8fa695f4d8a6ca85c2fda8d260d60936b78046391ab6033cc14e41daa52fc70079376b33d0a6fa61ee695a746c2be6ae8a4583384a" },
                { "th", "a4be0db8e342d0cfcf9d0016d2ab4e876d19e4d5becee21fda159b08a96d4c0b4e849153cddd00705e4a9ab6db2f132b7596afbc9ba848f63ef0c1dbf183be1e" },
                { "tl", "11b5963a8e08931fec0900f52510df4a7327ced1d3e1bea56eb3382ddd3906be2bf04282dc55a30f7915bc725ca2d0b63158a9ce76fce25dacd636863188f39f" },
                { "tr", "d4805246ffa35ec6bea3070310b729077fc45aadb890f9ada4add78c8b270d0a88a5ad4e2bdb95d9d0b8caa16f9f5303fddb20f4520ef4a8be9843f7519d0b8b" },
                { "trs", "8b51674eb5aec0106b0c325e5c7fc201f70856d324218b20989379fcf493dc04cbba8fb22dc5b932fb50bb73d2f8f9c212b3372af46b8c2af78cc47ab1bc8ad7" },
                { "uk", "41e10fc127be0d6e2f42d5247fab0474b3d2efa22fab1d3d5466edc057eb206142630cffb8faf607328cec5b44965e0044a1341e6b9f05ba2d734df023d9937b" },
                { "ur", "378b5abfc3d70d9353f15ee82a25a5d2ef1578923caf86d7256967af536ec5960eb7a613f9c22e0f33b93788071ec6357de0e18a4058c78872af47f819ea472d" },
                { "uz", "aac7797408bc0838c8eabfa4f63d1637bc9313081565a8cb5ff2756e4fd61a467316e8a11d91553b0b95447ee35e9b19e43778f540de8dde92b44bac77e484da" },
                { "vi", "e97406f8471b8802c28b88dca2941d0c130868fc4bd229db0bc626f097ecdab0abfabc0158c45a4a6e31e86e93e528fe428d2bb5f4d1c96ae47bd43ee0dbd69e" },
                { "xh", "0caea3c92f55701c3a71148a1d1a939813c6127fec4497627a657e93d0fb1d2182206278f7844c51a668bdcfd9261b9e4ccc6acbee7de671bac57f1388a93452" },
                { "zh-CN", "c5da7c18a983236dda9f4bdcb020e2407bcd0c9db6f900cb1404b1c1f0a5bef2b2c2e9e7f8e53e67488a72e2b8791bad3e14439cff1e8babb7fe05875645aeb7" },
                { "zh-TW", "adf19400192028704a593dcc48cfff37e0e2f78838d5be935027448f03e638823cc4104a45d9272b2f6184edf55330cfb71bf4b461695b758a0b01cae1ecae3c" }
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
