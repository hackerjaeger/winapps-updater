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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Manages updates for Thunderbird.
    /// </summary>
    public class Thunderbird : AbstractSoftware
    {
        /// <summary>
        /// NLog.Logger for Thunderbird class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Thunderbird).FullName);

        
        /// <summary>
        /// publisher of the signed binaries
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// certificate expiration date
        /// </summary>
        private static readonly DateTime certificateExpiration = new DateTime(2024, 6, 20, 0, 0, 0, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Thunderbird software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Thunderbird(string langCode, bool autoGetNewer)
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
            if (!d32.ContainsKey(languageCode) || !d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 32 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/102.1.2/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "a83d5894c59f303fd919f0e5e3e65b24a7c13e5fc3a1e9bdde2831c9b9bf118b3ab18ace9e8419a2b6d74e4f2c7b7a811869b0a87ac0377b9d39fad1b0efc971" },
                { "ar", "eb817093ea3db3b9152377cb0b8fddfe19e3e98e12405f421c523f4d1e0672d322fd6e28542e5dfde552dafb67da4ea54b1212a5dd49fefa0b19b1917691a6f2" },
                { "ast", "6ea02349015cf3cea1cacb2e51ce6b252108d3fb1aece6801acaf7a927ee431a3da0a499e5d028a9c80ad104af42fe978f7e4fb621e3763e40a512543ef6a961" },
                { "be", "80d0b0599e7be92aa64b023795d4cba42bc3e86ba44ef44c5acf41c50d6507620c6dbf480b2cf9677029763aeb96c1ea44b4ea2d65ddd841c4e97fbfc388d4f0" },
                { "bg", "e7090bbd2c017ca83960fd0dac4bf100ed5ff186f78766d6bff170eceda86d930d5c960bd1f7a87944c9796344d22a91b6256ed8d8041ba919ac13fe2d45c229" },
                { "br", "a680b17f146fb1a840c2134e3b59eab0bbe201996eb482404a2267eb2f53065da968f43059b4d234ecd8fb7147df0c38d20d72fe74dcc6c2d9c02765851abf81" },
                { "ca", "763bfa2e77e053747b3165d7a986509183f3c848a48b4e4896ddf2f0572210e39be973b2ad851a4a500c40f83d76ad5c90277720df2d80801592950f168fcd7f" },
                { "cak", "17541a3e78bbbdf897b9d9b8b2450646b0222b3f32a772692417079fb227ce447e53e6573d2222471e697e2c981f9117f431e329999b871cecf5316186c916c1" },
                { "cs", "c1938867f684ac67d5196cc617d1093b2704955fbed61c90d489f43d0d9c1642f91eec4dfe39d11e4080e9721fcd344c0aaea5bcf66ffa2818cde94f7c582c9c" },
                { "cy", "7f99c62e673bc9e67f3c02f2b3e5c0bcb61a29691db7c1eae392fc5a6185124d68f00bacb8fa1bd5cac96ee983d1e35ef8c6092f9f402df8088516c7a1cd826b" },
                { "da", "87b9af72db2216e00af2a6c482c548a4d59b32913054a2342e8a3ecd47eff8c8f78b266b409beca8280db3e1a0ba2dc80dbcc7d26dd92ff30556a378ce9f452c" },
                { "de", "efa0f6823fb5af43e296c6689a5eb30af8bf5120d072ce22efcc2a5fa01d8cc15c5ed99105806d56f9362e5abc0752a9768bcb46baee06ebdac953c09ed6cfd8" },
                { "dsb", "620aaa782fa8b9bf344d2758dd77516db646d2e04bf9ad4ef0c64f840b3d2c3ea4180e3d0f5e29619180be98415fea057d88e7373f4b9eb63d4ecea9a8205182" },
                { "el", "0adfd20495a2432ed829eaf26742d896cc8b65a53fd69b94ca7d0f028f5c8a9a8e6199783fd371927997528a6ff4a245e79f2c6ed8bd412908bc16208155050e" },
                { "en-CA", "ef818117d7589d224a0b61163c7aa02e6b2c73f474224e17a59d58aa46f830b58932974ff1fa1b8b4b742306aafa3b41453afbc376d77bc0b8312edcd4d571b7" },
                { "en-GB", "3b7d54aa2462bf546a1c0d94f62fea698baa75dd6005dd6f02eec174cfad3b3319857bbbdda5366edbec7c5645b30e6d254294c723e6c56d5e4e1932c2a7a12f" },
                { "en-US", "dbd6620564b9a79a5554691af5d3284e59e64be0b339e5ba461a51d7e05b92769509be1f76520d5de82d6a6be7d7f2fe77e4ea07f3519f8af1e2e254fa1104e0" },
                { "es-AR", "350446147bb901f43d54b7d236684d2e331da8e0082daffb29d2379fe4f12d590c7cef3dc95fce2c47469a39ae31a21639b77afc1ce2f36dbecbb54dcf6a6cc7" },
                { "es-ES", "ba2ade2b412aae18faaede9912748d63fc7a7e8e91f9453a868faf69645739662b36951ceeee69cea7a020001bf2efabfb7e718328b1e61b0ec48ffb41b68535" },
                { "es-MX", "b7f512965dbf9b425fc1774a779a75acf2396534a449024adfb06449b1c6cec73df4a3826a05aa5634ac379a6a33298beaa184bd72963334bfc8e2ce7f324f9f" },
                { "et", "6ed3e24785916ef8362fe424aa16ca4852a262856810132be562a4a2df23712eadf2e06a55786fe956a84e217a458b92c83fe494a50b87c4c10030e721affa1f" },
                { "eu", "626be08c3425579f63ef9a0fef30233fe54dfd655ca0e6d674bef4e992449ecfbfbb40775d437fcbfd519dfa63a802358444f3744cacca0e723f9384154c6dd8" },
                { "fi", "15a85a919bae5c981a356e6098b0839f52b7368c4e7617182abb19bd50e49e60e85932c3ab10b636cf527f6360dd41356784002bb365ff780d8883e0efc048e0" },
                { "fr", "f5e1b0f705885e6e907766c256f078808bc9ecf9c72b9af71905e63089efa3d780ea264399b1920ea2cb538c527cb829dccb8a5827899ccac93fb4d73f65a7aa" },
                { "fy-NL", "4ad93029e02ccad1f71786d9adc0ec1e75890e4272ed6d60315240497b5d93f4f6012e510e0b06a815f4950eb8f72d8232066a3542829eecde6e88ce988c52e1" },
                { "ga-IE", "d1abdbdcdbc727b6f490902f75feef225b64aba60b2fd8764ee87c3be6249265dd73878aba2e79aec25929a41d930223c28359b4809e50d3b0029da304099c7e" },
                { "gd", "c0e5a60b101ca0caf9b4d1155cdb442b42f36d1ad3bad4a9be3ae1fb7f71977078e580783fe74ef8e1bddaceab804fd7d34fa6747300a24a4af69e427629804c" },
                { "gl", "7a4ae170d35f7291afa1097420f7aae060f02a78ce41be5d4c47738e2e55a21229133c97dc760e9cc6d8c633e138c5fb5328b674ea3bbe39a64a1e716f078dec" },
                { "he", "ca0e00703aaeed20b6090cc529575bbdf5529aadffaf26b3381d07b07d274bc6fb4c815fb02d9a9543ac8ceea3dfa214a7e08277c4459fcc36e4099b6bfee436" },
                { "hr", "28d698e3faacf91c76ba47cb55e33aa06402ba5128fb831dc2f7bfa60469e5c05f37a13331dccabc3e21d41ce2ec5b8b7f230ad4809f3bf6b061ab5cbc2cec00" },
                { "hsb", "148f8cc56d6fce6bcd9215e31220ea58f864170e260c33458dc73554c8a74b13f864133c514df5f108560f211ecaa55a604a8678915a9b4b5aeae4df2b4bca4b" },
                { "hu", "27a4598c7010f22a0c97f8f249bbc07a010ba8874159ede1e5f2a642997f3a2db65c06c0b23e380eec2f282c268c7bf714fe04014eb95ce00787e0594f57020f" },
                { "hy-AM", "61ccb3739869a299af9e9549783c213440de4d779635288658baf64fb22f294e10586c318419bdcbad6a9325d3cb51dcfd267dcfec1e57486a128dc82660d4c7" },
                { "id", "27ba00cc40264a64e19e27bba385073adf8365d055d168dfd82a0f815e06759cba1deb82aa46fa99377179f57d68068510c64022c0338c0db49fc0780bf58dc2" },
                { "is", "f9912a76d5f130776e34be26d525ca4c64d5cf87f9e7f83adca90dda25e001fcea84e2128e77242a776694ea48527c407f6dee21ff881b5f4b339383f94fbad2" },
                { "it", "53e24f03e88b76ff17f577607cb34f8fe13dec714b3e9d7570d1383b614d86f7164cb92bb6da802fb688079a24753662e6b202b7544b07e5d21ed64d4f1fb5dc" },
                { "ja", "4364778f10fa6da514152daaa364ff12f28aca80da361a617ba852c91fa8e1515d282099842ed7c80f5bb830573562f8393db8dfc8eb865292fe96b6e346e6bb" },
                { "ka", "f70d503b2fe3e4a7a786ac54d906e83ae88005cfbe489d3b861443ad6f44996c743ac27a894d6768c443a4e2c1482f591d2a829e32f39e4977907325e9ab06ea" },
                { "kab", "1c22d67f240d234756af51f76e6020cd18b7b4107b757355763aa8d285584a48b800531fd500ea60340812bc3227732b589685c833c50f25a9ca77ad578caeeb" },
                { "kk", "292bcdc896a49fb2aab1e779999cf5a07471ede1de5f393447f8b2f8a20193bccfd6547ead01ab5088b8a01761f2edb80dc43f0c60ba7f3b48314a693c55d2ae" },
                { "ko", "0cd3b865918fd35a35fb3cf78960b6b8aeb90d0369e73aefc0c095747b2be3bea5c0473d35b9a567db163fb8a629ad049afa57c2c5da67606b4456a8a10c4214" },
                { "lt", "6b11118449971f03591d679782430613ad779c78b3d795b8b6346d61d6fd723f247c4204026347e96c13ff452b168d49bccbf4b9ebb9e0b2d11e91a0e36b23fe" },
                { "lv", "37d2b219987d14d3c794fd3355d6876270cf9b600c60465a63873298fe3f1897709db94d22c14a8b8ca05efb0e0500dee9422095098acf4ecb7a95c4600ddda8" },
                { "ms", "38090838623bff6e83df138c8d8defe3e75e93f8023e11bbf3e8fba2cbeaa1da90c1082ae9cc0371486651542d0f0d08f1b15d406e1bcf18c77e8f66da8e5815" },
                { "nb-NO", "b5d0d198ec8f628606c9b0d03dcce22be763df8039b893c6214e0398d515db8b12c02199b7fefcc812dfa0913bb77c39902185a3fa15802eb19cadb3d44c0421" },
                { "nl", "045b25b07a0bd3a7a0b5da033392194e61166cb4a0e991b6295ef5559721b3e058e65ec21eddfc7a650befc7562bb3a3c2c1c2438dfaef6d5eb25c771f7e6e07" },
                { "nn-NO", "8b26e5d0604dfa90b4bf46ad1732e752d80483446984ef4fca36c6cbb154900bf1c31e4013bf10332870df9dfd75619c2277056fa94c6466210e4a09b48715bf" },
                { "pa-IN", "2ca581842220a7d1e8862957e54c4028085050908a67374c0670eb361a6a1ffa864f9cba68a470992a9886fec3dc3ec7be39e2b1f6e7c742985bf8c26062e068" },
                { "pl", "adf10e28b37d62f785c5c2e862c0c4107876119570c073da8a2564ab630a4f71b5aea7bffa1134168ffaa649ffc60018bfcff1b84faf73b472f6909e07d0c886" },
                { "pt-BR", "88cf4fbc48cd62e38e4552f4695ff5b968663570af94afde6c58f82f003d167d425acc54f7982be29a39a3ff9ba68bb77c9833f930b78b64b220bf18edf4453f" },
                { "pt-PT", "f42e803f2e765db0e5fc86e57823825e873e84f1b3fb6b8e392ecfad072bbe7b319d4adf3db91abf38cfb13a92765f048f4d605c3c4e50a7eb0f72964db32252" },
                { "rm", "47940244def89373a20b3b031b65fbb8942ad4e93bcd934f2caec35adfc89528795e4f5f08c598ce3d36ef2513a71cc8f05719b4be2d58f4cbf3a71603a88eb0" },
                { "ro", "c18e4125c6f679fc996585d5c0d2e3fe65d40bc7c06a55f926b400a4e3a89f05c9ac2bb0e3b0db0522f986885b286c92477be71b91bf454d7087a542bca04ab3" },
                { "ru", "9374d709a485d777b2b2fd3ef758ce14dc3e1102ac646cfed0f8707b3d47d81ba351a3c67290979818e81d6e470629222fac44fc2ea8cce93c9a9be84ce64811" },
                { "sk", "1b294d60debc77c95ea817c64ce2bae733547dd1835b0b0db3a65d638ae7542484435ee005ffaa092d47b5dd065a98d0c58661d756db613d1e5380d532ffa628" },
                { "sl", "4a91df39d38dac2e1729a41fc08129b655ac381906862d2c0c02220e7c0b8debb6eec692344a60848363197e7e4d379e9568256b7acbffc16fd6b7a4698bf82e" },
                { "sq", "cedcb48c728986c59c644a85b3761f831f2856ab26dce919eaa6ab604590618038d2f7df5443912f3d93b56b53e748948095d08b4dc05a38fd9fe97acdb8e387" },
                { "sr", "bdfe809ff9c1f9e6989e2fd0bb67bfd0de2491fb514efe45fd2a89eb92e6a17c37daf0748cdef509e2a81915cbd7f6bda55b36e4e2ac2d4db166d7a019c9597d" },
                { "sv-SE", "8bb1f5273f373c243a1074a2cff39e3dccecfe026bf55346fa847641e1e247517124465860922f336b80f26e0de56ed02c2da422c44b369dc82509fb63dcebbe" },
                { "th", "e6fc39d10bc597e1d7e36778e48403aae99b16e972e75a5e7bdc3cb2ba153c40e3db1b359c74af92bedc98743143af50bbf882225deb003d9b2f0163ed4ff7b4" },
                { "tr", "38941fb1d2ec75cd47f9957e25418c95e862f74cf2357b6cfa87a014fbc71b9ff9d742d05174d1e38ea827ce92323ffa2e324e1ad139d34ae2ebd41398062c4b" },
                { "uk", "23e5d5a7bc7dfa6cdebf06dbd5b28cd2fc28bdf7acbe66bcbf3f7aaa018f940b582d640b6656fe0b51e2fd442feb9eed59e6f7ded30c5226d4c8cc898e87e7cc" },
                { "uz", "7bff7947f169470faac3ed4bf1f807ee42782b96e50d6f5a425d984ef9ae720e6692a796fa04afccf5d17b3366c5ba604d33d320ba3eff37bd67cb11aced665f" },
                { "vi", "5b781f916cda82d7fb7139da3709a81ee80e7f61b7d903498d24788326d05a90e09cd9d72b2d0ea65ad7d51f01ad61696756b810f673c2e7ba9bfe4f1fa9a7ad" },
                { "zh-CN", "0632e80cd07a5db9aeebfabc25e6bf1151d7581686a6adea96b459ed472fc565133510de2572947fddac40bb420bcc8aeee7bb7ea8b6c28fe1f20c446c445cee" },
                { "zh-TW", "95ca6e39a94677dbcedc97b41761d4afb358548bbe52f19fc3c3594cb92c3d83c212f8d1defc656ee43b638d6b85377ab92d416870163e7866e420e8c97a459d" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/102.1.2/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "474ec94a8fc4323325245f84bbfb8092588152142bdeff84ef4647f26619611d6ad50edabfac5d575efd92985f664f7349bf7f7faea8554491f2575fe84b5124" },
                { "ar", "0d8c68fbbdfd7a41791838e82aff8b903a8561dbb2010671d3a81b9492a3db33e7bdc85658511e8ee1d1a8e4bab2d219edf69fabf8ec4a937d0902ef2752d6a1" },
                { "ast", "61006cd8162eef56afbed88d7286d53af18255b1e25e68c641b03ac9859fbd49c63fd4f6cd0a02f93036eec5dd04655d3218f02024d66ab1eda689f6d0ffa4bd" },
                { "be", "2c2be9d1d0677f3f8a6979ff2e54ea4d0117655943e8b4a6e9651f840c2bdc76293a1dcb11b3b68df52c9c0131a4fc41859d5fed06e39e0c2564650bde42b39d" },
                { "bg", "07f8b93784b4134df73dcd410f6556f2efe35f8162e00c8b044ca2aae976266051d604026d800a6dc43d950ae197bea01b2c65f6cf8a5e74540dd567660e672d" },
                { "br", "d3fde67bab154cd1b998f02208cfed4af3275c691b808f1fe2baf71c6f3ca8401ce82a6309621c6ad615cf4c7d9eed9c5dd8b1bd562d54abb1f69dd38751c550" },
                { "ca", "b4ac640e1d932d352fc7446f21fa98939914be6bc684f7230d832b8914446199aed8ec804396d60df86af5a47164b137f08ea813ce48fc29f74182025ad05e53" },
                { "cak", "e0e34103fcf218d73d094f73f128e8616922b15243ed50ce6aa1167384e081a5e36eddfabeb73423d37b098669d1701694bc84b0aa5c8697e161567d7664252f" },
                { "cs", "15eea1fbe76ad55c6b8467d220ab441285fd01deb5b790b758f041a9475dfd8d7e024bfb86c1da233d6683a1c6072140d93b98fb39db1d35dd05edda763100b4" },
                { "cy", "d8e1cddcad013da93bb86fdf43cffa27233a6ecc0be9eae55f96804ef08205cec94137d946c6643e6702c0c0bc2f0fa169a8a2bc681420fd448297928d3cbe7a" },
                { "da", "e98a5c3cd7795b3dcbd1869c03c84e4bbaa02627cf580913076d73599f162f5e5997b490f01215fbbc940aa3c7650a35f3f438d07ab5169bc01b2f614499b09b" },
                { "de", "7a2065451399a142291630926e03912e075cd23fba97da590bd2aaf7b08c50814484659d4346bb931344cc8318639a47a04cb1da024ed947962c877be1b54117" },
                { "dsb", "645d3769c69e6f25c5f8f4eb2d0d8c0c62e6daa8ecc7206f0ab2c3adc94af1d2edda027bb1b96ade5df657a1455d22ea0a2dd2c7750b7f6c95fb76e2b604a50c" },
                { "el", "0054f6ba5a2e5be83833001626d5a12ca2268d4f2e408b110fbc7dd8bfc2ca24d43e1358f9245dfb3c05bd0d89fd103e9cda3b49d154a6dc5c4f2d361b5f2894" },
                { "en-CA", "45a98d51b13f960a9959eb8257fce47ab04edddca39fc9c28552dcc74f33d09780877fc9f12029bcaab6f93238afb29be4ca6d5ca6e10f73255b2987054cb842" },
                { "en-GB", "e88cc6f79b26d0409b64c47c0b45f2736f9deff8d51ab19090414a19e2318e2adc015f45e0b0505ef98d6f1808a9f9162979729ea6edaaf129d621374dee96ca" },
                { "en-US", "f376ba87ecc3bf0fc9dda53870ea25bbb0e4f230d48d389fd6b96df3f07328cc1675c9795bb3eb2ac7d81f7fb19d177fb3922cd0ded1d8b7390c2d906f6b5a9c" },
                { "es-AR", "4328d8aa674614d61a8464cfc3b938c6d9d4cc49a560904a6b4b755284afdc7e98e38f60086c815c2671b1c50af8868737211a07532ce1c5c3d6d24134272357" },
                { "es-ES", "f92117bf09c8ddd6c1047eaf77220aca333db3a936ca048a50e9d67c0a28582695f85398620e3c519400c20ccaaaa20cc4e72c7b9cc5ed5011728b5aed280b24" },
                { "es-MX", "8e7a4b3868dcdf10e33d93f678263d2f4a49980584486baa905f715f981b7847564a661d4f07a2ba3c696b430eacc79c950f25309d2c2fd57d1bb430b20f8154" },
                { "et", "05b6ac15aee110f745476f8ac69345600ee129192ce38a42504ae5ffc43b4823a4726e1f614be1d017ad4d07284f9cc9db11f943bfd8de96ccc7427f4ce635e6" },
                { "eu", "5aabc6ce598e0416f3875f6ba5cac9d2d192ebd3091da05b550416b225e49fbdbf929a4efe176f0f6ac8bbfc33c5cce74913f6dd72fe97b5ba64234be27522dc" },
                { "fi", "7a93f4f510c32e5280a0897bfd9f2b77834fa621f6c66854dfe48e99f1a40eab049a532465b090aea54fe4c6555a6e6b0af384b2c33a4330ff3ba77aac8653a6" },
                { "fr", "e6bdce92db6a3e27ed0a54cca1a2096ff311f3678b9e9033554d987d92bd32fb65d0a86877171c3051e6325d7df2091cc49a6fe8eefe93ac6bce9892cf7365b3" },
                { "fy-NL", "d4d44418c4489c82f43f4839b2ed2942cf018d39b0b33c6bc42ddd22765a755afcce9fb2485eb8373379150d4b431e18631c26e60f72cef085bfa4c2e0f3c48f" },
                { "ga-IE", "d8f10ca51196d5512e6c333045e555b06240b2b55e4a43e838741a5ef7b26059fab8e7fb6f348720153214dae31dbe93f2e963a79242e0fe2b8fbb52a1dd0571" },
                { "gd", "3daa6f73550cb24fc01a39c347f684d247549113fde600e0d80307f4fb9d77ab1d76bc58d59957d89512b58c523ee7e8da24ffce7e4914474618943a797c20d0" },
                { "gl", "f22cb91233a0b218124db132028c032f9fd8aab11912d71ec1f6d2e623addaff11e195c453dcfc33ee6b286829a0a33c0110826733d259135a9dc65a57916f7d" },
                { "he", "ffc697eeb0d1800e351c1e662e47e27b8390cc1d4cc6e2fb33d6cb6bce5faa07325178236ea6b2ac36e91853a2ee6934bc3d3909db0089b45ca75f6c962eaa3b" },
                { "hr", "6c4f62605186fa5cfc8e612ffe537747add61f37d2c534e8ac936448ebb66c1f334cbffbd14fd48340043071a2207e716345e7f0dbb6965bd6ab6c0f8d28c17e" },
                { "hsb", "7a21f2eac4acca1d175049d17f1e0e50fe605097b84109918163f630dc051deb095a549163218c38e142c8c614ec09a7e74fccca033d525233f2ca91891204d1" },
                { "hu", "5940020983f38cd11c428f6bc3264dac5a7027e3adafa68653e27c5cb0eae8913b5cefbdaee9f67d86321d66f1b716e46e6b83ae504ff2cdb1c75d581c67ebd9" },
                { "hy-AM", "5b3ab6965af3057b3f08fb87408ae6cdb9fa2c3fe494a485847f87e6e74b0a0de43cba6236b55d0321c2c95a4761c0df9f1cacdace17c0cb1b91957423c4657d" },
                { "id", "2d33dd36aec2a6112be34bce7e4c5e0a364da00e24ca51692a45632d51255334fe79877f37b4ebf60cb5f98a97aebfcb7e9e2310ccd99085d8c2ed5719ea6e71" },
                { "is", "27dfb457f14376f5481bc6cddc3e8ed4849e23eea1a7a3802917555e91efcd63937ea61d12e536f6db776d934e565b16fd8ebe9466ef64e4150ed2fc434839f5" },
                { "it", "127a90b3d465b69953d4ff0b8f45e0b7c4415ccf9359ded6514481f885486d4e8260f9634ccf617d588dd2bf548f51952260cfd33ae5429c53496f7d0b50720c" },
                { "ja", "8b441171ddc0b7992ef7705ae36a5301ad24309d9de368ac15a9e76b48ccd95338350ea69ca1af0adaa6d4ebfabe3f52a80d607a786dd26b48b97a4323e1ef63" },
                { "ka", "76bb7ba989272ef2b038af0f0ca8786e0168228d26c7342d94745f5014571cbfdb98bc4f704840cb64f02b76e857dbcb033df716d538d72a71c1277535184fd3" },
                { "kab", "d2d123a5f9ad0457036b801574212e83dd166d18b6b3e1123b4c6b5053cc6a496469f1392fbefbfc4b92e734fa1393fed09f5321062713f78eed3f540ffbe641" },
                { "kk", "91789294e8a3c66160a49d594b184390a11ce744eefa6d955c6734e76a4483e77dd4c5614c51c60968a220c30a013a3ec03fdb6b36886b75aadf5d226a9ea164" },
                { "ko", "c2cad8e13d36f415e029a3e2859122d14edadc3d6f04bf7ba73f31970cae443f10d50ade1b7aed783e2b2a70d7ae73fb0be3aac64d0ac1e35df812a9bd6d45cf" },
                { "lt", "bcff557981cbe7119fa0c3ea792118e1cd3f08e2ea5d277b8834eae3cc99370106fe42345d93992520900c455c6d1ea221669ba550c32c1eaf390b1eef097a4f" },
                { "lv", "a901bd0b38f84ec56086791ecd23b27f26ee73103ac81e390917952953ebf1c7f9344b6128fb07312c0475b8a2adee44ed41f4997f0644aa8089203f5d941a5a" },
                { "ms", "c16063dfcb462b7745d094266d12e2c61383fb87dc6f2bb0f4e8ae9bfe9710e670d9983fb05476a42e842c9935706f7573c20367e49a98ecdc26f78fb8d77dd5" },
                { "nb-NO", "d8c23dcb20c4d8e0d43670433e483063d133a31619f8b62b8b953bc712392b34c0cc4adac73e6ec3a520a28d09ffde4e25d437705dbda4d7ae3c50b068a39ab9" },
                { "nl", "2f89ad6d794f874d37bc199e062d488608f3da1d03ae921e5c580f92be3cf67a920a62a332f82eb21e2704e3893197d88f5a004da6d9bf61ed111928c776ca06" },
                { "nn-NO", "baa964db90582422dc60a729b2ea7970ae338fa5835339ec1108487dfa10580ae841ca8afa7cad2cad910a16eb690f214e80ffe0159b0e217172efa5a87529b3" },
                { "pa-IN", "5d84843e16a8dd4b7778692dadedacf5df069a9960b6619fdccf4dd1a22d19695d3a27c39adecbd687980c1476d6599e0dc84b66d08328139092bd303c6a1b04" },
                { "pl", "f0be3b09440cc392aaa371220d2831e9cffc4f17ba485a6a4de704bdc991cf0c27856150c853ebb861a0b043aa648e0760a6d7e136bcf05ffcf3491c7c36a9b6" },
                { "pt-BR", "6a601596fc5dac4fa11e9802f0fcf101dafcd9d00a34b8c1b5de9afe98a0cce840149e24c589267c32adad0bb5ade324f61fc96a7dbbbdbf7625a0c57aed07df" },
                { "pt-PT", "1c75fdb6803ac59fe8f4e4426553ad464681afa2d08d62d5e14d6ebaddd596ded60246440fe49a5b7f01a3e2ab1c96871ae0260699cad3f6a17b5198c1fa2a12" },
                { "rm", "68553498c85e1bb926cdb12983b6e387b84ca5f6f92181956832fcf6889d814fd00f3e5b5da07c11d57716a0979bd491bd38827cb73232fb6771ba4080861a94" },
                { "ro", "61bbda439799723b8b8bbb01c66f02084f976cc3fdc2f7376b28c986de65f4109bd9b6d2d4dccc6e7b3251434453971068f11991e8e41a2e27a29154abc7a613" },
                { "ru", "1a3d86faeacefad8e7a3de5fbb8617ed28ec1234438deacfe1950e7f0d5f1aeaa5a769bb93fb88931478c416fabd901e633995c5cec80d963fe4d07e56ce7a51" },
                { "sk", "aa705985dedc4696202945a7ce82efe29505690e7ca147c1ead459f238e152e687fc5a3253983219e569e3502b2abe8cddb902aaac275b931df31bae1a0998b6" },
                { "sl", "91aa2fa1511dc11a97ec948f9f18083589c9d41cc52219b6c525c29309cef7c6f7b84aebdb4b322e735aebc0bc889b38e1ce12c9e71c48442f88a1803161c9f6" },
                { "sq", "8a00c595389979ffe2f588f199888646e612339a3e40181d8cbceff1cf4bbf2286f38d1ab7acc6a967ff0f157961c74d740510d71d418fb79580db20804d274e" },
                { "sr", "53794d80d780f9cd29abdcb31588b2f0f7da7080687ea7c467914b478b3a116f431feb8f074f9cb305d523e871de5affad7dad5238ccedd747f51ca60a8a56e4" },
                { "sv-SE", "faed67fd5337d9bafc42ce602801d9328948dc22a9396ae2a4cce2f53a334d7b748eb4ed4a09f71744d6178a7fbfada34b72033ce8118dd9e76965d8bddc7f64" },
                { "th", "09ea007ce916d8fe0e9b4f17c8640ed577214319c0204dbab73fef7e909fd0698b180879432b0eaa86db1360d076b04ef20f05c031bd33a7187710704b1b3bf3" },
                { "tr", "05c7de3b16777ae9fb73ca9d244730acf60289f57637a59b00255652836f5bf087b910c2f038e9ac831899160bcbc950c94993f2cc7ac6a56dcb220aa6a6f34e" },
                { "uk", "204421ac7f0cc37a146b154634088e8c3de4076715bee254e276df36890cf612d9e669753b2e3ffa6b0e95b4250e573c37e990c93947e0e66fcfc275fb8aa863" },
                { "uz", "b03b7deea15f287e896763f7d59e1384642a7422de535f1be402f39d024eff4fcf7882ca8ad79290af49be3bc7ebf0bcf396e5bfc3a6f3a8b0051ca71fdc4bfe" },
                { "vi", "bde06aabc199ddfbd5998054e13d9fec6110c11d9ae11382c5c3ffb1d9f3654631ca62770ee1abb52546877157f81742d15d45adfbca738b9212d2edba8e7903" },
                { "zh-CN", "50292654e87b0b4ab4fd80bd48f5f0b3d1582978f89324eec23925685d9d724f84c024d62887447f0ae1840d8f27f216949e656945d5b116361d38bef63d3ed7" },
                { "zh-TW", "529fc0b5786871e5cba9d7839d12f9a320ef4ee4a7e7aff8de85990760870ef10e7e770f2f6b3974bfa71334941db954f29708398d95b2705fec2000715df316" }
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
            var signature = new Signature(publisherX509, certificateExpiration);
            const string version = "102.1.2";
            return new AvailableSoftware("Mozilla Thunderbird (" + languageCode + ")",
                version,
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win32/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win64/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma"));
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "thunderbird-" + languageCode.ToLower(), "thunderbird" };
        }


        /// <summary>
        /// Tries to find the newest version number of Thunderbird.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=thunderbird-latest&os=win&lang=" + languageCode;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Head;
            request.AllowAutoRedirect = false;
            request.Timeout = 30000; // 30_000 ms / 30 seconds
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers[HttpResponseHeader.Location];
                request = null;
                response = null;
                var reVersion = new Regex("[0-9]+\\.[0-9]+(\\.[0-9]+)?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;
                
                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Thunderbird version: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// Tries to get the checksum of the newer version.
        /// </summary>
        /// <returns>Returns a string containing the checksum, if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/thunderbird/releases/78.7.1/SHA512SUMS
             * Common lines look like
             * "69d11924...7eff  win32/en-GB/Thunderbird Setup 45.7.1.exe"
             * for the 32 bit installer, and like
             * "1428e70c...fb3c  win64/en-GB/Thunderbird Setup 78.7.1.exe"
             * for the 64 bit installer.
             */

            string url = "https://ftp.mozilla.org/pub/thunderbird/releases/" + newerVersion + "/SHA512SUMS";
            string sha512SumsContent = null;
            using (var client = new HttpClient())
            {
                try
                {
                    var task = client.GetStringAsync(url);
                    task.Wait();
                    sha512SumsContent = task.Result;
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer version of Thunderbird: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } // using
            // look for line with the correct language code and version
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // Checksums are the first 128 characters of each match.
            return new string[2] {
                matchChecksum32Bit.Value.Substring(0, 128),
                matchChecksum64Bit.Value.Substring(0, 128)
            };
        }


        /// <summary>
        /// Indicates whether or not the method searchForNewer() is implemented.
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
            logger.Info("Searching for newer version of Thunderbird (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            var currentInfo = knownInfo();
            var newTriple = new versions.Triple(newerVersion);
            var currentTriple = new versions.Triple(currentInfo.newestVersion);
            if (newerVersion == currentInfo.newestVersion || newTriple < currentTriple)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if (null == newerChecksums || newerChecksums.Length != 2
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
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
            return new List<string>(1)
            {
                "thunderbird"
            };
        }


        /// <summary>
        /// Determines whether or not a separate process must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns true, if a separate process returned by
        /// preUpdateProcess() needs to run in preparation of the update.
        /// Returns false, if not. Calling preUpdateProcess() may throw an
        /// exception in the later case.</returns>
        public override bool needsPreUpdateProcess(DetectedSoftware detected)
        {
            return true;
        }


        /// <summary>
        /// Returns a process that must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a Process ready to start that should be run before
        /// the update. May return null or may throw, if needsPreUpdateProcess()
        /// returned false.</returns>
        public override List<Process> preUpdateProcess(DetectedSoftware detected)
        {
            if (string.IsNullOrWhiteSpace(detected.installPath))
                return null;
            var processes = new List<Process>();
            // Uninstall previous version to avoid having two Thunderbird entries in control panel.
            var proc = new Process();
            proc.StartInfo.FileName = Path.Combine(detected.installPath, "uninstall", "helper.exe");
            proc.StartInfo.Arguments = "/SILENT";
            processes.Add(proc);
            return processes;
        }


        /// <summary>
        /// language code for the Thunderbird version
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
