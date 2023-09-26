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
        private const string currentVersion = "119.0b1";

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
            // https://ftp.mozilla.org/pub/devedition/releases/119.0b1/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "6553b8486b958b916e0a0506d59c560c7650c6af3978255a3e395a435567ddf1aae530317ce4dfda6cf3f500d01d9db9258446b3487af8e839bb511045afb093" },
                { "af", "e6bd374a2f3031b2b8d424613d4c4933bb44590328799ae4a030f1b14cd5c82ebdcefda0f8dfec66f52723e72b7f5174090cdce213b7a742ed35a5b13db47230" },
                { "an", "c3549af9921e4f70fe64e2428a1d65c2ff6b5e3b9bfc4817340b23513612192c78ea508c43cbd0123d7bf8ee48c17222e15b5e39c106094a4d9127d0ea304c8c" },
                { "ar", "61fb2682d191cd0bb6a2f9ff4b20690354f09c04bb0e96c446da45941b4d5b0dbad625d0e5366ea019e5ec1f8f9f8796df5f0020ada453af26c8525e2a50bf8f" },
                { "ast", "c30f731980f3b16c25d463db00b679b6ad3513484a2ac768e92c4763732c300bc21adaf29df1a446d4e685d615a6ac0e9558a5bfc7a8cfc6fc647fb6153ac5ca" },
                { "az", "f155724e997ba00253e376e1807e2b2a339c4787d8d0bced54cb236065578ac98a4c13e29ff788a1ea0cc2d21282a2844f570c7f40858a083e8715bca9ad067a" },
                { "be", "86117543c275e35dc2104a7a1f1c309f0c18c99abb35d7918a5d1da72fe242cdabe808f304185f4e40fc6f8691bfe62b3e6a2815d4bfcdfc57ea3b8daa3a535d" },
                { "bg", "6496b8a75814e0b5317fcbd22d9640c3115c31dbcff72c2081d7fd76aeba9994aeb78b7105abfe8d6cc814fd74af3eb318214ea1474831bd8aabb4a54d7a444e" },
                { "bn", "d3bf3c7ff002404af6d664b07403d261127ab208532b5d7982faa75e0825ebcda99b4bacd82263be0d6bca1a4221ea5441f6b52582d1e8001c095f6d47330c6f" },
                { "br", "8e92db0149c2684d57d3d4d43848c11a8c65a9728e565539932bed559ee2a59146a5732b581b67d9f2334b74cabfd84f1aa976374355208a5302b6407c36eef4" },
                { "bs", "67277f7212c1953827073cc9c1f6c547e5a067222f76bb77a4740eca493d419df31c31b584ad358c6b33b0c588bbdb67d14cab61fcc7fec2e0962aac9ed6da6c" },
                { "ca", "ec04c2a51a439f130ed4329bb909a83093aa28e834209f5e3b27beb78b22b5262a0c1a5061f2bbb3a927549df9620dce118380cbdae54ceb7b237bd4afca4e9b" },
                { "cak", "23cd8aeb8c01ce3cf3f4b95acb22d2847bc26d3fc990fd84d3fd6eebe58b31a696cc9fc6cc71c93ce6f932d2fe432c2e05dda20ab067dcc77127a350eb343f29" },
                { "cs", "980778c70bf9939fdc99d88c4c105530def05928843591eaa7f39699dea697103faaa84339150a0984ba37b3c1adeeb4504d5a0009959ecaf5b8b84babc6da56" },
                { "cy", "e9058ba3535c3ea8470928e55bc6f4694a0d81d41db307adc8be1967cd8f0742baaa656c62b2d685b7275cf382765446bdfe6dd24c8793bf38e20b0400c5879a" },
                { "da", "ff2a25e68014f092b09b68bce1ce46e27f37df3e224819e08e9db13f73de4a5ec42ef2b3fc73c64fa9f43eef464d3a7863cb7907e89e33446a0a3375416e5229" },
                { "de", "422dbc284145125cc4b007c81072468c9e38f31d7104cdddc169a0dc81dfbbe455e8f5913389974558ebac691c5058fd64aad481a1411cd9eb2497c7fa8bb1d2" },
                { "dsb", "f1eb12d5c316b078d3ea804a15daf8503310c37b3d2c389488e876525271feadc6d50e13cfeac40cac074a3a11f746aa89dc838b1ee5f4527cecf3df5f827ba9" },
                { "el", "98638f4d47c2a47c820c8610e218b9f473623060924b874d230e0fe8fbb6bf88c9c542303daf60515ad38492b14288b75a785a60262934d8414cece8ac476b7b" },
                { "en-CA", "a6412fb93b72640d9cd50f4a6fe238404384ae7a6f05d85da797ab1b459b0e2a0e4b772f165de370914ceb27f8fb0d2a9157efd9a85a127850ec8cc4faf300d4" },
                { "en-GB", "a2dc4019abb6c77fcc07091130283cc3b13b4d7c1ad5d164114b8177603fe021f5a6ba8648c2c993859f978b10e6d5376085898751cd633dcc58408b9e3791dc" },
                { "en-US", "7070d509cd557906536cf212b5ce564698c3e332d0b550f1ccd09cc798ab0255257a7a1b165141304594888e05184097349754bcf368acd215159afc983cbfea" },
                { "eo", "72078922097d7b8968fab5115f7f53e378a34fafe7dc33afb4e54ff079de09855979fb06bfc7a17c16a1b30e929b2ea83b9837ab74d6fe048b0443f663bd41cc" },
                { "es-AR", "a8432ac74fbe751f54ab870cb6e3c8ceeb766f1e384519ef0d066a68fbfd55df3eef033de44eadd4f44a59efbf72bfcb807def29158d6cb7d8dd018c4db633f0" },
                { "es-CL", "bf53feb59be2218565c52099639499b1c797a91d1b4685989115097af576f4491ff280ea5de2ee2816ebf146c74df190316e2a53d3ba0e53d7153bb33f6ba3e1" },
                { "es-ES", "7d3043caa225ee88dd1426ddb9dd8733967e2b450d9dfab7b43399be30d60581ba85763b59a5635c9012361c5320c77b0f70dee0b90e6db46ef72eb5c1aae42f" },
                { "es-MX", "30fc9c4f5ac91dd84180517e8a10fa96d3b9ff05d7e95e539b90510b3c11909470048c7e59a13855efe94ca016e4fc9620f39324b08d29ebbbcd0a04efb08bdb" },
                { "et", "2cebe29cdfb251c3ad0fd25ca700fb22fd1cc58ae1f6adfc8ac37d93aba537fe6399dd1c90bc8ad0baba1fe3832e4583a818ef9285720dada076e9de3c225056" },
                { "eu", "b430aaab9376b54540be4846b9459abe0d99d0e3bc7f4461335104a86fdc6b2d421b5feabb6aa86d6fa43ef9c868743013839e514f97636442972c8edcff6408" },
                { "fa", "22b925d31caddc86f88538a6e9eabf754053fa02d82ef31581a3d5c74dfd9d8419a6fd2f82d3f7d48015bf55333ba99419fba83a795e9c46d6f3ad0a882cef9e" },
                { "ff", "7d67ae30d9a973da2217eead44324f3eb52bf29b33f614f1e71c5ee5cb600d05d85edd37ac7e26b840e1ecf7a1e361a187bcb2e3fc189fcc171d689c65004ab3" },
                { "fi", "497852f67b2642eb73d286623716430364b4f96c29ec3bf53278d56174a24e5261e43d371cd32fe9c097a2d07acfbc7f1488cd595c7f53aa4ff36d7b0870ff05" },
                { "fr", "e1d56da0fd0b34c1b9b35a4ef52b1f0edecdd282e9f0de3402a1c94c0cb239568adf772728a92121d92c1a0e966ba79360b968054bfb277e8b49d62170137ce6" },
                { "fur", "46e79ab921efcc30025f97f53d83b148b24e8c33b3c3b080a65d5baa61554c5528b420f75ded2df3e76cec53b42785f35ced486b7c576d4735b08101d2dfdf70" },
                { "fy-NL", "5804885366e83070877db91d8c7d8c793d660dfd7b08a797153a8edbb5371c52fac62d35c58eb1f2ab57bdd64ee0c3befc426a9a0cd52fde27ca193d2d4b89cc" },
                { "ga-IE", "17b12084d8fe587d500a2a39e573ee40d3c1a6a737c0b2a5fced45e2517ac0ed82835cffc4e197f131bab6b840b7f73a4b5e3b119c0f61ef7c034b115230fcbf" },
                { "gd", "cf530e7a9d1002e6c9c91c49a09322c95e7ac3f1d7f6a3edb4ee2f7f883c4f982e312abeb4b6df5a20232c6309f285c6d9cbba15bd93ed0a84cabb60766f901b" },
                { "gl", "59ecf198559abaaec6ca6121d08752f54b27d61c7eb47ec64956e3089343c01e12208ffdff9e3ec666102ecd6a60a365824c0b2c08ad5e7075ecb86d5fb79f7f" },
                { "gn", "3d4ac909e50a1db05e4e6691a26ad89228494a8133640d59826cf2b2b0f88bb9590f173e0eac9cbfdf00d6776d80f5e28d945a7a7ef3f40d5d1ce3f55fcfddf1" },
                { "gu-IN", "827a85eebe43dc38ad790b32107916c60d2bb9b0fbcddc05a7889418b9f74bbaed21b95118a805127344f20af5b1dc457a459eada8f2eee4e7f5fc0faa05f4c2" },
                { "he", "7f60d57ae1d01acb53d40eb34aeb52bb654cd6460506073ac68f8457fed2fe08b6c508e7d3d0f7ceade672e6be5c866194f871473954383b9f8c4d5b93129c8a" },
                { "hi-IN", "3856effd6f9e64843a9c5984aeb934e4d7881e6b38cac3e64ca0c664b726403b5aa0d73bc94123ea87229db841606152211579db4bdc669a15b5ec97e866e574" },
                { "hr", "f8be6058718de3fe561da9904dfe39776aa0f446bf69f06839411e4814598a74f6b348f6d3fb8b9cd90b5626ed0f277a184560a44fcaed0a1a29dd5a00d04d7f" },
                { "hsb", "0c2f7286c57c4b30b9a4da27db47077a2efecebd46d87018a66959af42f50970818c417655f0459f04fe297f852b3ec00806ec56f3c0ed380a6e1056ce5308a2" },
                { "hu", "5bf75e224419aac417f0d70cfb9453a7895a0710859c51dbf3a1eb6a7e1ebbd629d808e0e1d328899be0887bc415caa2b91d193b8e13e3359ec174bc8c3e4d8d" },
                { "hy-AM", "0b173c96ba4cd01cc6464e441772f075226f436c2d9554fc3701f1199297c542f54eeedb31ad4ea84756d575cbe754aaa6aaf8edea5aa79bc7c53f5c1f074bb0" },
                { "ia", "b2e0b407ee65ce427a476fd6ffa826aa5ff86f08480b17314a32a84aa1a05b5a251caa568fabd696963491df1bdafb75ed5ff3cb542b08486459c07133069585" },
                { "id", "694a5aa39bbd50fb9776f009a1d45c3a66185868ed949f525f7f650be2ff96593b8c4ecb93b18073b482431689f5753f43e9455524b2a2b716b77d9bc3b576e9" },
                { "is", "18682f574bdc4a3443b94047bea132c13fb276561cafe0e94e3db7d995113c239fc587443e1f217cc754ef3ce382fc8e59f2ee4f8e974f58bdfc6238de9e2234" },
                { "it", "999fa43caa0fd0676f49391e0a1edb47e2244535707bb500c201b82377834446b8d9d692ce36da2a0594dbdc0d900c942a8645e9a32c392b3b43b3527126ec20" },
                { "ja", "09dc0e3b51828842827a03ef73c6330b4b6aeccd4518d8a38d962a82adc91c534668f180c3d3ef3d947d42498b4dbeaec1dd4f046bcd5f4b500f8fbc33e54c27" },
                { "ka", "d47949a7504f31f60faefc2d423d4edc9b7b7320ca8eff3e40112afdad857ff7b6e70f8f507c611552351e9ae158049dbdf985cfa01f25ad70bdffe736f8a94b" },
                { "kab", "7b672cdc32fae557dda79916757f0fb1406b036b193a7cef5c00c28c8cd797c29ce7778f3b6d9c80a6c732595691ed315b73f0c6dc45273650e6ebc1587d1c7d" },
                { "kk", "58e34077710accd29c43c4308b2afc85a55099fbf1177dc0fc89e6b6370b1dad6a2159dd29330d1c2d5838ec0c8f6024a50840e41fcc2b01c6009d89219c047f" },
                { "km", "c93a5899851ff24a1039e49c1ed514e076600423b06470a647035ada97b0b8f5b489ec4380f0e9cbfb4b202fe301d02328c6c980453f83c169a5f056293585d7" },
                { "kn", "6e4a80ea88353fe5e02add6cebe5b30b539270f0712c86207e14f11db1465f0f22fdf40aa3eadc47751319dca9879b1c66eac01363cacb4ec5bb0e1931587cbc" },
                { "ko", "dbe50ff7398cf029d61c5823b54faae628530e6454a4be4fd4b784af211688670b046bc5684365e65ea90cd11b6abaef38876ec55ebc192814f36d22ed656e86" },
                { "lij", "3304d66244c1bf8917f32de6daed6439c676f37c79ee54051720fd7e32a85111b90baf4caff2d7b2152b93bb87cb7e3272ced1945493f3707269fa46c15ec731" },
                { "lt", "4f8a9b3ad16e6921b36b8a628f6555aff0495aeaedf09b030c9f246e11f5b2ca2976d381657b6b474a45673023985d997f1a8a81ccc17c3a5d51dc6393325734" },
                { "lv", "eb601f6ac6635fe8d9946a029fcec10d6e368598b2e09549203666d503f88d296e4b7c2ca1a518d70778503b9d3849c86d089cd1ef760ea49745b51669d3a72e" },
                { "mk", "81bc53a2795ebf03924650660a60ad9bdbb512ea9303f5dadab0ef7be4f02870cd7e36637886cde0b66f031ceeeddfe4cf109e1d8b83fc0431e62795d25851c5" },
                { "mr", "bf0a35ed2025ac8c5502d3c2febd153397f72c46b12691e5a5f4aa3f31a2c917f67cdc298e8ddf72e1fe1833f52382839339d441b13f04f17989e8098ac2ae2d" },
                { "ms", "290efdf81dc1dab3791ce28ad2c489405570a064304ba7ded53f424d3076b3fc4e827bdac7d681463b9177d71494c8a2aa90978a1be584fbcb21847651499f4d" },
                { "my", "16bd752b51da1264bd2e735380674abdb4faa836e0208ddecaa0cb43949185c7cea98ec72f8829f0892903cef404f01c26fac8b7ae6fcd493b132f3d27d9be72" },
                { "nb-NO", "26a749283e2e2a68589c51090b342f81aae0bfb5e3691753b9e08e17f8d9a0e5102921945dd454855d07a499ce83fd0c865e77f4132836967cea30ab703ab8bf" },
                { "ne-NP", "76453206ca806980c9e9d5976494ace289b6a1027393810b17ea7a64b1bdaa73cedb67aefd01ae8e10866e7aaed7344ff17f504e25ce2b005d5cbe85f6dc03b9" },
                { "nl", "3601e6073cbedb1476c4f0938176b820c64f6e1dd52ee96e28a983e4dc0c732efd35ab53431c00138d6a577e231cb3abb6f3ddcde6d991dedd7aca6f227f7ce5" },
                { "nn-NO", "6881a342aae9d9ed688dfb292aada16a5698b4b6b3f2b2efa18bdee19389b59cb67f309dc8553f15daa0e5d7a5845c44a2e0ceffb540750642c00c66850f7af6" },
                { "oc", "8b62045dc7bd1ffeff21ee6babe9e0c27491a74efe16a47aafbe3d5905cd6acd73ced833f22fa9ae2dc4ec42e8e1e27abc7c9ed5ee5b8cd42b5f96237d82e8d8" },
                { "pa-IN", "a51f245b1dd3ee004f6db4eaa1492c9ca306b52ab22a3cc982e1a0fea8a88db6e9d0c84a6c4508fcefb660f44a8d1171901f8d26a5ef4c4dee4c8afebcaadd54" },
                { "pl", "aff27afd055e3eb3d47125686dd46db2456242a91972a00c072df2cf371d5e071589f2485707898a78b0d33b9b12f0d19fa6dfa89754a2542edf938bf3dcbba6" },
                { "pt-BR", "3e85dafb396acde9c2fc71578bd4b5724bee07c27db360e7303d37295824feae0e0fe20bd6f34c435080de65e3ddbf8301274f04a8c979cbd41fc3d839e433e2" },
                { "pt-PT", "6f77185e342fdb56da690d5b9bd82bc8a15e45277b35baa20f68512c13d129b2d421835f081cd20a6700a4b8da0aca6ffdfe7b059ba29bae7ea7d647456d5aa7" },
                { "rm", "53b3907047cb0e045f7d45affa5e4f11784206aae286d181998fe8c7966c0592b916f63f21f4e3ad012fd6dac9a6b2d28a799ed41391d6af4af7a8d2868aa477" },
                { "ro", "1cc6f85f8d0eb803649a7e5d05f9b0e37e165ecd27a16b24af7a8f17ffc894b62fa01a258a3cc11a8c08a743ca6f8cfd9f394754a8b32b2bb2b9b626f0b45668" },
                { "ru", "e9686c6a7875e9bffd251ecc8739739c3ecd682b3f9ccfd82fbcf1d2ed576ffd12ddd13dc94f740e1c4a4fabf3992f33b2c5ffc07dd02fd613baebcf394909cf" },
                { "sat", "3729deb7836b51d1299e0bf7ef20539519dabd42b45397c7735bd3f75de77eb9fda7291dfcfce997c60eae191c1d3b5501b3c6a2c881482ccde19ad3d6b4ceb6" },
                { "sc", "aad4b320e0d03449a8b2d8285a001bc81baca8d9004900fb58bb64f0bb657067a0ffe44fb9273b1d8f619d46d7e700df27ef57b562c049c745282f03c650035c" },
                { "sco", "bd43f0ed2d78a4acd74b8cc03da2fe2fd7e433e717ab59d2764f569a3894532e09d05dc13f65d48adfd7cfef48ce39fc9c0e1ab8fa056dd347d7f00b788bd848" },
                { "si", "3cc9471869593637a3583d567e416b0a996ca3dfb564e890894ca07f0b77832a76e183f7e835fa8e5b1aa451e79f0e52a3f59e63c2f90fc390995d33bee8944f" },
                { "sk", "14d351f6fa809e9058537c3321df2d63edaae29ae70f105db59b9baa91bc3a513cceb15deb27fefd6c1a6d61a80229f71a3699181e4bf4c08e1ceceb340f4068" },
                { "sl", "d473d96ffa9e22f5dda8cc829ca6bde07f9726fec170a14538e6723d8ad165a7537dbf7dd39113bdafecaba4af3c43b11fbd8e56d53c07cb49a2d833014d6721" },
                { "son", "d858e569066c8e116719077afc86c19a90cc47b19d1583d5af570db1a4dea96d13aab705ad130ee294227a2d5364269f1290618d1d5221c8152887f6b7c2128c" },
                { "sq", "7f50484dc704fe00ef83400cc316341ab13d8376f183129e9a1ef352896cab0e87fe505adf6471b6aef6e2c10944197857747ae349a21099bccfe7d2a749820e" },
                { "sr", "8a91841285d4765668c36aa398f1f0f2ea0aae12be9437043c33db3aab9d12c59b162b1fb7811d6e5bf81817630ac44d20c00b0fbbeb2751a1a0e8200bfeb762" },
                { "sv-SE", "44b172d73dbe7653a91767be21062d9a8cb3ed5fe9fb85c65e604a6fe4ea1e29af17ac8f50005de4dcd36fe3e12113d07ad282afa8787620af70d8c9a99c248c" },
                { "szl", "e4f19c9658b177cb3a5ed80f71819d5af1e741d83b5fa17dc7b4f76853cc9763087842adfae43847cf49593e94f143b034aef6cf8e72f7370729087d537c07b8" },
                { "ta", "19291e8d4c65067ec896ab1d7b0538fd18e8b003eceaf18e5cea5a71c57c68df9cf39d4a0806e75c676b4bdc384c5199c259e50eea57903bb159dcdcf3d09122" },
                { "te", "818028f5c4c895241a150c310257a300d32ef87862625b15445dfb025b94b8c1d485b440cda3ff40cf8d936194de2f4e412cdd9215724a2b55ce8f87f371e058" },
                { "tg", "bed38b40303a2f9709ddd36332f29ee2b5957a4c915da87ded015ed47566c69236db8edee928bc424f8fb0dc9c0fa1491cf4fd84e30509f4b469c484191c8d2d" },
                { "th", "a80b2ee274de30fe693abe22045ce8b7be43c9021f71a2d952e4a3f021b88996383397a58caaedc242a78611bd0add81cc590832aad92e8dfc58f47e7556f754" },
                { "tl", "76e61aee49062fc0f53830a81971f460586dd4e765add6e2e78a15c55621459ae319db62e86364e9e463c155ba7a90cb2a5d9bc3c32edd549ad30b13f3558c2a" },
                { "tr", "e34995032662e9a2c84d2949da7e9c01a17ee26505c0340ecaefe1f5ea98bdc82a248ca7d535208f2d8c894297805c862f30ef4991d8d14e70a1976d42f63d71" },
                { "trs", "ea35c051d16ad3e817c2cea229c59c61b5753bfe2aa71f536130c7cc21acd3d0f31fd6b3f011aadbf3e4c7f50a461508bd3f861ba37f36c237a6013121afabc2" },
                { "uk", "9fa92a548f08aa720452b20e3725de660f88d108a6ce1f48908f9a556590eaebbfc684e7d2a1e161c735e0af5909d91c1c4c3145b53425361959b7f991f11725" },
                { "ur", "8ba10b5a23766e434eec58e13ccc1e451d4ba80aad5969372f3650695cad824d8d8fefbd6d51ff08dad054f6e2477b2fa629f5dd80a133831c6064b404637fa3" },
                { "uz", "ce4243ce09ee66cff13cf6d024a226a5292d1a5120090dee1676dfc1b62d1d59886785f7c1ed8c7a330e7d036a19295793bcd4373a70ceaa06d38d59933b2d4d" },
                { "vi", "9809ac26b721648c0be93fb5d508ef17176d5df05545fd5dca920353fac9e36771b1188dd800c7f1b1128a3122f83b5340d954839c5a7646d72bf68dc5e54739" },
                { "xh", "cc6ad158f2a49e2aea211354e31ac11b04a483de91d03293f6a750f7971dfe64d9fdb5e94cdd8ab2da2e7e80c36d154548c7ef1e6a22d91d257bd1f89cba43b1" },
                { "zh-CN", "6a1bbcade5df0b74f7d3818e1b663bdb9d1c42204138d7bc571b21235e6325cc00710f6697d33dc780624ff5c62a90b60aa49d4fb85ec9b89e85987eaf23b2b9" },
                { "zh-TW", "024468604937b056c859ee4883c91fbc9ff53fb2cfd56e2bd17f961a7d427fa220482b4afba235394219a8a1bfa132cdef1c3a8756a061f55452ee055de42e26" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/119.0b1/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "f2d5a947c375278a041456aea6f7977cb8c51f66c0ebb937eb4b371f151705e406dee475609f43f68c09c1db4fdf9c3eb4c884a0d515ac37b292fe23af5d6115" },
                { "af", "9c6a3891990296ae3533b0cdc31778b325531b8885c0019e83dcdb09af6aa13f0174cd4fc5499db5b53d01bfa90d822116b86730355016c14f20a80b2c4a3278" },
                { "an", "98a1ca7f4db8d838847c93e529979885c20eb8e1cd35287be0cbf4a1bb72d924ef46ddf5cac89f6613eda00b6a5ef69766858ab09ab112d28113bbce0ba5fb19" },
                { "ar", "6772ab2902be19a72ae4d6863ee896f744ffbdd2ba5cb8c52acb260e4f9a03d0399678cbac3cceedd906fee336ab6b496193e79d69fc75d2ced4d6a02ac8fa88" },
                { "ast", "c1515cb31108538f91a1533a0f585e25216896c3943346a97adfea434da231dc377d41cc77c855147f530e55e7319c40b801e90ea736ea1aea0f4ef3dd2b7b43" },
                { "az", "9f30e7f6f62a35671921a3d21b1f1f4fda4ed09c4c0a21641f961ce1490ff3084d77038641731baa9d2498b3de9de639d33304d5ce8b761b7eafd8cd0d04b505" },
                { "be", "fc3d02383b101a199f97cfa827738a63db5c9d658d4034c3698c546cf77d93667216ebdef2b5d6afaa93639becbca43564fb41a066ebf8d51171b792eb250ef4" },
                { "bg", "9118b48494c3db9f3e39dff953c7c34452865127a173ba2b76a91e804788f25b4551a3a8d1f83f7a17a75c2d5a18d921e6e08c95b393e2311d762cdbd96be242" },
                { "bn", "ccf5bf2a6ca809dfc9dc9da3ed25c6150dc091092c8350a6388c4da1f744a07533e5cd560ee95c9cad21c76e8de665952fb90663fb9f673cc500df4e2e7bb4df" },
                { "br", "321e2026915b063033d18ca681b6bd6c0bed451dc23f243aeffbde8174977090944dd8ed00a1eb28d4717520d5ef9d637f214d8341ab558644b6f10dcbb8f7d7" },
                { "bs", "81cee1e207a8b788d90416988e5170fcb94d11d6b6d54f90de23931e387ef18ab5259ba33d2350d610145487c486ba85178694e5e23b304b1dcd60a24cf4c28b" },
                { "ca", "6770dad456314d3560321b4d330919d2f6733df5a62b79decc82eef99bfa99c88f17ce6e2fccee987ab20f5b88a9d4d5003d109dcd9f1566bfbb2c5e3dbb2e29" },
                { "cak", "aa997432004e1031f6120ffb393332619e81d437e470f5810fb888e895fdec884b2e6b21a29a7063dce52e37ccde8f88e06ed9ac1999544bb11d04efebb70019" },
                { "cs", "2906a45950dbc823a7474904e866965727b501564c48abcb7e16f8a12b87334ab8de3d10def6e60cd03887bea11a9125ff34a07899dafc4a8aa39866e35770f4" },
                { "cy", "be3243dd17e63412c904d390de72adef613b2799340dafb7bede33f75c9f4e90a0f7dfb911c69e925218fdac226a94672ee228ceeace5a86b80c0c6d2de37b3e" },
                { "da", "b9c4625a3a3c713644bb295de64e5e99c8af234954a1500bce102b757777c1f5669511401b83fc38c673dfb33e5fd497cde264fda8ebdcf2b863acd8bd55d5fd" },
                { "de", "c9073e979490ca788bb19e56883bb6f8a6b8e9929420b9663aea491a759db0a2087897c97977e0caeb60d998b0016722d0563ff1d9f32008d46161ef6c35bf94" },
                { "dsb", "ed91e326085d55f45e579daf192fce7bf3f5f2bec8a4000a86c6737af1cacc8af7f31efaf4a270c4feacc0573a5d49041826f1b771214d96d1b8697cd8d1b5c9" },
                { "el", "f6a07d8e84d9e8f1e9f854b5fd65b90d6e5d649683174b578459942c44fce9c33c661bff69848b42f1c1c99ff5262fe8377379a3dc594dccf6c8badaa61e839e" },
                { "en-CA", "538f3089d468b8af9db5fa1b5fffce851bd0271889e8ea9ada1a8b77c8b8ea901ff118c10f17bb0a985cd279631c8f4962ec28c4473d18efb8f4d9a3da68db19" },
                { "en-GB", "5b7d5dc992fa8df8f4b0b054665da10d37bb0bd631259860e7d051b10e7ba6e4ca236b97b0f96c76a27c98bca62a7a1b4d7cf1c7e9511e95266cef320db9c4be" },
                { "en-US", "a2ff2a328c6e96a838251bf64b78a2c96bf886609761810ae7a6d9ca36fc29383e8f5afd5836d59cf03825f8a80080fa26c3643fd7df6c6b3041d8f1adff6206" },
                { "eo", "1deafbd83b608ab9add0aba20f97c9cb3724a0fb1fa47642eee32ad6e605c9abd1523c53eaf9a13808f2143c42ae4af0ebd094418343466936aedf6ca806d016" },
                { "es-AR", "57323b44f1e91d5c158e017abe0674a7b39527fd46d6780fa8bf577cda1aa3d4d205331fe272277366fc4d9d400a93431b846ba198cde55557ba135f94349fc7" },
                { "es-CL", "20275cecfe0ebb1db59a3a2f71af6d1a90c6afa7e27574edd495e49c6bda4cb2fda47878a3baadd8c72c14785b4c5d71a4f5513cb5595bc874ec3e92dc55e00a" },
                { "es-ES", "228e9ce7ed17c444e077ec9f882017c23e6cc25c359c02764cf225e8f0fe9e73a00f6fe49548cf91c30cd3df1d48ed142618b939731598c442fa40477d842838" },
                { "es-MX", "dbcc3672371a3f8f28154f64afa10e3c910d93db0f19f5bec9fc5e2a2cc3f2778f0e05f6db339065c7ea6c41bfebc08def4a13b3e0c8dd4b4e47039ec0629b19" },
                { "et", "f7c0a86e48afd892d1bd260403563cdbbb4c00175c5ea66edc1605c3a2abc915fc2af5cc9a9304f4f11c5b07439bfd78387d387a0e7d1b18822dd9abb99f94ba" },
                { "eu", "1ad147cb26e3b7858e64c3018cd9a8e93a9817c98cb8180790091bda954e96bb644429fa4fe8b73a8e8d172292aba58e6b1b90f772df09aa924b2a4cf7a4595c" },
                { "fa", "2ef8e26f95cfbabc5fe1ad1d9dc298c84cfe2464a4f837520b2dde53c06e76b318d597d24f15f84c726be27b158c5ff2bb1b24a18a6aa72d99fbaddb5ed85525" },
                { "ff", "846d4aa38be2a4cfa3cce656e4320dda2d35dbc2d1b5f2d963851d6aa28810bee08214781feac7fdf52c78ad35589c17159e255ad951543f730d69b603f41527" },
                { "fi", "9a23f8224e9795a05a1a50805117b0ac7112668b0fa303530d720a107168b34af1048684fe3daefc83e3607351897e29286550082d0a08fb681064a0ff89380e" },
                { "fr", "c43dd150ba495c2a041b5fd4989a0a6f01341df1292d39516b0a1b257cfce46fef924eb0b7301f6a1cddb7a3977baabe00583a15774b155107992112cc64bea9" },
                { "fur", "1f7b4657e9ce92cacb94dc30c35274241346487a471dad317dd35d71ff81d0cb0a6dccdf160b68108afae9aca8c1aabbab6a766207ea76a5f905a83794016379" },
                { "fy-NL", "b9c3c5f369a3c0f97ae12fef2ef9e73d858928e4619723217a8f315fba1948964717e264de1120a2790242b7440fdca072599f2e7b860b8d5d38fc4818ef5d92" },
                { "ga-IE", "633970fa01a0948bcf1da49a6e2dd5fa79e111c3ff4fd21a3075be771102c092e5a2509a3bd63ce1f99270e1b2a306061c1430916e0e6da25e38e963ffc92d6a" },
                { "gd", "7fcace0f721dc667521891a57dde77d0d1608479c8734b198bac2c4d5009f05c2e5f8495bbac1386fd4318fc0a3dd8920deac2bd6c355673ab4c0f5d790bc004" },
                { "gl", "6683c839b9201e43110f8fad7a0634e6fc099c9b48f11993a7ab9a3b2b39cc7da25dfd795307b21c07f412a7a13f1939228030ad804b4260ed6966a8a2677322" },
                { "gn", "8f4c3a29d60f6e7a7708d5fa3d68208036994c310dc85fa08a54e6612d8462b6ba37dc3fac17845f2cf3983eda690a6a1fb0728a24a9fd2022dab77941d530cc" },
                { "gu-IN", "0927841d636d84ada45e4cad80ab62acdad14580c4609c65aefc415460bd525a76a85172c1f5287cef45d70a3561eeedab4023956364a1c87c879301000951ce" },
                { "he", "a2bebfa617295c963f92ef70b3f07fd8078d59b55670e261ea3a028c161a36b81e88cd92a9311191686e7da4cdd6bcf7aae01d31e9d29d5719210e27802d4797" },
                { "hi-IN", "07f0ead8fe4a028fe4c012492911af0a6470a4abbf59db863d8bdf4f1768749a811210f686b504fe4f16e5dee5533ceb7608811b5c7097fc74d96eaf298376fd" },
                { "hr", "2fc62d47f8e382f114dac82231a3b2d26053ffc5bf350b333c02cf435a87c1ec9205e31fa6d13bc1966f603a89c5eb7dd132c70f5ac10f357fc22ff2bf5438c3" },
                { "hsb", "dfa91c240a50cf4030c543e0ec9e353981a4d864a260d432b4fc913b472bd3c014a5a184c9fd2ea74228c62503de1f2976f7c4cf4532a7d51f738b4edfe23006" },
                { "hu", "6e989b91a8377563e2a1b841f4f82899b3271c6bf9d8b85f9c69a8f874193fac2030c30969a8bed1df54b882072c2c406c86ec38b542b5f8ae09e64a003e3a25" },
                { "hy-AM", "0d3ca5efd2cccd9e6706d573e67541f77e45a3b040b09b079ce2c89a74dfe1700f82095b4b303b4380aa0712357fc7d843b57f6a77b5ece39f21ac27751d2e32" },
                { "ia", "3dfa002616798068c640e49da22a097de10319755ca8bf9f4521292e1b7b9a21ec19f3e6dbdc15931ff5d6d4745a9559f197e3a13e261e4412e71d06776988cc" },
                { "id", "92927b576952ae9d60dc31259b7e9653cd6f826f0e6a170f96a1113f3832b207a0753be74df805c63a900a6910352c70dc38a45a2c224efc7d50e7d2eee5997a" },
                { "is", "0c42a5c26e6357bc652b2bcc50b4c3a78c5bf978cf78a77237455376d842a2dd7dc35aa476e0c8927c503f2a3adfb28982386487c2859b0a5c33380a7be1a64c" },
                { "it", "3c4b37fee3a09ba899802e248adfd95fb42bfce19aa682ec9d53ac98166e0dfcc50e91b225c2da22f91d2fa98630ff063ddab6c2c8e73262ec4a7f9f00b47970" },
                { "ja", "2d7df8bfe41beef100a8ed1d6228dfc2ee0be0c84a869114b66af73600bce919a3a7fcbc072c6690f7a4b918e972643f05bc64fe296907e7ddb0710bb7ded671" },
                { "ka", "c592fa798b93dca7867932df9369876e99a306b3c67523741e060a6d679fa2d6dbc728b66feab5fe742daad828ca3f787f0cde0c2571801bb78d66f61f65806b" },
                { "kab", "1c824c42fbe2ed771456a090ca635fcf4c6d51c2d82a49deedb4682ae9bf56a3a4e83f00240b43966fbb2687dd3b88ca9c3eb2576afe2e78301e9c58fdc43948" },
                { "kk", "526c528d7dd7eb0c292dddfbb6b8e7bb632e50066249a07f5cd727c187872be52fd2c3df09b4746a35adc6e323611bdc127fd4c7de5ef8a1149732c448d53932" },
                { "km", "f18f26bc431e4978685ccc702f5f3e1b6ec070a798538c0d2a7121ec7d92569089059b193acdf4e696afe5dd41ff53b3b6c14826cc1b044d2693ce22794b4344" },
                { "kn", "0a81ed809ee4c6fe372081a7127f58f3d1f58e4e40fda2d9b6ef40c4abd458a1ecffee0655be10df3b76eb0794ca19869f2cfa0077eaac4e6db0671c8aaa65bf" },
                { "ko", "2f8ba3c48b416064671d3e671e683acf82955ab06631f3d325d3bbfd9bc59d5fa15cce5e834e8b0f0fbbfd7dc42513dd0d03ef4ad2659d669381b3fa1889f5e8" },
                { "lij", "258c989fa1660c971029296e7e4fe0c3da02f9ba8d8564834f09ac4df2430bb7a7163d054efb32a2f0ddf3b894596b908deaa788714eb1ef661c47dcf5bd7a4c" },
                { "lt", "69ebd8463f8c3110af09abf5820cb67a4f536afdac4619a15d3e6b71b4b76e7bbe30a08ff4c440abb19ce3829cbf8342d1dfd280424cedbcf39f53abe7b17145" },
                { "lv", "6e4ebba60914804fa9bb1344d3422836399cdc4e05bdb71c1dd7897291d89b272eb662581234617c5a4c9f1a2532d610e73eca3b264e2759bfd760617e7a1a47" },
                { "mk", "f8b70f54d0d4f2e498f1315192bb41fc75f0e600940c93336f43b14d777479a43102171c1b783f2d4198c96052461e3ee8b5c84cb05a9a4f14ace51bf43e3fcc" },
                { "mr", "e0d3f5cc1a6ea7a7990426fe6945c88d366e9ace80065cc22188a434c4080119933590896e9797c0f36e97fdea9368adf4e7bba2ceda86178b4fbe9b8d676175" },
                { "ms", "f267f091aaa4ad8244b7acc0278c4c5c8bccf16b1965cdc4e247c530aee4158a2855485a2a87381402fcbf0be23b38f1b41bc5247779ff5addf66b227cb0cfa0" },
                { "my", "4f8c0b72bbe94a5b25cac0b870dd3dc0808b6b1697131e2bc4edc5f33023a62401b0d99b6c163b8e83077d9c18dbd20c276d6e86d985252ec9f2579c4f59de69" },
                { "nb-NO", "7401b80281e531520a2625535ee84fac4b6c1a282294e314719a5349339a968f76567069b558b5390544fa0eb605618ec58a6ff5d510dff8f7c90df8ea42270c" },
                { "ne-NP", "e29b1ef0ee6de384470f32dfc557807c9b572dd2833be124d012798dbe46c9400ec669e4e510b806ed0a42c03376b6512139860933640774cebdb8d3fa8d6539" },
                { "nl", "627fce0db6f7616f2689592755d672817b32776a2cd21a89491f710f4f59a2ab2ffde7306e4b9f8123ecb8bd6e89dd265eb229a76ac82539b5d7357c80363c00" },
                { "nn-NO", "32abe0c40b0a10fc19cec756b105d250cf9f9c1da8e5d510ccd459db21d833e900e880a866efcfe6f0e5b4631235239aa571b07d0581e623604a4f7ead8318cf" },
                { "oc", "8e6aaa5dc82942cf85a35141a3060ff1d6e6bffc71958a2ac2ba613fa6cd79068504e89cf8df264f6f8c869afb794fa606d83f5ae73632fad96a57d9ad02cfaa" },
                { "pa-IN", "01916ba8109b16e8640710a62249724958e709ac1ecbaf106715aeb6f6d2af3aea6d89784633eaeae243f069348e517398c615cacd8dfd4a38bb259710e547f2" },
                { "pl", "94914e54be246e94b92e660f7e50326554439343b720ce425c01429adc0d8cb414891c080c51591025983dac5d78b4dd99719b90ce7bfc5056dff38f1acdd1e2" },
                { "pt-BR", "d3bf2aa70c265c3182881526947c815a66e759e94950dc2a58b7baadbd37ec01885378ff626db454fde8ee60492bfde3fa511fddbde8f64b24b511db33de12eb" },
                { "pt-PT", "eb5e67e3088a390dcaeed0f41744a73481c56ce2d2e44ef8b7065117ce0c02950d263809e6bed0a8acbf301760aecf4fb85e3fdd466552c04c48f9de226f3914" },
                { "rm", "9072714b4ce9c088474db2bc01ca2316d181ca3def9e7ca3352b00617d7b2dcfecca8b16280d9fc173d3b64cc610334c44ee7b1dcee07ff7b61efa15ce49f1a8" },
                { "ro", "d3f6bc20ffa101cf3e59e84be4cf5a52095d7d1810ba809874dec5fba51acc80371ee261815c3e46d35552abf5c1f9c549b2fc478b10d1600b3f872c22fe7fca" },
                { "ru", "bb07f326a844717b26169a92427bd1a0cd991354d58a330951b856e01726e91ea83f8dc6ecd453e36363765ebad12dbb2728093612e78006582098dfe30d86dc" },
                { "sat", "fdb146ca054461e28c09245512f244e5467beb8f3e0ef80e3e00764f43560922979bb7b1c5b07f5f77b8eb9e898f07be1e5f71c48f6c6befe6b5ad71deebcc28" },
                { "sc", "9866ceb942d020ac78d29a6f7b646210debf7c0fd6faa7f16273d28bbc6fd1fa29f54fd9bc20572b034fb08e3ec991588cfb802ab276673866dc11f6d1c9bda4" },
                { "sco", "8edce566b7e7fdf4005fa9a76fb1571ebd59e731bebb670e95b75c543687218857ea2747115c631d82245c6ef043d603d9cf1e258327dc96eb88a267ebddb8ca" },
                { "si", "5bf03e03d157297536c75559f779e4a4239003d32ad46530df0ba160161ed7eb0dde7ee0bc6184cb1ab45bc392b4bc13141542616480ef71052284aea70e2596" },
                { "sk", "2c9d78cc5f3d21706686d54d756868478f3e919207cd29f6aa2e2a7dfb6baf801106b380e92945641f9181d53fe9180166f4bc14bfe1a8482b324ef66f9fdd4c" },
                { "sl", "d1b6c9744bde984a9a652d653ab40b42ec7edc22bbd3b2b21746858297265a6425f877d9a9830dfbeb6ff94bd9d4b2b0dbc6f062ac363126d6e1a1be7a347a9d" },
                { "son", "424be02626bfd35386c9ec7f3b3f035b7559597c1be177f8b674a148ddc44d2d3e28241cf00effadeafafb8e27c1f50a337282b9e437eaf62faedb518f3a368d" },
                { "sq", "0e7a9f8f0aa18ec286e3f33c75a5a13fb16e05365a7ec57306db7021a4aad65705cac3f94490f33ddb5d2eedc1bc33a4163abdb4c664fe31d77e27b608aa322a" },
                { "sr", "2a46cbcd2445f733ab1707e6808fbd748c4b4ae33f65c4cb6c4a99d84129198aaa9f8a8de3cd8ba04242efc23218b878ee95879e97a91ee6dc75138359bbe088" },
                { "sv-SE", "ab6bb786c28c26b15c95cf40660d881fae9f148c0c0cf01bb8bd2e60dc5f5f8505645bd8b38061cda8a020a7478aa8c3d4f8b9e4a4233bb46e034b312e495e30" },
                { "szl", "12546eaa8f226016da0a594decf034a5334b32d1020d0078d693ea23dcddc1694daac233e8b534eec4088e6275af171c137ca96720b71bc1e1c21da187f22581" },
                { "ta", "86235c803a26a2c8481cade835431b15aa9e3f3ea9e13ad422d96ab116a4df9ee9f8b867d61ef908ca5c1e7b40fd8d737bf0f1b37d700c7de6b8f70a51420366" },
                { "te", "6c40d7b13035c3d887cbd97b3c9a4cfd8d769e03c1d2cc9f3cb0fb65d49065402228c1327b96018e1853a93e6c3d051809da5ebfb6eec0ad687d42aa613b3438" },
                { "tg", "97b3b5b700ba2ab93c549668f6fcade59bbdbd2117ebdd90a9f75f1a5f7047a1bcb38f0e55e9bd52bbed42769e6b5f1ed0ef46fd78358e23759153800630b33d" },
                { "th", "637b4131876963db6c7c0405e5a185165835798612e31cc642471cbe19f04dc55bba916f845a5c4a92e5321cdf40981bce05b4f7915083b32d34ccf9b1c8b18f" },
                { "tl", "c6ca064a0fa989daeabc8ef4bfed6718f580b0ce8d3653a9f424564ce25c1893cea106c25695fcac1fcbcd174ee76105cec276362c17f5774482e5f3933eb6a1" },
                { "tr", "fe7d8790369060f157b69f6e1a82fa24e8bc4767737cf3102ddaeca98951515f35097884c19770b0c01eeffc3eb5c3269cdd468e912d83bc3bc7c26634f33868" },
                { "trs", "54f695def22a7095661d222d718d1850324f76478eaa5462921c0dd99e1cab00dcf0207f45953c628e549885ec675b5edddafb967eb8340a91fb169edf95788d" },
                { "uk", "c83b5ebe6e9925f07ddff2d965a7f54203af2cba18d1a0da05039cee5fb872f0cbed838aaf90e312b66c2bf2799a4777990a3f4527836de1aded9729bc3beb2f" },
                { "ur", "47fd73a7aaec6c340fc4e3603899bc7433e89dbf80ba500f32a0af54d7b4e473900205cc12c1c41e1a425e4f963afc493081959e9cb7b672773df285a7cd2185" },
                { "uz", "0db79a776ec773590a8bda7b15bf240e2c4926154c263ef8613e262fdbbffd4d31a3d607e68fb9176b1add4c1d269d2ae6753d871ee78cf57b10047c1819a579" },
                { "vi", "b872dba7e3bec62fc7d95c7859552306fbdce69e782042c424cf33e04bc1da7584e094e29643bce448889bd521f093b11fb0cd008e23d845ed27a6737ea1b41f" },
                { "xh", "cefc676fe6d42dd8a3c3a656156ef30ba9322a299a6883da2743106e7d98b6e070afce87a76b588310001aa6fe528298b5f6ad78a4c3478640e08c58cff97c05" },
                { "zh-CN", "7e2645ac855835557aa5860ecad29b0ba1f239b2d2860090463087fd0b109ffd6b8bcd99d7851d5900f66da247f49e892448cac91d1b55e72d3b6f0d69d46ab4" },
                { "zh-TW", "c6852462e99113b7bad46699f058582923215d2e158241326d1701cd64cf14fe76a772ffe6f245f45118f85216d4a608bac319be3faadd57c1dc4d9604845585" }
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
