﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022, 2023, 2024  Dirk Stolle

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
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "125.0b5";

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
            // https://ftp.mozilla.org/pub/devedition/releases/125.0b5/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "ca2f0415ad5dcdfcf4c697679c62a0623003e6cc6e319bc29d4c04a68df16720288e9844198a318fb3ff681c72574c6554b995ce666f392b5740e644eb8613a2" },
                { "af", "e4de4265b22ee0f6ae821b744a97ec089f27925cc2319ed4e89566df589eb535013250a08689cdf828e0ecebe792d8b5ac1c8d7923b5be7b6889ef48ad737e24" },
                { "an", "cfdea1564b38b34c6f1fabc8eabb266543388612fc4c677146828217ff140511981f28f1df9003fee3fc8a2bf3b7da33d8cbd1eac1cbbe02c20dd244f0293ce7" },
                { "ar", "742fa88857a7d85931ec871f62cce73f1fe32496dacf5772ffb900231a48c16c5e94e78eb6cf3d5317d0eefcf4fb3ceb6b8f8f449196524a7f790f2129b266f1" },
                { "ast", "1e23fe85a6069ea9d6fb78a3fb1a4a21f5648a732383c6c169c131b49f5c8bd975d6a15c2adfab854b6ab7f5c3a0001cae01c1de72a550c35854018d36cd51a7" },
                { "az", "56d4889421325c179f34d1c6d8d31cc7ce8d8a0be3b21373e517b1b7811446bdb6b06c3700b66803f1f060f1696164f83e89f0099aa0e6dcdd9b440a5de66a26" },
                { "be", "f001a4dcd0ae73ead0731223b30d2afe83017f3226688edd440bfb7c40f2ee5a319c7593e75c07d0240e41203587c3d28a738a4476b02c094b6cb345533d3b5e" },
                { "bg", "2411ee55b91cb81f4d80437c8e75a9f9e45495a33d44c576f3ce9e273845f51dbab324c2ce79d974f3fcd2205a4054abba38dd5a8ccc68b4da531ae50962c71e" },
                { "bn", "e99e29c79af73446cdb5c8f95a622386b9dae3963e82c8679611fa8b0f655d5c95293b210b68276d24dce59a62abd21a5d70afa9bf8f07d70fb165b22b56390c" },
                { "br", "2858e9947bd1591f90b1d12a1b261751805db431a2b54fbe20f06a72339df9760af530d184e1bdf5fe0e34e2e0c4beddc3f07c31e5bf3d75440ccac79d777405" },
                { "bs", "b6e3913eaf152313dc1ff7d95499a0f379b21ea6173e60fc42f2851f55099f8032f89e451fc88d629cad8d913f233e5bfbda1e777f4522a43e8a8ac31e3ff1fb" },
                { "ca", "b6ac487698fd358bb7a7ec18dc94b42fd190fd9dc0114d3ecaddaed1df76349ac01b4aa12e7f7637316621f39396bdf464daa0dbf1e4c6dc7a00d0a41216534c" },
                { "cak", "331250ab1a274418a486b912fae51ac7d43924efba2c89f5e6fdbcc439a168b41dc88dded3c3536cb71f463d02dbcfaf0c7775013a8492f70c6cf6ddd36faa3f" },
                { "cs", "ad7b7e7f01150028405b4be507048c35c68c2e1a2ffcecf5ba8e9dee5c3f54fde0a39227a738a52279f82c64d486ca060817c477b59133072c0771c07de747bc" },
                { "cy", "60f27f99fde67015c81c131e1e60f893df1236406b9b62353a11741edd8f42964282928a81d88a397336c552fbfdfeefb9695f4e142ca8f0a747923294b7553a" },
                { "da", "5b9260dc388583fbd34cfbf3b55a3233dcc05c9ff59f8a121c48033287f78ee4165c1fce7ed95b39d0f45493145399e1e516f31de6120581d04cb0f034023e65" },
                { "de", "8d873b5d6eb61d6ebb8e823d89d30ab0d34e80704cdb738fc9752b1062ac939b29b988c3b115b2569cdee0180b154d435bba8e7e3172a27c34a1a777847cab54" },
                { "dsb", "05584a02028797258cd003c17c648d05d5f9b4a318a075331452679781f0572145e2fe6602e7c8d5192c4e5e52c9c279cfa51dd503939658c2d25ee4a3a8e18b" },
                { "el", "852beb2c6c29b88d3963a5c5705d6a6b0d4b37521a118032c3a6bddcb364daf9cc2f835bc1156f8c56ef47e89813bd87cb0a76634861a9df82ad7e216a8f2542" },
                { "en-CA", "f76fb60b9cd0aced6536d450bf80a5ed01df1ccdcd67331f2dccc1963cc79434ade94740a329f3137eddab8f846a42b3b298b3a4d85949765419c5bee968b5ab" },
                { "en-GB", "71040f2dc347904d2bb39ef34fff10704e773910f3cc7f3d258ac51a3797095765b60d8c9da5f58c9780a95f4d9b88ffbc8ea21ea05dc79faf157a0bed77454b" },
                { "en-US", "24a8a20114314fd10c8baa1a1a61a5184a94b412e0834c9ab73b77a1bb2f3aa5fc3e48eb8b3049b960cd46e0c3399ce5eeb175b49b0a6bbd6ba8f412dcb53596" },
                { "eo", "e499131dccf1715e0f87a569357acec57386c92c61ed267cc305f4b6b6e055ef921c0901761687efc27f8db3490d42c129ab4265706926bd425ad14b4d5533e6" },
                { "es-AR", "efc921357f209f6942bf071cfaa25f23e8d8c7561b1b8b72e95b057f4d74769b296a814d5aec81c4e5cd773e8d3b49054473b450fd101020965f03b4880872b4" },
                { "es-CL", "847a2d99429f3da2c0390eee55744441bcbcc85939f374e6ee57709abb3bca749c56b7e21d8c0af90795d53a7f103eb2acc82ab540871c13012724986a6d6871" },
                { "es-ES", "774711f7d9af264f88baca40b013e002cb4350762e0fda5a609625d90c77b939a42b3a6b6e4ad107beb607c67bb73c9f146723e2c3454f1abe31faeca6862e51" },
                { "es-MX", "627aee418ee19ce3003811b168ca06e5451b52715104cca1bf792b581c250a53ad615ab1ed6fde99a050954611daa4b73dd1bfe498c3503576f4e3928da2e1b0" },
                { "et", "535b378c185998e5183e83bccefe4fb2ebbaeac85ad573b958714e38bc675ebe2187552eb462f3d0fc103a368e22c13c08a4397b716b9a91f0f7db5a05b5fc4f" },
                { "eu", "d97fd361bd6d8a1c95dd4f49c67774e1ff2c27a80d6a7b47304e4e1262b6217cc93524113423f4bada7554daad3596d1ef92f39e100ab0e03bafbe1e7f64cc8f" },
                { "fa", "844b5adca9e16259db2613227b4fcc9e6b30d2e8c85ef853e44d8262eeb72ffb44255dce7346dc199b1eac0c75814839620834a5198361e58853363fab68b8d2" },
                { "ff", "8d5a557226d665c2c778516f871853c39d858af3239aec64d8715569e88badb2e74fb8fd2cec32a555f3b186bb52603fec4033b81f39d74f6bacaddd77f9ec9c" },
                { "fi", "4b2aa1782de24e86d0de76cc0bd6a771d9e62f4459e9e3a0f2f463fb9cb791180a19f79dbaa55f910c530e33e999d5ea1d1bbf93d0c4ab66800eb4adef40ae66" },
                { "fr", "cd26c7ae321c19c185a636229d2f5cf8d96c09bc9a14a4a267ea69db0b2aac1bc2c09c5788dd8822f1797ff63cd2fd077d0ec25e4ea871d699f673a73f35c81a" },
                { "fur", "cbd2f2b22e8ee70dddd99cfa31bb78c980a8cce51d16ebeb2f2920fbd778105d2b9adc3fca54daac4ea035b7dd5725d1c29d3b745b120b0098091d06dd9e04d5" },
                { "fy-NL", "159fa1a9f59b4be695158a601e130b3ba62e5f88e35e47338cbd54c933680aaa3f0a0eed45c31a9b18c950780a310acf182f8ec9c2b797890e76b0a93dfcece5" },
                { "ga-IE", "1905dab878c83bba0eba28b9079a38182ef4a020272488765877ec0eee4e48fce5c4258eb6c7d54bc46c646f3790c1a6b74578955d75499ca3d749801e7ff330" },
                { "gd", "38fc28673fe4dd1ad4f624a3d19600f2cd9ff411752b53dbde8892c875420e2f45c284fde208b5c65d689790b90fc5b405ee8ddf5ea8673734ec225231cf9723" },
                { "gl", "e30e474e0bcac33ff9503aebef9094ffda0fea8611936b630318e2f15054bf292e35a55570884f523ef8b0936fed4c94666a188fe8df14499be1c27fc01c6bbb" },
                { "gn", "01551cb7cd36d0b6b17359a576303fcf49c4cec53fd9e5575611d3c2780d0818676dc27d6e7a18d93247fc563115d84f67048b8fd2d9541f0d68c72e4183ccb9" },
                { "gu-IN", "6e66e415e6f7dba2d7ec0230e3ad4317f46db5bf1c5cc23a42c6b496d12a70fde595b06e1ad9a2fc342594fe73ab14683bc3832470cbbf76f2a5f539b02ca10a" },
                { "he", "6774676a3570dc004d9cde5504cbcdc303d3cd20607f859566e96e63bcef9c1c5e95948606c3c43ce1f225704bda552f8489a42298f9c7eaf4282ef0edee5834" },
                { "hi-IN", "11c71440a8a8fc2a4b3361c34a4521b2325a9f83db78d5dd99221776fc41b579ae15cfee50920dc0191828a04e5075536836c51bbe4ecedb1bec572c9c125384" },
                { "hr", "76ec8371c5fc05960cf61542fc5f6e6f977842df50932aaf68c8e27b06226ff26c3e6077103961f3e588af48db855e7f937c629a1aa2afd8db0f1b86d11d6bee" },
                { "hsb", "ee5b5d1b6fc33bf659f4e6ad9ea4d28c5f8ecec240b2dd4052307c44dcf60e45f6bc4ee0bca132f84a0a9dae324f9273075d4125e61f931844011a785d125cc2" },
                { "hu", "e5b37b3dae7b5b39852ea882c7c2ce68939afc3b677943fab52002889847e170b435261280d717cfc4de842bf947d484318a95152a02a54e1c222200cbebee0f" },
                { "hy-AM", "8390ba4fb420698bab3252ac838420a3d0e51f18e50f185567d68267262c26b59f12f2a8faba71f1905b301d9eb90de68d46a526ffe8ec7c258bef1fd50150e3" },
                { "ia", "6bca3f0320bf2d5204e7a354c7f537d695da24432ca64626f67351ce15eb455cf315015eed4e27af3fd660d764b20f281a6e9d25673b6f39f83891ad0620b68d" },
                { "id", "abd1253f9f43ac4ff84526bf6ea9e7eaae8d0ebfa45255b304105a522667cc87f436b5a7d55889a43eef6f7c611ae33e6be42fe45e4016243abc167d56c8281f" },
                { "is", "d5145c7db6326b5af849485ef2929428537486c12f78d73200aac7b4729165bc1ba108f3bf70f1defb3cf2a5dc0c6ace7b0bef4a408381df5c1ebdb1090d19da" },
                { "it", "daf129c7c574a7a18f0bd715ec6ea3824968caef4f9ea9944a76ea394328f93809922f40ebd1b1a9ec262b72fa4ebd1334c5edd6457cbf3c6015c5d34285c792" },
                { "ja", "1c8800c489396856afb9886c52460c3c893bb2d2aca1c1923bc8eaa90693a413ebeda3b6573a44ac1ae38a43308f354f26bb71ca8d3b59816e8f66ac6d37d89c" },
                { "ka", "f666ecad79083196655898ace8f6f14ff4de2b9fcc68e9a7ef97e08a64fc486d8b847028d7d21c08079f09b7bb9a636ceb41f4d08bed85f710826ae8b8f2f150" },
                { "kab", "6c0f7fcb12e0ffe6673ca234c2dca1541e2f794683c7dfd867bb3614d423ddc4b08219e3bad3698dd76504fbd20371e7d5708c01b5e9e897d628a09d7fefa154" },
                { "kk", "17526a1591b45b16f7d7f6eef5249d430aabd12d4a2da1d3f70b5c303f4b9de424bb7f16592dfdb3622236de006d806ac8d6ff62a2c28051969a0968e4cb29f5" },
                { "km", "6b537d643cad1301f74c181bdea968b4fc5f86eef3a06fddfe7685af32894325f494c71b9f6b706b3dfc4774a3db042348be73f15a1ddd4a87815c94306f1da8" },
                { "kn", "c5d46dbf77d52123e5f6ab0473a753961bec35d26c6dbdc1fd1c8b330b4ba846fac06d941078f50d0507a931894fadb444bb0110e35118c1b38c0731dbd1cd19" },
                { "ko", "1c40016bff293b07a2d5e74e30209b4b83e9a8ddcf3afd8c02aad65453f252f136233f86f6d23c9c71f06455fee0fedb549ebfd5bab175bd5e4e6051caab999e" },
                { "lij", "6acce4d6b1eccccbc0cc210d111890447b21b918857df0101705bf3d1c935702d48ab2fb41f57cb15c3662e584fb3ebb42d5b773cb6e679c4ef2de7d6f9275af" },
                { "lt", "0d3723517f313d9441b6aaed1ea71d6cd9756ace376d45201c60a753a74e3cb8d8fb8c8ac706cfafe60c273cf5b552da3ac8eabfe106a86330d00f1ecf8a673c" },
                { "lv", "de8a511c30bf4944cbf77cc2373faa1cf98f7e45cc2a6ae818acc9b48ed20049bdcfb19a1bed8dd868326679466fd099dbdf0733d0ff39a79ff21dcae85468fd" },
                { "mk", "1f351fe94f25c6bd44426a1253fb513581e9adeb19a7c5e65801ae8330cab90ea91a3d473fb34fcb63e3fd3bc119159a1e9960892a20ea8ea7406e5f0101516c" },
                { "mr", "257958533967c6fc8ee2b27412af1d66706f7b6d11dfa1c103d38db319d9c8ac67378b18e5e1099ad46d49ecba446dc2c2f4374e3c23a86692258e8f4228bda6" },
                { "ms", "d73566ea4c36303e89d6b8e25931501669396e7fa573e1fcc7efd01f09f8b12ba3afaf6fa5012726e8b9f0eafc6423db2c4c3d962b0bc6b8149a743d11744393" },
                { "my", "57b5786de80afee5f1aaecd748e0f59f9d5807c7b8d09f679b3c1e4e79ee9ada537b75286fabd8e62c7f2cfc9c62d7410faf3dc83f5d29f21e2ba49d67f20d00" },
                { "nb-NO", "fbc34e98513d6b7b60dba17c3285d97407296c150afe84f8ebac5b47ad7ce8e0a920f1f37472c67ac82a7184288f5be451c9015393ab54afcd4b15abcc61cafe" },
                { "ne-NP", "d7933cd85e2379ba7f9a44ad329c352a8a32939109857aba518ca85ad3466a56e42e100abf03f0f7a562e41ee6f079414215b93d3b2aad8e28b59c0920c66097" },
                { "nl", "6ac67f05a2c1cb8bd713d34b2b532de41cd60d04d38c32119fab5c52463d74236e0ef0e9e3e207d6a4a1820f5ae3a6277fa3466468e3f01f6fad1d5dffbff517" },
                { "nn-NO", "bdcf314dff548679802bbfe0ffa147ad82345811bf1891928bd9c5f00545888a85553f3940e5e014bf49be759b44124ea672a498aa4a784b0e3de77637e7b6a0" },
                { "oc", "30b48c7870053314af9b6cf26cd845fa5af1c77fcb426d96921893fc569ea10a4d96b3918fe32e771b26659fda7a365dd0b602921a1398da39ec7f4857de6b41" },
                { "pa-IN", "1631c5838cf751e92bf519717197ae8d01e8aa338413011f9e2c0421f12c557fa984c0cc6f35190aeb3aff2a7032b52809b5f51150b785f0c41baf9a641483b6" },
                { "pl", "bbbdc5e41132e8fede79a8775eacb6de4a75b7ce40150987ff65851a09354c4539b232c71eec72e80f2ec60097fd23d7a390f5da127cfa7734e275fc78be8242" },
                { "pt-BR", "e8275a6547203687704e3f6adb0d1d8073249b559bb4f6b498b4a0020002aa169756bb8e7c7db27b357a9c1c74c30d3511c1d295072e5dc8d137a2c2e5ed80f9" },
                { "pt-PT", "8cf8e80b1e3b3ae4ed215c521da3492a239523430db5eff2501f24970c8168a10fce98c10f0aac45b89fc0d8338ab6e4aeb7079953629c0aa2999a9b0bce6084" },
                { "rm", "11cf7ce109433ebb5350c7f1cdd966d2bc91d123d6280c388d6cfbb43e7327873e60e00982abc90838ffa131773ef1d65b96d987a0ef8268b063dd1fadcaa66f" },
                { "ro", "fed9d3ffa4a0bce726c066d48486d66c1c4840d54809561e57f2cf9963c89c3dc46449ff167764047ac9a4ec355b28c4d11ae31a84595f8c0ae2d1be682211e9" },
                { "ru", "a7826090a9ae12921b65aebfe87d214101548d8a7241d9631530f2bbb942f6036d10d3920d47351e86de0e3d7a637c7f158c4cb0838614ee6d8fc7857ba61879" },
                { "sat", "3a3a6623ddb3f4ebcda151404f8182b3c5bd967a0ef352166f77deeafa3209cc3d1d97219ca227c01cf6c49c477bc9e69d419077e8555ecf2b4c5ac2a85729cb" },
                { "sc", "16b966f691089790181931bc4e7ba31324e4e5d620b63a43248405799b6fa4102de35ba5bcbefb240f4295c93ea40556502a5ec63e9234f6ee5ae8c7db1bdbca" },
                { "sco", "c2ffb5f71be15cd4229e47f96f9fa09694dcd1033dd2edcc48bc3c38a1dddbc3143ee536b2a109a85838becebcdd184ff342e13675a5807954c352139fcbe741" },
                { "si", "725b44019f12fa6fe4946d3b4730a82fab7dd11e77c67811e8e0ab85af5fff060f2ab5825781661a93879f912b5e18af25456f485e9d8710bfdbc9b7b6632a94" },
                { "sk", "0587f04d819fbeea22aeca26e61ff370b5c06aaefadab3c680edf3d6c4e6f613ee6bb3084007006d0ee47e16891e00f5a059e474a13ce97c223155984eec8a0d" },
                { "sl", "925f1f28a88ea69d1b80ede82721b597f07c47fa0e2ec9b5e97ed16181c397a1f776db57fa9d15635ea95ee8ce38a8591cb88219796cbf18cb4afa328846a5c5" },
                { "son", "50e688b76613ed049e42423d9a773629abf18c45670e06dcb41cfaa90874ad691b746b908a180b03638fed80e5d7f9152836f9fc73a30e5fef631be4d43c0635" },
                { "sq", "ed5b8d1ecc34081f4b8caa4fb0dda90af07fb1b9e0f4e20d8c9b6a26e787c586e70e89cbcfbc13375af7bf1327a51d3b1b74933378ae0a2677582e9069dbd50e" },
                { "sr", "140908f37bba5a1a23c50777ad6e63eb9a7db21a89a4c843885a8d02b165ecf7adc07b76c295388d595d306a55341a3930a1bee43c353e3f1ff1f0dce0e276d5" },
                { "sv-SE", "e8c040f0dc6ee5310aea45f840925b8f2e53530756237289ab111ccf300202ce12dcd79cc5e1dd7a28817e2931df8d04d644bac0cac2e5f6d90e3f1f093e3a9c" },
                { "szl", "2a84b6221599c6fe798b2fd8ef985443a83524e3b311832cb576248a0b9f93e29802732aea4f80475c9110333a447f1d98efb900810152f5d6cc90abd83861c8" },
                { "ta", "2d8de3b62a334b52032726561298d60a3a9566c7a57f448136d4ddaf072769c9082ce941bbeb2cb4fce2ae3582a08b6997c754785b236d8beeb583f4e4edc9c6" },
                { "te", "cd122e5e50862856391c4be93071e4ddef2a3a2df116e979a928aab34cf5a90771ea7c5785d2d8a2ee69d22ec2d5e8b95ae108f5136d3bdbc1fe07a1a03b8f5f" },
                { "tg", "118d1547f1e824f6b456856595c660c8d6b3358399452402328ecf170fd52f2b54fe918a3ce814be707dd2e7940f5047c49e3f97d18849a151a2cb50c477ec6f" },
                { "th", "91c95f0f0421a09f907c43c294722c01135737189ce07af69b1bf55d3b95a5f2ec089860be9108c88d61d8112e3313dd3d9e605e9171476d5c2d4c32c97b5c74" },
                { "tl", "ec7a934f6517466a4edaa28274a7142125515e86554c8a5b52201ac880db0c9fd7b4479f279eb179b14ec2be1af3dd058bc36dd54d15c45a491155a24bab8cfa" },
                { "tr", "00cd1ab719fc030fb846a92d61b193372e72cce5afa35f0b98e59906034f73f159f451f1feb5876d6b87fc89c1dc99214c9bfaa87944d257652404509cd15eae" },
                { "trs", "479c2571868a5f8f6b77289184421df5f293bbfc0e294a4af7656f922377ea811067fca36107576a8569a3d531cbd14363779fa6cb9b1e050a17e5e0c728e924" },
                { "uk", "138aa9622c64047c6c6cf8ed1196c7bb7880e08108dc78f7bb2c895a970052624a84d0568d39b0177769a5964fbb2289bb16e380d96a70c91ac7a7bec36f0058" },
                { "ur", "4e0750725638839f308737fb36ff366488ab7ab0f2ec7f31b5f16f546ad25d76d66e0dc3c0895d007dcaf73b3448d0fbfc59ae4ad1f1a59dd94ffaea241e25ed" },
                { "uz", "70e77305025fa5f245f1bfd37ddcb3b8106330b6708926cee4d81e1d6e74778efcb7fdc2fc458a3172f0d137060dfb5953548af6f726a8c56c71ab23d19aadd2" },
                { "vi", "1120ce255801c0f5d607ca69f104f1ab3e39d5d63d50dfe1e2712eac44c55467e5e975d55595d56e2cae7a5aa804726243f41b8922c99046b14b8f6d0c7d8b64" },
                { "xh", "1acf4decd0b0a8593bd9f4d8433b84d3322e95a3c984b15421320c3b1f0ecc5da4d0d391ea89d7108ea53462c26aa093889628d4eaea07ad40631cc339bd0bb9" },
                { "zh-CN", "efb0a0f9444bb980fa45b0b881a60f01d4b7fc3cc5c8ebed90f2c3c3ca2a51f1fa39f6b1e7452912b101af07cbc4a74b838efbe629bec0f0b00d63265efc2a46" },
                { "zh-TW", "c11beb7803f3246b7a3eb4d657081efd9cad1a1ab9988d63a55578a64b05ad838a87df90e673aeb5c3f296d77f8169735eaefe93ad2efd1b12936086524b6d47" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/125.0b5/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "77dad354a4a318a4ade6f0bb21995d701af8d3c0a1e8ca50a00868f01908eab3753ba15b7ffb960cb7cd92dd9c3c6f4ab672bc3335ffb6f46f5533f3c8e29a76" },
                { "af", "4ae7d4200e82939a91a5854ec6c41d0a97246d4bb1a4956c39c0ee4d48692b006c576b67a84c47fcf49a1f34bfc8720c9b13d5473f3c39d87855b8c4cece52cd" },
                { "an", "644b717c5eb6fdd5011248df10ed328f3ce396f9f06212e8185299c9439c6f308d65c4a035f689c39243395a736df101bfd3d0ba1d8fd5d529cc56acc1aa3438" },
                { "ar", "59846194bab432a4b11d03a4392d2625f439a185c64c0e49a76f3e923c1e5a5d7ee7a217aa0bae773c2e1739d32065eefaf2abf410271f61ae601f5be8c7c738" },
                { "ast", "8b8b24480ef483a065e3ed1917ecc526fcedd29361e09e1ac385f3b08ffff459fed4eba34807055b98940e62306fdab2410e2921b07b96b7f3a9ef2be50b5ea3" },
                { "az", "b7f179214fde282ab4ceb45de9678ad4335feed7184d226ead8461837d1af7755dcf8c0344ef6c01cd01b80566a05092105a456cccfb1f9b4fb079980fc8f0ac" },
                { "be", "fca9d5c3df59a2bb50cbd5817f9bdbe379d877ef1666cd4b5da3b681cd0ba632c1df4209c4f94c01588270935df99cb0d56dcefea9fdb5143f76b1a064942a7c" },
                { "bg", "f6598db58d9723a5c0d5161dd2ab882f548e943beb4e14e3cf9847d457cd9a54cbf0276df8fde0151adf990f28bbfa718d7f2ac19f9a08762171bb4e995aa20c" },
                { "bn", "d4c5cdc7bd802ed1b70296b98e6e8c6f9aa0daa479e33e266883511a6f81355d32cf201ee0e0a9000ff5771e0176c0d1dccb222bf659f4373f19d3e8f3eaecb5" },
                { "br", "9080c000c3c8d6d01dc07c573478ca253faa92c1b0d624f4139b9f7f7697b812eec0dc632e7fd026e9576fc4c28fca264b81d08ee9f12893045a9f3935b015da" },
                { "bs", "4f8337b0ff6f958bcffd1b8dd39e3b60d46349f50269956da7bc5fbabee34860def26a05e452a0fc581f3be2fcfbda8514e07b0b049eb1e8e0fd9743297f1944" },
                { "ca", "058fc93755c1587738b324980eb7ba9aec94ef9e7a0ac276673423ab8512b0ed1fe889c38ebda632485a3057dd1c3d35a3f519d2003e1b04964be7c717da3922" },
                { "cak", "e01bd1ab1e2a53403e3a29fb65331eea1287cb521b8f8492e469190c36b81aa613012c4bc3266d49530c8535760be55a8a6a58e608f7823e7de633b9e9121083" },
                { "cs", "46814b72277ea3c273f1e8257fb30e4914c6e2bd7abce78534dabd997c8205e175abbf2418d0cd7d5a5536a8cc50a2dffe5109af0dbe74d2156ab786addce09d" },
                { "cy", "e1b976c535e38d08b4e73cf2216f3ab202adfb53471cadc24bb139c6122d8f885c40a364441a93953ac4c148c0bad18ba71a2e160036277e5296353b5114f6ef" },
                { "da", "3106273816759bd6e7326ad26a8ffc5074af1ae9060c3fc51c22d66c568369cfa529dae2552006a6ffb29a9b95cde84615b8c64fc884255add258eb942313943" },
                { "de", "6b3bedbcac3770d0109e2df537850c0650800f0fdf646bcf313f7dedc380b89a557a3ae6361623b2289fc05415953ef70adb8d0f9d4e113a4ee523c31a4181db" },
                { "dsb", "a8224a214a7a5ee31fb0a77a5c1b26125dbd389c74c11db606c7c4a6cc5768e4f3c5d2bc72774f4345194ae1f32afa378c4c877c37d6b892055862590902bace" },
                { "el", "9e6d985a4f72b688fcd12e3199cd2f7b2013aa1c32bee3c08b90a3c16004c60ed112c4588e3bf46bcac777e87da69fa14eebd0300ad1ac62e995a11bf965faeb" },
                { "en-CA", "4daeb126ddb950b0402193df113c0f8d1ea3cd97a18659c5df93c203ab789a97f3452fd3a18094e3184c60af29303753dfc284bc1e8a37756d87cef3d5fc437d" },
                { "en-GB", "ea30dbf28e7751845310b2af223a5d71cc365a37b362e8767bd93cdc3c74b7291b649fd13aad1feb53e9804137e0dcc61eefd1611d7dd37141878c3c83e653cb" },
                { "en-US", "d2067fb9a38190b0817c5346869d705c74c72aa05f307cfd0e6dc454fcabcaa8b49f676c431d3fb0a4a5f7a923ec48ff5ef81a89456dc203c1060c79d3fca20e" },
                { "eo", "6297560edc9691a2c31c52243b05151d7a4908ba2bb815f06f2f3c2247d79aa883c68532e751f95f051ff91bf05047437f5c9fa8b227fe4417ac464687767763" },
                { "es-AR", "9a26e76785dc8d928a5ffa6e7f8ea55118ba39fb0cd0c6ab5d2f4f8447230ebd5a65cf8f6c90924d253e6b2cbb9f7bc6571252f6ab3e4d9af2ca23d8a4412820" },
                { "es-CL", "df77bc825da2d42eaf932d4e4ee8cee926e0441e2169882ba6661183137512b950717b591e83dd236a643b1069fb8893ae2aaeec6fed31ba4983879dc4b0339a" },
                { "es-ES", "d8efb9a2720bf6f4c15f581534e23ed820735a4da0ed2cdfd45e9f8ddf862e1a4625d229348003da5591f7fe3dc7b0812da66e1dbd97cb4eb6a3e55fbc751932" },
                { "es-MX", "106882405d634931ea7e527e0c2a4813a746307cab2c28f4d4a744ae6a7d5e5059469de7ff6ce566f1c7f9d17bbb87a5cbb40653fd51d4c05dcce9c4cc9de786" },
                { "et", "d6ac818e5bd9e94b1d78bc6a7b676f0582d226c42e70adad29b569aec6854dc79eae9b3c648bc04038944262d01b6ba2f4ee684ba2366e32544eff4ea5eb7ced" },
                { "eu", "0697a48f2f5cf45654c0b39308cbcb7454bd7b302ba851617e9fef293c639efe8e7b1d948bb83d3cf82dd594c32b27f98fa6761b4460beaf69c58468e8a5c8c2" },
                { "fa", "5187bfbba341c3a7789e497af329ca2ad222e3060d944e4771ddef5b17bfc8f485da2839249254ccf90c7e30b14a3240e84afcfa92561962d72d79a7d4860917" },
                { "ff", "7e9cd45141862ebcb13e91b70e963d88f70b999c5cfcc8325c76268e6d90f47f02f8c6fe65495f14ed49afabf8de88e1a1efdd96db94827250fc202b060fb14d" },
                { "fi", "3263cad0023cb4cf075b3f20383c6deef9ddffa2f5dd1d67316c3b23008aeef69aa583c61a65052ed4e9bcf94515a7c91a0487ed3a82be6ee139a4c9dc7e849a" },
                { "fr", "bd86db244488a73e8412665d7d4542cc01ce646e5e6a0d3be3099442734bf5eccec021ba51fa89cfe94d9303287c15c55e0d37492e565d73984161aad7a6ce0b" },
                { "fur", "25ffb7ea918823087786b2c486ebc664e36031307b51a2527529d8c843e717584756769e0ca7d1d935f2e3574d77e75ec9a5f78b56cbdb76a50b9a4028c6215b" },
                { "fy-NL", "f62f47b0d2e800bb7f93de5d0aee2d5d913b8415c6c0ea0e16d86ecb4b4c1c3b6b0553525940ba35c15006f07e842cf2babfde6bc80e26f3d144a6b35f8ef61e" },
                { "ga-IE", "ab0b846f26eac5fe55443d02d03f302bddcc236488cff585a71a9b8ae8585d11d90097e50ffd7b818ec5fa79f001a4fb2dd6727018f3f8d853c570e17f52adc9" },
                { "gd", "4ce33970ab4fe6611c6829966398b90770f5bb92f57e4af44018b04da5162e3296f03e1cf408cb18297ddb8f7ad6a90f877d68da294cb05652258868854d8bef" },
                { "gl", "1444a4cb815d7b4291e88790ebfe6d43ea12cd8def500ded38129e52d4a6ddce98221ecb657ccb1915ff11f8c095d3c4b7ece7cd06849422df8882331766905f" },
                { "gn", "51d810112cab40f86635caa41045fbe31858015827aadabcb560148c13447a20b0d2bcc316915f8aeef67c097032974431bdfbc5627f63e01012b92d1cc3c8bf" },
                { "gu-IN", "4447e3e268f055c0d10034cc2982f6d9a5c90eea51f78a11734138d1af45d79d2dfd60fe7bcdef23d594e6ae43f5d790d283f51bb96377fb4d889dc485742637" },
                { "he", "076b85bfbaa907ec17b4eccd0eeeb9e8d5e0fd107cc197dea902cb191a29b132c4e58eb4e4d489772edbab6ba9c47f752d2a2750fac059bec2d41a3a6535f323" },
                { "hi-IN", "907347b3e6dfe1ec5a91e6dbf2c8e709d7075d770cb3b07b08d6f101dacf8947e37c9a50dd9ec925fad2bf64880423df803474cc24e588ed665f3cd63a50d56b" },
                { "hr", "f29f7ef4b685a14f5dbc2233192b06b55e53f606bc474e6fd33b567ccd9ee24aa4f109066c8ee5068f432b18c6bfcd4c5de46ceeff1f57f319aefcada414bee5" },
                { "hsb", "ac2e6bca2183afaa2917390e17ff8216cebf4f932e930e677cba553563fedd95de2f5d11e461843d384cfb9ff360d8debeff9c882945295d5bc4a8a5f636e432" },
                { "hu", "9e16d1ec4d90e7142cff5012447811d5fee04243759b405a27be710af5623be076e6d32481b6941bbec88d119560952cbdee37d8cf220ee0fabdc947737af466" },
                { "hy-AM", "95e7fa14adcf6268fd9ebd15d5cd3956f530ccc0dd9b693c4096750632639f83471b0fea2d2c53b963a7eadfaf0c22f368e3562fa24deec73f5b7985db05eba5" },
                { "ia", "1378571c581217ef99b3e17b20fe49087e388dcb43ae76d558924fdda72ba39cd76af17fd00fb01278294476298018882f1a6e7a01f855cbceacc4ac0a501875" },
                { "id", "9f6558cc20ed204f33feeac54b330fa306a45874de57f1e3f7804761fbe753bc8223c2b0837304c70f0b119b060f2b2125c291a908a6e71d8a9f9d0edfca8720" },
                { "is", "78a139969c6a856ae0d46725a836245fa5709b054d670ee2668b9244b8573c56c30af8f3376dbf584335c27eae3c43f9619564d0648c048550a86c10ecbfc713" },
                { "it", "19c0efd1c8798f9644b8bee49e1f89b37a0fedb1570a511d93fc6844ef0e711289461a759dab21ca09177bf58e9183652ff8defe0dc69b45fb1330c66193f578" },
                { "ja", "5325be31b83327273f7d9984b071789498510948b66d1f8d4624d2a2facabde66e273454d5610d1c93dfc689d5b646874cd940757622b5a3c66e80e6d3766649" },
                { "ka", "f623959e44ec70b51241f9e71ab52646cc824f3e6ccc938488d5abf9064d77a20e900de939a91e02fa956c342c81668e4c090fd33ff3b682cbce98601e05ca4a" },
                { "kab", "72bff71a1a0d5e11567ace8e5e4f4f93cb4c99766080b8827f660ead967925bb7e7dde59177e91aaa309ebe794d4345c31176d91ddfb5d791d9f1de2cfd8071d" },
                { "kk", "a937c2a1f4d911f3dad38923e5dbe1a4ce9a90f435d81a4ab52b0109dc5b44a70f00de9822a1926dda3a4843870c542fa94f955c703fe6c39a3749013f10ea41" },
                { "km", "62ada16b0a6516862361c078d5d097359bbe0b05ea1c3c16f0d33a375fc0b83864c712ab24ae4b027b0f628763b1f1b9765824d408263b5852691c674682b2d7" },
                { "kn", "cbea717eebedcf868eeafcc35a8c162fe98d41109853578ca644c6e6286b69e59f4b9369737940d0d787be4aaf89536c12557dcef8014c34f33452eaf506db69" },
                { "ko", "8c17abbcac398537e9f92d5535c4421825dadb4495384333e0fb017310a59d7f12243981cbaa126f9ef6bb4433e856330fbbca78ae444fbc4b1188d7aec68b08" },
                { "lij", "4e753181b4774269fad605179724f00a8eba51e644ebf11f3eedec3856cd80e0bf5dd93adc009a969a17c38baf001f4c676a23276f69d0bb00d677d804281152" },
                { "lt", "8c692e3155d3a10b289df4d736447dadb7ce25f5bd58e518d18754a1edc13d2201ede0bffedf3cdfe9cbc11f4893e3a1c9a8e9881a9490af8147797049ea13e7" },
                { "lv", "fd4254872b79340c65288c32bb3b6c325b0dc5c2f98a1463eea106fe5b976a3b918522f18067ba23eb2b9fdb141c38b9931b424800d05936c4972e74f7533358" },
                { "mk", "4f30f723b9a0117f7c190a4dde010e572a11c04e46f35299232b5e1240d2407f2d718b6b57e25a4007f63293f1f29fb03ba052a8363e46b4130282ca7664b0e7" },
                { "mr", "7fdbfc5a0d22dc5ed6e04dbae71d0e822a6231e02a4cf090674dc3f7f0151fd68ce645c462f59b08721e9d593dc43c860b57b67649f398fe9d78d679a31128fd" },
                { "ms", "de2d741fc48d6a2fc7afb27f854d7d05a1c77c2295584151566af300056aa713535fc4b346df55930b34bcbce176af5a85cd5ecb7890d5fa91b57458437779a7" },
                { "my", "a418fb1f94bb7629997d768b77ab68e708fa3434f411400356edfe86a40399a0f52be788db4b3fe63dd5ee4c347d7704350b83dc3125b0bf6183558a82a4a599" },
                { "nb-NO", "353b46d8a1c6cc91a32d6c91b0603bbbb3fea2327cde86c24e91f187f39a59b6befa06a2ff9b4cb81d2268598d5066f5974285f9f2f3dfe07e5139f8a3a46070" },
                { "ne-NP", "a1563fd76fc721192b7c0619df92b1a46860fd3859d8e62cbad3f28efab4187ff1c7205a2dc2c759c7c40c71470f620f67183fe30ad0b9ae27163c678e37b2fe" },
                { "nl", "28da9f87e880b15b5059080a142122565145e98d27151c2430edf5f0691f75966174bebf266fc9cffa4dea19840df3aee481f9353ba96990e6bb2a63fe8ac6d9" },
                { "nn-NO", "87e0c903a5fbc96482c1d2d3d80a3c5b1057b2da70074e2811159baf1c0bf43fea9b0d3eaf95113b39359f6da9ab40157adbee2ed7650b122bacbdaa93335857" },
                { "oc", "4df0725a330ced03d949a410d5564b7633aada4ebb921ca1d1607c9bea7d8f3e21db442ff1d2b9cc82ae4a441f523ec2267a57b605081776d034b1390f6a1aa1" },
                { "pa-IN", "16eb5aa5a464b3468c5756a74bb32c2a9015578d8adb33b9e49fdc1c33bf35a0fb4ffc78555a84e3439c5dc084ffd2bf0c788af9b559843682e46bc22716225f" },
                { "pl", "412bb506c098282eb5bc21dd3c459572ecfb4c01d070b833ec31846f198a35b9e2b35b7ed6dada0c5486c34f3d0c2dfd2f0a5f9db59aa5a2c2123464f5988d85" },
                { "pt-BR", "3508a1586ce66c74cc8b131c18dda6a2defcf8ab52dc2a565e8947a43904aebe31e9023f2af510e3292a543494ee3280e8de2b64cd650ae6284811006129b01a" },
                { "pt-PT", "cae946ab3fbae3888a7a43e159e3d2fb1e863cd19d9f5d9cf605832e7f64571f25a6e1a5194564957ef2a0e48e1b0ae1a4e8cd7ced523f7075612ce8484e55a3" },
                { "rm", "581245695c92f231d60d640266ccc40eb85c02b318fa121ce806fee42031831e5c6b1f66a6fecf5e810a623381ab32aeb5b395ca647c14eb3b71fd61f7e4690a" },
                { "ro", "59734d8d036cd3d4a6958df09e1e257357a4d26c0c919ab21d315ed8de224eea11e2c982c0f77674e87ecb59d9b5f36126b92de2035cdb3cb44b75c458156032" },
                { "ru", "0f80650f35c923199e6cbee9a3fa65ba09fc7a9fd51d89e7d22f746249856888ff0d981a6cc523d0bd71b4c476db03ce5b46c5aa3403880fb647ce1027de75a1" },
                { "sat", "ba2a9bea948bd7247b4b1d725a5c6cb955dc68f40cf856acb5f20286e5b10530b7417a79b444f28430201324b386d9513938b938a9227ebb2f78ed4a5e3a1cc0" },
                { "sc", "6b52e7112b43d08e657e0948e350fbe6e334f54540a25c17810ec4293f39e6dbecee7d6062cf11bea4fa572999b1fc5fe816c700f53e03b9d961142186fba85c" },
                { "sco", "84e9d96ece02894468395e5d62951015cc52227bfc91ee5fc601123642458e4cdd46937095d5c7d5be2da848c65bdcf02e1c790f47cfec5294064ac6173bf57a" },
                { "si", "b57d949bf8b8136848d03a71b16b17e5d2aec767172f9981f920627caf114ba3fa84d841549a401a8bfc0ce1ce869d10e9d8a7b9ac8f7475c8b935005e92457a" },
                { "sk", "73a11d605740243e7d4d42eafb724d916ab26f4eb0ae4173f747b311824feab97f69b3932186bf2afee80f5d7dae77637a20cce37b63b56217266350b60b5895" },
                { "sl", "3c0bd3f53ed7e5735788940b6fabcde6cf81704f2a6248bc4758f33ddc0c1ea8e30a4a4a19b5d66190ba962bf91a87586ff500e79ee91eedb41a0abc4d952782" },
                { "son", "50068ceea5708f9a60ead0ebfab829decc16992478fbe26fa08c97bf32d1c4df3bedd3cc26d0b8914f544e4d985d671c011f63f7e9d1f0aed5a3e657010ce53d" },
                { "sq", "7918a2ddd3d30deb7eae9c370d60c1b1d7b15d704e122d973f0361128260f7cb25dbcd865d9ff53f23f87df0cac87f34abd02c5948f825356d060b9043f2a761" },
                { "sr", "b169c0f6174f98e46d1c7638b4dd71894150d7187100f1bfb8d60549fd4807d4a69ae094f0df05b4b342dce121109c5fdf2c2225615b6c50a042136cc3654a8a" },
                { "sv-SE", "606588e74cea99490fedf8c755865001e5daabe9748ba920b062e69b07f6926989acbfde68b15950abbc1e658c841478867e35dde7cc56ae2b557f09bf9d4aa7" },
                { "szl", "622ae4d952c4bf9bdac59428392ae24d1790dbd923fb472f8996b25358a2742448ed3629cb9b34c37404ba70dbb16ba6084fff36787e0c6b9b7b2616167b02b1" },
                { "ta", "226fc2f752ee456da4ac40e18b7b8818885e67ad79b838b957c78ab7e43d4b2364cc753878a7d1b52df6330cd1e7a4b9599d48aac1208e49e9de5a90bd40f083" },
                { "te", "46bc9c53baa7b83f848e819db7a8b3f54aa8b69ff00bcc20b8451fe94646ac03c80a352f39979686199c01de215c2f9c59dce8e3fbcfc1b2451d96dd8f7d6ef1" },
                { "tg", "2f72260980d681bf7d217307141ad3a1f6f1b70e97bd2ea07856a10d0e5618e57778b656e0a44c741e9518d57c4c8c55cd33d8cc03a85ba7cfb447b9701e2d86" },
                { "th", "672aa7e22b80e4fc9a4655ba2e3e6a165db8cbe5507dca2a0c579298533fa18c68869fe1ded7e267c2bc363d74d0a262cde4bbc2492abb6c34490ca7438a1c5f" },
                { "tl", "436bc3e9d54c5b92195f848b62550442d446665d1bafd781cefd9af146cbed75b27e7e05658b0d58b1fad5805820f98b266643dda68e5b384f9595a503c91781" },
                { "tr", "84e8247ba6c91d2a6e2e7a24880bba7715eea6542963a15f69391fb952f5f855d722bb273ad8dfa5806958c7e2a14fd9e4bbd7fe20ef1f9d5212651c7b75cb88" },
                { "trs", "679fd6fea3515a71ffac18c5bc8b40111ee59a2bb10ade733c5a8696feb26c241532154328401e54fd0f6d0fd14f61c7f9445c471baba2a34ee2c39c4476827a" },
                { "uk", "46755a5e104564ead27804feb7c1b8a0e0bec48affb727aeedc833dc7aee0a5174bd97771fde509d635ac45ac97929d2a1f2e4f0dcafaa37e822b1e23fde298e" },
                { "ur", "108eed5c078c97bc94ef444a0bc4a702bc177de94ad7cc4cc1e94be4496017302fab12c05aaaebead44bdcbaf37dfcc58955b4b907d29fa95b2989e72f075c28" },
                { "uz", "a9f3db90dd9cf6954ad7ebe40239d5d9c3e4d38f2c8934c5adb02573dfacf3bd40e47d7dcd77261292aa53126a70c56db39ab1d0e5c8585da0b82a10c43a4aaa" },
                { "vi", "5b2c8513e994c89fe4dc5b3bff960c37c324afb3db25f012f551c6f47fa56abe90904bec5fb1bc3d109cffe5cbe2704ab6adec4904f8f8af7ba0c3b8fa3d8865" },
                { "xh", "8624f7b7761c85ad04f3e4a5684fd1fb3f57ef307a96f3bc2d1a1bddb2270d3c5263e062b870c9b5ebf3a01ca9fc2f42475910b3b85d83cfd915f1caaec99595" },
                { "zh-CN", "9169a7cad7ada52573198d5b348809ec807f251c8372a22ead6f5f1a886d6f76412256a566d96bf106e1d0b31d93c33ae0445a380dde281f48f4674d952bfde0" },
                { "zh-TW", "23aace2ad438a017d4ba4d870e2d3d2e2d7eed09fd78a46af53f7013fb1db1d2f37d625330713df4f9c4d2ad87a8851a01c1d5627b346a1bbdccf9696dfc88b6" }
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
