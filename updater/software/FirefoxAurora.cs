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
        private const string currentVersion = "114.0b8";

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
            // https://ftp.mozilla.org/pub/devedition/releases/114.0b8/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "1e38d6accd553a78afccd9779ce55249cd72fac77c3781420dc46ecab8599b2b716299ab213b3571c6ebe396953513cf32ff922c37510318f7813164fce8b04d" },
                { "af", "13b5804a19994c13034a61fd1d8640ef8e5f8e2711bfb78354412265f91595467dc222b36f83ae50dda163ad284346d348cfe04e77406b3abb320949500e3f03" },
                { "an", "88f894dfd4aca1a45de7b60cfd668beb5b1bf9a193621c3ff8d619dd6e3f1c3188a739c3885cb1e643a7c4f860b91580036b08334ed5d1ecb40e4cc29ebc78bf" },
                { "ar", "005de0531d02ee40bf561dd77a6dabf058f46bd0b006c8cb3255db305184006d93e3b5d8e3fd018d98180659013706b234eed6dadfa0648ebc75d2c2f1ba9d67" },
                { "ast", "b0f5c511ac99de30e97fa2430b617f70684d57594f752fb2731fcc1db7d8e2287188a215f8c3822f93488c82083c7634b824e0da3ebdb796fcdb5ec769013c2e" },
                { "az", "53f21794f6b401f95e75722d6abc334f17401fae082fab1cfda102c415986157075706022ac54d006d78c690ddbf8fb450bfad83329cbe72d7c77d96aef85b93" },
                { "be", "9d9e32433b1e94a2c1dfeee0ef99f225ea952e86a3953b5786604839997ea43170e6c9c3ed45b4400274ad9ebc475bcbe3fe1aabc507bc2a64c82c4f34fc98a5" },
                { "bg", "bb3badaebaea3de6fcdc2020ed051649bb3957aa6aa4355ac44de294ca7ac04d399de9c39b25597d16346b82367c5d48043e9290aeb879ce82dcae7d4ddb92ee" },
                { "bn", "778fcd935f79326d159cf9b4f6eaf5084b795880f5808ecd629455a91f1f5cf0282c5395f1a224210be511c961bdc1c91c3b1950a830809ca499f1686eda2a56" },
                { "br", "6f70dafeff2466ca9f648c8d7060501da7cc1ec2eaa4add4a98de67f849c09665ca4a9fc4ffacc210b29a12477aae207f88f3049289865107ce5998bb8669f9d" },
                { "bs", "eef5d900ecd64c261421986b2fc129c0d93fb53de601eecb73787c118c96405f09411db23131b7a57865fc94b8f17de1405eced93a45887625e9953d30a4306e" },
                { "ca", "08410a4310338864df8d9b572cfd983feadd097480c19da625ba88111e5561f5fdba06d54850e654ae69cd0ae0eac149677cce33315107de70da479983eae9dc" },
                { "cak", "18e226abe276880b7c09d418b1dd2873ca7fc10a00205b33b6b5e98733b2c0dfcdad41e543ac5fbb01003fd4c6a70ca26497cf7d1e342aabb423a35dba1d030d" },
                { "cs", "82cee77d4fee07759f10def1aa44c1d9b7f4b0aa2538f43f24c168359cd0e2118212b227f65b484133861c2214b3b7041171b4ca6119d5f7752adbaee8436fab" },
                { "cy", "2727e737588c09acc75200e38e717b1a9d82162874af66015a93ef8d49d38657bc4e96534c9f75467db94e25d37ab036261bd48107dee2211dd6f098e2baf002" },
                { "da", "7da36420de2409925d23c6be92fd35f3127b960a465b2018ffcce1090e21049090ad4e7b83f83d7224286289e9cee985c025df7e0ff8d3b4d2fd67a7d8f93d88" },
                { "de", "b41cdf34ea2bddc403d0fc45c7f904df5cdf6caa0829a63871cb5b8f9eb339fce091f420be5a15c136748f4a09b977d23adbd72ade974ea4e10e01a37838ecce" },
                { "dsb", "0d0f247671a7cb711877446ae3a03fd459f8254dfe57d9772629d4a9b50cce4c16c9d55edfd696e5148345e4de4174ea7947173dab204b07d28beae8799f45fa" },
                { "el", "e58875dee05ebc92463382a9b1cef33c8be2a72b9432a61fdc1d869b3dd0475903793bffced9ef13d8f9a657aa7ea5e89e13bd6a5b0e6cbbbceb35a7c495d837" },
                { "en-CA", "441e8c73ad536258ddc992751f1227d1df2293f2f59b0e1bfc301919ef9ac7963c5a7a8f78b0d77e8b2c4ef5cb2a0ad16ca94373c08e2e80dd8afc64ec501c37" },
                { "en-GB", "208328aade3bd0ab3656a6c75e8e46f27306ac9182890dd61e6ffa0232cc05ceb112d9f45e40fbf6ec1207b480f617f70733ef795e6f6db030bd1767d9737db8" },
                { "en-US", "7564656d5093705632d3144aec8749ff26cefe5798fafa3541425167be7cfaaad2c212960ce3a09bd2ac88b1963ac3a5c2afb0d3b136d04b91c964b2119f6526" },
                { "eo", "975151fe33b276eeff799ca26719f06fc5f6f0e24c279d1b69d79a0665c8ebf42537be032d38ca87f6381227cca3abb98deb87e9a1593d454194ce90787dff7c" },
                { "es-AR", "230f797e7fff43d4a26f16f6a28772ba63445b410512a28040724579f5327afa2b21c48fbaf8b538193349b474cd998c76a60cc21d69ddef58f0a81bc93a2e15" },
                { "es-CL", "138d7f0ed16f27a979e3c7d08b1092e93a259bfa0e919945efd7ace3ef2cae6875425a017615c559010f7eac10bddffef9cba9a57f6bde26cbd80227557286da" },
                { "es-ES", "7687b88f42729637a55966845f1b168e5aead62b1def1b4082cf68381099ff34db3d34c0e4596ed64a0ff46b5ea60840e889061f05af19583fad7fe97eda25b8" },
                { "es-MX", "2218cedf5360349fe3c193bd17a83ad2952ba1c34b2a9a27704fc1f64b0e7fb4c18996fea4ae78676f8426779bac458a6275aa844822e91a76932c67f79c52e8" },
                { "et", "7ac1e83ed36cbaeed678ee17f1b6416be32f508f6675936aa08a1085262a12e41e673307c591d3e5a64b17d463ed06304a87f13fb2a686f8dc535537ed07e40c" },
                { "eu", "54ff7c76c7c236039b1605d5f63cf9e7e2b8da824ef16eea88784bd5636e4d4697891612a2b4157ed9f432122204d0eb397e4e68a42e5510ba412c8d117e2f1f" },
                { "fa", "86e4f3f609828de366bdc5dcb24f203002e379beeb9ae00841173a6e647ef6b466bac1b87bcb07cb08a256f882aaaa5cb6f5f21d1a4246ac968f73b3e9ff3ea2" },
                { "ff", "86bf852d6d1212b8edffb1d3c462253cffa841c4c088cff2ecf76088daf16faa0aa55ec55ee09ede358d49753b649bae6a44f56a4547e230116518e2c7b10f94" },
                { "fi", "d8382c4777c933f2cd800aa39cc95ea6e30872390e8ea6bf3de45082dd4767a7b9e79465ea3ae1e1698df75e60f915f024c9c896a8811c50309507a7e155444a" },
                { "fr", "1140d765013741b252f43ef4e81d6eb94894bf7b192256e5e9e79dbffac83e1fe8e37648d5b1fbca847a86b1e6c584f17564d7d1889336f946730fe1760c001a" },
                { "fur", "01951787fdba8d45ed0797011c49e9efedb92113d095cb817aa4286c31409b3ff257f0c2e21b4d2d6ddceb517ad953bcef7b9f97493925af04fa536fecf2f6e0" },
                { "fy-NL", "8c4ec83779b5a4feac1ef2ed65d4a54cecd04521b91c3e699a9c4c78dcda77cac6f26e7e40e936e033ff32e2f6b0a4d50b5ea0506b7ad4d8b6e49b08e9cde864" },
                { "ga-IE", "4bad10a11887fb92300be70d0e86bc70500f482daeb7a6cafdbe303189183917152ed6bb5eaf457c7c552656c7a7aba9ca27e030c853794be1c392589f1df60a" },
                { "gd", "f2c536ce674c236bb37f18270ab3d3d491cc06338c708449a6c3ab0679af07f00276b1a02ef0d7e36f1e847e4e8edca4026c06fcc3dc4caa114c54a38c22359e" },
                { "gl", "ee72841512891150925c4dc66bea8b694a0d3a4d2f76656933f0159cc9a23d02e2a05d7b79d958e16c390f71fc196668a6793e467b752c4dfa93c96e1f748975" },
                { "gn", "2b710a12986e37282dcd53c21c526b952712ab5ad65b827558abcef003e65a4046949cb668a14c1550359182bd4e7900b08af44f19acef84dcf744bfff41ce90" },
                { "gu-IN", "5104f8c7021d28db6ef8e0e917e0c07909b13a3f90e747b9bd4d76d56d9b50593cc992f1e4a890503a832f7164b978e8ed24077049e46d4624eeef3a388dbcf8" },
                { "he", "480bd815055d96d651b827aec2fa02f91a516539f611166402724b1f4612d295ee52d4077804cb784ef7bbe0bcdbe30853cd33fd0b28aaf4ff29adac19f5a483" },
                { "hi-IN", "6f580b7fe56a0b309a210ded25fbace27d65b26756d06d333f0d9c097774fbc45ff0a51fc9bc042dc950f38ebb7733f7ca83ff0c6125e395a951ca518c1d0c87" },
                { "hr", "66ab7e88e24a9fe249f9bbbaf7b01618fa0ec25440b7e55c90650f90048614859c60a857b71265e7a8768a84225048b3d8b302028fbb89bfa5ac6a24b5baf451" },
                { "hsb", "2c0bcd9dd65b74f4b5fbbcc14cb7933c5c3582a43467d106768651090d85245559591ba7878f0348d904e9d5d1cb6f9a50b4ca79cf75b0ccaf5844f3f1288305" },
                { "hu", "43c39e105fbb17ab136cd704f03ee974c4edb18862580afbb961db7bf29a60d613e75c2f18f1159fcd43385a14cf8185a5e33cc05bf973c9afef42f27ea9d0ac" },
                { "hy-AM", "1a02768f3d97685b5585f90dad02ca3678a6661df2fd3ba0258d5ac3872e7839ab185fbfa3c712e094d5b63866ac3be4f7201c5a19a8c66821b248318333c496" },
                { "ia", "661d80995c97a3062a589528cf3aa7a8b209b5557fa90d3b4317028dc11aef568d9a4f5e7d75dbf640bfea8f197bc57adc103d059c4b62164f15e724107656a0" },
                { "id", "9933e6c48b114929183c6e437ed65fab854df67252f4e2dd150ca12522d34a329a5ecf4a8e4f022e4e45ce92d284f30430b30dc789c5f990fddfc2a19de83d84" },
                { "is", "f26db1e90155a429151cbb23aaccd0c285f315cf3cda74413c66eebe46641fcad079f7d826f7c35e97568ed84278f9b27eca9426a38f834b3dc39b2ae004d198" },
                { "it", "beeb5047fe605abbdb85177fd608e82ce990b44ea9b7109759518e2484a2e471ef4a7aa14164f70bd0bf188a5720657a3d728934d6c9ea01a827bf283d44870e" },
                { "ja", "699cf33f8b7afc52da90202f1fda465833c922e7862ee5cf83d9ea962c41bfa5ab0b4266f16eec5bd5456c089a0cae44e4845151b138993a6f075f48b74f8a86" },
                { "ka", "823522f6ab4743ebd6d1319a89b537952b1e08271568eabb34439d82d796d410079829dc76a8777a4ca99b1358c9495df2bf5193cf1216129196733f15dc1f2e" },
                { "kab", "e380e07b471d4c3c3659d9c18097e6e66667c7c210888629a5cb05820730a0d73f2b39b5658b19542f8b0a3e4fccbafe03e55f5f8d580a94a35cf2360c772d40" },
                { "kk", "34f128eb753d8c1bfa24e50561f2b08ce3bb27bf1f8f7f39e5e3359591aa42545eced257eb9aeb4be8b290b670844fd16473035fd39de08a87610b42cf1502ff" },
                { "km", "692bd87592921815db88d3570978f8bf8a2c0d1d7546ce3400315be1dd6faee30a0c1600d5a22749a843d2365a89521c4588df0efc16f3ce31b81d40106550fc" },
                { "kn", "2bbd7a56ee240f466776d65a3d397240c1368af7791402e236656003214e1211c8fc90af3e474bf0c2f777526c48063192e4adccd9a4ec5923d21810fada0c62" },
                { "ko", "7277e815d9b5d383dfba20f6dd18c983045d57559b5e843f66cc7faa478f602d28e2666056683e6e62ea5c02c78eb3c0d30b31191c5dd589aad961f9d8f0c788" },
                { "lij", "bcbe0d8d8492b064eaaba79a8293e67410e895662c25376c4b55afcf61875461e90d6e42ed5861ecc56ea5b19e0f4ac2908e38dc4ddd827066f8450c14ff5bb9" },
                { "lt", "5712868795acfdb8b726cb48a271a1741f72b668c15529695da3fc1ac9aff79297ab4f0ca392491978df400352060e99e7a8d5a0928c2e6648839afa97205068" },
                { "lv", "0e4af3ac21001480c444d37ddf458d14773962a80d1cfb99e98f2fc28e3035986bc5d1275809453c4f56df013999bbb3af3a2c4f3414f5a5c4aac7df7287ac78" },
                { "mk", "a602eb399998df9fad054e07fd6edb056f35d616dfcff404d401cba951ad85c417d118b09c1eb20672f68ff5f65353f11865404b1d650054c34c79c1364862df" },
                { "mr", "897b9c876bd2a442202b23611f95badfbe17d6dd088decccead7461daae6848c850f96379ad1414c01aa6db387588b313bdb6eacba8d2446d2f60c6e5606a846" },
                { "ms", "372457278e83ff01e397fd95529eaf4717c974edc29cdd3b05d49e6307aff436d9597647d73a8338cdf4e6c2da391389939425a0832454ae53030e971e7239cd" },
                { "my", "86b034d32cf601eb899ebcf8222d696804a3a6d9819cf4f6ee9401cb0701555558c4fd3e97a2e01f029c9e72350c3b9fb6580c9d672e24e2e18a031c7db225b5" },
                { "nb-NO", "3441bcfaefda6bd12fa9da3147ed4f4e06ce5516da415af8201d47ba4c8aefc0b64400db6a83e689fbce2753d81acbe54a58b573e990e557e0bf1337871a14a4" },
                { "ne-NP", "2f86f88281dbcfef183930f3781d0c049962bb1e36f788ec03005e974f63d3b0cb392f1cc4fd4a79dce48aea95292f814f37af5902c9c4dae5804774d49ecd59" },
                { "nl", "c2f877d443ffe1feb3eff9f87d95799187a5dd94d5f9b2542bb877230a32a5f6a57b3cc35eb0ba8efe9fcc03a7cc136b5e00b0ddac2b73110ab2b51f0caaeb62" },
                { "nn-NO", "9d9e4f51aebc2b674da45f259a1ab35d977d8064bf98d389708549059d655469586433bec5533f66d2ba1e3cbc573a8222b307f8487bb82bba8494636cda26ad" },
                { "oc", "415ad4e04009cdda32090a64ce040923c32a0bd316dabda19f6a49142f044d020ec875e7ec22edc27b098f67990c36318956b26252ac8f5a49c0aa76a869c255" },
                { "pa-IN", "1d098a5af4f93f77ae5986710eb4a096673b678cb9ca72161c616df7da3daa4d08ad948d88ad2806700c14d01bb6c987d546235103e8b0066288cfb326715914" },
                { "pl", "c12499457f07d37bf6ed3b5a1f562162685d379c9f071b3dc5d985283fc9cfa353c2a88284af3544edb7f4438d514bdc14f4bb8bba0ba445110332fe65b9bbca" },
                { "pt-BR", "4ba85e4d4f0a17d13683183865c9d8f8c12d0d588520a76aa63fc91fa4de6e8ef46a4249fda66bf1b2dc409556146710f80d12bf13c11ec627f312f17e24e13d" },
                { "pt-PT", "82b9b4ca79681721cfa00b7c53a7072edb49d94faa4620e84bebe2559993e256616ebea9143de04db70ee4a19ea1205a344fc0aeb6fe0ca9418e065d78e2a55e" },
                { "rm", "cc6f0d6428ba7f7ef62b7739139571bc1d0334f17335fa247da0df023f3e2c3c4d55c7fdd4f73a2392a7271330461b19df932cb71846a49e30f5b5102ce9f47b" },
                { "ro", "fbf36dcb91cf1c6c59b9f90040185c8edcb2b1fb601a1e9563daa3c9bfd9d0a806789af39d5f65f3acda6549ddf9825a73fc16765703c5e2db3a40c131a9abb5" },
                { "ru", "789e8a23efa3567ed23bfb31689bbda77e0c9346869e66b6d39c6839503b7fb2c2b4b813cfadda8f231ca62553d87340faed26a6a34e1c928a8d30aa31c8fadf" },
                { "sc", "e3fdc10dc2f911fbe44f7554034e3f11e04f0c72c5b509325bcbd15c13fb096169ef08c0016e86ccaf6fa9ccc112e0d1134d4ba91cb3ecbb0453b5ab84bd57ca" },
                { "sco", "33b25b8565dba933d746410b5f563c72afb79cf5d0d900b90fe9d9143361017732ec4a7d02aa24c5612bed28bdcc8b165c455cf81295ddd6aa9151ff11b3d311" },
                { "si", "3e6db1debaca578a6d12719f32cc4c26048cdb4d1ec26a6f4604fde5467cdc176c7cf05fcb3636012322b526f5acaaa17b67f70704b9d5b0d629f0191c8b188c" },
                { "sk", "07d2f3f4807f947376c58bad706cb625cb53afb68ba7bce63e84062cd37f3cdfe87cb99dcb5882648c0cbd78dafcd3237453bf361d4b00fd7e2d07b74dbe5f72" },
                { "sl", "281971ced665ab94790a70e5782de15b00c86981616b0d4be48964b54365fd3eacb1c134643af58c58ad8e45b130c78b259c68caa71f3ad9b84a0924347b71c2" },
                { "son", "45e2decb612a63aecd40712ab8e7ba2e3bb11a24a4e253a5aca1d0d39b2f54132a6ea75a53a45829baaf01a915015a56bed1f9a84a2e47c8c75d49b68af2f623" },
                { "sq", "cdf4bb329bcbad1957b4added1c6745e9fc309e39663614028424d3f97fe3dfa448677f9b045dc9f84064dec37243c358acc456bb81f59409d1b0a5c38f1d227" },
                { "sr", "e3ff0250950548fc9b5ea131d1e4a40e1257fd0fd02b378c0389139d8bb87c0c9e3c7202eedad44fa58a082ab732db2d7aaa662f5ec51deaa5144c9b4b3d1209" },
                { "sv-SE", "6932d568bf617c4375b1da7eb7926d7b3a56dbf208378ccd9aafee8dd7845e1ba0382537e576ee8a5b4366c64b4dc49a103b77123ab9fbeee536c54c0c1c1178" },
                { "szl", "83388fbee1e28d8422f744a0af6e8b18c02929646999f6be50af8727d368b4e8b89c757e7dc486df4f1543e9218215fd03c55ad50d5172138ce98c956bb707b0" },
                { "ta", "d400d8f51c81b588ad8fe17dc5d87b06a9b46a225aa35f59e7704aba5823981a522492b21f882e5731a22c57954fc420a68f5b6de1ca440aa931d78cad1d3cbc" },
                { "te", "345daa03a981846173c8f19c893ff87cfb72af924a41803686c7826481b8cab3a4e9fd23f517d74011181ed2c156fdde79751e9a892fba4da5ef99fad67ec5de" },
                { "tg", "11b6317e2c1ef2f23c2cc0432ebec85a483864ab0ff2f7b98f0631fd72c926b66bfaafd48dc2eca05d85876e751796e8099a6858c6969c6a0a1fe9b5eecb8507" },
                { "th", "193e556c7db1fac879e4bbf80090487b25cf61378ca863e3173f50977c8d9884223f6adf7983c8543eb3234d2d65ac3cf562fe7b1eb8747da94b668ce70ba2c9" },
                { "tl", "b0b8b7a1e8841f71e1f72d32cce4a41264e28ce440c1628ec59e1e6c564228e685dd3ffd958e373a9269b9b94e14715ffed0bf115321367da23a4336745d6d7f" },
                { "tr", "1181648b21fd587f80dc70946e50ff41b0ffcf5d63fa00ceef3acd7757fe3bc3b4a32b3a24debdf21356e332648e2f264afcba827242e23453bee74f2a812481" },
                { "trs", "480e08354f2f41834b900d1a76d22b462e933edd9bf513bf65affa49171a8972391d9e23b464528e0e5967e9abf16fb6a7d5d8735eaaefdf85faa3a862576b47" },
                { "uk", "a4d02a31242d0ff2c3a8c24a251e61220cc446ad9801f8d53a4c667257e7eff40c7376ba649f3037142e5adf2d897de8a072830552206c7abc02d203bce29df2" },
                { "ur", "442a03bd82173ff9661a0feef9242ded2b20321636033c13837047d69fe2486c510be5f7dba4bccd18a3cfdc11c1c7a1e91fb29bb5a119e52e9ffb4c566eddd2" },
                { "uz", "af59cd4ec9dc679d6497df9638737ceff998cdc2ee678fb7d416909c99e042f5b2854716a71294decd08f51e7d723d11ccbc66893b8b6ac4d76e38fbe3cfb6fb" },
                { "vi", "999218399eed539b94a0eb92d044caa4f499595391d6c645334ef2bec7ca4cee1dc7edb72a6d5d034ef6eaf2cd07b5ffce07f2830da17721c498e159d65f195d" },
                { "xh", "f9e930c5e18f6d0450fff9bcef8eaaab946da32c1cd098ab9383b2b12529b2744e9285f9a6932069d8522c26632e3506cb06e37274d3dbf0e1fff8b6a68d4b1a" },
                { "zh-CN", "3e870685e51db4f9e3e8321ddf15e104016c86eef44a1dfb6ff0e4e77bef3877d8bb5ad8a49a1610038821a20371bcadfacb08315e2fa9e6e90d0970756b4e33" },
                { "zh-TW", "7eb7c6ee099c12f3fb0a6066f43133e3903630ccce8df94220222e29de078ee2df9d1290aec169156712e4ca935aed5491e0e2fbd614ca7714cfcc39284a4c2b" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/114.0b8/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "170a80d7f8489a372ffcc0115c41dd367b62bfbf5247a3afc921dee2b03a4dd3f3f9893a57a2cd6873c5ef75765241240b2b6f7f0554acb6f9464b6d6737b0b5" },
                { "af", "dcb9df9ec7f54d4e20eecd43f00f2938ae73e0f329ecda2c81dca37b7665e1dfcb9c221ad4cf52a685f1a36fc0f820ef4c53d5fad95d43829f7a52e81b599dec" },
                { "an", "39469b258e36e8ea0c40187489f5379e193fd53ba62acff6e09e411037f8377bc90c0aefe68f19b4de42157fcdafb49ab0b0324fc63af2aec49a54db25177a0c" },
                { "ar", "b59f14f76925f6830dfc0dfbde7279a4b6a73566c007ca77db8a5bf61f86f200c25aa2b03b0c999adc9f75137dd5bbf407089db9c879c41236ead341a3c16900" },
                { "ast", "de6a3169be543ba8b5c5bbcc71db6a979fe1612e7fb99451a56ac8233db6ba6b9365845eda5fde18255db5c2c775606964b3496d45e3c9f387da30336ecb8cc5" },
                { "az", "468bf7dfbc96399e7e83a5c3d39fbcd468be45a253856d60720fad1ec2fb4eaf4c2bda785ad4444e16c34c668c335f51592e92a43ae72db7a6e91c5e22a95c1a" },
                { "be", "ec5b4c7793c35aeb7fb9423368023eec22411d245ee4a9a2a639f003fc60b4a7a5c854a0de6b2ce2f99ad0fa3a5ba20cf7ecd7f9e19f0d38c7dbb69d2dcba969" },
                { "bg", "3a3ae45a947920f651918b74d2e916b034089419b84e69be0681f2d83596d5c446d6dd64f6effce8af007e466f62ac86e004375570b17548c0e6ec5a73b3dbd4" },
                { "bn", "1a4a34a6a282ab1b27cbed56f48866a2ccd1d97f063bf4511f76fae5943f62fa515d36e69a388e461e4d6c4d8295da9456a3ef834f235b7af677dbb744b858dc" },
                { "br", "080f07958650a522327157a9a52fdde88476d12608e6cc97f2dc5894e125c75f1906148728855f500f053f46d1f7704cf717746cc6b6f83d4d02deab0ffd70d5" },
                { "bs", "3bfeb59f7c1ab60fac4c3f2c4e65fa55fa950ead59b7589d18d3769901b6f063ec0a7ee2865710067a37e3898922568365f02c2b7f61bd4fb50199a62cc99482" },
                { "ca", "d3c3560594dbfa3534e41c3a3fbd7b66aaac378a9f97cfe4ac80ed9b6f2731e91ed726c0955a9d9a95ff1b9d17778ad5b3c4587f6d58258db598244f9c26e1c3" },
                { "cak", "867627c25c576fe83f140f0e335da9d8c610a15b22a4122f0170f325609a405f857a6cc253eb6fdfef87670531c95e03bcb6a48ce3c28f71a7fba821ae3d009b" },
                { "cs", "3ad036eb398783664c5ffca8233166003a1d5f32d75cef862ba85a26c4c91748eb197bdedf27bb5157bc2bc944e651fce3d6ecbbac669b56a0b05865c251a447" },
                { "cy", "64ea5c23d3a5fca75bc3e821f8e23706740ffb270b287444b16dd4188d65d4fad00f01a2fc959ce2fd171e63c19d166361425e4cbb480671779c056a1e626244" },
                { "da", "c94e5ca007a6ffdabdbb769369fb8a53051a81d9ae4c58180200173c198a26a8b53aecfd95e457d7411466886b67a681b891eb9953688970faa38155329b72d1" },
                { "de", "08d9d42e7e488ff8ba704a51ac75368a7e8e51a5d0c139fdd66e104c1c8cf5fde05e3fa5db60f4e49beb2a5f908fbc9a9f49edb8f884f09427ec1008ffb0b8d5" },
                { "dsb", "911f0338b860e592c22a07e24403f322ed901834ce14569c3c650ae119348584e11b13d0f1d0202f4d0f4a6c2eeded12209d61895dc6c953753a394f780e7b92" },
                { "el", "39fd54a88f0e87a09d42990bf0f834cb2b3e8e2d531cb2faee62b5df386d45ac7362e6083404e3e6ace28f8695e4880b7249994871647269da6b9c5f373b9c8e" },
                { "en-CA", "6e6e12d59cbdcf357a99330def568e9a35683066b317a9bf04bc1988fa05abf0462c5744c68b7fc427512b2601e7275de0c30e54cde2b33b216d1fb5c70b4b46" },
                { "en-GB", "c542ac2a049544eab5e3e23427ce70c276d4a9dabf2f117f3f563f381739d81c937e06abffc02e55f51448465479d2ec3ca6de48e6632d79bd7a8ee67ebd9e97" },
                { "en-US", "acef3d67ce85be4badb6b62c645c7e4557389405b7ff9ff6e5e9d2bcbb8921055c50c3e8d89b5ca19c26374673df9004fef847ee3d5f0709c4292f0200e07143" },
                { "eo", "65bb622c2e670764310257de57c1ed29eff3f5af4143300dac66cf6933e99747158e98d64bbcb270308f396ecb8235e07d338fb3777baf9587f8b439726fc158" },
                { "es-AR", "c59fac9859801fdc3b4bad99d735516a0c3b1f978695c53c72af8bb9512bfeca3436a29b4f2fb10037f2be33fdffb1f508264fa1650e808320730cf349a30085" },
                { "es-CL", "8ecfa54fb3616cc325980765bdbea5ec991c0a955dce68119ddeac58940fe8b3ec236a0b84fedf40a1a2fbb94d1162eabb5fdc2c36e6e6f5437503d2473677ab" },
                { "es-ES", "7b2885d029c0fa1f29fa3991ef78a1e57e687d0b0780b94897d0841743b180d9701e1867295dcd8860d6f68100dd5b7e5e89efc4683c348cce177ab5c07ebd8a" },
                { "es-MX", "e514124610254f792eb3453af3772b84ce7287bddb5b5aab2402efe4836e192de8a3eb81b808381dc97d4e64e6685ce497add387860e5c4be48dbfd4ed4e2f74" },
                { "et", "d7b0cf92a1a4c935de9f189c87a4bd69ac9039dc8c2b563686971b703e39ce9a4c03202708614e5fcc9f558029ef6a5bfe3834fb175859ba8259036c0e0f9948" },
                { "eu", "383e8926efcf742086ccba32588c97b271c6e56db907a6d2409a17d23407996defddbc8d8adec27d1a746b93395d2a343d982acfe0ca6259ac2a115d31ee75bb" },
                { "fa", "0a4acef21a0149ae5a513bffa419a3fdafe32628536aa6ca892eeca0ec96ea62c7138610ceec698159ac01118de129adf3fea585efd5699c076e8512e266cf36" },
                { "ff", "06dde1edfcb299f4f12cc11d1450d787b5bef8a45125d1221dcfcb9114c4de4d02fc2300bc6d37102f661d7873e79f07fea2689758fc2bb57afb2dfd3489f0e6" },
                { "fi", "d9e1c1b5b6fe0f0a58daf14769d6772f72820cd0749d92c18baae747a073db950a1a19f9930bdc4a5f89fe518e7de5b574ec9f7fc2e428f46d76ee3bdfa12852" },
                { "fr", "6fa7b0ad2b597343bd5a19e12a25d5f95898bcbf1beaa37ddcef34ddbadebc45e670e27f102425ccd1a47732ee39d403e1c16dce367669dae8aad60915a1df35" },
                { "fur", "3555e750b96553aa9ed762ebcf2af6d92c710e44e037de3b22c8f45f36a89f34d1029f1a16801e26cb85cd16faf0e73284cba303bb6fa188363f29c9c31c19c4" },
                { "fy-NL", "724a89d56ff7b2ba562e37701d1195e06f3d9172ca165da0e38cc173fa16bef919aff4c7931fbacd61605d0f2e8f460ba7c644b98956309d4c2e5eea911b265a" },
                { "ga-IE", "dce3713d6c1d1124fbc68dbd809ffad8334816e3aa876402faba6be9b9a67a868585e22c599eb5c2d4070f0fb8741660c981f47cb4a2c1fc20e3592882232cc9" },
                { "gd", "6ec54a26efd270508f2ff27930dfb3e2798042c61f8ea08ee174a2c6dd3e2f39c314bdfeecaae9a9324a2578d6f0aabbb979508735bed71a1fcefc6aeaa1fb91" },
                { "gl", "79052538bbf02135c74342fa2f0ec88dbbff7d265a1a14b8b9852e31164a2a7829103af51432832d5e4c6cf1e057175c43a815b4e7740909247816bbf2e1a240" },
                { "gn", "f475f7a52cb1263f235178a05d8b00e987f3288a1d2a17b5acaae2f646ca3f0620b5c20bd1ce871f00791cf60d6834948abdf647f0362d3e99df9af6bcab3e1d" },
                { "gu-IN", "3685718cb6fde6efb6b627360fcaace81c65218ec57ff83ef63ba94ba2d3c32d3da0c3a97392bd718b8d497032a7cc795b5843e781531f07bb5647decf53894f" },
                { "he", "672bc8c8b742182e0f6339e9ae83eeb521a98d538c742742c6d1053c380ddf0d2c6947b3708c7ce336965eeb5f60b26fb23e714244ef18d014f0599786ac1bb9" },
                { "hi-IN", "a51e46584a65340dd3610c124107d683427d45aa232e96cd936e5ba92411a43e8a6502e7435f528bc89ed951534e35ad046070605f2335986d22597fb9aa0b96" },
                { "hr", "b5fa09c9114e5e2607a4730f52a8f27410401d9586c44eb5db59172742edf460d933ce34f57abcf7ce8f39fe017b86fb35104f5f90ad43b4bceeef296be431f4" },
                { "hsb", "b226dbdc4a9f3cb032d2aff34752e694f9c2f1462ea7c2f572be9158ff5855237a02561b0fde3169cde253b0f82a911840491015f56f70396778c713053a532d" },
                { "hu", "02f7241934ebc8ce2dacb939ebedaa83d4490352623bc9bf945341500352e5cfd2460036a8a3e4878dbb70cce0d81e973989a99c492d4d9c82526f6d670e5b7d" },
                { "hy-AM", "301d5e5861115a83fdc2a264e9f60226ceb3b9356770473529d2839cbfcae9cae26f3a3033e9044c92bf1b9ceb3d20f5e3f52003a71d1af8d426b62b6525a23e" },
                { "ia", "0b4906d97c09b25c4207d63c9b6c27067e7bafa01c09848075ef8bbd0a37c0fd9d0c9cb6449c6a6d59fcc66567e9c6ac9990c5d7946f230656a11d890aca077b" },
                { "id", "73f16e1a923efeb0b017dfd65d58e9de9f737baa8878235dbce4a6429e5980b6420685bda66e1011af28386576bc4fc8a253bdb71739c249eb7ce5818c42adc9" },
                { "is", "c6eeff871012f1357130c8b905b7bd5f86fce57f155f86fe926139deaffe055977e4b6d830eb741bbccb83fe4c382bd79d87f9770fc40c4401ab728ed3386f13" },
                { "it", "6a6ee6e815aa24aa3a124935ac9145cc669718080f1b894a55798118b3e90061cfe9e8ff54914abfab362f48645741ae401884d407a8376d3b3f667909232666" },
                { "ja", "8677dea5d61e69131d573ccd3f44e40aa6fa9874da0cb15a9dc5ecc4aa60a6093737f67074b2b95081c9122025bf400c842a8f5d754364037c090d2de7a93058" },
                { "ka", "3f579d3e53a7df34e6509d09d9579268c86d6e3e692bf1201866fbbc03fa875c2da22380c007155d83912312891e5b3942c53fd2112280a0142abd856f2d4329" },
                { "kab", "e73212288241cdc3960860a41e3fb669072cf80e0545c7c532340e5e2e4a0ac5e9885729384285a80c3f71b6dc9ed99cc850decd07bf7e97c6c8df26f2e7e471" },
                { "kk", "096399c46592e81b9dd983b272d7a368a73a4fda63b71d10a97d9eb109a72301a4e13653076f405f4f3e865fc2d637eebe69c47b21652b6347f14055d43d7056" },
                { "km", "96e5d187f287e79870687e8070b0931b7a17561c136fa4f984f6cd1e6aa070b8795c2ffdc97995299dc092d3431cee601a44eede3aaba41979497887c4c60201" },
                { "kn", "b2dd4e68f3a45c93feab161d2904f6f206daba4b4aa4a919b9337fe3174c4709c01c5e52aa624181d6f9cb9a29eac287263a84abdc13b4efe8f2385afb73d58d" },
                { "ko", "7efebe9b7fdcc0d0497f5a2d6e92fff556a8b702ff9aaa3c75f7de56cd4fb8a73a6abcc792cda30db666d4c91832b8f1850780cbcf4e55f5549e3b47f85ee290" },
                { "lij", "ca49e720a6da9aa57045b58842e9031a2661248ddfbb10652477d6063a1a8396b81cd92d3f6f837c84bbf60a086b9c7e80f3b27a9475e1da8dde5a502c351175" },
                { "lt", "cf31d9a7882de97f46fbfdba86c7728a9e506135434efd1eebfe0e622be49f7ee3bf347e6ccfe0e3e9a061570e837f43b3140b7bc581011fad0449196fabfb48" },
                { "lv", "e7f8f9431c2180950a88457dd7a6a5ff95f54bcb926ad5ac257bd693bb5adbbc9a1f9d91aeeaf226ba9287c852c60b2432dcde9126c525bd7231ee1288020831" },
                { "mk", "bda8f583b7667fa4e01ed39bff644bd8ab83d157701ed1a9eecc6be2c6d18b6d76c9cf03f8f17d737baea50ecf0125054243efa6d42547c2ccb4037056b34ac6" },
                { "mr", "fbc742c3d76725203a9698f3a041fdc96952e9325f784624d5e1aaf6d5bb03c33b821b7efdf9c768f17add26df4d6d751850162f46cf4898884781b4831c8b2d" },
                { "ms", "74454c8e9334ca1dcb67704acf5e49d1640f837ba58c56b37caf3dc88a448a73fb19f57ddce95bc3f1b9962a52bc78b4442bf1f53dd993d568d7e6d7fb25a452" },
                { "my", "e042fbb7f1b2000dc91c12c8dafd707d5e46dd57a43f5977473d21d0dd504280331ddf15da8afa6ae26575ab959cac6d04a79db578dee34db1ba04ad446f0cce" },
                { "nb-NO", "ab9905b91166f1e095f5a042501578d4c308a939dad8e088fc3f9de1a3ec0a2ce5154e22f064e671a6d5928c9ba2fd19a3f50a852cb3d06d6512190cc6aafcf4" },
                { "ne-NP", "8bcf11208c86fa9f149c8b6a89767258dabe3d99874fd9c6db65c71b8b41b4a791dfb4dceadfce7f60898bf4f1b428ccdcc542bd2f262552de22d847421e2934" },
                { "nl", "84156c09a748b3c80b62c0c2e28be667d9ddb101c39cf7045cc28e9b72fdf355730018f67e7d14aba362346167d637d840b65656a3cf319c4b14de25ffa71114" },
                { "nn-NO", "3df2ec1ac11e75a3b8793fad01489f8481ce47a9f73c11ba8261042fc18068d0ef690e79d6ccf52f3fa64c506c4b7d34893a328e2fde135197da0c99280ea4c8" },
                { "oc", "2e791a7745514ccc8ee456c884afaa1dfdb4d6133122d80942f8aef4c75ae29379cc2b4f7d34b438590c1a0112cdc05c6548a0c996aa8e4b7f4c32a2b6463ecf" },
                { "pa-IN", "063886f271e4c0719638955812321101757e04d01ef49afa3086418092597819bb91d9c23e59ba315712b00e6826b018050a4d9dde050f7f41aaa281d93f5bfb" },
                { "pl", "3af0ec2f98ee54f98f14a80e8adaed770e8e58d6d57e76a4001783bb5f6b2f404f8a67682521ec6cf35d093ec952c1552df0025f102dd15218ff570541ea50d9" },
                { "pt-BR", "e380663b38853dc9f17ded6fd55e3f50fe858873bc921ee1847f9cedd0e7fcd0ac0ca42ca865706076bb637044295ed6cdbee36c29bc23f58ce1b82f8ec79458" },
                { "pt-PT", "d16d9287bf9083d22bbfeda41304c7bf892b58c6b06918cb501a0c6ce220d1b5155ed014c46110e3708788165ebd51d8740413bbb659295689656d170a07b2cc" },
                { "rm", "735ab55714d78c1fd3be98aed2f116802826acbd6e306a2d418eac62dbac909e893ec7cfb786ce1a97b22780bb2992ec8b01379f94d291ff264007c8cae90464" },
                { "ro", "88fae71e3474954b0a17a3da5e85bdfd88a9123d835c1cf7c081045003d5cae069d494d09c45c3aeabae6f555759d05e7f1ba9eb1d2d7c7265b6d2ee53e23a19" },
                { "ru", "4d10559c5c2ae6bb18c8e7e95caad6aa2b45b92c04665717f3b3c3e44a4945d89dd84d83ed8322453bf955dedc9ab29e15c67e06e16bab85171d88f41cba6851" },
                { "sc", "b84ec1d769b9fcba2b6fe618aaceedb1deaa4857fe7f84f0de8d7f31e74e3ad7b4d8f46e59d8ac45d24def955bf3397bac557cb0147ee54726ae58b68d3024b4" },
                { "sco", "2abcda04581c3c738adda53df75890b623f38bddc89eb384b7304d0abb2a5d1ad33ae79fda938771bf5929eeb6ad2882c6a674604b15fda0fb5a49ec6f4549f4" },
                { "si", "9e02443fb51784451e55955cdce2c6e4a2a600cbc4059ef7c0bf7788608922d30ffeeaad8a2febfa89ddcb3d64ac05015407597c97081e58f330af6265102418" },
                { "sk", "804e0e85be7037eb6c6f7fee4983e2d0b279377d935b3f3a3f2eed8eb8159db9e32e185a682b0f6f274aa30f64d3d3dafe89f0ab8bf9ff545cac80e3431dd568" },
                { "sl", "98320191c571db42a056124d46481aac78a3e27e03578daec176308a23b81b33db47b59aeb6217fb0ca785a235ed231418000893bce09b47db70027e891d9a5b" },
                { "son", "3f08534f5a8d0fe53dc465c9b030c6b650dbf9303d13d365f9e23b64da4faa04d1beb1563f869bceb8ede47ee2b74719ac855d4b80815d3d889333ea32ab628d" },
                { "sq", "82618a440d1780d130d21eb186776dfaa59a799c47d7403e8bc61d7c2fa160c5370d27c02604578114dab2a816eb5025afeb3e9788ecfeaf8f4d36092c1bb0d6" },
                { "sr", "147a0d2392f0f957216501888fb2d339d5393c34f545c65dd814d7ca0f078ceeec9dffb66767a591d9b6f8dd4ecb6aa5595937a191cd220fde2856f6dd9374ab" },
                { "sv-SE", "b53bfc8800dd66b0bc4f348b693693f51d13177969c3f8a5463478631ada2038ea486068c1604f4e841270ddb81caece0830b4e5027da0fcd3084bc969ebcc2b" },
                { "szl", "bc3e13a22163231e4cec9167057812df8aef6dc0554ef976422a24b964a163e67bc5182add2e899a96b242ebe4292b2cebd279fc421e4ff290486f35ddfa3ec1" },
                { "ta", "2ff4c08cc0ba6bced3fc84617c65c8c2e1eb49dd5bc40d37515c019c99cda8d5c7cab57dd6809aaaacd87ad7a4fb40951a919c75eac6f5c813041df8591aae7c" },
                { "te", "79a2b608f30006481f7ea2ff97e2ad38dc788186728634461ea690216047c47fd9877e24cd8c0262ea9623bf41264d27d9b08aa1cc1612355de36020eb834ef4" },
                { "tg", "3a34e4da35813ed8bbc92893cd58a071590c4daa14686c03a8c873551f9fa56a420919a961760ca2a76c172efc8363072a3e4044ce5e5f7f24996c7d9089692c" },
                { "th", "94e53563eb6bc4d3e78c7e7da446a4a33d2dcd3531e7e18390803985c0a2abc0b84c09077ed7195c8fcec5358ea14915c02b4bfd7933e68029ca388341bff466" },
                { "tl", "c540e0086cfbb138b93145bc2334f3a83e45ae9a4c87769fd8e74be2f43771eb45a082b428939f70663b854c9b42d23bfb1bf8654d8da2938dbf0b0fc4541b4f" },
                { "tr", "e8bae25b240068ecbc9f13a41b1d5383a7280c7d2cdb2ac5591f3d5de544ebef1704e8e5ef948c170253d0074bf23d93adce33ce418b0712091e2866d08fb926" },
                { "trs", "7d0d1805962ad18f0560e2bb1a9e31905c6b4de4e0887d2386e4ea7b719ba1352450754ee321c5749142085dad01bfd73a1481cee8ccc46cc4dce6b05f08d229" },
                { "uk", "08cc995988499b1b6674b798ae25a67f9bc9cb6c1b4af6af56bfb9552695b9677b404f0ea98f51b82d6b7c7226e39f096c69bd898df32400761cbd1cee24ebba" },
                { "ur", "db62709e40fe15b50ae4fd76a5c35ecc2f53fa3f1b404e0e2f8b5ecee398c6b0b73839a485d6239a336244586421bb45ccd86a5d0f5bd45c2b4788bcd9f8df28" },
                { "uz", "8b94480e0acb6059207f6e937807712bc33b351a5d14ce089cf03ac4cf92f3827e23e4d6221bc32f3f74be09606b2b11281459ded1b088443ea48d53fc49a17d" },
                { "vi", "a5393f1a5fd07665fe7e3e335ca64a4575e6278fe8324dafab6ddd0170499d7fed29718c562dd752e497fb2d323b86f27fcf791c6fc9046e3453c8c43eb9585d" },
                { "xh", "af278bac4b3902d56fb1416323c9e6197a27389dca92a85fd9c379b0f7022a67f413a914fc05d0d29a560c53b6a85e6b4483bb2336cbd1c38b867f20fd9b00cf" },
                { "zh-CN", "f82fa6034c81c2919271acdc2f6f92a478d48d57f5974786536b2c0a74519ed51499facf7c45a81eb1431e4928a228223500d6dcd3db3fc188c5084dc50e250e" },
                { "zh-TW", "16b3856b7c24172d7c22053a21ec4b40ac9cc2e8028f315e845df22ad0319d44bc31d10137739af2237e62a9c6a7e895b7bda577bfa6b5f93edd7847307d3c9f" }
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
