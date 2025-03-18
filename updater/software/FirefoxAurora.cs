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
        private const string currentVersion = "137.0b7";


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
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/137.0b7/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "0e3f749de7fd7b0bfe9c5abd1613b17aa094a3ee51062be7614c2411f78b67289ef6acd0db13d38a0e7022d1367f006334ee9226bd80cc8dbb1ce95a3c257148" },
                { "af", "938d41112e69e5efffb9bb9af4dd33f4600385a0c2ecb3cf70d289c6cda3a4d84a697dfaf114f453ebdf8bf844599c4f60f0f8b0ecc73b217ca9461e11fdf2a4" },
                { "an", "97687af7c1c8bcb5144fb87234f7ddf640471de36f9e73b667d1720c4e7f4ab74fbb24d58a9e02f996549bdbc0bc2b0aa35aa09bc59de6324576269b1bbb090c" },
                { "ar", "b799dfb3357fbf784bb2bb5f67ac5b17da782fbca7f3f119c3181ba6b545caa96112136c91d1fccbe01a5a5c8b68aafe85585b47d8f77ecbe4cb051ab634285c" },
                { "ast", "8bee87f7968c60d1895959c8e3231e21592d57fbdc3322f0b523b81747899f6228a660daff0064f99b61689f3e5f7db5d6eb07efc6248554885341caa150ceb4" },
                { "az", "93ceb8627e30c3e1c4db31d77c7496aa2856f1d76ed68e08e52d7f68ff316689ad0cb4c5d274a8121cbeaee1f1af08b1fdd3c3ed3964f1d9402b9a1764040f51" },
                { "be", "f8096a1c0fea1d78886f5f44feaa79ed24c16c290ab0f16556c90a82042ebc49f83831fbf5044306bff34cde1244c3e6f05266ef4a514b8df92523f7bbe47dd2" },
                { "bg", "ffe2c51a7f5f835bd1bb1dacebdb57ae6815773a41c4ce3a989adea6592efb492a609e676b1168372626d913754e014a37e9d5a9d5d6a99c89ae903716566d0e" },
                { "bn", "8ffa85e78ce9ad0a9f220000744320a32da0efcd9435cd30e055d64eb4b46798861ecda294f24207180009409f0cc72b6c53e29398f3836939a545c0939d77b6" },
                { "br", "e0546ead0a1fe0455d5aada30fd21e6cd3a9da824036262ec9140c4d017331c534c83eaf27ad15eac40f6d999f7df873de57a6069fd574cbf90225cc929b656b" },
                { "bs", "1540c23d08e30c6f1082c8b0f05fffcacd0f542d4c751cd06a6ae2441b00bdc39a0a568f9b3eeb8b1f1fc33b2b9d77d5a026e2fd97340300b56b50a7333d49fb" },
                { "ca", "c532082c69581054464e2a5d55efe0dbc8afeebccdb0aecce808d4a48bbb70eaee9242e7a86a48db634e66f1df60f7e71151483ec88a08fb0befaf0f024f0cbd" },
                { "cak", "c496f96fdf8db518b60f3b41de65e7885f8078eacd35210e3a9bbce892fe2a06697e92f2ceda08735b3305cb1d9004f9c82fef6fa3760bbb447e15c1014ea02a" },
                { "cs", "8834ad43f4b97432f0d17500adef823a86c516325807d7e7c48f185e83d5c1415ceed75e6b2086f6db4735e4aa900b208e234965525fbd373417ddc2edca7bee" },
                { "cy", "79a7879f80b0b874069bdf788e2024282279b0c0a6f27538adbc1c82b430ce6f39f8be7435bd3abc93c989f11f51be14a7d03155d6a0141c1c7ba871637b99ef" },
                { "da", "465cd515403c46d01fc8d4647efc68ec11bc741cfaa6c9dc900dd41d00c581fe69218314de8a100b50fcb4391351520c26a7c9b18314116050819cf8f1f271b9" },
                { "de", "32f538bca6e593053dcc936216a523be54143a424c91640bf3d1b1504a42e7cdb6203d8409c39cf4d2b3ffdd064b01c990950890b4c612652d3e77fab8ca0fd7" },
                { "dsb", "bee55998252c9ad3fc05d408bb6cacc22644080833920bc24e2863ffd87ca40e54821770ab69f239918882ebd55ef45f571aca2253d68ad008a2ce18aafc8d30" },
                { "el", "2deaf732d44877b3970f0b8d5e08273466e9aa4a27d199a7471e165660c5712651d971e6edbae17f19a4017065cbd78a4a0889fef6f2ccf3509e46956bab5c1b" },
                { "en-CA", "b46c79c60b2e62d173ca9d47e9f2f57045f101fa0a89e793c2a10bacd68a6b60b60cf7f4064fa27c7bcce5309ae406e798b0c52583b068ebaf701d0eaf092262" },
                { "en-GB", "706486043ba433628e421a19f053224c8e6be4e0a9a7f425a31ad065bdfb605c56d018438f60074da81942091bbb2d745e99af046708b4d27603e8bdf54f0b4d" },
                { "en-US", "704949b8e71deefe21b3c1d70320e5d8dc2581fb29e8a914cf357a76e86764eee4df900e38aeebed7ab6ef57590c6920fe18e20badb3b5d99cc720b4e9027dd8" },
                { "eo", "bfacc1a6f637c7b22e4112771f59fa4f948a526a535a03b55f8b49491d821cef3f15be93336e4a1e2dbf923fcae7570afe6e026402ce17c01e0167deb794540c" },
                { "es-AR", "88c7f7da0ae0bbc7e5e5d3ad34557bc597f3c1e6caa5562409b2ab7b2365226a8e4ccdcf0dd7a1bf0ab6eb7a1bd6ef725a256cd78d4f4d6bdb73f6779346da4e" },
                { "es-CL", "506f7a289fef17bfb66231eea7f3ae038495a553951e97382bade7be83faeaa7b13c3dc3f912a3b68ed94a23e7462d58196e84624cad5b195bbdec3b364cf971" },
                { "es-ES", "242c576e4c574279cdf535f31e1bdaefb6a29307e74d9ec0c017d9d00558c1e575d24a922681d573739ae8a8e38dfd2c98577413d4372817c4d34c381c6857fe" },
                { "es-MX", "41bea80074212d30517a55807924b1343bc9959d684c486f27e30fbcfebf9e679dfe8bc184c3a4c2d76dae187304b72219829b8cac462672a5220c6d21e308be" },
                { "et", "ef9f0bb63a64cbc940f8675334f617daa51d15e0a1876f053d29630c4cf05f852523c2b3af740fd4741dfe7cc90efd610db0cb841be63ed4a214b0ebbb49ac95" },
                { "eu", "a55be348c65e1ba6bd723ed4071ae9160118101f825869c659d1305dca1f412fe817518c11e3c30ef3f6cf265a1b8f38a3f07a0aebec969c677b5c0909c8b3cf" },
                { "fa", "12b87d69335cf2ca4c6751bc46fdbb259ecccf459c2078663f52be23911d9baef49a4144a7a8d0944da3e7bc02ec1615d736c70bc7d5b1091ef08363b55b6e36" },
                { "ff", "3ba14c3f506a40c3c9422601ff9504c57e8b10049f427fca55703dba705ac33663ac95edd9f9b84d30811b737576f27457c82a11fc990c64d9f5ccd18a4ea654" },
                { "fi", "110ff433b06be9a7d3c826bea712bdc06cc4ae2283007a0c0366f98525244643a785da6c6875a959dae72e6165fcc8f92a9ed16c43dbdec67e0940ecdf72ddaa" },
                { "fr", "9f25282d1a762132d3e3775a998c20b19968633a0d001c84dc20e33c9e53d5754acb643b4391564ea0f420fb76e55fb66d1f719f54cfe325bed5c84f44fbbde5" },
                { "fur", "37e1b2cac5682407a5160d72fc35fe0fbb6cb024923c7ed6c32f1b4282c2c811b473984decb9b3e0e0c86e89638e62a6fd557df51d72f8b45a905f5caf3d7ed0" },
                { "fy-NL", "fcfd2f768cc7f85c49b263bf3f006dd59373597e17de721105bb0eafb01be90f231e654f37b92caafc1b3d0045ea82c622698570538491d603c3ab00e51b3556" },
                { "ga-IE", "326439d48c61646eaeb9a436b4315d5f0b71642589dd7189feddcb5c54c6f6a47b155f258e1770b6d7c8ef20b7817438b4226e844255f47136a2154ff66d2136" },
                { "gd", "3dcf0b90b622bd176d8eaa79d39ec776ca22938a31a386c702c02bfd4f02702d658124486ad920afcf3c51b0393c7e45e53df371dfbb8024025780c8233c9385" },
                { "gl", "24ca5d18f8b2f6ee04b94d9e4604612731fcaf61010cda3da7be1a518ed651576637c7fd7956e6e5fd7d85276343520b0b41fc1f5fa3ccfed96d3c870cfd6db9" },
                { "gn", "2d1ba8f6671e4b93bf7c131e21f6f5074e746c79808deca595cf17db74d48a289fe05a58a099f20d266ca6e136734090ce9cefe8705e272cf3f4eb8957c11824" },
                { "gu-IN", "afe5951b075bf7f4ccdc172d8ca92c615281bfb7069aeb7453b4f2149d55c2546a0fd47cad5d8230edef855a2a4bc08d2aa7b298f91bd19b979a5e758b8b1714" },
                { "he", "64a309ad1328ef798a6954133352e02375fd70b31b464c862388c717ee1b1dcbdc0ac7e89003bc35d0233f9e04c4fdd6d64bfec778a6f98c98e5224496150ea4" },
                { "hi-IN", "fcb12765840b0873df9481fb2bfb10f8d5c397e3cb22aec96c9a1c3b88a28a89503bb7b33ad1ba8c438b72d260630e6f77ea4861a11ab17c425ef3823c84b6e5" },
                { "hr", "607e9ef2a3d6799c9fa395b331d7f02b35f0022c577721fd2d4adbe8567b481d81a6d6309902759aea249762207b385a91c148c4616ae72e5bc6ead90cf1d860" },
                { "hsb", "624c6ec5fae98d394b62dea2312354997e24f015c6a002ebeb8918c2fce7090d63fda8f51b356688fa630917e7d9ecf1cfdb05ade3ad10fa0f878f532208f050" },
                { "hu", "9ff3d8855f610bc91bd261c8212397e6b56aef0dbe303f99a947b02134f1933a174475647adb3d4a16ab5f05a7e5de3514cd4a40fc8cb7446ef5466ace2d5675" },
                { "hy-AM", "11565d8df085a27ed67fe7d7c1ebcd29a6b14ef2716ed6b3ca4066d08290b3cd0a8a6903fc7c4f49120ee9a95e5ce9764836f765cd6e1cc3e7ac5fd1efb03f97" },
                { "ia", "6658c28d9b1def3938aa4d4fec03493bf78ba247a59bdd16a9bfb3232d828a5780e8179b976960309d8ea9916cb160a538e8a1ba93355a36abc1872ce872189d" },
                { "id", "a77fa2f2f016d52a853409ae633f7c9c290ba25e7e177723d2fb53b81e337dca65985cdbadf03bd80bcbe36778fe857dffc899a4db8be6055b08a340c8e907e1" },
                { "is", "ef00619de8f5816e84d9e5f8254c87041953c1c9e8c26353b29cbe6756a865fe78bc441f38784f02551eace87cb4b806eceac6b148c7258f7161fbaaac6487af" },
                { "it", "535f528a03d798bf7fdbd7effc08addd0be2d63e3c07630d901ab00dcebf5401fe94d7e92d203658a319e2037c57623a4b21fb955fdb08297e8fa58d54035502" },
                { "ja", "4ad8089b15e9d5518216e0d634fb920204be562b287ef61cc813de3596d1e221e71fc592118aa60f5178c2f7f20c3047b636d425332787176b711f9103e21e78" },
                { "ka", "b1f8d65c85d06f50bf8b969869859c6e29af01e760131965fea0730b8c9ac0689780111de86d3fb9c0b04b772b12273a836d93c09d04fe8d79b5afa077ff6e52" },
                { "kab", "e8ee5381b3d0cdcd0b10e382964cb31c20235eebe2e5d1ebb2f07575e86bbb7bc92d8ce5135a381a76f620b96a9bb2210f6f1b509f0ac5b0a01f17823b49b54d" },
                { "kk", "f4119e3b68c70886b576a85d0e795c5dae74e11918c7b6b6c9f49ed994e29e77319f940ceef604821f7ae77bc2bdc8d06d79e737cb30dddf5eb447aa0421d562" },
                { "km", "09b563e93e6a252423200c46019d5b2339cb9a6f78124971541303e16d0aefdd09bcf50201401ff944eab91a2adb1881fed86660c722543e1741936f4656deac" },
                { "kn", "1c3f9c3297993aa9dbfc99135876cee45736fadd19ae4574dd0f945e86a2ade6cc5e64bad198f76382dbda9669881ff1528c148e42f4291906ac669ffe3cf600" },
                { "ko", "beed2412aeb57ec6fd036884df78b9cf504f5435c89903b91225e25b3917bd11e1ce4cd641bf4cab1481754df5f82a830d54486ab6f1b80666aef99c58810cc8" },
                { "lij", "bf9a71139d69722eed48c9abaf5064694bb374aae69f8a3ddad5de5bcbc24bc15574c9401e52894b7aabc8cf16eb1dcedb5257cb6df59cefe5f07d26114938b5" },
                { "lt", "b4a05ca65fbc27ca3af57e7ae5278b4557e814a3274dd6f8f01b55deccb4d073b1e2d68121b07564d65f01a7b20929e36c7b3ef8786b75ed18b9504591b7990d" },
                { "lv", "533a4e1b2768829bbd52441504eba1b13558d947e3ea4a3b7433a15a451a69432b011cebd35674099683f57974a24b117aa8c4b9826cc265cf0f6dbedc13c2fa" },
                { "mk", "1ed44e0f6e1b3f246366df60b12c7fecf9b32a8250bea5cc88dc80f01cc72366523a39394eedbd840e17c00dc12acfa803bd8717fa10c8771a6116f9f005b97e" },
                { "mr", "e63ec8a2ffa25a8f5cdafc64eef1519a57b954d1b0c9955cd29e08a1a559bf2cefda2b5f43f498495b0fc3bf88427473a9febf39a50d4725bfb4e5ea53db7064" },
                { "ms", "8b6b50c13e2f4766e3b8e9a7e7817dbbd2b70a6125076f6c330182e5141ced939e089df16b55166bcfc43c4dc23616a0b686a7661a26c34c17f3fb7c81026821" },
                { "my", "47050b88b5879df397ffa6b08286b00536e9b8a13cfb62d6d6c2483146c8700b9eb6515fb7165c571b3c7f0ab748c6e5fd31e4a68b36b44290ba6894f2a8e472" },
                { "nb-NO", "cfd518d879efd24d50cf314b9f73749d68101cdcff56dfd90ce22478f8065766f92d1e1b1c3db65d460fe430315d462f8635a7b165919893230f9445e22d80df" },
                { "ne-NP", "32f8f4b5b0b82dea3defbfb2d1dd941db0c0fa5937907f60d3df0a3e8aaa14b09410ec174a45304669cbf98325ffb10e9d8a3c85ea8708214e9afc9d8a4801c9" },
                { "nl", "ed0b468ae8cafdb1db58ba5efd741ef347b4231778d03b6d47aca78021f7ace6512f0f873e156b9a9bf2be8c173ff7a28e3ae1b4f074c29bd93b8779f1e18808" },
                { "nn-NO", "d536e2788b1e0ee1c2e3ce1c05e7fdec4919f72c46ddc5e6639b3ea86b4a52eae6c28eaa3321eb50963f57f1dc131b425db1d05663bc8f35a57917020ad89ea5" },
                { "oc", "cb597c8c98820717c8f6713cca1ac51431e09de2b34a616b1537bd146a558bde7cbde64ee17aa8ca461d11a42b8f5d9e58ee0ec216c3a6620ff6c0802724733c" },
                { "pa-IN", "1130ac834f1d0322294005d16709af04d695d0407749e0d591b7b5193e5b1ff8cc63d02a69623fcdb091637a8623c3c19cccf73cb6db5947e0201a460c30b431" },
                { "pl", "cba2250528f58c2a8ebf298ab7459b4d9edc10e757c2bb05da5aafb1df3b987222d8c9a19bd1a16f82817a723b8782ea9b64dcdb8f1f1fc000e90c8d919671d1" },
                { "pt-BR", "ba8c23910dc1bc8dfa6df04c895571ce23d3de00d3529ebdae70545d44573bf054c4fcc8aa125b3190608080968681d5d30dc157e6903aeaf2b8e378b1b8c33b" },
                { "pt-PT", "4e99e16188441131bf4d5b1d433a5886f30bfcfc97835977958dbccee2262518ce178949ad6b61db664bf1c3313e2a58d54c7908de40429b421fcef49982dbf7" },
                { "rm", "a51d075a3ddc2ab161a201624da346ad0c365cd2ac651e786e1f67a7ab34f59b0145a779f690783a2308cc9c73c70cee520791597eef2a9b32fce6dafdd46d4a" },
                { "ro", "a7b385f2753763489d2c9a51af85ba9f32608ed65d59f88c952370342cd2ace57fa33e0e86c4f3598e897dca89169b42337aa9a6e742d5c1fd9bcd68f5e81b12" },
                { "ru", "97ce44edabd9badb47d2ab1b3713504f12150c27e2d6880fcb1e956bdaf32d006dfa61449d3753fbbedb2621839586af5633709cb079553d5962c030af3aa7e7" },
                { "sat", "2c735496a94c418ef143e9f65593d9d9c73d57cf06e49682131f6c4a2d506b19cc1bb2d02616026676bbc67ca2ac5674cd3998852924daa7e6abb0c91807f1f2" },
                { "sc", "99a92d1bc0a6f481644229c3ab0bda005a2dd658dc0cd36f802318802f4c78c8a7a8fe30691add965187f5f46b5ea397a1951c6ab5924edad2d9c96857471981" },
                { "sco", "53e9a87181ef8c57e30563c99d19c83d248f8ba5c883c1644430dd6d9c6c7aac3267ea4270ef16f6d9ca5c393d147048a35db0170184cccc4a106e13bfa9e7fc" },
                { "si", "23320ebc752bef48cacb3d157fa20be92c36bb92b950f04f371fc09a990f71a3006ecec5365d62a31d61c906f7a8cb415556310690d00df56d63be0c69409e06" },
                { "sk", "8c0efdc29adae1966101045f1ab975424bdf18bde07150066ddbf9cf8c34846c1aed4fc14103733f7e44610bdf75d538a10f24cae3f42f55e9410de7f20ca545" },
                { "skr", "adbcfcbe5a5e73d56a246d77699e861d8e323859c5992b29d235d9f4423d261a4241296124aac50e8342e4f81870a96aac08a515fc7af3f51e3d9d10e3115571" },
                { "sl", "0580aa1e6b08ac0ad70574d12b11cdc792d7385d39a859c72476597af6201cb09e3fc737e4835579784e4eb82a22a0807bb4cf669e8575346b9c570b211921c9" },
                { "son", "3098b849c2fe93b6166621d5208d271dd1c03bf89ec73b3ed53f1e037f358cd8c956db6d7263815697ea070969f14c13feea7d17b8d4b68296c30e9a996d555b" },
                { "sq", "3fba13b461e73cc67d287c25a513177d06004a56d14a561400172d3e7f5c3c87b6c871b4dca5bac0ffd9497d35dcd0a39dc936518bb6d776ec059914238ca5ef" },
                { "sr", "498a1f8e21ddc63fd51e589445d40d1d9314aaf777e84d1cfd0f09da9732f70a8223ae90b34429b99e49ca1d7f0353d7029e95347f42333598c23e24dc92040c" },
                { "sv-SE", "346b0c832d982e9a05eb7b1dfc428c562ed2ac36bb254fd348e284d4bd2cebe23a998d91bff385b69f912993b81d18928f393db874bb39603c23192b50845de8" },
                { "szl", "3c133d1f7d2177b472d5c7b146cf7b14726d9f67ce5819424809a301c7337c9e5266170dd9f1595d23ba2b721acf070737267f4ca4acb3b3f6e5e9b4c47a9bb5" },
                { "ta", "953d3f4816d3689a7dc257c6659dc5e7bfb514e119f2aeae652c3a7e1c9fab5fd799745680a902c62473fa5e27f166db6a7b2e627b1ed21394e81b5e7f7909f9" },
                { "te", "e6fa848e7020fe67853d8f11c349df3b45e87e4a1f8c1785fdaf6ddaabe10368e84edf83b79b70224cc1d99a15a06d96889783c16f8b563fc3cc3ea26fdfffbc" },
                { "tg", "b939038399f71ec9fbbf369ce21e76cf1ae5b1bc7207dc2371682e7b5c5d08ca61a4fb25a9de3d8b23b082fc9dc33f0b91feb72a50c0589de842bd651129e4f1" },
                { "th", "819f07afbefea69b9ee7a925c69e419e3e7912c4af35d281e6062a65367fca55f8fa03e1f182b7f74062c45f5648c6549a6f10c163fbb3de6d2cd35cec04f828" },
                { "tl", "4423d3cd9fa0828cb03de60e731b92fd1c146ab65bb74d731be02be75c130aa0f9e3c53561c6e9c6dcaed2fc23a264760706c1e8c96149f16b5c4c2aba66b047" },
                { "tr", "393b7cac5ef5efc5232a9ec85e89e1bb9d14671ae946aca2eea0cd2f96c574c13aac94ea6c4dedd04a2285a3b142b06e3eebf149a22cdb7ed10859122f6c0d09" },
                { "trs", "cb80c0444b823f9ee742374c55dee8fe650822b20532c6e1781f0f2febb5b72a6019cbabc211ebe6a2e6db315fbb273e8ee4d3b220556c4cd4f8cfabddb502cc" },
                { "uk", "bb78cde8c45553c88cafeb609a663037f4846b4d73a3b00481d75fd9cef767148076fa4e61477171fc1af0fd79e875a0acf1506a49ffd450a1d3f1a1a97ca21e" },
                { "ur", "0220849972a44adec2e2cad2bd004345b325c7a4311ea287abd52d99752c700f92be98303babb3850d462d2a0de70d9d56e6425cf86a77c2bb58cca5ccbe817b" },
                { "uz", "a0d3c891b7d4ee14efbae988c051aa3ea3589ca28cd38912de689a8835a6a32972f8e1a10f0ea812f3aaa76ecd1ca589c44733bb91eb8c110b2817dca7f2349f" },
                { "vi", "4974a653ae33aa6254eb65fb707d4ba09adfd9fd88ce9f3946758964f7789e791447ac4dfde323004f5a89950e017e814aa1b2fc6e0894d8354a786f6583d9bf" },
                { "xh", "7afd00bcdc1bdab5eb37f0f8cc53a9b83e4822a14799878502b526f223de6d7bfd6afa5f3285411be60afbb3c5962759d7853908fdfdcacf88d8d56c2c6cd4bc" },
                { "zh-CN", "05bd089a76a7abad20c0ce2c0153d5320314352e08c01c2a4731c16750695c930a64f329f5d674966ce2da83c131b7b0e4ffeac785f32965f3978bd2a298e6d0" },
                { "zh-TW", "99df745001fc7552223442e80d8f077225028cc70f1d85f243af7324883dfa21fa58378da4db9f0c1fddc26cbeef9243d64401281adbc2e2b4cb3168a91ef92c" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/137.0b7/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "61ea218c359869921646ceb33c3b571dcc75eb30dd963de2f470700edbf44a695b0637152857bb4e62753c08a68ed84edbc1a1178271996e72062b1c76f1d640" },
                { "af", "7094405f85da72c1ccf1c5fab12ead9b5265e7a4b69bc701c4684ca63a3b8756f7219bd5eccd4a26309b1517a9b7fa98f183d0a39c56149dc39162a334676a51" },
                { "an", "d5019dbfa66870e1849c0c16d9e1356a06b284b135d8f74fbf5f9b23b81c7bd369b6bccf4663c7d39b78004218b35de1505f486832bdeeef550adbed54375a58" },
                { "ar", "c12fed03213c2832564ba8b553bb21d6744de685eb0a58b9a6cef43049a579549090e3f4f2611f35e92b136d139bd1c4b8e05723fff71794749c2a74ca6e10e8" },
                { "ast", "d3cfa5645a31fa410eb5191993e48fb9ec4faf11ee7055d56d750abf73ca7a17ba6a29f1adbcde65731b65832bf662bdde56719c1507053ee47bf5675c175ab1" },
                { "az", "1fbf70f18cd29cf4cb83d41dd4003e0a4bca98659a3d5d855195edec81c1d992e190d0eab58eda96b4fd45ef2fa4df1cded50cb6c0e3bba121c6965854608374" },
                { "be", "b7654b6158aae7552d0870439f7b1dc42194ff86ff3393fa3041156a52e435153344c796773a21b2026c6a8777fc308c908a20d09eca98fd881a95a38cf384d6" },
                { "bg", "dfbd2e7069bad8113b0fa2eebb8742ddf47591c2614080b993211788cd5fbdbefb5be3d1c426dd42863571df77b72a5ec1117f335ab44f697fc0651866f5e6a4" },
                { "bn", "dfcf0928d25eb7ed51e0adf38c20a4d2c877a9954623bd79ac360b246dc26dc2b4ecd384b1307163d377a82b271118ef6ec868194066d276331f894753f1445e" },
                { "br", "d86a23e03b0fd64e879241315f3af3631e030e77b265caebec3a079d372ea298d5f57c666b2cf3cfd7ec3546eec94ed9fab2774e0fdf26f82f06570e12f42062" },
                { "bs", "5d214f9a31d3e114d91eb4b2c044931e33e1c84ba4a6ae5b829786ec0599b56566bcb571b3090060d4c94dcb674c54aad7e058c646345a622cb509538064977d" },
                { "ca", "989653191588f28eb514a4a235df8675f31e5779244db9c97ccae1a60480c3019bb89d730d8dc11b6f47263d9303ec6d138bca8c72dc0f444e370b2169e2fcd4" },
                { "cak", "4ff8613b98a65cd8a378cb226bac17caa7f573e8c665842b231973042fe537196e679dedb026fcafeacc9263dbbe877c83bc748fe48564138ef7fc4bf72d62aa" },
                { "cs", "57675d715946b11db2d8339d887dde59ad4c2d3c9943e2fe9cc777289346dc80679bc9c58e08cef13f985a8166a6daf2681ded01f43596f6845ab52a12f1fb16" },
                { "cy", "bf7de79606a67ed6f3a650e84c423ae4fedc02569d7712475771a8b44bd4d8a7036dcab512e241132c85f299f23787bf90233a852c24e7d6dd6a614842e91193" },
                { "da", "68d3cad15a4d2e0706d402eae30d5a8a7359b68136142b5bafd3c3254001714f966eaae82b7aad859f4e6137b1f134278f81ac0d8e5344cfacb1e196c7ba2a9b" },
                { "de", "0248814e61622020c984a47367cae9daa7dbc9ab427867b00ef773f78645dd92a697bf95daed8a6197febe231bd7ddcaa3bc3dc6335ec1605b69d581ba6f3bc9" },
                { "dsb", "e4c8a7e7877949b2f168101d672c3c277e47a367e099e8004f27b92de7c77ff61a88310fd277db0170094f2f0674dd79f4b0a981ef4d2512a16c78001a73292f" },
                { "el", "65a7c1cb69358926a56aabc62c9d4c2d5e2cf1d55852e4ab567e4f06c7ca943d7fd0dccdd215ef478709e8ada6f13263f5971d6f0ac65ade8300bbe6149444b1" },
                { "en-CA", "8c7ae3f8e6af0f209d984d9c821def3a37271dc0828e691073851b587830e4ab15294ee467ea8f0dfb283732293d124be49884f8ff97e6d2ae27d2ffa88f1678" },
                { "en-GB", "bfa7de3e2eba17c259a2b15a76a8aa6cb52f1cb6afa99c91ad7e5017f7529d03c0a01f656a37e6235f5fe64cd588d759d7416b2308a039bfb59a26c8686dbfe2" },
                { "en-US", "3b2115f4bf0c45e3c4577afd64ef720ba6e19ebfd917b7b40700089fc2dc50b720fd288b45ad1da152c1adbdbade3d0d073132f5a4c509c46c48749d92ee4945" },
                { "eo", "eb2e885f6e3ccae7c91333f39c10392fc3bdf876fc3f91f6b980a030f94f9082e84d005f4d9f23b3d82cd638cd20b7cdd483e752767a9b4d0942f74a7e0e63bc" },
                { "es-AR", "ce3f1c1a7fc0620674f1d14e57aedf36dd35fe7c3887fec8ce4bb6157f50ff23e0cb2a8119e173caf7ccbce5a34b14367d0e4d6d93d81f6baa9889dfd4e5adae" },
                { "es-CL", "1146888f2506c60d452608c1c5eeba5de0f2644794dd56813a53a6e4f8320f4657a6196250cb2e7fda63646eb326b67809ba55c527f60a2b78bfc4fac70491ed" },
                { "es-ES", "6e1ac83b8836bf47543023bf6944fefb2f443de206a5f7d8ebcbed238fd396531b2646195734cd5db61a4c9ad44bf9ff2725468d538e0f95a3ed6634722b5dc8" },
                { "es-MX", "36db55cab46c2e235190dfc2ea07ae9ed3b827c4033ae974b501d45484843f2dbd4964bcafcef4cf5892a4f2cdfe5f2147b6089c69f66a518f850f86f8f9fc4a" },
                { "et", "79439813c39e29b5ca357b1f6c92faf14c2507bb58e716279d89fa76a56f3aa6e921cf152d9d4b46106ba1be345429d6c411f86b2284753763eb6703ce920ae5" },
                { "eu", "de133163df590340f044e82f8e7c1956bc2ef82fa3ec629f8ed7c5346b323174ccb2aac3f06cfeaeb66ef7f60c51e6c55095d1b0f1813848861815f04fc90ba8" },
                { "fa", "b0c00a7563d28f8b40b2bddf6ed19a2bac9851d9c8f94099a14c42e823292e35f7719632bb00adf7c357ec9aae35abb4211f3fc2693dadde24ea1be5175fee57" },
                { "ff", "7a3622ae3930682bf6c9b74781777acad82a359ccde8772b6a676bcb958bf8d7b80e54051b047bbb98deb3c2c00592b2d58b5a5fc7827ce45d03e6fa1959e2aa" },
                { "fi", "d91f9148b5c3a92a197d182fce80d163daed7089f2829c9dfcba4f7f22c8153b7780f241a13df770a8701875a6999ca1d57c64d3b6ab4ee45c113987859f5d96" },
                { "fr", "8126837a074c0c179e6fd8fe612a5bc650dc01fc5b355e76092ff104d43829ebcec8185629939e0a7a965a990b98ec00a2327c26d414f24414d32606a2f3ce67" },
                { "fur", "6a723a3692519b4c3f4c195d48c04ed3e50cd714e1ff8b95adffefe697a43994be088ab1877aa461db3e6f5cf09f3e6ff9799e7d5e61656de875f13d2978121f" },
                { "fy-NL", "95229cdcb36def0e1119172ad99945c2495cf91660996fc789173aaf340b3ffe371baed605935ede1aea64af317bad34e5d3666d750cf8da0c9ea3463897e5d1" },
                { "ga-IE", "1a9f869d277291f9d775c68d351fba1dfa4a2912f0eac316ff0f949d05df17aae4fc080ed64a2b9c4309d08609d5ab22fba6e284fedaff8394ac4e9617d8211f" },
                { "gd", "ee1d540a4b498f776d520d3d63f34de96b9b6fcacb72468057a566e4d2ca9ad8b7ee5931c03a3c8f91bdc744b4e94105003f87e06c233ddc08d5c175cc5a136d" },
                { "gl", "1ef7f40e383edacd303f2849c23286c13508cc7af1ee75884af9ba00847e267cb08a10aeecb781251dc79a8c7f147506d99eb334cfc5f6c5654475b5a4a81ab2" },
                { "gn", "d21a9f146979250f94d2214fb1c16d7ce13cc5ca46e6e1b165616657783afbc84a3b655a1d2cca46ce0eacb8fb569e1d63dd93a363d32ba33c81cafef8b45206" },
                { "gu-IN", "953ef5d27480c7c453d854c50390d1f1b55f24aa9fe20bed3cd0d81812836f85508f1a75a440a997800cad9e457d133d62dfb18a7442eba5847190c4707ac726" },
                { "he", "cfdbfc16faa86642ea64eab098b37f593127792a9af949e4938a012714fe24647bea236a281e9a53120cfe0b68cfa2f312be314a03200d59b8b4cac9512a7277" },
                { "hi-IN", "ab59dc6528586289e7e4844e2b64d90bfebb3cf0ca705edeec694d4b9d0febacafa6397514e3e4331f2928eee6f3da07d5b0c27ccee7d4dd651f1e400c00f66c" },
                { "hr", "4dabc500646353ffb2f3129e785f7a272c34ddb8c59ba63e38bac547595ab00d9f0a5011202d9d3cfeaef4ca822910b4110f7a46fbc8bf35dea34c64fdd01eae" },
                { "hsb", "eecfcecaea8be205085776a11fa20b6e4403c32aa0b6e23f14f41cf34aa09e8661102d6a13d967d82365f2744cce7cc42eeab852453fdb6ca1e22d14fae87e6b" },
                { "hu", "45b01346aeb027e783cf55bbfa4f484eba9d571e0e0f2d8ad91a0e0fbd09ef904861f8c66be46164c84d74707b98053216d2beeeead49253d2c69e68d5b2c081" },
                { "hy-AM", "855263af0193568d74522a0ee672f082e4472bd739ea38abffe50d0016aec26a9ef4482ff7fb2c48c173e106df581dd9835b83c8c584370fb2b3cef363e48738" },
                { "ia", "8756affd32d5571a3ef108493a3d3eea806204e19cc2209ced12f5759a10cab080e6dc2ab63eedad01aca169de9aa94d32f05e31698d6ec971e983af91f3e22b" },
                { "id", "d1935f60fddbfc3ad5278cc760dc6f4435f5b8570de3f19e725d0b500564a0b87206c79fad9b0c7352e4fdd7304171d32a2f5a5003df1a75ec547be18c1c7481" },
                { "is", "ed09be723ad1d3b70fd9720fc7e65ac288975fb2e23e99478e2044c4bcac9cd7708aaf5aed95116d1da78488d3ed3018e3c877a96028d5050d05df99a1e3cad0" },
                { "it", "2ebb8f38679bbb9d234ef241b239e8431c17e900ce5fcdffc52beec25157ed657faaa4a5b4b498b9886273ec361909d4548235d9d857253808419fe8c7b09122" },
                { "ja", "09a8b4b73eb3aaf160743506b3dfef235be6a003917b6ebd7a700580f0b094f58f807c4b450909e612393b1b2bda1bd4c79988d2e1b7b7b2bba1ac24b6d950e3" },
                { "ka", "c9a374bf436a3667286271300483ef0befa3b8e0aa03651c84a50de2f5b78cb9638a1b9ddc4b53a464f21a7652197798ff3e29cbea8e05a809eb1dd0f5a6c40a" },
                { "kab", "d5ea7fd264eace003d87c25c687acc162f9cf80df3cf74a718b0a8850a2c718b90b4f0e4026beb0fd1572451a58ad4831aad4238702ad12f232f70b416838cc9" },
                { "kk", "d1619c7c5468b1d031fd2826e2a914af0c1b734e3512b4b37933daae16b0e0ee556828135df158e9bb631f93f2d125de4343f1c1f886b3a330e9e3b4a552f8ba" },
                { "km", "74c7d6ce76c686047dd2233c530a182f5a80c3c13b6a8c027f8669cf3934f9aace57a35537beb4e7d35914bce681977165d21a4dab6179df39d5cc9311903c35" },
                { "kn", "6b324809e4dc54d32e4d83e3ca3f8c1fb422d36487fd00549db98da72cc5194a5a4d54635f760dd0fd9d06c4f3ece57349d46917069f46dc6422a034eb6507f5" },
                { "ko", "e9f91511052aa838e57530316321952a1be8bb824d640406781625da5bcca76659e9ca1153c752d0da92248de02a30fe7af501caff7b10bae6c1fd3f281141d4" },
                { "lij", "e731adb58e332a7c2199a8dda06ea315704d7ecd8f6a11f5976e60540119d284e153ddb7824c03edf14337b396f2f006c9cbf676d18492d3e4014c58bba14b0d" },
                { "lt", "95d5f5c41da38c6d362729582a7ca0dfa1ed9f4b9330dec5216e9a86a97e2b2e1b98a9e45531ec14ba0fb3a5997b98ec5d4fefb61972651feab9ae4e9166083d" },
                { "lv", "5e50cb761b0c29f85133f839d95c69d191d5a1089472fbc401e090cc52cb4157477461832d08d2b51f550b53eb15df0a197f669bb3a38b882738e8595082b366" },
                { "mk", "a6ec5be4e973c1e23ed5603a49f5de8b8416b49dd021622035fd6b88e5d3759817cea2e61d81e357347f69c15d3ef22f975b6b3f3c31f13d89741797e9de108d" },
                { "mr", "25035f331efd227375ecdd3b1f3ea16fa0421ab0a5ea9a6370a64a0479fd50b27f896abaa8766339d8cd25c00f8be348c58481f9680b72f591d7d2a3802085ef" },
                { "ms", "c2437cdb5b7b5d56c3c2f364bf4aa5dd25b6da3c6e87e07439efd26b7e13beff3747775cce9c9e8804d8695c29ed8e9a3ddcc1f6c10bf28fb71a23d22a0864aa" },
                { "my", "1e277297061377d7c7b730eb14fd5f74749d1b20bcc02376d08f29499a7142e256de7e028238292e11cf774a3ebae1d91d0e7973a57517556b89314d893b6624" },
                { "nb-NO", "9ae0c318d34e73d9dd5843faa7874c39bbb46e2a4316b3335e4bbad84d6b6a2d76c8811bd103fc6b823e6671dc4a73085a322ef9d212ff2289c3b3abe1b11b38" },
                { "ne-NP", "c5ca04083217b4958475543d1de233a70235498d12d709831a93b7252345d3807e83d8885aeb9ccd7f1883c33245dc0d6fc788d83ee223e6dd95ba4a71f26436" },
                { "nl", "750d51e0c86682f87ff72f5cdc8c0268346dbdba7ed51f853321557075ba03ee88c19c66a0bc38b8bafef199263c2e44225ecbd07b27bce7f35df8df24e60f2f" },
                { "nn-NO", "2a3c839917baf01976e81e82f5d86a1f268fb9364f2bdc5ecaf76442d16a8526d6a3143587aa2d6a946a6dd5a046717b1cc2d9ec9bd19e1092d178e46493622c" },
                { "oc", "113f262a5280d14e4b34debdcf59f231ac8df2aa02e93a921ab3a9497200cc1a889ec8b11f94e6bed170be7de8793d763888027b875e6357b613e6ce70abf816" },
                { "pa-IN", "e9a4998415f538ef729487655492b091ee1ba71e3b4679cfcb0d75fce4c549e0dd6044557182007e2c5280bb7b4c89c326f07198276125b01393dc8256342305" },
                { "pl", "6502853039c1d99e0bf05836c16022525dbc0e49a69b06e3598a9efcbd63e2256018ce88ea302958b0dc375f294f6cb005cd627285a6c224170c6bd4722f3387" },
                { "pt-BR", "64166de0c9f7c6b580cc5ec98a44d336c3646ebab034e0b09a55b2a813566c520a40f5a29a7fe0ebdd831596d27d852937a5e3d8e666d9d02660f2ef80e5d2de" },
                { "pt-PT", "2b113b3dde34f8bc1dfadb2698199c592f37d5df9782ac3d9c6725c89f41836e753323216a53622aa6a018a2552e98c02c5249afbe018e26ff809d5b58ff373a" },
                { "rm", "b5555609af4d4c1c9da0d86d82e9c66dffd761b94f5d8e20517deca5152a5779ff32ed32b35c9ac77cb999ec91ed6bfc8c864ac189515d1be7365d7104ce5c50" },
                { "ro", "4c20d3719e3a62dfd24aefc0c5b3054df2c8d602cf13e3463486f46104a0ee64eceb5c9d514bf0b61185cdb673f8739487249818095af801a359964380461ba6" },
                { "ru", "bde7a9e4ad1e11e456b3b09611056f13e14596d1266d41ef6859a93702a9936dd95a66ac0d8be0ab68c6e5242c502b85d319f4442c406af41036a4b830647801" },
                { "sat", "957076f5450c6a1ea3a16128234c7859f07b08723e884c25ae67aeec6f53c999b1dde727ffee6537c24ccc33444ecbebd1fdcdc0863c63d7b848f6a07bb56893" },
                { "sc", "692c5042f125db3fb49517e8d1620e7f7cfb6d5bf4a74c331e9a7a66c49b88f448e58b7bd62e6d0c821741de6ef4498d05b1cbf406a47628b79a68989fdf03d6" },
                { "sco", "d5dd05513895da889b89e7d6f2fc113fefdf58d3308057b739f9b23ada2f88d6be8f9ce8c0c2d7087c25fdcdb02c9f39c33b47278351f40c78d646822c444cba" },
                { "si", "912d95ea63918a0dd207d217272243f03d9fd915aef333541320187122d21f2268c4c4575bb447404c2662a436ff28a52857e81e12727d61da2aa1ac11403aca" },
                { "sk", "bd324b15a5262763fe5b23d1e48e48c286b9a10b3a681ddbba490ac09e86f4448ceb693d4342855929b4ec72e4d61ecca4fbf6ea290e16bb7e528582b6150c77" },
                { "skr", "2883c2c273aa4634e089a349460323f9d6cb913819a040ccb5849146e381327bd343f199b3eb865479a49665dbbb105394577e40d0f540bc9af0f7dda1c5a6a2" },
                { "sl", "1d3ae0a2d1d1505dcfd8ef35b7084ba63bdfcd6912598e4ed79a1b379081155a1ad365426a11563b7343d0e84803a7de46a16824147e6262276f7a268f26d534" },
                { "son", "d1c91361551b9999f1aa9d304c55bdfeecc3d192209c6b31d609f832190012f1b3ee5a56c2fe0ce683caa67d6ecff9ae7fe1b81d85923e5078bc627feb87111d" },
                { "sq", "f1fb50989bf487ec5221f0a97dfd0adad67b7a74567eacab5a333b11aefb90cf2625e94c515ac0907490ce06492836015d38cef3581d867a361aabc6bb7394a5" },
                { "sr", "0b071d9932cb77c155d32b08b70bdfc370fea19ae278c2793c59681a0cf568a27ee814d98d5a2fb3cef637f4b3a431bed9d5de95e3177dc593193417638f7543" },
                { "sv-SE", "10c9480dbd997e934868ec543a6784bd19395e14e5fa2f9a65e4ab7b6fee1da5814fa12c2a0a803733f867e6e6d87cf1959728ab1d5e22c1ce24d315286952d7" },
                { "szl", "12f95e50146e47a9abcb3a4b41cedf792ff25b8246aad0d979c81803a8a6851e59bde3e10043a7296346caa55addd8dfae2f0a5eda1d9e8adf597d5163f98b27" },
                { "ta", "25d7f812f48bce44f6b68833dfa501ba183c917612d7d31489ad296c7dbd2afef460009db16e74360963af87cc523a68aa3a0cc4e223b06a76a56483805979b6" },
                { "te", "d1ab758dff0c9be25c3fe51f09fda26ebd3a16fd79c32401c9d6874f48b523a02cbeef39b3d318bb7b1b70d173aff5b60f9dd937ddb029f8faf3c771a20d3293" },
                { "tg", "6f8db68e6d033887db1ee9f013d706083e283bf2465548e12913c1f5ea2d40ef33321e7a1850df0ab1394139a67f72029013328bf15d39040e8dbd55dda5c23e" },
                { "th", "4346e69c72ae8c9861d0e266fe3ebfb2d1aaaeea81b4bd82b2bb4aceb3f02bee197874495600718f6be56f66861297f827855423a130f8d415d0ef46fb1d61a4" },
                { "tl", "bfca7bb4ff21e94c7dc3f5a778857432b26bf04870afc4340a5616ec8c80602a85cda123841c4ab15f34193f722fc4a913751b075c00d0c7ebba14f4d7237435" },
                { "tr", "ac2f1987784f1817fc9e84d4875a5059d26099930c4162a05fafa6ea3dad6a1bfcf5f50abca9a93e437186e7d02dd93438bea59dada335c0703f140baff2b9c0" },
                { "trs", "af78173ac1dee2e42348ef00851738296ab3d7e6267e40b78fc93128c857f4cac72c186fed50b04261c98e297dedb4dfbe0ca03f3a10b996debf571401fbe0d9" },
                { "uk", "209d3b320f70cd13bd1380ce998f734d6f3935454ea1347782002318e10778b92d8f1095689ca0ec848bf8523facaade8fcd1e862ed77bdcdcf7b57eab676aee" },
                { "ur", "43c2bd570ca088479e9340bfa45a0d2839d2e5473b8cba95e91fcdf2ceb1e02ce3154c8b5b95291df36218169d9c48ca79e635b548482d19cc8af0d50fad9353" },
                { "uz", "53394423057444f47cefef4166b3108a024a1f0939408d830cfd7a26d48495b313925998ef2d089a410e9cf6734b2b91e88fc430ab49d2b915374b9371554680" },
                { "vi", "d36646ed07ee2a9c8e680933ba1d1bbc8094454efd3097789f6c2b8ccfb65f0e6d242f03017088cddeb3ba631eb1d23958b7dd8d2cb41682e03ef1ba6de4abab" },
                { "xh", "2d438bdc6618e9bf0e7740d674fb0850c6f13e2e16b6633916d112825ed186f78b087d0283469a6713d8bcbf3544daa80b29a34da48ddccb48ce487dc0495ff7" },
                { "zh-CN", "eb5540b221b71f715e4869e6db1e2e376887785137f0f211bdf8cdef4186e4996264bd86b75dc2f997c90fcf32506810eb7e2666f142ae5b6d6838b4d248479a" },
                { "zh-TW", "589b18d65e2a0e296c32ebf7567311a04448f75e4e7dbf86b886ebc1ed6b49780d6a2825bf1cfaed15d3bcaf2a77c07f8eb4774c8ac4f53590a96878e68522dd" }
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
