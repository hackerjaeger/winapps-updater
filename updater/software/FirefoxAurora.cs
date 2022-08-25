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
        private const string currentVersion = "105.0b1";

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
            if (!validCodes.Contains<string>(languageCode))
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
            // https://ftp.mozilla.org/pub/devedition/releases/105.0b1/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "57e1e864f3c9c30b4b2f4da4b78fcb5006e0922ad2b0298a8be70b356af1258dd54395f0c1be9836f3ac6a3c0ca05ce01e6089e47f09899ea54c05b151fc7a80" },
                { "af", "9bdb8696e7edbd5e4df661fde01c29d2bde9dfbe62d9e8ab601598904e4a6be17f95c5f297721a9a4f20b583df420b32f09e72b8be123487f146d39b7f03ff5b" },
                { "an", "80f6cc608ea979b62a4578dc827b0abb227682f01c88f8cc38dfbbcf81ed82bfb8e0f30e9c507ea54b36e4c71fa237e4d3ae5c679ae1161b1bc244e8cf1733c6" },
                { "ar", "ee6d1e187876050e1750a202b75a8e936c9950f9af612fcb57ff542a576de3b25daca4bcb3361f1340c162bda85647b7fdc894ebb92bcf6f95323c3370dbe166" },
                { "ast", "57d410dae423c7c2e257a4d536c682acab0e7d373ae66f91732b9ffc5d99c3ab0e708f406a93b7608661be4d06069c8da646466332783b9a533367312eaee602" },
                { "az", "0662b3aa560f7d84c98436ab64a57d6195701bfd6080a43e3ce91e64424347301495abeb0d6b5c855944576527e66f085ea80ec864945c33c273446099a0ffec" },
                { "be", "bdc3e68ec6d376a0ec949e498995b1debda87e9ef91a074da7c11bef646b5e5c8b801270ecdc648002d3ce9c5e2f26e7c010f109f41ff04afa754fedeec60dc0" },
                { "bg", "e941b5445894349883019a371d9c28f28cad0e8d4e6060cb2c6b46dd0540482941e9a36995078bac086343a2e59f427304f42a264c745842708fc59e97b4224f" },
                { "bn", "742adcc87dc8880f22a05b6f973291dd9e51c144be9ad6baf844efaeefe9e5357edf4c26c83fd041029e5b474d246b8e8e050cf0479d94d1c0a24bcabd553624" },
                { "br", "348ccd31e58cb20fbc0ff12f44b9fb975a6a705459f5d6f29a4e33217fa3948a301d11a29a946944f7fdda69066b054304d9569dd69c5952b023696de321be8e" },
                { "bs", "e15194f3153aa6dd73d9daa8234d380b5c29aeb29a4099f364087ae5580cc3b1e53852572efa59a339988054114e9046861386c11bc6fdd56c8174256e05c240" },
                { "ca", "2226dce8e336bf01716ba4fa488231d666969a325d8eba1e8ab6e3f7f101d1949bd00a4a27eafdc74c7e87110de39e620d7df3ba09b1aee43182cd857f56602d" },
                { "cak", "b28aa3cb34eeb62876ea30621e2db5ec4cd9919b194cc3b4c1343facc8d522d8e7bef235244989acbe48e0b08c1270d5b728303d309617b5585406dfac7478a8" },
                { "cs", "891f69345576329408b3faba76782e9517c55f288bad10b746c43f032bbe76fc4e79aff6af9330791d8c6bd6fa6da4a39801af718407845099ca0e08237cea91" },
                { "cy", "791fdd23dc63d866298b7cb910dd8e21414bd6ff0c04bc2872d0a902db15a541e96b7fb47670ed79a29a4c098f36fab8b1f7110bb9cb57ac354fe611e460b6b5" },
                { "da", "136e0a31ab26d88acc852653586e3903a2448cc3eafeb5c77b0c092562db0eb1c166886de106d54e721339e1c033a4e386e6a7ba28ba805beb554bc286451886" },
                { "de", "4ef0d4ae2219099124ef30ffd1b01fe125fa27d79ecd0f91db398c242e12e1ac1dac4dbf52d4f081466460acac0beb5c916ed59afbe21a68a1865557151dcc05" },
                { "dsb", "4f486030af36d679c2c6380bddb1ec2b380d52f2f337a043435d02f57e697ef9896f585192e4e858af11ef85530102a29e7ec517e2db6e3a3d09aec8977e3309" },
                { "el", "5a1e79d082291fcd255261f3cb8b3f862774ecf4106b0f5841c18fdd61ec544b683e8a7c4c65c5ca8ff3c7da0b51bead2f0f8055165cab1a6e9ad2f9ff277456" },
                { "en-CA", "5bbca7b3376f0406920a451bb7ec14d7a73eec5d37730f800b4a8014bf2525e2ec5afb2cb0df911468160a384b9ae4bf9d10f1c54c8885de216402f823c1144d" },
                { "en-GB", "089e4a15b2aaf49b7ffa43dd336fc8994b7898749847726c2898c3756fde871054dd4fddbc536cf3714ee734b416586b10e973360563fb3fb2677043a1de14aa" },
                { "en-US", "64ace8db2f6f27e92a3cdf9b206b177c63819633efa38997ebd5e861a2fc92e5c137164ad905c2580f978d317e440eb6393511ee52347565fe652fb579649a49" },
                { "eo", "71a06341331dbd406c728220b7242145f132ca2d3da9b12605a84913cdab85599312206eec603a8e6ac99dfc27a4dfa7359cdd2961289f76a67fcca370c10fe1" },
                { "es-AR", "891942ac1516b41496cb22b6f4014300adf3a67a8f9f3b79917d72749ee6247eaf8c7fb8598c7c561cb8aef72ca026c2b47b24df0ddc29cb8ba552a0caadde64" },
                { "es-CL", "f297fcbc5b6842045ac9b49d99cd3c4635fad89cded8820e3f1331be1686927f9714cf83870fbfa5206214afc0b11620e3a03013564eb33a03890421818a1b3f" },
                { "es-ES", "89b5eb4dca108892b159f9048ce0cc7289c73934478cf6d0f058a230e8e07f7139581b62de69885bace64fa7db055696642ea725f4b5a2a5ec5734fe7ac10905" },
                { "es-MX", "04921059b4b924be54677db6bb66e63086d5831a4ecf56997c5715e20bfd54c6cc9051c5dce8617f6202d4c36fe0261440c5229c404f59138c1d9c0b9566be40" },
                { "et", "03753815d8253feb04725777e3d4fe048af85d830a143299835c0b38a78c4a330d7dc29d44942e37c6433a7a64a3e08e230bfeb5d601e696be292f990c750d39" },
                { "eu", "eba90dee89688ea21911c2a2d18091939e2b9b39e7946f92cd3c108492419cefb229cee99725cb33b9d55720b00f182b643c22fe11fadfa9a9b91d5c0ba6af6a" },
                { "fa", "044de575aabb789f59dcbe238e446b5f9ae5db25463cc2d42144b3db949d98bce5de4f3754377766d8474a777ace990424d890813ad259f2380f28119a96672b" },
                { "ff", "cffa49c4bc5906c7b4e7f1c2b733126f9ce66146469739704f9c088eba56320e84a88ade8926051faceb859e60211d1b0ac7b5fc6a2c6507b84d8257b64f6ad0" },
                { "fi", "c3e467479464885cd6255b1edbc623f82212f0a0a515e25adac780da91a0a5e5208519a9b87577b927ad9c4ca427c466fab936b8226433c0df51364021a04d61" },
                { "fr", "3097bf8b48bcd36af1003bb862f7805ed90da0180e391dbed0ee5b9ad3419a150c709d73c2dc73f9886664ed13d435ef19fbbe7fd2d6a8b9528231fdecd1340e" },
                { "fy-NL", "c635acd6f612ae66b6478060372faa732282489e07d87d17946d24bd95b7de2ea1ec1e312f312422a2f60be7d02d7830c7f162e3fa19f8a3b6d198ac3a6dfd09" },
                { "ga-IE", "7105ef0ae0aadbadc396b0d1617e383c527c8a02730229caa281f939cf7109a474f50b7fc46d98412b2dd8fa0f02b86f21029dd92a036533ae435b5caabcfde4" },
                { "gd", "646661684276c502d9d0bdedac943c5105c98c0ce061a680d82e772b85db967d54ef3a0e592e6c5604eade234ff969035e9a2c8ae9abdb422d4ee69409730f9e" },
                { "gl", "77ef3bcd9e830d59fdb80f482c344d5ca333bf5a110f70d23f824e3dfd4ae7c94f107e73bf0796b997e7a41dec258732c103880d640891ed8aa6cfb9ca8bb55e" },
                { "gn", "117411c4cfbbb33092e8762761cf44b086e496d1eb24353413dcb051987bd39107ee58f0e8a153c6c558f6684caa36a3941e8a4feb0fbc261e437bdc403d5510" },
                { "gu-IN", "53a314f5d9b5f7497560d7867a7785601a5416c8796424eeea97bcd6317eb049d2eb47683cc003697797a97348da11d6dac746d1ed57edfaae18b26ebb9ded56" },
                { "he", "67c66ccf692e25ac4731c77dbd0eeb766fb02da56193e907ac9edef90d24235cfb4c9022d4b1bcbb2594742b8f10952b27065b4110f09b2683e95b2508d20b2c" },
                { "hi-IN", "a00c98eb1fd38704015de5c05ce2fe893024cdaf3a873c8d0df8a94949107e5d41e4731aeab6633d6bd301319127ac638c63278dca644ccb0bd4729b6f173cac" },
                { "hr", "c0152c76bed1eab3b415cf5e333f12d9315219a8ca56ee55f329d95cd73a56b4ac9f5fd41454430ac2a40037d74e30fb1d529eba3826517e3c6fff806e5220c7" },
                { "hsb", "48f41bfd10832bebfd39d9f75ecb3c60b0e1b70ee1e7518d515eecc51f5aa0f811b41ed7999f2da8425ac22f300287afff05af1c6565e6a3b2c332d4338b4e7a" },
                { "hu", "2f73705035e5d37f11812a0ba84da9f3e6fcae984c9586f47fa7ffbe97b233d4eadb44b7233fc2aad22c3e6eee8399609e64fdd82f51b0b8ac5834558ad1395c" },
                { "hy-AM", "63f71fa93149f8ac966154a230e37a3ec6a417603172466220f13b4e8a5939ff040aacb47627760153831b902749c6531f513a9520b5482bc9dbbe3b708396fb" },
                { "ia", "61767e1c55ad2da5d15fd1c36ed3ae78fdf327d6318913c05848beb74d2faec6d35770faa319ce398d57d5283992aa81fab7c474f1a972ce54d15e0a483b7446" },
                { "id", "1d7ea751fba2057c24e5e70ab3989f4c66c163a2e392dbb9a034dacd8993d2addf8f7e9d29135716f1f83d06bf4d96d3b86b3d6b2a594337e5f396996260f5dc" },
                { "is", "2966c2a09f79ff8c3d814e96f5f9213afcf7529a407f7b9708db948854709c82563808fcfe5d338bd6b44b2dc2cb3841db18af1fdf0c7b4f90e83bc867b65735" },
                { "it", "f363fe8ffa04d9ae0a17a9a101eae0ab96ffa54a1cd07db899e70526796385e6a61bfcb97abaa5eca56c267f94e816696f5d936c5bf3ea15c250ed3fcdffa593" },
                { "ja", "e86971e9f076d855788953353702455a1a8c4713b5f70412dd4f30934350cc792656eb5ccac8cab2930f23ec7441b8498731ce4395fbca7e6d765332760b4acf" },
                { "ka", "581d4f8a9172a6f5c8369a2a81362ccc4505f569e41298555fce72dc5d344ce420fb81635c68cf4d27d19d6205db643feb553ffd8947bcf991da0cfa2c24043c" },
                { "kab", "9a430c8a83ab89d1509f014f9d06d88eae70ecdf670f839bdc44f208780c840aa9e7cdc02b86f61d855573495287d769fc5f17290ea7bca42de0b844d9fc9f61" },
                { "kk", "3984630acb7e5dbec36baba2720366015ed156c5275c7acab3d468047aaa5f6b7763c9b1b9b43416d12363934c0feaf1f905404facf4889e51201050fb98ade4" },
                { "km", "671f59eba306a7b8e8f615055dde4c76ac200bf719ea45a644f4dcd85aa70d306486d74a1b5b66eae3f64578f4b8c9266830213bbff2f72d288a62be70e153c4" },
                { "kn", "a46ef3332eb276fdbc102ea4f2f3c772697160c92b76b011c48459ce79f8c0b10f4de5da72a2032302f945b48ec2ade794572c157971bfb2bc871b59c2c40ad8" },
                { "ko", "4941ec06e088e5fd72545bba915ef63234f27f1cd4f6964b8ab6853353cbe4fa13d0ddd8bc19adbb9929a1e38e56ef97507cf81b1a7179aea76ccc9c059b8dbc" },
                { "lij", "c888d35b22e225d6bd1c63c59acf7a39ebae8ed75766a858cf3228fd18a5c7479133306d9dfb13da361ca9ab53d46a497427d360831e29277e48a37216f1d338" },
                { "lt", "fc8f4eaa15180149eae94c9a20122d368685305d40cb766fc958304402721779ed02ab84a83de82c6997f2b9dae459ed3c8866071bc8c39d932f18334f860712" },
                { "lv", "e7811799074a764aa37c3d90082d653d84e5e0ffc763feebc7029a7687e4d06bb067ebac08bdc23e677d4f0d9cadf7043877ef2e9eb1d9bf68585b73b0dd5fbe" },
                { "mk", "4e36de1d027c0230e32004a429f9ddcbfd282638a4dd6b404cc53370012f4d37d3c73061a2cdfca102e6a6c27d82a73f7204c325a7d032ff8b2094a98106d38c" },
                { "mr", "ad0ef20493b87a7f4633f61ffde88c486868fc415b8ad9aaf4bbc0b01f5782da6444aa30152da5729930781ec456acc873d3318fca8d3201c3b01390ff8f2911" },
                { "ms", "20dae9793d6a6ba965e1ac1b5a313d8f429c98dc60aa41494e79765244ed86e059e14b1b392f752d231885455e8ccbaef7077c926a074e2fa4a6311e230c2854" },
                { "my", "05831087f384a2c27d82208c724a3b6f9fd9d36eb6767c72d94226e1544851b2e1ffb9dfa77f9da6b990fa51664241a5ca425de9331e515d843e215b234f820a" },
                { "nb-NO", "8a84411bd45e58508672b6d9c32225b1670acdf899197a6ab3d54cfe4944448d00f077869e2b3ab0a53a7e18be22552afccf1df151cf9919f1e044bd1ccb4b70" },
                { "ne-NP", "9d04aded2ed690078815e53bdb141e9fb202b54a6a908c1e66b4122f9701056359bca67054059a7678a00d83aa156efa460ea76d4213d50d1d55540fccc1c8e1" },
                { "nl", "70be4cf850c82ef54a35d066eca3e65b65aeb3af17737d0de9b71ebe49f33d7cbcb29760d43d6bfa2a7c03205f227441914471d353a645678c21d0700376bdb2" },
                { "nn-NO", "70b3a274c252990caa1c082fc4c444282adc78bcfce5744386aef277bf1b155ab0ddd729f065de6a5384a238ddb55a8b0e0f4bbc3e40b0eee0278fec72670b26" },
                { "oc", "d0e35bfd3d8d5434c72551a81194ee9f2e9e09a64b6b97d177fe66d78ea7b7655f9a71e9f4c3a186635618b7d74fcaeda50f892dcbef2f3d1ba87a987824ada4" },
                { "pa-IN", "3e0e4587b83d92803b51d37cc3e8c3fb91afa29b6085282143d8635a7c6970049611a38062c41a85ab7984dc9c9fdfcb1616db41b9fe25b3d3971e264ec51bb9" },
                { "pl", "17d7f20e07263ac9bba81e89688abe738d5d8afb758725f9e59e339c746cfe548880512c660cfbb469ade212544358b6b37ddbc7678a0cba843ba305b077cb7b" },
                { "pt-BR", "f6503182b9b4ab6273facbc64cda07bb949cb73565cfad6b65611d12656755172c7da02f6d2f1a7477bc8d4ef9ec025c02cff18e718dc8b86a543fefbf22c0fe" },
                { "pt-PT", "55201d44231aab5a9ad373ab2cc1628f741b070f2225fdbc56b18da7f4f5f6a306822fc7dc26258964fc5a5d5b6b7ad978fff7f9e86d676989c428f92229596b" },
                { "rm", "14704030f45068e0aa01694ef377950ed982faef95ee7dfe0c790682a8a5c4c830fcfec6aa6e721045ce3ade4fd82042a60b349f3495a8d1e710931499bc3fef" },
                { "ro", "5b1e29d147be7140f29be4e2105ffb13aa4cc2a774136f92eac8e5ab0a005e8aa080f0247d35dbfc63c8955576a703c747367daa92398a9d5662420d00b4c541" },
                { "ru", "571c6a52dec22dbbe3ac8011aeddf867b59d9b829629c807559ce3da09de7ef58bd2c61c9a03ad5616a1fc04ea4a3b289719804563df560a2de09d094a99b1e6" },
                { "sco", "0276e73eba50080639c82c65449177e4cfd68fb59aeee8dce242d5c8dc979f0f324d5c307475f5ce141d6809fc559d9692bfd0329657e872fc38d3d92dcafef4" },
                { "si", "b23be2f39c5e3b8eacb3674135391ff267b3cb1da92f54ae2bc3cc6f535a2e8caaf0fe558f0427af94d629da884e3950b263a06eda0aa152240db15f094e817f" },
                { "sk", "541b46831fc621115ad020c212dfc6f00a2a9aa46f08df7b013a85ea40222543a486dbcf30f174fba0480eff5d3283ac8285697f8d22d13524d66636488d5f10" },
                { "sl", "ce216afe1e118f0957519c02df088a6ef1892c063a829625e47dbd5da05e8404d2533f85a66012d8b0653aaae151602198fda6d77acb3caffd2dbaaee872ef6c" },
                { "son", "1e896913af963c96b2f38fdefad77fd13e6df690358a8e7e7cc74b73b0aa1b2d4a70e3cbed2af22dca018eb2252f0e00eb484d253d32448dda279b1476165b18" },
                { "sq", "23f72e2d47c12ce10649573b74852e1f95ff55464618527da70a2d86b0c0213c1db59a8aa861a93460081035c8707e8b3c1b6d258ddac4ee24ad83239d392ed4" },
                { "sr", "48dce7829ca3b00cec5534e9dcd0aaffa697dbc9c427e49eea301b5ccd1cc9897a10532d010f90e253f0402bbf96b2bbb71d612f5dfd410964fcec791a0f7c8c" },
                { "sv-SE", "793b0c5218c0927a4cf299480812a03780e61e0a3d88867b1ff03e7c85a1b879021f3ce65fb37c72368b3647acc2c43df113d06bf1170bd61adefb264113d5e5" },
                { "szl", "59fd37aa1b4c61507c82a84b3c2ea16436033da99459002c2cc55b840d8be6556fc2ae4dcdefbb73d5e7d239112cf7773b9aa6aeceb02a2ea02252acbd811c60" },
                { "ta", "ce742f09f2e8a27c8ef1faaadbb35c315fb7ce9b0fafd868756da03763edbbd9bd8121633610f8f2f9f905b0071ca780b6b07285cc17fc4392a501ea5635b2bd" },
                { "te", "67ba81a3928d09ce26b067b700d0a81e31077fa6bf1fca78eb2754ecad7995140eec994d31d48b2519b596a4df6c333044c77ffa978d397112ccff19d98ccd06" },
                { "th", "034b3b7ef22850d1d9d83ce9186e67cded9d23771528d669ad7fc2e50fd1157a443752ce97b6afb37a1af61a8a9a7c8794ebbca556a496b408a544ccaaf72041" },
                { "tl", "70e96c30e5a496411be1e44f0faeef9d0517b3f25d3a36d45350f5045c8fac9604972da3a1bdb1d7585362d6d8a038d984b9d6a899b0e7d48f86380883c872b2" },
                { "tr", "60092ef5520da762a7d5814dd2802ac8104f26319858235cafb41e3d932b60c92097481d39d78f7648fedcae4c1e9215189c2ef7ad417f3c17f4de4184b5a83c" },
                { "trs", "c528c9454596de94f35dfabaf6ffc1149d945bafd2ab046d83df0274b9394d2a2f0742209b069b79d110c94af6f48fe5b297c69f8b9fdbca662508d5b764b9cd" },
                { "uk", "3d53c1ae30c539dec1468c3ab7d598ea5b6c65ca1e8291bb54d73ea37408abcaa5a65c85901542e41800eb2bc944a3d432ed7d125b9969782cd37dfe72d8a019" },
                { "ur", "2ea2623f65c3f0113d2424ca600f298460453aa2763e4acd694fe402d25ab31c066b2db7a84d8a3133f8977570bc784b854f70b88744f9ecea1e32557c528e3e" },
                { "uz", "9989cd1ef8825ab76f9474389fa527cb5221ca0a760f7e397704836271fb55d5184fcca7c9d64cb67e49e3c17c909182715eaf48975e0250c71c9c7ffd67c62b" },
                { "vi", "816528dac79357c5c72f4f124f2cd280b3cc84b3258eeabca222666e81a95412b01817bbdd0319672ee1abd4ff9e8a684614b939b69417b22579121e89550e19" },
                { "xh", "2b661d9f7247d8d9803f709d3888a023cfba059c332ecea837599e819c0044057c949174b956b420b284b6caa023c4832f3f7ed0bcc0823078a5c9851d801a4f" },
                { "zh-CN", "c79f7461551dce7ed75dfde211601d4c75afd3ca570a420d0f40cf87e74920036eff555cea701e2d2295450096c82c0316e967949eecf31971a9bbb26903f91b" },
                { "zh-TW", "7cd80010e3a51eb4e9c548a76a195e6eac880fa42abdf67640f0f6b615803236e055d0909a683ddfbefd38d7a91eb886340729747ab998fa7f23e549f31b8f56" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/105.0b1/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "d3965e2610867c1de8d3d542f333d0870bb64a99788864c9846eddec79873c79daac38f930fe00e792b9fd865adb80df3d52dad4c412b61ddbcaed2b94713d0c" },
                { "af", "5caa976a8afa7a9366022c2ba9f58f5ac50d150abbe94472501220f415f8315184e27d974d02d885b84ded0e5390cbf3f70e9ab62248e4e95e9a99d9eee73272" },
                { "an", "eda582817e2686fb4b69099c65255062ca1f8f3eb5ef7c5d00e3dfacaeb7838698667cc02c65295c2cff30f22486fd7d0984b47950976b072650e4c421305c1f" },
                { "ar", "8aa6fb8ba4b610210604372fe3c9d3832a0bfeff791ae90ab1b8be2d1c1c4dde344503920090ade9b5aec0f29c8611dae39ec8ada8df1d781839349e8584cb58" },
                { "ast", "699d903818905e3ca193f3cc7a3136c7832e1f785d6210e52686ccd1846ca55b1063cfa7e2c2b761646ce5c4acb14df5b93fdff75e4be55959020842f0eccd46" },
                { "az", "ec1136b6c6659c343e2c5933118f969a409a150a8c4a8addccc8d6b32652cda739fb9f890296befdd53af10b8c7de711f3587db3e9824286cd7bf929b3670b1e" },
                { "be", "80e8228d7277bcffb9050ee6bdbc14d21bdc7581db62266e7bdeb9e344f96274f49d8fcfc28739193358f798659e0c302d14241e905fddadd4aa49a4cf5ef2c4" },
                { "bg", "3526a1cc85901767f2a5f5cda039eb9313252bca8f7a6e0f4490262f4a47ec35a7ace8f30a6b44e71b1f35b2e3ce2e8225b495f117a5c09dbab07bfd3f457c56" },
                { "bn", "66073c6c88f28de07b6c6aab73156d4962f6ef58456afbfbf1c6b40181d51594f5db403ae329381de93dfb5dd7ad86a3e9757e204e4c95720b67d007af2aa387" },
                { "br", "8e6e14bd1e5bedd8ee8cac67484e7c5d9f02bb622d8b0330371526cface7e87b07a6d09b40086153de6771e7352ef38eaceff790501ab6536f632a9a01ea9d24" },
                { "bs", "f668e27b7c8e6972a7a38b04025ccd7e25ec07c50f39c01b443c45a89c8ae28739ddd9a27515ecff33201eabb1a8eb08b2f85bb213be84de8980a5e5374c7a42" },
                { "ca", "b366ac91048954c46fe089d2271bed1a80c6d3f2e902e3c45cd5356d2b09ede879bc62d3d3d78b315daf1cf1cfd95435172d388ba335d1bd1db166a47824c728" },
                { "cak", "7378a97f35c5a86bfa2b7a8f923f207577ac2d7e06c363197f1d411892abb41365d5b03b0d1b1545597436fa0e1c2eb6f75fab68d8bb9af35824fe398d0ed5bf" },
                { "cs", "9879b3932b7525d0951616daeb3494fe78570ae42b0c8a89668e9eb24182b60725d6e0cc5be65770cc934b7ebaebd903930fd60e1be608689d222b1f754766da" },
                { "cy", "2b9b06ddcee6266fe2fa4c4f18636a874bf27af28e3209c15503c7036d9918ff275200efd2c5b46fe36f0f2dfa02255107efcdc0211804b05097e1e4d7463af1" },
                { "da", "cfe81fb2dc0a3f0c2199b0c3db221d277f5a20fd4bb8e252f2bd16f9585dcdae888da25296926b07d4f83ad936b2382cec13f17e55c34c306bfff9cab889366e" },
                { "de", "87bb9b78aedbe14cdc60e394696c8eb93fce65c5f9ef50a2785c63ecc0dadb75aad2ba9ed6194146284e37e50caf195309efd59b40da01cde2ca23026bf7098c" },
                { "dsb", "886e14ffde84693e22ace94162e2c713fc050e48cccf200bb49f77b51389e817c4c8cf34df76fed8b833281cc6e900523c1113284bfaaa0ad34b8765fcb0a77d" },
                { "el", "22a0daa15ab927c32524561dfdaf57eb187a07d69e562ee6cb069d79be940e1507a4a0c0c313a3d1532eec4280618ec61539578fa318d6ede11212e7d5cde688" },
                { "en-CA", "333f332152a32dac9739089c98d469dc3f86d7e150b1f1863239c8c9086b3a49ef737c0d7ca38b71dd56acdb133c6009ed8d88e5dcbe27230845c4fb873034b4" },
                { "en-GB", "7739f2e10ea01d8858cec939a6164c5ac0103483fafcb27482470e084cad57a3dbf3f887c2a332fb883d2f133876a84c9592c6fd3bb1b7d855d05060ede022a9" },
                { "en-US", "0b633059acf62a8e1f18e231d392bb08fdbc9a5b14339b28814dd58911b6232f9ba6dae6d74f759469c83b247cf635bace1c3cd607bed169d74591f3b22ffa68" },
                { "eo", "62c3721a8f9137f380c4d5f884b8611d2a33b809f95d6b5eef6bd4f72a832e12a3630b52f7b2d8bf047652b7aa6ebf130cb263214c85870949c832f3285290a0" },
                { "es-AR", "44dae1239a42fd307363568e8ec735954674890caf41a472631bdaf713c8c7af72978c31be4b6f6d79d51b2a3f22081ff2f49ce09c0f49d296b85c091da73760" },
                { "es-CL", "aa255eb1752e253b625ac8de369c29f9cc9905dda0708428718af3c4d62ad0314421856d1b19ef9ab9e03e76881a421fc2090aab7cdde383f4f9b0129e15c743" },
                { "es-ES", "72e43fbe744d4a04e685adc63ead6fc9e3b3edac8c1cef77e3df4938da1e6fda12ab6f8ef85882990b5c8f69f99763b615a0323fee6c049831a654c8fdda9cb2" },
                { "es-MX", "8a461a436fed90243e1410a1991314f5814b35b2f8308de6ffdb82084f5446f5ab688cf7934a14b881f45eee6c3110997b8c08062ca373bef28cd9101c27adbe" },
                { "et", "cb5ecdd3ad9f7da57ca277eaa323d819d365ad640cc089aefd1bca257de9e16d05144943c439721c65d38d873c67e093b9d92f4dff7fbb5949909eda68d1b738" },
                { "eu", "bcebcffd2fdcbb3fbf9535022f0fa37255b70767d26fc27fbca654732939ebf7db5108746d8fe7ab8be23998599398625bb2d2a826ede0d1b6275141243c5bee" },
                { "fa", "bdfbdae3a367d36b0eb6661ac089f3c0ac0c5f34f437bd97787399c153903ea1e832cef4db1e8b66c42c29f3d14431ffddcb4fdaeb8d3f07d7425c9e1781e73c" },
                { "ff", "8b68c0bc07593703e83f42c522b8e77503a9ad7efdb4e5ec057d20f328d5cdcf7ee2f9a94e0b81a020a8f8ae4165b150920decd499a8ea70d1e26b2738548cba" },
                { "fi", "e9756b190a2c3937457d24b2ca522405f7cbed3fa048cd8d039748d7fa0014e9ce486f029fd7548847144c8d32bad2414cadbba98908fc2512f366199647f4bc" },
                { "fr", "c56f79f1151431c5794e935bedc6c8086d64e22c69c4101ae8461a09c5491bb516caa43623d4186b874c439d8cb9ee6f6cf6fc05a23536a39cbbb357451b44e4" },
                { "fy-NL", "1e8a4a7861f33db22105f10e7743133782c51c20618c2b5ff3a7367d4c82fb436ef99a4ad62508992a89f29e52b35e678fba92ee4c8abeee08c2c5de417c8883" },
                { "ga-IE", "a003cafba0cb869dc7bf77d339350789b2d0ec76d17f23e8a51cf6fcfd0771568ae27a3c8aafcbebc53fc5e3fbe2e866c659accb71c0d31b19074b370288ef50" },
                { "gd", "4207d6a9111c1f0d6e2abddb87fd4af4e3904e9fcdc081373c2fa7bbc7743ea5154e574eea6017e2eb4987b5fc8f99938ef5c6f4763fa2b9f2cd44d43f479ba4" },
                { "gl", "717967dad99820272bb02a4e50fb57cebfa25edef8064cbbf964959176a912edb52f9969b3163e5a9067ede464819ae57f4cae8afb585080630a664fba90530c" },
                { "gn", "46bafb2e1752f4957868bd63b704dd55af3250588d2fe0e466e83ee45ffdab8c5da306e286ce709133bc43683af19ab2a9b912933b4729c835e05474e677fa02" },
                { "gu-IN", "6ab69519732ef34367fbc2cfcaa84f18a8eb3087ed4c6efaf6ddb14d67f5d45fa15e2139f1a302b641da52a0b6acd8f7f7b88dee36191f719ce8aa0f4e88ba0a" },
                { "he", "64f81f7b6d5bf0853d0738332aaab50a6cd4a6f12ef9540e33fea799c39f190e4ecc7d72993d1d711d6f652efdf4a668cf43bc1fcd93a979e73d12f5b4c2623a" },
                { "hi-IN", "cfd34955addd6da9fab944f895c0e3f9c327872541d26a1941d9fe5b824300e9480363ddda63b48868fb10ec38ec1e10dabb9430ba44a31c5c8e85ac3bee9cf6" },
                { "hr", "390972b6883417269c93b0b16f8f9dbe39a61834d17f5ff4d675ee1ee323674af6efc88588abbe047608441a8adfe0371897895969038b2f6d5539da0af73ccd" },
                { "hsb", "9f51658f538107854fb08d7d735053344f660eec9e8ef51219263c33a0a6a6915b8d00e8de9dbb14cf11cb0bccc3df58e62710bcb3d5d0be160d98a5e9fa06ff" },
                { "hu", "93783b1d6d5d813a22543326891e7f4b44cfb1eff2bbcc6b93f2a00e348283eaab3c07826f48013b8d6a9bed7560cebdce6ac3b4de03ce8c4594cef4e0126eb1" },
                { "hy-AM", "b7b727bc0c75b2dce0332a1e7bbf5f67fbf87600459044250a20b0407e66a059e562eeb8ed3cdb420a125ca2cada05aad9f9f35d2e4ad73be47c562a7b2920bb" },
                { "ia", "ca272274f9e3f4c417e1987fe628b7afdf7585fda896e94077b8342a83cf844c5346caa4b7a5692ce63fc1ac3f778e571501bfe7bdce30d220b07006c260cdb5" },
                { "id", "f712d756f8438013ac290b867c98948cea8e8c98b694851129e369feb356073085a06b718a0de7ce1587ff7a8a8cef6a817592bd7a4573bc874226174671e3ea" },
                { "is", "e722beeb5eeef2748a3b6a200f63652bcd3d7e4c09f53f2e4ee83eb9ea4201774a76cc77066d8c53b18aca3a291aef37da2dc40c18cc68c5cbf6feb5601aa9be" },
                { "it", "46ab92e3eda4cb1f69e34594e9d1708aa15551e9b0e69d01ab7829b0e93c8bd6756560ec090f5014f759c7ef01317e26769b7b84780eb4af9cf55104e999f425" },
                { "ja", "beae634865063971b928a851995fed251ce2df495e9b677b0c8045233528072a580fb6963180519bcb44fab2e9070e42074825ea1383a0a128a5fcc6f9bbba96" },
                { "ka", "1813a9ae01ce957a1ed426de427b18473dba47549edafef6cb60c11dadd39bd3a6dbd772540d4d583b69732a184aa34a97e1f4a7ff8b65b6301e33fba5c99291" },
                { "kab", "33ad3f8f995e80a3fe1fd38f7b33e520cfaed30b80c16a8ecef283e880e8539fc01eccdcd97ed44cda64eb67a3fe1c68781e18d3c41d888b75ede9b0c5885f49" },
                { "kk", "28e46013662c8ee6ef65f651b0878b6c83705396fbc0f27de7043465707ba55c181264634a4f6adc24e425e79a6563860f85482846f510de25f0a302f5598aa5" },
                { "km", "069ff6f440e9e6b115d7fd6031670f9a583a17e655f09f7d114bd4824a9676a20c2fe41f1dfddbdcac523b09f17f5ac9e530def213349e947dc30af32224c112" },
                { "kn", "48ec3cf94be10f4b28d43e78f925d71aa3775bb861071be7624d3dab7610b75b89af3f5169ace27523473548cba3073d50c893a0ebc55f4900406a45ae081db2" },
                { "ko", "379b1a3f947ff7adadc198f6150df3c27366c3bc8a53f15418fc6ad9de87d5a30fc2fc21e5ba585e5072bf67a8f1a3163571386f7ee854e735b3a7ea0a2c5f0f" },
                { "lij", "425c2654e23a81cd4471f0d8d542d52d296c1f78282ccda956ac7e87e15d11e50cb2535680abb7c4ffff4861970b98231b331fb915d52e04d387ca8b87152cec" },
                { "lt", "e79109928c2b1f03af07978b5c3052a6166e53354b0254f2bbdbbfde900bce36bded32e7b25cca54c5e284a8286f5f3ff2813bffdf86dd4ee686c1fbf62860e0" },
                { "lv", "a8909ae4a4987394477a2d0df9cc75e13749b4401016dec4b1300fc2a5a8c2db5bfd83622f1973b7ad398f7f0b3e902d697e1a626276c6e6e2cb3d609b84db57" },
                { "mk", "529ee340385f36d4a9f1b982e44c83113c74eee14c574624d1d36b2d995f02bd91e2003d7d3690385a2bdf1222b748fb095179a96d0c51a424a2313c081fbb4c" },
                { "mr", "430e0209578b45851abbe7c502848c762afb1f0527df42e7b5f4c39f112cee23084bb3bf89340bd117dc246a6fe97c184c69303680eea426d114744de9656f65" },
                { "ms", "471455f9603327f074b727f13dd954e58a40fe34e2035de96dfd5b85ea27eabbff98915bc056a81a422477d8f02389476c01b0c05e63ad4e09d5fafa20360912" },
                { "my", "50b6507fe0f8b1e6a0f73cc0a6cc372583168dbbd9388e351461d2c62a20a1ec331e7ca43f33bbb534faf6f53ad0810b9eefe4c0bc7b3733c03456513568b424" },
                { "nb-NO", "c2f238c1506029f045f8e04be27fe653dd24f27285856d43aeabc2b3227a0f0de4e4aa9aace7851d71fb8cb5af559258b4baf78c768a70ab7d8c350813b103c7" },
                { "ne-NP", "b7e92f12ecdd4bed593e6a381ea86b500d865bca25e25ae76f637413e25d1ae4132e78b682efeee3be3365e6e4e0845199ce4dacdfe547fd40da7c5e3c7e09ea" },
                { "nl", "8b64b108b61dd1f1af187beb1287d32d07c9e498d60e992d8e2e67e3e02e6fac2f9c5a2d0d83386fdbb18f27b144d16ad27df06ce7d24bc4a3b1d52642ff594e" },
                { "nn-NO", "e15461eda8af43cde8a9df65537cd2fba84cc5fce8dd799f6d89a5da6ee8bce6102ec923af753bba07e4663a3b9e56e6fe47be86880b24a9eaaf2c66f58c67f4" },
                { "oc", "8db92feab014040474b50aed966e00ae23732a15d1de6e7a5f402e1b50e5058989ef8ad6a15aca5e3db57bb4ffc94ee576e9348937d2883e7e6b3902dd374092" },
                { "pa-IN", "176d36dee313800afc6da16b5b3241957e2e5ea4957b3ef2cbe007326251a25782c120af6d0ca0863ac8f2c4238aa47a9bc479b666bb13e72e61bcbeef4961db" },
                { "pl", "577c25915c3e04a239a6bca546afe9305b1d92f132a39b3edccfe6cecd0bf2c853f3ae82577b0cb3750b7b5ced7a1f43f48f2a49f71b6e44570b03534c945f98" },
                { "pt-BR", "610b1fafd1d1b5ac8aebe622f5cf9512e8dd947fc2ae872f0c58c59fe1950d83587b244ecf4b6dcc5fd013c0b176d65e838809d6d880f918f44f2769b079729b" },
                { "pt-PT", "bc551e1f4ccac82037ade255ca03a436cdfff3738615b363e9780b2718952dbc4831b9ac5e9712f54175abae0e7adf1a68baa5648c2efe630ea7dcb243ae415e" },
                { "rm", "9c3183230b8ac15fee37b5924e79610011153e2bdecdddac1e6414669cf57b2b668a97bcb78530cf4f67dd1a1128a99172f2e8c1a29d2fb9c2bb97f1e76c0bb7" },
                { "ro", "fa37cf0d9f6fe20b4f398923caf282ad1e0f7dd44b69e8b5cd556ac65f2d05cbcc0043d3482725bc3f40362fb79c9726bd2e076c5b74e319712e17aa4557ce4c" },
                { "ru", "2e628a8750304f90d89db354857314fe09ef1d6bf9faa96aaaae73d6afb971bdd22266d5ad79627629c22157e273b0729db875424872ce9cdb920b81dae32555" },
                { "sco", "66889bfec1e464c47b284e8f75bbd09c6698099bb60b0ae802389bb72731fccb68cdb4eb822e73c3cc436dd0e810eaf5d4b974497f75636f5a640f2ecba0d796" },
                { "si", "cd55880e09ae60f1bd2ca3d60327c6b2475d1c5c1fc41704a8d9c271e60bacfb156a46a5afa80a5bb12f01d71d0d46982ff5578ccb0d2854583c4b96f93c4c24" },
                { "sk", "403fec2b4d56341270f4d4ab4b4ddbdcd4941c8e6f4cb43c33011217e626250a60ea778fcfa0e1ea87a52ec0b0c3aff376952d65cdf81ae3c151a175b67ec03e" },
                { "sl", "41e6e7bbfccb01e81ac2f3fd76ecd05a9625df3f846190ca7966120c97fb62354ff4ff994fac010cfebe788c77508fc920b5cdc752160881cc715efecfe63863" },
                { "son", "a955b4fc87e8aa1d4bfccc51de1d5eafda849630e94ad25bba944a69fa405603f70066f28cf36250e2154cbc6e9e5d13cc02de7db63ac97fa3d80990dd7d8baf" },
                { "sq", "9a3c2ff40ae65d660f0a8bfcd8ca4f5942cfb9ef8106bb778f9e82f01bc9530fa47efc664f759d33f67a7c07b83137409cc1e6322fff22425044ddf387daecbe" },
                { "sr", "fa51379b0c498631385de6784acef441f2f549841818e112b3b9feeb1bb4def07d8b70b4a0eed639e27d1ea71bfce40f16e4cdca3667482c4176da41ad9b3369" },
                { "sv-SE", "4076779b0d1a2c00d0e63b9d868871a25ca1c2c534145df1346bbd1933fcb3414d390ac434826be12e0fa5cb580773f223d24c4d1c43054c0c28ed9829aa5190" },
                { "szl", "c33e52fecea77a75616936e31828e34ae75899f33561366ec634ae814e4d51391c593d5bbbe4d81f3a2b941965224057e9727e3544d9dd349eab5ff8eb804ef6" },
                { "ta", "4e5a5efbe001429cdd07591332d362c9d05a35ac96665b32cd188663f399568a7d3a30ea87ff2290e747697fb4c71373953fabe80811343915d11cdeee3dd533" },
                { "te", "fd6d5d7d26181438656992a193e11b8236960ef67c65eaa1ae63cda14f101e3fa3936e107c926f9927a41cc62151e111d319c89594faf9483b2a492fc70ab4e5" },
                { "th", "d666462db68ca70f9b6ca4382167f0d024d7ab953b5dc790a55d251180a4e348d11e04e62f49849360f142159f280b4f7f9684a8a093c415303607b76a32846a" },
                { "tl", "48fb127d9a5de25a13ecd7d38b2e6fabd0706076a958fa5908a12cb075705e7e4f37912b4e425f09250ad01c03147fb99544803689d7edb28da3481d9453a355" },
                { "tr", "e89a7b26be41a547e7c9f0afb26088ad47520b7a4a059a38ee7834f275d7d1d954d39d164f6e091d81c10267d4fa0a7521ff6e1398b19c052cdb1e867843cfe9" },
                { "trs", "c10aeb2db929f8761eb3031be29653f5ad88a765377b2f9dbb792f5c74dd8b3540a6e5cc67dfaa3a950761c02a9ffdf864541335f092b6855b9710b415036537" },
                { "uk", "b5df8ae8a16ca7775627058e276fd48b6da6e5cc9ceb640e2dd57cb4887b5cb83a58d9e114132bdd5e18e019565f73a862e5e1c967b2bf7b99d8bb8a22eb890e" },
                { "ur", "5768510eb600ee8ae9403747b5f89ef383c2e5a899c1014e2ab70deae001b1f3e6b44cf0c7ffe7470265e2c6d9cf81250b7e003f8d019a1e194ac0e61d90b986" },
                { "uz", "559318a063f5b220a0d0bc693ad487e798d7292840ecaf64be03bce6bfb295b8ab557f3d1ccacd4b0166d1a66a655c8b0534cee88968fe2922e1c12a8ad85f76" },
                { "vi", "e2540b1edbea73ec0f1265907143241b73fa31b3de31c5ebb005f037d8b764bf4860e9f2c1e95d58fab4843b439df8d62dd38f8d7d9d5a47e99dda9fa087b2d3" },
                { "xh", "a46e4849ba4ad6ff985eb127aa6711fd9a084814ee2d4f59ba6381a1a08d170a30fbc60b4822bb61e809b20e960e11e8390faedb10792afca1c08b596e7b3252" },
                { "zh-CN", "3f7811ac2fc0d7ba9e7ee36013403e03a69535717b76ac908b1d00f863cae9c12e29ae7a5a28791037e80a72f01254c0a4bd2892fb84892c60970b1934da35c3" },
                { "zh-TW", "39ea7e4e971f543081fb88e0d6f176a5e270fa021060d9dbe54d1181b7b5950c94e17c6d181c7867a6fbdc491d89f6d4954aa320bac79854e8d95dd09b92ac8f" }
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
                    var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
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
                    var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
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
