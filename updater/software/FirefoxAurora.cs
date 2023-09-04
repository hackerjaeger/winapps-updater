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
        private const string currentVersion = "118.0b4";

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
            // https://ftp.mozilla.org/pub/devedition/releases/118.0b4/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "ecf24620df71b229dbdede7b8020ef292a8af099e89f7ac1d27dd104dfea61c5aa47b278578cac49f05a07d907e666d0fb0dc6b35f9c8d6d3aee4dbcccc07b04" },
                { "af", "cd1033996d0b124cc877a2bb7e4978351c5ca5c670f3227be57461fec094f3cf20fbadf7f4e6d070f48dd48c2ac374a9dc09bc1f5743d6e4a940e83c38efbf47" },
                { "an", "8e9f47668cd51b916e95b27cce524ab122bb21346bb620860d4f2fe6137b157ba6ab7ed52517730fc1b08c73288567c757e1bf6b6446ae97936dec68f2a9b4f0" },
                { "ar", "44a0e007a7c0db07f6583a232c95ab7e6671964463bc73313346b16e706e70bebc61820e69c037b1b32beef3552617d8db2404745bf6f5168d87694d602c57f6" },
                { "ast", "74a58f94feb6cd25bc5e1f0e983db97e7eea3963a61920debe0b30416c3b576c614b5ed1d9e15a23900b29551991fa8e84fa8a0244aefd656816a107a4b5976f" },
                { "az", "2245973923f473cd99ff4772fa580d906f436eb384576668eef7c82f35956e2021cfae4ef6cba0f38a6942b1e6e715db0ba15ee578d9b1bdb2a297c9ba03e2cd" },
                { "be", "024259dfbcb9850787e5d61889e2c48985a588ea7161ab9dad9b60b29260b9d32b3f993d52600ac639d7f45f319a10a7d9d6699a077c15816ec238a9db71364f" },
                { "bg", "8f506957be691c4b3563fbf0c34cbec7cb7b1b539ace4bbdfe9b6916291b694b62dbd1ea002599d7432f1410f9831e484c6582da69d12abb6f03148fd587465b" },
                { "bn", "c4e0e8c91524b7f3cf79d6b23becc0be453ae68619cfc09c398093e29406e1217f8f10fe0066b3a502f32ce9e190a582bd27b4e3c78089dde8577622577c2ca5" },
                { "br", "f73638aa3d1c1a9f417d877927c22f303f6fb704fdcd448c80ac6c834ba38e1770b1cf22a7c578d4ab21049da06b960c666ddc338ae6b48b17076f225d30ef17" },
                { "bs", "b02ccc872f8de28dd6d11f76ef722fa8e0a9edbfdddb19cc5da936645c7462447b0f5803e0bb7aa00a8516cd3278a606d6d7f2f380b823c27e66a350ace24345" },
                { "ca", "b0fd0cdd07b673e94487d5e19a9936009d4dce7d7d9314aad75846afcfcd6fa65a1e7f6ab7a7be006534c75dbd10cdc0db97e47246ee7c40eb65aca6a3255cad" },
                { "cak", "15d0a1d5adbc29fc0442cc5bf7e06353286b32afb854aa21d9fc3153cf83d3969885a54990612cc0a6fb2993d5a9a69e785fcaa65f38479fa63f60e3365f399c" },
                { "cs", "d8460b69d12ad25550da4d7c96caea6d78b7b69b4abf612c6f5f4f1c0226f296670a7078da68e854269f1b29c646b5bc9562b1bf0bc0cc36f4a1f2bb030dd357" },
                { "cy", "6b6e731ac30aaac2bc804e7bed11ff4b879bde5acd63829c1aae964d92f2628df8b2d7476e6297aafb9f71cc3ae735df7835b8cbc9f5883807969d07ab041ee6" },
                { "da", "126c112c74b669a77939732228e81da66240b6d6ca547a7b650f9fae298a73aa0f5eff4a64a4e680085fbd0907cb444d58ee5364e8fedebb5d2dc7dc31940792" },
                { "de", "ba6e07cc93cf2420c17a5fa2f598c3c9945cd1e9f13d712a4a79b3beb246ffc0c5065297cafef1b73ed2a78514755bbb2bc726483730b0b8e5ad442d817e6bf7" },
                { "dsb", "4499f09e8bd14ae889828e3e705c188d46f505d4ef0686f3fa7bb155a280ae5b4496ae13c8f16d479410f40dc0dd9d2dcd149aaf72a1b6326d98809667749f0f" },
                { "el", "08e87f189596704b3dcf26ac14a7b092afb42075082da2b52560d657fef96366ff405bd9b13bea1134b139dc7500ef030c7981605e5cae46b777b005b0030285" },
                { "en-CA", "cb251da98ad78eba0808ac5463b84c97743a7ee6a20ccf745701b7272127b5512835123787132c0603ea20426c05d7a0d5f2b80ab33a1481fdce21ad325fcb33" },
                { "en-GB", "5a7b8f1fe9dbe74065cbff3de6abcfadbef94d91d951381ea9a12a1cb008d22b66a3b1557e0908e409bd022266f47146b0c4374e51507d5fadbdf4b2b517b819" },
                { "en-US", "8946219a57938c9698bf47445315b76fea2c3570db8c598ecb87bd625786b7321288ac762b2e69440bb9a946e82a5e77980a5048a4a309fd9d455cce4f9892d8" },
                { "eo", "84e02bc2db82d3aa5f2b4fc9915a40d7de102482c048a5bfaf3b30b58e2a669efc1e86b7d16643b9e30816cc3336ae47fe701a7908207262412551e16a90dab2" },
                { "es-AR", "a7bd6a75867b0911c06eb22fadf3ab509557a700622f472742f2d7d4d298b1eb2b22410b2ed18fd81a9257407b551b0c789dabd17ec016cb6c5cf26b3e9da0b0" },
                { "es-CL", "7ef89fa0f7b32b8a5ec88ea81475604d6c679f0c0811f7fae8cc3835ac945725a88b77ef442c23a95e66f93a9eb66082d93bce6beb3b3fb3285bf32043040bb7" },
                { "es-ES", "a8fd9226524b11e2ef60fcd4313591d9a2e50ef703b0b5d4d1b052d7ae732dc2990e3a4130b8b5d8e374c7e31cb1af5cdb0cffcd473761f04f47b2997f6443be" },
                { "es-MX", "5a05f99a6fda672c377fec5cd35366db1043992098d5955e3099001dfa00e2781d7b370876f5d9ff4fbec64a1b513bc2d3e3aea8feac85f283b9cd8557bef8cf" },
                { "et", "bbe38093091fd136f06d933d22ff5dd6e9ce50c1d7a7807115bcd155f9be1fab02904992143842e7487c0fc9f056fd2f4197a728f522c6d336cbbfd646c22c81" },
                { "eu", "fd1e2f7700604e9d30a4ea2c24e6aba01875ab484cefc1b72267fc95140cd248f6c34993575f3b2b4d965c44b44c8b40a5825a68da7a22f4c480714dd231711b" },
                { "fa", "64db6e45e5e2a3112a8865d31a2c027985f27e1ec56bd52727a0be19ca982d6f9a296faacc034acb4b84e9f53eff02af453ee1d02c9510913b0ed720c26ba47c" },
                { "ff", "a6f4690f56da0209b3a3033079cff8d85fb1609bb374a466a6c16e4db33c79b489328ab530f8db1677dc71f7dda5d418d9ccc4892a27b18cd59bc85adb4a5a7d" },
                { "fi", "249fb93e35a0660408c146823b1f2cf421b95cdbc573c78aeea8db49e2839ab121ce8a2755f20267bac174e25cad9472e6e71728f50732a9d6175dc7691a1a3b" },
                { "fr", "69cf8dde6b4b0aebd1ec8a5e96b327eb780e4fa826dda9501d8a52659376274fe0d9e2d7a3a607098ea0fdf53e58f084bd4f531c79701e04dee66e82ad845723" },
                { "fur", "a32d7003d7fbf898b146d4f3cb0c313abd70ace26c076a093d1a134df0744562fb0d2f796b859d8c0a8b731d85b967ef32cfe6ab564d5b32d481111fe78929b3" },
                { "fy-NL", "d83bdb56f04fac77b0d746b4f2f4f82102a3c1e3dd17aae353759ac6e988d65e72233a10d1db968ec8cd4529442d149e119916c0d0948e0389748dff1ff26099" },
                { "ga-IE", "b37aa9146ff60e417d147cd155dbf835c4712c2692843ffa3e461c9915c05ccf10ba0d3e8aa4f502bb67a78e00d5f0710f83f3dcec6fd17eb9927bcff0c23225" },
                { "gd", "074afffcf492faa3cf287b9c6bfe711655aafd3f0c1122d59fe57b293a2e385a6dd375abc72f2deb3d00b72462d5f25db1e01f0c602c4f0db059fce16302adc0" },
                { "gl", "5a824092dac88cabb2917300268c12a6c852592cca10266b6fa9a013b0c96d3174470af5c88a3dcc91d64fd11b7369133111cf5d6289672c8787460841e7f3b4" },
                { "gn", "1a84012aae3dcfdfa13e647f3e31737612c6e9029bb1fe9d0df2b8ef552229a8df505e5f1d213ab74433532afe141fcebc499d1ae20319acc8b605882b30685f" },
                { "gu-IN", "407dcd98ae279fb2ed42b0d93d702579b2454d65420ae2fad31ca7e84aa76dceb2e925e197e3a8c91b2c0132c4a1299379ba248c484bff29870a4fe1ad3a3a04" },
                { "he", "c3297e2306e423388c304301e7d0035e5f4946e076c95bffd9012926be3d24f9c0b47f8db49b320a3fbdfc71a448012d1b4682ec8d2aae11787e21a3416fdb80" },
                { "hi-IN", "92cbc9e6f214496fca801d13194de77b82e0406cce0f6d211bb815f45f45940fefc51b51f962bf99a1d655adb448fe0b029f9abe5539323a3b16e75042d9afaa" },
                { "hr", "26749da3408445c9c7dd6092c8a902d7bba495e9301d2e7c3e752ea350f8f4a99eea484f44f6d51fc53543e83b1c4404198d11a04993996bea0fe3cb06fb5cd2" },
                { "hsb", "d2d6f288bfa9ee801cf6762054ec2da6d4c68ae809ccf41d5418f78bfd1339282f2b9750a7faaecd0c65faacb398dea28820e6cac1a8df955dd9b5403eaa8bdd" },
                { "hu", "f49b0dcb72e8c4902a84639fe4e4e4959955b83aeb2e36c24975a8e6843d93d50ec7bee8bc56c0730a9669b27bfbc592f65b6bdae8b3ce417abf048cb1002eee" },
                { "hy-AM", "8945ba204602e72887d77e2fad3e5ee8d9af8e6a06e9b919bcc04aa57e120f4f4db194fd71e3951ec057d62f46cb7597b059bdfe391ef0c376ee133e75769a20" },
                { "ia", "0851b6feb14c4ed7a5b9e1dcd5c8fbffbf8770d54e90eb9eb8fdda1d655bf2a1db12423174cdac921c4ebb9d89b186cd48893ac4b2b0c28fbaf6365ae2ccc147" },
                { "id", "25112f054e5b15fb8f1a69fa68a77a3a05d3395649e8016aa2048bfa2c2cdf09d0d08982f5922bd8c70bd9e5fbfca3bc54730c12f39c461f53ca7c62e79a77e4" },
                { "is", "ece43f3317a567ae5b8ca7155f23a4d797f7a06e4b95e4a1d90403d6b03887ff3f6ff03795eb38ef09108e6ee3dba5841055deab32e14a8c400df27934c8339f" },
                { "it", "2182c4f3ef8d17aecf2927f020d1cc238e43b7963016929588f537b2cdda65a177a8b433d507ec6e2000583fe43a65accd1106981cc6697f05b3c27e0b73cb53" },
                { "ja", "dea30fa67d6c43d4e2666355ca5f2caa929512df6243b2129d522ec016064279d9b32b89791bb17027f267bf4ec126747444afd1829a0ffa9a4908bd74d16fa5" },
                { "ka", "0353908afde8d7f2b62bd62366a178430f32bf8d1f0e6db248e36a3f4a9216ae74ba9c5d938c33371a64f9e6be5555ad3e8c6a92912e9f73af262b2299818311" },
                { "kab", "dc1628c6d679d22016631e8fd5af7cf0e790d18fe3c46c6c7c58da39a6e550e2acc9ee7bd1cc12f15992d66c5846e9a3f6f951ba7780fcfc2d5629393f17d25c" },
                { "kk", "0f83454d73d8486b510cbe01aed4217205ee3c9fea0ac0530b979e9a5d8848eb5626ea5dbf43aea42ee1008daa83a02585c057736c14c331140d878fa74571d8" },
                { "km", "76c591cf2f7a9f5c7d7e7ae22e79c5fa064e47da3b52723a34e03e1c57832a0e7c8a801d44ab1c64ee0137e8afd4869315d36813a83ec176efca0a91f7ea915b" },
                { "kn", "2f8db60499096c81beace965f6746fbe59a9433b38e28be00ea15209aa7b03ac91f3e91a281eac4e3fca055d43c5122605b8885a6dcb7c50052f3df6987f46b5" },
                { "ko", "d53532187420fbed5b4630537c8807f122b5a9502c45529e31bdf16def59179f5edf449f5022e5b0f15ab062e017e30d155d213e851e660acf5bfacadb10c371" },
                { "lij", "742d2ac4722b5f84946c4d06b385989946e7bb25aed504e304e127a4e82a8ae7d0efd418729eb19c3e95ebe77181722e3dcbea1ae2e96d97dbba7e03e120e67f" },
                { "lt", "658734a34b60e79f0e61d49bab5b750b09b08180ed400a78dde25b2ceeb9aad0437d55ae476cd601378beab7d9a610d07ba33cd544f0e7daa23eaa8977cc31d6" },
                { "lv", "46fa6c59b086d6874bb36481fb456ce334e21238b088f37c10a4a75bc42c02856b2f5a628bd34e2b28b6798b20162320c366d9a641affca7311aa17cb70fcd85" },
                { "mk", "97a2d6685ea0245de26833037e1f1020a70a6f8aef925cb1711d6c9b693fb189d9b4e7a6e7f281252a2fab44cf4e7113b7b6ca08e287b073a2f3b891c32bb454" },
                { "mr", "1358f8a84c476fadf623ba79c86af5cdf68713d2b2baf1bb620e05c391302ae10d9cf9b431eb6100153ac4db99a0ea84cf240399ad855bddf056af1560ae6bf0" },
                { "ms", "b1c20463c1c3c67ebab6717c6559e9dfefd307e4dd18c4da9e0fa97d0988919c002ac71ef52309436e167313d42108f10d77a01335cbf731f4bfe0911f61b7a0" },
                { "my", "b5bb2d71fee8cf9aa4af89e857d4afd9684a3d7213233de697031888b7d3099bb51d3d618d5991165fb97ac1ef97a2f6181240f1ed13945c3fc35a4994d3a826" },
                { "nb-NO", "29a4436ffb0d6033310750c20b3e71b6ed4b4e418c9134b2cc6e82f55f2f8d7284d9f277335be94a5bd0d5fa6f3bce27846f6a415fda2ee7a9d0b5c91bb566c3" },
                { "ne-NP", "e0c8056cd9a922acade7e99defa42b31d441f2750000e206dc581b7d22fe4480fcd0efec2cf15333706e24df66f6b6fcfa4e3cd3ed4f587bcfe6210a7dca82e3" },
                { "nl", "f2a13cf4e66f97e77180cf471f1f54083ea7911ce92b1185af4c362efdb551593d73bd7c25df44fd68a64774ba812206966f5b23bcd148e77c39105021e1e618" },
                { "nn-NO", "29faccee37a9000deb33a0c4dfd0eab50499e69d0b3fdec43ff2235823cc8504bf16867009e3ad12a03673a95d5b0430d9858a2da5ba553ea50387b1b573ce52" },
                { "oc", "c83a1177cdfc7c4323d4439341342ee389d1bdeea4d4cf7000df13d673766ee9082fa2d4518b634e704045d1973f36013b56cf0ce4b84c1f7948e7a1d7aa1d81" },
                { "pa-IN", "58776b31dca753792204810632ad3f82f8debb2c5e6b38a709f0bc8f43703be203d0ce7f81449e6a40e2804b21fedb6bbe7b4983845a8a1dc39a30c0d322d8b4" },
                { "pl", "1df15fc27a374bc1ae32819151b2477175c38157544ccde82b396d0e446964668f4d96354e8590b7d83aca1d60c7181f986f4f31ac10d8a52ff3c901d80e7e63" },
                { "pt-BR", "31422876fbcde21a6ba682feb7fbda2212acdc484f29ac9c5b79e204b9cf58570d773042cfdf33cae9a1d68cb069d78730f99b1c948ad551b83f3ac5b5b6fab5" },
                { "pt-PT", "7df04b08dc533f7b524973948a7f24a897225df0d42c924b96091a6e06242f43f0242403a7aa9eb56643984c899b4c700f489371371a1a298d4451a5f0fb2eea" },
                { "rm", "d12e986eb8f6ff98d806062183968ec1664b1ed57e7a4774e1e9c735b6e2dd79fb02f5b8c3e03bbe1c01eb5656c69c99d320c688d385a5674c645ee672518ff5" },
                { "ro", "9e9b35b69d22f2d5abdb3b10e57488ed5757d5aa7a85e2a427988649c191a0fab9fae3ac16fa346c00bba8c879ce3701cb2ec80e9816cfda5c7750158afc1e60" },
                { "ru", "63de0a1be1066863ec4e461d4ad9deb97bc2bf77f3a5c4e55cd1ebff07d7c9f390bd7012cd5f416b1e29243eb0f11c04b6777c73e5e06da9148c7c43ddbe62e9" },
                { "sc", "55225ea88458e30ae173ed2205f9e80171819c215a9d56d0f4f384f7bddb4c5c3d3ceb6990dd3e7501b1680b12f68fb99c47ca328e28f064ba759739106d4454" },
                { "sco", "4a760baf0a49d75b30cc21268cb646dace5c655c79a670d5a3af9dbc64701760f3ac2d32fe573565e9c6752339b5f1c486c594eaf3bd6b1ca28bacf2803ea160" },
                { "si", "a4e055e3099e078384cfc324f91d9841c5884d18d3f2209fd40c88704e7f79cad423499384b2b500583c36ff78c63bcff4818d3b38a4de8c2c14d02e33609bf7" },
                { "sk", "c6b8a7cc75d59ec4ae5e2b271bd001f9c39607ec5a62f4103367f8c07a229ff9626c37d9ff9296c5c4ffcf1c968b74f54076a68982b80da79e662758715fc88b" },
                { "sl", "b3b6a618a254f23005406f8c5345261261185bbdf1f7da74745b589e2ec67e1c48005a4b6248b868c6cf7f0cb189eb22724a1cdb0a00e828116d1d19e76234e3" },
                { "son", "a732d3ef9740f3ca5899be5158d5312caa117aa7c3c7ad3d04b3855c20e3b5b1099a570b4d633e4a03e3a45691bb2c512d1396e968f18329932e95497390cd53" },
                { "sq", "d79ba00d7a8ef09f2b0819a6d8ad9ed1b5fb0a151e484a77602d049ccb6cc0ee53b4630e6bc92cc86927adeca6f9618b2cd39f0680457d388dda8951510df5ff" },
                { "sr", "d31724bd1f7d914a75ca54e6de115453382ac7c9cbd4c3e578009098320cb0d7c3adaf357a8b06ad5f0cee43494859c7b5ee8f9a024d0b769d1b0f93e17039de" },
                { "sv-SE", "f6ce30e1d4015ed972c4ceaaa91c5d5bd99d1f97cbb814b105a42af71c9927dcac5d3f8e22270ae903f730d19dfa375b47075b1f4fbc46908a89cb48bb0f841a" },
                { "szl", "bfa26209d4fab013e9403ac18461f1739617b1b5b937248d90bf659b3205d845498b26319c9d0591a7221d90bbca986ab0ce9375756ea5ac97bc6c510e8f8215" },
                { "ta", "b8bcada384db958eba45d969efcf2a0ed068dde5a692c92c486497b95749065979d4f1f2a0f68443ae0df57b527b08fd81d358adb6cde8e13891c6d122882077" },
                { "te", "31396bec5c3e08ecb2546d9c6120761c3dff76611299a00132ffc169f981df2b677b3f4f53fdf9d0544e81834d93cc0698b90bb6788214c5861edd846002731a" },
                { "tg", "72297e6ad9e659162125bc0775168dae5eb26fca1652dfd5a1e1ee86971e36e02cef8d87c72d01e044ac8fb59cb1007afd1c294ce459ce4185c85e4c8e55e24e" },
                { "th", "a1ec4451ab645ce31696eb374704eabffacbf74d057645f65aa410b8fd6dc17c1ed228f0fba4fece69ed6f68557416a02cee22c1fa26e57e671ddf22ca8a438c" },
                { "tl", "2bce8036cde6c955dd747d3f464cdadc1793b2ff548bf5b0e814e754f527ef1980a1bf98c029f874b7cea5e2fa5269861534c8e47cbb9da086bde080cbd46743" },
                { "tr", "17b0153de6eb868279ada7ee0a6c8bc56756d1daba19459ef162b770ea9655c6df5450a2515ed9b0b8f8818ce921038d04021bd8c566843c761dfd378544d161" },
                { "trs", "2a91ab90460c6f5bb1f85c2104ce1d094805a5f12b344bf52787035960289382ada4a3d5633651be0a2f68601c70835855193b0826bdbbbb854cdc5d17fba817" },
                { "uk", "b986f81ed7fabb63d991f9c5dfe02a89f5f88d17f87017d6b982c07f2c0456fd304d2a5ff502757ba957425816103c21ad67329eeee1e7ae8bdf26f80bdc596f" },
                { "ur", "e7e13429027a56178c1feaf421933a02d47fd0ed96bd8b07b716f107d8f221c920900d9f47b1d1ed082325cb8de764f56bd63820cb2449c019021ed31aaa2fc6" },
                { "uz", "5be36de3dc94eb7b9aa233a03972c9495cbd33d196f19b4af1b65e298991f05607eeb37d5eed09ed0338ba0172120156c459e1ce7ad7bc5b0e0f5d4cbf95bfda" },
                { "vi", "d93a08ce5f4ee28eb746c35f7803014bcf6b1d7a7d0495009447601d8e5960ae2a235def5aa1f7d2746acd3182bf2308df1c7529cae5db73ac4c9d6ca806c01a" },
                { "xh", "9090efb26b8f559865b631791a959cec6788bc1754a862d5b2558626cf95cafbcdbafbd4187d9b56d1b513741b634d9faf9357fd451c1a7e509fc681c607f9e2" },
                { "zh-CN", "67efa828b579b8226f91246dc709426d6904ad56a08fa92c835b44e60784bf53bc41937e2b930c2bd63e9f9135101306fdd857a137bda1391ddf9792106fddc0" },
                { "zh-TW", "5bb446bce803a47e8c32032fa0d7f0eaac220f1d3cc63b4f0dcfc3c7b583f5df31e07418b61612c42e83153e4a830419a3129658476bf3bb277d515a7f2229a4" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/118.0b4/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "fb957f6f290c1dc503b89a0b0f37e3c8a29678fd2e44edb1f715ed2699d8aa2b74c99c9ff724b4ce9853f9790899230754362a18a726aca76e8e4a3c2ca7a107" },
                { "af", "556798da640c3ff4b92439f3668ad39577a1dcf2c5d4e8df09aa033d3d5dfc457c81f3f39b7d0cb7a8131021bda772ec3d70f36fcf321a51ce8e122bc2067dab" },
                { "an", "e0211cc5c05faf1132396e6c9102ef7011c78d97c3c15e11ed6348daaa211db95454cfeba68ba829d9743c9500bb0a0a17e39695e076c6468426ace68e34fe71" },
                { "ar", "c8087c531d7ace7643dc574f95542ec7a166e26b96f895a2edaed688d96c4a7fea5fbafe131d2c6ed6c0bfef09987440058ad2a68cddf07c3c38a20cb5128340" },
                { "ast", "0c3f40ba1845908308c5e90f67e94823cc1b555ae1530d91db3127e6d1b829c55df4a3da31781e6db44ee2ba7f2a7f864ae4b24586bc1b32e0d72e3af2b818dc" },
                { "az", "22a1d8b8e2cadbdf24f5fe5d8f3b0b20de256a98fde148f93bef3c265833d5253cbdc3badb20898fa46a1f62eb2b21cf6fd0ebe4340778f712e39de1b04dfa24" },
                { "be", "a145dc1752004760a942512b06af6bd2c792f3b1e223e96244849f813c7f5c737bc1426900b8346152a91047bb7981b6e7ba960b36e753c4b4322fe6d50b1210" },
                { "bg", "9e706f1681e0b2fd3e89e75d86d1da94374c3da3bddc768dd9d12e4d3ce907a439e017ca8a81c0ae688dac9cd90f4011f6291c60c1904fd42f4661a9c4138e76" },
                { "bn", "b927b09af797bb15c8fcce58098c2b13528f5ba835e5aeff50cd3079a3af07ba0a10738ed4985feebd043ac22b8e8b1ebbce66bb547bc62e6c3a48f48b31833d" },
                { "br", "34c94961972b678dc7e19a34890cafd8525045b2b204ce628ab16bf79fe52463ed0920c8fdedc613f2e78c463dddb37d1c5a60da5db14ceb2734f22b1ef2a2fc" },
                { "bs", "53b4204b2e329c94af016ea74bcbc1d7ec4729d56c4b821a6e642dfe0404337e38edff21c40e3c96aeec3a0cc8b78ce890f8c7d6841a3481b0fe7e4ce67b3b92" },
                { "ca", "dc7504aea0bed25ed3c64cd2d5b51a04339bea464047446c9f76285d7657bf3ea1daaecaf15c8895797bc58c4a905afa75927e6570c6f2d828b284d067db2bd9" },
                { "cak", "f04d53327410c1b0dffbb6d82297c742ad988d642c3b4c3ba072ee617bc98bf01f0fc756d52e3e7d1465e20958c3b3775df66a699fafb85340a8f96481273acc" },
                { "cs", "f40844fb69d58020fb6f202d1944f89416118476e74913bdbe88178993f8e2c60536e8207298f921388946982911d201a2bb38832b16e4ecffe9a55d9b78f213" },
                { "cy", "61f891fe22edbcd9f6a1276aea2527cf059a15c00ace6ccedd4aab08ade13b7a953149e714614381d18558311edab927aff1536fe92afcef6a908e2fc5d6bcbd" },
                { "da", "54ef8d074d7acb9efa69ec6a76896abf20205e4540df6e9fa5b5d7cc93dba532f1907fc105c3c1c1a79897fa2f91874e3fc4779cae9649a070105c9db69f3691" },
                { "de", "461da25853108b9b0aa920493f9b12e0c6288aad5f57cabb716fc25a820bec0ca16adafd191f3dfb9df07f64e38331009cfa83f25b8630386a21b3ae284146f1" },
                { "dsb", "710a250ffebe1663d13813d4607e93e1989dda7c14d5a68f73738ee55ad2076fe57587f67b828fdfa1bf1837c46f0cd128dc4710769eede382f761cb8337dcf8" },
                { "el", "430266c9e4da257667907040464b0218374a758a5d3c95a215b597ac287f9fbecb8990a31738706bccec84efe110c25a4ac3026570d886da38ea32f621b1fb3b" },
                { "en-CA", "8123d5ca0493e8ff9e9dfdbc6ad4c7bf84b49e1359a62cdefd4114258417d03580254f9f2c43aacc070912366956a322511daa3cf3915df2b108dfd095e20870" },
                { "en-GB", "4b67f9b4079d7cb41882e31c8349bc2890409842a25144fe4a05e611fa069d38817f5ef18fb6375f60007c7d03e96cfe2bdeee72f78f7a710b7d92f88d599a08" },
                { "en-US", "471a0a9a1c2cfb4b87644b91aa014f8581af3dbe18951b4290460b4886006e2c4cf5e544e7bf5e26b1c24e0c68ec26f95aeb6a3a56fc3de2ca824df8feb56258" },
                { "eo", "3a81a7df554d182d1c3df7f5712ec3a36e033fe94a54cff7226d3564dacc21e00e2be410316f9f38fc8893a2be26eead26012f04f7118753690cd2b8c22c282b" },
                { "es-AR", "7f7e4b18df60c2cbab3bc90170b952247a3d1825381a49bbd49d652774368b24d6803ba4404c414dac52a3db68408064465b1d14d2187906c97206dde10be8dc" },
                { "es-CL", "7bebb9af76fb90ab5c4861ff65baf5830676e00c255978f0641aaa06e7ea947ff259c83af442b78fe43196944cc568892acb64f6f1df681351dd07190388ba03" },
                { "es-ES", "283ca3c56ac55003363fa75a19cacd14a5384ea5a351078be7681ddc10bf64e60562be1de956567849ed3ab07ac7394b56eeae2ec8dc99417e4740a1d824787b" },
                { "es-MX", "b14be6c60a92e18295c03239b8d6ba650007d836d35a7ce9465b8c7c11ff8ee96c2eb5298cf51cc223de595a6d72acd461698bb7f3cf5f0be611fbf0495bd7bf" },
                { "et", "ef8036f4d736e6a522e0217b72c062a9618341f5422879cb5d4b4ada743155b808b053c597b808edd715bb931a2f28cc4ee443a8acba1bfb18678dade14404a1" },
                { "eu", "9c343947bb4f2a03a3d3c71e0c23795fd59ef49d5981730ef7ed9205fe5c3e056d70c58ad03b82996266f796ab46a41c495de04716c44bca40f2ab3277c1e347" },
                { "fa", "40241f4c7855c0715811c7622eccfdcf8a09e80c9e9195ae9ca45f914f57a006819ddb31e6c272c031621e36a74fbc24f41c4b618477a9e46a35f9867f3fb8bf" },
                { "ff", "7374ae984a060c5a162702394149d5b1cc8cdce8bc0cab8584f6281c9d7e17475c4aa87e2e25b50a4f08a5b013d1b60a401962ecaf32460b1ea195bf1e98341a" },
                { "fi", "90320f6e3e64909a7807abf4313208d1d64429029976446dd4be9e09ecce6cb28abce9f158b31faac5042b8b6a7ef12fe70d4710db05039d4ec5e9430db38533" },
                { "fr", "9fba5368c7e32d33b7a13e7c1e4a061dfda467a10c485dd0ce402cfddc26489e2244d058b141cfc138a461fd3ff79341695d09e760feb509ba38e4d689f52464" },
                { "fur", "be0cca8e35d5626fa976cf47bf54c2705313ae9cd1199cc97bdcf7235af0ef623063400af70cd3b969e10230eb21357af09fbf7ccd20bf04bfa0cc6b770f93e2" },
                { "fy-NL", "4ab9c538b577820ee9c9f693b0b80344357594012b3c67d9d688a4042a31acac8df0af934fa6ce9de1a22e38b131935f86201ff817dfa95070615169dbad07e4" },
                { "ga-IE", "37fbaedbdacee499d2cda0ba8e1bc713f40914f3859f4d14356c412ffc3a6f8b686e97c573b2c43df51ce7d8a859edaa2cb76f85b3bdd30638fe3522de73fb74" },
                { "gd", "4a243a96596ddbc5df768134fa5542fc87ceba0b237e5437f1f421917222910129e81b1323a4711fa5530e4414158ee549e8e19e69334bf40464e16d9fdf0e45" },
                { "gl", "fed1dd70bdadc1266fe9e4280b4b90f6a870e23601414511fb8a9edd27296ba6ce3a5f70f903525ab589f8f7756bd7b9b572200801c9123213077f372c68afe6" },
                { "gn", "fa087f3e1926a7e86829825d7da2426e436e46cbc803512723d32340d4f8befd166bf1cd186e648ce9f6f384e2bc08f7d665c60012a961f4c9a150806860fcb5" },
                { "gu-IN", "49e9ac3738887a9169da003c5ac26b15a34a6fd979d323f93393b3870296a836e731c9c0009e388a1f9c7c3d59d0708eab55c37ddfc73ac62e89bf974aaf4a7a" },
                { "he", "02ab2e4090df195f79c884ac23906ca56d2faacc114aa74a3a5dbe6a937aa9d37c29f06dd66bad7b5c0b59b56aa07ba69d4e35d1ec0017528a1e7613d6bc6418" },
                { "hi-IN", "ee087a00df507e8c5087410949b9cf15dc438497c9eb1472c68a90e2d8d958448f65aa64041696395a98879e2dee7b6037b064611fd29c15b0fb7f2cb63a491c" },
                { "hr", "a4dd0101f08e1374e39681ab282016739497fd3534ec8b3422f218b4404d88695509b657b08df80e7042015f364812506059baad9e2152da9482864428e1660e" },
                { "hsb", "a00186b9eb314233b08a22b073204a7d8ed138a37fbdd4cb8a550ec11468c938d9017d7335fd3d8e2f8ab82f12ec0140aa239196ac2863db8ef95380757c58df" },
                { "hu", "01bd7f225e43e566ba6d2a199e0c0fe1c408964fa33915d7715e13ec469ec0600e5bd4c794d4da4d8941da7e472d96b7149ed3c925be0da45df28b86df69e9d2" },
                { "hy-AM", "3549e9f80e4de1c39f7db5159bb6b9345d21c5f2409192ddaa82ab6ef72335513550e771f56cd2e4cb65d7a2ed096c48297ba3a3f5cd3ef32aec9f4a72ac76ba" },
                { "ia", "b01d4780ff7254ff71994b6f31e20c670cf89f59e5127f50d07e0661e9a54ff83fd56b4756395b30136929cedf1ac692913f0c4b48b566b5f4b45836a1019d6d" },
                { "id", "858d5becfd04e7740f73a088b6deca165078d3ecae92677dbc80c14cd77f3757684c93f2c22ab68f9e265957343eceeb22b319dc33c3f3b9b57a8129ee36ef50" },
                { "is", "11cd7b7ed11630890d4fc063081559bef645b4e3559ad2fe63086ebfd267d459d06a08cbfb4de02973d689a65bc076e980179aaded4c3ba12f945af2278bdff4" },
                { "it", "d2fd30f29b77f7307e6a72930df9226150be032c9718c189a6bcaeab8ad4b163e4993155054ff890e601ea091bd93ba0b3878eefa6b8d214b447aec2397653d1" },
                { "ja", "6e75b6e08979f3ef6d778fafef19a2d6063b45ff4049e93bef2936592ae012064abfcd1c33b8551f700b5200cd046d2ad70872dc11e7db8fb51c778071a43b98" },
                { "ka", "d9dee8f427729d7d5013f952449d3f01f6a77ebb199194abb8dd0b1a871276f44488d247236b6fb5bc9b0795eeeeb76204b71b3f47f010243ad0fca330e878ad" },
                { "kab", "62612cc8431119cfed222ad2c64a8f44d6a2686f03cecedda0fbb9f3799e85a5fb592740b194e9d885b6e61d452a31f071e89174cfc6c711e3c2a98520f41ecb" },
                { "kk", "c595722cd979b10bfbcc21070621779531374ae772ae7087e90530efe4dbab262cb3118090e53bb5dc67da9d7f6e55fd5f76d3154690b709862bb5b2f6980fc1" },
                { "km", "601d899e974529ce33f66eccaf22399ad5eb3c876e973d47cfa52946c588e0f0657152af71a91cb3bffc97b917390606c636f55dee288766dac4dfad8e27b132" },
                { "kn", "63c5a4403aed79c4fa32bd257ec85714c994104f780c0401dc392ce4669f447892339160f3fcea060192ccd7b01ecf1b027a70bef88860966b2553432362b3ea" },
                { "ko", "7e634cdfcc1eae7507bc5ab82cfb1240205b855b371ce6e813758e9c8892833669afc317dcde41cc34def8b1fb660661062d0e508e79f1e5ecd356965cd915e2" },
                { "lij", "7dea10fe14869fd253e493fe12945d0b769b9a287b4aed28575d3c44728159405a422fd3d55e90aee1f237047625561c6c8532741bbddfb5b5ee1d4ec4eaf1cb" },
                { "lt", "2c00add3aec91f4f876f64cffe5f2d0967a914902ea2462d9772c19280c7c4a30f1679ff2c95f56b7d543d3d8f75e8d27feb0f8006516175f9e2156fa0496c2b" },
                { "lv", "0b12564f48dc5723b7028a11bd4f32701aa004ac21aa06e54d2c5a8ff97a70029347ec29ae6e159e6719dcf9d7bc5b227a55b85d3473124043f6864870c56d45" },
                { "mk", "c17a7053bc1061ee7bdf50da479f4482f41113f60394853aa43aec435d7621882e0e8bce3daa40a4bd468ef660accc37d3d301f94218e738ae96da704f919949" },
                { "mr", "141ce8f54c5c57083a5678fb2092bb1c612503bd11eb35a11f731787f020853c6d59ab15f7a9d242df44b1544782bf71cb6e7496411e50e42de68246dae2d587" },
                { "ms", "aba66c7f4c39f1f36f5e6a37f55a4e2af47259af4525a287110c7a0f81ff1419100a6d03c9e2b2d0c67c9a004f807b8c898a0f996503f6dedc0970c30a0bb09c" },
                { "my", "794fa5c11a12b435f7562075ea7adbee68eb3cb919ce70ec71c7a3779269500c70efcfc8bccc49eae7fe1ae9092914c04b78b81e2b4c68c825b4e62e463786e2" },
                { "nb-NO", "2dc37589e545bd89c7f6e7c222d60542359bf58335387c8e25799d742951efb6adabdf289767ea4589c0d4950ab42fe4ed0539dc78ff673eaf172f17b51e5a3d" },
                { "ne-NP", "4600140f24e7f6544efd03547afb0839eba12c39f0b93136c108a8075b7d3b1509eb09969cb3b597000007cbadbc34adbfe4647a286baf1d44dd47f7aee1cdb0" },
                { "nl", "54023b68be93b1839ed8d5dd660f22f02f6ef464461886ee6bb0be5712c656ed79db89619429609bb1729de2869de880eae3a761cdb2af367f02ff2202fdba7b" },
                { "nn-NO", "1c5a239ecd9d9d8f7e2e2f2f14618e8f8a8b90dc48ded0274db867124366e34ebb92359a1fce053bd8e80fbc1278953a4556c3091241b3678ef7fb231cb76c87" },
                { "oc", "9deecaab4065ff380b684ca06f70ed61fa06d06da0415f9fbc3c381c06a4325cf9352037b62e47f9d3446dc059603a1a0a066f3b94bbd0e469186508f1c7901b" },
                { "pa-IN", "8c1b83d11797effc7d40cff5885792e0dd91790af9942da9a1fd65bc07e6ce6b9efc7529fd17df4ed3c5be488f872b52bef1abc4d00646f551f74d5b4192bc2b" },
                { "pl", "8dc8ec71d0c66811e669b1a73817a280fcbb582a6e1552321541af453812823fe9d13621fe718fca649938329ddeae9dc63fef56ca748a17dd6916e80a3e3593" },
                { "pt-BR", "7833d253488a9e40285a65acf7be101180cc644869455acb2aeb9a8a97d59c37b7d39c0e53efcdf494e7db3f6790418299567c3d020f8a9a7180859186f7f83f" },
                { "pt-PT", "dbfc60dd8fdda9444e534a0afc827304be669b9c50aca4985906c348855a6d0c972449bfc59a8b6a13cffc960ec5a032dd555aab617ae1059ca3e26298ec67a9" },
                { "rm", "f890ee75f81253c321215ca62547d93cc1e63f0ffa9367442ae94ec11032a050a6a1e4bd942c0c639fae99d74152ea1e03341c120bf368b7c2b61f250de3ba1c" },
                { "ro", "017c5c0cacc4e7d60c672b1da5243cbab4be27d98ae83e4ca0102fe1ee77cb91953ffb248d59f639f585f518be1ad159cdbdfed10a504a2eb9811a4371fe4d95" },
                { "ru", "d2fe28772079104a7395c3697bde91d5503f115698298a8f7b9429e90c937cae451f0ea4c1f260b13e81b837e2cb3a2526aa6cb157df64c68a74b48802eb4167" },
                { "sc", "2937038bde29105e35017c609ac746c6a1314ca4e6bbec5d97f983b2431f74f38bb55b802ef1ffe27c3397aa860b3318be7b3a0d9d36273e3c9a2ca135fe4d1d" },
                { "sco", "4f13f863fe081b68cc191fbf4b8b5c6c21c910dc7bc24d84d1ed720bb9f5ba3c34e1418d649b9d50a9e362bf618671e21bbb1613d21b28395b6b9f308aba8ec5" },
                { "si", "8499278e6c75e2ac00daae6b55a3cc47e651ebb3fb20918957608367e356179b5fcd084344a3455e0b0de89395390e15ab5376e3f5ed93afdefd95c8d37f3e75" },
                { "sk", "49adc69194b6605dce1c01243ed0449031323444268d316bd2913be9115fc65bcd5e8b429c66a8dee90e4d0121821a572bc72c7d4cf2d0662d58389a27959e25" },
                { "sl", "fa31caa48f052a1617fde3fffec4c6aa5600da0c13bfa6421a33e2f6bad52f69857d90404c64ccedea5b740ea366f7cbadd703b7eab07133465893434ae6e944" },
                { "son", "a938907899f36568398e018baeb93894c2eb898dcb20a7d3f68970ea6aa937ddd4d7522f5097f16736695d9dfa6cbdfff3a72939fb42a9d83641cedaea0d18cb" },
                { "sq", "32a4e140de0a3a5458e5c0deacd9e1f22de6364ab85503d112866d2312c0d0caf54836034b02bcf1f63da839a184c33862359a9c52a83100dd47d3547755a785" },
                { "sr", "5876d8bed905183b57bb33c0e9f3eab0f3cddb9df6d71e93a418289c1a3e6df3ba494ec228293ff5608400058dc8323f0bbae44208f00de486f5c776e87f11a3" },
                { "sv-SE", "e9910c1df21d9fc0fa5d38a9f65131f9f592aa9a4bd9f710b832c9981d8d82aed94ee94e59bb49f1158e70b6255e3087bc0e7486054efcb2c3927948e6322992" },
                { "szl", "f8991a7925c30620af9c433c8bcb04fa90aa4dd5f0f39833c1f8192e6d28a0b86395d0a93e74720109698a764dbdc6b82faaea251634b81b803d58a42eb454f4" },
                { "ta", "3c3d286f78979e8cdd29af8ff39810056a9bfad3228c8d0df7cd3dce5fcc2d5a468f87c782e0a618921d5c63defcd014dc2ee7f219deb6cefa9ac4b7fd947508" },
                { "te", "9e47a599b95f61d0b18b0cbc08d3a928736eed4d3977924a223d03bbaef4b775d6f33bb37c363c544c1a3c7de2ceb05dff9854c03b316423c4cb5d90052d1603" },
                { "tg", "f94406ad46f219e74221195fa8397d4bf2a21a469bfe5f645142f79f117d03a0c811d3c2f5cffc81a8439c397d0efd1aa0dbf8f4b76bd469b5e3956201f989f8" },
                { "th", "e617166dc793bcd6bf56c598db1d1e2cdd03bfcee4de230fc04b4e8a44658eb41057e486e26e062f041eff85d58c3638e4378135774bc14a580a0451cabf04b9" },
                { "tl", "c5e4b477d607cc00676d9c67c30700ed6540683c73b2d5c7df1669ec11ae22ac2a9fee2ba2e980cd519f737ed606d3b1988968b9958baa3fc146716d0b8f2d02" },
                { "tr", "a6cb7ad23254f19d6e97d3aed38898416305c82c93276b17e23ec93c12cec5b98cb3048669d7259d92a37e7b021f4a86078ebb57b9e506c32db52d2bedac4368" },
                { "trs", "5949170bd83e59b63f58495588cd132b2ae898d6dcd21340da73c78ce31a8c5bcf3071acebec88dc59b01b2467b3d6ca435dac9fa4b15b25d30a05565d89e37d" },
                { "uk", "27801b30f984575d08fcbe2837fec8a409e9937996e9162b2829a5e2515172f1d1c677795100c0a1cfc2c3eee5b77251a520ac6b5af15d7e7035518680e51817" },
                { "ur", "8f9f07a50a9350a86f6b87f06078fb10148b708c5de9f1e3599870691dbb418675d864634a786fd57f3d495dc24202bc0fd3a0497211c51ea52be5fd80fb57a8" },
                { "uz", "1d9a1e87c407a5696d9131cb4bacfda3ec7b69934a0b965736e61579405acffe2dadcbe633a1a7a46444b260d186916ce26f63e7a9d9e0c0b758034f6bb534af" },
                { "vi", "842423666441ae40a199fbe92ca8bfc0e6d2a3e1f555c1422c280cdbe5a3763836fe57ab429a5ae6a053268477571dfaeffc96074e4f58d77573c6829aa5ebab" },
                { "xh", "4309f7afd4d972baf77ff93f785d1509b89a3ff44d2fb8c7147529ef94720ba3a1c530d63518792f559467d84e02d4e3be7eae69ff2807e7ede827cc8ae0020f" },
                { "zh-CN", "6ad32e363ace8d5c5f6834fc2ed02a4dbb0c743075fb48bef1ddf96c01d9ffced318da04cd93b87449ca46fdec144820813d0006faf7ad3393059648f8850616" },
                { "zh-TW", "7928bf812cc0bc746f85f952c0dfa2968f22b8b9da8b68c31b9b5cecfbdfc525aafdbfec6866c36128e5836edbecb408ab74ba3dcee7e9b8d7d858e3b713025d" }
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
