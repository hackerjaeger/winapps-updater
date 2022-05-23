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
        private const string currentVersion = "101.0b9";

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
            // https://ftp.mozilla.org/pub/devedition/releases/101.0b9/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "14f1acff91974c5b11034ba584769777d62b59c25e97444b5e47aefe8756e503912e9e772ab20a073ba8fd8effe80a1dc7936fc03f92a6f5f98de586fb077721" },
                { "af", "e8f611f7c3c3e0866035e376dfac57f0d3a8711cd9f3f82f12d2a24c4d2ce0b291ef0183b8035e8b51c67a8d1388ffb1185091f5b378470ad977140e367b7fe9" },
                { "an", "9e0caaaf4c421d15b46e45015a993b98949528641f9757073c216651debf8250117c4b49e11613e80a3b48bc8651d08e47ac774f6deddc9a450601e0389789ad" },
                { "ar", "f02cafebfafb4e25d94a5b1add5a96a3a1113ed280a0dd20472bcfa7398cb1300e68ad3cbee3d044c1d48808b1416d6ee24168ce08cf99b487c3e6acadab979e" },
                { "ast", "96afb5c5e6a6e162ea96504499cc7c37e9f3227e555a133f7a06b3ea0492a9853f2e10c10d06c47bf91b9cb0a7f2f6cb260919be344fff1df6d71736bd2b2a44" },
                { "az", "97741cbea1e0745319335029727fb2590cc4ba67949ef580ad11077426c94cf56a689413ac165cfc3a1c70e568a0fd38f75acb7a0cfc373d368f4fcdbc7ae64f" },
                { "be", "95312d653318c3bb4107bcf8a009adfa7a24f944a5fec7653a29f4aaf860e45c9339f728c68c80fd87d59441a1c0c7061e7c194d385c5e8800cebad8920bfc20" },
                { "bg", "885f25638bebf0bd1a224717f8da0c1d427e0ec8db32c954c87b3e87bc64185371be97528a9e7ac4b5f0b8377fd21373dfa21438d57889d27e8da4b90649031d" },
                { "bn", "d28bcf3ded103740720588c1d7c027c6c1e7bfc8b064c5bc6ab9e11ec865a9e8c269988be9b627e115eaecb80770b7b2411f8fce99cebe863aa8899e0ee570bd" },
                { "br", "525b449326e5cef5f01837f47f43a729cab0e4163cebed25927b006483db93771df7602e03ce6b6173cc17ff6a4873f5fb365e253ec97d74fa89f98c1ce54e56" },
                { "bs", "3b161e865da935d8a2846842e1b36f53227205f9348d126eed0dbe9b7593bd4220099c03ade5182d6e7510bc6e31b8bc564bc38c6778730a678fe29d32a03c3a" },
                { "ca", "0f6dd378e8e2b7685212e5048a1ce24504511a41e9ff437f7db76de6ff8d9fd3cfd3cb4afd18b4bea6bc62f6a2400eb0e1fd156977a70cb39d42099e60ffd123" },
                { "cak", "71a6dd2542bcdf70cf598c8895c05c0723fb2d6a0c522f046c115a98498f338f56d69bf04a6d730cc0f66aeee8d8b160d0e11c8c83e0670fadeb7e66f98e66d5" },
                { "cs", "5dd7b96aba3b3b776705e69496e550ece34501c9a20ee7d631d693dca8e670b73521b364626e3a308ed5622ae34eccdd0e147b321a6f07bf6c161662d9fa9398" },
                { "cy", "0d01c52f5f14aa7c927400a26b61d5d1b37ffe3b1c5456eecfabbcf3fa2cd4f432e0708c9efe2c4c1a32bb0d7e2349fca2e4590639f846d0f7900d4fac6d82c2" },
                { "da", "a5fec89241a5d2da6dcdcc6264b2e583a282f63fc1a7e8392d5b21ead6712037f5825fe5612c3f12085d0bd6fdd83bb23ae28cc2bef0caaaa0af7d3123e8418d" },
                { "de", "8bb9eddf0366b2725d52eaa42d2c841eae2c3b380d2099775843ad7181143bf1c34d9e688b76f574b8a1b199a74d4a22452a641a65cb2463e9619b7105e76f61" },
                { "dsb", "3028c27d78ce9a9aa8e4c4b063d914e019a202aa902cdb5d4aec883ee74aab67a17472c9c505edc24e872ce25e7ffaa9dac25ce4a9ec4115516d5a1a5f6e715f" },
                { "el", "b8f62b2df677a8022f20e14635207365a007c63f2f9bbc4fd364cf0ae66e64daacfb4264ce2033cb772dee1dfe5dbc9e6c5c7a5519df13ae369c5cb138ec9ff7" },
                { "en-CA", "c246f11aece43d8a2e1b87743dac4590f86ff1cfbbfde85feae6ef3c9124eae58a098d74236e01d699eb3d79b5d02d25bd2fbb658c1274b033b27954e5b1dceb" },
                { "en-GB", "5249ce4824790712318625bb05c0399f028a1986d3d91083b1c3777e6750d087ef1f510c5c6c4dcd543d0a72e029eb66a825f302d265f6468701990caf46c7ab" },
                { "en-US", "44f5029c58a393c296b0a5e368ee2559ddaa824e2ef631a9b71079adb53a24bffadccb01eb34a6a9f4e7ebdfe299719739a1d178396a1011914302a65dc7064b" },
                { "eo", "c03df99d6cd89dfc3eaa3046bd1e87bae888d64e5e6b11fd21c08950e891c0e92218ecd05c1a3d4e01f6e02b7bb1a0abc9a3aa4e18b35e5f8603f58d0a2962ce" },
                { "es-AR", "290448cefde198a3347a4e50340a0ca2bf270bcf9bbb37d3adf6b96cf0cfaf6d59a9b6b88c6ac3816b8848c36e84306da6e4e8df60a2631350f40ee3ed201423" },
                { "es-CL", "34fa18b5705759f117ca4ee62a3ed88a76eac413fb2dc81f327bac23930b29b6c799ad840382ebdf05db0d94349087e4ba040a356648a487abfc3461f1e7b614" },
                { "es-ES", "f14942fc5dada1035e0b2a7096a0cd9734cc1bb2d3feaca63227c23830dc040cb4922f95f920cdcffd379e23f09bb323f998a44e8caaeedf321bce338ebdd952" },
                { "es-MX", "4f8c1ba73237d8a77d3ab6ffd6e7fedc19f311fce73d512b9f7990bf797ad06bf9224c0349110aaf3f2b8f10301031a53c06d4f1b6803d31efa62404475f6d31" },
                { "et", "f9627d7f6de77ace3adf5eaab6ee123bdfd74f33ae6595a43df6e131b88bf64257caa1486309e6a8743b6cc0298031fc8bf18b42a0f74e523c6be605f9333f2d" },
                { "eu", "e3d79a251ab545466dd8030848ae081d4e37680f2a732a03e1de5212dd47c9cb834d8755c762a5c0b60c26c663d7e7ca0b69720a1296635aaab978f65d3e8fcc" },
                { "fa", "2578ccdaa798d258a966015248b9225bf7b1b2e54b9fe7d96ce12d3ecde2a6bd6ba88b3aebbdac1a030bc5f205d200d244a985800d5e3d4e6b9bbb4f3660163c" },
                { "ff", "4f88e8d7785d9eb2d13d392a7a262fde889db8bedcb46e3c89874172424a75b32f32605489ba45e0929aa01e4005588fc5de329d3b47c5a968edd71faf6cd396" },
                { "fi", "8191606af162d68e3de552ab19400f0ac4962702c3469452abb06b4a473759e5cedc8abc3f18a0dfa5f41c1b6467c4e4d96532de2f6f7e9e75d92d0f856429b0" },
                { "fr", "2804ed2e7acd6881b87852616ba4d1097b47d4a066df6f99d285aa0a35ccb8f9e7730175edee856eaa34419e2ae77ec4ed2cc1a0a245a7a08f2ae9fc16911099" },
                { "fy-NL", "9c1821160ad859bd71abe57d3e77fb8dd34909e3766eac9151009127ab1c744d6d8936a492301894b7fa8758721a72f76149ef38302c341159c1d87d5fd6b610" },
                { "ga-IE", "066d42908f0e68dee769e7b33829effe803f083c4b28521c2d87b755710ecaf93acc6655b28fc8dfcb532d010304bcd3aa5f2a66e0ffa6feb6c40d62cb9b76d2" },
                { "gd", "360431e2d1f988b350b2f20356105ff178604810a7273f2ab81c765d383ce7da7021ec4a778b4b4e707d9b37a1d3eb33bd4aea5c2a7ef48c0fc5019f39a79a1b" },
                { "gl", "869fff93b01d704178ec173c03b51c17c65d9e4a2121dcbd114ecb74076ec9437d8df3e1349068f31ab4e245b815d1e7d41d4eb1d507c05ce9bae7ee0b3978f0" },
                { "gn", "5e2ea3580407a319560ce806b771c66ef5f204b36617eeb9dd9343528eab42d2f3d0b07eb35568420fb75909b21032de7a81ab691ed469ed26332881354189ea" },
                { "gu-IN", "de12c808229526c2a91ea77fb9775fe29f964590e16810ee83afab4e239d51a76980cbba1b61339f6a2987068c1a9dec3e9a29e26358f55a78104ba6f2331c1e" },
                { "he", "177df5e40bb9834ad7129987c12f3abfa5f3ea02cbfd35b86f9b5f53f48dd1ba3b545a189a477ac9a26ba18a45c2403bcbbb0e155e2a599a6b2b338724554d36" },
                { "hi-IN", "e8f896bc4cc534d8fa392d325962f5d469afa7eb5694b08e675d19d35936504ea25da108859fc7c64fb862bf460d12c3b845e66aae91c0bcd8dc4e7beac2b6aa" },
                { "hr", "76a160cd684ec54530363ba16acf890fbdb0774377dcc7df70b525137f11c6fe078c15f157f65932a439b65f1f313049e37203790481c1f646bef43fb77815ee" },
                { "hsb", "6e045e9e5611b08e6e8edfa1e1c5016af773812fc28d9dc4cba3fceab040efd5a09a2cd87fcec5bdf06a13713206cf5146116f2fae7aa915399b203162357990" },
                { "hu", "168de7fe7de0918c40d4f21001c0f63e7bc9b20db27b855c1148fc176e3fc4c8ccf232b82efc57b015252a9a1cbadcca61696c836903aa78d03b036e26257e09" },
                { "hy-AM", "f80da46bbe5c6ae4d9ff2d7c97ba2b13a30398766d5cf786df14b69a2cb74e46f7da9ac6770b77e081ed6602fba2d648e030fb17c2656aef8defadbcf62f78e9" },
                { "ia", "9d0e389d09097cd90df906f374a75d34fdcc0e339de9d1eb6824a0e9463aa8d021feb9926f14bd30d191fb5f18db16cf422cd80d8df8ac8cc36963b1513112ae" },
                { "id", "25070d011cd768402624d48b261f3202fea34c55c0cd1e67095658239d36cfbbe5ae258138d1875ddf69a3c050ecc799aca4779f0ada1c706f3786c899afe8ec" },
                { "is", "0934a46abcf4e5ad6ce1657ba417a0a038443f9bf1a879b913ad40b62c4709246ea0b3a9c20de0884376b8fd4aed181c3a1f817682b0aea83505c563c4150150" },
                { "it", "aec57e7d2d259d3f4cbd075cfa623940598f3c7ba5a13e342e59df6607ee18792eadb8656e69699855fc973e8821d7aec04ac660e5ffe0236a5e353a30057778" },
                { "ja", "c3ccb6df14d3e08bab6a0305f7ddddd37ea072b4e6c365b0bb8c0852af27a400cea4404221ba8d2479bff8a9f6d637084abae10e6fa8fa71ba40ddcd6b4922fd" },
                { "ka", "a183a4f9ffa061025cf6ae85c8108808c201dfd1e96a114bdbbabd4ce016b1f3f2790f0e88900aa8b9827658ff72cfa053c8decd8699606798dea0286ed2655e" },
                { "kab", "56d3232002efd79eea535dbe44dc2f1a1affb545bbf07a2174d8689fd66510e26a3e60de176cbe4f639018c13edd4e7d5e221e90239f0062924ff0dcb86111be" },
                { "kk", "3e356b5b0a2af19562e4aea5b0bef40a7d65cf72c4da6200d147374d0d9e4fd1596192d3ee874f3a1d2b408778121f7e6db355f35bcc9fd9ce8ebbcc23fdcb4d" },
                { "km", "49cf13bc1b467f6576e52cfe1688b7e5df9df72435d1dd1e7474e9845f3832116d0cc7bf5285d852d889af58c2485c19b2267474a7688735f4292954724cf7f0" },
                { "kn", "2ea7dd50838b36eae7ee37455b02d67e9d58d0df47d0260d09fe6d41ae26c97eef30e053d0e331802f3c10622be066ce4d518e234de5a5763994c1373f686708" },
                { "ko", "dc636d4ecb6e901de8f23be43ac66fbd4826212b0b5c20dda8afea8d32fc7ccda3a1b54317b5261940462eec6cfdb652ca969f0ddc054f9f5540b53e8ebe07ca" },
                { "lij", "9ffacb063f0076868c280d2159edda52bf3b336424382e72822b6214d41f8a4549da4c6758c5c3d2ff4acc602547e49c4fd6c6b11668dc1e5a13300c271f4f5e" },
                { "lt", "2625416b239431254016910c80b5a1786cd232216dc878d0f4077be4ced9bc574be1cc25ca83fa2d02dfcf788177a1a7ced74ac312fdda6548a18250f831b108" },
                { "lv", "f213ad211d552118ed42c77e09051f1f3ca93ff256a8129e4095b7a157d491e9015b2a2e9872c813047c5105382c08fc59d16b752e70c0d258a2f0d32636381f" },
                { "mk", "657c025ab1ed70f701439bf05cddff42fe7c814adac7a3d751714f9d33250a85c38dd9628a32f3cf43e7202c66320931f0496f3221f11a9657946c78cd4a0c8b" },
                { "mr", "3377cd18fad8387d23af759ebb2a271d1f2327816df6c893bcf893caa08cffffd98011a11960b0dfa1b5428fafa370520f2b0ed4d1e593cdfddd47ba2c771bcf" },
                { "ms", "47f10d1afd0bb898966bb99947a5eefadeb6d011cd1d068c178159b1ba524971cadeb34237545fd18f8579d94be08545bc48a1c0d1152b0d75b015a443c19980" },
                { "my", "07d0bd39f0beb5dbe500d48374f82fadd206f875c1c389b5b13bd42d07cc88fdaea72d54333e9659f1365b671ac25c2c0fd91754155917b13d3ae7e3a565c6c7" },
                { "nb-NO", "d35ee18de204cbfc6b90e0b949d782735bddcb1214c21e75a7de2a1e31e5bb9838e15d507364fff8eca5bbee716c5ea3e9b20d98a45f70d677294b6fa5de1be3" },
                { "ne-NP", "dad0ac750cf2f6a31b09da0979fae835d494233d7d3a9688ba487ad80ef204c50162246a274e47bfc77b2f259da77d42a8a9612d3fa16eadc10a539bab16cec6" },
                { "nl", "ffa61c0820900638b35b9cc45ca311e8f1280b127e3bea91589c95a22e94bf1c017e247bd5a7b349c0bc21d73017fd44c6b1f939e431af70fbbfd260c638588a" },
                { "nn-NO", "b40f6958d1bf1c05ea63f346d1fa5b71537b34e413aef647ab912a34fde69d0ecaaf8a7e7418d3509a10053b8bf7ebff5d1942032207ad8e35f247c6e9deb39c" },
                { "oc", "d043e49437848b0a08755b5db8510a4cf0d1a6dff4a99cf22adb44761469cdc7ccb9fc6ad637b6494a5a120499874f874c3d57757dd59ab1968ace9cce2971da" },
                { "pa-IN", "bae6cb4bde332e67e27385a06d8cb6d66d3c47da89e8a2538d10747758482c516b8d33d3b03b6e2cab22c51a360b77d003dc57da17db5fdf80529d440be62f14" },
                { "pl", "428af5ba86308a2069ed3c8e5790c9b4a6d16308d65f68c24c23051f0bc1ba8d5a23e5fcbd896c9115629594cccef4139bd74b31d4092453f8de697525383df9" },
                { "pt-BR", "d96b00bf35912f6b655795a30894b2c1bd4fd3ab8bc0b55d8f5a5c0f59a806dd05b0c9f63f38fddb70145cd00a0a0d9bfbea6673a5ad9cb580b16744faf228f9" },
                { "pt-PT", "f118aeaf5afb6150520a8dac61e60ab0735302abdefaad8701161e216dd25ad8cc5d53062eff9bdf9a41b7f9ac6dab38e1d98820f75136b979bd339060e07ce7" },
                { "rm", "04f0dc644dae1a3372ce88802766a1865aee9e5e9617b55b1ac02e35500e5c26d1d0653a0dfad8d85b5ee58fbbe9eae2566e14d7208158471770e171c8379d7c" },
                { "ro", "b3e107a63314985dd5413bd120181e32d86e5a41ac6a0076891a1c02dd39ea3333ccc23f6557665061b8c2f94e221f21e97a329c6427e61285c2d0167ecfd14c" },
                { "ru", "0126dad9d566946fa66fa926711542784673707818fb100d9c0dc0415f093cef03adccc1788d9788e7d164805475f2318ffb44fb53928d4e4cd8131ac8203642" },
                { "sco", "03127e94807ff49ae0c65a075d4497c31209d2fb51fe8df30a1601d80a778832ba04c6081824ac21f8db3b99d850b2c05609305226afa9cdd2234fa7b7b965d2" },
                { "si", "85232e228856922c079af7d220516253da2587f3c39bb1c48bcb6753701c23c949187689212891e23f167bbf4afd0dbd3093949984bbdb01735dcf38720c08ed" },
                { "sk", "d255348a112999133e114cf4e70426f4e2f3ab216c6c8f8e270b09db024286fe7caf08382db50e1f13cb179b65504a4949cbaa506b895311dd5863c6dcc3301b" },
                { "sl", "081924b64b8f8cf249adcea994b8c8e40223af718dfb08795700ca19520d47aeb5c5962a6812852f80dd99f44ab7b044847578cfe4afbbfa7eb60e4977824640" },
                { "son", "51e1033f92349b72afd8c4f1abeac4935bd5a171318f9da40f0af6840265c336c27f4f6c52d8f51f5117d2fd7ca9f81a85901098e083576febb4f684ba005e33" },
                { "sq", "b342f7fec8506bde810a16eefd85c6e8539f8dddcb332bdcb206d2184d9f95f1247f2fef5d10b0deda3fd5decf1ceca8ea85370cf62c5dc8eaa1feef0a4eb91a" },
                { "sr", "cec3c326a40bd61c98335a12c39d1bd2644bb5ddb609d43d25fa5330f56e0f349f545b188afdf052dbf3732622cdeb6919222c4395119827852c45aab8906429" },
                { "sv-SE", "be40951185aa485b7235efb382919423778a61e78856982017851334de26a8ce82608447315355ad2c8fa09dc71767a51b5fe0e6589c0b802ebbc924fbef8de1" },
                { "szl", "7e60b553257a4ae19cd7d44bb241de9acffefd4dd2d5a96d4991955c40bde8e6fafb9da412d18db8ed6279eb44c8dec37d7afba4027ef1525e842e4dfae6d710" },
                { "ta", "673d76f3264049ee6f9f0a61cbeca5e3b1117ab36e300cb5d6a86ad3d9dfea4e81c4e4663adad46cebae3ad66d31fd726332cd4dd1a2cfb6c9d6224e8b09bd29" },
                { "te", "4e22b3509e6ee1305fccbfb4ea49db288709d1c98c73afad776c1afa61dc7cb09d0891ae3c5fef11f5b27d61846dcb10dc3184b57e7cf6d62c8e44adf0948f8d" },
                { "th", "5ab74459004da0bdf20744d4ce60237fb6959e7b94bf3f257efdfc2d4144c40f107003a1588f5fc7165c90f2c725b33340a70e66e5fd71102cef9d30bd6b1f1f" },
                { "tl", "d030dbce8e7fa6fdd896421c5fde620169b8e7a5095c1f12ff53e166a8acc4aa397a139d9b898c7df716e79b69f2a892532cd529fdaa3ea8ece8cc3aa4b9fd91" },
                { "tr", "791845e8944ce9f7e7d620669c7c3795b67d86430a9f1572ffb42d3ae88eca0d5cab0d993f3ab8b063bc63ed09d54c46650ef8580aedad07685417f0c4184dc7" },
                { "trs", "2042d9e54ccf2eee2308abdb80e8275774c6dc70231d7a83fc288eb04a5f133e923e6513995418d46f5cd295c9911512cc9acdff01314a9dd15d17851213dfed" },
                { "uk", "14d4a7a92a63434831d9957ae60c852cbd20585a2e424415bc0dce25c45b4d09e9c40431376779964fa24dbd66b66d5b7393bf268a13ed47e8d511faf7fe1fc1" },
                { "ur", "1156ad6e2ea1e338ff1871cf11da701a7c5fdef834a43c0659239debaa667e93996f46ee0429d6c54b73e98dce81e09c7c7a89701f5eae144cb6ba524cb793f3" },
                { "uz", "e130a3919c3e394da2897035b098a8f75dac4b82fcdb74c9eda74474069ac50cfdfd8c2e418410e6fc9358049036e5d7a2520d14e09609bd39ef5ffdcc42639e" },
                { "vi", "bb8615514cb86e200cc4ae09979f2842e47cb05cc7722a2c88b772d1afebb8b1670a1d6a946872564cb7ae9631b7305e67bd7088413aea3d922a30b476e784e2" },
                { "xh", "44b95f0eb5ac568d6cfae857744b9f09d7d01ea59d67961b1039a2b09e344cbd3ed71703cfb5e588c13efe6b797d61d2723b362864ec868eb3e0022092945e9c" },
                { "zh-CN", "53c12ef08178d8fdc14e96e88717d3ecff728cf1dc0e407cae4f1e3599d381fe5380dae8545fb57c843c0a33bc193c8e511cc727883d93142dccf70abda281d9" },
                { "zh-TW", "b2f9722626f838e54a6234a9929db6e57208ce1585eed8b2ac1ae06d4aa32ea4700a65f1a935e564f05a27d7236471c505a9eae790bb53a0de4a9b2a8ca854e4" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/101.0b9/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "a98a143189faaf7fd72b1868680eb6f6b8e3a4e9252dab7898a9f93ba75783f0de9c0c5d25632aadc3870867e00075d9cdbc803956826bfcf27b7196ee6893e1" },
                { "af", "3aa69466a5393a25496835c31620d79932ebc985738b0f7ed631fcfa49f1ddd5f16b1cce07b77ac6f743b528af3d1cd5a17f4b455119a1feada111a10572970f" },
                { "an", "bc0004812855330be45d4de04abf2a339590f77b5108b5ea46b5129980521d1f76ea8f3ca6ec9b1253565bcbbaa4c213261d38f60f3bc41c085dd132ca9ca0f4" },
                { "ar", "6344b8ca098faeabd7bd81c4b75828d8a85c699f4ff5f6146a02627c3cbc2345eeef2bf35d0027d9a6b56d18f709e790846f2c6ee2adfb797bcc63c0a1621354" },
                { "ast", "882b446beb5a652cfa3243f80d35282fa218843bea5dc8fdbf5c062d82e7cac4e1ae5eb4a681d97015aed28b87fb41f4b77c5903b26550dac4e71bfa62377f0c" },
                { "az", "2f08395e79d953053112786c65dcd34354c9fa33b29f690564904f3076626f7efedf6987bb1f00138dbc759893d19c82f8d1579b4c529c88cf204358ac625145" },
                { "be", "af5189397b56e2b332ca0c0fd3b113fa50d42da9337276a0eff93139677c804eadb0e7b80756112d5a9dc20cda602a593de53100ebf531242279d94c4f5ff47d" },
                { "bg", "90923511c36e7a1051e5c26904fae095be9c7373485ca930911dd3880a47dc2d7164465e48a882fca1e0cbecc637df7afad1010c3b11ba3dc734d3d5e25a7ac1" },
                { "bn", "d2d1690f3014c31a659489d27b0739bc176c88b69c5fcd29d01c84a73d6a15007ebb8cc11e8426d80f03f679e281ac7a87889b71e4a43aae1c2b1b6fdd29d86f" },
                { "br", "50b9d9ddae73801d6d0bb64cd258c6abf9d50dadb72f6b8b4ea360d855e5fb7cb8ed07d9c7a21afab46613b02a349488605be447599f4f26779aab8b77f28986" },
                { "bs", "c6b3e774a373e5be2f4535fe482edee9b4af5d83bee8a4a5a809a50eee5827903f640f268a2e092ab51f01fe1b42c492402a7fa781537a251365825741c9b0ac" },
                { "ca", "95894823ccb607f3031223b762ef5564c5f14c061d77b1f3a24548800052a630743a2528beafa03a49bb986ba00b69ced3cd4e98d4ec18d32bb802c157cd693c" },
                { "cak", "4def7f34e41968d945fd156a66f5d8349077bdbaed76c827092148565d30c8de47a233ca66c5bd7b42612e0d5626a65290497fc2efdbf5e54b8dc3701c467027" },
                { "cs", "f1071d9e4a269aba3dae0d51bd219552065f5cc424b4c16d5638094bda9f77f436593a4328019f4e45dd96e97d7585a7e825972a7078df54de487d4e2406ca8a" },
                { "cy", "e01b1c0e16aeee80cc9ee019108e17fc8287434e7a95238dc63c327a29b6da5868ac1cac33abb5497de8ae3f57db7b6e157b067e7879770b7c2c2405e73d30c8" },
                { "da", "19687b1dbda3e95b59741e8e21dd1f6fb2d8dfcaaca11333a222487db9ee049089c119dea048069aa5eef61dfba9518f9405fdb60a363ad21e825bf2bc749cfd" },
                { "de", "69e7e53a52b19d3aa8c59f883ec1bd9e0e4d3f2ff694fc5ce0864b9c169bb3cf73ce7f0abc09c4e5980dac9fcb75cfe62d1aae7fd3fbaeb175a5ecec8d314d0c" },
                { "dsb", "e4b9ad8cad348da64ab72a3177f647423dce905dc3acde60a3b6faf279ad90bddfb777a49d7dd7115056ef08d7caeee924a8cf129c06fd7c49384d7c4e5f3d10" },
                { "el", "fb7043eea3db2938a25d59f86f2fc1557b3ed9ca153a29ffe3779d5d77650625385abf30766c6337e7a1f4bf8fa18dd3d5a2c6016d94b9c0eadcd32177cd6acd" },
                { "en-CA", "a6335782d09ed95f04681eda7ac113f687a44c760cfa80cb425b1db2f7c28502b22897eb2d2dbb219d1dd0c09e927e02cf7a0103b00b7eb454cd7aca13acaa78" },
                { "en-GB", "8006ca5d95ecdd46c932ec327b4bcb0d82ecb52f155b5ee6e1f963c22efed88078351ca9157b8e453251408cf967bb6b9bccb15b78433eef892b61ee78de303a" },
                { "en-US", "a1718aeb9b1c7955f9fc5158fa9e7e5440a364ddfe3584c5373538bc4ddb67b779d9750af9b98662e5d98ecca89eb7bee2c7259818b527c0baf02c0bf7abd07c" },
                { "eo", "d80c05d51a3c32d766d31468f64c34575673a9bb7a4747817df3ec57736e2a29d6a5631138a97bdc64f3a04aab0da891ecdd11f293dd155616273a7b1a5e3c10" },
                { "es-AR", "87a354be64abce1c64acf879abd5b439c6e626ef8997a485fae34e7ccb738a16b7c8df944256d98ecf148ddbe6da4a00f729102f59d7eb18ee36edc7aae6a7e4" },
                { "es-CL", "26ab6eeb344a3459d8ec37b07d784f2a342fd189a41245f532ed55a6856e07e934efa64059b7b0b04f7698271e4f9a31290812799335f85d6262eaac8fcc0937" },
                { "es-ES", "225bde07fefc129cc307e51fd303cdb2779008b165808fb926d41bdbcebc8c02426dc2611a6ba8c3d1673f1e848e1de6634e8bf1d3becc5a80e728e5504f28a6" },
                { "es-MX", "be2f2349c3435aa79b6ec512b94603f57efc795047cb49ce4cb0e3d6b71a7a39fa5dcf033a09840905b3c4c4e7f5de07673f40bbe5bbe23ad4004afb52e3a944" },
                { "et", "5b0d7cb2d9cc854326855557dc4100e47d8e141b85ce08952d53b7042b3985e4ab96e353345bb7b520ea07a2e3deb6b21c4ccdc075a13980e27e868afe398345" },
                { "eu", "383c04d11b0d40d505442217276919f36a6ffc00b2b93b2a87019349dddbaf9d93636073e5ae3a170fa59910067d73960a28a32e53c481d76741e0ff5e168983" },
                { "fa", "9243d926aaaba01460799a8d5acde8caadedda15d86a79eb5f33f41d8a4f6c4578df94173d77d3f13183ffc5d918009e4637dda165a08238f75307aa38fa93cc" },
                { "ff", "849bdbcc2b7eb3af97292d073b99707a6d20e86022cfc5f30271925a94def4a11d48d772b3ba0adc4cc8ddc41badad38d7eaf5a309140043a7ea8af21b32224f" },
                { "fi", "93bed0c39791faaada4a508ac8906952d9e8b7548ce902f522b85a2862d2c025d441a5ad97247e4b26d42edc3e62eca78aece407b474c3357c3731f37207e310" },
                { "fr", "5e6282f93c65c0b71def584a1d9c5c01664106cde21576a13d50ec2ada81fed61922393e585064276911a3016a4e73933a2b96b0d444174476c7cf900d9aa947" },
                { "fy-NL", "bcc1aa3d72a6af31eac5e9f8c8bbe3a2eccb353861db2957958235ff31b0da02ae527ace65f6b1da7698a01c8bad386b92dcd4aaeb97a1654bcbab425ea1358a" },
                { "ga-IE", "cc8fd216d5b7da7a853e789e74e0acaa4764a35045a61728bce7b1b4ce5aaa968430aa035e2d1de20ff5750209b8e59a0e9b69afe7197dfbadd7bbb0b2a7833b" },
                { "gd", "429a01ad05325fe754083f573375a9f376e053998169b016b5339baf4a92e887f72d48ee0450cc6731e2c593bde773ab8e83a2bf834c7b39c60b1c18310b34d6" },
                { "gl", "dde384d485992d36a3a7a697c21a559b62f8160ab0dda80e520a6918f9842cacc0e4f247c55c626b8e078e129671bbd83e70fe61fecca0821b7171d0f8eca20b" },
                { "gn", "d6208f81ff253cc2efd2be804e00b313ab7b1b29660e810ed4abd0b5196e27d5fc63367fa1f9a9e4428a8adac969ca30af753c8edaef1273430e5aaee2ecd1d6" },
                { "gu-IN", "8e7be5201384182153bc2b3c2b7e11380371c41ff52c38d16640e0f85b107e962a71c6fe0eec7cc0a24b60eed377b0a0c6fff6e9f35938aa9a1eec0517447b7a" },
                { "he", "d55cd01102e02fa8110ccbb0a9b48ceaa3ec83ee28ddcc447ef4bf69c959239048b5ea4c55bda4bcb24f8766553fb32b8180f337e757e340ecc727444bad45a7" },
                { "hi-IN", "a1c5e3bdd7dceea27b83050c225416795f377666fec0995dd1fdb765a13bb507ea17e05c9aed19bac590b74041e9143bca3bd86d1bba070e970f9d428e029a2d" },
                { "hr", "0753a369a1af85d48c2af8b47b2dc6b8a1f881889004800219bc2afb31055de676f7a1af5a672cdc0a8c768ad9b6749fdc7406967c90b06734acb59f023b03b8" },
                { "hsb", "0a40f9a5448fc0c91f269b5d5221b67acb0f1ed0923cc4e10128f84c34f1f2cf6129c03b6c8228920d2c22d2529e6eba9a46bd130982bf945bacd1a829ce8dcb" },
                { "hu", "5bdaa2dc73ca5a89852e47cf258ecc802dc53b6f5f67ba48b259b6c3b155eff9eea8b1605785f2dea6d45e09ccf03575c7fc2a0f85171c268446c89df5543359" },
                { "hy-AM", "23ed55f8f305135783d1f65a2ca6d0c9522f8e6947b20eda6cabb426694856f8e11d06e0587c8b016c204931605b80bc2fa26bb15a1e64d1213a7445e4fd6b29" },
                { "ia", "e0d90e0a1c955b2091258abe522b7b272bdf1fbb8e61a0980a86300683446d9c980489f8fb9caa536c9bbc04cddceda62f307925ebb100c4168623ee19f78459" },
                { "id", "fd9362cbaaeea01916249158e469faf7c6804a3675b58990582dad5ef8740e9fdb82aeac462693250ff21f8be6cb804abe86e2437452900468c84878e6030ffa" },
                { "is", "e1a5d43708adfe446ad9ffa6b732ba6f06364132e8461cd226c6eab35a61cb8d17289e825c20d02151832bea11634ddfe6e57d6b5142cef4909a0fa9ec784570" },
                { "it", "3d6d4df1bbe357438a73800d125429b183fdbc57556244de9eb739f018f5ac42e370fc42c8cffbe3e2579ff8e5ddbc41329f0f3be2a5b71330d6bdd4b3299c0c" },
                { "ja", "ddce544090e704f5095ad088a01f83f5dd5b0c8e10684a9852d878ef867d9fed0a343c24352b1665aa894d12b414b7ff62d7cda899f77477fb08b4b5d1a19ce4" },
                { "ka", "ba957af9053da5b882b7eb9928e87bf54c98c740cd8eb3928f5fe34e726faea4ecd29bd4e51494344ca9a1b0d12195ca479ec009a6a40e030d6e7baf98ff5015" },
                { "kab", "e3828dd5645c40130b220e77a50f3618267655473d500378ca270c1d99240a77a924e0109a09eb322cf9241c33241505962a9d06fc9102e04f9a69a1dfc0801b" },
                { "kk", "464ebed3bd80abc9ee2ad94ac24b1fc7e6726ea85c1af4bf4ac6c60b3619219f586f754b2e11aa7bf0276bfc7084bed3a961e081c7cd6fe9c3162545801a65f4" },
                { "km", "71519719722c81b474ddb04ddbc2d6105bf2db45ba14e6cd229b77c56b6ed39edc66523976d6086b675bef31fe2e096c4fb607f5e6a0b29bccc65665af18c165" },
                { "kn", "d6327b3482e0786ce62f03cd3dab1c4b7fb3adc587326fbabbc97dc61ccb5e5c41ad3da9c69d7303a8012b6829518b325698ab9d6a49d648cb6ac3f9efdf289e" },
                { "ko", "4542193abe5e0f8d4b5f70c2d4da29ea321e610f0ab7c19cbb6a590b00112a3c75b3fd3a61ab7d2d9ea8c42e02d54614ab6ba17235c917b88363a55bd7fe374a" },
                { "lij", "23fec277a4c08b0d3e56c6254a4331bd7ec8aa329a67a3fba8c8e90b84d4e7dfa7f633a80d4d0a60dec2c468ff42c883422b0a4c4d273d92559444d72a496dab" },
                { "lt", "d507c6eee4952cc0196f1ebee48368d858482f0308bfd715d4d6ea2f0bd2e8194afc9b5f1ff463ae6aff5846ab7ebbd7a9b0bf1cea1dc134fdae757871deb532" },
                { "lv", "54b72e866ab5931a9a3601aad7905cd4e9600dcc1d2668a6743753b69617643adf39ed241abff6bd89178739a218c13192f3c200a1abe0c9b8b05a6c4f2fb71d" },
                { "mk", "a8dd5a35aa03d2646916ee862a260d5242e4d4aa7595cbb337a02fe7d3ef32620b7f0c54f9483ebe7327a072e726161db23340f440a2b30ba3f58ae74f17708d" },
                { "mr", "497a3dab4e1d4783946de8afcf5e581cde7ecde1c951c2ccbcdc6d5ef475639677b906f468e196bac76b75ed04cd78be24730e20aaefca4cc84ee57b91327187" },
                { "ms", "0b8a70e88726040e83bc0c4955faa18c71a7b04097e8bb7c2423e8493fa12bac4b56f40a240de3c435f58cede03fb61d7d2e231cb6c55e2ed2180788aee9db70" },
                { "my", "d73bf5880063d27e41540a98d9bc5aca868233390f8f9367e7c1aae97b71f3d4a62f9820c80fa8271c4ae1d517ea2793043d1ae383c7b3d133579b42f87dd23d" },
                { "nb-NO", "c1ef28770b99d261c4db30ae5a90187ae0b618deed19dc0e035e75bcdf56b2aba08c887891f7c965b0c76b23db84d3d7ea81cbb31b52d9bfafa105fbf1505d8e" },
                { "ne-NP", "5b43b085a4589807300276743212477498af6d53b104f509be51e8ce6173125bf864c83e6cda9ca45af00af7f04e32663dfa770dd9147f4a14c7ec6093a8e0a5" },
                { "nl", "071ded5a80eefc5ee103ed2f8e436253eab15d8c56e9c08a51c07359da47683317342b43d1e5c988da49e80fbf1f67660a4eb1b353889ca752133ca343190ffd" },
                { "nn-NO", "6609722a96e4afd206c91a4a8ec37d0f1e47b72db87ac57fb3dbe2d9945e4455f7b820963b0f214599f0a6488a81fbcfed5152311ab17f39df86db5a31790442" },
                { "oc", "852124fcefe5c3384e7c1640bcc826055c5d0fff26c45197ae8752ae1e423115cd323c64bd4ea24e6b89ad3aeadbd2e67ce2146b10b33e919e6b20fa1d20318f" },
                { "pa-IN", "7ce20a020d89a8cf15901c468879df16de7724eefb193fcb3afd594411264fabf29f69e3b7993dab0f1ac61402a5022f4e3113411b4b8d1c6b228673f39d6f73" },
                { "pl", "16c1de2f821d7f99be701911a574bd58c61334709afb275c660724d34f827552216d7ece3bee57a438db68e1164f46574da53c600f15b6f4f6854d35c5360313" },
                { "pt-BR", "834d223e3d7dca2e758ae0b181198ea6ed77b5f77573422a030bcd28d1af873364104fb72c94fe378b3456d259f5f686fd559107cd712a217571fd9d0816ed0e" },
                { "pt-PT", "e4999cc95304b9fc8bb3212ccecb94a991644a7e366fa6c86cc1ec2e625ca9de14fd10348519586483de0815756e5b9c74013c9c224b3806f0e0a6fc17ed7906" },
                { "rm", "591096f1f993be0828f189e3453825db9583b5848960afa29785eabbaf20dbd1304bbda46399d88a8d59b841d4f8d6072ce9b90ebec2503cb4f7ef165c46e498" },
                { "ro", "04f99240444120f5b75f9e2b779af525ed43bd1238b9e736ec352242b3de730cbad832790309c711acd87efeee5f3f1af3107cb80aeaed07f722688a80127d8f" },
                { "ru", "c3ed2b95ab482ea5a8ceaf5258ce3d2d7e582c5a5794262e73ac5c400e1d4220dc24f8f9d35065bf5641516edde6db39ee38ce5d8bf49fe5f9688730768fe013" },
                { "sco", "4eb7360fcd64d7c73bfc0e41dc8e3d9d31f35821b9e31a409461f9f33e8226376931d336034e507a15c6e62dba155d15a4942bdc569bb6c2b51b95a6e54f5012" },
                { "si", "7bcfe90402ff468d445ca325b11ea6e7e49142300ae636e502cfe6e78374404ef889c25bdde1f48cec26f6dbdf8058c451d48ea1f3cd32346a668abd471640b5" },
                { "sk", "456ea585437d7f43f890356f95a5a1541f257559613e40a2fc7b0026b05a64a37bb8ed9b9b87a33160012ca4a3d0be3ccf7192ae1a3200f4214a9d5d417a8aa1" },
                { "sl", "cd687f13e6c9b2ab3cf451ca53edf201402be3d60dcb5f826163d15b96f880c6dd2a8fad1ff4176d63520cccd5bcbe8ee677550105db376bdb7ff3f9e232d378" },
                { "son", "a3fd2670fe42a3af35c8368971ecb00c3aaeffd191ee5d3ee71928cf4be1728fb7738c2fd9ec11b184aac7359f5347ad943b3a25ecb2e37f2f921c93d41a542c" },
                { "sq", "63d5998ea1032f7585567bfb5422bb524bed721f455858a956d23ac36c2b1787066e7d73184b07879ced4e2f7b0b8916c84f94f7515478034a382f02836ab400" },
                { "sr", "bdfbb11deed2dc027c38c4ccc61c9d1ade210c48be589d3fc7119a0739f7324966fd41ea83f11f9afd98b67af5eeb046b4c6acfe6fe2e6e32490c49e49d32d1d" },
                { "sv-SE", "25c62da6b2d094ecc389c728f661bbed25e64a15df55ed3d6b21739ca67858c4cff5abecc4671de159c423ddcf58d4de064e85569c858e3b13bc7aeff5f40ede" },
                { "szl", "08c4b937e50121385e7a504d9c58fb02f66aeb2d58007082c84178d4f54b66705c74fb37fe38f9ebd122ad9483a105ebf06d750e6ce6c1ec7490d1d13fecbbc4" },
                { "ta", "087f61de9434da0643fc8ed381d3c7f3b70641cfea357e2e114a12eeeff839fe98986a434cd5779cb0843dcca99121670e91b5d347930081c03fdd92e9212b02" },
                { "te", "2fee9831f89a5cdb6feba32d35332ac667e0db9105f5c4349cdc95d2503ebe685402ec4c090decffbf3a515f1df685eb2365b4a962af5c8d098b2007d9eba9db" },
                { "th", "58a47d74973f9fb2db859812a3b136b7a3e01de4b3ca8fd2d1008e68399130ffd3fdfa075dfe21279d10a34b0fb4003d072add9852a8169b924a4cff15124602" },
                { "tl", "e82d44547f31f788831c44dc04716e1b7f263b7604de8507a20c528af3e2524cad7737b93ce25b5b41be7b0b486f64d87b46bbfc68d50c9b5a874721bda9bde1" },
                { "tr", "8ede4d04e1bfca2cb8897bba49fb7927d512feed26d905c69267a29c124742297512b6fe2b81ff8eef0f1c01e984a4f1a593c9f18d21ea9c99c29edc9a59448b" },
                { "trs", "0649cdeeff7f7edb3a82d0b7b59bb73658244a319777dfe885fb56dc6e845100fb6c57b19ca64266da39b66ae3d5e5596f8ae17ea052d0000ee9aaeacf733a38" },
                { "uk", "b8cba00a81af6b08b1c1811e7d574a99ed5669552030e42625222cae5fcd6854d44384952cd853b1e8d8fd76a1778e5693482ed75d4568478c72cda5987a9680" },
                { "ur", "98a526ab77c54408767857956060562c8c78582aa1865fc6500c08a9cfe0ecdf5ad60dd08c4ea22b7351ae872256cdfded2bfc9af41562330c89fc6c24ee9743" },
                { "uz", "0b1fa9316d5d7eef95ecdd9cad5ff302e7f56e39ee294149276baf03ab9f5607f5c2fd27253a558d712ecd4a63fdf3b773da51e7856c922cb2e8cc7d9746900e" },
                { "vi", "729d8dcc6bfedbe26bfcb5cc478f18ecd51f46dfb823e5ec6bd2489cfc7c4f957fc95152b66e0301fabffedd81e57e03886b35b1d9912f787fe6983e79ed16b8" },
                { "xh", "c47489b692fb70989176537736aa430595c86595c5e9fc212a2303a94bedd327fa9321e6edac747fa305e1d5f140579fea3eeba6838e547655396302260c2d5e" },
                { "zh-CN", "602709bd2716b04c44b09ce81487be87be1c19c64b7677c61745fe8b2201e29ff26cc27706582ab58ece92225293c1453884fe6bff8fff675ba6b6dcaba4f823" },
                { "zh-TW", "e7cb24791b09858552f7258781799aae4a46242fb20557e5ac17ce9fc01dbf33e93b17e24eb02f7c0ad7f1bb73bb33048f274fe8a200ee4bc0b9bd98d8a2e1f7" }
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
