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
        private const string currentVersion = "91.0b8";

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
            // https://ftp.mozilla.org/pub/devedition/releases/91.0b8/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "d1a05bdd0234ff036ed1f79703bbf6e8c63847763430d4537a8ab26352ec60bab18a243974810641efce826ad7f8ed9b58c21e15c7341f9d9daafa4950b6c52e" },
                { "af", "fd1af018951fde105c4f2d61594cff98b174e39b92d7146a5db8b2b284e26cbfda0bbc0de1fbf6fd84019b784b2951d0c92f450fa3aa68af1c37c720010fa5a8" },
                { "an", "db2d1e5fb8ff3b154d6373e3746dedaa778c372282ec0be3c37dd36ace9f20f3243d5b66871a16fc6906d2967f8cfb2ef5ecdc84c8b1dcd5032fdab75298a0fe" },
                { "ar", "d147a4e3395f342dd424ffc29e7e853f0a9f35220894b5669f3b01a8b6a4a1525f0e44e716cba4ede4ca5fa3cf758d898a95c47c2a1fda8773e4ba56d7e69f33" },
                { "ast", "628de18ddbf47f798569ada4e9e7d91cc7f7328142aefad6ff1468854b42778bc4eb311688dc23472cc5745c11fc644824d8494aa57f14ab786776e4fb681398" },
                { "az", "c96976183c949b86c34c161850591e3b4fd58c7a5af1c03849d8e9c03276331bb34796f748efc668f1db7d9d998f49037ab96a58522729046024fe15b8fad45a" },
                { "be", "5167fc10da39bf35f1c4e62ee0417e1fe6cc252bc665f086b7cc6098a4f149589d6e7f6f89fc471081eba8d58dc6cfe5b1cb062fc407a9b4159be342fb06d35b" },
                { "bg", "3423bb4ad3b55084c3a160812f29d6a285294351c78b699a736e67d3a5ab5091551204d2cf731089291190df3740f8c06db8f85f8cbfbfb3951431c4c42ba2ab" },
                { "bn", "551acf65d08ff8f73df454a0c30e95d58200cb7a558ee8f6603f4ad5a3c162ff04294cfac3b69f15f6e833e6370b18ec5dc92e807c589839c522af3f6dc98315" },
                { "br", "db116f11b26b99f8abfd7cdf7bffb72add315d8dcd5b8f11051d06b7cb18cc7098091345ff8614f5f6ef5efc9f59e00fa25c0d7e2a61c024ddca0359bf36ae36" },
                { "bs", "2c4800a5ae873d39e288d7c67746a010ae968d0a8b31dcfdbdd03fb643f26f6adca219ac2b09cc7ccc35c6557bf21d37f294fedeac8c36bdd0df9423b84536b0" },
                { "ca", "628145b8b315b4a3c0ee23c3457ba6ad3ef3b053cf804de0535c9712f4f13ade0e360870e5807da4b5a4fb3d18112758440fb4f6d1a1f2c044578e12f9fcee21" },
                { "cak", "0b7ef362cefc2677ce6a8d5104e94a3411d96dda761153a0a487f502e915b345a6c63a1fb52214a0b00967be9f1bd07a55b64879487f9aa6756b564a239aa470" },
                { "cs", "089f67c5711983aa63978f76015d58ab59fbabae7ea4b2317141276814e1329304f9c19003c8086f1fdc1eecdb38be5725374cf6806d6cea481f86b72067a206" },
                { "cy", "30c996336715f9e001670fc5cc0e1977d3643f431ba6731434b4cd118e31632fa5467fdec409d9135ba5ff551edbce8b035fd54f8d0046e8c6a6d8715ca27c13" },
                { "da", "977128721bf085b2f038c04d5068814772f1165ddd04e89905f41fe2eaf2086ba812adf29e728fec75ba11d48c597a46dc652210acade5770557a69a8b5e2163" },
                { "de", "aa2cea5e0f687bbd8591cc29134d97d2987d41bdf99bb81aa1b7860e0c902ceeef50842fdd2afe60b1cc4939476bfe8f7c6f6ba8c25cd5a0c4d6ae9c75f39fc1" },
                { "dsb", "c0792df1bd73f5ec12a632b56fdfa0192077a1ce5e6f7e8dfbcc6cfcb347684fb629a8c8264582c295a65e14510ca7b216e814a837b5797bc4d3b88e19629b9e" },
                { "el", "afc6ceabbad0b42762761d3459856c2e645af6543bca7541108c2dd3ad240608d9e59a8d9ff8c64d57b8fd164487b129bd050df8b01dee15805caad873b8fd7e" },
                { "en-CA", "98a0a7b10aa775d6e82355f7d479144bd44d0f17a779f016a21bb8a1ecc51a4aa63f5216aaffe862630e57c203943bfa29a502b5bba1fa2dea50dcfcb983a22a" },
                { "en-GB", "c49388fd9424364442f289993a64eef2cfb41416c4548477ff75a512129697545eb1c5a7344e668d4036dc1700a28b5b6d2ac69672159bd484bc8894149cd327" },
                { "en-US", "5811070a99fc06bff68536f5bf72472c6065c3f0f02985c09612280fb11f3a7dffa396ef3400abc7570b4b0524485e3b4087a9fe1881f7b3fe6630151aa72e15" },
                { "eo", "69653ae42efee7c2ac8dc6617e2cbab86597bc711c3831bc2d31fdc130174a1706532992ec12aa9624cfb26e8e3cc49fc4592dd9e1be334b1df508391c226025" },
                { "es-AR", "cc8e6aa0c349015d92e543240cf2ccaa1ee9787dff8441277550cf8bfb64564644ad07d400c37f837e15dec8b722927654f6ac6383969007f0e06acaf95c5973" },
                { "es-CL", "3007b408e1f6972d117314abfb9f2da8896b654900126bc0c835c66cb693c4a9eb9646acc40534724cbda9432fbeb0fcc8a2da96bff5b099c0f5d342301b6c7f" },
                { "es-ES", "0e4cd4cc53919b15e8d91f6eef47972b86e3c94475b8613d64108ab2bddd62a715b0168913f75547c3a17a948ac6b28a0a102d72dc0c5b0824d5d07864ecdb58" },
                { "es-MX", "a4fa57feae845315e67d88cbb57fe8d96990f8aa3faa5fe6f554bc5b85c296e5d3ce261d520fe1761da366f8a29886c4c23986f29228b751c0ec4a75e4d89e86" },
                { "et", "68adb4046a5356f727a563614f9850d20662410ed36fafa1a2565d7cff7f47c814b590640dd59079e97ec070f58c98346c624abe4bba0148318a886d43c0978e" },
                { "eu", "3bb70e36b0642ede772021688caa01b1a637343e66a0262888a07fe63c85e89fdd895e2f3e9d7243bfe5b67f4a63244062ee7cd7bf04165f79d05f35af607b5d" },
                { "fa", "f4a8026af56439c747469961b29c1d6caef8a133fb0e5299a9713237ef9ed09ea12c82b220f7c557b1f89c8ba772da8bbda7d9e554bc8af54c7dbf5ac03ab228" },
                { "ff", "2d3e1f365dc8d8017e9eb30988445330da2dd5785232533de3300a8546b79888dc374a3829b5edce2416ec9bfb9fa249d4746e59cf5f31987ee02cdf8c23a12b" },
                { "fi", "bddc5fa00f63226b035041dcfc523ba9416a4ddecf23d36bbd1db887a27cd9fc96376708d616c965cc7f11100a8f630ae9d68935d65f2e72e8f62da68913571a" },
                { "fr", "ae4d4cca6f3c069d9a167601eddb769745a6659e42c235d43e166f91c861db2ba364525b8a0a7fc726af2047ab926e8e831c1066e38665035a04e21df236d5e6" },
                { "fy-NL", "1f02680e1ed3486f845f6e6caa33bd541542ed4ada19fe574c32ebee1e5b00bee7f094430b3f782a1c8883747c7cede1959b6a4c3c21cc19965432970c089b6f" },
                { "ga-IE", "22c13341205a105facfd09e82bd97a4eb1b5b5df7244e9f3c123e54fce0af1410b89643394e8ab4c4efa84f6ed60c743eed3d211938767230dc32c220b0e1a6f" },
                { "gd", "e745f56d8c8c8af6375235e4b123b0f4e40505876aa7e6bf155ba3685b88c35c132457b28bf18eb509a648ebe38c1c272087cc853d4ec3da88c8e2934f2263c9" },
                { "gl", "e167a7443b7c5ffa92d28df7f39b7ce41761503588c301bc30cd52a9caea720670db3ef45e1daa560e9bb49956892c33f35c2b96eb4fec2ba74344b9b87ab29b" },
                { "gn", "007c8e0471d7d83bc2782bac7290a68deacce65ee6a4a9193f69246d72554cfa0dcb0ddd19fe5744b766e2e95ef9939398b0a8d7034b0c1c9beaa446fd8e5b2b" },
                { "gu-IN", "e81441ed63428178e09e5bd8dc9a7aa3b6a9c8747842622fc0a3cd61a1d14d6b5008b3a7b796aadf17543dc93022648f302e558fb23af4c0e4660110462a4eb4" },
                { "he", "7089ad829520c2760185c8fb17544ecd0d6042bf36b97e60f7922947aa3ac4f901ef9575b7e510bfb1003d6375f8a11052d3dc099432689f58566d80629f7633" },
                { "hi-IN", "ac9fe79b9a96b1bd3aec71b28fe215c61d5bfd2d4701937f1a832db386bb2cb78042767f6c146747454a7807873c06b442d8228561072bc4bb55f142b50a0248" },
                { "hr", "54335f40d2226b3dc28936a3cad88a3a7bba5b0ef8a8105011cc8d078a18d008ccbf691b1f93f369fb67fd512577c3e50f1bf7e6422fd5e8c3b9f4a2cc99f641" },
                { "hsb", "c871d12eb36b8fddca590b77dc59dc2c66cf0ea0d3f99da87ba292b69d4fc8075bf1a3b033da85fdd0e8307477f4ef313d235aec2d2c006ec32bda20cb127c5f" },
                { "hu", "fa424c1813dfd319a173018509410f0b25a84639895a8c10757b7e8e645562bc0c9a5a5d48b4e3d47f575f50fe5dcd18396f04be23c832872bc2e649d2e4f012" },
                { "hy-AM", "417471bf21a8c923aa6e64f52b2f98a105092f4d167712d1f176366d109d55c258d39d3faf568305ea6d9181ae344b606c53e8e00a9eaa9205ee65a915f060bb" },
                { "ia", "bfe2728b633bd277c677791ce699d8dcf4aef34612f20ac17e668e990dab52a0f8ebee63464a3ab9d437e78c058eff8ee74d7a58d178ed5f377e9f4f508b1f53" },
                { "id", "888bf75487449e14d1bc85580abce7e0e5d41182f5aac05f58c9d8bc45d2dc54c8f9d39777a89d4d36a93194e46c1dc8c3ec8b51e8ba2516745b6f2283abcf3a" },
                { "is", "cdeff065cf5e55b9949b0a17032c077949fe1316c030e34d4250c11ed32adc1fc47df945e98729e0b3bd34bb1a4c8b403eb9100f188cfd3eaa195cc8ae1a27a4" },
                { "it", "c62893a2fdcd38c667392c672da4a129725430b4afc953695e9802b356872fe20e9f4a3377866e9f296304a19890680b438ac139a63800d11ef811a0c58d5c37" },
                { "ja", "0264bb10eaa23c33248fc7d2b574c951c2fd8e0f7f8d05fa5d5cfec77ce164092c151247324084ef6d1888e2f9001c3444d5a54f4d58a0d3d355d9651ab13dc0" },
                { "ka", "5f4458ed5178df85b13a4c07ccfe874d9a61d50ac21a4c3748fc60af46ea529ee5620614001cf7a064a807b27a4aa8d258dd1d07fe4f3e739f12564da415ab8e" },
                { "kab", "7bebc9d6c177a1451cb06c13717322712c45c5c277d069cc656e458d058d1053ab42b58d6c8d114c51e7075164bc1aee74f38c5ee346b0529a507099fd3356c8" },
                { "kk", "f1b42dabfd4c5c3fa5b1752a84ea5d37e9bca42f272833e6657696963feec9be8bc8fb17ee5a707092a2fca475a9c21ef91fcbac3f66e64c5d5c105c8bc275ef" },
                { "km", "e5b2fe76b0a5750b63ece1091cbd302f6b92311b8e00394a15c69be4247828ee3cd65df3b85d373a1148e3caa5cfe20849c1040e2e0d4f6f92b62b8eaa6a10a9" },
                { "kn", "e41cc04bc352ffc611c17c470347f556cf161b9fc66ca43d798371ab95038f340dd98b678e8d644601eb2be8fc6f4c684560b03077f43f0bc153f13e9559aca8" },
                { "ko", "eff18dceab98e12613760ebbef1f6e1ad47827e8dce383dcbe419424166370f0190edb6ddf6d1d2e4f263a20f756032dc9bfdb332c76407c461789071c25cd63" },
                { "lij", "051726b8a979797cb39f799772ad9053d30a331c3fac4a13ab93d9b0131bd4c56de39b9855a2ce6b9a1cc8583cb26d0dd3631305ff2f1d745529ed66999d58c0" },
                { "lt", "8db8a7d36e071363e14c840961e01970dd39b6307cdccd8f4e70ba1388777063f79f7a0d1c58169403fc6c03d7978a37f82f19ce092bd5f3e0248b6b0a2c9e0b" },
                { "lv", "8984fd43182685cbf519dcd5d7a33c94455fe3c82e8f4dee5ba96dcebe96046d7a8cae257fc82e88277ef5e52fbc19287530b00599f9c1234d18d2dc0686ac77" },
                { "mk", "0fe795e8f8da9c0565629a2e42750bcd8e3898e6c785c4bcf12e34baa65d3b1773ed9708b2a3ce6d2ca96eae886d560ea5011671668f9e05d40a0312e7b29a84" },
                { "mr", "0a0ba113801ab29101f224152346cfd4f024873ae164c7ee3e956aa62203de596ed50b909ad90d3e8c2ff821ffe3912c5b276eb5767b95d61f65fcde076f5225" },
                { "ms", "111ad759efa3ccb5669865483ba6f1c50a5fd7201d3730a384f8c5e87ece8173e0e00724d53517de56df7d00dea8b8c690eeab6279962d4571b4a2cb1a254510" },
                { "my", "380d0cdd0a889f05d381a6b37cba782eb3fa6c1764cc4675c963b71f72610332e0f082bc48af9e41e84a009c40a03231993fef8bd60438bf197df46993ca7fd0" },
                { "nb-NO", "9ff51ace4dbb7a81292f47613a7c9707b31c2c465c8f4b2f5cb2cc40b648bcccaf5cca2eb58fccb90e7fc1c808a78320a5556c65d514e1e4b34e832f32f9a0c3" },
                { "ne-NP", "609b80dd3f529810ddd2d95de19b6f87e41eed425a085716406f90f7ac64409824859359712c2022a386f7ac5580de571ba51e539492bc406df1f38aa22f47a6" },
                { "nl", "30e2763ed4e1a35701bdddca5509f63264e9f5866590439b21a3134088c74055c0ee34e23f4abd85bf1c8a9281e1f10bc3ccb5b392f68ec2da456c3407c96c8c" },
                { "nn-NO", "add9a61ad1b5eeefaad95e8fb516c72bf68c357dc6182b6c5e7f2225d78041a48ebd01c80e085822b5765a47c039a503bdc7b510f8a857462767cc78d4935229" },
                { "oc", "f32904b2e1973829a5c11df91fd81deb9d527e1c9a57cb88142a785a825395ca11d206117e17e2ae25e70c52ac3e3148c18a07f935010e9cf3f7ad819c190b2b" },
                { "pa-IN", "e7d42f284fba0c1835ff247bc85bfbd9d662ae9a84d8a763f02db520a1905d8fac424e60d7d31e9322edd04b344874e445f7d124786e99e3a16d87c793f9d0bc" },
                { "pl", "20101d21c2677e68efd7fdae94becf0823d5c334a2d5dfd20e2e257d50a5ba5808c2a92e2f52aa7df7ac8b9e3bf081c04f5da48f2540d9983900d29ed8f3adbd" },
                { "pt-BR", "e90dbf31245c8c8f655a9ac33f73e0daea7a073a03559d82cb7506b7c1a34a2a4ab5a522adcd8d3e6770e7a7d6a12cbc6433e5df3fc5ea1ce18d60aa1a0c7939" },
                { "pt-PT", "63084f7d57281bfba7fe08aefe2cc67075b6aa02018504cdadcfbfb22f0fd1065225bfe6427e5e8eed3d8e27e11c634f50532cbdef2d41b74c7aef190f6dc56c" },
                { "rm", "21dec0a9efbdacf5cb0ada649339cd2bcadda40301b12d015f2e05e26bf5306802356ae3cd5c31727b9864f555703d9a57b921aca0b4c31b182a10507d70e90f" },
                { "ro", "ab80957d90ba8447a75df1cf68b1eeba878382558b1639596f48237a789ae6488022ad37ed6749986948c79e04c1f524c4ea7fcbfaafe85f5b25da424a073ff5" },
                { "ru", "73ccd6b7a56eef05f397a19ba8f98f3734022552057ea75b88b7a84ee741207738a44371457dc2abb5486dabe158c846e947f859c6ddd25bea27f671722cae06" },
                { "sco", "e2f361af180a6a69d80e4dd6a72852ec0085eda77aebf6b2377109cd27c69d80d95efade5eca90efa0a35de4fa38250d27cb351e3d1502c2b279e2d29ec6f1d8" },
                { "si", "9c007ab649d02818a6442836605881baa912f2b4a27e6c3420c04427ff448c0fbad571a8e5096c3748bb1049831c5e0e39dabfde679d8866fa2977627a73e6bb" },
                { "sk", "9e47c8605d1c3320b3788a51f256a29560dea76a9131a032b919081d12c370edebf9b6200a863ee67ad1863306c37ec99be2fa54b3020ef8c84ab5d96f902930" },
                { "sl", "e8f898f3326e97593512341826cab487496331bba370c963099630f9ec67715fc73f06c64e346a652e099eb090cc20beffdaaea639cc843dd4159acc39f3d3ca" },
                { "son", "44e9bfa553c2f0b4735d4b0467b7f98bc39dd47073a15d520f5ec91edfac3c9357d3781576daa99eef0a1664d7e884896df37a3f27fc236fd47c9058a58a2711" },
                { "sq", "e56be84604bdc553996021a45fd0c85a6b97867d7e9880610f8a085cde0dffe2b1a156b183b538cee56453708dc404c83a51ca30f598b37c497ca40b3f613174" },
                { "sr", "03f38ac6570a46cefea0d6686229ced815caec37494bc879bd86e080ce0cb7db3ab62ec011a51a03c356dce17d5b69e31e9a478f54159a0c2dd89d3bb37e9b29" },
                { "sv-SE", "a6629df66c1f81a65e28569497741949160880e06a5d8c222c57c6fe8bbf834dbba7c47584d2af68e23bc52056f2d326dfa64d83206154aa450649ea557f0223" },
                { "szl", "d54213261dad282d56f9561c1efd9f456b0064751d233dbb331556fdf070f983726ee39078433cfbb430b61b00c9a651c30d15a7b54952fae3bef35702a9add0" },
                { "ta", "621763d0557a578b7d564ce3919623c4e4c2ad86e69f23dfeae4a4d98ad5aa5e2281ea438c49f6fe9ea094b953be09d4390fc8f0c204fa846bf56c14b12a0175" },
                { "te", "2a3ae8f39852e7b5c1cc3fc657aa69350d4cc05bb0a887d91a2efcc9253879e3ba8dd0bed5256111a0abb37661350c60cee8f87673edbee37f129b6ab5250cbb" },
                { "th", "f9d5c0c3ab0efd37ff2834bf73a1780931dbe1398003748987919e6d595d86df14bf73730d791eaf92f0962f92913264b3b9bf36e0e187b6c4a3510a391b0626" },
                { "tl", "7ba3b6935089b3974883cc5c2b164da8a0e00e0084d907bdb3f39681b8688e0ff7bc4407d99c9ce93835452c1ff1cfe762d01668e6957cf3c56de11f473174f8" },
                { "tr", "13a6a4496d06f441516edd0aca449625aff25f7025bd1462ad55e6afccbe8b7639b55bdf4c34458f209b29c855957985d503d09aef98aa52f3660a66f8555a7c" },
                { "trs", "31b968cd711467217269e9c05a50cf3e6c1bb784fa7a71a0510b9dc527080b08c93fba1cf515ac5d780e8dfb7a6a824e4b758b45fdfbb07e7a5df98984426afd" },
                { "uk", "a45550dff0ac6e890732c08ba1fc49eaa42953ff6399f00405f8cbff38c22d7d830ac4eb979dc46501edf842a477e904964c39a95a2b8086f1473e03127637bd" },
                { "ur", "7e2e44745a797ccfe10b352898b2a203463850c957da25656e88247b0aaf1e425ac9ceefe9b756fc9a8f7121d2305ef87c90989bdfff7d7bb4b3d69d778e5629" },
                { "uz", "76ef943c26d37dd4ca055f4a44d90076b91dc5bff9ce251fd3fa62aebcdde2532d5b6ab3ad8225b098b635bce352a6ef99f3a5b411f1e01d49ba2d880ae74858" },
                { "vi", "6aeb837a736970767c6fa3fd6f77e089d53bcf437a12aca6a8602f6dd4c2ad68067b62c8175ec31284a5710212e1c6006430b49ee500d58c0e71b56d945b0c4c" },
                { "xh", "09687c92f6e5afc0f395c26b7fbf54019f426cfb2e314487b3d5f92a310ceb398bef24e7c48f36826a3828e7ff60f80a7f83b1fa6d292d58f51913e9ae2c5e4b" },
                { "zh-CN", "423044699169f2d9eb5d2617a97900795790e02991ecf6f61a493f0fd256f8c557c4333a082fe4184573cc235272a5e2379693f58e76791335988b582a259f68" },
                { "zh-TW", "ded5f3645f8b163d104557fa398fe94105de85626705ccd590a901db89b966d233362ae90be6e0e0f36b30e0b4d57f6c443ebef54c6eddec4cf050f707885ea4" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/91.0b8/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "baf4a70e46a5b3570f60b987cceb6a92a5d4f6a98d29a93a5a978f02810700a53f591faf37e7665dff6324779d09839df5b0ae4464eec60c6ef3604d522ba743" },
                { "af", "b9b505b85f17f63825f61a1f82ea61eea2499d20ce3a4273d593566a861880ea4b305146a652e86b28ff386bb0977999c735527802ae09122094dbb4778db2c2" },
                { "an", "49d76e74c8d887f69a53e253ca073aebb5568755c1d738c438d86eaa40a9b06b0d4cd2878113ef1f07365b9a425f70d93a280a1de963f2232dae56d02c8683a4" },
                { "ar", "f9f537802cba2ebd3c66245ca7234ff43fceaf0bfce963d39675f6362f2a85a6038881b52828817599c22079cd3526303556d70abeb21147f6f8677b2f5080cf" },
                { "ast", "340c5f6e65deb28df3d5a734b681902000545b297949d4a5b23ef6eba111a6cd790455b257695516cb7a7b0e020ffed462b195eeb61e50e3c6014fb77c96daec" },
                { "az", "2a5eeb653ae29daf0f769047e78bdefccc2594cfc38d83a4f24e7662fd34e9b77fad587c3454fe27ad00e48fec4bc3b1acf1d94921b805f5bfe0e1ce9a0c390a" },
                { "be", "e3e5fc211b19c932dec66871bffd4adb2685745effe2b85a9ff9f3978630fcf08bc1476fde95c2b7bce54b2cf81ed4ca6a45a4686c9300b67e8e9d0b491ab65e" },
                { "bg", "a22d75e02e4fb6e4312f8f23500e7fa50a79a66f084716170c9038fc98866522a1324b00a85cd2db9381ee0565827f7674cbad74cc00effa808cc1785025b1a5" },
                { "bn", "608b5c8529c3e4fbb53ddd3bb7bceda549ae105135789e84ecc84a9fa81d762601dbfba6e6439ff8495cdaa985e58c527a4efe6bba394cfb6f66ea59574cc4e7" },
                { "br", "dc532212f27616cfa1b7585032a15c7b40aa5d57144b76839758cf82a9bb2b2a31d706b212de01d6de7a62b9ee05dd5bdbc48d66e5a983f812a2901929ab0b07" },
                { "bs", "4320cd15b614bab921c20d30744226ba04507d904a98b8fbc384cf4c751f30d8d2f08f5fd7ed63fc3f6aef13c00bfed35f3da14034671379021e744099251880" },
                { "ca", "4a9b34fb83738343e76111fbe96b7adef7504d33b9f65cff9425a7b06432dc191aca5085e7a5e5d281b580e9ff91e82d84285b9295e33da1ad7ffb0e82c14a31" },
                { "cak", "bb35af0ac304e048fc15a9d34b7bfbb6cc67fa643172f32d2677e31217b73ef991791597e23b31ad501d8b12ef8555ef9d5517fe5ed59433b7540e75b3f4d8a8" },
                { "cs", "b93b89ca901bb26c9056891056f59eb3317e2f2aa6615309ffe722803b218b853cc82e36d9eb78a9f28f8cc54595bafd58b147e702b541ae4476fb646fe1152d" },
                { "cy", "a702a0b7e550c5fbd00aea38f814e418ac4116c1bf4858724a013736a900ab45e4295754462ce7b22525aea31144d61cd25118aa0a831f42c86681ad8756d186" },
                { "da", "7be140c0786a33604993541fdc1a0f6f045f27135c122bf0a14c03f58df08c9a5b7389d77016e60585147ff72579342a5369dd49ba2ff641d8109efbb05b16e2" },
                { "de", "dd1f8d05c6289a08a1118674a6427a277aadc5bf50a1ac95330140dfa21d0216c7542653c3515aef9d8a4bafefa8ea4105dd55ffdd7c617aed2a4c60b4fa2873" },
                { "dsb", "4c2bcc1bd8d581dced7144bef5ae77902fe1b739db1896369d4fd727ed9c203b04d7b756626c66ab4fc3a26547b0422dbb83d1c6122453abde936c0eefde3011" },
                { "el", "4154d95de3df116b852100f5172807b6a926bbb41fc7b88e162ce62665f519d6c6833c1149e7862486e3acc80877c9b52c2f0a624ef85523c17e99dfe4e76ab6" },
                { "en-CA", "ae24841de79133507209b93c2943b0556fef8ad08e654977ee8d6eeb9a376d231121d6feda787947982421f919b7afdf9267e65350f5f48138e3f7de12b23942" },
                { "en-GB", "b0148d6413d22e348976d4a7ba8d5de629507cd4e4af16115b4185353e9de06e0316068464a7ce59f62377cbd2c0b923078dd4f458581252afda27130d853baf" },
                { "en-US", "0e3d2efd3931db72754cfe27f3ecf86a07bead8d10b266d35f613bee2e104d9e35cb2d56713cbc8b65bd95b74db009f8c8e4a8a43387cf3d7c7220465d268c78" },
                { "eo", "1f94d25005eb23f997808c605fc3778d1b0c7271e1235957fc99728dcb1bb4c7e37454607c97a9323976e0ef5ba9ceb0f56d625b04ac102642a41c5105c6d72e" },
                { "es-AR", "2e477f8abfc6e9554baf22a953c80186badc9e1719879590a32fb889dd15dae46dbed5ab3caa5b4fa8c8c58c65420a2fc12a7b827ec911e0c20ad1d78d3f75d7" },
                { "es-CL", "8851ead342a909fe6d3474027cde593b642f718dc44df12a11c5d5905b84bda4b45d272f14a932ef70d29d53695ffffa54871c420ba13cbbe0da0e4225178125" },
                { "es-ES", "d0dc5806fed17801466f16a01ee96780dc74a69d4e384a2cda9ff0484329e82c9ce37213ce94dcd87a5f39d192f0f159d194240939506945b2b749155a1e1e11" },
                { "es-MX", "85bbd44b8bcbd1de569c229cda98631aaacbeb5b9d2c215a592a4c52d60b25ac4c2aed0392c3de452ac18775f1dfec7b4a5b9e97d29b9a94a32102a68977eeb0" },
                { "et", "139958c04104584b2fface9fb9281c6acbe669862831af884e466ada74e9904e56ea83bbd56218354eddc571eafb9d386ff1257cb4022cafea75588d64e7bbb5" },
                { "eu", "7cc94fd680ef19629807b2ae234b037a01728ad80adcd70f4748965deab2eed43d02fbdec5cb233d65c235ffe8546f12b535c0581449e741b6d7f7d0dd2c7796" },
                { "fa", "a8396414d2c4bc7a6b13aca2bfa4065527b694f5d69c0ae0010bf70b895994494de26c13c57d6f479bee5be8035ab0b316a58dd33992b854092dbe3ce04d7ea3" },
                { "ff", "298c5f31857a7786b584d548587de1edf35a86bbebb9ba85c361e151186b59d673a0052b7018428aa91006d519ae4818ba5811a94f87507f8df4713b35bf83d6" },
                { "fi", "2954e614b4cabc1dcaa1a1d7af212e4b9ed35d9416a8cfbde77be3a153b27f6ea32d04aee1430e40bdf4b2d75a6b2b4cfd179b813479117c90f147d1a551c89c" },
                { "fr", "b37f5624cabb1639ad6eb4f68b3fe30572e0540b05cdc2d84bf99ab3c4f69dee39ec11e0b09729de07f9a114c6a2b36cf6a8a86cfef462746682a9736c0625cf" },
                { "fy-NL", "d11ad9cb9ec29487e808e19ba8897d5c4117bed901b10b885d6849e509a9a2734b1a071f4c75edb9f8432726526044dee351cb7cff8393b9bfd08120dea3f872" },
                { "ga-IE", "78de5c090e10b852275b26755ba889dc1b24cd90b2e75bb4305efff9b56bdd17bb9c17791e6b4e95879a51069284f0bd3443855d10be0456f56b43ca2a1c0c36" },
                { "gd", "6069051c1c11f6d43eef11146cb94ae97bedb637b684c3d7aa71247d50260ed19e7e9cab7730d04c6eb9cb7d665f452e09e295ef26b3c202ce8bf2426b219d93" },
                { "gl", "1b6abbc1e629ace8b44e79f19d786b95c416b1f74436f7c548994d96a31f1e0ce88c12665b37d298b2ceb3a496d3dfbeda08b477e01d83df1e1683b6423a5b57" },
                { "gn", "d075e412a4d8937736952574163fb5218acaa5c1c6134992d42dbdac2594d87242966062c00c7a132c9db3d52470922b1a5e76aa828b1675c0364043aa0ac89b" },
                { "gu-IN", "9e96b8965bb578867cfc396c3e0110a8b7cdcad58d9e8512c651763c36aab31db1f501c5960162c7e9dde9f2d94db090c1a4891c55d0b14dfa410d9b67cda96f" },
                { "he", "3604806830542d1ca703d02f924797dfdbf07e502c80d09b77fd0f596933a66065aa3004f09237ef49dbf26cd19379a291273304e94307e5be12197671aa3e1e" },
                { "hi-IN", "d6a45b1d649cf63a8c84d2c56b8da463ad2805e9e68605651f6ff681d809f67d379ff60f7c688f617e8940dc570dc6a85cd6f38268db8fd2e6bc250a2a764425" },
                { "hr", "40e44b2f807967f18d8308cadb1e12f1158b87f5a6d1f2d4b5d2e008de3a6069bed53dff8279906b7d60f47e24529e9078fe16ab4c871862b9cec0bb96ffdc8e" },
                { "hsb", "f2ebc045c6920dde44ad4bf2e531588145a5348184f32a97bea3734da2607421ebda440da1024f4a385c89689cb36fef6f8a75f5439141f0a9c069014e0772a8" },
                { "hu", "ffed92998cf89f40d67c618b84b3d8f5569c0f3dc576042ea4e273d9450d7e8b953d661a0a2bbcaf1b41bb32ceb0dd0a537dd477b013665027d5d9e27884d4e6" },
                { "hy-AM", "33c913d74a929175a5ef94e1d5a459ff5a06b0851d690c76da235a16c38b0dc9a87da3faea0321eb76eeff3e608fd7ea5f74d0dbe485a17a0b23c117f5e6c933" },
                { "ia", "2860a97c85721134db2f315b7f3fed17334f5dfa33856037a5a67d6a1a24d5e20158507ee77d76c48e1734c676c53d185710e3788052bc6c752fbcd279665128" },
                { "id", "cde6f118bdb61cb374fa3d0bda1d86edd4fd9762d90d6466e60e449d293157a1e2643055b6313983423f23a2cd59cc34474d871523e3969c9a849114f6c9bdb0" },
                { "is", "b73ff543a57b79de0790a4212b69a407f885c01b2683ac681177fd44ea4f523afd6c3b28312373a32c458f426387efc89a7d8f48a001d7ac5601cc80b95e848d" },
                { "it", "58bbd7636090c71a5ff7acb5f875395e561ff18f4ca42eaa7f32454fb6536e477f53ffb62dd15cba564abc92326ea1b529e05c1f14cc39aa1030522baa121cd5" },
                { "ja", "f34bb2b5591cd9ae42539987186b46191a35b9e3b5d693ab88d29d32c34b6caeb4ec5001831d321b6dd38241dc1a962b6a2597374c69aa0e267e97f21aea2f10" },
                { "ka", "4b27ca5d60e0a3a21c787c0b615558368fdc135a92381cbb2115375c291278f340c063daea167918aa5b760b968cb151038bb8a9a833efc854fecf22335f5d52" },
                { "kab", "2a5fa6b1e2ed6c268977d42ad79a4efe8b39345dca684b95016cf3bb8c695621ab1f496191080cdcef5983307107f998c831841c84e2e086c727b8c799f53362" },
                { "kk", "151b6c118221c6b54460f1e33d25adfd75c8b760b682abadc2375be370c1cbe1a296135313bddf77e583052ddc2d8376097c6553363ae80e12fccca531991637" },
                { "km", "78e1fab6610b99c05aa2c4dac9eb39fbd5b9626720c142f0bcb7845e4e4106fd966fb2928ba4e2c2cdde985e235f2ed348e2cea73e36e4a45132b1ccb5f09397" },
                { "kn", "aee3304c3a032c12fb3601219bb245b87f4e32ee1b3daee8d6e06b42e8b399d5c4bdda475861e30230d28a70c11e172560e7705ecf6e4cfda878583f45fb4192" },
                { "ko", "8d5fe8f0439c05384266b95e3f84783b6bdfb2d2040d365d4ea57cbc9c81672cbb7dea0bef32e8a5aac16b07652e3c635515d0f05258076358c75c6f5a5a1874" },
                { "lij", "9795244350f6c671df891f3f93fbb4e6a6aa47116b1316f8db49661ecd5e07eab457f4375ae744522518944b177bb258c627c067834161f000b6b0c1864af136" },
                { "lt", "86d60564e769696d802448a3571beb4be32ff35436e8ca2fdbdaeb392576b2049f5b883882071630873bfb7ccb802c603d3ca3bf9a221bcaca12e011f7b3a6a7" },
                { "lv", "b81d66b436d3927a8efd8d9ef99a3134db00c63fb97be99a79820844f990062d3085481cdb88703985f6d169423bbf8c9fe55ae49091d9854e9f7c702739b838" },
                { "mk", "ae0de767198d80f81079a05db9b857f87d65382f2a66fee89d58a64b0b30be3b3a22c5be957d85f4d146cd2aa53e73df5fc9bf67d4baa568e48de8ec6b5dd884" },
                { "mr", "117dcfe3c97f717610d838688aa753aeded7b1d097dfc6aea04f7a97e4b9190c33d4cbda206d895863d89622c51981fa0e5d758824906392abccbfa7e04bb045" },
                { "ms", "a51db9b1c2d68aad983788cc38d41970d3f7d709fb2f4303a77610dc5a755267a6181f1152afd58b77ab179e54f236dd5fb8eb7a98961e68a415964eeb180e57" },
                { "my", "5b4c6f04ccc5d7a3933cfb8eb825a401a2b5e2db3bb6b730d36686620b79bbba557c24633e513a6eb55892886e45d28b32cd832cb317c2ab3803e4467f85fe42" },
                { "nb-NO", "fd59f2f43495c5aa84b9835b9debb929ac96aa9f619077fa4026423d4ee4591398beb508983284f96fa220cb41cda67555a668011eea5874f0ccd6acc7780d88" },
                { "ne-NP", "48556d43772c73775fbd425937ac7c3d03a8229bed248d824a0a39b336282a1e0e7a89ce6d292fae6288e0620240609d5f8d4672060f7b7ae494bd6e6302933a" },
                { "nl", "40861f4618cbbf0606b335a4e0a3fc19d27bc203bec5c79f002a5a17a26e51e52a307c1226770a064e13b0274d2b161a2dd5882ce5e808a9b856523685fde0be" },
                { "nn-NO", "7aacbaab58f5b8f723f24696ddef66f807aecb9f02e776f8b99c76338a3d82da87c0ce0a48a1664f1d1c177c3e1894b6667b552ed1dff25ea0635a1ef738783e" },
                { "oc", "5273b49ef33af885f800207af149bb82f2fb352b945f88acc65b55f73d3bfc93a01d954085a0abe846f2d491be53014d82ff139908a1583ad4d485606ce96e60" },
                { "pa-IN", "6de0de7e24053d6a7afa0e71f2b6a2332d428209a3a993809141e750d7d1c75a6462c562578006adbe583b4bf74d6cafd888d8d07465700b1d893e6bbcf7edc4" },
                { "pl", "fb0fbabedd0ca6068607ac68462f0235a83135538e3c7030f552d4cf90d956e272e39ec639ffe6b3761eaf10874713d5f89ded9621b56daf1d43305581126652" },
                { "pt-BR", "c32e8eb48189d2acb1a3e77eb40acf61f2560952e638abe28ac4ab595f59e0fcde9014a011bf79b49faa10ba5e719b6beae01986e8664935bedf87675f88f890" },
                { "pt-PT", "06c01845ed903b77f4089f3db6b28cac7ef24c46104062c5b9bf29eaece5434237c44887044f7ad5cf544e0068e655d3ba716f7cafdf61541fd6700f1f3deb7d" },
                { "rm", "879942564bfeff6b9a4f8bb3c2bd3f858701f01beba5d3f13a4c8216fda1b18c5cd40910cbabeab131b3fda5969dea7c321d8b63c30401086f7802a09033764c" },
                { "ro", "d32fda54922ae5843e1eb0d0613fffed6f1cdb85031214f976754ada75840dac860c37e57ce06528747cf108f163ab88bbf45196fbd4bea7057f7bbc18a7bb87" },
                { "ru", "329d93cbac7fcfc1e42f6feb5ef09b155aef350a62b20d8580c2b132a3677c6d6bafc2991be9c0d227262983c5682391988c50d01d4e23f6f2fe7d3188f97cdc" },
                { "sco", "d36d8362dd4c119dfacab5a97b11f3eb10d1d61ae4a051c394164a5ac183f4c94f5e48fbaec346d77098ac4ec53cb1fdf909fe7912c4189045ecf50864b0fe51" },
                { "si", "9e2ed75f5bd06282478139ca015416341cad32c524662e4a73c8d611685e11f1dc7f28cc6f817eeb5f748a8f46a8227c9f572a02a76e6d4d049272544df09371" },
                { "sk", "73cc4ad8cb7a2270e4796aec2289d950532deedd0f2ece063288794e22b5c1c3cb6a1524b8639a2c7d6158a2d144ac723796c4a0ed25933082a970e13933355d" },
                { "sl", "fb4701144310f6b5de8d0b35620fab7d47e610c33527932dfcd65103356880161acd14f686d8e84d07c7a8f21b57851c09f287d78a2466e96200893e12940291" },
                { "son", "4ae2db5a56907385e8d09026e2a003f37c1b47076975ba4693f9e629678b59f51423d640686ce8e9cd0c2edb835f06686928eaa61a02d67f18a0d09eb47c5a15" },
                { "sq", "5a2cddf890fef274bac73a903bcae63286f7764a4433f4a52b90e125f9cbb5440dbbb29af9d9e488a8f72d1bdb5abdaf95fb585d957909dd9022c7ade2e02e80" },
                { "sr", "024a7568d3075f5bf3b280085c6caf06d487229c8197aa52c3d023e0d15ae47c0f1e9c757198f3f3f3c8306ab2cee54d56f40bc901dae6ca36ec916ef6c93619" },
                { "sv-SE", "a5339d312411e05fdbde05ff1b43db6ac8aef558d96e58370a18a189f9f3d7cc7a9fed9a4001e2741941a0b84e5286096245d6f4ffe658c2200e7cbf2810424f" },
                { "szl", "00dfe106a9a50a25c5414f8cb20417ac0f3499b75e3687f9f3a97d44ccdb246b69bb5b6ca22d38c3ad565c361a228ebd8c8026efee482f951c16ad20f17095a1" },
                { "ta", "20fccee8a03948213fd8baf88f1280f69af1dc42c9bc3a0bb07d29d38bc5af958b783a5f1c6e1ab3b6282159cf454546f095c5714f6a835e47a3e0cec382c8d8" },
                { "te", "f575a477bf63ffc0af975fc26dddb4d9a0ab6b2be8d3b7960d77ab55db7881d28a5470a1436db46d34ce28f295b06e841bed7635dc20e1470f8b4c6468dccbcc" },
                { "th", "f2032f5e122460e95a8d9a3adea718bdaf563fb2eff341ae466ae81d44211c797f90ab33bf002cd494e385160fa6492833a8eadd8d54b600f2acc30ed39ea74d" },
                { "tl", "9a914897bd4a8896dc996ed396b66a66871bb54a4a0aa6e8a5907fca47979291b08f1c28bc1334fbc92a05ec2dc7ec22fb632da463759d1c65d5e26d617374f3" },
                { "tr", "3eec0c76896051a51bc2020b9c57da9f4eaab8e0ef7dc3e819cf01cff15f8dd398f2ece408f9eeeb08141eb911116880d13cf8f0a53240b108644c4a256440de" },
                { "trs", "68bf043c87235ae1ac6f7d706c06d57b71392d9cd6ec38b7390fc12858b0b16a0c9de086d4c40154f75ba31705f1b63b040fd13304ee02d89fcab981a1c8010b" },
                { "uk", "24184a86e3e23a4924e88e116e7423139f4ca28713b97291285197206f352b5a786c5e37a4dd3d027eed8c900f12f5d214e5f84e39b740f31f5c2ea226d04865" },
                { "ur", "9e360ba86b674d208f956afc6cf979943bdabc7bd8152c4bdcda6cca2d387978cbcca3d9bb8f6478b120dd640bbb5f456dfd46349f7583e3b8ce2c16dcb5c30f" },
                { "uz", "7c6a609d27096352a76037661527bcb75032b3775fe4860b23f50b8bbff3c455f214053b47f5564ea64b9905ebe8417abf7d7c3438d31f2d9d030a4495f671cc" },
                { "vi", "371c8a429ce054c7d950b7148d0993fed9acf9e4936d7781cdb4577386df7a3eeff837da3b7e3deb6a3b43ed943eaae8739e4074b9976ddeb6008153d3937f0c" },
                { "xh", "86302fb2db4a2c5f17f17d021492d116cb5efcdfa022208ac4909d4d615188bc4197239ae68c7213a54c1c1771888d5c7a2064ac8073d6dd7831f3e63e9edd82" },
                { "zh-CN", "add8b5d2d584a230b3a8af2f2e236ba2650216e3610622d9b9a32940b41be1def46af693f0c0f1a3a0e95214f6d09219aa2625c560ce5c6991b23ee8eb131540" },
                { "zh-TW", "dc3b3597d2730f496e23307dbf92360ac75d4858a76739664bcfaac7572e3fe0b2878643afed78016af3ae2e85f67c56ead7b2a160203cb780bab1708dd5a740" }
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
