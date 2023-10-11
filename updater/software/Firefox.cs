﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022, 2023  Dirk Stolle

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
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Firefox, release channel
    /// </summary>
    public class Firefox : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for Firefox class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Firefox).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Firefox(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            if (!d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/118.0.2/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "00cc71d8e1d43a33598bdacdb9e850176a6da081e0dd660a281200003fd170c7ce1f3e3b189a045444c5737f69457cf1fb6fb98e1ad23953e22a4e54ae9793bc" },
                { "af", "ff4210e8534fe2c19e32f91af13002e033b409f122205726089f2b75724d7dfee86ea19d248878ce5a2e868aa2c547e1f9f3e18150ec989402197a9d2f94d5e3" },
                { "an", "505f273bd65e58e27be0d03a185dd9092864679b62233951238e39ff2816a77c413a90d7207fefb20ee647fd706c761e1c755dcd6fc2b95a24c835527d108420" },
                { "ar", "f2f1cd6f0b4ee46c4fc435a5ffcb01a1e9b63d18805ae6f25d8c3055df9f753501e320cbccae3c050a937f1614347f19190bc3fcd50f79152ff15632f3e1aa95" },
                { "ast", "0bbfa3cc8a9ab1ca12fb64d412270be88cd18852b715cf118e44481356629feaaa98c95c8dce9a511381296dabb28528b47e0a275348f198e0c7aa071f653928" },
                { "az", "75e180d4bcd7697d3ba73f1ce725dd1dd3459e5dbaffc0d82524ba946dc41c9956564f5586372ff6a14c2376f81b0805d3708e45cbe677189427eac67f9d2765" },
                { "be", "b5e7cc644a987e1b87dffd803e46e25d1c6a8d4c6cdf08e350a34a78ce3eafb5cd8bbc3f6f2dc742b09c2dddca3f2c565ea9bb42272c5de2a96c01abb43cb4f9" },
                { "bg", "471c522c4c37f62fe8912bfd8869ff5cf9b3f79e20c25bf7b50d2419351b274489c1cbb4a576d3f5ef6a044b220292f9ba741da9c8cf7fe6b03239779156c20c" },
                { "bn", "2d1c0eedcfa5bc96613d30e20defb8f29f085a38e36c4eb1a0651e878d7c250c3b9698bc7658d7188332b808508c96d4df280e5f07b56d0186130496131b54ed" },
                { "br", "c538288acb7b3a7f5006162864c5b1ff7d1784f81f7a67c3affeb54c2de845d6c95a70ae177c87ebb49d887b966d998206505cc3566f1a4341a4dbef60fd796b" },
                { "bs", "9ed3918f8d8562209a8385798ec9e7c768994b5e141b62af469661e8a37f72759f9f3f28010d920e75cd5d77c20f8ff9dab5d54f8f794d87bb4a858bfe4b20d7" },
                { "ca", "04497df444489e50e04676fb42f35a91f09d1e846caa81128a67661f038be810979b5d144f63f1e8a641f798d97e1592a210789f01f52701a47ef3cc95a1006e" },
                { "cak", "7546d6eca0c6724987b945378275ca150f23c889cead10c8fa91a36fbc4dcebf174d6a2facf13df988dcb66b4f3ff018d5dae5899ef16c9dc67d388783754501" },
                { "cs", "f96e508541a3906477f398bfc9d625ea99e17d5ff8cfebad11084c3fafc71d6daa3ad390d2e45f5359aa9b55a02693ef62d9f3ea3e8cd5505ec553c97df0cff1" },
                { "cy", "79e92046f7a911778209c76390d171a9388183320adab96d60557c5d56d48983234f1a589fb188192cc4c6f7d9a637223f3227a381660ce39708599fc60b13be" },
                { "da", "d0717c74447b02ba044d728e82a2ff45abafa8892d55c5dea57e75918923e4e07bed7ee04f02f1c04bf9873ac37d684db26c0af0cec5668e591f962c1a9e13ee" },
                { "de", "aa9c1de4e7f73ddc8539d29f7668ffa09cbe1569c50ca8fe88b52521a6e804fcbb8f0b4f2ac3c167452adae5da027f6329d3c19b9347cbec263ab30640faedcd" },
                { "dsb", "83389c1214b07575febd6b6a7fb290d23570042eb690ebf8c2ff603d7806f878d21b2ac9f583f2f13f9bcfd7e669efeba5db434dbe6183e6d94532836d32aabe" },
                { "el", "a2162bcd2e12cbf573f75fe30921b75cb1129b8790b151e974bcf28978a16f75c060db3e142eb558518b286b5d907512cfd3e7df45dd405e3c230149b59e1fe7" },
                { "en-CA", "16c93eaf0472db4ea297727fd37d292777338b4d6afd747c262d65a16a1c753de81c2abd65b2860384c5eda21ca46b59a73557b1567f765d4a4f0a38c0269461" },
                { "en-GB", "54ad1952baafda3ebfb797a88eed70d1591d182349f150aef5d5399ee61682dc8476f1e300eaa613e020911b4e09901a3374c413530516fd63e30dc805984e23" },
                { "en-US", "83edd443637ca760775affd5f5452bc0b0b5aa8c5c35304cf94eb2d1e6af2687357c303ba2b2d96066e3edbc2375afb429dbd0c7d36c5b98c86e6fe500dd8298" },
                { "eo", "e19a53b2a21206a78cd6340f6adb4b02574be7271cc38760ba3d2f68427e8f037d82f135b6334b254efd8d326e992da8f6bf3b398b09e71e827bcc93872b39c9" },
                { "es-AR", "8e0aab5ca91727ab3837b52d969a1b8b022dbdeb4c8554e74d10923b8bb49c1175e98393e1eb089b4b10772de4566326c12f1fa787e25f9486e47c7101e0d38e" },
                { "es-CL", "a458815f7ec8b88199a960928ba7f8c04e6f42da3bdf31e6282489b1a5ad2fdf04babbf021df4852f56b74c78bfdffd777b47d58990652761a82db5de2bbb37a" },
                { "es-ES", "fcc4058479a1dc601c65c79c8faac8d24382a0eea95eb8c981749f4f1b42d392d24d5db005041d260240baa11f03f602b932c3e9821bb57de19b4d1d01d5f17d" },
                { "es-MX", "cc4f99d222c1d09e52bf8975f92d06a3b36271806ade84ba1b3a3ee86d77a9342e9072914fe40dfb4bf2f2becf2651b8c06c45adaa0934c344c8bc4784160c32" },
                { "et", "68abadeaf3b9a3fce26d780a3051155a5dc85f22f42b4687d90e492d226402530b88f453a311391f9a8f55b9ed344c5d387d5f71536cf075e44440b8cc23c8d5" },
                { "eu", "3958a67261df4bec7d660df446d115da67fef8b6841c2cb928b4e532d1518aa068d1a59c3f1bf0b70a5971708f9111a34a143134e478ad061d3de939c61a7e6f" },
                { "fa", "0bce3f0ef20c6ca7cff7284ffcf03b3e93ae68676eda3c08781e7f3e0c6f105b50f0e85543ffaf9bfae943014424d2ec2dcf7dbd3da0bf0fd2e53ca5517b6dfb" },
                { "ff", "9f8feb9b0a0653906a1524b96138584bc618eb2f05cd9b35b1c326c553e05cdcd36107b0a54b36b549d0eaab14d36f0108152621cca34ef535fff04def5d7cd0" },
                { "fi", "14eac45c31cb5cdaaf736adb5dedaff50fe312a18066d1d9f27556b5d4c8f4f886140b805b2d9d5454b6cc58c51a92b3b35084b3d73b89457e345ece98da7ff8" },
                { "fr", "b090b3faeefe2b12a1cf96fff5453e8479cb9bdf10227da1c0f2db555e607e7d50592f30710fec1a59edb191e33d884b1ac46f9576602edff4ae3c66d50f44e1" },
                { "fur", "7045685d6d9d90344dec59d06a30cfe7c85d5dad5e9d5669a1eff4d1b9660b20dc8883cf7d71ff5bfb31047063a51eb1bc209801a6677bd770bbec31db133f7a" },
                { "fy-NL", "a5af3af2b56ac9f1b52086e56aa2a5a08f5e692bcc0115cb41a8c64e15f64d2cf038966610f30bc71e0eaed93b42ea1e504d453cb75a32d9227e92a1ad155595" },
                { "ga-IE", "9cd1a83af08f8285c9552957cb0174c95594d702972b0ba7002f7163e2325fbccb2e42e0f034cedc0046940270a6a477aac186326b73e72e4374ce97852b3562" },
                { "gd", "ca43f82a3f914a4c1773366096c0046b2a5ad37beda10d4a9f1807658f217d29cb66d3ca61ffb6acaeb119a7f8ef645c45a5680ef7dc5ac8edbc97653e847dbd" },
                { "gl", "9a55c8e0dc3869e51b310075d2587fb8c0bef4e08a4edb9f8dc3c1b3bd2ee9b10ab1ce52b83defbc033e974ae767a9f076b673c277d665b9c1ad7756d04e7b35" },
                { "gn", "9c3edf7db58974a850d959a75726ea3bc1628a1112cb2afd82d2a88d773ab03e328d32202acd016dd1c63472b8cdcbeb3c86c853f021cd33bd469210dd788b42" },
                { "gu-IN", "87700e7f0daa03b110513addc2e57ddee8e1ffb4ede3eff24f7092ab1bb3e4a4f8df6c331c13abc6037eb5f9cca3b2e58157ecd15102b52d4a8923c730de9f8b" },
                { "he", "3be15cb133ba0bc618c756e34c978040e4b2710d2a8299325ac8cccad6cb7f30b92624b1420c3715502cdc5f0c4688aeff3ca9dfe680f0f87dcfbded0079a395" },
                { "hi-IN", "7c3e12f7d8bc65b237d400205cda71a72aa3ff21d41972f7e7e7bcec23c2a2d365dea08d462c5a46a5cc74933ed57b6a8178e2cc20a50ca7b67d787147257ec9" },
                { "hr", "8005272c70f623ba470affb76e9b6bdd1e3b284232458c4d41a83ffd05703be07ce004892531511038a89b0906ca2e1efa7b42017caf6f7abae500eb54f897d1" },
                { "hsb", "a1570ea0d7337ed74b34df54ee2fd0c8de59941292f6f7b7e9d567abcf9213f708851ed8a702a009100d8fc052016a171de5ba89ffc9659ac7d1c2e786e21bdb" },
                { "hu", "95d7951570b549b489334031bccf5f2930eb2abf1cba0e412316c3df16ab7afc3aa520b386bbdd1eb81dd2ec6ef90f46f90d778e91ff3f0d1e10dfb2c69080d4" },
                { "hy-AM", "6317e591f94469285a64ff31b698dea6bc1e4d5e5eb6270815b2939bf93ec1467f22eb258a13a61603e47d9b6229e34c79c08ec2f7caf20c9e97e3a904d0cf2f" },
                { "ia", "71c328e9c7bdbc780af16fea1ff05dd1c329930f7065377eff5200cedd08667ad81bf35dfe38ddf62c2286192f9c7420c58bf2a3810924d622b374595144e81c" },
                { "id", "2b89f7ee66e64f3bddbb1d5c6960bd4247be45a899da8e21b91228c139c9e30844bbf6117b14f5b84e2d59d8e9e29694ff7cc396cafb70555435b9cf4fa6a4b5" },
                { "is", "51da97c95343bce1ff65cb242b9ff86349ccc1ef5372ccc6448c05ffd5450c88c1d9f93027af1234c031ee632e63119e09b88858964735465937fe0718c5f8ae" },
                { "it", "3cb44d585adff217d7b26f72f09098e32e991e8601d2de7459df5a1dbb14db53ee339562498395b3b433a8edd2b17cec27088374d7ddda470b333ec55e3127dd" },
                { "ja", "c428eeb664a113a58c62f1da762e614daa4c9249576980d3b8a35c8b6d63a1779306a78bab3d6ba50c1b27e9b493b7277b9a401eb6ce600271c8871ba2ead73a" },
                { "ka", "ccc9b40bb256f91cf368323d2854d2108c0de4c347669cdf56c0b3bfff339b2cc0ae500eb83212e35c9b1d22d868bb08b284ac2989b5a2897d90fed42d3fbe93" },
                { "kab", "e77fea0cf199e902927de7d6fa77e69c995437b845a96e2915728d64cf98c9843b8a540c4d030a9cdbb5b0f04df2010ba956a52250bce519a76182c81e5f21be" },
                { "kk", "ec86ee13fcd1d3e73de2273359358ef2fdde4d7511c2f3151e0a867a92365e9ab2f7a7cf0d02ba9ef61c75e05910fcd0c75517639792094eadc39f813864e599" },
                { "km", "4cf24403da690e4e3794df3b1fc00203554bdb6034176bbb06bd11422da3abb3acd60ebd04515b37628888fc5f4e9157426c82f00e2c4f4120939c2b182728a4" },
                { "kn", "ffd8dd5fec58c8587148ae62b272fbb92d7de311ece97026c0580ac3b13f8d61ddbe3aa0078adec476b8ac8364f7d9e42fd0beebd8ea159d1e2d8170a0989b13" },
                { "ko", "bc105fc8af89d8643bd94b1fa308cbf841b17f25a16f7be4f213edc1d7a4702c07f22eb24ff410b9069e76b5f90a69334dbe3fc3a92bb0ddd67fe2514ae47df9" },
                { "lij", "7ebdbf05e1a6b75e5bd359407af8ce55a6206a698d1b7966bbf5a4497236b3a93f93c38d58c93620be906d67c8b2bb3218c7f3e415cf4aeb4bf9cd423e438721" },
                { "lt", "9ba029e84c72f9c9348b0ddc77bcb3763a8bf0cdec09e3bd35e7a8474ebf11e5be808a13f17ff71156965ca8077187602dc874524ddfa89dd6aa8a5469a09061" },
                { "lv", "d408f83f3b795fce8f2e5d2199bdb32cdec7fe7529c12a3e62dabeee94765f186b4456d075a8975456736a0e1b53827fc51bee01388de2aa500cbed4efc7e031" },
                { "mk", "a5f71eca8c7dcf78229f1114bb4805f5f92708f025e176e44e974a32bba28b6f5452630b47a4015106ebff91addacb647263b52f84d481b5b547b899e08a2e1a" },
                { "mr", "9419cd9518f80d40d4f8a7ac9fe5d2083e9fedac0ecdd7679628912169b81e0304b4cdf04523eed895cf695796beb803fe56cfa21cd0d927248e594a8d7fdc12" },
                { "ms", "05ee0ed0f37186105697c8f4c6e5f05b8cc414babeeec73706fc7c85164a0abb2e63387e4d1d9ed44067646519103d8bf6f3eaf89eec01a0ace5dbc208e8a030" },
                { "my", "f38eaf2c1e6955c2f99555e60f01bbadd286ee9996af7020f4bf6f4ba712850a45f2000c38ef796916ab004fb1045d727e8b1e136c51df04eaf665128e7c01d0" },
                { "nb-NO", "8b9f34dfbc42ebd97879d70c5f22014501b96bec9fa267e149f82e23f4fff457f5af15dbd2c9e13c9c43c072ebfd4cdf9ea9f28221e59750021388bab11e6255" },
                { "ne-NP", "b1173176d64da766d66c434ddd60d4e3c3f302b12cb163689fcd67b15b6c9912d68d00de9c4d65ca95a2691e9da4b14df30808f98c097460bfbd4e048d9c0b1d" },
                { "nl", "be749efa83f1de0713e24e476d578bc109c79f3d972075da70a3a179088bbcead9936da0f0b9867367bdac675f95b56faeead26bcdc617290f87ec0d3c1815bf" },
                { "nn-NO", "3edc1d1d912d0cc1106dc8cd869ec55fb06df8ff4964c8e588ab67b436d9c674bbfc18d674e3af9c06b0e0c9f56f8c90774e482da798be914dc21c7e30141102" },
                { "oc", "c1b490aae4cae984c6b7bf59c735ac15faed7c58f5b6c4f82f192cdaa4c71611b1a652001f22bb2c569d1ecd23659fe2f378a69fc5249dae6aba21271097a5b6" },
                { "pa-IN", "a2b1e8015fef2b893bd5f5f94707b36ca8994455af73e16de88ea613d59fa7721d2f6d5960a4f0d77339cceb54dca6317507cbb5cd4229e61eae2a9243057154" },
                { "pl", "1aee132388020e7f64f815c1f3f9dd1fafd4212181725978d246fb9b6cc8aa0d055cd118a8f91d42416bbff2b2a3079b0e08e8ca02ac61541a3091efc743d2ea" },
                { "pt-BR", "181eac8a9beeae178b68992cdce455ea8d7eb1951be8baa62895898cd20823ca1c18e8b915f883cfc328354479eb2e667bc7f03f0a0efdc846f6a1e694da5d9f" },
                { "pt-PT", "c361032fb95797ceb7ff19a32c6772aca2528510ff552537766f0c881aaa19b19e579b319a7e9dcbc0cfabc8de54df0cd6d824c68f429414a75de9d63bc1d573" },
                { "rm", "1e7741d90346da5311721f7e85246552bdb19268f176ae23712aefa0c8bd4bd97e6f1eb4153d7d8714e28fa361d3fe57984df0bc24811c21dde3d33c7edc6258" },
                { "ro", "5f8d2039cc0b3e5aa1bdc7a1e01e25c4c08bbaa8c555803a6e8957f6dcf445edff724e98f1885f653d171a719a1c9c7dc02a2427b42b204c39156e71f6717405" },
                { "ru", "b6d44b23c0f8e9e4504afbc66650984e3abe89c50d53e8052c9bd90a6d99c59c5e3506b54c02dc5928f099190086d008c5d370a31ac67dd01c139f74b39e0631" },
                { "sc", "2f26e7beada3815daf41bc255049b68d2ef4ad01705006ba749aa007cc8e0f0f515cec82ffe2688340498bd736dfb1930e238e20c434de63241643c78480d85b" },
                { "sco", "9b0a60b3fa7c8c366f9ae8558bd5d420d64938331284a791592ca84b42678492cf43fa09341ab680342130bcb14da5270a0e5c02df862cd53d00243cb14a41ce" },
                { "si", "ea12273ea9b62e14dbe63796373bd96c4f9f5ad9383d76bf1d1e2e682519d5acb3dbc6eb2846b6b264ddbf984ccf6930d0f11ce66aa6af43e3c464a7a39f8bd1" },
                { "sk", "61a3d64fc7cd631a084a1abc37546bc8170ecba587c1d2bd5bf828ea708d8dc8c3396b3595964155b586583986492c58ea115c46f7d0ee8a2b85f02e049f5ab7" },
                { "sl", "1497fc6784201db9972a9d06c7152ad01703b47a4e77074afac1cd210e2fbcfbd1859b50bbcc8da39648871598aaabf4863b3fe9c64ebf510bfc4eefa168e478" },
                { "son", "353eeea87633a42ed0bfe2fd45389deb208e9f6ae4b2645ddd756c048196e8fd28fbd9d9314173d1c1a165bcbc90e9c0c0888b365ee6cfd74241fd9e22944d24" },
                { "sq", "6926de2138a9f8c20c3d7dd2887c517348c5f52df2abfb4fa2811687d13a04c572f1227fde4877caff4b58a3f19dbd02e1026211f9dcd65f47bb90fc57a59b07" },
                { "sr", "0c08c372baa99b8e5d684df53da1ec7d687629452a27f3740a629760388c82d2396acdd06777c4d92952f75cb618089b57af411b7559ecee6b1f8170382ccd42" },
                { "sv-SE", "bbc2a5bf92fa93c4e701d61d16fa5cddcfe8b0d8c43c664efcc7a7b423b8111a3d6a56954f71033a503566569ce7178fa8ac3b049409163e029a63c7f64cde54" },
                { "szl", "93f6b56095c4b7d60265cfcbb0c6977ef9f8b318fc0a74cb0dcc4197c2e3843b0fa6c69f41da86084cfe926301773f3dedaba1910cae6ad35d5ae98c3badbcd3" },
                { "ta", "b030b2e856e9d9c2fb41af7f8dbc9a8186158ce3a85480cfc724a717e21a6ee99be62dc4e6874bc0bc15ccee9f1790c7a48ef14ef6cacb356f57015933ccb116" },
                { "te", "43bf5b6c6b7ddcad3ea4d6ba8940ffeb2e2370d0ba243e254c8cf1778a9d8a9a288acd9c492d83765354cb78d65c3e86e8a0efe17bf4218df1146e76bcb60e3d" },
                { "tg", "7976bd394029eba601f3a2c6bab79b76d65733f347db9528ade80bedc0c8ae299e2c8f24944a4b613f55f7cdf568cc407050ff05317659ab3154a3db404fdb9b" },
                { "th", "f6188f12deb021994e1f4bb3e00becc320ff9270188a09b417b23a82144f0cc4b53e52b004c3347d260b20df92938e589b8295666b6eedeff5c6e5c42dcc7c9a" },
                { "tl", "7501518a3a36ad6d9d091872473a6e8df73affbab87f298028be8288736e171cc8aaec1cab67a1ccc1b3bdb5fc7a0bb4554c84c757dd0a90311983dfe136d4c5" },
                { "tr", "472a12adb8a8ec7a18e6c82a1424300e1af3e611b285ba397079fe133fd306d998258f449a506920f6f833593f5ae8050a126967f6fdd15cd6d633dc9f30944f" },
                { "trs", "e51ab96a2b52104d170b3f54fed7945a689c2405dfb05326b7cf93f09196643a77d6c1c9ad44c627aa0472fc7775c8f5057e75f786e3fca79b820487b0da50f1" },
                { "uk", "88f2845ad2e99ad5bb8f74f6a70a872e9e5bf6b1fb633d9dbb715758ac338a53216d9c7053387f28ec45a34b21c2d8aea8f44577d81bfd6c6376fcbb270abd7e" },
                { "ur", "4497785334dfcbfed67ea9e18046c304b56c1bed1e834a5c8a7f1b7831a79dcdff90f0d8e8e96c0019d624f2cb7ca8fec056270cae227793c8bf64284e7d3fa0" },
                { "uz", "28c2d57d9164ec60a3bf33030da84bbe1fc94f0226b2f2a0b0366f073f4b04b23fdb08c1679c4866745d49fa3adf5e7e675a30b0ecc53dcc753048cdcc66c053" },
                { "vi", "4d09e83b540b8d967462ba4d13c131a002e45d73dcc52c9c7948c816b4949756aaece6d65e7e6a22c7096710427d6cf96700ff1e241a0a0a6c2a5654e6ad9840" },
                { "xh", "933aee0c82fba497390d03c8c8c1feaf5b203ec868cc4405a200c240d684ae40b1995122cea7ebc5f34d34fbbbb6b13b80be2c1a7f172aac361af7a8d1d8acca" },
                { "zh-CN", "b0652870b427a63d3effbd9801ca16e6c8e1d57bfe17d99a722966d1ce838a09490a5ea5f858aa5015846853899363bb980ae75f81c47e79dedbba0dcecfc1b2" },
                { "zh-TW", "35394b827ee824bda4f61e6c4d621fa31ea2466fc75b18d17e06d23145bd9e4b79aeb881771a464691842edebfff348b75583abee8fdb2b053c2d02908979d0c" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/118.0.2/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "377f987fd7be2ac0abe5285b4e072f38016d74830740dace4e586908d5e3cbf5b7ca68003aba85cda3a8a1f54577982f8c614445c8066c137113e98e6447689d" },
                { "af", "df7a05112ccf035e01ee8cc57e6d96fa92b24eaa31410c16485b0a53dc46bfdc29bfcb5505ccd706f30f6d35d4463ebc18db80b9ca99ed110372e8fbb7549c82" },
                { "an", "3c5ceadeb270d604f225e7d614ddb549915f5b82dab6cc79332ea5900e1453652179e1e6bcc5fed9709b2b53498f6bbe86a999a4808f19c479a705d431ab401b" },
                { "ar", "539e42d299b9e77313c0bb8b444d5107e3da7540255ad7d4ba976ef9b62bdf17b06f92780fb474c39b4e1baf27628d675fa418344c3ab410fc4e0c0d700d23c9" },
                { "ast", "b19d7ea3d655f7995f53c152068d6eb6ef8fc14edce5cbfc13114503879df8c8e1270db3a2a51aa6a42a85870af1ed4d5e891836224940c5c6461d9d7e7cdd0a" },
                { "az", "50376c14fe56c2ef2355e64788a9c727b38b0cf839bd6c5999ce82ccf1f9f26accea295a43e2d38e7e6187b79467b618171be1cda05b8f840a0ef47652a68c08" },
                { "be", "03969ce68b4e009d7ca4e909f87d9f987bdb3372e4ad7cceb35c5007675e3fe7f1ef77bc9d67d9bed9c0d016032e42b276af49126ad46ee220782cf72ef443d0" },
                { "bg", "b710b3aa02265098f20dd32c60476d9985175522691556eac21a661ca7061e59ec7f16b1f245c333065e8823a75929f461112d8869443e24e24ea3e7e647f235" },
                { "bn", "4c705c8bac821160dcc08f974371e04dfa315073774e4a89f3332481e25a772c2e07dc278e641e24a8fc6c629e741ff53f61117877b8fef3e6db15ff0244d63e" },
                { "br", "c412b31650d7ad90dec2aadcf0d52d44cd135d209697467362494d1edf41e4e9697d8b2967a4e06a75d041f367f14fe769c00192fb5920ecca1bcd73e6440f8f" },
                { "bs", "437572721a0585a20deea8504995425232ed8f06d25e80ca24c56e3937f972e672a34700d1a005ec1b5e1e1334d30cf56750a27ad38286acc3584e1b634ef161" },
                { "ca", "0451914f2ca85bc8d9b96bde0e0110c11609301817cc1e2e8febbf108a5cdc6dc5b9bd40300315fed4dd29fd6e18824f73cf2917e750537516d6c65b771688c2" },
                { "cak", "d018cf52bd707063da00c6b7aeb71383bcee49217a76719e207db52612e529489cf0b605705a183552aec8a1bd1da89497684dcf21cd322dead31f6723cd9a82" },
                { "cs", "8c6be83c12018ac9fafcba38373bde135951915bcf45f695804e7046512f2355634fd9e75623931b300c51a0a8bc4fe415fc3e2f513409ffb95f03e29e76df10" },
                { "cy", "3184bbf78358db5fa6e92015faca4e8c267099a9e7beab830d54ec8ebdfbdf2c1ab04a0c8070221e6d7359878d05496ecc0e3813047eb671ae78bf64353bb9f7" },
                { "da", "ee57be15fe73ebd11bd7a95b72d2235eb55fbd8d9dab80aeb852e401dae4c4971769935fc4e14f9473354b64c7d2acbc99d400e2b783b18f61ef17869439ff17" },
                { "de", "9c6e5458645db1fd59ccfdb41bc7964ae04b2fa0bf41c88567b3d24c32ed0680e4d6c5bbb350a9cf3a4473ec8218008e2042ccc657401dc37c169dd60347d12c" },
                { "dsb", "8584f7dfc82e42860b0a1d478fbcc2dba61cebed021911dece214853ebc1597d05ee4cb0049d604b30d5d1b9b138e6e48262c2f530371bb617ce4096f61ee842" },
                { "el", "820fcd8d55043363a4e7d2555d9dccb6a7deef8ad3453eed46f891fb2b21db15c00581b19d3a9e3d9a179f12779d1ccf0f787d0522c1bc9dc7882d10094c0c08" },
                { "en-CA", "98e08a313258fc5fac02082197a0b213173d49368ca1ef11fd98b97ca5b1f5fdd7f7b533153569d071f7263f0572fe40107322aee3080763627898e2a1f25f73" },
                { "en-GB", "25101cecac43e92847298c22851e62aed8db835a1490ff764d178ed8448de5038e0cf5c5489050d0c9021a0cceaf518a3de3b3f38ae47c16451a9e2393a1edd9" },
                { "en-US", "949f024d1eb4d8afbce62d82686e1917ea5d885e19688e38f4139dc02159690a18b0cb957f7fa27c613a3d8013d70223ace988aa189e63ee5fc247aa4f724b4b" },
                { "eo", "2f40504297be70a6cb66c79c2ba0f242b7c092deb8fe0df8660465dc1e882216be127446976f3a57371f19d89155444644034b8cd32d1d0197f0bdf240ce7a0c" },
                { "es-AR", "5c42daeb5d85860b482846bb81913c57282761033180c80c260705fc499dc8c70b52c140d7e2744ace6e65f4d21a90bf4d76416ca8a3f71bd00a60c6c3319cc6" },
                { "es-CL", "22d0fd64aaf436781a2b21ab846bc3173384cd0ca240feb6e81c8723caaf47dd0610ea700602b047af9054b7b55b12972f3011a13897751a7dee712a628747e8" },
                { "es-ES", "63d2069a7a22e1654bf90ff8578d581747a9e244714090c4053a5f0a39d75423551777edb7ad78b6233dd4e909d33a5fc7a596962f07dac9e2e436d30dfa0ab1" },
                { "es-MX", "8e1143873a7b9fc2c2c9435f85b2b8151677115985be75d9a4007a95b776620f9dd9f3f88826c72b17d07a29fc691ea00c0be70a110d7b65f9959adf82f72b38" },
                { "et", "be686662402ca269fd8ef77ae173edc20a8ec2e0ae962f1f9012c1ebda062466d5e1ab0903e67fa4023fbb4c51719b0e72ded92738ad6df7b1d132854a7d845b" },
                { "eu", "486d90f46461783c361bd032318da35cd61bc04ea8f532c19f3c4b324204e05c6a0364857faac1e50c998a6e7264e85fe966a84e3ddf9b67b74d3f7ced4746ff" },
                { "fa", "f061b18b73ee0a2f2d4e70cb3cd15ad212768bcc0aa99826c13faab6d435974c2fb1c850eb293e441b16274d60f1706e1c16e1634207182764ca664ae57aa3c1" },
                { "ff", "589333a80a81b756445b9b548454b4d4ef4d7a2a440bffc2741216ad2c835982480b582fc121e726ab81362af9d87ed08f2f6bbf4928e471a6fa9cc63f16112a" },
                { "fi", "c85bb93d875cd8bf7fc043ba19eeb59175c2cbc921f5c13e321833e729d1ef5f36cab759bba08b4a4d339346ae4c9abdfb1d0a542c0bd902b971ec56219727a7" },
                { "fr", "5274afe9d78449701fcc28114c6d2b8dac7d0f79b672abf4a9774e77037c14b9c42871f721c3df6544d8405446b7a9739a42f2f6d01de5875d3faf5041b734e3" },
                { "fur", "076b81eedca5fa10a93eb27e17b0e199bfd4e69089e7f7cfad1660e16e9023dd1bf3ec43162ea71b6f30f2569dc255b0f459072c9353ec05c3ccf59744c59f87" },
                { "fy-NL", "a1ae1d5ed8782eb40be8ed06c71fe26f2ab6bee327b242413fb4ce666f82e942b8798b67171a54191d1c83cc6a214ddf1a9844708001f5e4653cd3ba2b18bf73" },
                { "ga-IE", "5cfdaa012373a64bcc8c90baf918669ddff193daf574f90322345000dd303ad18597275b8c2a03a7741551fb3bc1a5ca708e4880a0f0249e67e06f36bc1c5d1d" },
                { "gd", "850e0c56e0f4b5d777bfc31fc1952c091cbc2391540a627288905eb828b05d67afd878d3598dda83716df5c60263a54c87e0c23363b54f27643cd6778c740934" },
                { "gl", "5c8b29398eeadcd29ff6e30595ce80bc3ce5dc62a9cdf24a1036e695251baa2013644c559acc3378df6b1db0c90648d25b7fe197f037b88b4faa043712b4f481" },
                { "gn", "26a736e9cc5098ee4110b0730c2a50c73c5c9d0230d6109363aa2ca2d6f92277992de2cfe060619698022b24787cde936b1da7ed3fb1c997ab45c9c4ef21916e" },
                { "gu-IN", "724b2fa5e2efba4dd6d302caffeef1baa42b21576a44b8e2f707b04c384b4a8c73f092eca858b7bce47a927d1fa2ea3d91d798c321e180da96d6a4fd406af416" },
                { "he", "070de736fdd252c01f5ef91f1278e05b24e092bd5a134f0bd9ccf8c57210239f5dc0034e3ab668cc7ede79c19f326069e9c60a43408e6485f7a6485d0db8b57e" },
                { "hi-IN", "86caa540450cd1661b2ea0b8b7298ecfa0b4e4b639468c8b2b99cc470430e8e50d941a430a16fa5cce139e655f6be5e593930f810f9a6d878610f489c1950519" },
                { "hr", "1e2beed160425eb32ff98580d8711af3889b10f0b3918c5f7630bfbaf9328f1321025af758db09310621ed0684783149b9bf6f17b1bc4844c808e7cb48fbccb7" },
                { "hsb", "d08d292be889e42d660b2a9940c9679441c838eb6e62436b0796c0487c1164f3bb53281b1d3e51fdfec403f5f25a3de0235652a8b3f678e04169118e2d134730" },
                { "hu", "5b497d84c937cc305fbb5a56eb4e4a53509dbd96c894b374ec4eb5882e870dd8fbf708ebab8bc0ebd19622db4b9b23199b08bd3a0b41b77654c18f51474b62bd" },
                { "hy-AM", "f308af8acee3038aeb90f0fac572bb10573e5edc87bbfe924d6b7dd2f8c8dd43a5a25dbb18da6c928926cab181172b8d0530c4f7b8ad1693120f0cc0097bb224" },
                { "ia", "d4cdeadd8f3f96bcc102b9335b83597ddb2e5e553a74cee9a9880397a26b36e7b5d24c563295f2ab866f3df1fe71ca50889dff345af96d468d23b651c3fe7c55" },
                { "id", "c9923d5ecbf3d38e0fdd686c5b73a6a75010cb11ea21c5e8555fa443f201f664c81611d73a5f458fbe880bce9d1af562365ee2ca32195b01a4c6e75dbcb9e9fd" },
                { "is", "015e18fdbc77d15c4107faf4159a4c8ad9aa2fa470d267b5c2e753b0bb652845234547e3b718e5b152da6e8b2aa826f2b44ae807e1abffe1b57dbcbb348b53ac" },
                { "it", "1c373cb070527d2e7c6a200894a4bbd15a8c3dc8d7ace88cdb45e97c7958211ad81760b847836bb52d97aafa377a318ad44d2ff94ccde1ffe841fb3ffa49ff7b" },
                { "ja", "597fc57c83c77209edf71c83b826638f6feb8629ac04878992f7798ade40ee6ba126c2182aebd5e3e753c8e71c7cdd8057c48e4ae73996c0c3ff8566d859b446" },
                { "ka", "39ba5ccdc8aaf1bbcec46a87506724c81badaa37c7cea8ec5160f68b2918d4f38bc4219be0c534fa3aed76d50f75d3e90277789a1b58c17065d8390e829166b4" },
                { "kab", "9c851132d0803744a04bf829792d1618265f8329d15a09472a57b94c52ceb2107df2b7106c66704987e78620f6da6085f07b0d73d840a078473c3ea45430e605" },
                { "kk", "db14ffb4e5e014553fdb565640b6f3a60b0f4ac405c035bf818a47a117ed8ee2ef61c74c7f81d6376c81e3044a693f6ce97eff2269e67f666b9746d3a884064b" },
                { "km", "9aa38b9032ee71f12176df563f1dc60144774c26eff8912f1e6dc259848c902c1bc4a52b6d5ee1cb468aa2d99797d9b0a3167e2d0f82043da70699310cfb2fe1" },
                { "kn", "0b88495a5ca131005a1011881f6d73cd185e5c349d2cce4ee7baee1d9d937386b7ebfd1d3cd1cb25bcfde50ac70b2978382372b3b16fe6b34f1b277e1031a338" },
                { "ko", "6ba28ae333f38bfb53c01b54b57eea71164385b7b812af30fd05e2a905212750a2916e49f5ab375fde5f55d203a03f07fe6eea782660c698e1f58cac00133bec" },
                { "lij", "67f8db36d01b1106ca57cb15cde52e0e63e1891ec71c3ce5a40b7f899362d0f3671f93d20730010fd1f2758e317d22586015ef4397269ae52169e8ce0f5dd45d" },
                { "lt", "c1e006dbaf408692b809c04562d677b7dcb98b7c787ce10d64d027f7449ff5795915a9c683cb8c872ceb5e5d438f5496ce36b449a4465727e94ab51d960ddb1f" },
                { "lv", "dbfe66161bfdb2c3c75e31318524d2df7543873ccff232b31c22f8682eb6fa8efb4e37cc0d5ee87876dec688e435118359db670150e1deff9c73fd170d1bcfa9" },
                { "mk", "cb9fd609e1d85225a162c98edf25a8daed72ddbefc1ff591fa7994688f17107a69cd4ff3b2729ff8f42e1a8578a16ed74581a3c7f411b2ff3bab6c990647e341" },
                { "mr", "cdd5e017a2d711745ca6d57f8ab9a6c67797e67b30250eeef5523a692ad06d511b39048e2cbd8a009edc1ffa397a8ce956186940d8b12cf703c2baa2b9e2848a" },
                { "ms", "5a2ec3a8cba93371c0093655b0f4818b0db8378cdd5f3fb65af0793121bb43cc92b1d85743ba7b64c39084e2fe912b5f64fbb9df1e8baa42724ac144ff22bd5a" },
                { "my", "fc6636db6d20b61ae03941aa58bfa135ec6bb3a6b2c4b5033e484632a1ba0d879081f08d7ff862d522efe0d41873a3a6034b944e62fb96cd91ce14f675c7aae9" },
                { "nb-NO", "5e261cd8f34d84c1f4206248b423333174199e900fb8bea4c198cc89db986a57a2ad8578051dfa0986aefb074b61399a705782b15365c6f026415aeb9524abd8" },
                { "ne-NP", "69b6235cd495dd1720689e2228ec3e1fb1b48b6d48c03650002e4309465140460c08832abd17ad6ce224de50929c1f0b3233d87d6d9a8d6f0f083d8c3e07bcd3" },
                { "nl", "940020415abc888c9b31708eb12e9f2c27bf55a48085525d1c9c075fb666d9b74c0231033ec30d1fe20c28e0fe6bd2bb50912d0a8b45300229dc8d357910448f" },
                { "nn-NO", "7066667f6467fd3fabc40bc8ddc4a1ddb6b75985abceaac1ae11459cb6067d5192e99bddf2fb3d681654f31a85f411dcd28e4bebb8211833afe9cb66ff3919e8" },
                { "oc", "d4b7693e37b32481f8f5dddb3487e839207b776542df24b591c2127b4dba3fc21bd1b5a3ef992c8ad5a21dc0d23fa23510da06575d5a43229b18e69f9d8c7f0c" },
                { "pa-IN", "e18945bd7226c770b9624e44ec3a0182c75f8d944d170f136f86e8045cb71dff8ba3bb84c098ff79d833d2b7994201c22b6acc287a58ba163dab38797d97ecc4" },
                { "pl", "c4f364862c418ab5629a078396afac5d006d25b227897123eec8a383fd9324380e7d4027c4cb3689934ef1c46a38b82fad3306c62e92d6dcbeedc40b83b78f5c" },
                { "pt-BR", "f57bba9c059e49a1d5232a4735651f4baebd55793e4261370d408d828e692708f7857b7f1be27b9fba31e223093aeeac929ddef27369cd6a9cee83b0c7b1998e" },
                { "pt-PT", "68a53634e626a66d50f4ce8b7bc22432889e40884844e8a3b5ce01eaf4539045a05f0fb18c88feb30f258d640020c42048014a7928f4497a82643ee0c18bdbf8" },
                { "rm", "20217ab7ae66102c98005dff8b20efd1449267a7dd33b52c839a8184535b6d101bd085e59add6399b5e0af1465879bf767e4e19ceffc197c255fa764867d3fd6" },
                { "ro", "98345204e82260e5e3c31cfb991079cc0aeecade9ffa8d341b48fb0dcc61556c889de5e930f27ba4b1cf2f54e667c9d09437fb8ee8658054e5d945a6370edcef" },
                { "ru", "3983b63063f739e0f311c6cc63bc776fdccfcce17d6744838f5143f0eaf32ee35accc2e8e70806e350236bc335956c1e50fadac43ac53b1e5d3d4280da2e5b5c" },
                { "sc", "e65965564d4212513107af423842ed97a4f666a556a0c4269667cf992bdc4fb63d13ac143b5b71e052336dce66946ff4e5d0e0a820332bfffe1a49349c4a811b" },
                { "sco", "130a2f9c04ea56e7d95d75d3c39420873edcad23de311b75c7fe9ad23d192e87fe67e8056a4eb934b1e672333f3476c044605a6dc8b8c0cfc031c5b48809399a" },
                { "si", "503c4b5c08684d676d432805da8f90a8fb254e0c1e128e60cab6dbca25c690eeaa3df9ecc9ca1420743dfe865d3441ee37e31207dd912dc93bece71df3f0afa1" },
                { "sk", "ab0aeb0dbef7597707ad2ce43e5dac99dd429b820d1e835ac8c684576973fd7aa698219a9ad8de1ddb814dc57bedfc67f7373063cd787edde18b09abebe4e05e" },
                { "sl", "52edff46778023b691a9941ae126de1625d0d459490a6d614d531074150deddf278e601a50009147903a749120c6c8d7958686e5e5103443072b78e45764703c" },
                { "son", "a0e4436c96eeee6c781bb410cb913f55350518e9b1a76e377d19ff06f1fc276390159e7a1d107b778d1abb9424eb387017112767bda4a4ac82d893d90df4b22c" },
                { "sq", "1457ca01236a08ef03579b21b052792c4e522596e7678d0e0c72a5dc7b90d524f74d099a9ee92d86aa8529269f7f36e1f3c4a55d94680524b6e3513bf1c6c579" },
                { "sr", "88b0dc29ef56295d9b6ef1f4fd6eabc466f41e3dc9166def19cf6dbfcecf59b124dc9b249846e3fda4ef365332831b0656535d72716669d899d452b9edd6638e" },
                { "sv-SE", "9bfab63ea865d96e8068b4f9e26b1c8b7f1ac116ed6223737dd7359886d09032bc7593369b258cad37012204132361664a84dce43e68482c3fc35460eb9951a5" },
                { "szl", "a5b02afc3dc6fd3e6f05eb3cfbf785b3ac9defa0e3ec7d55f6a2c0184e724e630efc2b82a081afae337bbbde16a241cfcc43142c5ee1309bb161b069b3f974f1" },
                { "ta", "5d9fbb3d83388a6f02af6e31c7bc6bcae09337dd6675d4ecd18665e66d994a5ffa335f89f3568b00d3d80cb829cbcac1e08ab8d9d6fdbf455debfc264fe04084" },
                { "te", "c82274c3626e6e7c76131bd78c09e36b148920e36dd8372c8754aefc3c93a0eb66b526d1c6c9fdc7532b5557ae9d15ec3465444f581f5aca82610316f65d8116" },
                { "tg", "58cb6c8d24b6fc7c2bbc1f97c2f7820c4af296716834abbe7171cb52992c3fdabd22b5843c13830ad583ca8cb0f9476d6c825a5df3474d990eeda0484a96abca" },
                { "th", "91a1f97640091f02f0eb6c069c0ddbfc7a7026b70a68e5bc59ed620aa605dd4661ad9fea8ab6876932708eb2128363e5452d1ab86f4ecfad1835cf690536bc78" },
                { "tl", "99595abb69b699ee76083f9f3973bf4e16c69bcb2cc568fb0881b749de722968258a40c8673885092317083efc5fa2e2d91ddc1c264b9800f3453f6d0660cd71" },
                { "tr", "f5e85e60532d782dade4dfbb84efc939498f8382a38880434738e85174338b188b799096cacc2e5fdb13b4207db90fb94bab0fd022e61e300881bd7d9d719e41" },
                { "trs", "8f60dae0c1baceaec3337e3e968ac4a42ac7519cb3839a47ca41140b37edfd4fa9c9c1e24a04582188436f882495d5f9e5fd0e582818aa0b31ef3afbeb9a228e" },
                { "uk", "0e7ca46b1d76eaa3f4f3e602ae8dfc069123ee46b15c5d5fb536b2f104105125306f3f8e4cc227314d4fb365872a88b75b27d4af11f72b974fe79cac55f56154" },
                { "ur", "ef0db98c7bc7229a241c00db4b38ba86c0b5b387f29ddf3cdaa5fe256ba67cfd6982d9843552026640ae996a85bd1ae01fed53de7683090f3e76170515937d35" },
                { "uz", "0d5a1f875d8f5597a20e9019af430a39fc663c18192e7e9c32bc3cef5ddebd658be79ea02435c51b19f6040430750122ab7bcd47f5981982abd77ebab277edcc" },
                { "vi", "5f7b09e54f0bcf19e2cafaa2096d60aa2c06f1ce35be308a8dc2933449ed0a1df29f7058b5e0ccd8c821646de7c28da6c9d92f22ef37ad1a61304c67bf13eb51" },
                { "xh", "5f3ebd819a0fc013d9310e110da1877cbe460dec5d005b3c5b78a27a479e29f01d052fef0fa61e12188e4caa70ce505ded7f126550b41c5ab411a23ed8ce3929" },
                { "zh-CN", "d9e1953ba718afee6bd23845e857aa8b9ac6d45100705cef855292f7f264207e29137e149f7d22808a7b496441f80c5ffda1c596906803e13c30f7a776b1e8c4" },
                { "zh-TW", "9430626dc2576d8973b800ad9a756798f4e7e125b163b1172438f36349ff191a557b0c75f466ecdb3857307a79aba4a4704e546dcb34ed0799fab775ba0f0df9" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            var d = knownChecksums32Bit();
            return d.Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            const string knownVersion = "118.0.2";
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Mozilla Firefox (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
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
            return new string[] { "firefox", "firefox-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=firefox-latest&os=win&lang=" + languageCode;
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };
            var client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
            try
            {
                var task = client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
                task.Wait();
                var response = task.Result;
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers.Location?.ToString();
                response = null;
                client = null;
                var reVersion = new Regex("[0-9]{2,3}\\.[0-9](\\.[0-9])?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;

                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox version: " + ex.Message);
                return null;
            }
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
             * https://ftp.mozilla.org/pub/firefox/releases/51.0.1/SHA512SUMS
             * Common lines look like
             * "02324d3a...9e53  win64/en-GB/Firefox Setup 51.0.1.exe"
             */

            string url = "https://ftp.mozilla.org/pub/firefox/releases/" + newerVersion + "/SHA512SUMS";
            string sha512SumsContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                sha512SumsContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Exception occurred while checking for newer version of Firefox: " + ex.Message);
                return null;
            }

            // look for line with the correct language code and version for 32 bit
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return new string[] { matchChecksum32Bit.Value[..128], matchChecksum64Bit.Value[..128] };
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
            logger.Info("Searcing for newer version of Firefox...");
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
                // failure occurred
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
        /// language code for the Firefox ESR version
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
    } // class
} // namespace
