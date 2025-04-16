﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017 - 2025  Dirk Stolle

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
        /// publisher name for signed executables of Firefox Aurora
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=San Francisco, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2027, 6, 18, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "138.0b8";


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox Developer Edition software,
        /// e.g. "de" for German, "en-GB" for British English, "fr" for French, etc.</param>
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
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/138.0b8/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "7f27ab441c90de8ba134739b3ff0a817097b114a0dbe6f7e1ed16cd90b4f011a4468313648584f7436a32680cb39aaec0325cc1710501b25d235fd0dfb77fd5b" },
                { "af", "886d72ca41f4556f59b735c0e090feee5ed693c59f07408603878f2eda350d6fb65f6a90c7851693f46b38a3bf4748bc86c3d2d27b1213e31c88ccfeb27633d6" },
                { "an", "5b1e7b0c479de5d9ed400e85b2b8ab2325e45f7d4d716474d91ef65e779ccb780ae12b6e3b8a9320b85cb861792fe04c422c902ae65b287988826177300dd6de" },
                { "ar", "5036c7cf34cc86ed085a3648da23e4c3a054045fb0f65b96574c75531d2323cad3a0cc51bf0d0b073c29d3150ec588048f8c4443887f714272f9f622aa719052" },
                { "ast", "2f2da6586770321b55bb51e3cb4bc69acaa3b6bb86bef70fd3f889217e1a381040668f44d70568fd14fac1caba40478f0c1ae55d5dda16646255650d39dd2d1b" },
                { "az", "ce017fef88d218c5d835f857d55e76dd07f13146db9c6360c5c24e02279d6933aaea4e83c435d0aea9773b9816d2bbb81afd33e9df4056db6f46c4172a71d01e" },
                { "be", "c312e94102de8c8c236892398de220ea767616227998bf8397ecd09601453533f0daab586780ee790c885428b75b1fc81ec474dfc31b6b0ce1f7d8fc3edc7870" },
                { "bg", "b869ccc680603442510df0a8e5f15de26073a7029f901eb94dd3d589ffd830df7c4d37dec5d42edb1d0aff024789ec452a8190f4351f7fa57cca9ecc199ea8b7" },
                { "bn", "f0c630e9643735ad36d3a18de0b8e923eba4c4801e553bc13150eeeb0ffd8697109ed21afee09b3b4748b4f7dcb8141fdc146c5fb0cbf01af2abdf9115e77d62" },
                { "br", "a69e891442dee71ea9015a8a198046b110affe80e00eb824fdcb2c5fcf19af857673eb71ab0c2f1025d8b20481c4ca91cde4a51c8b98a9c26551901d6b936ae9" },
                { "bs", "1cb0024d6a35f1457cc2a7b706ad8cada7df7be9c8751c46d6aa992aac7a50830de8235d41323de7ed93faca46ed057947eb0bdd3b867afaab4c7123515ec7d8" },
                { "ca", "2e9df1b0a32bfd3df0a89fa844515722574a0c0422a2cb901fd004e27a006db24377f7ad484abc72d9ecedec585185cdb273825109d8b9fdd833b53b9e7c8fd1" },
                { "cak", "b1bb891996f60c983e14017bed8388a40b6eb8bfc47b3806d568a4e041a97762a8505d1f5feec157deb8fad02b5f5533a7111ab60d7e827be375f365d655da27" },
                { "cs", "53c8d08ab5e69e2948c0628ce1b1c2548842672c456004868f8a0a9ae6d3eafff15b00fb5fbdc95c3081f4e3aa238da30dbbffbcdb8ea7d50d42381e8889e9c4" },
                { "cy", "2105ceee96478747265e2b2a606781bac5beeae71b2f53c27010d1f8fa1dc73ab4adf32c0054cde3d854900ce6952d6b6a17f06fcbe6532a1ea66c5425cf69f3" },
                { "da", "6a282e37e2c5c2ae1f2b8a82c384de93e18e66ed074f05b44dbeb6ab06c21e26598e5708dd4451d02813cbdb9edd3f29fd22381d3b506c664024f4ab46de7adf" },
                { "de", "ea96dba1791f251ba9573777433a6d650a82c346c6a8290d6decc175963a674c2be9527adae3b1c92ae5142d10bec8b829316b2d238c8881ef88421873c7f80b" },
                { "dsb", "f1d40bbce47028bb372eb758411bdaf9ff6e56d5f462ec01b370b31407a014b41ecbd9a82ac8448deda763e0cfa3b3bc3916ef757f1ffd363dbfb39a9c154767" },
                { "el", "f80baf474cfb41396ac42a9cbb2ba04da0039ff944293cc634ec1abb76d8cfa71401fc4de575f727e633a70b396ae34553ffb4bb9203606b26e3b7cb5bac1ed7" },
                { "en-CA", "f1dfa5f564e1c3f2b7abbb4cbe564a047d1bcd76fdd6f04884e39eb8e98f0c8918b0c92876cd79a1d09cfe19856ab6bbce4921aca95fedb92b2d80fa96901551" },
                { "en-GB", "8196c9ca72b4bb47ec7ec0107c36a81ef5671ee3f82af8a5ab0666ca56eb3d7b0f65cd8e308dd805afe8e76d8c8ccd2a5204877b8e6367c35030c709b8c76277" },
                { "en-US", "641072da4680acddc569bb8066c344659f9e6a15b0e72be68762e501ad07b401897dd876b9adb868f6acd0320cdf2de0fcb3aa57bb5b4babdee454b4442e3715" },
                { "eo", "8eeac919edf483f032bcaddf69a8755bee9900d0548124ac0f6369b4c6b9f78c58b9d10521873e3ae5de375fd201a58eeea4c6456053222123a8989f5cc0b2bb" },
                { "es-AR", "773c6a16976508cd0448e027310c905190f7bdabff691267187eb25232558507cfe81a22831b7838fe2355537dfb81312de92d3fdc69e1f79e84ffffbc6942a1" },
                { "es-CL", "d9aa1c4e3bb71fa5819864f0f24dccd9cd44e8fb7a565070d8564710365538b512c4b424d1d5aefeccd784c5c7e659a1f2c1adbb79bb5fc551b987f59a7abfba" },
                { "es-ES", "0d45c28351ce4579fd234f5c57758ab3b68d254a0d4ac7d336940d9bfc4df2f8bd8eccc1473a3ac2e44e2cb9c5a9cb7e2f1e70a3e3cb7f8e7086d2294fb2f6a7" },
                { "es-MX", "bfd02814ed2c4aaba9d38bb9ce21e8778e3c0d485628b8f0278a3ca679d114d99f4fd3be0d6d7a638b749eff00745e4965266950f97a74cb0935ff0915d441b3" },
                { "et", "1082c407193f39c6d33d93f21ca22375007f4e1bbea0d160d8790bf50b120dbbceb282ff160147cfafc1c6cd776595bd4048f1f027daad5360ce7af6cbd2b275" },
                { "eu", "a2bd8db2a9e40f5b9e83bea1af65304b7fd6339b728e150747a2e65cf1d3d17630c1d1585d17683465c6f44b848d366f60b2ce0a2b6f380df20e283109556e98" },
                { "fa", "0a4b4c2645d0e701a2e5582ed7252eefcceb34a03160086fad6c8d191cbb49060f27c15fd290f73720e5d739bdfc4cf3c6ab51d3b9a369c35f51289fd4ef0d3f" },
                { "ff", "bb4fb0e184f18d94834aeb9fd34a3c6dc659de789c37707b1d35d9aa2a1194efcad138b34487f4950aa204a703b860cf45c6d8a7bc2333041b76aa50c52b1d47" },
                { "fi", "110f2e4a88b74a9ef805721dac626a7698eca286e961bea4d68025f452f075d53807ab3c476ef415589dec9d8795a778b177bf6e69d491a95cc50b511502cfd2" },
                { "fr", "39532611d72d32d2dd1faa851898b582c8f7871b81fa6c194bb8ee468484e66e40c24bc50402ad9b6c86c46d42dc4f6b9b08336eea6eac6aeb451d671a6df3d1" },
                { "fur", "9257f6117a827f8127aff97eec02fed1a4562cdabd59e76bfde42d174b0eb1bf7200029164889dd3fa28678d35d68acbb5a6404a953f275b4e1cb683de7820cd" },
                { "fy-NL", "6287a9af81ba4aefd1325064cefbe4bc07e88f2e6a77c9063df77bd35718fe964d427bd27c660bf7af2efab295992a621a2e459451c6625d36431d894b05cee5" },
                { "ga-IE", "b4ab7fd4c97747c3a3c707ef6de74f3d68b8a6821c203c9531ba36d6ad93940fecceb118a839cc5d1248917b06fb87db1f1507e2b50b175c581a18050eb9f120" },
                { "gd", "eeb09df32b6bd4932bc43b085a8173460e5662a161edd4e8a4ac60abc5cb438915c309edb98010bda8520054d5bb9f1edc2f5c20f78ae4cd6859cfa7827f6e1e" },
                { "gl", "06ef2672c10f3903957d0c01d58a1131b02cd33a9d24fe761bb841f45b4dac12ff58a2807e51a2ef7dc176100dc5c2a0c842c8157e7be5e5b635d28811c08387" },
                { "gn", "a4fa742c36d8e1e1a80158bf19d10719b8bf4fdde40081b0c18a3577100fda68483a2370bd4f2a06c48562c18711d2e790d9f330a97416de31eb6124143b0690" },
                { "gu-IN", "83fcb9636b3c3e2f872101c4c7578e0fefa328719c805ec8e15e1083af089fe6a609757af6085d534289bb52d61c9b694251454a9c1bddc1498eeff9fffcbced" },
                { "he", "9c79cf2aa39ead48e77efb36cdeb59113732d550620316dadb016a1f307813d59e22b849f92ca7d0bbad15d7d52f93bc123103fbd41a44408b7f08b865044e84" },
                { "hi-IN", "8079979ab15f42848fa3c0bd28e80510db664b6aa9694bb4f1a762506f60413ac148d20f4c1df06aae365be4d164aeafa733f9fbce6712ddf89e550b4425fdb0" },
                { "hr", "3cc811b23e69df23d9606a2ad8d5115a40315a641eb15bae0bffaa1257453764264a68375cd052bdef5199a36f31d51e94f7acc26447d22da973be9e93a838a4" },
                { "hsb", "8984e71438604babcd7b95d2f29cbd463121b1e5e646fcd4935bdfaa50174ecf45b1476801d14fe50a2ae13f51e892bca1eab130af9927acd9d6a37d86583dd0" },
                { "hu", "cbfb927d7cbad7fd238a5763ce445a80ff3e2846bc7d1cdb43ed91399492b5a604b84bbd26fa71d33d65642aaa113e89affe1a6dc2ae9daddc638fc2793bb320" },
                { "hy-AM", "d5ea9f941f26febbd53ee37db8109f7098e242873e0426c88bf392b6f74c915df64d700409ff003e177f69fa94e51fb4703980fab96f9b78d91b295a4dd22c19" },
                { "ia", "0b7d9595792ee7b56840f42971f2d53ef284252928f6f07d62886cf6ef8abf0ce5df48a1a5ab575e0338e4673e460edf1e20455839f7eb83448e1fc8cd0d6f9c" },
                { "id", "8faf069786e9a023c0bde733f3df9b9c83bb1e1ededfc8716eca93396f2bca933525829c06a60e87995f526aa45d9ad0c2dac968e70e9c12bb9c9eb01c8cb63b" },
                { "is", "83b5bbba61bb43f19871409e9cd44325d6e3f533f18814a1a04dda89a1d01d6642591e876a5e4ccd3f791bfb8d081dd494039d9d8649f791f6dde360fc7d330e" },
                { "it", "88a4fa7d816c14d91214722a87f3a1bbb0b26fec40cc9d6aa42c0b15bc6a92563b8cd4f4273956adeeec68b50ac62f942e94d520a9510dce315da5d3ca518304" },
                { "ja", "06ff376fc40ab4ec604b95ae6ae35e70a0f78532ad0d74c2b5e937b8dcf1c18692ccf8ae2bc6a8a3a4c1541468dff9deff4e9a21f94a3b4397aeb4ee4c725c6b" },
                { "ka", "92e6a584a78c19f024a6453d3531fd2ffeede1a9758b50f5b83c6c20bb7f8df2447c09155d6e890ab0cbaf9bfad8b79cf90ad59d6218a6ab68febc89173c84e0" },
                { "kab", "5456e2bb6ea4b13178dce3701a12d496ba6796ab583c269516fac322f5af6e7e9c3e5407a2a70bb0dec22cea6d65783d2619f3319e468a513bfa27095cfbc09f" },
                { "kk", "fc99528caaf9792b9964de3aa93145c7d4cfc7df54746d6b34d7e78fb8398bb66d13b278d7f2998cbed57aef6a3a42be6f36edc7cc2bac4d13db05e3faf1bb46" },
                { "km", "b4235bb6ff6805bed6bdd7d1ed5f0709f5ca370d48e5628bb8d16e05a21ffa19d8da33fad4cbc070900387564018f8433c8573be4685a68579e8ec854bff45f4" },
                { "kn", "99d29dd8f5d651beb532a96c364ff43fb51062af85fd64cabfda5969fcee4766fbeb94f67b8e60b0f040e31283b92e0ad370f3ad8fcb9c9dd1eb74df57259b4c" },
                { "ko", "befe8450ea9b2fb6e667590e094989c7a441c0a6da6d277c670f8c4a43d755d364120e7143f485f686c3e4256b14882822f081171560547903357bef7a51038d" },
                { "lij", "44733e73031ed171c47766b3709a9fa9690e4da269bc3cb0de0b0ff002f9c72eb77ff7cd5135d3c5b9e1036cc5ea4a2baac425f438a0f58757aa1be317762794" },
                { "lt", "238b5ef548b123194c68c97f2b5065c4f25a8eed789556ffbd0257ae37412c722bc1313763fdb7e5c365e863722f7aac8179aeb9de8d5f9af72d6a7dbfe30e73" },
                { "lv", "995f8211999365d950e28f968c38dccf3b95a88eb196e7d1869b9874466c9779686c91f3c342680d547df91ad32b14f94947e8275cd6754374754ff704b13d87" },
                { "mk", "99c5c906599cb18fd971e23f75146445d15de10d58c1c9ec7a84fbef80a485fa59d7a39218d34f57990bc08729b90534eab7a6e61faa515f335814332dc4a8bd" },
                { "mr", "ae93c902fee5e332cf52301c0cb4c0cabd5a95f9841b69dd32485f7987b8cb58ace13199fd6a41fa38c7f87f2d29d9f32f53eba822480694d1af1542704f91e4" },
                { "ms", "5130aeff91dde26bc11edcc7f19081ebf121c605fdeb75cafd1bc0f96b559e69761072dfdae4abec6ad5f035f2478a12fa5febe6d576ffd03b42550adf2f6d30" },
                { "my", "be2c3bfac77659e240bf9b240a4fd0c745af61c520b8cee1ce2cea3128c802e676e7b083dbe06e0208f147bca2a2772e371cdde828099ee48601cef316ed21c9" },
                { "nb-NO", "6c98e6eb759f523e7340f960d6d0596875c717f2337dc4decc49035006dfd3a9b59194b585da1580af15f80ef89cfe73a8262c5aee48bcc755e64584cfa9af09" },
                { "ne-NP", "a0946de6e227441f769d32d802a6223544ccdc7d5778775d4d524f9c37443fbc2c3444c83b5d0d9d7df23b9dcbc80e6a5b2b9886a87c05447d0abebef78e9b4f" },
                { "nl", "bafbe678f66ec94893e574fab3bd092cc21502f15d25c46c836348086ed157d2cf1b680dd392e849c10bbf66a70395bc59611a1f93aeec5cd8fa4ce8748eadbd" },
                { "nn-NO", "fbccc34dc6a507ce9b0d672975dae86319296580305c9783ed1fd7bec89da917e5713a99a4e368845c99a803da9134e46386397d236b9564af074e97953f97ec" },
                { "oc", "812ad83726f846da8504f1d9eabefca8755b758c71107a34a110d560befae5370af53e930b8227dbe64e44b632687a026f606b004011a2996c57dbbeab22d4c3" },
                { "pa-IN", "b1cb54d3ba1e4b4bddce4a7dd4bab1d3d24bcbd3079059abe8a089b78d4acf4e753539b86f2124cbc9c3f173d91783aa1d194fb4395f1d268fa6d7082d2e70b7" },
                { "pl", "b07f3cf99108db37e35f9dd4b786ea1d86517d2ef3dc9d9a536c05833f1188760d668b0cc5f6f53a7fa24e2b2ac92e0ed22a01cec4f27dbaf1d6208835bdf4ce" },
                { "pt-BR", "9e3bb8b6cd4ab2659973870e91e18183d653846e7fe4036aa93c77d4eca0f02c8b135aca800dac3241b19854b901f49241d51bbd5b8dab50abe7e26a1c212518" },
                { "pt-PT", "60cfd5b17e4927d26b6fca323a484c6fcf88975fdb4f349beb37fa363139a0345cb4902bd211ff084d65705d75153a106fc86bfdd4a9fb56b6b7ca9f69e9be95" },
                { "rm", "c9dba8d3f434e34615486ce9b34d53ac13a322e3ba7c2a12ddc6dc5abd4900f59c0048a024ecc01da12904a2b441fcbb2e494337d67ce8761493c3af7fc0274d" },
                { "ro", "cf2d3a78cd935ed8db721cd47b2f7ddf28d70d8ced57ac48c918ce9b9978d63e0ab985823b9abc89f46687c2b0bfe254252ccbd657f2b19a5e778ae4a996d44a" },
                { "ru", "20f40320e4e2ffa4dd2f38012e09706772f440d415a0150725070f8cfd1ac675c41fc1f6b48f0517a7423eae59d7cdd139e0167ef8b7a829e157219dabb16825" },
                { "sat", "11bd275a7cf7b7ebe6a2174d21350c02c35ddc5950b0411a20f0827aae19fc6b2a0a253f5c4c5a6a89a987e2ae0493b65254ef04d8247f7ef39d339975fd0715" },
                { "sc", "c1fb226536d963791e42181aa1d96eaa42a91c99f788e0b46cf4fe2c2e679ae7c53bbe03d8846d08b5c7df362b4631044e0d932357f6d4fb0e2ae53e93ae7781" },
                { "sco", "ed250dc70cb605b13dbbfe9d0285203299e3572a2d108a3a4b044940540a1273bc5122195bff6d7c07d86d15657eee5dd6c9ad52c58da273cdbf78c405436446" },
                { "si", "c204f75486ca2cc6df328e5850cf137520f7bffade9ddfdbe30c24b395d2d5606ac6d91e98752bd4761af9d12c10ba69d8080eb0eab1da0fb5cea40c0d686f21" },
                { "sk", "b735fd41aa9378bb6903d602e074f871b4959174e0de5be2679e84fb0b05f8b3efb28fe43c80435f6c4964f5b9c6064aafa4d6416e6bdb164a2d108ef1ce1718" },
                { "skr", "75c3b7ab5c99e1c1306dbd222b69ef16bcff4b8c5fc2e4bd90a9902ce9ebfbd0328eeb209b4c3f156f8734f99845d6a62fdbc6a415027f784b60def3d2f12015" },
                { "sl", "2fa1e86c962de8a845e8c6f52daf842802f95507597dd1c31e49325fdae1e08044fe2e6ba97258f532c6d3cece25257de3364b7058b4feca6ec4028705d595d2" },
                { "son", "7ebcf5e5e881b7239b0ebeff68c915b189089d22d1fae32ab1837c90dc92b4a8463f454ee31ad87328e58ff36b974e405572fe03af24b82b19118b7e2f11039e" },
                { "sq", "c5684b0c02c4b41e9eb463ee9faefe22a015356939f69c00cec26d3020595582ce9f1c5ce7e10ebbf0371c8f75e519e07cdc1559e1e8d029c84f321378aef1a5" },
                { "sr", "5029f0851c46bc61b030e360159ba2db8e2af2273a813f960823d9b7471b5c7036e5fa9051dcee74af08a7a31de9a965b21b3d0aea142f6b5cb67082012d7ee4" },
                { "sv-SE", "39e94a02c8ca5c9a3e7f8cd77b9b2b3bc2e1ed8e036e85afb1e229fd916d6411e9d104d1796a14924b6e1592c9f67d1d5112795fdb23dddc482c9649a5d229ab" },
                { "szl", "c0bfd793d273fc5d4db90984c7d03975dc45803df04d13a90fde759cc9816b0448f2b0083e1335effb1473ab0894979e356c6fd97946e10835137dfa6fe41921" },
                { "ta", "ec1033c603baa430fcb0d40ad9433414625e5c4a555336d2092486a01a4251325cb8c2819076cb5b8e655e3caf9f2f98ac468da59f74a2fc29779c8efec12d52" },
                { "te", "d677c58a76929a46582f12e0c7950ef6394f45e6d0884ec5e939f4fbf683a50eb686eb4d77b9ec9d72edaca36a38d8c187b979a6cce474e3b8d052845837777d" },
                { "tg", "10da441ddc442e02494682619eed46ce7df5b04be23f6d93dc0b5f71c64df76f4010193d4832f5726a4d873b54844d8b20516cd45c8b6ed9d9540876be9a0dfc" },
                { "th", "7b30af9ef6d927c9e4ebdd4c103eb6ff9bca8d974770024983f87bf85d85b3d616c8a1c5d872c09ee50241fa5cfcd00151f923818d1c115848167339eb8d1d51" },
                { "tl", "a1da7b4fa575f9d8dd72472055ec3d38aa33d823b14d3156fa42945e9c83226952a16eaaa4107962ecbfd98e85f465725e358d092c799eaf203248420e8e4bcb" },
                { "tr", "a50bd05b92f45fe54ceb4c8935569a9e98aa87443be4d2fbac65ab6dc9b7ccc31622efbc92bfa6605640aaa9fe73d958ff882cf20926a9170a05395b49a20122" },
                { "trs", "0e64e6caf9de294ffa20e18e5ab834e1d52f36bf8322cb9be0e0b929999c4f65f3280545566017421a0a1b1804e4f0ff21d6a9c7cc873e94860fbc47957b4ba3" },
                { "uk", "2117bf45ee75b8d46552c2f1ce9aa7b0fce46369a1aed657010d3c72921edd186abb200ed13972ebcd20bc852e2402614a681ab8443dd9194dd837d0949d7d07" },
                { "ur", "6ea6ce5bd302bad90fe4002879d767a412f8eac0cd6bba44104a586480485d392030299fc7f76e0124356957ab2f4f6d4d79997b0e9b89a269d4c10cd14b9282" },
                { "uz", "025ae473cb7f07ab63a9c042a6d9e5fc23060a222f20b364333af767eae3a6eafc77cb8132ad960fb1acb63ed440b520dee2057fad3927db1d9418b50b3fdf8a" },
                { "vi", "8308b1f25b660869f1406200aa07d948440632b6f2c9fd138be210b8520f8a0c4e8688ddb5e89243bb08fd47025f2e189ae4f206d72d7e2595762450465b7674" },
                { "xh", "758e58c60672a6c1343552a4f826b7ab4c111de1184af9132171f83eb30d27c9776400df3e2932fabba3baf71a64acbf24e7ca4379aecd185426d8b3e81a2fe3" },
                { "zh-CN", "ed6a3865ce3ffaaf535e2a206b43012a189deff1f5c4c532ef7b47bab5144ee882bef9ac28a6f7e342c9b29ce15a4bd8af13b87934e387ae7bc55a6ef4b405f8" },
                { "zh-TW", "d043b9069a00532371a105af8ed2aff6b0b34bb1ae7617d2bcdbdfec4d90afc1dbfa1d4a5110b0372329d6b2555d7d054432db8652d921e5db4198e515e964bd" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/138.0b8/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "8cbc1cfe827c736c21a7077e89c7abe7471bb173f5ac4d64ff0a445a263fd8c21834c1d1a90b1e5d3d1addf21954719343405c2670e56ad91bd69fa465b46b58" },
                { "af", "08606a74a77ebc5886440959ae7315f155e8ab0f518fe96d7490ec436d60554afad8542c4c7363384ba656e42557f10bcce52aeb79cd80580230f12cd737f129" },
                { "an", "3ca44d376ff163b70e8b8fe0afbad50e3839a8bbaae3097a64e95c550b03af3fa4af4b8da72ae7ad90a87a787aa57b5a9d06b2fe10d8c8c9e499b7e0dd63eac8" },
                { "ar", "b79318bff9f34b331bd7dd4f7d9d95fba90626279055f0a54ab8f24dcec5bd6bf92659231713953b9b88a75d1b64d18bc01f57edbf3e79f88a3a12553f70a38b" },
                { "ast", "f5eee7a8bc1d5357de0e32e0f65e3680d701c44d912dcd3f10cfe7e0515b77936ccfffa41c5bf6d6f725a3f7b1abef23a972f0de1e940b8fe62d88f7930733ed" },
                { "az", "847f52b8665cb0f543aea9f8dcc54322b59558d623c99cc9bdd7fcd6ed602393906543a103704aca5ddac9a2c5b39bf070f3c362f43531c2c85de4e01e0157d0" },
                { "be", "c9eb2138d4b9687cd31e060d43b43d72bbf161846cb295adcbe9ca03c2e572e69f2d15869ad1b20f71e5dbeb59a5dffaca6b9662414c3be5adae567f85b6a949" },
                { "bg", "d420f94113a9f68a1931b9d28b632e5ae46433a7eb554189d9132d080cdac142dd6fb408d6ff7989456cd7fac7a25e82eccbbbc01539e5f825a6890ad1743a64" },
                { "bn", "625c7143b545c8a7ed4bf2e785db7131e91155f1db6d6ad7b0e2c42bfa6eb933022a0138a4bfc8b2ab63043ef4b1f342a5d4ddb805ee126b66e90e71db0eb4c6" },
                { "br", "72476a8eec07d3645a335cecf4015b7ba0865d6e9c8425371fbf35b41d9c65b3ad81d452b1591468bc02681181aaf279425fec126e0b97fc3d1725354ce5fc4e" },
                { "bs", "246d29c2afeae0404da36563972a12b8ae5265fc1bf4b4f6af9d4bd3710e3a2694335b0400c71b3373714999bd57043ed02803cfd798e8ba240af4cf4a2160b3" },
                { "ca", "525e3d03dd14b212ac95a57afa5671f7a6ee1b6c3426a484bc9e0904fb94e1b655ed61ed7813182e792276d74f24c491d2f1e0faa0e6bcdb9624bceea519e3c9" },
                { "cak", "e44da87658ab7f470825eaf28d87ec42eae2f370da1abfffb066a4e91cb73a0ee608a94bce57c96474b96221bdeaf1ed1ccc34f642c47db2e5aa8a020944f8bc" },
                { "cs", "63a3d78e7d267793ec7c1eae2bb2838c32efa54003874749261e5cc9b0fbea605f4146ad31d555ce06d1f69b6c79e0a3c015c76f5c4b5fda9fba457b45d1cf98" },
                { "cy", "de3499508ea3d30eddde260bca5046420f279c74e4a12ca164ec2c1d11ef80fa4a65be46d12ee662197361ccd5f5fc61dea6420064df7fe80e18b3cf93c0e0d2" },
                { "da", "496d2a7fbe04406da072b214124b760fa2295de3df863cd74f18b3789933b30154de6e32551ea65b0351f9e89abec76afe820199baed2b285d116471c899c061" },
                { "de", "f4f4a226c157f92f37e266e39df40c5fa5a34a862f1381ab06d816376817320df9117609706e46366c2f3bd7f124b691011e9734d40839c20b31f65016911462" },
                { "dsb", "69b2da51ed912cd191be12e1986503fd152861dbf56261722ae832e5358566e38822dcae383a50a7d8d85f3de69c8bf37f58be7c3b6bca83e6080ac6aa74d1a3" },
                { "el", "670d8fe72db4762fdd8e48079adf41a7f52b52a0f572d0f27cb101023d4186b2ba7e5369662fc9e6aa008b144e28ce3abbdc52e5a2d41f6b8a0038d43eebf95c" },
                { "en-CA", "74b61b1fdf46972cc2cdef390d724f30b5c6f0a2b2208a3382e6bf6e5983dbc9540d90006e951d381966dd1ae2a32ece9db142ed9b5c1eaa83d07905281248c3" },
                { "en-GB", "0674f86baf335df31585948ba6df28c24fdd5160cb134560f42e9611af2f08baccf50e6e8dd1c49530c2cefeea8da9b00dd17268fc400017f39dd1b7718760c4" },
                { "en-US", "c53e7a69cb100ad85bbfda3aa834ebf11d2a2ca84ea56c942614398b00009382a83ca7c820f1cb7252a9816dc700c46295c116f67a50cd0e7ce50aff4cc55ac7" },
                { "eo", "7333db16261f8b2d7dc25499bcaf8c04a79424eba2d440b49780188ae2549d4d97a7102c65aa65f00423be4efb8091d72cfca45e46360e39bf81ed0ac77d244c" },
                { "es-AR", "5351d048636681505bbd9a9179781c5f961762726af0872a708560da1cc98e5a223a2c0926e0a5a6082c460dbe9a98fae66c30baf9043e90d5b65b11b3a5c578" },
                { "es-CL", "069bb1436f050c73afe655663e3b8c50d42ed1793ae3a697f94a201f3d568ecdfebaef31152b018b1d634142ce0b39e17ae12026543f8c3f13b0a23c638e8a99" },
                { "es-ES", "f848a59029cabb02bdd6fd96a4b61bf7550c148e196172a404a0a96ca6c136df5b64ce39ae3b831e12689efe54c33b46205e231f413af0ab1d98348ba98641e4" },
                { "es-MX", "b947f4df1b0845de513281d5c6dd1c5414b4c74a72c2d736fcdebc95af4d1a164cd34ab2a8dfebaa9c83eff1f49c1ca5d95b41bb92a7e64be01986f2615c6884" },
                { "et", "855386768d9bd1e485b987d9d34a2329983c0ae39996838f82b2d0ca96fdc4162f983de781e73e0a353af584fccd042b3b4c72f538f702e5d3b6758c61d8899a" },
                { "eu", "cfe9f095d3cce64359d83711fe302334c2468952073dc71f8fd7809f298b015227715fa5b739b777ae002e883145de49e5d92cc04c813e80c61d73c8adde16c0" },
                { "fa", "6df3db90d6b5728cc83ecd7496c3bff5d66983d9ae2257102722a427c3d5257ec15a4940597223d368eb7c61417687dd3772356482b57346534459e930e32120" },
                { "ff", "1d19942ee5495af01d8784c7333b9c2dcaf954a897e53b592b30fb7459c46e8b3f2cf0224893d949bc2fee510b9bfb0407c008ff48eeb59b3d15c3c9aa11941d" },
                { "fi", "0893b47b46a60c57db86d93884a4fd3841a39ff9e7e996bb65cbda5fb5710d00f10dc8bcc04df9fccd4f56caca38d2da84391e075ca892cf5e5800c1c95f089e" },
                { "fr", "cb981e125a5d3cce871947d9dd2e699dc4ba5a63387c1137e69ddda6c586381d1662b47a142a2bdc6b23ac6b1c24d328f68ced5d6388eeef2b45fdd958892cc4" },
                { "fur", "8dad365844a295b11006ff1ab8cfc8d2c9c377916bc1b417d874a541b88d36fb31d1b192034dccf6ad4b38154610ced59e1c90bb1a1a37733b1095177e6014f4" },
                { "fy-NL", "bc4f74a777a5f0d3c21b1c3ffa603056d6e3c13afdbe07555e336a75d5f43a974ccfd67a5078b149b1375dad73f65e537d0c7953b5ff9b907270c820ba9b7e3b" },
                { "ga-IE", "989c822b5f3a8b34202772dd4eb5347cf4d30e64863770ad10d6ffb8333bed4c1e5978431d5b3d19448a94a73f51bbe086908962d3a8009aa1636cc466781661" },
                { "gd", "1009229b59e326e3513edde2d5045d7f42426f41c3109d417268df8ffd8013775968516a2bbded87bdda606c4a282b0adf1a87b32933e34e4fd34140b8884f1e" },
                { "gl", "b2c2e645d075ea3ad233aa1ada9dfce8a4022b61e2a28f36809fc7e5fda4120453a6eb1eb7343afdb05d11a779d5386b47041c90b0af22f0cff080d54c335d72" },
                { "gn", "b295a46b640c5d48b402d852ff1b0e8f794de757c8d92cba9931603553637b0e54bb036389c7b84513c2abd7e7d42ceeaead4f346b36ce4363cebee4c958e269" },
                { "gu-IN", "2ffbd915d61f2ef57ba5408a624e97bf0c0d59eefb5c11c26c9c0fc1795357cbf527ac541866c0fc5f01e6a54272c4502ffb0b86aabcb708fbddc6ee748aed96" },
                { "he", "073c78085eb4dceab9b4ae6d5dd18892034c89480aaf450dfea812f868615df8e20629c306bc51295e5934f1fe1ba554183e0cc9ec416f9493e9853401920c96" },
                { "hi-IN", "f9eef044b34ba932dda829cbeafebbdb6e9d19cc4551f2fca47fa96979f326b70a3dd3b99ddd94dc08b3e94aa30c76749ad0397d054aa688853fee1a5fd5cad5" },
                { "hr", "ac06f879547e43d1c6ab150c952dfdf2df6ee357f155b9e4071bc210db8e50561881e276d8aa057afa9b842abffee278e8b042ee29e606b148c81c4a3e97dd79" },
                { "hsb", "1271659529ce94747c45423c6c34bdff6ec394f3be1df8b745f5966a70f5f204fc0e470f6d1951b74dc8915e6d8476b33b7b7861e6623a13c75af1c430d62a61" },
                { "hu", "f37e335625b9c7ca792bda767f738e1b4e21efab7368c6b950bfe6b57bbe60b7c585b7a688ef4b2ac8b51bf2f98dcf9bfdb6439748f9cb716dc6a04e6654b174" },
                { "hy-AM", "03b73494bee18a13fa90a7089eab92712b9e48a178b5f25f333ef7fb81529ca058ee2da358f628e66f508573861951066a58110279e4f60f5ce7bdedcca65e74" },
                { "ia", "f8c0312ee859c7432d0397074972ea4bbcc04ccf84cbaf50748743eb297b751f90f01bcc2d282291fd0def8d15b2ce17c6f318562e5a8867319b9cc4725b86ec" },
                { "id", "219664f3e7019070c63b86b8d4d6895264e3f37e9ff27bf6bd0db85472639ab3108a810c8064e293368d01e836141d2245f548112424c793fa44721834b184e0" },
                { "is", "1b4e07af7d395656d9a521960387142ee8d358524ba6ea088daaa36cb17e7f4401a74fb34635b65c7166b31eb3a08deceea096974bb907c537842087b3fdcb1d" },
                { "it", "9667de4bf67afbbbac4e849cea239a1d57c17bdda493f9c415cded8f04f14777296e56163ae41956748672d0256ec9e77632f29cc147c402b7172f01d5d8c31c" },
                { "ja", "2fbc6486a79c1277d960ec74dd211bd6ee4a90ea9cffb6b4b799fa43b1547870487ae06b6e06d1788dfe2a8ec26737177290e8d8b707fc40f9be904ac65c9b45" },
                { "ka", "b401273c52049e903f332994d540a44652cfd195708a16bf4e0430abe967c2ea44002974d880ea8a15a9261f2a2c06642d79c2d601a8e7a82d8a5c3a5185f10a" },
                { "kab", "44d73a05df225b11b46cd06de8679060b58acaa564e16b99548e9cdaacc87661055d4ac07e7ee2fc0c3afe96fed1113b5427a955aedd24feb11da11601e95a76" },
                { "kk", "5a9f73b082827faeb567f1a37f349b836761c844e6bf8bc5d033fcd2fe3c490d155772e8daa5a98569f888e818ba4caa788050ee85f35f65bbffbbd648eab3c3" },
                { "km", "770d29a954db52d439ff9bb864e8ff60e001db56ac81f70e2c6e3a38ad34016450b9d999c05f024ee31b6d492a7b4e0a02e58e28007a4dc74bec4d32e597930e" },
                { "kn", "155618e121fd51ef11451734b00b7b0af2306422fd555463317dca49aa525dd5a5e6c4593bfc9e7b7317ea6829de4fba0604da254d7f1fd830f4c13d5b276d1b" },
                { "ko", "cc758f3fa11919b0c96774b2702d5924c333c4902ba839adac6653a6abc4fee006fe5bc38d9489c7cdd606269831beb2a4ecc0732d73dd774e55c1075c73e64c" },
                { "lij", "24b7791fc851e76e056d585aecb78022ad4fb54996f558aa9f71c042b3c00a3d95a32ca4c4d6a62b804fd47de8d589cfb0c5c18441cb0253c49ae2ec46c0dfd8" },
                { "lt", "37ab4b31d70f114356e639d46d63b523a027da5c5fdaf812e37b92f99018e474e99a21202feda1f6b0ea847ed7b6ea8bda532c10109ec4480249e1b2cbafb272" },
                { "lv", "b9472db2368d6f40271e2ab9cf1b112631f238f9d5ca0d6b9581039b0a1c1cd67702a336bc1e794685d0245c65acff9dfa756d231a1f88bc076a20d834c1c868" },
                { "mk", "52f1e31f0a3434d28421eefcab0247820e17ed735c4d321902798120fad051aa5efb4e93313155bd90db7952863997a13c0efedce322e35ca8a66df50b547b3b" },
                { "mr", "6ee61c2d2a8555a0626f6231669a03f1787d852b89cb969587d700839c0acb5710ea79d8dfa8fbbfc34d0ca57ba50b962ed5e23b94303d8e77063d738a5ea839" },
                { "ms", "d4b20991b57c79349e8945d65995856b57167611fcd0e0b000030e21dd0f77fa0c193757ef82c03fa6b6231fda7f51b7b66e249af85e8bc7b58849b40023ae93" },
                { "my", "54362f0a0656cf4ea93ecd587f2b6b2c9385ae3382708792e437321c6ab1ffc377f63c302c392693db28a64941607c5ceb08e47660e26023c8cc139c247565f5" },
                { "nb-NO", "ea7795a9f168ac8fd19fdc769044d0482636a11394b05ce81801c11eca8b488fd5668906b221372a8d9806fe5cada77f6bc412dc36be481bb91d097e162a1884" },
                { "ne-NP", "988927d5d082eb37b71e9c1893ada4d4e1adb5a0cefd92e8a5b29f671879411f24eb0a547193ba99e8ad58f1b894b84682225ce75afd4cdc4e15339f05e9531d" },
                { "nl", "b04ffb61898a82474440133b9a26abfda8e5ce238a4180aee591acd6b57865f0b0fb2656bb22b172d2dd548781f4114405e99b586c8f961bb4aa41571ab8aaeb" },
                { "nn-NO", "04162d1f6277f4a2eca4602710ae8b3ffdcb79f69da0bdfd4c5ea1ad129f8058edbad7f4445fa23f0a351fa20657af415c938f162681b51325f01f94dd74cc3a" },
                { "oc", "380d861b8bf52fec8212a753eb9c72a52dfef2cb58d031b75bfbc7904d6c599ea2cabbe9f23a907475858c0286cdac2215516caee6893cbd27e425d637bec65f" },
                { "pa-IN", "9d72fcbd277e00219f713d3d628b27b9e5e27bf9232b071c1b5b2933a0f6845ab684117ec38dc0d4df47e92eb73f7f903b1e92f571755c206727cbbdb3c0fe5b" },
                { "pl", "643aafdb7a6287d1cb75021f7ff8e1c0650fad38863bddb6c834398b2339a199adad5a47a6da6af1122d827f9db86183551d37a3c8ba78e0e85a99c547d9e5e3" },
                { "pt-BR", "029efa312946619bf4f5392ac3d3dcb9e13a287b3d83db316d4513a63f260c93b5d11060eae75b4150e1103c5e8ebd93e6ec10914099eebda223b0c8fbfc9d26" },
                { "pt-PT", "dafc5e0ef4f5207ad2916b0f4048f383c8bd97cebc4700c5d08dcc2264998a1216eb4f1175eb0ee77129aca3d3b2e435bd635fb2fb1a58ae8022bf95e38b9a3c" },
                { "rm", "8d422d531024ca2ad6907bfef102328639f08b7092542bd9f4ffc8609083666fad3834c07f9c3e2190874f9eadfed738511f802fef318ba5e1c21c64c1a12470" },
                { "ro", "72844bfb32a6d887ace78b7ba5888b795af15aca56787ed0e458175981955b267fd9ab306b396c4f33f2b1b7797a65aacf081667087e707fa8ea1a902c63f144" },
                { "ru", "9cbf9adc9f40ea8f24a2e1ece213ef4de7bd24b20b46294d6fa4fd3d45a2cf690576e1a207e5771bfa864fcbbbada6b2e6e798c6cb4521026968e24fbfcde3f7" },
                { "sat", "f23d20ca7961c424889929d45e76864b5a1aebde8267ade3aa76a78717a8d6a97b4f80672899188b80295a253ce64640f1e85b0c276945760b32281763b73e1c" },
                { "sc", "e52e80b1b8d3f2638e57a86523745ad1f403245d1af9b101f912274c8c304a1e06449231c3f9036a149b9f4524cecedfccd458a0bf2c91e8a24e6a2e6a8495ec" },
                { "sco", "0f48a49e47f579de6913ae0092ed44114c7dc1031321c90eec39f072ce61ccdbe46c4706d0fda317c9ed3529b5e07f6c2dcc034db220e5fa5719c0e73cdabd0d" },
                { "si", "7508027d19024d7f7932398f790180788f913028ad51fb8f317f568ce643fd25b46d587e5d897488e5a68aa57e12a782e895d1cd7d8cb855207724ca077890f6" },
                { "sk", "02c855062cc3fc7b1b786940dd1885cafc8ab6365cdd32b81502cddd3057e391d49cf3d62affa56e4265a612742c7a00dfee8e7cb81d9825d3e700d952613216" },
                { "skr", "7925dfafeb16dbf698d92f363cc0bd85b85fa9137c926fbf464a11b18409ca63ed88024f2f0cbf60c064b2e26e5d3a530ec46bd46a5c758cb7362d6801561e75" },
                { "sl", "7944e364e5a42e3afc5659fa66322fcacb6fbac81f2aa73341f98ae66343a1f7d6e29917c47923a99b39ada26b880fb9ca4e3c88a55a900396fcb6a87b88b92a" },
                { "son", "a678aeadc2d85314e53ea3bea54543d56a54b643cd7dbd845aae2d1a5b7d159de5d65a2d5f48d7929d7a3705ae152e4585656f12ab126160b57384a45ddce737" },
                { "sq", "3a7fcb5593ba808c9ec4c065d02bc2a4a8ab0ca4cf14bd7431287eff4a30e5c2159b4d658fe251f8d3b386769f75fb44a8cf378ece83598edc206e8f36af0209" },
                { "sr", "d614474f51a14a1e7dd8bc5fe5f2bb0a7605b37d47e7988307bcd4fe4e476d0bde09f5690aadafb2d96374d422b734b07810fc837c86c4e8f74988ace2bfea6f" },
                { "sv-SE", "ec08ba3aba3d4f5cb397162d176c7fa7f9e73ac89e853a147c3640005613ccd5278b250507b133eee86bf65f164096aadb9990ad732c360f9fcccd61c9ab1e00" },
                { "szl", "5d3f7542c7e662ece243c7e1f61b3f166d67ddba22771be523ed2dc4f7bab0a7c692b5eef84001f7e4341c3e875129b05e92382b9085ef0d0567eed5e5f03511" },
                { "ta", "cbf945bdb5ddf39311f61dbb392dadf4ab1178719081d27f5446fa9b3bc33f68b2564b470c0db800078d75f3a453d615311d0c4916748343fd1747aaaebd8d5f" },
                { "te", "8775e065f057eaa4e7d308234390a7d4c08e0a150b9c1467aa88b35ee5110e3cce47d40256357c97a8ce75871d9afa5c51c00e6bce67708f9c3fa41bb38ef048" },
                { "tg", "6d11729c7b33aa0ed5c993322fd92c71caee6a9876a125fbe53b98e14b9f9a7abe6033b88b3ab04c03612ed29c1c269528523297546f4b9f907dbda624edb5e2" },
                { "th", "4efd45998ae39de7331b879e9ff846e720dcfbdc423c6de05d426a4ecf55f6cf419c8ae54bd02f02eb287b5b7ad71372269ac5e1ee5b9cfb8b2914b349513ac6" },
                { "tl", "91696caf5f26e865d62f6b237c52eddccf99aaf24a4e0aab9190760187f704d3b7d9f83ebc86217645c450836a1e227c7c12a5af75c0a3a8d41367e43e21338c" },
                { "tr", "f837774137da1e6b751efa00e68073843f53bd3064e430559ec2f97fe27af6a413bbc7359a4a8ae2ad5c2ff1549ef1860fc845cb2cd7ff280d08cbee15c1e4b0" },
                { "trs", "5a22b60a0bf3282795cce292286a67e310c64b5709e1e03e76e7e974f32f10c002b233b8320869479620607ee9473fc2451094007971ec35466863a117641017" },
                { "uk", "92e8d510c16958d546cd2812ecb5687a4d6ed4be06cd1b93d3b8650e0a62658885ef61bdf52c6f83fa95ad9457ac45da62a21622b00e9105833b2b84c8f218e7" },
                { "ur", "d87c6d8148c6f03c422f88bc8d133d601ad92e8bfa494c33bd3484c7627ba6c78dcc7b97b60d84ccbe2801e83b1cd3bcf6b457e768c564fbea2ec006aa74d383" },
                { "uz", "c4b6a42d6e0c6f08dd6b97c28e3fe822da515fe9650106043827e09100de1af45c1496e5f96c0d1fb8c174b4b481abf5cae1d76746c28bdce39089599bab739b" },
                { "vi", "07ab8dc09cb59f8ccef69d060c278b8d2c3b7b1a6e3c20eee6cde22d65d8b668d489576469161884672f5573cabc25bc78a73fa7e1ca00688ee4d45b1833d8c2" },
                { "xh", "f418ea8f6101988f7c99186953fc6830d51f776783e5d73e718fc87d8a5f9c9861d49b82403e24da490bf84e59224a6d293f7a35f4e27b77df0ff567963f6e87" },
                { "zh-CN", "f761d2c31971a08fb3a37d1196811cf6beb01fde7e902944c22db3a1decd1f7fe60fcc158b88492d1d264ec2dee425ea0f87e13b03e8586043785007478a797e" },
                { "zh-TW", "21b4b5f21983fc24d70320ebfb3a275abc8910de18b6a14d2779767e994ca58f91896ca2d5d324532ff768f250ae91c99c99916306196b55aa30041dd2fb6085" }
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
                // 32-bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64-bit installer
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
            return ["firefox-aurora", "firefox-aurora-" + languageCode.ToLower()];
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
                return versions[^1].full();
            }
            else
                return null;
        }


        /// <summary>
        /// Tries to get the checksums of the newer version.
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32-bit and 64-bit (in that order), if successful.
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
                if (cs64 != null && cs32 != null
                    && cs32.TryGetValue(languageCode, out string hash32)
                    && cs64.TryGetValue(languageCode, out string hash64))
                {
                    return [hash32, hash64];
                }
            }
            var sums = new List<string>(2);
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
            return [.. sums];
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
                    // look for lines with language code and version for 32-bit
                    var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = [];
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value[..128]);
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64-bit
                    var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = [];
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
        /// Determines whether the method searchForNewer() is implemented.
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
            return [];
        }


        /// <summary>
        /// language code for the Firefox Developer Edition version
        /// </summary>
        private readonly string languageCode;


        /// <summary>
        /// checksum for the 32-bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64-bit installer
        /// </summary>
        private readonly string checksum64Bit;


        /// <summary>
        /// static variable that contains the text from the checksums file
        /// </summary>
        private static string checksumsText = null;

        /// <summary>
        /// dictionary of known checksums for 32-bit versions (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs32 = null;

        /// <summary>
        /// dictionary of known checksums for 64-bit version (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs64 = null;
    } // class
} // namespace
