﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020  Dirk Stolle

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
        private static NLog.Logger logger = NLog.LogManager.GetLogger(typeof(FirefoxAurora).FullName);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "73.0b2";

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
            //Do not set checksum explicitly, because aurora releases change too often.
            // Instead we try to get them on demand, when needed.
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
            // https://ftp.mozilla.org/pub/devedition/releases/73.0b2/SHA512SUMS
            var result = new Dictionary<string, string>();

            result.Add("ach", "7932e93ce6878ae482d8e560a308353911cc2cdd1ba442ffefc34feb4b085615ddaab091491bdfb5b214fd3149e02ce8aa40b46cdd2afb76689c336dbf5655ae");
            result.Add("af", "547ac5cbfdbc18ab037efa4e52b68a15c1dffa8cdb887a3e499b120815f257a2fe4310a08da346bb45662c1271e6712d27d684a041c6b56204c77d3610006b34");
            result.Add("an", "d8e754d6b3da23aeeecf7f7ba91f2a60edc0bde1473b0e74194c0da4123130e929d4f39509adab31b0faf084b7cc95a7ea968789b26079dc59664817e302add2");
            result.Add("ar", "4a669059d743933cd0126af5180c237ae16f08343a151c139e113ba886b5a8f7ec0a1759e5c890b5752a4b26ec8f60f40f07dbcbb1f817dea49ed7e5e5448603");
            result.Add("ast", "940b3e992330792b3d5f427a655e1023f7faa06cd740e3b56037d16743031fb20eda0465fff524d62e6ae7b8ee6152f31111385d050d5f963cabdfedcdb9cb2e");
            result.Add("az", "b07ffbd771545f886b4ceaa1c5f405853ec041c3c7a3455900e6fcd4e3ed2a08f036a081e0809484255e12e7802c9773a64110599430e4cab6d7cd2145ad36b5");
            result.Add("be", "6ea77c3aebe188ee32d6b7d80551d99003a4e09dca92a0bb1e877f1ca824e19e37f21a2a79b4b43041f41a570860087b9b0b57f989c69e839feee472d8c3268c");
            result.Add("bg", "0c7945aac79fae835e7e36bca5bfdaf3129770fb096581ef1233225ad7a36feb7a1dfbf91b17c073821d326e422ac453db3bb1560e2dc262b73dff7a5b3092d6");
            result.Add("bn", "aaae65009167555be779a1a148dec330dccb5b75207a6f96713414b6b7c9ef650d5d88bfdf263a613584c446f2342099a9ff4460cc8eb87abb1aa12e4f09c383");
            result.Add("br", "c3022311c6c2a2e3eea47475f7b1e6f59bfd402c8506550a95f881f3ba1d43e185a93303abc8fe34031d98c16ed841825b5e333c7e2f2cfaf6383676bb9cef45");
            result.Add("bs", "f0de95e03a150b7be36adc094f75e77c2a321646a9d310d999a5754c90cc901d36e7823a6eb7d1aa0c95b9a8efc13980fe8bde90b1315f709fdfabc0dfd2f61f");
            result.Add("ca", "11298521617a587b347da75d01628e6dc2d08d4020048b66596921f0a4a8af6f939fdddf6a30c736faceef45238178c1d822d6bcb3f1ec1803115783370f397d");
            result.Add("cak", "0dbdf865de9be88fc2527df0eedb0d3a9ef007204ce393ecceab6c3fc3bc0f28cd277ede0ecd9c95b78b49227f5edfa54a1dadfe67e24ab2d1e1f7f0575f1410");
            result.Add("cs", "b5a1702ec65c4ab2782033b846f7f440980618b23f7ec36444d9fdf800ea4323405f210c8191b31c2f4b78e9fdf6ba0e91a159d951ddeca982631f8147d88f22");
            result.Add("cy", "bc3031338c315780bcf574ae94bf02e60fa94fbdf9ca0c14030325cb1815e355e40395b494f6d893b8ff8ef0bb21ac4b6d6f69bf2db2dd3a053c38129b527d2a");
            result.Add("da", "44f4b13b85d05b0162e7075a30ed7d08e3d9f01755ab6a52311e09945acea69c5747891d82ed7c2048f376f502483c3fe929a0617394d7e38541fa861b8616d6");
            result.Add("de", "7c8b404d23bd0a9ef63f95d942ca2a187e5a4fb67a831f4fd1f4d9e55a8dbabd56b7181befcc5b1bf0cf507fb275e22646dea4ad60530959fd62aa24de76befe");
            result.Add("dsb", "2d4b4e18a5614a50313c822016af9b1d6ce9e785ea4db5bd6c972e109393ec916fb893836444bbc7a71be585fd6c3170d53ae9c0db245ee5e4359766e93069c4");
            result.Add("el", "3530c8ddbe72e8a568e37f02ed3f6d829245e1981b5da69a0dede2dd17749a6825453d1fbc2d54bbaaf41c04b43c3d444f214ca10b20a2d4a127f6aa658c8256");
            result.Add("en-CA", "54798c2395b447ea4883926b331be7d658c5aedb501971491386814c4e58be7915c14554fbd67fd531420ae16283dab6e5595d9b9276073cccd42f75cea6e3b6");
            result.Add("en-GB", "a7d9a89b44b4a22c8472e86957c8234b3e2f074ee5daa4ef5407ea5fa4988f4d5c953629090bec255e65df07ec994445ab80f6dd8eec6a86cfb8effa57dbc499");
            result.Add("en-US", "3124fbbfcda400fe6683e907d534bd1d029a92daf0b8885c64138cfde389a8f07d406675fe40ceebb13ee37180ebbbd1ed1225ce908ea58b756ce8931738bc10");
            result.Add("eo", "8dfe903b6943f4ba5cf830f1e8cc2fd02962ff7382c58800b5f3196730e10aa9d381df2c8219f800011cb82faad94b56f1953286f6c3234c2a49dd2f5f89b829");
            result.Add("es-AR", "70699314d1f631d5bae9afc334a321a41f8331b4cafee65895128a12442efa21ddaf359e2d66955c5387ea935ae586f6f22b28ba358603138e5edee6494de8c4");
            result.Add("es-CL", "0cfe956e1c2777b4c2417539e92a6116e453fb98c3b1df2e9b26ed04061d217be5d22d672d093a220c10d0ca68534f47ac3e027adf40a7123a2bb080ed4e3f01");
            result.Add("es-ES", "b0bb51da11a04fc7c1390c1f99f89a06ddb1abe5e36c3b80df523b2246d42824fc02458c9c5f9006bf9416962725c0ee74a0b56507a0799e674eff18681ad92f");
            result.Add("es-MX", "646b61cfe2a3113b3b1ed78a0cd31ac0ba2c3e99ff5c4241a60baddcb000cd95418c682ca7b8be5eb55f73145d0e9bb4e3b7af8264ea528a3565639a8804b031");
            result.Add("et", "33de3d59add10479556c39f24019b3497c17c485e708b6b8a234d7c5e1a2d94d7d59586e0adb9409c80f117f782bd7ab3f81e85ba72ee8d16d136388f80a6f11");
            result.Add("eu", "9f86020f0306c390b731462333dbcddd44852bfb9f8d87060141a766cb90d170912cbf4e8cc904495e8edf50fda4e61187d97917b58143e593551a67565ba8aa");
            result.Add("fa", "f13a3a2164eacd9d0d1b4d0bedb00813a78d31f0d28d755ac03c251576120d5116e5f87d9b88b22bb2a63064b33a7cce6154659c6c4cffd36bd668592d954ab8");
            result.Add("ff", "9e27021d84bfe981f611dc1ba8a131a7521b1eb4deaa6f628f2cfb7d04f05cebd8982d68023aa3a172638bbf41aa7cf3a22605a09bdc230f781f8e160fca44a4");
            result.Add("fi", "553b21851ef43717d182279ec5afb2fbaef4b787e0086b28e4977c9176d39adeb34d31c5c8c781ac90fc083f8713b09db719cc31cf2907bf5605fb44bef2d9b8");
            result.Add("fr", "2a31ce2afe5ea028c170622dee3b27e246d09f9432941295ac04e7c1ba92a2ae43829bc92a4ce3cbbef018dee9d98991e47a3afc979a97d71db988337021d52e");
            result.Add("fy-NL", "7177884b153099c8161708333a36aca37a67cd04fc0a83b91afa30a5485b624aaae0b2554781261964881502e994070cd2acdebfc2b2402c56d5326486436410");
            result.Add("ga-IE", "a6566cbbfd4e153de8dec230bcf80cedd57fe0e8519601eb9e1464da393cf925eb9e991b4c24852b07ee160ded8d516ff89b40fd78687987af7c92f80079dd45");
            result.Add("gd", "6e2c1d0394c469ca3acddfa06a7e260b06deb131debee7938b68c093be33d9f701214478c4f8c960cce796cf73e7d78835cefef5a291133b9c85f2139e30c1bd");
            result.Add("gl", "00d6d8bc019872667b0c0ebf2e177657b8201852bc5b9778a6463776203a286a5e29438b64c17aee148288bb777f4be4ad411b73c1319f525f086d692454fb98");
            result.Add("gn", "29181c85ba24b239e391fab37354f37ccb2e6cd52ae9179748d66d2185872fc74ac464f16feabde80793b0d4f6a507ad2bcf584fcdfbda0857f9a5dbb78bc805");
            result.Add("gu-IN", "79709ecdbe5bc4f3476642d76a14148a0b49fad16f8c65aa71cc27777808f14f273ce1d8801c7abe8a9c9a9bdadbcd812183c2b1d90f8b526c6c44c29991ad9f");
            result.Add("he", "4037b26e02d6a3d7941030cad6d86b74164d264389d58e8ffe86ad382a344c2faa167765fc5a10d0864864524f1886d75ff489f68786462bbc73f799315b5013");
            result.Add("hi-IN", "f221aada59cf1eada6d47aa6cb70e5f795a4e86a0da024fe060b54969620572d07e35f5c8e46f59dcd2e14c979f273dc98acf7f19f9b783d16226568e59faa7a");
            result.Add("hr", "291dfe34981281fe207442dfc2a391fb6d1d3f214adec5ff6f89beaf285c23679daec1a90e585b33c554052f8f80748af8be811eb3b8b1a3fce9dd6073826ff9");
            result.Add("hsb", "85e21ab378f73baa4b499c94806b4fceae07e4096d2b2c093e14ac12b9e6d7b580e6af95570fab5e5621a08f3b8f9c5660dd3bceab90de42477ea69009c76908");
            result.Add("hu", "86bfeb287e3004830345b71ddc3305455a93b185ee47e4faf3134984ddb28c25ee57aac1ebc32fc70c642268d1a64a4e2171f645763528569593d651171835c2");
            result.Add("hy-AM", "897b357041bd4939ead71d1d55789d09a7eca69c3b86731b62b90b548d91d36a336b493de3f52637f7f4ecd30b0f429a743bebd08f0f8fa85602456fb36f8c30");
            result.Add("ia", "2cdd143a73adec7d9649e9077b42208f4ac2a7940419bd79fede2fd1670181ba73e44fd9528d0b051d966aa212a63cabff3b032ec2db7d3b7720fbbe68912d57");
            result.Add("id", "0771b19cedbda18b95f5de6301ddb9452bebec09f1b513a2f1f2357aff30c09fd3c241cd5e8a390dc3c68417416752f563e05f1d8fdf5f98ab4fb850efd3a4c3");
            result.Add("is", "96513ccea7535c7b5bfc7dedc8ab20d8856c07008a718af29f534389a0a0dded8338d37f7dc8b1b982b172462dcbbd8ffdfded3f02acbc0e45361527fab3fc49");
            result.Add("it", "6046b4ba659423e6acfc308657dcb86b5422bd11c7dc738414dfcdd65d23b5b2dec554df91ff6a61cca4db2a2ef5d01c35f5c64ca8a8ce9296f963e578a28bf3");
            result.Add("ja", "a25d084919091634c401aa20cc44da08927d5d369bec4ef63850599213e1907afc192c44e04cb6576ecc9375b12fba3f2671617b61bfa3790804a396b4ec5067");
            result.Add("ka", "8e6b4880ddc13e20bee45a96e8736e0a10575eda81da746ad6f31d0555077d8221261fd55950dd513e45b5bdfec1684249c02483769f0547792efa5295a931a4");
            result.Add("kab", "db75d219f819e9e9e6853934febfe98459d36bb4bda1a8c6de2101b48bd48e7e82cee97a959bc5c0facf41a1925d2c0359f46acfdf8d3174ec68cff21cc063db");
            result.Add("kk", "cb475f61969e8bf899c80eb432baeaadfe88935b9bd6566acb3876492211f1e324d44c6f440f6406e28e9d907376407f7f66e8a57d0f7dad23b7b37ee0c0c083");
            result.Add("km", "ab1efbf5156aa2aefbac1b9665e5841f2b784c19a8e482465679179c85bbae518350255ddc681d91c8e3606ff54c413d2a19262f9b655e5a3eec5d6cb687483c");
            result.Add("kn", "266feb1ba7673054edf6880183cfa9bd018c8fe5ab0304b7739bc0a1afa2adf39d30097608555d8db9345404830ef09c377a0d670d6d3b5b8aa0a6254164723e");
            result.Add("ko", "5c8c28b05052f279ced84b375db99740d2ea827483af83186116c44969b61dc9891f9eab06780b78775f5008392aaea3f279cfcfb27fae38ba26b9b8aa4ac359");
            result.Add("lij", "c90f033d4a343e9713e9785067c5bb1ff16ffcf3d24ead99c50f944ee37b64ea083e5ba99651646d6fe128c66fc9982446a25175528473339f38d56d86675017");
            result.Add("lt", "fe1bf0f39e3b93976b667e98d897a147852d8bf8350b84fe38e58be11b24eb1f02657434d1a893c0b1f52074fdade6dcb592b54cf68f2a814d7d9dc2012cc540");
            result.Add("lv", "e3a3b6662373d9f27dda51bcdfe5a184f5b697e9c6f4051bcc0723f56201dd86dac0e4e39ee1780699abd6d537a8804bf2411b99df6d220d948820f8ee993b5f");
            result.Add("mk", "4f22d042cb59abf5d08c4a0dd022c97b77963242edda6fceff783eb1423f88960c586faa9931f52f3859b8e876bfa79e0cfa08e07f94bf483b0f934ae8b9435d");
            result.Add("mr", "e567bba5495d1e43f8ff0dec60ec7bfb60db6b62cc613efe68c305497ce2565678b5ffb473dd583979decc3c2a77e52367966544a19e4a6ddb792172eea55de6");
            result.Add("ms", "6f933fb1084fa95d44a688a4d5869a0ac460c514f468b4724a3d68ca4c5404e61071118161eebf0abb442b5e56050dabd4c8fb63f2f5176f96de1880ff7c5c98");
            result.Add("my", "a01b82e868702f630d1b7ad15b319180be79cca48a07db8eae50c0bcbff86bd476e973375efabc5002ab7ad0b92325c13f4b2dbbf01d966b42a49a62ab344c24");
            result.Add("nb-NO", "70437ebf87916643cc207c5f09e511ce8e5a699e26e53268ea870c3ea605bfc8f8551e699632c4f4090961ffb6a9c12223f65a61fdabf0aee5887ea9af72c140");
            result.Add("ne-NP", "eb5c184d4a8c9940e5ad7f318a151f853b39276c6af5b492a0d9a771106950646c44e3a881de99cc46c7d2955521c9e6cfb2940776eb312bc5b31713adf21470");
            result.Add("nl", "31a45ba39e9723e07f96537d96c9fee4baa27124a78abd9d5097142aa5dc85212a9a5a7ea126168b620a9fafc26d5ef6b543c7359994c33b449f9f2240c008c1");
            result.Add("nn-NO", "049a750380f83ab8b917d0a3b5ccdd7b8d2be408dec1cba043e85a98176814eecf1c7b2b82bd69e3a0f82f78cbe89b4ab6bc95f25c5a638edda68a42a96149ca");
            result.Add("oc", "f738f4a0bb4e28f66acdd1486bc44c01bf9093ed61e36bab1f66744517d77d3a7d77a52cd9c1cb41065fc60fe56fffb742e255c152a56febaf851b8da1fc7fb7");
            result.Add("pa-IN", "9f209aca823a0e93bbe6c10ce2eb39008af67bb808146f03fe5138159dd8d32662f18eeb7e13a027a5f0bc820d4802c6f93dd0720ecb72b6c6fc757072adce1d");
            result.Add("pl", "ff0d65e8ebc5bc221c3091ecba0570f6f944eb8943214eea75fd58e1abdabbb783c6f03978ece99b6b023da051baed8e6adb5951af4824a994292974771fbf13");
            result.Add("pt-BR", "d6e28b33006bd170587e1a5b728e7c10ddd10dbdba15666b334746d86ffd324608230d6b4aa9e939ce3f40b8f2b4cb46976d8d29da47a22c69ed65b40f41aec7");
            result.Add("pt-PT", "31cfe341b79266aa9d6e516f720ee3b45824f26e5b516280927b45abb1ddad7d1ac8c3ec4c9dcad213e3593b9810f5b510799ae32bce90bca34112923603f465");
            result.Add("rm", "df954788baeff7ac31101232543cc881bf2382a8f10f34f387d6242bdde0f768ac328e26d1c8e1c0f41a2f30f9882776e490a0aa1f0e3aafff7d088dbd937c6f");
            result.Add("ro", "536d42d7ae7ca3d1f377d231e402cee38c0442447f5d5ffc6d70f3862c15066cb972774b5d60078369dc08e268cbdd80a75e6e2cc1a8680dc438103a88ccc56a");
            result.Add("ru", "a38466ff034fc94092f9f7979b4bc915f814e45ee0427ac2f5aeb134d7b9fe70bede8be371413f6e0e108012c2858efbd6f2d0f7fce94c2c62031fe08efc8540");
            result.Add("si", "686883cbe32a50b4aca7029c7dc8a262a37afe7f9880482b5e946afa6733404af160e3a256662400161dd37f0e1a268491305acf733f8386b625adfe5544d821");
            result.Add("sk", "1e803f63d75e8b319a81b9911761ee922771db6c9b8eef310ddbff8d12785738208c5e166c6c4b594f6496c43a0b410a8a7eae735755fea5547e72fe72d1734b");
            result.Add("sl", "439ce0bf9b62f5128045e350dbf2bc62b5db07dc8e8c99fe36e0fc3dd065533953e79b9149a446bc86351078cadb36d94b741d4f51870be9271c4a8469a1f0af");
            result.Add("son", "0b4b6f750180fa577b16adc7a7142d5b4e222459ea26138b840dd7b3610c3d46cb6b8fc9571fc51336416da532c957b1f2e7bfb188149d0e5975d59df993ed48");
            result.Add("sq", "89a72dbf74720f7e77a09b302519dd090fdef6db9abc2091bcda41fa0c4c8b3804de9868b91e8819a8e7207e20445cb72bd76b50a476579d6b69a3ad3317960d");
            result.Add("sr", "0ae95a3aada7944f6e3d4cc969495390682a84cbf40405d9f5bf2ed9a2adf61e5c1b3de8debeff4c8f39af09a5ac21bd23a3223b76ce8c9cb7ab1f566794c859");
            result.Add("sv-SE", "4e29c498df2798b5734c23aa78489345f16f50ed4572df89cf4f0150e7e9ccc938a2be24ffa36d283cb87a87ace537da271940a11cc5c5b45daffcee8e427c08");
            result.Add("ta", "73f787b0c8df47a210b9909ab6bc235e60fa357c8ae09492934cb13970969e4df52ec1fd272082f60dfcb0ff7f2de724ad6fbf401b9ff1daba5ee7938d63775a");
            result.Add("te", "ae2be85460f56a288db1df5f3b6b5345498f9417906ae03a5dbbf01f328a44fa3bde17c5d750d6e2ce3a40c9452c9173359bdabd372c9bcc26cdf285464cf593");
            result.Add("th", "6c162ed3078fbfdd43f996d1b2c9e5b74320d3c41cfa2d342df6ffb99b346cb6cb6ed138c651c02a985bbc4333f8537e83a23ee3ccc404e538375fff708bde09");
            result.Add("tl", "de0ca80024483a63f4f8a767788967c50a0aef27879603c3b6424e973618e29731eb5605ffd4b4f5f23f00d62fc96b17c1de65f4c0eb8677e508d4d156790b05");
            result.Add("tr", "6f8086c83070c58e627c79b9e08f5c62b34a250e55c8d38e8abe3604eb7f5d1d0c7d992871f83999842edcff576ba69ad26b26a7a70a4507297d1c9e1c086674");
            result.Add("trs", "f33da3bd371f31885fc7e9bc50039891534605e4a58dfb1bba0288852380cc3ded57576da45bdb825903f8e8c48bdccd81bb4c02ca2d2ad7e679a1f4edd81186");
            result.Add("uk", "53eb6051c84c6f152b3ac8a4a96522fb3486013641a379077aecc1e16a0f2d7837f8193f57445d842dc005909d3dbafbf0b13090c2e81135cf746ea768425c63");
            result.Add("ur", "211af05f44236184d412ae3e1f13376a189b2d990e785d8d7ade3932d438308a21838c142da7c3792fa1ab51d13f9bd7a077a4c8995e7b17ac25a8e3b6eede7d");
            result.Add("uz", "20df1593713b9b0731612938f1441a23abd90e5dc2a51d0c8e0bc8b9bd8e3d0aec13fd224f24944a258c4448331403d6a180256366fe1f6afee363ca03e98bdc");
            result.Add("vi", "87db63f37a07623fc6014bc3141ca65c2aa4196b5e928b11b7f0a53907e81f03ca22177411fb23d4e2402eef6312cca60587a2f7e11d33c0196cdbfe54b60383");
            result.Add("xh", "4810d95714deb08107bccec6527d403dc32dad605021645d15f9e596c317a22b59d36a93e7215e3a3df21e84b6917abf544071782fb34493a6b5368b08fc1c1f");
            result.Add("zh-CN", "9fa5f617420529ead3d5bf6f9e5a37331ca3c0a028e737d13d1a0c82baf11191a17779b15b2a2d2754d73de6e9ae07d40ae46e711863cc24abc4054c0da0e8ba");
            result.Add("zh-TW", "7f9155ae4bfe8e4e77a762802b7ff4d1cb0ad81d8be15cfd01c2e2da6a72d1d3ac01ff77cbd816c1fc624e4c11d38426f9ddf0fd92ff9916c5960d0d2794b747");

            return result;
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/73.0b2/SHA512SUMS
            var result = new Dictionary<string, string>();
            result.Add("ach", "4318f22c8e75189b645ca065c8a8faa99f3af6ed511176699427a2db636fdcd4c5b1bfed95f22b9b9689b9594899bab4360bad8c27b6b1455a1879b971ac214c");
            result.Add("af", "bfa964c84bab659420218aba7c1e624d49e8725abec064dba097205588c7cf8941bc9bdce000e9a9b6aa2724ea6bb66fa47ba6e99541444e034ab7260f3b6cb6");
            result.Add("an", "70f28c0d3942f43eb7748f8958964ee30d8b0849d27acc27cbe95ef82d110b54243522fb9befe6bac0ad898a7e7d0ef9ced1bfd7f328fe610bcc925612b38c19");
            result.Add("ar", "ac6da76855163c1155afbaaf32961096fbeec1a7409f56ef06aeb3ca6e8fca2922172ca654f28a39808413b8aab6a8d52491d9864bcd91ee7722cb4283e5e0e4");
            result.Add("ast", "b977880d4714ca64c80bc1f955fdb4ac91ab50d6904a3fe04f06ed2c1efa14cf919059a8b19e6ab6d048d2f9ec25f78f8560de498fec5a4309fcc66b1158a472");
            result.Add("az", "168b3df36ec2280f72c30e6841fdaae1326cee0c9717ecb9c778e360f77d81ea40af2920d003bd711150736c21647b75aee3e7daf3e4b3214698496f4265c17e");
            result.Add("be", "776f5593dfac08a674e5718b22cf0b484f5c0df34410f24c2ba9b4fa5b2cdc8a0616a7dd9f0d6a45ded7235dc2a3bc05aaa75bbab5ac6c9f945af8c45edabc2a");
            result.Add("bg", "cd05e0ad0d0f6c685920088b270d729030113d67aecf615349949fa5f401c30ee3ce8b3c141a00a4b53e2178b8eda82097ea8144198d5d45477087e37a15f36a");
            result.Add("bn", "9d38d0114df2a13978dc672be79fed3e186da26df97411c781f4e7a0e9d29644ce60d5466df3cd988ec4f4f097f5bba3805130f5b2a7172ab96a5a85703287fa");
            result.Add("br", "df00ecae61b20d180b7fef121c0b2067cca5f5290edd725c6bb54f71ad3da2c2b29f1680a628435e3e83796cc13c0974d812f261198d0327dd4b964d59ca0193");
            result.Add("bs", "034273bea758441ba631e1f76569e7c4306ca92f34d8081cdaeaa211e8fd2b6fb826087f24cc7180764d42af3c25f8b6b5ca38910a081151690729dafe4430eb");
            result.Add("ca", "978831ca00c64d3dcf26dac59378e5cde50d4b353e27fe74bc23eb717749549ceb30142af65b6a33dd822ef2d777d06fa350c119af3edafeff4b9872d40a0ef8");
            result.Add("cak", "659b6b0f13a3375c7ec44c1bd28593d5e1468d56d036e23fb619c7ebd39f4dd06862725869b61b09b2527804216005b356f4b16458308924a83afbb12eed5fc1");
            result.Add("cs", "79367005187e4bfe84aa94b0c189fca1fddac9f666297945cd38949de2fa61fe3f553c4086fd1f07d635a1f1ad3e2064d40d1095a27c7cc56dbfd4b996da1c7c");
            result.Add("cy", "d833fd7cd8d0dbbd8b52ad4bcbf3f23e556b7364a28d587fef11e04ac6236224b6600a1c40d3e99a634171bcca704a6bea8bdb009d0c39d5dec2d47e5da63925");
            result.Add("da", "a56345432afd7fac6bf2be4be50b0f9dc38947ec9f0a070de17ca2fb6b0b26fd121bde9e18fe90a9bc5b2f92efa7e94c6031b7d006f545ee383c00ab7f0fc850");
            result.Add("de", "f7dc8fae909d79ae350258bb23a307565e641ff62ac70d979c8575413f1b40b78cc6950291f7659732eda4ffb881bf9c28b12d631b462f9e542789946292253d");
            result.Add("dsb", "5635151114c9715d0ce5acf55f6e0c3037ead105cb2dc3463fcf10bd5c7daae17af240451fadcc5ffc4b8cdcebeac99b6fa2c41c5bca204db593525769a927bb");
            result.Add("el", "4d35283d056c5ed15894963709d23238fc4fc6a21fb525304b3580770110a50f4e1936fdc4c97aef98026b1e579f3fbb56a28b59f93d4a1989e5983705ea68b2");
            result.Add("en-CA", "36b14954dea960f20793a3d11495004b8fc0a8eb79f1ec07735a573c22ba2a3af6b0fb6299d8e0bae3505753edf182fa87242ac096077bcac9edbef1a460abae");
            result.Add("en-GB", "741291a390cf3f48ff58f8b52dcabf327ba4b09cfe3232cb7ddc4bf3e00fb0a627a82c588cc8a92bb60687bfe86d70024c74666fd0d3fd59a481757fcf3ed6f7");
            result.Add("en-US", "291ac1db873992db0ad4af38a4c5f31b5b67b6f685ca316e30f9caf5b5e6379a3919869f10601517daa305c8abb243d077c80f85d13f66b1e454e11167bf74bb");
            result.Add("eo", "3bca2fcba6e3f8ef837ee48e25e643d50e35a1e5ffff4fe086b0046d485348f371f6ec0e1d12de99c34fd973fa9d3f12592e1a06c2b43f6f5dabed9dd5ca985f");
            result.Add("es-AR", "6832ae969def83c3afce841c3c35569c4d404741849db03556f0d82b92df3e2022c56be3e452d11416fec0fb5c43c55c27f585f475468670bd623503f3a7de65");
            result.Add("es-CL", "81fd64738aec49534668099b003ebd086de28102a82ef5166dae187dfc423567f731160f9b012f5452ceaa3ed9ac5b4df50fafb40ae78ceb41eb2f7b9e590e1f");
            result.Add("es-ES", "b4064effe83e4438876b7d490bcced547d6952057e833dce49eca76c2cb492156e5ff2bffda277ebfa47efbb1a7165d9f37c979aeb0f5899ef0554c0fb33f4b6");
            result.Add("es-MX", "ca8a1182b8264361971786ae0dd2d6053500d0e376b65add323c55c70a321ec5a79e0a685e3aa75d805b0c3176ce86e9b6b7bbf93c162d8a2c94187b8663d2ef");
            result.Add("et", "53868a07653454886b51e6dc9e887721b4ff998bc54e4fa3bbefa52c502c68cf112eb01f6033d4f254c8c8d3179248336f2014a3576059ba5efc5886dcb11f77");
            result.Add("eu", "40a1aaea145a44eac03f9d0fc5d87c55a4611c645494edbeba7a68ae261355a0d5ebb3c962994f2917e524fde3a619f848e551c3f19a1a57d639da7a4bb77abe");
            result.Add("fa", "e038838139f37b4ea5bdccce291d4c760e4ddef236f2f0127b71984cf197428a9ad22954a97c9e0e39ad1a19b6a32dff3c06edb37fd6b161c4ed6a723580fa95");
            result.Add("ff", "29c58fb65aaa3602adf02bbf352a60466a66a3b01dbdfcb3a9ac8122ff720731123d6408bb442ae2d6931f56706683bbf6ba1df943ba60bbda473c3bd36fabab");
            result.Add("fi", "a05421fe92d615d980d3eeb2da2f6fef2a85529f156d2aa6cf22e3ea07f0feb357f06da0f95d580a3154bc0d915aafb3d5331ca0dfad5be6fe2a2c23498b5641");
            result.Add("fr", "f717d5c65f96dbb0aca8bc68ef05b8e2776abf686c195dba10bebaae1abf1d79d42056092607f396ca8f4ddba566e7bdc61b02ee40c06ab1bf55e5d28d6bd4e2");
            result.Add("fy-NL", "ef91464be8c57a77177e76767c1a633a2c9c788771e1dca7aeef9b6e8fb0ab1c80c9fce459f9280694b6212c3855e801b9421979d0651f98c8436cac571e8e83");
            result.Add("ga-IE", "e335498dfc83548a4a3085174c05f990942cf5f7ead2cd33a1593fd4c2a2451510da58b70e989f0a98b09f68e567664a76b9a139d120d4923d8bd423f85f3b1d");
            result.Add("gd", "bc8fb01a688db6a5c6e7f2b3575c4a2ffc2ba5e9b73e61aa1424a689b8dc2b58376942fa9e732f26e2ba16756f21a1a78f101c6c7370fdc6c8f66007ba03ad72");
            result.Add("gl", "10f536cea2189a429ee5003c905dee2a65d83d85f5802d2ff4638fdf7033c6ed1055731a6bdd542edf0055017343a5708e16f3e7e23eafaedb12c7ca44912a78");
            result.Add("gn", "0c67ec1a55daca9add137b29422ad9ef0c7ba539ae42ad81f735db4de73a27e35bf3b6ecba940dfad1fa6632aa965eb2d7cc12e356fdbee9dbdb9b839e5277b2");
            result.Add("gu-IN", "db5274553c89a8111fa0e4ae95fe0397a3a7e8c81f2d21d9e3554c99de6b5ae6f86837e1f333a35b87c00042f6719ed049fad4501c7a6bb515b2c552438d15cb");
            result.Add("he", "206e12267e4ee2e06bca5ce629d3b8a086951e312240a9a23f99436c9b53fc542401885f2ba11ee34b8739e1f5fce55352d57c44b9ac53dc6d7e40afc9dd8748");
            result.Add("hi-IN", "e9179dd20e20c48f846cf5264cab461da15fe69c63fda38ace14478ad0804cec81d1aa7b09dd49005329f953dd4d65d30b2a22285cb666f7b450aad9725c718f");
            result.Add("hr", "f9ed54df38d2c0ec9e0d520371350e3ec0893efa4224f9b17063eef0d0e32403a43189d96dd9cb23ac364364ff11e1a8e35b67ef7b81baa9f963cb13d59a0882");
            result.Add("hsb", "f83d9cee78779a4bb35fc6e3746439214d638cabf3e0206c341fd9bd4be8a9d94056cdc2c50fe1c70f68b48cf40cae02df3ba3e3a91ddbc1e0b7b13141cbc491");
            result.Add("hu", "a5897a91acdc520d4203ef9406a8f37ec693a0a723ab808157fa9eca256e52a5aacfe725cb4fcd4d3c7ddf9c04443cc5a5d717e8e60029538da509900a7ea861");
            result.Add("hy-AM", "273fff227deb7d690705ef25d30d9f1ced92f2d187d3473fbe2da8bea38a2030b4159a4200d68b56c5290d175199e2ffc3723e6902c4ed6f1d2cea4714f4a9f7");
            result.Add("ia", "976d898f65abaa69ce275be51952992157e7874be572b763bba9f8bc41a1d46d477aaeced8707a45a075f89cb6280925fa59109dd3d013cba7ecee297207ec71");
            result.Add("id", "8dcfd9ac0eeeebc35b5992bd50b82ec27a01c3ff2f09297e50a5c9b6b5cf78affb79ec790b456558a4987608319b26dd06ef87134e719b7020bedcf0c7ae44e7");
            result.Add("is", "c39f0ca227c0d4f86f53112247b793f742769320aa9c16220e9005ffdc8d72e8dd2f020d2ed9cf4de09b3b27cea64b91e6c38377b7d1c50aefbc6d5060b04477");
            result.Add("it", "37c4914dd97c9e9b1aeb7958fa5fd5b1d2d37991bca0704aca080ab91e6285c26c5cce503e5d46c83abf544d5af7616da675831f26407a38a6f28125bc2fc1d4");
            result.Add("ja", "bf2fc9a2470cf1b004809f0094b4068e132f341e7f50a79ef333e6dc81e85da158368d117f06f83ac3d063a2d4e9dbe3313ddbc104a1d5f96d183feb3b89d808");
            result.Add("ka", "0c35e8a7d9478748d456cd9188f7adb1043579eb9f9b8ac7027fff4e5d702be69fb523f14af301becfc7684b276f13e6206c22936675f7222a2c8f1c1e1cf9fa");
            result.Add("kab", "f0ded4f46807e62e46d6aa73453320bef175e319829fbc6cd06c4fa8e1aa39ef45d62373155a30a60a3b2719065a486ea9031b84544986d16a8efcdd8cac46d6");
            result.Add("kk", "5450c9419c24293122d3aa85aee5ca09dbe52ed82646fc92f1b66928cdd2d520798b686dcdb04b10a4e796af3cef162920080bc5f12ccf0be5d91083c5931135");
            result.Add("km", "5c93954d66e2855094508a06cd1936ef09a8917d52eb59a0420a1cc79a6662a0d9f28484800e50b8cfc6b52ad296e648c415af75861173266170119d0d7ca4c1");
            result.Add("kn", "dd2291a1161773300a0efea438490ce228ae16bc2beecbf52284f8f6d1e732ce4e4f5006e561ad901bd351a6714c9b21e4856d1897d54c4bf97903ef32bc548c");
            result.Add("ko", "ed1d35a8544a93b2b2fa46d37e49a6cbcd7ea65bbc0dd4361534477b86a2e99dd339f5c2e87fef5efb4c2850b410cf12d0933bd6019cb6a09b9d4c824d8edbcb");
            result.Add("lij", "b619208952067995a9427f4392da780d5837cf8af33c43b5de22ab67892dc9b5278f2d728ef9aeb41552581eadef1cc038876b40118f61fc0a59a96b00224df4");
            result.Add("lt", "d9fab5e216a672020d4232762acc20b03b18080c9363ba93023a619e86e79858f8687a0ae5358f08b4a2512992fec6b25377ae33ef9c76d43fa9c591eddb3229");
            result.Add("lv", "1d7b916a688aa4dddac0d82fcaa746ab0ab89670bbd38af350d9111f08fa496dafb6627e541bb58d4d8964a62f7901ee621cb5f9f1060fa6eb33fbeceea10180");
            result.Add("mk", "097183069581cbb2206d1719916d0606bc4a5cf0918b97dbcb201cabbe8bbffba64d62d1e697451c8682d92a2f8520aba196d4d69dca273f09b91d569afa45dc");
            result.Add("mr", "bc3c27ad102353d44e1a05dd0a3075a30cdf04ea3ca3ef6e334f3a77eea1363649a36aaaab995b4083caf21828e8851fc21daf4b4ff188326fbae7be21f995ab");
            result.Add("ms", "dcc1562a4358b092125d659497a1d18a3886d015df156388469e22c24e6e3e5e813676de35deb43e62d4db45f4b57eed99cbde997c55cdcb644dd5486c79bbe0");
            result.Add("my", "fa39b79a313633c7d07663143b57ba6a7a8d50f94086bae6b43e4a71865f401d8a22df7a24df0788ac48c651d9c9d847c464240111a87d9ec26fe19a6a38185f");
            result.Add("nb-NO", "4317214e4e6f10dec0f91502e0295d8d857cad8f3c426c119811821ef4d17d5ea5ccedb0c56242d38d4eb7fa54b1a2a6b4c3b712a23f8bf9dc54810e54a420d1");
            result.Add("ne-NP", "5a6e8ae8a1b2bc2e000fe4017e8a330c5ab912cdf11df6064e924cea2a3a66f532bd5b7bc5a689a18b7df18a60097d238348d394b07d6c304c0ec8af14267bb2");
            result.Add("nl", "d832bdc250ef29f6c2a2872844b7020e2d85320c91db408993c7dd1db8cedd9d6fd9d285b99109a7f3f2067b661c57a6a5dae825c4b064c46127c2ed8e430dae");
            result.Add("nn-NO", "ee6f1d770c3404a4203671f2145e95d3c282c3b0f46a97e2b090a44d6d406d61483d25b4f34ff73dbd8cf8317b7790fd6d9d16e2d9308be58c31b6b64d93a504");
            result.Add("oc", "7f81ca009d64b062a415fc1abeabf081d2f81e701f09adef40cb3e7842838e568e3d388157456d16e53157576de6c6b2ac20ae7c520799c9d473ccca9c26b8ad");
            result.Add("pa-IN", "2ee5dd28228a8d45357a95b01738a722486c1c853e2581d02663dedd017cc12ddf61e9b63af1740603f7685b6750012259316a1b32317b9946c4d73585a0ac0d");
            result.Add("pl", "b93ad9cb84d17cb961c68d5471b8f4a080617a323fae9a43c2669a7903df63507cfcb28c1b3511ca75e77fc3ee300ca36705779f8c2d67ffd601a23470e75824");
            result.Add("pt-BR", "2baeeeb908a2f566421112626b455cb43b62318e63854043fe8d2648805bd30794d96eade405486acab45b8f6658a439c631b9ed04507898feb0de231c74d577");
            result.Add("pt-PT", "de5f2368fa59cef1f82a1b10bbc1434d42d237d2543e3c593f5c1d4692c1401ecb51a65efa8d7d8d252c8189027ff71887e0ba8cb38d311545e233a4c9db87d8");
            result.Add("rm", "b47e78ce73ffdd3c52f7b53afa9ab7be7e08e501a3b7d0d0395c7b24101dd025cc5398aea5646b59f9d7472c3ddae298499895066d71f15bc30ee46e854ba089");
            result.Add("ro", "ea33e90544f90e8bc8f2cf7c50d8c664f98c23c6c02c05a25c4d7a2f1e1760a522e3416d39ac9712a0a040a62c50492a6c8623207afa0a0ebd89ca36d7fc65b4");
            result.Add("ru", "87e2bf9a174231a82daa75198928a4a587c439b031c004d59185729acc42ca77e5525870b9df97001859f00303ddbe1fc6a147d83c8f2c8444af96bd41ef3ade");
            result.Add("si", "69710c63b1a8717167c693ede9f703d521777ba3ed7e2d73484319c788c171960dc666ae36cc2d4c7253e79d998af0144f27cfe97a77538eb7339c4628ea4bf4");
            result.Add("sk", "9480f4c42c13699d1b871914d77e9a7ce7b7a015de990e2979e9ca1c2d2ed49d1addd058f22b5e9ec2e7689264b34aa6bc1c0fc855a7c6108899b93223ff9721");
            result.Add("sl", "73b83b8b28f56e652d658f542ba2eef37fa9be7856efc31a6849ac0380be41a29c233ef04397519333b5cc7f0d0a51c0ee603d8696be9a3ae31576f4fe837e9d");
            result.Add("son", "4e0364d396478e794266bd31a6700bb053e821d7f4c83e8a1920c2ff9a2d38d16c1233f039653101902bb5ce4e7cc84f70ab5f5f62d9d603628d6f2b1faebfe7");
            result.Add("sq", "21f45fca4ab1bd443fad36e0978cf60c73f26da468db98d6959ac71d4a40577678b33f224f36a0cb8c190a1f75094a5b3fce731509ac0332e4a8ef49ddfe38d7");
            result.Add("sr", "954f67921ab65f8df1fab9599caad1d5ee0d8cef8b0e7d7981c1f55cced1b7a8cbae2416f4148fb430f917c9f5efde0ec060ca30d7e46314fc04511f75e9673c");
            result.Add("sv-SE", "c0ebfee407815c6669f9792352cc68617d26a2e6fd9b01dc44bd67017d12426fc0cac13a3436853c028915411af9a473e9b408ca08a6ef5163dbb61daf448236");
            result.Add("ta", "ed3fcf4fb05d9e2e1e704447f1c6b8b6ebaf3c4467e1587e107513f706641bf03dd7c1c0894847b80fca5105c695d0186271e1a122ec643865785f1758e06da9");
            result.Add("te", "8aa2e08449350e9b3fefa6bf440a9aed6a40a2cb19751a544b1e52b09690133092dd5e5c9b7ac5c1f1e2096f3482df165eb32fe4d7b61eea423cfe2501230054");
            result.Add("th", "b7a98e12e1553a25d12521bb2da09ba50fd56bc1c1c81dba53de4e088969d223144a9bc8a0a38ccad90923859663b8cb3953ad804b5c8a3e78fbcd7803c0f5e5");
            result.Add("tl", "5a137703c33085cc5e4f58338a97c40ccd3a78782803f15297d3f888a74450d2e11d39e34c8918d8c94c2ed2ff0a8fa4c8a074087ee42de85d72f7fe10528cdd");
            result.Add("tr", "23ab256faf3d2b2ea840b3ff4d0aa1a23800ea98987b3df9b5c725f02f80af499d5bdc428edf578024210d111b997bdc854f905d3502dfc868795d1b08fa2f68");
            result.Add("trs", "8fe97952ca56272ee076f696c74c4a34ae400bc04a7b81772d088af6d56c3c78595696b7532294ce4c4e3b9a152373bfbe343a11c6377ecb8f58efbe89ebda9a");
            result.Add("uk", "d3ef8aaaa986c82157d35cbbe059ce046340932a838a5755ec77fe7bf9c798176bdabddb7d3b4065d3d37a2dc4397cc4ed001937b98fa18b1f7b0b60c99d42fa");
            result.Add("ur", "f4b0c8fa00a56fefc6b6e8243f2df8ec22b587116de393f73b435222fffca7b26ab36c43eb39e4a8df7b57966951a731616a16eb8a5d1bb420b24b045e73eb20");
            result.Add("uz", "4fcf80dd51c1d10260fe717b48bcc21af0fd88821f27c2b07f678aa1d6d0abba86915277b7971aa252f2b462c5e20eb19d229ffc72888a779762ea371484a811");
            result.Add("vi", "346ad673af84cbc9e7b5abf2de95b82bc613dcf2c6c2a14c6d7f457ca4ba00e632f2a0f0fad0d0236917762f8ab2246e7daf24454dcbf78add2e15811cc69e56");
            result.Add("xh", "7b0b857d294706cd3581708531d15ccb412f530e5438dd290b20b6fa2b935c8a72bd62b66c23cc5fcb3e9cf53ab9efbebddecac3bb93b6252fe3199c18dce025");
            result.Add("zh-CN", "c6b343065f95c0edc15050b84e08869c54a5f5377d6c211d09e695550b2488ecd4b38d7cda3edacdc3893b6e61cf1785df3f4593e727a60ed7ff032a4cabf6d4");
            result.Add("zh-TW", "21caf7825c4f241d839d5a8bee35d25ad5e7b8679c1f7f331861d4318717f555ad441739bb11c2c54b607376811c859a2465f0516a1079a662bb9602bed1963f");

            return result;
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
                    null,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win64/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    null,
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
        /// <returns>Returns a string array containing the checksums for 32 bit an 64 bit (in that order), if successfull.
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
                    } //for
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    //look for line with the correct language code and version for 64 bit
                    Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value.Substring(0, 128));
                    } //for
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
            logger.Debug("Searching for newer version of Firefox Developer Edition (" + languageCode + ")...");
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
        /// the application cannot be update while it is running.
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
        private string languageCode;


        /// <summary>
        /// checksum for the 32 bit installer
        /// </summary>
        private string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private string checksum64Bit;


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
