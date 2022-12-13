﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022  Dirk Stolle

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
            // https://ftp.mozilla.org/pub/firefox/releases/108.0/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "6a428d45576abbd11b59747e9a082ff9cd1b63f48336168ed16d186db85dcff4cf89331b70a79c1212c5e353c8b811ed58370b896c609b72bf706079e87ef14e" },
                { "af", "3cc7e59a81e2f98e7486251d410aac88212fc1666f6db48ab9acea99b69a790b5f3da31714fe7dad2c97ee294be2746f70527ad2f90b5dee3f62e4ded97559a3" },
                { "an", "b5c68bcc64ed01f0e8e8cb3782b29cba794b7b8399c400fd928a6cf96baaa92bdd7feecc67191e05b1b88f226557f618002143e40b3e1ca9792a224970284b0a" },
                { "ar", "d6716a962925acc0cda37a51390cc16ffbbda3e56a5a4046045fde661cc0f2c756aa60a469447ae7fada537adccde99ea0e2ea3d5ec00bf35c9b909d518876b2" },
                { "ast", "f2d1a98bd0dea09d5b7a572c6ab2b542d7267a4f93d888165fc0481d86123509a4e4d32615a3f77648febfab028ef39dfc593e6ec9d795baaba4be8c4b129ad9" },
                { "az", "3262730fcb7c976e4ffcd3e1f1f9571fa25597286df13677bc633d153129a0e32fe0835176d6142d9bce64a4a12f10ef6cad4680b84e3722dac5020b70c9f373" },
                { "be", "f6beb34d718f3fdbb0db8c462ad806e6e70e862201a1c31de9313f8a3e654cf9b7e7b7fb762b75826af660879a3e5b4deb8b85973dd878054a7c3231be7d5c66" },
                { "bg", "aa44ce6595dde8b32f2b7ee07f159a3254db3d3dcfdbd2f890d58ec53309fee1874042cecd580dfefe9442a94ef3529d946996496dbfe4556c73fc32b65c235d" },
                { "bn", "724a8e7249cea7330fef8eab660f49a6296e1394e41b7a4f77f8a4351529fdbeb92796f42be34ba42a12812da70c91976b6a74360b8eb3d6efbf87ea557010ee" },
                { "br", "18e490d71f1eeaf48aae1d968b00582b44dbe7f527df900e5e5f48a23bc53bc8cd890cb6d29d294702f610f0f8b0cb1ec724636353869f1d642e3ba7f921edb7" },
                { "bs", "22f20e2e486aa66f59548edfc61fb8835c1182b8724094e1071da73a8c72a5215203d9051b9e0f8540e91c4df85f498997cb7cc8c0223c68953d0d7cb344e9ea" },
                { "ca", "a5cbd7b0c70a165272edb138153a50d9cabd90c100de5d96efeee9be6e3811221a8cacdaee33d5eaa61d0eee5c21afa691b567256e86e91b45a57eeae3e91daf" },
                { "cak", "79d74fd410cde05e09256b979b11c30029e6fa12baa231fe3925469b2ef6802eec88f9ef259eed7b5bee86fbe6f67434094d6e1895cc389d2e113929f003d552" },
                { "cs", "a85fba838960accb33e753516a0d12364a156fda92c9b027e2af447847e5da91a4f651968504d9c5bdfe27554212c61c8db67e2dc469f433702dc5879e0e24c8" },
                { "cy", "fc6ccd5813ecbd8aa71f54e04301f37d9d68194b12faca2a84c4ae334b6b8bb9bf1ceb4caf0f699fe876463a1e9e9f852a2f2e323747ba31e8c0483ab47cdc7a" },
                { "da", "6002f1d33a579c70d7e1b4f8b23dd64d3d8dfddea04584e19009e4ded444783d96709e8bc68f1390dc7b0df5c798fa9e41b5eb77435495029dc5d094c608675d" },
                { "de", "2ad5635ea18afea15a69175d5d6a31a9b4fe1628a278fe81f01dadd92f568d86c522aec7fa2275096b541877d44d3a16a58318d45b3ab3a0899f3347312c2a6a" },
                { "dsb", "5f94031b8fc042d32779b8f02fd148d3f26f6e0a89fdde5ef51d1ddabd9136b6fa0deb23932befd4251bd83482103ffb047eb0a7eef0dddc17cab35f2d46703a" },
                { "el", "265c409ab1f78df14b6997e58c147c42371ec730ce44adc753889e8912c3e314e7e1f1913ab65e1c36be24fc2a1200cbc9dde33493e4a2247b8b11085a6501f7" },
                { "en-CA", "e6e221673ed97c524b4e45aa1e87e8a94873ea84f7b075c0fe829d4dd59e2562142366a92fdd8957c5a61e1c826a6db2093fcd0718edbea1e88a3c3bef7d11bd" },
                { "en-GB", "e2e73d6629991c125710f2727eeb397f08ffcb79e2828a99fa1ba231aaba7b92aa8c7d394421945fc9f19f2f8050a683a5e9f3b5ee7bf1fca149be303ccee873" },
                { "en-US", "8c497d13e639a437f6020bbd1ec7e3a0eb651e30cb6414613cf8aa092ca78789995627baf4f4987148ff433f9fc44d396b9548169c9a6f86bc7d0bd40fc08e2c" },
                { "eo", "42ec87b033b4bab07cd9a5957c37f988d7609d98fc130d233bc9ddaa4d3cf3504fcfacff90aefad2c4ce086bddb3c41338f96e3423e7eb7579d31bf072e4c461" },
                { "es-AR", "87933c8b3346b3368d73f3f493378446f68ec5e992ca31d3741ce125bff85b0729b6ef16c0e13ede6818a825415d9ef9066134b4f8a4f981558d77eb743f6dda" },
                { "es-CL", "2200596e2d8eb067757119a395c83269f22e804ba6c18288a66ac2834907b822dbde3eefc7f952137a3409a28e9ffb6f38c4bf0814c447355bcee8f48635460e" },
                { "es-ES", "7e83e9ce40c13f9542edbe71043e6546f4e213856f8aaf72e8ac76fc57a9fee46a2e31b010aeacd6d7d8e7ff1298cd7cec1627999162f13dc2df7ea3029b2033" },
                { "es-MX", "a835967400afb1a856c03ceae96d9c83d7eee01c8ae2aa9d11b80021f16550a2ce7b2c5e4a27aaecee172b3ea80b0475299d84f81cf69c6c8ca4576e30d8aef4" },
                { "et", "630b20d39272397bddacda7a0ec761eab5d85cb9826be2ef409067b9b1c80e6ad3d882471a94aed6978bc1a455e350175927dc51afa9db22f2d06cbc599a1029" },
                { "eu", "6b666a7bac288f19b3410393f8b1278765c7bd32dc2ef4817f2cb54284024a9d8f80cf1c2db3777060cc4963dc7817e9ff02883915f89dc0fb4ddd0716bfe579" },
                { "fa", "2a64d75a7a2880ecd37d9647c6728cb03d74219c3dae216d7af87cbba565083c3dc5eb8d0f452b866006dbbde1e3171a7493b8bb9c1b3cd82547c72cba63901c" },
                { "ff", "fb7b13cb5396a903b86aa30317631073aeed7557464e79b5db30a2d2da2af19db0a657a8e275dfe5d729273ec9585c2fa35985681d86f983a73ad328eb9d57b9" },
                { "fi", "131b8cf67f21d3df545f1d004edd8ad3f8c680cd4727b73346bd15c6a63a564b97264227580a382479d0d86853e8357ac2dc1ce448a3e82d348bf508da10efd9" },
                { "fr", "482e9f2e4d540056903ef8c75bd3c3cef19a417acb0d983b2745611c71daaa2553387cba720ae9c159b562596e90910a6688364f4cb22a4a41dc7845ff2346bd" },
                { "fy-NL", "ada17c5071eee8f1e7dd7c67267dabfd60df94f6f94fa6ae2de0198d8ef22e4000a52c49bf826403bba273b7b5db28d5ca433fd633624b8df0ef9de9604ab0e5" },
                { "ga-IE", "43022e1eeedcc1268fa127dd7a8fcd5245de992c0e89cb9ff89f8083675d676623f3ebfaaaf77ebc8f63390e440582b1e88acb965690e0c62f7888d8f5500eb5" },
                { "gd", "34053d187ee2c404646d38487bcd84c8c6e4b03fd7aaa81e40cda727c8fbebd4f0cdec2046c80b5eb35bfd9b2e3983737f47ec5d4d50d84a30c59399584c884f" },
                { "gl", "6cedac36e49e32d7422f1b70fc4559fd2e0787411da7bd24a32691657e5a63149e6288ff381c4b570921ea259b926f657164185e5ee230721cd8613304b38c93" },
                { "gn", "70579c68544d0b315b0894afef23844db3bb84286483552c6972abb3f137b516f46f7ebda24134bc55b59f0502024a3c5dda04da9ecb92dc6e968ca7f00469a3" },
                { "gu-IN", "a49633b1aaddabdfb5c1f34165ce9e52301d29638231700e1ae884717b6980e13b9b0d759718d68006dcb78dbb8975a01fce7b0e2c4d0c380b45145045876564" },
                { "he", "d7c31f319f8b66592930f5d2450043b5f7d0bd1325e5f2f285db7b18a760106fa12f50315a9520a2b5759be0ca0f7a9ff478b4b0e068cff2e1b9ef06be1387cb" },
                { "hi-IN", "f30c3b54822d07bfaf2bffd9b9274763eb40d8fc3c51991fa77bce7eed8c675c5a55afdc5224caceb5f606d7308cd15851811d0a893cffd2813e5ce2d66b625f" },
                { "hr", "25349adefb95f214e11787d9984538d3f10d1017a12de913641bcf048eac12e25db96ff271c80a62cfc2218c39e0677c4363fd7a2552f8d541eb6b907ff02d1e" },
                { "hsb", "c138b40d5405e0104cbb87296aed46a296242e2bdc175c14b9dafa217c5541bb2297d5c5f9e7647f96f80177ebdf5301bd11a0f546b2e4869196af34b0ac2c11" },
                { "hu", "23dc8eab037f5acf19512ea6fdaccca92d9130c97609149d156d6f8576da166ab3d88873f3845c5232f977ff6be58e03881418d8f59b7efcb54bf4d78a9ad933" },
                { "hy-AM", "4a3bb22313f882fe67c0832d9a4af7f4c8fc3038a7930a40ec5bdee46b74fb835aff088d2000a409ce37b4b39d133a083dd0ba809265d24afc355d1f3f8c7b0f" },
                { "ia", "7f0cfd8bf5f69be96d4d598426244cd8cfeae52aafd1c9e4d3ab205137e63edf420c47cd46fcc154b636e580ce4c5deb90701e52bfc829f320c0b01804a65fad" },
                { "id", "123d9de14676ad6b52c34570a32b41c690dc2593003357cecb66104e778b58317aa1ffc35d9ec3e2b27fb1f4b3a0a077d9322cae034b4e0cb3980adbff6e0875" },
                { "is", "4123621b2307158b8ed6d0c64c00b442fa1fd6418d8e97db4628d028527a6147dfb708a2a4796e3cf7d478d697d4417e999255da1a12fe119a65c1d573338873" },
                { "it", "bce2c78f402ae822af84efa691185e126b763d8c0a5fe03e4f536df994d0dd426e928e7ff4b777723acaa63debb0d400e84e6222aa8653fd7a0f5e56d4ca00f2" },
                { "ja", "8ab9893ca628c77578b2aef390c4b107dcbadc78b0943f256465dd5cb4267d8527828092e499d620eb94afd24abaf4cae903ff394ee8a80fd4117734e6e439cb" },
                { "ka", "feb0f6ee3e385200cec00f203a0ad7efc93b0c65427d40f8a481b06acd1b9b60ec32093675d5eb44579f5f683985d125eed0ba0c7353b51114e7c567cd4bfe13" },
                { "kab", "2ff7b7defa5a5b49ac7c438212249474f49a844d9f975d64bde694f1cbdf11bff110e185518d9f4269a2586050e14ceece1cc1bdd44a1474cd975d8c19b252f7" },
                { "kk", "d618368bff19b9d9e778605f238b81a1329966b83bb0de000c99ae555760bb7aee9c0be48a24ebcfa01fb8c343da03496f93be1ee838f0cdfb79f6e0d63f13af" },
                { "km", "413fea1b5aab099d410bbd2cc5e96f7067ed8ba4c7fc3a07f50ffd2f5b6def03a5ddf20359bfb1370bd66e85046494bf4d1f8ee28c43c376fe430686c51c004b" },
                { "kn", "45ea203ed1d71b252497b80857ddd258ed2bfe6bb17f6c980c67ec6334658fa329321ac9296545e23c419186c973098325d57e8dc857e46f1c8c6dea5e2ddd2b" },
                { "ko", "e11d3a48ebd2822aa00a4a6d25203a0610d1db1fa17b6b7660dcce41ca3249dfc91ac7e13ec47822c70e0580a87d4df23c57f713fbf0d0cc568b4884f9e6c123" },
                { "lij", "26ec2ac70eaa9417ea5a84a142afb12b47cb0594f74192a86248d13642006ea5f538d3bae6565f32ac129036074bf8bbc6160e523e54e6651837914588ca16dc" },
                { "lt", "da1aedff2cc1819cdefb4a83449ebdb5c7ae8364d8f02c6d697af96aa8da12f39ca60ab688c4a85a564f01855426a82c4947f592c72eedc926f76ea7848fd020" },
                { "lv", "7d4e3f26dfa41845fdf19cab5bcc7f9c96e703c5408edf114eb6f75e654a3b345f0d8a69b85591597882c9031b52b5957a1f006750888df979c8a0c3d4884921" },
                { "mk", "e4f2c2f90bb6284abad256157c038a024211f1961e06614bb5ac6f51c8af2b45c43cf685e26ea9615190a3265f2632f01e2e8ba126a091d18268c9dea6b467f2" },
                { "mr", "ec448f84630cdb21b7ec7611b2072200db567f05433adf83c76be9f8e7fca8863c886a704c4b2fb7d46ef3dcb5393958c1f6397a4ffe739f249b98d49d55ccbc" },
                { "ms", "1f90a662311026275486abddfdaab2d2f37775f90047b06599517cc9facd43a623152100a5e666b197d9d421724ea7fbea496e58188a057afd5c34ced40aa044" },
                { "my", "7a2ee7014d97d128770fc47d8a010b50e7e1fdc2b8cf38873789f897ed028e16ebe1afa2b6a5670d432318b0ee7194ab8369296c83fdd6233086bfcb1d7fa0cb" },
                { "nb-NO", "1edca7454239de152a9e8233bcb6390d3bdc1362dbf78f1d39b3ff14803dfc112fbbca31c162039d1e957873268cffb14d8c4410ce434d812f204707561a7567" },
                { "ne-NP", "8ac83f4b4023949e9e9e2a8eb6056f4ab9967837febae44d183ebf84109cbb7b5a9ba2751f2fc888dc29bb13af541708ca425016449d5dba6ea285ef72cacab9" },
                { "nl", "58aa5d33bc58bb0970de4336ef7d804f333d5b4690225df26e09c9984ab7e258c37eb834e8cd218b0e593c29b604382a7748b2fd4bb4384bbd7ec65d38945dca" },
                { "nn-NO", "04393338c8ca5846de61ade5c7f766287f96ef92d272b2b101bba6922c114ced6807e9f106c3e3cf8d57be57495e4257ea67fa505e51d105e33a7ee6565c51e9" },
                { "oc", "e0efc4ed3442afa4732321bc70365bbfe3aaeb71f6c8e20bac58a75c6345724cdf25439827c1b3a4038b5f9762c406392f64c79ae2b51b29e1f663b4225a3ade" },
                { "pa-IN", "a2ce6257d8d19a91f201e6520271b0c086fb2e6ea0e4a87e015b98e06022ac56ea4d8cbbb89bcebc5572e5d7fdb6d606aab3a6cea224ea63b623cb625d67429d" },
                { "pl", "2986bcb8ed72353d6957beaae65942d8611b57a1a3a426aeb3e2de825bfb5578c3cdadec94849d1f1e83ca765526a2c1ec5377675b3688f05fb45e382dffbea6" },
                { "pt-BR", "7b3828864cd4b576cd266c374aafa987c90bbe996fed0406d39c0bb99f09559c03d9586d4ce137dfe8e2e3125dbc05865f1e1798468103ce03a2beedc75bb87e" },
                { "pt-PT", "97945038035ff82e5a8fc3cb3c326368208533a2ff256e4bdcd5284a870e0dac7a96d06c39af3489199a7c107bb2f97bb84baee00931a724b7a11363bd705ed1" },
                { "rm", "e9a8866c649a3246837c8eae85d579d41449d1ed1fa0727325b1d8efecdde00d13597aab830f80f153c9dbb0d5e927f0476a6196baf3cba7749e983ca24b8098" },
                { "ro", "c8f00ca4a4a406c34ece30a0e7c25850922dcfa8da1ead368c5bca9620388dd4b52eebccb56f8c7ab680a3f2494ef092ac7008a32ef52dc00b8193c1d85af3bc" },
                { "ru", "52c248e3ab4605de147a9c3bce1d35d289058a9ea088ab3d42688495813d8e6d8eebd9a87ac86ebcca6a1684f6fdde92fbd69bd921bd68a1207dc2a7a2a8592a" },
                { "sco", "f9dab30c4037cc766f70d59e597f8a54e3bb6641893506bc3dc1ca55aa25f2407ac1278017abac95b9330c12bba08fe12aa339e92f01880c55acfddc0ccd5ed3" },
                { "si", "659b82991963eb24683f3d421ec1c3bbdfb0335e3542abfe75a17b16ba437e409813dbbd013176495c1776525dea38fc8d946c32e7cedac526f9bf04255760fb" },
                { "sk", "de8523ed123a92301fe36002905919257f1aeada0f8f9ecbb9ecd8ecbc3bb089b954546edbf3353cc5259cd41cef2f2ca7b7ca5125cbbf72013dc536097c9759" },
                { "sl", "39069473ebdb904ceafdd6d7f6692895479f10b7423ca8409c441c969b61fba88c148a9cdb3a6b3ae14e1f1babfeedddb23a1e1aec9069a527b3bc748010fd73" },
                { "son", "93824b86362f8119c31b6b8a61400b0f6a881c6cab6c4f0a39549d00e1835d48c82cd5d4b09b403cff162cd4bc7953154e3e15cf8cf0cce4ae8d928e799355d6" },
                { "sq", "470b31df37d8df182c6626ded01635537792b96cfdd89bf527e598b4d407de59e76c4a8d9ef7eb44aeb70df28cc6e127af797f56c7086c9dfafa7aa748a66bad" },
                { "sr", "ecbf1656353cf2f172960936a749250e2c17b67cf5550567b026a51abd76f75e7d8a599ee92e83ae92fe474e29730596a4714980d4fa224cf3f45061cfd48e70" },
                { "sv-SE", "5b05ce4caa0ca9da2872ac4e159e0aa26ea71771b3175e4ba0b98e8376603cd69c8eecaf4e02a99de5690e9cd1f2b9266bf0826da8500e8a6066ad52b27541d2" },
                { "szl", "f0a8ccb8d8917710c3c70eb9d68c385b8e5de793f07c9528876f900db376b150c17fdebbe26bc64c07633c387de8a51e4355cf52b763e167283b72ce7ae8d19c" },
                { "ta", "f97cfb432116038ed228449e2375fac7b7781345823cc265b2d24dde2a15888ef290805d1ac676a3c23c5c64ec3fafaba6886befe7352eb8e58046979562427d" },
                { "te", "8a2c937c45b50a6eb22ec889b8356195f58af2b75f5ca6e40f0f2b1bc6c055b47d84fcb6695046209de09ee775512b7cc1f8f06a5f9cba7efab0926c6a9fff91" },
                { "th", "87010714e20ea6ffa797a616849a15d9e5741e403b4a663eda94ffed7b8035244ae848ba13ec5e3c3623fd6abbf07afc40453d73407a7c033e3f160f74548bc2" },
                { "tl", "50de76df891556eb8a25cf20bdb2cce70f5b9c752f3f09b4f27e89f2162c56405876da9265fee0df03110dae356f4b4a89b506f975cf37c4fe2dfbcbb2c5091f" },
                { "tr", "28781672f37c0e173f621a41c360357e9b4f94a42ce56315f9ad74c826284270c2e194e313a94e2436663d54920726db00513d619365be067c79dbcb21218fee" },
                { "trs", "d41a3e9ca2870fc556b359a56f575a7ee344b03920d1f359127de3d9f9c6a2d2451a73b270649c4230db0382c3a32a62530e7fec02c43eafb1886ebb8d3247e9" },
                { "uk", "d4ce905bd49fe18dfa805a8eaa8aab422ae97abbea0eae8666d682edb869ae1bc25f411c4f9837c8bad3fd1361f88301a9a0f7ce3625dab71b13b9c268b2507d" },
                { "ur", "7c75396140a82a512f6bf15222c15d6973c6edf0bfa62f436e9e758a6448248fe0b02ed695be6b113a3fe62e5e453f347ad615774e4620817686103c540d4709" },
                { "uz", "03796cc61e495b1598ebd700f17fda1a22f360522042da568454c6ce9489cf8769cafdb8a042830852e6b52c8def220776c27dc76fc6267e8382c9acca64fb7a" },
                { "vi", "6c2e8ae517aa1e51da4323a2e2e8f0c9e7d9e342820d86bbc3fa4b3e4967a430105f925e58d3b461edb0b081737e24c5628cf172b6e208369c6eab1f5c2efd3a" },
                { "xh", "53c2827c78f90f8a3fe8aa329bb982e84f2ee3d36f563fe7e7ba61e58c6713d7dd3f5174a29a15177cd3840d96193a5f471765eca2a783fc1907d32876384150" },
                { "zh-CN", "b5e6136e85c79e2266991bd62b3269ce00c8f84665ff292d35af99f9000746ccdeed54677e825b46c24462a257b751b888e9078190e9ebc9841fe25ccea029a9" },
                { "zh-TW", "afc54cba63cf18503efca588e6d163400e4af0cebe2fac8c4e171ff159d59829e222acc9d3f33fe6bbae7c84d94e073250460d3f9be1736cfaf84ddbad0c94c6" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/108.0/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "e3c4f0718111156ff0d8e638acd31fc9652501a45231bd0a7ac131efa53a67fc1a558f2b34760043d2458d6eff4fada81726e432f5afe8ba1da28169243849ac" },
                { "af", "12b7be1f0beabf74707675c67ea2753b589c08c0872b3bfb29b738c99a11dd2d4c58b13719f54cb5af8c3d3eac1008fa64c6259df6035793008a735d7c7223e9" },
                { "an", "15e727171023d3d314bc82663b3b4e9f8a7d56c14cea3da313d01d07a70c57d881a9a756efde5f61af230033dc4e1680274f608a07a4fb607406652261cfb335" },
                { "ar", "e2390ba8eeec71f22f78e8cb495cd2ed2338efb86b662f6307eb667e6fdaa9c67f3c30f980e60e6fd8ac7a9b7838a953ae93c637df096b9090e3a37ee22caf64" },
                { "ast", "eb7883227f00a07a92f435183901f026d4ee1909820462e5fe5741f757183fa5d55f28998b2aba6d7813c2af2d3e6611253fa6f3567d1ba0fe4e7741b112020f" },
                { "az", "bed1b00c4f7f4629797bfd200696840fd648e312fa5092377075e16d434f1cdea8013b2b478c9a6478980e0a8b2ab3f392c773a155015477c606ee2629477ebb" },
                { "be", "e9eb3743c7a36ee34a7d65b10a100d0d03d1136de74c081bfb2ffeedc87f48ffa2ccc9ab9c20738184214db8e2b2f4373e6b093807e75ffd65e2c1810c056da2" },
                { "bg", "0ac9aa10d2d3726ce18814f28266bdc117e35199fb5f746a7a9def27ba6baa36a600e86d5b28d95b9575a033666bed77ad6b563c5bd5b6875548e3552a2cd7e3" },
                { "bn", "6862519cbedcb3b5a52b4aa66c9dfda8a440116e274360189d86cbb37950c0cad923e4a8c30d36e7b740a2856ddb9f7e55f15e6a72b70a5b7b45943ab6fe6ea3" },
                { "br", "c1b6e31b4c6c908268ddf935acaf0d4d099653c5c47feac0c28fc956b21f5f69e54d0a6bd57888cc748e15b4b3cb4c865138b1b871022694eb2074f1566e9c9a" },
                { "bs", "25dffcbd254c5756f351fdc411436cf439ec6b8e33f668d90b702f6f69b4c1de0fff7eb8e81ae76a1f492c5cbaca5b3d638287de569c0ca6737617df270c9c85" },
                { "ca", "2201ae195199cf170713c64f0b5340a55ed57123519d2f0c7698fdfdf192292dff5e33e644512ff32a49f4f020ffd7610972689e977660f411c73f21739a5de6" },
                { "cak", "1d25ffae50f323e803403a1fec2f50c9aefd985c86fddb2c0395086517ac50a63cdb2714d9a2a87fd1f2db38580310613c2bf1e71f10814e9ec5f11606069324" },
                { "cs", "3db3cb11e303f3cc3c528a66fe4d8637cacf9280e2f95c91f76923c73924a1fb5a4b2b0962d68df7ffcdb9e404ec74d0c210a15c44087731f6d1441272a9a135" },
                { "cy", "b20637085ba2e9f50f3352d42b2b50801b4fd4314217277917d003a78dc9489f638ae91681754303f650f52043dabf854dc270f454feed01a5f5d1aca6302d89" },
                { "da", "eddf90ae95af0f3c576b337f77cafafeff8ee136e17d36ccb1bf56e91a8b7ada007298468497675b780a50cb378bde36201d70b99ce3a4ffd752d8f00081df1c" },
                { "de", "d8708bf20c538d43cac9d96990cda1b8943c3df591e0eba935ea0b06223a0cee2bc4aff6fac816d71a3f80b8d7d1d1dcb4223ee44198c69eef62e7ac72150bec" },
                { "dsb", "3e252dc4c67328759468bfc598c40dedd368dab152ea22eb4cf9ad9b7eee19885932d0fbaea2c095fbceb31053ef779c2a93be6590256ee5ef92fb8602234eeb" },
                { "el", "2c44af46a0b9cfe53863bffda01b2854e8e08b29abc3fb063bb1c4f7532873a01eb1390ed29e46782bc16492c1b5a86c137aebb4d561f8b8f6858a844594792f" },
                { "en-CA", "7555321fea6cbe8f69860b123412600645b460e6c29d4726f5b100273551a930126d659ca3f9ff23004c334418792f248ee38c9c64a8810f2df6d7390d6aced7" },
                { "en-GB", "0299976901ebcdc4656547092a5a8e31cbd41f11e27bf3039b77ae651f71a3bf281eaca17d609c1e4117e10eaf5529622d3e3cac7245697dbecd7b1a510c382b" },
                { "en-US", "7bc36473b3b194f624a871e0e9fc16149d8adb3365718d7cb79066a60bd34b9ec715a6f077015f061befbea44abb28f8f40caceeee8eceb225e7573439ef542a" },
                { "eo", "30f4b97649062bf501e13d52cc077bc0198531d63e9de705b8da5d1364e74352bb433bac0a116bfeab72fcbd90a5d01242f8902d24b58f4682569f1c5f9d5dee" },
                { "es-AR", "2fb514929e1e63a9d5406eab3eb7fd57ec918f8619fb81919d8c4abb5e00ed336c71f55c6c9ccdf2a53cbb856a921ed3d5ddfcf81f3aea47d508146ab5c36eb9" },
                { "es-CL", "f28b899f56b729cc805d583422d4854d9cafd42717397c42442ae9973591b17c7aab1e2253ea47f735977ffe64c3ec86b1d5a1d7b76368dedd4ce68d3885bf32" },
                { "es-ES", "21a2a0f31fefdbc21428aabfaecb5099fde7164b2d29e94018c4bacde161edabe22a52c6992c27986221a7da3103159d15a93de5aca9ab9646404f879b40a4c0" },
                { "es-MX", "8f8f0669c3daab6e9ac2543f49d0421daf18afb2de2bc0807e3cd125428a82f6bf92e1b5f02a2c66f836160ecd933aeb2da6c23f2e8fbd2eb25e899886b440cb" },
                { "et", "66f36d26961eb9ae86ec1fc7cda76260768c83b26c38355f935a48498338adb7d9afdd1fad095fa4ef11af7fdfc5f56fff6f0738165052e90d5d163e6cefc345" },
                { "eu", "bbbabf54938fcc190c000955ddc1d559e747804ba38ec263f9fc2ad6c785574783c4f1df02c918919545e575343ccf1fbb75c11d0ffa4c779210ff0d2115e805" },
                { "fa", "fdf924ef9a0d55e35a00456fdfc4f648f613386ca438b2b828f64a85dc7bcf68058dec9a6ebd88cd15c59c4d3c2038608556419dc489099a5c075279f4457834" },
                { "ff", "445b19f510915ffd9bb4de26e878ed0f6322098fa2f4d49a3603bd359327d0636e8a0e9af80df647b69e20ee71777e1d0f3a15d62a0cf59d2c41e90e92142f6d" },
                { "fi", "59ee2aa26a2b49c270d410c86bc4e23bc2feef276d7f3b2cecadc2c5e6239bea122cc331527314a307861e5d0273924a2aa8baad2f3ddf0bb42582cda8d3723c" },
                { "fr", "455176f1c64e895a62c0d125db3c29a056d1fd3e9f4d7c449cdb11920c3095097988ecfb28b9f8bac223de2bd45f4ed8ae8ec96f39ccc89a17f64ef45fd74229" },
                { "fy-NL", "efded179a7727c9acf35c0a24a231f64d1e3486b155c7e2cb1b2a5197ab5ac9a5db53d052c2644ff7d55448ebb7fecd6084f7313fd1144403f1be55fedb4c88b" },
                { "ga-IE", "b0d3478c0ebc5b441fb649c219c44c1bfa75a4be4a8272b666d6410995595cb11630edc4280ea14a1b7311a19d57da01fdbf9c151e3e16c9a7037d994ed9d4af" },
                { "gd", "21d1c50380cd636eead5fdd5a761203d2a5a5d6371c59e46b4ab3529c0e45398687a01330ea07cb0d9d1217e6d664c2d45731e540138eba86457a32672ff0494" },
                { "gl", "5a0c689d8d52a8e6072b10b1e105adf38cc239a0e28cd10c5b16072982e6798630c06233f860deb277a8eacaeee59a86693435d08da55392bc5eedfaa1407697" },
                { "gn", "d0d79792ed481354921cb16e8e4cd56eddfa296aacadd498e0473d759b66ffb4e3842cae4861fe1fe20d6bdb188b94fce32431c4b57e900d70393863beecc617" },
                { "gu-IN", "40d067133f43c498a58fed93559dbfeb7239e6da2c170820eb6d1ea978a33dd4b23d8260a76a2c1b0e7d803ecdc7f662e4c968c2064a250de17fe03d5deefa2d" },
                { "he", "5171fbd5a69a87b80d3540d5b6f48a2dee938b228883d99a4e48f874f104b19c23c651e6f1c67b90b334c02baceef9cf3c7108255fd2e6bc46aacc8f30b61b72" },
                { "hi-IN", "b3a396e1ca0e82b29f68a46e6bdc962264fd079f6a3db8ef9f06b35fb6abcd70770a16e185c474a8a59b72e5f3869dcd7151f59c6666056759833c15e55d1fff" },
                { "hr", "78949c0cff02b33a4a27dda22f991dd4a2a2c58421d4d75e954feafe77dfe0c27525f653efea68737f7a0ee36dacd93792191238a9103eafe5fb15eff794b902" },
                { "hsb", "7d75deaec17d5f7be01d40cb3f97e50a44ab77ba61446ccef49e47ed04c577cc0fb59619e14c8ff1d00ea43859c52afdf2fda7faf10400ce42f7d99d7f5bd12e" },
                { "hu", "cc0d2175ed0ab4a89d277bbbf24f10df4ce17b5023b466e4070e9c0d68e25d81c8a5d4104dc07eee5e0be8d3cedacb59fca3035f43ab08b655875a679e8fe441" },
                { "hy-AM", "cca98abcffe04b5b70d9e1980a84297791d54a72f4d17e3093fc75e6c51ef33ad9bc59e8bcc25dc1793eddf30227f452bea7bbd5ff68957fe2f5d40fb131d704" },
                { "ia", "75d46d3bfe6f71e132ceaed1659347a4a41d44a94b45554ce038c4e84ad99ce7b0576e22828cb412c97f540146289841a8c8df415e3d27732b02b558d5df5c85" },
                { "id", "f239733e8cee62e1fb48750805c1c2eb4a65584446266f515dcf615a7676fa398517acfd645927199ad0421832844600c8c26b5561c2eb4b7b5fb9e364ec84c7" },
                { "is", "f9512814a5fbc9da30f6ce1f4b979c8b133ad1f764afd68291ba5dc5abc06639773fb5f46a2a4493dc95f14f83c1922bccbd28c83c2274918b47e3c3927e8bbc" },
                { "it", "1b8b8e8e8a8cf8e5bb04173709452d0359d388b53a63d60b23a852e42f800d9cced2c68b67a3b6c02fe21ceac755b4bce253e38358e4bcb25b34169989df4970" },
                { "ja", "17b43fede6c77fb6f569f086de029b866ca1911ce8253fecefd665d87e64a4c4245f57d0d3d508e9495160df97d2cd91692957bbe461f5adfaece1755db0f33b" },
                { "ka", "e55ea5ce0a908023345c11ce8f588b71924f34da49aaa9f3354667fb271905dc1fa18f9e83a28687bd33bcc18061291737e0f672d69c3dc95119e86cdc2c8677" },
                { "kab", "bfac5d3cfbb9972541fe2b4102d8f4aea2634d25edb4371b4ffd246c875ae18911249ce4108db1f834246481e60c17585d8d3c99eda11f60eb5915c3a05be750" },
                { "kk", "eb1a8baad46ec592dfba5fecf9a848f7fbd1abc43c2bfeeb69d08b1749a95802bde70e44d76b10c1edac509c2154763ab350bb31b22b526bef1adede30b6b2c8" },
                { "km", "7b7121ca614790105d29e21a69883ad0a7bb11b2897004a7e5dfa9d43489dae7d2d80eaea9c82963a118b6220ece5307d0f755e64606d0c81426160a63848ce3" },
                { "kn", "992d55ff6fd99796c03a771f032f6b4dd1631b89c656ae5ee0495d3a42ef44257706d7454e7c5625d5340fba449c6e63bc990855b9edfe672f312b2f59ae723b" },
                { "ko", "06413d3f93ba2680153b1ec7d75b2b1997a0d7f082cf5fe0e5cbaeab2a7536195a01e3a641a442ed4706422f509af4d11805b40339302ea9f49b598f4e7d3582" },
                { "lij", "f1438fae1452de7d70a3df6e40941458a8f0b71fc1bccd02a36f590c27de44f84e31b43e20d9dacfe234ba81af2d59ef0ff88936fba7b74efe9bca63bf42b137" },
                { "lt", "8f462e8ab7a3c1fe835d0c58fcbcc479c83794168635d59fb67c89096d19750344750f8d754a0ca8484054ed1799b4b0ec9d04a1bb041b6759a4388ce6fc2785" },
                { "lv", "fa773447719dbfc74d058080cc30334fc5178385fb724ae5d53380ee50953a94248da51b6ff93ce33e7a4f4d421c1dfb8fe51821825231b4ccdcd757f82bc8f0" },
                { "mk", "3ce90aa83351891180fb3e130dcfd92661c56e3369059da4a9cf7719fca0540bcb0f71745fc52660ef17ac79b6b3bd352c6378ff09c0f65d4614edf4369b6ac2" },
                { "mr", "0c122f0d7cd25c99621717c1bd0275507adce6c00fa6ed8d3ddb5ac5432a9b606a5b7be287905302b7250655b8aa715f40a79f42a8af054d69ead47de5335a4f" },
                { "ms", "eb966a46fd5df67a2f216946c73ee2a35d0baff949529a330445dbc5cae3504dccde4f9d9e35909c311dc522028610c399fa06240df7ea43a494e6b81b825d26" },
                { "my", "2e907cbb215d29662ccc7aaf0d120eff2677e29f8e1f7e688c3a56727b4577c0a1562c0ead6f4b8ad96304af3d9de7b8233c3874b6321a06f8628a56787ac35c" },
                { "nb-NO", "cd9f137c0ef01c0d9f5d21cba20a2f55ecbd7f78ad0eeef841c8625e427f24f4aa792f8c158e09291342d71d94bdad24b3864b7231df7398abf0ec43dea69fa8" },
                { "ne-NP", "fb34e7371fe06e5989519c93482fff6dd22a233c313571f70d0550bbba817c03db6a552b589fa960a741e9a48c4762a03136db538c2cec091790c659ec7587d6" },
                { "nl", "40590a76bf5fc2e2ed89cfd2fdc2caa591400a93861f4ba14bf4a988206208c1a390e7271e3f880bac1d6903032400ca941b56b4a25f732fcc95aec8aba6ffee" },
                { "nn-NO", "9e555014d968348e8b7559e951ee5cba95f7f79212b81b0e506483e98b43032df89342f676e5dc7b7f23a87a8ca847be204190725e33190a91f449c32c826721" },
                { "oc", "9ec6b9efe468117d215b37ca22191be2e13b016f98b4416e7dc634691a0b7f5268db8ccc58d86bf08a4be980c4d9b7a01d8c919c08cc914601fd869cc3e83f95" },
                { "pa-IN", "c47b4d71c4a624817d584a818082d9a5cef18ac95dcb8b85dea5cc51d3f88cb6a80eab73de97e41260fc860985e37fb10ae963d62921e312885fdcb168d3ec02" },
                { "pl", "fa2d9511da3ab3a7cd8a879f88634090390e7eaf6e6d527a7a1d23a905d53386e5c88401e774601a2208a359884c4d0f51f7dd5c1859f2052fc0bceaa16dd88e" },
                { "pt-BR", "beb2448b45a73fc9cac5e1a24142524d62c7550d9a3d2ee7c0d53e820bc5c9b66251813e82598ae8ffa120dec34f677e1c82ac325da55caf6d55e578e0d43222" },
                { "pt-PT", "5ac2a7c82497d29f3b6cfaaacbcad4082e2b0968dcf94f4cc748e12d99a4d59178203776329f6fa54dd24494db14c961f29ac343538092467d8a2a67a3fdc628" },
                { "rm", "5e364b7c542e5fd8ee2e382fe1d9f96bd27ad632bfa880c889b835f2b8e810420f74422de8850ea76d258ae830a0fc06e3cab5189469d31c5526feaa6451a6d5" },
                { "ro", "92678535233f416530cdd9463d53804ffa0864b40c7edd278077d779ef75043ce0aa0765b9d167bc48fcff986c73fed1bcff7586ce49b2c5f5c199f9cb2b64a4" },
                { "ru", "d1203011d2911bf4876ef23310dc2c03c5522b49d7afeed244603c5d7755a78010b7701a501ad180b52f203fc866c8f9145e76f7cdc46828354e6abe39f10d37" },
                { "sco", "efec3beca0c00855ef92d384cd33d05b8834ac6e8a74ee541d65c04b15d101f43bc02e4f40d091e20b3f3f9da1da4d921d9654f4b636434f48646dc4fa2ac02e" },
                { "si", "929fbe771aecad219f34a234342b7e02ef038a5a418be837ba0199a21b78dff756894dc1db556a72c508ac218d7341f5d6302b1f7c5d45f981849e8a20db4387" },
                { "sk", "cc92ce2e034803a4e5a80179e1a914db6cc2dcd0b4d72b99eda3f0eaf7f8d136024a92912fdab79178397e8e2b599d5c83072f0f4dda21625c908c240938513a" },
                { "sl", "9f9a04bd39c690c4e4d25c700ef125a83502bd92b6354b8b663c07ceb019f0b6684dbdb18995b015158d047f505c32be4451d851d0295d0c33c2031e87539064" },
                { "son", "e0aeb6e8f9aeb49374f592f8725a8a6e7d1da8caa3bdbba953a1a02fb3351eb541f98b09d2395891a98e940c20f0d0fefc82846db3381a3c860cbf46ba9ff189" },
                { "sq", "b23540e03777259b460d521bc6bba2c0dfa89889a6971c8a740a5341dcb23702cef32cbb5de70607ef90e94ce8fd3caadd8ebc8b733bb2ffd0e74d8806c99f7f" },
                { "sr", "1e083241b04a30fb855ab6278cb724ee814224c9a015101ac6aab42cb8bfb88dcc9d022be055f9a8cad011eca14192bf98b2ebc13054427b37c17083c00b3844" },
                { "sv-SE", "cf00e7dc15f8600e8ce03e7e4a5358284b822497f75a05bd9ebab75a27b9bd2d786cd707b1ccd3e91336d620788b0e246dc4c0c47cd0b07d2c1ed40964588b32" },
                { "szl", "d8836729794b51aab33e102a20890168fa7a5787069d4eeaa9d44cb23561e69f69f7f4166806d2768829d000e7e50ac649aefe70dd0ad289570391624c3029ed" },
                { "ta", "899c435d70f03c1c2b7933bad86f7c30d3b19c1517f39ed814cf4a561652dd3415f0ec59ecf40e71d756926ad921c39f0db0af77c2ae168cc4945aa004dbf37a" },
                { "te", "ee0234016144f92d7e188dc56029433655ffc8d6bfa21dd5d11ed3e05af9adf6f0cbf244f9e54a31d8f110ed3daebe1ee27c6ac532d42ac53b92e305045786f2" },
                { "th", "0ce7caf598d28af27e8b835a1c7325942cf0abd307f128fa50a5399832504ab4a6d78522f931cc8cf8bde955215d04663024c6bc87511ba8efd30513c923090c" },
                { "tl", "5011be4161d83f963d1e347a3feca0d1572c489234139bd69485a300987847bddade85731a510940fa620912e1078d9a1af95f4a49890a7900c25cf94dac453d" },
                { "tr", "22c58e49c4d8e1b9f3f97b710e7e1227e61dfb046be896a9f053995d1e622d25401711d76c1e368fe4912db3b9aac42e2ec04b47b944fe09645d1eb72720253a" },
                { "trs", "531fa457b3be17a26c8c0d4eccd1221307d045240d93dfdc120ea967feeb8f2f5189506cdceee756847d4658bd1c8b27dc43d15ba529ad8960c6eb3bd9d5204a" },
                { "uk", "7a8620560f557ad75c6402c4b41876ed31acac33b09ad5894db14b6aaa3ac565efe6f9b906ebcfb6269b29b4e7fb68abb06bab9cbf5512d2123aa892df31ca63" },
                { "ur", "0025f5b398b78fc096e1cd3086c6a7312453992e10acef865c3c7255a0eba7d4fe8d9c3e6591307e351b95af0f335ca8eb22f77fbf1aa2d256aae0ec7d1bc484" },
                { "uz", "61b00540801954e38a530d5eb3b1e6f90d4dfa3e70c98ddf47c40fedee0a0c172b853d0d6096339c2c5e0dfcfa377bce2e40bf062c1226a1b3e501ac5e463145" },
                { "vi", "c2f5fad23db4f462d87aced5383da6dead14ee2e2b55dbd869f2a5526aef620947510e30549c5e788b7f4f8c8723e355283513e0f1214a2db86d6415d72cb838" },
                { "xh", "84c8315034d2b0b121176f31a3be0061d2e46a8bfc2f14c70e2ac318029b8d3bf905647a286df41ba1cdbc6526171d7fbfeb3a7136f226290ff2323e6aee774e" },
                { "zh-CN", "5bbd015dec4eafb75411b03d9733d1afe19eb089cc0749e80661b2925faa22fe9c0ebf73e866cd9e674cc2e15efd927a73c5c11b62f156caf46a046340ed09af" },
                { "zh-TW", "fdeab392415ff81805aecb360623df2297a80b8b1d5d193961ff4cc7252095ce00dfd9f8630027f6cda77a14b767b1ca9ef3921e4888711c326dad8e18c4c502" }
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
            const string knownVersion = "108.0";
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
